using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using System.Text.RegularExpressions;
using Observatory.Framework;
using Observatory.Framework.Interfaces;
using System.Linq;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Avalonia.Media;
using Avalonia.Controls.ApplicationLifetimes;
using System.Runtime.InteropServices;
using System.IO;

namespace Observatory.UI.Views
{
    public class BasicUIView : UserControl
    {
        private DataGrid dataGrid;
        private NativeNotification.NativePopup nativePopup;

        public BasicUIView()
        {
            InitializeComponent();
            nativePopup = new();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public static readonly DirectProperty<BasicUIView, PluginUI.UIType> UITypeProperty =
            AvaloniaProperty.RegisterDirect<BasicUIView, PluginUI.UIType>(
                nameof(UIType),
                o => o.UIType,
                (o, v) => o.UIType = v,
                PluginUI.UIType.None,
                BindingMode.OneWay
            );

        public PluginUI.UIType UIType 
        { 
            get
            {
                return _uitype;
            }
            set
            {
                _uitype = value;
                UITypeChange();
            }
        }

        private PluginUI.UIType _uitype;
        

        private void ColumnGeneration(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.Header = SplitCamelCase(e.PropertyName);
            e.Column.CanUserReorder = true;
            e.Column.CanUserResize = true;
            e.Column.CanUserSort = true;
        }
        private void UITypeChange()
        {
            var uiPanel = this.Find<Panel>("UIPanel");
            dataGrid = null;

            switch (UIType)
            {
                case PluginUI.UIType.None:
                    break;
                case PluginUI.UIType.Basic:
                    dataGrid = new()
                    {
                        [!DataGrid.ItemsProperty] = new Binding("BasicUIGrid"),
                        SelectionMode = DataGridSelectionMode.Extended,
                        GridLinesVisibility = DataGridGridLinesVisibility.Vertical,
                        AutoGenerateColumns = true,
                        IsReadOnly = true,
                    };
                    dataGrid.AutoGeneratingColumn += ColumnGeneration;
                    dataGrid.DataContextChanged += OnDataContextSet;
                    uiPanel.Children.Clear();
                    uiPanel.Children.Add(dataGrid);
                    break;
                case PluginUI.UIType.Avalonia:
                    //TODO: Implement plugins with full Avalonia UI.
                    throw new NotImplementedException();
                case PluginUI.UIType.Core:
                    uiPanel.Children.Clear();
                    ScrollViewer scrollViewer = new();
                    scrollViewer.Content = GenerateCoreUI();
                    uiPanel.Children.Add(scrollViewer);
                    break;
                default:
                    break;
            }
        }

        private void OnDataContextSet(object sender, EventArgs e)
        {
            if (UIType != PluginUI.UIType.Basic || !(sender is DataGrid)) return;
            dataGrid = (DataGrid)sender;
            if (dataGrid.DataContext != null)
            {
                var dataContext = ((ViewModels.BasicUIViewModel)dataGrid.DataContext).BasicUIGrid;
                dataContext.CollectionChanged += ScrollToLast;
                
            }
        }

        private void ScrollToLast(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Only trigger on adds.
            if (e.Action != System.Collections.Specialized.NotifyCollectionChangedAction.Add || UIType != PluginUI.UIType.Basic || dataGrid == null || !(sender is ObservableCollection<object>))
                return;
            var dataContext = (ObservableCollection<object>)sender;
            dataGrid.ScrollIntoView(dataContext[dataContext.Count - 1], null);
        }

        private Grid GenerateCoreUI()
        {
            Grid corePanel = new();

            ColumnDefinitions columns = new()
            {
                new ColumnDefinition() { Width = new GridLength(0, GridUnitType.Auto) },
                new ColumnDefinition() { Width = new GridLength(300) },
                new ColumnDefinition() { Width = new GridLength(0, GridUnitType.Auto) }
            };
            corePanel.ColumnDefinitions = columns;

            SettingGridManager gridManager = new(corePanel);

            var pluginManager = PluginManagement.PluginManager.GetInstance;

            #region Native Settings

            #region Plugin List
            DataGrid pluginList = new() { Margin = new Thickness(0, 20, 0, 0) };

            pluginList.Columns.Add(new DataGridTextColumn()
            {
                Header = "Plugin",
                Binding = new Binding("Name")
            });

            pluginList.Columns.Add(new DataGridTextColumn()
            {
                Header = "Types",
                Binding = new Binding("TypesString")
            });

            pluginList.Columns.Add(new DataGridTextColumn()
            {
                Header = "Version",
                Binding = new Binding("Version")
            });

            pluginList.Columns.Add(new DataGridTextColumn()
            {
                Header = "Status",
                Binding = new Binding("Status")
            });

            Dictionary<IObservatoryPlugin, PluginView> uniquePlugins = new();
            foreach (var (plugin, signed) in pluginManager.workerPlugins)
            {
                if (!uniquePlugins.ContainsKey(plugin))
                {
                    uniquePlugins.Add(plugin,
                        new PluginView() { Name = plugin.Name, Types = new() { "Worker" }, Version = plugin.Version, Status = GetStatusText(signed) });
                }
            }

            foreach (var (plugin, signed) in pluginManager.notifyPlugins)
            {
                if (!uniquePlugins.ContainsKey(plugin))
                {
                    uniquePlugins.Add(plugin,
                        new PluginView() { Name = plugin.Name, Types = new() { "Notifier" }, Version = plugin.Version, Status = GetStatusText(signed) });
                }
                else
                {
                    uniquePlugins[plugin].Types.Add("Notifier");
                }
            }

            pluginList.Items = uniquePlugins.Values;
            gridManager.AddSetting(pluginList);

            Button pluginFolderButton = new()
            {
                Content = "Open Plugin Folder",
                Height = 30,
                Width = 150,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            };

            pluginFolderButton.Click += (object sender, RoutedEventArgs e) =>
            {
                string pluginDir = AppDomain.CurrentDomain.BaseDirectory + "plugins";
                
                if (!Directory.Exists(pluginDir))
                {
                    Directory.CreateDirectory(pluginDir);
                }

                var fileExplorerInfo = new System.Diagnostics.ProcessStartInfo() { FileName = pluginDir, UseShellExecute = true };
                System.Diagnostics.Process.Start(fileExplorerInfo);
            };

            gridManager.AddSetting(pluginFolderButton);

            #endregion

            #region Popup Notification settings

            Expander notificationExpander = new()
            {
                Header = "Popup Notifications",
                DataContext = Properties.Core.Default,
                Margin = new Thickness(0, 0)
            };

            Grid notificationGrid = new() { Margin = new Thickness(10, 10) };

            notificationGrid.ColumnDefinitions = new()
            {
                new ColumnDefinition() { Width = new GridLength(0, GridUnitType.Star) },
                new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) },
                new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) }
            };

            notificationGrid.RowDefinitions = new();

            SettingGridManager notificationGridManager = new(notificationGrid);

            TextBlock nativeNotifyLabel = new() { Text = "Enabled" };

            CheckBox nativeNotifyCheckbox = new() { IsChecked = Properties.Core.Default.NativeNotify, Content = nativeNotifyLabel };

            nativeNotifyCheckbox.Checked += (object sender, RoutedEventArgs e) =>
            {
                Properties.Core.Default.NativeNotify = true;
                Properties.Core.Default.Save();
            };

            nativeNotifyCheckbox.Unchecked += (object sender, RoutedEventArgs e) =>
            {
                Properties.Core.Default.NativeNotify = false;
                Properties.Core.Default.Save();
            };
            
            Button notifyTestButton = new()
            { 
                Content = "Test",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right
            };

            notifyTestButton.Click += (object sender, RoutedEventArgs e) =>
            {
                Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    var notificationArgs = new NotificationArgs()
                    {
                        Title = "Test Notification",
                        Detail = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Cras suscipit hendrerit libero ac scelerisque."
                    };
                    
                    nativePopup.InvokeNativeNotification(notificationArgs);
                });
            };

            TextBlock notifyFontLabel = new() 
            { 
                Text = "Font: ",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };
            ComboBox notifyFontDropDown = new()
            {
                MinWidth = 200
            };

            notifyFontDropDown.Items = new System.Drawing.Text.InstalledFontCollection().Families.Select(font => font.Name);

            if (Properties.Core.Default.NativeNotifyFont.Length > 0)
            {
                notifyFontDropDown.SelectedItem = Properties.Core.Default.NativeNotifyFont;
            }

            notifyFontDropDown.SelectionChanged += (object sender, SelectionChangedEventArgs e) =>
            {
                var comboBox = (ComboBox)sender;
                Properties.Core.Default.NativeNotifyFont = comboBox.SelectedItem.ToString();
                Properties.Core.Default.Save();
            };
            
            TextBlock monitorLabel = new()
            {
                Text = "Display: ",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };
            ComboBox monitorDropDown = new()
            { 
                MinWidth = 200
            };

            List<string> displays = new();
            displays.Add("Primary");

            var application = (IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime;
            var screens = application.MainWindow.Screens.All;
            
            if (screens.Count > 1)
                for (int i = 0; i < screens.Count; i++)
                {
                    displays.Add((i + 1).ToString());
                }

            monitorDropDown.Items = displays;

            if (Properties.Core.Default.NativeNotifyScreen == -1)
            {
                monitorDropDown.SelectedItem = "Primary";
            }
            else
            {
                monitorDropDown.SelectedItem = (Properties.Core.Default.NativeNotifyScreen).ToString();
            }

            monitorDropDown.SelectionChanged += (object sender, SelectionChangedEventArgs e) =>
            {
                
                var comboBox = (ComboBox)sender;
                string selectedItem = comboBox.SelectedItem.ToString();
                int selectedScreen = selectedItem == "Primary" ? -1 : Int32.Parse(selectedItem);
                
                Properties.Core.Default.NativeNotifyScreen = selectedScreen;
                Properties.Core.Default.Save();
            };

            TextBlock cornerLabel = new()
            {
                Text = "Corner: ",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };
            ComboBox cornerDropDown = new()
            {
                MinWidth = 200
            };

            List<string> corners = new()
            {
                "Bottom-Right",
                "Bottom-Left",
                "Top-Right",
                "Top-Left"
            };

            cornerDropDown.Items = corners;

            cornerDropDown.SelectedItem = corners[Properties.Core.Default.NativeNotifyCorner];

            cornerDropDown.SelectionChanged += (object sender, SelectionChangedEventArgs e) =>
            {
                var comboBox = (ComboBox)sender;
                Properties.Core.Default.NativeNotifyCorner = comboBox.SelectedIndex;
                Properties.Core.Default.Save();
            };

            TextBlock colourLabel = new()
            {
                Text = "Colour: ",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };

            BrushConverter brushConverter = new();

            Egorozh.ColorPicker.Dialog.ColorPickerButton colourPickerButton = new()
            {
                Width = 25,
                Height = 25,
                Color = Color.FromUInt32(Properties.Core.Default.NativeNotifyColour),
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left
                
            };

            colourPickerButton.PropertyChanged += (object sender, AvaloniaPropertyChangedEventArgs e) =>
            {
                if (e.Property.Name == "Color")
                {
                    Properties.Core.Default.NativeNotifyColour = ((Color)e.NewValue).ToUint32();
                    Properties.Core.Default.Save();
                }
            };

            TextBlock scaleLabel = new()
            {
                Text = "Scale (%): ",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };

            NumericUpDown scaleSpinner = new() 
            { 
                Value = Properties.Core.Default.NativeNotifyScale, 
                AllowSpin = true,
                Minimum = 1,
                Maximum = 1000,
                Increment = 1,
                Width = 200,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left
            };

            scaleSpinner.ValueChanged += (object sender, NumericUpDownValueChangedEventArgs e) =>
            {
                Properties.Core.Default.NativeNotifyScale = Convert.ToInt32(e.NewValue);
                Properties.Core.Default.Save();
            };

            TextBlock timeoutLabel = new()
            {
                Text = "Duration (ms): ",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };

            NumericUpDown timeoutSpinner = new()
            {
                Value = Properties.Core.Default.NativeNotifyTimeout,
                AllowSpin = true,
                Minimum = 1,
                Maximum = 3600000,
                Increment = 1,
                Width = 200,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left
            };

            timeoutSpinner.ValueChanged += (object sender, NumericUpDownValueChangedEventArgs e) =>
            {
                Properties.Core.Default.NativeNotifyTimeout = Convert.ToInt32(e.NewValue);
                Properties.Core.Default.Save();
            };

            notificationGridManager.AddSettingWithLabel(monitorLabel, monitorDropDown);
            notificationGridManager.AddSettingWithLabel(cornerLabel, cornerDropDown);
            notificationGridManager.AddSettingWithLabel(notifyFontLabel, notifyFontDropDown);
            notificationGridManager.AddSettingWithLabel(scaleLabel, scaleSpinner);
            notificationGridManager.AddSettingWithLabel(timeoutLabel, timeoutSpinner);
            notificationGridManager.AddSettingWithLabel(colourLabel, colourPickerButton);
            notificationGridManager.AddSettingSameLine(notifyTestButton);
            notificationGridManager.AddSetting(nativeNotifyCheckbox);

            notificationExpander.Content = notificationGrid;
            
            gridManager.AddSetting(notificationExpander);

            #endregion

            #region Voice Notification Settings

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {

                Expander voiceExpander = new()
                {
                    Header = "Voice Notifications",
                    DataContext = Properties.Core.Default,
                    Margin = new Thickness(0, 0)
                };

                Grid voiceGrid = new() { Margin = new Thickness(10, 10) };
                SettingGridManager voiceGridManager = new(voiceGrid);

                voiceGrid.ColumnDefinitions = new()
                {
                    new ColumnDefinition() { Width = new GridLength(0, GridUnitType.Star) },
                    new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) },
                    new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) }
                };

                TextBlock voiceLabel = new() { Text = "Enabled" };

                CheckBox voiceCheckbox = new() { IsChecked = Properties.Core.Default.VoiceNotify, Content = voiceLabel };

                voiceCheckbox.Checked += (object sender, RoutedEventArgs e) =>
                {
                    Properties.Core.Default.VoiceNotify = true;
                    Properties.Core.Default.Save();
                };

                voiceCheckbox.Unchecked += (object sender, RoutedEventArgs e) =>
                {
                    Properties.Core.Default.VoiceNotify = false;
                    Properties.Core.Default.Save();
                };

                Button voiceTestButton = new()
                {
                    Content = "Test",
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right
                };

                voiceTestButton.Click += (object sender, RoutedEventArgs e) =>
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && Properties.Core.Default.VoiceSelected.Length > 0)
                    {
                        List<string> harvardSentences = new() 
                        {
                            "Oak is strong and also gives shade.",
                            "Cats and dogs each hate the other.",
                            "The pipe began to rust while new.",
                            "Open the crate but don't break the glass.",
                            "Add the sum to the product of these three.",
                            "Thieves who rob friends deserve jail.",
                            "The ripe taste of cheese improves with age.",
                            "Act on these orders with great speed.",
                            "The hog crawled under the high fence.",
                            "Move the vat over the hot fire."
                        };

                        NotificationArgs args = new()
                        {
                            Title = "Speech Synthesis Test",
                            TitleSsml = "<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"en-US\"><voice name=\"\">Speech Synthesis Test</voice></speak>",
                            Detail = harvardSentences.OrderBy(s => new Random().NextDouble()).First()
                        };
                        
                        new NativeNotification.NativeVoice().EnqueueAndAnnounce(args);

                    }
                };

                TextBlock voiceSelectionLabel = new()
                {
                    Text = "Voice: ",
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                };
                ComboBox voiceSelectionDropDown = new()
                {
                    MinWidth = 200
                };

                var voices = new System.Speech.Synthesis.SpeechSynthesizer().GetInstalledVoices();
                voiceSelectionDropDown.Items = voices.Select(v => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? v.VoiceInfo.Name : string.Empty);

                if (Properties.Core.Default.VoiceSelected.Length > 0)
                {
                    voiceSelectionDropDown.SelectedItem = Properties.Core.Default.VoiceSelected;
                }

                voiceSelectionDropDown.SelectionChanged += (object sender, SelectionChangedEventArgs e) =>
                {
                    var comboBox = (ComboBox)sender;
                    Properties.Core.Default.VoiceSelected = comboBox.SelectedItem.ToString();
                    Properties.Core.Default.Save();
                };

                TextBlock voiceVolumeLabel = new()
                {
                    Text = "Volume: ",
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                };

                Slider voiceVolume = new()
                {
                    Value = Properties.Core.Default.VoiceVolume,
                    Height = 40,
                    Width = 300,
                    Minimum = 0,
                    Maximum = 100,
                    Padding = new Thickness(0,0,0,20),
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                };
                
                voiceVolume.PropertyChanged += (object sender, AvaloniaPropertyChangedEventArgs e) =>
                {
                    if (e.Property == Slider.ValueProperty)
                    {
                        Properties.Core.Default.VoiceVolume = Convert.ToInt32(e.NewValue);
                        Properties.Core.Default.Save();
                    }
                };

                TextBlock voiceRateLabel = new()
                {
                    Text = "Speed: ",
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                };

                Slider voiceRate = new()
                {
                    Value = Properties.Core.Default.VoiceRate,
                    Height = 40,
                    Width = 300,
                    Minimum = -10,
                    Maximum = 10,
                    Padding = new Thickness(0, 0, 0, 20),
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                };

                voiceRate.PropertyChanged += (object sender, AvaloniaPropertyChangedEventArgs e) =>
                {
                    if (e.Property == Slider.ValueProperty)
                    {
                        Properties.Core.Default.VoiceRate = Convert.ToInt32(e.NewValue);
                        Properties.Core.Default.Save();
                    }
                };

                voiceGridManager.AddSettingWithLabel(voiceVolumeLabel, voiceVolume);
                voiceGridManager.AddSettingWithLabel(voiceRateLabel, voiceRate);
                voiceGridManager.AddSettingWithLabel(voiceSelectionLabel, voiceSelectionDropDown);
                voiceGridManager.AddSetting(voiceCheckbox);
                voiceGridManager.AddSettingSameLine(voiceTestButton);
                
                voiceExpander.Content = voiceGrid;

                gridManager.AddSetting(voiceExpander);


            }
            #endregion

            #region System Context Priming setting

            TextBlock primeSystemContextLabel = new() { Text = "Try re-load current system information when starting monitor" };
            CheckBox primeSystemContexCheckbox = new() { IsChecked = Properties.Core.Default.TryPrimeSystemContextOnStartMonitor, Content = primeSystemContextLabel };

            primeSystemContexCheckbox.Checked += (object sender, RoutedEventArgs e) =>
            {
                Properties.Core.Default.TryPrimeSystemContextOnStartMonitor = true;
                Properties.Core.Default.Save();
            };

            primeSystemContexCheckbox.Unchecked += (object sender, RoutedEventArgs e) =>
            {
                Properties.Core.Default.TryPrimeSystemContextOnStartMonitor = false;
                Properties.Core.Default.Save();
            };

            

            #endregion

            #endregion

            #region Journal Location
            TextBlock journalPathLabel = new()
            {
                Text = "Journal Path: ",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };

            TextBox journalPath = new()
            {
                Text = Properties.Core.Default.JournalFolder
            };

            Button journalBrowse = new()
            {
                Content = "Browse",
                Height = 30,
                Width = 100,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center
            };
            
            journalBrowse.Click += (object source, RoutedEventArgs e) => 
            {
                OpenFolderDialog openFolderDialog = new()
                {
                    Directory = journalPath.Text
                };
                var browseTask = openFolderDialog.ShowAsync((Window)((Button)source).GetVisualRoot());
                browseTask.ContinueWith((task) => 
                {
                    string path = task.Result;
                    if (path != string.Empty)
                    {
                        Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => { journalPath.Text = path; });
                        Properties.Core.Default.JournalFolder = path;
                        Properties.Core.Default.Save();
                    }
                });
            };

            journalPath.LostFocus += (object sender, RoutedEventArgs e) =>
            {
                if (System.IO.Directory.Exists(journalPath.Text))
                {
                    Properties.Core.Default.JournalFolder = journalPath.Text;
                    Properties.Core.Default.Save();
                }
            };


            #endregion

            

            #region Plugin Settings

            foreach(var plugin in uniquePlugins.Keys)
            {
                GeneratePluginSettingUI(corePanel, plugin);
            }

            #endregion

            gridManager.AddSetting(primeSystemContexCheckbox);
            gridManager.AddSettingWithLabel(journalPathLabel, journalPath);
            gridManager.AddSetting(journalBrowse);

            return corePanel;
        }

        private void GeneratePluginSettingUI(Grid gridPanel, IObservatoryPlugin plugin)
        {
            var displayedSettings = PluginManagement.PluginManager.GetSettingDisplayNames(plugin.Settings);

            if (displayedSettings.Count > 0)
            {
                Expander expander = new()
                {
                    Header = plugin.Name,
                    DataContext = plugin.Settings,
                    Margin = new Thickness(0, 0)
                };

                Grid settingsGrid = new() { Margin = new Thickness(10,10) };
                ColumnDefinitions settingColumns = new()
                {
                    new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) },
                    new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) },
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) }
                };
                settingsGrid.ColumnDefinitions = settingColumns;
                expander.Content = settingsGrid;

                int nextRow = gridPanel.RowDefinitions.Count;
                gridPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0, GridUnitType.Auto) });
                gridPanel.AddControl(expander, nextRow, 0, 3);

                var nonIgnoredSettings = displayedSettings.Where(s => !Attribute.IsDefined(s.Key, typeof(SettingIgnore)));

                foreach (var setting in nonIgnoredSettings)
                {
                    if (setting.Key.PropertyType != typeof(bool) || settingsGrid.Children.Count % 2 == 0)
                    {
                        settingsGrid.RowDefinitions.Add(new RowDefinition()
                        {
                            Height = new GridLength(setting.Key.PropertyType != typeof(bool) ? 40 : 25),
                        });
                    }

                    TextBlock label = new() { Text = setting.Value, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center };

                    switch (setting.Key.GetValue(plugin.Settings))
                    {
                        case bool boolSetting:
                            CheckBox checkBox = new() { IsChecked = boolSetting, Content = label };

                            checkBox.Checked += (object sender, RoutedEventArgs e) =>
                            {
                                setting.Key.SetValue(plugin.Settings, true);
                                PluginManagement.PluginManager.GetInstance.SaveSettings(plugin, plugin.Settings);
                            };

                            checkBox.Unchecked += (object sender, RoutedEventArgs e) =>
                            {
                                setting.Key.SetValue(plugin.Settings, false);
                                PluginManagement.PluginManager.GetInstance.SaveSettings(plugin, plugin.Settings);
                            };

                            settingsGrid.AddControl(checkBox, settingsGrid.RowDefinitions.Count - 1, settingsGrid.Children.Count % 2 == 0 ? 0 : 1);

                            break;
                        case string stringSetting:
                            TextBox textBox = new() { Text = stringSetting };
                            settingsGrid.AddControl(label, settingsGrid.RowDefinitions.Count - 1, 0);
                            settingsGrid.AddControl(textBox, settingsGrid.RowDefinitions.Count - 1, 1);

                            textBox.LostFocus += (object sender, RoutedEventArgs e) =>
                            {
                                setting.Key.SetValue(plugin.Settings, ((TextBox)sender).Text);
                                PluginManagement.PluginManager.GetInstance.SaveSettings(plugin, plugin.Settings);
                            };

                            break;
                        case int intSetting:
                            // We have two options for integer values:
                            // 1) A slider (explicit by way of the SettingIntegerUseSlider attribute and bounded to 0..100 by default)
                            // 2) A numeric up/down (default otherwise, and is unbounded by default).
                            // Bounds for both can be set via the SettingNumericBounds attribute, only the up/down uses Increment.
                            Control intControl;
                            SettingNumericBounds bounds = (SettingNumericBounds)System.Attribute.GetCustomAttribute(setting.Key, typeof(SettingNumericBounds));
                            if (System.Attribute.IsDefined(setting.Key, typeof(SettingNumericUseSlider)))
                            {
                                // TODO: Suss the contents of this block into a function to share with non-integral numeric types as well?
                                Slider slider = new()
                                {
                                    Value = intSetting,
                                    Height = 40,
                                    Width = 300,
                                };
                                if (bounds != null)
                                {
                                    slider.Minimum = bounds.Minimum;
                                    slider.Maximum = bounds.Maximum;
                                };
                                slider.PropertyChanged += (object sender, AvaloniaPropertyChangedEventArgs e) =>
                                {
                                    if (e.Property == Slider.ValueProperty)
                                    {
                                        setting.Key.SetValue(plugin.Settings, Convert.ToInt32(e.NewValue));
                                        PluginManagement.PluginManager.GetInstance.SaveSettings(plugin, plugin.Settings);
                                    }
                                };
                                intControl = slider;
                            }
                            else // Use a Numeric Up/Down
                            {
                                NumericUpDown numericUpDown = new() { Value = intSetting, AllowSpin = true };
                                if (bounds != null)
                                {
                                    numericUpDown.Minimum = bounds.Minimum;
                                    numericUpDown.Maximum = bounds.Maximum;
                                    numericUpDown.Increment = bounds.Increment;
                                }
                                numericUpDown.ValueChanged += (object sender, NumericUpDownValueChangedEventArgs e) =>
                                {
                                    setting.Key.SetValue(plugin.Settings, Convert.ToInt32(e.NewValue));
                                    PluginManagement.PluginManager.GetInstance.SaveSettings(plugin, plugin.Settings);
                                };
                                intControl = numericUpDown;
                            }

                            settingsGrid.AddControl(label, settingsGrid.RowDefinitions.Count - 1, 0);
                            settingsGrid.AddControl(intControl, settingsGrid.RowDefinitions.Count - 1, 1);
                            break;
                        case FileInfo fileSetting:
                            label.Text += ": ";

                            TextBox settingPath = new()
                            {
                                Text = fileSetting.FullName,
                                Width = 300,
                                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right
                            };

                            Button settingBrowse = new()
                            {
                                Content = "Browse",
                                Height = 30,
                                Width = 100,
                                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left,
                                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center
                            };

                            settingBrowse.Click += (object source, RoutedEventArgs e) =>
                            {
                                OpenFileDialog openFileDialog = new()
                                {
                                    Directory = fileSetting.DirectoryName,
                                    AllowMultiple = false
                                };
                                var browseTask = openFileDialog.ShowAsync((Window)((Button)source).GetVisualRoot());
                                browseTask.ContinueWith((task) => 
                                {
                                    if (task.Result.Count() > 0)
                                    {
                                        string path = browseTask.Result[0];
                                        Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => { settingPath.Text = path; });
                                        setting.Key.SetValue(plugin.Settings, new FileInfo(path));
                                        PluginManagement.PluginManager.GetInstance.SaveSettings(plugin, plugin.Settings);

                                    }
                                });
                                
                            };

                            settingPath.LostFocus += (object sender, RoutedEventArgs e) =>
                            {
                                if (settingPath.Text.Trim() != string.Empty)
                                {
                                    string fullPath;

                                    try
                                    {
                                        fullPath = Path.GetFullPath(settingPath.Text);
                                    }
                                    catch
                                    {
                                        fullPath = string.Empty;
                                    }

                                    setting.Key.SetValue(plugin.Settings, new FileInfo(fullPath));
                                    PluginManagement.PluginManager.GetInstance.SaveSettings(plugin, plugin.Settings);
                                }
                            };

                            StackPanel stackPanel = new() { Orientation = Avalonia.Layout.Orientation.Horizontal };
                            stackPanel.Children.Add(label);
                            stackPanel.Children.Add(settingPath);

                            settingsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(32) });
                            settingsGrid.AddControl(stackPanel, settingsGrid.RowDefinitions.Count - 1, 0, 2);
                            settingsGrid.AddControl(settingBrowse, settingsGrid.RowDefinitions.Count - 1, 2);

                            break;
                        case Action action:
                            Button actionButton = new()
                            {
                                Content = label.Text,
                                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left
                            };

                            actionButton.Click += (object sender, RoutedEventArgs e) =>
                            {
                                action.Invoke();

                                //Possible for the action to have changed a setting, save just in case.
                                PluginManagement.PluginManager.GetInstance.SaveSettings(plugin, plugin.Settings);
                            };

                            settingsGrid.AddControl(actionButton, settingsGrid.RowDefinitions.Count - 1, 0);

                            break;
                        case Dictionary<string, object> dictSetting:

                            var backingValueName = (SettingBackingValue)Attribute.GetCustomAttribute(setting.Key, typeof(SettingBackingValue));

                            var backingValue = from s in displayedSettings
                                               where s.Value == backingValueName.BackingProperty
                                               select s.Key;

                            if (backingValue.Count() != 1)
                                throw new($"{plugin.ShortName}: Dictionary settings must have exactly one backing value.");

                            label.Text += ": ";

                            ComboBox selectionDropDown = new()
                            {
                                MinWidth = 200
                            };

                            selectionDropDown.Items = from s in dictSetting
                                                      orderby s.Key
                                                      select s.Key;

                            string currentSelection = backingValue.First().GetValue(plugin.Settings)?.ToString();

                            if (currentSelection?.Length > 0)
                            {
                                selectionDropDown.SelectedItem = currentSelection;
                            }

                            selectionDropDown.SelectionChanged += (object sender, SelectionChangedEventArgs e) =>
                            {
                                var comboBox = (ComboBox)sender;
                                backingValue.First().SetValue(plugin.Settings, comboBox.SelectedItem.ToString());
                                PluginManagement.PluginManager.GetInstance.SaveSettings(plugin, plugin.Settings);
                            };

                            settingsGrid.AddControl(label, settingsGrid.RowDefinitions.Count - 1, 0);
                            settingsGrid.AddControl(selectionDropDown, settingsGrid.RowDefinitions.Count - 1, 1);

                            break;
                    }
                }
            }
        }

        private string GetStatusText(PluginManagement.PluginManager.PluginStatus status)
        {
            string statusText;

            switch (status)
            {
                case PluginManagement.PluginManager.PluginStatus.Signed:
                    statusText = "Signed";
                    break;
                case PluginManagement.PluginManager.PluginStatus.Unsigned:
                    statusText = "Unsigned";
                    break;
                case PluginManagement.PluginManager.PluginStatus.InvalidSignature:
                    statusText = "Signature Invalid";
                    break;
                case PluginManagement.PluginManager.PluginStatus.InvalidPlugin:
                    statusText = "No Interface";
                    break;
                case PluginManagement.PluginManager.PluginStatus.InvalidLibrary:
                    statusText = "Invalid Library";
                    break;
                default:
                    statusText = "Unknown";
                    break;
            }

            return statusText;
        }

        //From https://stackoverflow.com/questions/5796383/insert-spaces-between-words-on-a-camel-cased-token
        private static string SplitCamelCase(string str)
        {
            return Regex.Replace(
                Regex.Replace(
                    str,
                    @"(\P{Ll})(\P{Ll}\p{Ll})",
                    "$1 $2"
                ),
                @"(\p{Ll})(\P{Ll})",
                "$1 $2"
            );
        }
    }

    internal class PluginView
    {
        public string Name { get; set; }
        public HashSet<string> Types { get; set; }
        public string TypesString
        {
            get => string.Join(", ", Types);
            set { }
        }
        public string Version { get; set; }
        public string Status { get; set; }
    }

    internal static class GridExtention
    {
        public static void AddControl(this Grid grid, Control control, int row, int column, int span = 1)
        {
            grid.Children.Add(control);
            Grid.SetColumnSpan(control, span);
            Grid.SetColumn(control, column);
            Grid.SetRow(control, row);
        }
    }

    internal class SettingGridManager
    {
        private Grid settingPanel;

        public SettingGridManager(Grid settingPanel)
        {
            this.settingPanel = settingPanel;
        }

        public int NewRow()
        {
            settingPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0, GridUnitType.Auto) });
            
            return settingPanel.RowDefinitions.Count - 1;
        }

        public void AddSetting(Control control)
        {
            int rowIndex = NewRow();
            settingPanel.AddControl(control, rowIndex, 0, 2);
        }

        public void AddSettingSameLine(Control control)
        {
            int rowIndex = settingPanel.RowDefinitions.Count - 1;
            settingPanel.AddControl(control, rowIndex, 0, 2);
        }

        public void AddSettingWithLabel(Control label, Control control)
        {
            int rowIndex = NewRow();
            settingPanel.AddControl(label, rowIndex, 0);
            settingPanel.AddControl(control, rowIndex, 1, 2);
        }
    }
}
