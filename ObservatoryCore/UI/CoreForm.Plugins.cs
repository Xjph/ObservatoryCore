using Observatory.PluginManagement;
using Observatory.Framework.Interfaces;

namespace Observatory.UI
{
    partial class CoreForm
    {

        private void PopulatePluginList()
        {
            List<IObservatoryPlugin> uniquePlugins = new();
                        
            foreach (var (plugin, signed) in PluginManager.GetInstance.workerPlugins)
            {
                if (!uniquePlugins.Contains(plugin))
                {
                    uniquePlugins.Add(plugin);
                    ListViewItem item = new ListViewItem(new[] { plugin.Name, "Worker", plugin.Version, PluginStatusString(signed) });
                    PluginList.Items.Add(item);
                }
            }

            foreach (var (plugin, signed) in PluginManager.GetInstance.notifyPlugins)
            {
                if (!uniquePlugins.Contains(plugin))
                {
                    uniquePlugins.Add(plugin);
                    ListViewItem item = new ListViewItem(new[] { plugin.Name, "Notifier", plugin.Version, PluginStatusString(signed) });
                    PluginList.Items.Add(item);
                }
            }
        }

        private static string PluginStatusString(PluginManager.PluginStatus status)
        {
            switch (status)
            {
                case PluginManager.PluginStatus.Signed:
                    return "Signed";
                    
                case PluginManager.PluginStatus.Unsigned:
                    return "Unsigned";
                    
                case PluginManager.PluginStatus.InvalidSignature:
                    return "Invalid Signature";
                    
                case PluginManager.PluginStatus.InvalidPlugin:
                    return "Invalid Plugin";
                    
                case PluginManager.PluginStatus.InvalidLibrary:
                    return "Invalid File";
                    
                case PluginManager.PluginStatus.NoCert:
                    return "Unsigned Observatory (Debug build)";
                    
                case PluginManager.PluginStatus.SigCheckDisabled:
                    return "Signature Checks Disabled";
                    
                default:
                    return string.Empty;
            }
        }

        private void CreatePluginTabs()
        {
            var uiPlugins = PluginManager.GetInstance.workerPlugins.Where(p => p.plugin.PluginUI.PluginUIType != Framework.PluginUI.UIType.None);

            PluginHelper.CreatePluginTabs(CoreMenu, uiPlugins, uiPanels);

            foreach(ToolStripMenuItem item in CoreMenu.Items)
            {
                pluginList.Add(item.Text, item);
            }
        }

        private void CreatePluginSettings()
        {
            foreach (var plugin in PluginManager.GetInstance.workerPlugins)
            {
                var pluginSettingsPanel = new SettingsPanel(plugin.plugin, AdjustPanelsBelow);
                AddSettingsPanel(pluginSettingsPanel);
            }
            foreach (var plugin in PluginManager.GetInstance.notifyPlugins)
            {
                var pluginSettingsPanel = new SettingsPanel(plugin.plugin, AdjustPanelsBelow);
                AddSettingsPanel(pluginSettingsPanel);
            }
        }

        private void AddSettingsPanel(SettingsPanel panel)
        {
            int lowestPoint = 0;
            foreach (Control control in CorePanel.Controls)
            {
                if (control.Location.Y + control.Height > lowestPoint)
                    lowestPoint = control.Location.Y + control.Height;
            }
            DuplicateControlVisuals(PopupNotificationLabel, panel.Header);
            panel.Header.TextAlign = PopupNotificationLabel.TextAlign;
            panel.Header.Location = new Point(PopupNotificationLabel.Location.X, lowestPoint);

            DuplicateControlVisuals(PopupSettingsPanel, panel, false);
            panel.Location = new Point(PopupSettingsPanel.Location.X, lowestPoint + panel.Header.Height);
            panel.Visible = false;
            CorePanel.Controls.Add(panel.Header);
            CorePanel.Controls.Add(panel);
        }
    }
}