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

            TextBlock nativeNotifyLabel = new() { Text = "Basic Notification" };
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

            corePanel.AddControl(nativeNotifyCheckbox, rowTracker.NextIndex(), 0, 2);

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

            Dictionary<string, PluginView> uniquePlugins = new();
            foreach(var (plugin, signed) in pluginManager.workerPlugins)
            {
                if (!uniquePlugins.ContainsKey(plugin.Name)) 
                {
                    uniquePlugins.Add(plugin.Name, 
                        new PluginView() { Name = plugin.Name, Types = new() { PluginType.Worker }, Version = plugin.Version, Status = GetStatusText(signed) });
                }
            }

            foreach (var (plugin, signed) in pluginManager.notifyPlugins)
            {
                if (!uniquePlugins.ContainsKey(plugin.Name))
                {
                    uniquePlugins.Add(plugin.Name, 
                        new PluginView() { Name = plugin.Name, Types = new() { PluginType.Notifier }, Version = plugin.Version, Status = GetStatusText(signed) });
                } else
                {
                    uniquePlugins[plugin.Name].Types.Add(PluginType.Notifier);
                }
            }

            pluginList.Items = uniquePlugins.Values;
            corePanel.AddControl(pluginList, SettingRowTracker.PLUGIN_LIST_ROW_INDEX, 0, 2);

            #endregion

            #region Plugin Settings

            foreach(var plugin in pluginManager.workerPlugins.Select(p => p.plugin))
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
                            Height = new GridLength(setting.Key.PropertyType != typeof(bool) ? 32 : 25),
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
                            NumericUpDown numericUpDown = new() { Value = intSetting, AllowSpin = true };
                            SettingNumericBounds attr = (SettingNumericBounds)System.Attribute.GetCustomAttribute(setting.Key, typeof(SettingNumericBounds));
                            if (attr != null)
                            {
                                numericUpDown.Minimum = attr.Minimum;
                                numericUpDown.Maximum = attr.Maximum;
                                numericUpDown.Increment = attr.Increment;
                            }
                            settingsGrid.AddControl(label, settingsGrid.RowDefinitions.Count - 1, 0);
                            settingsGrid.AddControl(numericUpDown, settingsGrid.RowDefinitions.Count - 1, 1);
                            numericUpDown.ValueChanged += (object sender, NumericUpDownValueChangedEventArgs e) =>
                            {
                                setting.Key.SetValue(plugin.Settings, Convert.ToInt32(e.NewValue));
                                PluginManagement.PluginManager.GetInstance.SaveSettings(plugin, plugin.Settings);
                            };
                            break;
                        case System.IO.FileInfo fileSetting:
                            label.Text += ": ";

                            TextBox settingPath = new()
                            {
                                Text = fileSetting.FullName,
                                Width = 250,
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

        private void TextBox_TextInput(object sender, Avalonia.Input.TextInputEventArgs e)
        {
            throw new NotImplementedException();
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
        public HashSet<PluginType> Types { get; set; }
        public string TypesString {
            get
            {
                return string.Join(", ", Types);
            }
            set { } }
        public string Version { get; set; }
        public string Status { get; set; }
    }

    enum PluginType
    {
        Worker,
        Notifier,
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
