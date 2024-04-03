using Observatory.Framework.Interfaces;
using Observatory.Framework;
using System.Collections;
using Observatory.PluginManagement;
using Observatory.Utils;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Data.Common;
using System.ComponentModel.Design.Serialization;

namespace Observatory.UI
{
    internal class PluginHelper
    {
        internal static List<string> CreatePluginTabs(MenuStrip menu, IEnumerable<(IObservatoryWorker plugin, PluginManagement.PluginManager.PluginStatus signed)> plugins, Dictionary<object, Panel> uiPanels)
        {
            List<string> pluginList = new List<string>();
            foreach (var plugin in plugins.OrderBy(p => p.plugin.ShortName))
            {
                AddPlugin(menu, plugin.plugin, plugin.signed, uiPanels);
                pluginList.Add(plugin.plugin.ShortName);
            }
            return pluginList;
        }

        internal static List<string> CreatePluginTabs(MenuStrip menu, IEnumerable<(IObservatoryNotifier plugin, PluginManagement.PluginManager.PluginStatus signed)> plugins, Dictionary<object, Panel> uiPanels)
        {
            List<string> pluginList = new List<string>();
            foreach (var plugin in plugins.OrderBy(p => p.plugin.ShortName))
            {
                AddPlugin(menu, plugin.plugin, plugin.signed, uiPanels);
                pluginList.Add(plugin.plugin.ShortName);
            }
            return pluginList;
        }

        private static void AddPlugin(MenuStrip menu, IObservatoryPlugin plugin, PluginManagement.PluginManager.PluginStatus signed, Dictionary<object, Panel> uiPanels)
        {
            var newItem = new ToolStripMenuItem()
            {
                Text = plugin.ShortName,
                BackColor = menu.Items[0].BackColor,
                ForeColor = menu.Items[0].ForeColor,
                Font = menu.Items[0].Font,
                TextAlign = menu.Items[0].TextAlign
            };
            ThemeManager.GetInstance.RegisterControl(newItem);
            menu.Items.Add(newItem);

            if (plugin.PluginUI.PluginUIType == Framework.PluginUI.UIType.Basic)
                uiPanels.Add(newItem, CreateBasicUI(plugin));
            else if (plugin.PluginUI.PluginUIType == Framework.PluginUI.UIType.Panel)
            {
                uiPanels.Add(newItem, (Panel)plugin.PluginUI.UI);
                ThemeManager.GetInstance.RegisterControl((Panel)plugin.PluginUI.UI);
            }
        }

        private static Panel CreateBasicUI(IObservatoryPlugin plugin)
        {
            Panel panel = new()
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Top
            };
            plugin.PluginUI.UI = panel;

            IObservatoryComparer columnSorter;
            if (plugin.ColumnSorter != null)
                columnSorter = plugin.ColumnSorter;
            else
                columnSorter = new DefaultSorter();

            PluginListView listView = new()
            {
                View = View.Details,
                Location = new Point(0, 0),
                Size = panel.Size,
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(64, 64, 64),
                ForeColor = Color.LightGray,
                ListViewItemSorter = columnSorter,
                Font = new Font(new FontFamily("Segoe UI"), 10, FontStyle.Regular)
            };
            panel.Controls.Add(listView);

            string colSize = Properties.Core.Default.ColumnSizing;
            List<ColumnSizing>? columnSizing = null;
            if (!string.IsNullOrWhiteSpace(colSize))
            {
                try
                {
                    columnSizing = JsonSerializer.Deserialize<List<ColumnSizing>>(colSize);
                }
                catch
                {
                    // Failed deserialization means bad value, blow it away.
                    Properties.Core.Default.ColumnSizing = string.Empty;
                    Properties.Core.Default.Save();
                }
            }

            columnSizing ??= new List<ColumnSizing>();
            // Is losing column sizes between versions acceptable?
            ColumnSizing pluginColumnSizing = columnSizing
                .Where(c => c.PluginName == plugin.Name && c.PluginVersion == plugin.Version)
                .FirstOrDefault(new ColumnSizing() { PluginName = plugin.Name, PluginVersion = plugin.Version });

            if (!columnSizing.Contains(pluginColumnSizing))
            {
                columnSizing.Add(pluginColumnSizing);
            }

            foreach (var property in plugin.PluginUI.DataGrid.First().GetType().GetProperties())
            {
                // https://stackoverflow.com/questions/5796383/insert-spaces-between-words-on-a-camel-cased-token
                string columnLabel = Regex.Replace(
                    Regex.Replace(
                        property.Name,
                        @"(\P{Ll})(\P{Ll}\p{Ll})",
                        "$1 $2"
                    ),
                    @"(\p{Ll})(\P{Ll})",
                    "$1 $2"
                );

                int width;

                if (pluginColumnSizing.ColumnWidth.ContainsKey(columnLabel))
                {
                    width = pluginColumnSizing.ColumnWidth[columnLabel];
                }
                else
                {
                    var widthAttrib = property.GetCustomAttribute<ColumnSuggestedWidth>();

                    width = widthAttrib == null
                        // Rough approximation of width by label length if none specified.
                        ? columnLabel.Length * 10
                        : widthAttrib.Width;

                    pluginColumnSizing.ColumnWidth.Add(columnLabel, width);
                }
         
                listView.Columns.Add(columnLabel, width);

            }

