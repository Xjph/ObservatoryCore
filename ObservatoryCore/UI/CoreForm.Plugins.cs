using Observatory.PluginManagement;
using Observatory.Framework.Interfaces;
using System.Linq;

namespace Observatory.UI
{
    partial class CoreForm
    {
        private Dictionary<ListViewItem, IObservatoryPlugin>? ListedPlugins;

        private void PopulatePluginList()
        {
            ListedPlugins = new();
                        
            foreach (var (plugin, signed) in PluginManager.GetInstance.workerPlugins)
            {
                if (!ListedPlugins.ContainsValue(plugin))
                {
                    
                    ListViewItem item = new ListViewItem(new[] { plugin.Name, "Worker", plugin.Version, PluginStatusString(signed) });
                    ListedPlugins.Add(item, plugin);
                    PluginList.Items.Add(item);
                }
            }

            foreach (var (plugin, signed) in PluginManager.GetInstance.notifyPlugins)
            {
                if (!ListedPlugins.ContainsValue(plugin))
                {
                    ListViewItem item = new ListViewItem(new[] { plugin.Name, "Notifier", plugin.Version, PluginStatusString(signed) });
                    ListedPlugins.Add(item, plugin);
                    PluginList.Items.Add(item);
                }
            }

            PluginList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            PluginList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
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

            CoreMenu.Width = GetExpandedMenuWidth();
        }

        private void DisableOverriddenNotification()
        {
            var notifyPlugins = PluginManager.GetInstance.notifyPlugins;

            var ovPopupPlugins = notifyPlugins.Where(n => n.plugin.OverridePopupNotifications);

            if (ovPopupPlugins.Any())
            {
                PopupCheckbox.Checked = false;
                PopupCheckbox.Enabled = false;
                DisplayDropdown.Enabled = false;
                CornerDropdown.Enabled = false;
                FontDropdown.Enabled = false;
                ScaleSpinner.Enabled = false;
                DurationSpinner.Enabled = false;
                ColourButton.Enabled = false;
                TestButton.Enabled = false;

                var pluginNames = string.Join(", ", ovPopupPlugins.Select(o => o.plugin.ShortName));

                PopupSettingsPanel.MouseMove += (_, _) =>
                {
                    OverrideTooltip.SetToolTip(PopupSettingsPanel, "Disabled by plugin: " + pluginNames);
                };
            }

            var ovAudioPlugins = notifyPlugins.Where(n => n.plugin.OverrideAudioNotifications);

            if (ovAudioPlugins.Any())
            {
                VoiceCheckbox.Checked = false;
                VoiceCheckbox.Enabled = false;
                VoiceVolumeSlider.Enabled = false;
                VoiceSpeedSlider.Enabled = false;
                VoiceDropdown.Enabled = false;
                VoiceTestButton.Enabled = false;

                var pluginNames = string.Join(", ", ovAudioPlugins.Select(o => o.plugin.ShortName));

                VoiceSettingsPanel.MouseMove += (_, _) =>
                {
                    OverrideTooltip.SetToolTip(VoiceSettingsPanel, "Disabled by plugin: " + pluginNames);
                };
            }
        }

        private int GetExpandedMenuWidth()
        {
            int maxWidth = 0;
            foreach (ToolStripMenuItem item in CoreMenu.Items)
            {
                var itemWidth = TextRenderer.MeasureText(item.Text, item.Font);
                maxWidth = itemWidth.Width > maxWidth ? itemWidth.Width : maxWidth;
            }

            return maxWidth + 5;
        }

        private void PluginSettingsButton_Click(object sender, EventArgs e)
        {
            if (ListedPlugins != null && PluginList.SelectedItems.Count != 0)
            {
                var plugin = ListedPlugins[PluginList.SelectedItems[0]];
                if (SettingsForms.ContainsKey(plugin))
                {
                    SettingsForms[plugin].Activate();
                }
                else
                {
                    SettingsForm settingsForm = new(plugin);
                    SettingsForms.Add(plugin, settingsForm);
                    settingsForm.FormClosed += (_, _) => SettingsForms.Remove(plugin);
                    settingsForm.Show();
                }
            }
        }

        private Dictionary<IObservatoryPlugin, SettingsForm> SettingsForms = new();
    }
}