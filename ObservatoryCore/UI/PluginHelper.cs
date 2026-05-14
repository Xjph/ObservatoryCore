using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using Observatory.Framework;
using Observatory.Framework.Interfaces;
using Observatory.PluginManagement;
using Observatory.Utils;
using static System.Windows.Forms.ListViewItem;

namespace Observatory.UI
{
    internal class PluginHelper
    {
        internal static void CreatePluginTabs(
            TabControl tabs,
            IEnumerable<IObservatoryWorker> plugins,
            Dictionary<TabPage, IObservatoryPlugin> pluginList,
            List<ColumnSizing> columnSizings
        )
        {
            foreach (var plugin in plugins.OrderBy(p => p.ShortName))
            {
                var newTab = AddPlugin(tabs, plugin, columnSizings);
                pluginList.Add(newTab, plugin);
            }
        }

        internal static void CreatePluginTabs(
            TabControl tabs,
            IEnumerable<IObservatoryNotifier> plugins,
            Dictionary<TabPage, IObservatoryPlugin> pluginList,
            List<ColumnSizing> columnSizings
        )
        {
            foreach (var plugin in plugins.OrderBy(p => p.ShortName))
            {
                var newTab = AddPlugin(tabs, plugin, columnSizings);
                pluginList.Add(newTab, plugin);
            }
        }

        private static TabPage AddPlugin(
            TabControl tabs,
            IObservatoryPlugin plugin,
            List<ColumnSizing> columnSizings
        )
        {
            var newTab = new TabPage
            {
                Text = plugin.ShortName,
                BackColor = tabs.TabPages[0].BackColor,
                ForeColor = tabs.TabPages[0].ForeColor,
                Font = tabs.TabPages[0].Font,
            };

            var themeManager = ThemeManager.GetInstance;
            themeManager.RegisterControl(newTab);
            tabs.TabPages.Add(newTab);

            void addAndRegister(Panel pluginPanel)
            {
                themeManager.RegisterControl(pluginPanel, plugin.ApplyTheme);
                pluginPanel.Width = newTab.Width;
                pluginPanel.Height = newTab.Height;
                pluginPanel.Anchor =
                    AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
                newTab.Controls.Add(pluginPanel);
            }

            if (plugin.PluginUI.PluginUIType == Framework.PluginUI.UIType.Basic)
                addAndRegister(CreateBasicUI(plugin, columnSizings));
            else if (plugin.PluginUI.PluginUIType == Framework.PluginUI.UIType.Panel)
                addAndRegister((Panel)plugin.PluginUI.UI);

            return newTab;
        }

        private static Panel CreateBasicUI(
            IObservatoryPlugin plugin,
            List<ColumnSizing> columnSizings
        )
        {
            Panel panel = new()
            {
                Anchor =
                    AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Top,
            };
            plugin.PluginUI.UI = panel;

            PluginUIGrid listView = new(plugin, columnSizings)
            {
                Location = new Point(0, 0),
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(64, 64, 64),
                ForeColor = Color.LightGray
#if !PROTON
                ,
                Font = new Font(new FontFamily("Segoe UI"), 10, FontStyle.Regular)
#endif
            };
            panel.Controls.Add(listView);

            if (plugin.PluginUI is PluginActionUI actionPlugin && actionPlugin.Actions.Any())
            {
                DockStyle actionDock;
                FlowDirection actionFlow;

                switch (actionPlugin.PluginActionLocation)
                {
                    case PluginActionUI.ActionDock.Top:
                        actionDock = DockStyle.Top;
                        actionFlow = FlowDirection.LeftToRight;
                        break;
                    case PluginActionUI.ActionDock.Left:
                        actionDock = DockStyle.Left;
                        actionFlow = FlowDirection.TopDown;
                        break;
                    case PluginActionUI.ActionDock.Right:
                        actionDock = DockStyle.Right;
                        actionFlow = FlowDirection.TopDown;
                        break;
                    default:
                        actionDock = DockStyle.Bottom;
                        actionFlow = FlowDirection.LeftToRight;
                        break;
                }

                FlowLayoutPanel actionPanel = new()
                {
                    Dock = actionDock,
                    FlowDirection = actionFlow,
                    AutoSize = true,
                };

                var radioGroup = false;
                FlowLayoutPanel? radioGroupPanel = null;
                foreach (var action in actionPlugin.Actions)
                {
                    if (
                        action.ActionType != PluginActionUI.ActionType.Radiobutton
                        && radioGroupPanel != null
                    )
                    {
                        actionPanel.Controls.Add(radioGroupPanel);
                        radioGroupPanel = null;
                        radioGroup = false;
                    }

                    switch (action.ActionType)
                    {
                        case PluginActionUI.ActionType.Button:
                            Button actionButton = new()
                            {
                                Text = action.Label,
                                AutoSize = true,
                                FlatAppearance = { BorderSize = 0 },
                                FlatStyle = FlatStyle.Flat,
                            };
                            actionPanel.Controls.Add(actionButton);
                            break;
                        case PluginActionUI.ActionType.Checkbox:
                            CheckBox actionCheckbox = new()
                            {
                                Text = action.Label,
                                AutoSize = true,
                                Checked = action.InitialState,
                            };
                            actionCheckbox.CheckedChanged += (_, _) =>
                            {
                                action.Action(actionCheckbox.Checked);
                            };
                            actionPanel.Controls.Add(actionCheckbox);
                            break;
                        case PluginActionUI.ActionType.Radiobutton:
                            if (!radioGroup || radioGroupPanel == null)
                            {
                                radioGroupPanel = new FlowLayoutPanel()
                                {
                                    FlowDirection = actionPanel.FlowDirection,
                                    AutoSize = true,
                                };
                            }
                            RadioButton actionRadioButton = new()
                            {
                                Text = action.Label,
                                AutoSize = true,
                                Checked = action.InitialState,
                            };
                            actionRadioButton.CheckedChanged += (_, _) =>
                            {
                                action.Action(actionRadioButton.Checked);
                            };
                            radioGroupPanel.Controls.Add(actionRadioButton);
                            break;
                    }
                }
                if (radioGroupPanel != null)
                    actionPanel.Controls.Add(radioGroupPanel);

                panel.Controls.Add(actionPanel);
            }

            return panel;
        }
    }
}
