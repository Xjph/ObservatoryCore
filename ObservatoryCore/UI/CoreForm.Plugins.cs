using Observatory.PluginManagement;
using Observatory.Framework.Interfaces;
using System.Text.Json;
using Observatory.Framework;
using Observatory.Utils;
using System.Text;

namespace Observatory.UI
{
    partial class CoreForm
    {
        private Dictionary<ListViewItem, IObservatoryPlugin>? ListedPlugins;
        private bool loading = true; // Suppress settings updates due to initializing the listview.
        
        private void PopulatePluginList()
        {
            ListedPlugins = new();
                        
            foreach (var (plugin, signed) in PluginManager.GetInstance.EnabledWorkerPlugins)
            {
                if (!ListedPlugins.ContainsValue(plugin))
                {
                    
                    ListViewItem item = new ListViewItem(new[] { plugin.Name, "Worker", plugin.Version, PluginStatusString(signed) });
                    ListedPlugins.Add(item, plugin);
                    var lvItem = PluginList.Items.Add(item);
                    lvItem.Checked = true; // Start with enabled, let settings disable things.
                }
            }

            foreach (var (plugin, signed) in PluginManager.GetInstance.EnabledNotifyPlugins)
            {
                if (!ListedPlugins.ContainsValue(plugin))
                {
                    ListViewItem item = new ListViewItem(new[] { plugin.Name, "Notifier", plugin.Version, PluginStatusString(signed) });
                    ListedPlugins.Add(item, plugin);
                    var lvItem = PluginList.Items.Add(item);
                    lvItem.Checked = true; // Start with enabled, let settings disable things.
                }
            }

            PluginsEnabledStateFromSettings();

            PluginList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            PluginList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            loading = false;
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
            var uiPlugins = PluginManager.GetInstance.EnabledWorkerPlugins.Where(p => p.plugin.PluginUI.PluginUIType != Framework.PluginUI.UIType.None);

            PluginHelper.CreatePluginTabs(CoreTabControl, uiPlugins, pluginList);
        }

        private void DisableOverriddenNotification()
        {
            var notifyPlugins = PluginManager.GetInstance.EnabledNotifyPlugins;

            var ovPopupPlugins = notifyPlugins.Where(n => n.plugin.OverridePopupNotifications);
            var hasPopupOverriders = ovPopupPlugins.Any();

            var disableMessage = (string type, string plugin) 
                => $"Native {type} notifications overridden by \"{plugin}\".\r\n"
                + "Use plugin settings to configure.";


            PopupCheckbox.Checked = Properties.Core.Default.NativeNotify;
            PopupCheckbox.Enabled = !hasPopupOverriders;
            DisplayDropdown.Enabled = !hasPopupOverriders;
            CornerDropdown.Enabled = !hasPopupOverriders;
            FontDropdown.Enabled = !hasPopupOverriders;
            ScaleSpinner.Enabled = !hasPopupOverriders;
            DurationSpinner.Enabled = !hasPopupOverriders;
            ColourButton.Enabled = !hasPopupOverriders;
            TestButton.Enabled = !hasPopupOverriders;
            PopupDisabledPanel.Visible = hasPopupOverriders;
            PopupDisabledPanel.Enabled = hasPopupOverriders;

            if (hasPopupOverriders)
            {
                var pluginNames = string.Join(", ", ovPopupPlugins.Select(o => o.plugin.Name));

                PopupDisabledLabel.Text = disableMessage("popup", pluginNames);
                PopupDisabledPanel.BringToFront();
            }
            else
            {
                PopupDisabledPanel.SendToBack();
            }

#if !PROTON // Proton doesn't support native voice. Don't fiddle with anything if overriders are changed.
            // See PopulateNativeSettings().
            var ovAudioPlugins = notifyPlugins.Where(n => n.plugin.OverrideAudioNotifications);
            var hasAudioOverriders = ovAudioPlugins.Any();

            VoiceCheckbox.Checked = Properties.Core.Default.VoiceNotify;
            VoiceCheckbox.Enabled = !hasAudioOverriders;
            VoiceSpeedSlider.Enabled = !hasAudioOverriders;
            VoiceDropdown.Enabled = !hasAudioOverriders;
            VoiceTestButton.Enabled = !hasAudioOverriders;
            VoiceDisabledPanel.Visible = hasAudioOverriders;
            VoiceDisabledPanel.Enabled = hasAudioOverriders;

            if (hasAudioOverriders)
            {
                var pluginNames = string.Join(", ", ovAudioPlugins.Select(o => o.plugin.Name));

                VoiceDisabledLabel.Text = disableMessage("voice", pluginNames);
                VoiceDisabledPanel.BringToFront();
            }
            else
            {
                VoiceDisabledPanel.SendToBack();
            }
#endif
        }

        // Called from PluginManagement
        internal void OpenSettings(IObservatoryPlugin plugin)
        {
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

        private void PluginSettingsButton_Click(object sender, EventArgs e)
        {
            if (ListedPlugins != null && PluginList.SelectedItems.Count != 0)
            {
                var plugin = ListedPlugins[PluginList.SelectedItems[0]];
                OpenSettings(plugin);
            }
        }

        private void PluginsEnabledStateFromSettings()
        {
            if (ListedPlugins == null) return;

            string pluginsEnabledStr = Properties.Core.Default.PluginsEnabled;
            Dictionary<string, bool>? pluginsEnabled = null;
            if (!string.IsNullOrWhiteSpace(pluginsEnabledStr))
            {
                try
                {
                    pluginsEnabled = JsonSerializer.Deserialize<Dictionary<string, bool>>(pluginsEnabledStr);
                }
                catch
                {
                    // Failed deserialization means bad value, blow it away.
                    Properties.Core.Default.PluginsEnabled = string.Empty;
                    SettingsManager.Save();
                }
            }

            if (pluginsEnabled == null) return;

            foreach (var p in ListedPlugins)
            {
                if (pluginsEnabled.ContainsKey(p.Value.Name) && !pluginsEnabled[p.Value.Name])
                {
                    // Plugin is disabled.
                    p.Key.Checked = false; // This may trigger the listview ItemChecked event.
                    PluginManager.GetInstance.SetPluginEnabled(p.Value, false);
                }
            }
        }

        private void PluginList_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (ListedPlugins == null) return;

            var plugin = ListedPlugins[e.Item];
            var enabled = e.Item.Checked;

            PluginManager.GetInstance.SetPluginEnabled(plugin, enabled);

            if (!loading)
            {
                Dictionary<string, bool> pluginsEnabled = ListedPlugins.ToDictionary(e => e.Value.Name, e => e.Key.Checked);

                Properties.Core.Default.PluginsEnabled = JsonSerializer.Serialize(pluginsEnabled);
                SettingsManager.Save();
                DisableOverriddenNotification();
            }
        }

        private Dictionary<IObservatoryPlugin, SettingsForm> SettingsForms = [];

        private static void PluginExport(IObservatoryPlugin plugin)
        {
            if (plugin != null)
            {
                // Custom export method handled inside ExportCSV
                if (Properties.Core.Default.ExportFormat == 0 || HasCustomExport(plugin))
                    ExportHandler.ExportCSV(plugin);
                else
                    ExportHandler.ExportXlsx(plugin);
            }
        }

        private void PluginClear(IObservatoryPlugin plugin)
        {
            if (plugin != null && plugin.PluginUI.PluginUIType == PluginUI.UIType.Basic)
            {
                plugin.PluginUI.DataGrid.Clear();
            }
        }
    }
}