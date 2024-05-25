using Observatory.PluginManagement;
using Observatory.Framework.Interfaces;
using System.Text.Json;
using Observatory.Framework;
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
                        
            foreach (var (plugin, signed) in PluginManager.GetInstance.workerPlugins)
            {
                if (!ListedPlugins.ContainsValue(plugin))
                {
                    
                    ListViewItem item = new ListViewItem(new[] { plugin.Name, "Worker", plugin.Version, PluginStatusString(signed) });
                    ListedPlugins.Add(item, plugin);
                    var lvItem = PluginList.Items.Add(item);
                    lvItem.Checked = true; // Start with enabled, let settings disable things.
                }
            }

            foreach (var (plugin, signed) in PluginManager.GetInstance.notifyPlugins)
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

            var disableMessage = (string type, string plugin) 
                => $"Native {type} notifications overridden by \"{plugin}\".\r\n"
                + "Use plugin settings to configure.";

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

                var pluginNames = string.Join(", ", ovPopupPlugins.Select(o => o.plugin.Name));

                PopupDisabledPanel.Visible = true;
                PopupDisabledPanel.Enabled = true;
                PopupDisabledLabel.Text = disableMessage("popup", pluginNames);
                PopupDisabledPanel.BringToFront();

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

                var pluginNames = string.Join(", ", ovAudioPlugins.Select(o => o.plugin.Name));

                VoiceDisabledPanel.Visible = true;
                VoiceDisabledPanel.Enabled = true;
                VoiceDisabledLabel.Text = disableMessage("voice", pluginNames);
                VoiceDisabledPanel.BringToFront();
                
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

            return maxWidth + 25;
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
                    Properties.Core.Default.Save();
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
                Properties.Core.Default.Save();
            }
        }

        private Dictionary<IObservatoryPlugin, SettingsForm> SettingsForms = [];

        private void PluginExport(string pluginShortName)
        {
            // TODO: Allow custom
            string delimiter = "\t";

            var plugin = ListedPlugins?.Where(list => list.Value.ShortName == pluginShortName).FirstOrDefault().Value;
            if (plugin != null)
            {
                string filetype = "csv";
                byte[] fileContent = plugin.ExportContent(delimiter, ref filetype);
                if (fileContent == null)
                {
                    if (plugin.PluginUI.PluginUIType == PluginUI.UIType.Basic)
                    {
                        StringBuilder exportString = new();
                        Panel pluginUI = (Panel)plugin.PluginUI.UI;
                        ListView pluginGrid = (ListView)pluginUI.Controls[0];
                        
                        foreach (ColumnHeader column in pluginGrid.Columns)
                        {
                            exportString.Append(column.Text + delimiter);
                        }
                        exportString.AppendLine();

                        foreach (ListViewItem row in pluginGrid.Items)
                        {
                            foreach (ListViewItem.ListViewSubItem item in row.SubItems)
                            {
                                exportString.Append(item.Text + delimiter);
                            }
                            exportString.AppendLine();
                        }
                        fileContent = Encoding.UTF8.GetBytes(exportString.ToString());
                    }
                    else
                    {
                        MessageBox.Show(
                            $"Plugin {plugin.Name} does not use a basic data grid and does not provide an ExportContent method.",
                            "Cannot Export",
                            MessageBoxButtons.OK);
                        return;
                    }
                }
                SaveFileDialog saveAs = new()
                {
                    Title = plugin.Name + " Export",
                    Filter = filetype == "csv"
                    ? "Tab-separated values (*.csv)|*.csv"
                    : $"Plugin-specified file type (*.{filetype})|*.{filetype}",
                    DefaultExt = filetype,
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    FileName = $"Export-{plugin.ShortName}-{DateTime.Now:yyyy-MM-ddTHHmm}.{filetype}"
                };
                var result = saveAs.ShowDialog();
                if (result == DialogResult.OK)
                {
                    File.WriteAllBytes(saveAs.FileName, fileContent);
                }
            }
        }
    }
}