            Properties.Core.Default.ColumnSizing = JsonSerializer.Serialize(columnSizing);
            Properties.Core.Default.Save();

            // Oddly, the listview resize event often fires after the column size change but
            // with stale (default?!) column width values.
            // Still need a resize handler to avoid the ugliness of the rightmost column
            // leaving gaps, but preventing saving the width changes there should stop the
            // stale resize event from overwriting with bad data.
            // Using a higher-order function here to create two different versions of the
            // event handler for these purposes.
            var handleColSize = (bool saveProps) =>
            (object? sender, EventArgs e) =>
            {
                int colTotalWidth = 0;
                ColumnHeader? rightmost = null;
                foreach (ColumnHeader column in listView.Columns)
                {
                    colTotalWidth += column.Width;
                    if (rightmost == null || column.DisplayIndex > rightmost.DisplayIndex)
                        rightmost = column;

                    if (saveProps)
                    {
                        if (pluginColumnSizing.ColumnWidth.ContainsKey(column.Text))
                            pluginColumnSizing.ColumnWidth[column.Text] = column.Width;
                        else
                            pluginColumnSizing.ColumnWidth.Add(column.Text, column.Width);
                    }
                }

                if (rightmost != null && colTotalWidth < listView.Width)
                {
                    rightmost.Width = listView.Width - (colTotalWidth - rightmost.Width);

                    if (saveProps)
                        pluginColumnSizing.ColumnWidth[rightmost.Text] = rightmost.Width;
                }

                if (saveProps)
                {
                    Properties.Core.Default.ColumnSizing = JsonSerializer.Serialize(columnSizing);
                    Properties.Core.Default.Save();
                }
            };

            listView.ColumnWidthChanged += handleColSize(true).Invoke;
            listView.Resize += handleColSize(false).Invoke;
             
            listView.ColumnClick += (sender, e) =>
            {
                if (e.Column == columnSorter.SortColumn)
                {
                    // Reverse the current sort direction for this column.
                    if (columnSorter.Order == 1)
                    {
                        columnSorter.Order = -1;
                    }
                    else
                    {
                        columnSorter.Order = 1;
                    }
                }
                else
                {
                    // Set the column number that is to be sorted; default to ascending.
                    columnSorter.SortColumn = e.Column;
                    columnSorter.Order = 1;
                }
                listView.Sort();
            };

            plugin.PluginUI.DataGrid.CollectionChanged += (sender, e) =>
            {
                var updateGrid = () =>
                {
                    if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add &&
                e.NewItems != null)
                    {
                        foreach (var newItem in e.NewItems)
                        {
                            ListViewItem newListItem = new();
                            foreach (var property in newItem.GetType().GetProperties())
                            {
                                newListItem.SubItems.Add(property.GetValue(newItem)?.ToString());
                            }
                            newListItem.SubItems.RemoveAt(0);
                            listView.Items.Add(newListItem);
                        }
                    }

                    if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove &&
                    e.OldItems != null)
                    {
                        foreach (var oldItem in e.OldItems)
                        {
                            ListViewItem oldListItem = new();
                            foreach (var property in oldItem.GetType().GetProperties())
                            {
                                oldListItem.SubItems.Add(property.GetValue(oldItem)?.ToString());
                            }
                            oldListItem.SubItems.RemoveAt(0);

                            var itemToRemove = listView.Items.Cast<ListViewItem>().Where(i => i.SubItems.Cast<string>().SequenceEqual(oldListItem.SubItems.Cast<string>())).First();
                            if (itemToRemove != null)
                            {
                                listView.Items.Remove(itemToRemove);
                            }
                        }
                    }

                    if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
                    {
                        listView.Items.Clear();
                        foreach (var item in plugin.PluginUI.DataGrid)
                        {
                            ListViewItem listItem = new();
                            foreach (var property in item.GetType().GetProperties())
                            {
                                listItem.SubItems.Add(property.GetValue(item)?.ToString());
                            }
                            listItem.SubItems.RemoveAt(0);
                            listView.Items.Add(listItem);
                        }
                    }
                };

                if (listView.Created)
                {
                    listView.Invoke(updateGrid);
                }
                else
                {
                    updateGrid();
                }
            };
            
            return panel;
        }

        internal static Panel CreatePluginSettings(IObservatoryPlugin plugin)
        {
            Panel panel = new Panel();

            return panel;
        }
    }
}
