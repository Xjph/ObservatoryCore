using Observatory.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

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
        }

        private static Panel CreateBasicUI(IObservatoryPlugin plugin)
        {
            Panel panel = new();
            var columnSorter = new DefaultSorter();
            ListView listView = new()
            {
                View = View.Details,
                Location = new Point(0, 0),
                Size = panel.Size,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Top,
                BackColor = Color.FromArgb(64, 64, 64),
                ForeColor = Color.LightGray,
                GridLines = true,
                ListViewItemSorter = columnSorter
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
                    if (columnSorter.Order == SortOrder.Ascending)
                    {
                        columnSorter.Order = SortOrder.Descending;
                    }
                    else
                    {
                        columnSorter.Order = SortOrder.Ascending;
                    }
                }
                else
                {
                    // Set the column number that is to be sorted; default to ascending.
                    columnSorter.SortColumn = e.Column;
                    columnSorter.Order = SortOrder.Ascending;
                }
                listView.Sort();
            };

            panel.Controls.Add(listView);
            
            plugin.PluginUI.DataGrid.CollectionChanged += (sender, e) =>
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
