using Observatory.Framework.Interfaces;
using Observatory.PluginManagement;
using Observatory.Utils;
using System.Text.Json;
using System.Collections.Concurrent;
using System.Text;
using static System.Windows.Forms.ListViewItem;

namespace Observatory.UI
{
    internal class PluginHelper
    {
        internal static void CreatePluginTabs(TabControl tabs, IEnumerable<(IObservatoryWorker plugin, PluginManager.PluginStatus signed)> plugins, Dictionary<TabPage, IObservatoryPlugin> pluginList, List<ColumnSizing> columnSizings)
        {
            foreach (var plugin in plugins.OrderBy(p => p.plugin.ShortName))
            {
                var newTab = AddPlugin(tabs, plugin.plugin, columnSizings);
                pluginList.Add(newTab, plugin.plugin);
            }
        }

        internal static void CreatePluginTabs(TabControl tabs, IEnumerable<(IObservatoryNotifier plugin, PluginManager.PluginStatus signed)> plugins, Dictionary<TabPage, IObservatoryPlugin> pluginList, List<ColumnSizing> columnSizings)
        {
            foreach (var plugin in plugins.OrderBy(p => p.plugin.ShortName))
            {
                var newTab = AddPlugin(tabs, plugin.plugin, columnSizings);
                pluginList.Add(newTab, plugin.plugin);
            }
        }

        private static TabPage AddPlugin(TabControl tabs, IObservatoryPlugin plugin, List<ColumnSizing> columnSizings)
        {
            var newTab = new TabPage
            {
                Text = plugin.ShortName,
                BackColor = tabs.TabPages[0].BackColor,
                ForeColor = tabs.TabPages[0].ForeColor,
                Font = tabs.TabPages[0].Font
            };

            var themeManager = ThemeManager.GetInstance;
            themeManager.RegisterControl(newTab);
            tabs.TabPages.Add(newTab);

            void addAndRegister(Panel pluginPanel)
            {
                themeManager.RegisterControl(pluginPanel, plugin.ApplyTheme);
                pluginPanel.Width = newTab.Width;
                pluginPanel.Height = newTab.Height;
                pluginPanel.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
                newTab.Controls.Add(pluginPanel);
            }

            if (plugin.PluginUI.PluginUIType == Framework.PluginUI.UIType.Basic)
                addAndRegister(CreateBasicUI(plugin, columnSizings));
            else if (plugin.PluginUI.PluginUIType == Framework.PluginUI.UIType.Panel)
                addAndRegister((Panel)plugin.PluginUI.UI);

            return newTab;
        }

        private static Panel CreateBasicUI(IObservatoryPlugin plugin, List<ColumnSizing> columnSizings)
        {
            Panel panel = new()
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Top
            };
            plugin.PluginUI.UI = panel;

            PluginListView listView = new(plugin, columnSizings)
            {
                Location = new Point(0, 0),
                Size = panel.Size,
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(64, 64, 64),
                ForeColor = Color.LightGray,
                Font = new Font(new FontFamily("Segoe UI"), 10, FontStyle.Regular)
            };
            panel.Controls.Add(listView);

            return panel;
        }
    }
}
