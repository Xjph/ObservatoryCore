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

namespace Observatory.UI.Views
{
    public class BasicUIView : UserControl
    {
        private DataGrid dataGrid;

        public BasicUIView()
        {
            Initialized += OnInitialized;
            InitializeComponent();
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

        private void OnInitialized(object sender, System.EventArgs e)
        {
            
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
                    break;
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

            RowDefinitions rows = new()
            {
                new RowDefinition() { Height = new GridLength(0, GridUnitType.Auto) },
                new RowDefinition() { Height = new GridLength(0, GridUnitType.Auto) },
                new RowDefinition() { Height = new GridLength(0, GridUnitType.Auto) },
                new RowDefinition() { Height = new GridLength(0, GridUnitType.Auto) }
            };
            corePanel.RowDefinitions = rows;

            SettingRowTracker rowTracker = new SettingRowTracker(corePanel);

            #region Native Settings

            #region Notification settings

            Expander notificationExpander = new()
            {
                Header = "Basic Notifications",
                DataContext = Properties.Core.Default,
                Margin = new Thickness(0, 20),
                Background = this.Background,
                BorderThickness = new Thickness(0)
            };

            Grid notificationGrid = new();

            notificationGrid.ColumnDefinitions = new()
            {
                new ColumnDefinition() { Width = new GridLength(0, GridUnitType.Star) },
                new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) },
                new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) }
            };

            notificationGrid.RowDefinitions = new()
            {
                new RowDefinition() { Height = new GridLength(0, GridUnitType.Auto) },
                new RowDefinition() { Height = new GridLength(0, GridUnitType.Auto) },
                new RowDefinition() { Height = new GridLength(0, GridUnitType.Auto) },
                new RowDefinition() { Height = new GridLength(0, GridUnitType.Auto) },
                new RowDefinition() { Height = new GridLength(0, GridUnitType.Auto) }
            };

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
            
            notificationGrid.AddControl(nativeNotifyCheckbox, 4, 0);

            Button notifyTestButton = new()
            { 
                Content = "Test",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right
            };

            notifyTestButton.Click += (object sender, RoutedEventArgs e) =>
            {
                Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    var notifyWindow = new UI.Views.NotificationView() { DataContext = new UI.ViewModels.NotificationViewModel("Test Notification", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Cras suscipit hendrerit libero ac scelerisque.") };
                    notifyWindow.Show();
                });
            };

            notificationGrid.AddControl(notifyTestButton, 4, 1);

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
            
            notificationGrid.AddControl(notifyFontLabel, 0, 0);

            notificationGrid.AddControl(notifyFontDropDown, 0, 1, 2);

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
            var screens = ((Window)Parent.Parent.Parent.Parent).Screens.All;
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

            notificationGrid.AddControl(monitorLabel, 2, 0);

            notificationGrid.AddControl(monitorDropDown, 2, 1, 2);

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

            notificationGrid.AddControl(cornerLabel, 3, 0);

            notificationGrid.AddControl(cornerDropDown, 3, 1, 2);

            TextBlock colourLabel = new()
            {
                Text = "Colour: ",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };

            BrushConverter brushConverter = new();

            Egorozh.ColorPicker.Dialog.ColorPickerButton colourPickerButton = new()
            {
                MinWidth = 200,
                MinHeight = 25,
                Color = Color.FromUInt32(Properties.Core.Default.NativeNotifyColour)
                
            };

            colourPickerButton.PropertyChanged += (object sender, AvaloniaPropertyChangedEventArgs e) =>
            {
                if (e.Property.Name == "Color")
                {
                    Properties.Core.Default.NativeNotifyColour = ((Color)e.NewValue).ToUint32();
                    Properties.Core.Default.Save();
                }
            };

            notificationGrid.AddControl(colourLabel, 1, 0);
            notificationGrid.AddControl(colourPickerButton, 1, 1);

            notificationExpander.Content = notificationGrid;
            

            corePanel.AddControl(notificationExpander, rowTracker.NextIndex(), 0, 2);

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

            corePanel.AddControl(primeSystemContexCheckbox, rowTracker.NextIndex(), 0, 2);

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

            int journalPathRowIndex = rowTracker.NextIndex();
            corePanel.AddControl(journalPathLabel, journalPathRowIndex, 0);
            corePanel.AddControl(journalPath, journalPathRowIndex, 1);
            corePanel.AddControl(journalBrowse, journalPathRowIndex, 2);

            #endregion

            var pluginManager = PluginManagement.PluginManager.GetInstance;

            #region Plugin List
            DataGrid pluginList = new() { Margin = new Thickness(0, 20) };

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
            foreach(var (plugin, signed) in pluginManager.workerPlugins)
            {
                if (!uniquePlugins.ContainsKey(plugin)) 
                {
                    uniquePlugins.Add(plugin, 
                        new PluginView() { Name = plugin.Name, Types = new() { typeof(IObservatoryWorker).Name }, Version = plugin.Version, Status = GetStatusText(signed) });
                }
            }

            foreach (var (plugin, signed) in pluginManager.notifyPlugins)
            {
                if (!uniquePlugins.ContainsKey(plugin))
                {
                    uniquePlugins.Add(plugin, 
                        new PluginView() { Name = plugin.Name, Types = new() { typeof(IObservatoryNotifier).Name }, Version = plugin.Version, Status = GetStatusText(signed) });
                } else
                {
                    uniquePlugins[plugin].Types.Add(typeof(IObservatoryNotifier).Name);
                }
            }

            pluginList.Items = uniquePlugins.Values;
            corePanel.AddControl(pluginList, SettingRowTracker.PLUGIN_LIST_ROW_INDEX, 0, 2);

            #endregion

            #region Plugin Settings

            foreach(var plugin in uniquePlugins.Keys)
            {
                GeneratePluginSettingUI(corePanel, plugin);
            }

            #endregion

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
                    Margin = new Thickness(0, 20)
                };

                Grid settingsGrid = new();
                ColumnDefinitions settingColumns = new()
                {
                    new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) },
                    new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) },
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) }
                };
                settingsGrid.ColumnDefinitions = settingColumns;
                expander.Content = settingsGrid;

                int nextRow = gridPanel.RowDefinitions.Count;
                gridPanel.RowDefinitions.Add(new RowDefinition());
                gridPanel.AddControl(expander, nextRow, 0, 3);

                foreach (var setting in displayedSettings.Where(s => !System.Attribute.IsDefined(s.Key, typeof(SettingIgnore))))
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
                            textBox.TextInput += (object sender, Avalonia.Input.TextInputEventArgs e) =>
                            {
                                setting.Key.SetValue(plugin.Settings, e.Text);
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
                        case System.IO.FileInfo fileSetting:
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
                                        setting.Key.SetValue(plugin.Settings, new System.IO.FileInfo(path));
                                        PluginManagement.PluginManager.GetInstance.SaveSettings(plugin, plugin.Settings);

                                    }
                                });
                                
                            };

                            StackPanel stackPanel = new() { Orientation = Avalonia.Layout.Orientation.Horizontal };
                            stackPanel.Children.Add(label);
                            stackPanel.Children.Add(settingPath);

                            settingsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(32) });
                            settingsGrid.AddControl(stackPanel, settingsGrid.RowDefinitions.Count - 1, 0, 2);
                            settingsGrid.AddControl(settingBrowse, settingsGrid.RowDefinitions.Count - 1, 2);

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

    internal class SettingRowTracker
    {
        public const int PLUGIN_LIST_ROW_INDEX = 0;
        private int nextSettingRowIndex;

        private Grid settingPanel;

        public SettingRowTracker(Grid settingPanel)
        {
            this.settingPanel = settingPanel;
            Reset();
        }

        public int NextIndex()
        {
            if (nextSettingRowIndex > settingPanel.RowDefinitions.Count)
            {
                throw new IndexOutOfRangeException("Trying to add more settings than rows in the settings grid.");
            }
            return nextSettingRowIndex++;
        }

        private void Reset()
        {
            nextSettingRowIndex = PLUGIN_LIST_ROW_INDEX + 1;
        }

    }
}
