using Observatory.Framework.Interfaces;
using System.Collections;
using Observatory.PluginManagement;
using Observatory.Utils;

namespace Observatory.UI
{
    internal class PluginHelper
    {
        internal static List<string> CreatePluginTabs(MenuStrip menu, IEnumerable<(IObservatoryWorker plugin, PluginManagement.PluginManager.PluginStatus signed)> plugins, Dictionary<object, Panel> uiPanels)
        {
            List<string> pluginList = new List<string>();
            foreach (var plugin in plugins)
            {
                AddPlugin(menu, plugin.plugin, plugin.signed, uiPanels);
                pluginList.Add(plugin.plugin.ShortName);
            }
            return pluginList;
        }

        internal static List<string> CreatePluginTabs(MenuStrip menu, IEnumerable<(IObservatoryNotifier plugin, PluginManagement.PluginManager.PluginStatus signed)> plugins, Dictionary<object, Panel> uiPanels)
        {
            List<string> pluginList = new List<string>();
            foreach (var plugin in plugins)
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
            menu.Items.Add(newItem);

            if (plugin.PluginUI.PluginUIType == Framework.PluginUI.UIType.Basic)
                uiPanels.Add(newItem, CreateBasicUI(plugin));
            else if (plugin.PluginUI.PluginUIType == Framework.PluginUI.UIType.Panel)
                uiPanels.Add(newItem, (Panel)plugin.PluginUI.UI);
        }

        private static Panel CreateBasicUI(IObservatoryPlugin plugin)
        {
            Panel panel = new();

            IObservatoryComparer columnSorter;
            if (plugin.ColumnSorter != null)
                columnSorter = plugin.ColumnSorter;
            else
                columnSorter = new DefaultSorter();

            ListView listView = new()
            {
                View = View.Details,
                Location = new Point(0, 0),
                Size = panel.Size,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Top,
                BackColor = Color.FromArgb(64, 64, 64),
                ForeColor = Color.LightGray,
                GridLines = true,
                ListViewItemSorter = columnSorter,
                Font = new Font(new FontFamily("Segoe UI"), 10, FontStyle.Regular)
            };

            foreach (var property in plugin.PluginUI.DataGrid.First().GetType().GetProperties())
            {
                listView.Columns.Add(property.Name);
            }

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

            panel.Controls.Add(listView);
            
            plugin.PluginUI.DataGrid.CollectionChanged += (sender, e) =>
            {
                listView.Invoke(() =>
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
                });
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
