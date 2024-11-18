using Observatory.Framework;
using Observatory.Framework.Interfaces;
using Observatory.PluginManagement;
using Observatory.Utils;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace Observatory.UI
{
    public partial class CoreForm : Form
    {
        private AboutInfo _aboutCore = new AboutInfo()
        {
            FullName = "Elite Observatory Core",
            ShortName = "Core",
            Description = "A tool for reading/monitoring Elite Dangerous journals for interesting objects."
                + Environment.NewLine
                + Environment.NewLine
                + "If you like this tool, consider making a one-time donation via PayPal, or ongoing via Patreon!",
            AuthorName = "Vithigar",
            Links = new()
            {
                new AboutLink("github", "https://github.com/Xjph/ObservatoryCore"),
                new AboutLink("Documentation", "https://observatory.xjph.net/"),
                new AboutLink("Donate via Paypal", "https://www.paypal.com/donate/?hosted_button_id=XYQWYQ337TBP4"),
                new AboutLink("Donate via Patreon", "https://www.patreon.com/vithigar"),
            }
        };

        private readonly ThemeManager themeManager;

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);
        private const int WM_SETREDRAW = 11;
        private static void SuspendDrawing(Control control)
        {
            if (SendMessage(control.Handle, WM_SETREDRAW, false, 0) != 0)
                throw new Exception("Unexpected error when suspending form draw events.");
        }

        private static void ResumeDrawing(Control control)
        {
            if (SendMessage(control.Handle, WM_SETREDRAW, true, 0) != 0)
                throw new Exception("Unexpected error when resuming form draw events.");

            control.Refresh();
        }

        public CoreForm()
        {
            DoubleBuffered = true;
            InitializeComponent();

            PopulateDropdownOptions();
            PopulateNativeSettings();

            ColourListHeader(ref PluginList, Color.DarkSlateGray, Color.LightGray);
            PopulatePluginList();
            FitColumns();
            string version = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version?.ToString(3) ?? "0";
            Text += $" - v{version}";

            DisableOverriddenNotification();

            themeManager = ThemeManager.GetInstance;
            themeManager.CurrentTheme = Properties.Core.Default.Theme;
            themeManager.RegisterControl(this);

            foreach (var theme in themeManager.GetThemes)
            {
                ThemeDropdown.Items.Add(theme);
            }
            ThemeDropdown.SelectedItem = themeManager.CurrentTheme;
            CreatePluginTabs();
            RestoreSavedTab();
            CheckUpdate();
        }

        public void FocusPlugin(string pluginShortName)
        {
            var pluginTab = FindMenuItemForPlugin(pluginShortName);
            if (pluginTab != null)
            {
                SuspendDrawing(this);
                CoreTabControl.SelectedTab = pluginTab;
                ResumeDrawing(this);
            }
        }

        private TabPage? FindMenuItemForPlugin(string pluginShortName)
        {
            foreach (TabPage tab in CoreTabControl.TabPages)
            {
                if (tab.Text == pluginShortName)
                {
                    return tab;
                }
            }
            return null;
        }

        private readonly Dictionary<TabPage, IObservatoryPlugin> pluginList = [];

        private void ToggleMonitorButton_Click(object sender, EventArgs e)
        {
            if ((LogMonitor.GetInstance.CurrentState & Framework.LogMonitorState.Realtime) == Framework.LogMonitorState.Realtime)
            {
                LogMonitor.GetInstance.Stop();
                ToggleMonitorButton.Text = "Start Monitor";
            }
            else
            {
                LogMonitor.GetInstance.Start();
                ToggleMonitorButton.Text = "Stop Monitor";
            }
        }

        private static void ColourListHeader(ref NoHScrollList list, Color backColor, Color foreColor)
        {
            list.OwnerDraw = true;

            list.DrawColumnHeader +=
                new DrawListViewColumnHeaderEventHandler
                (
                    (sender, e) => HeaderDraw(sender, e, backColor, foreColor)
                );
            list.DrawItem += new DrawListViewItemEventHandler(BodyDraw);
        }

        private static void HeaderDraw(object? _, DrawListViewColumnHeaderEventArgs e, Color backColor, Color foreColor)
        {
            using (SolidBrush backBrush = new(backColor))
            {
                e.Graphics.FillRectangle(backBrush, e.Bounds);
            }

            using (Pen borderBrush = new(Color.Black))
            {
                e.Graphics.DrawLine(borderBrush, e.Bounds.Left, e.Bounds.Top, e.Bounds.Left, e.Bounds.Bottom);
                e.Graphics.DrawLine(borderBrush, e.Bounds.Right, e.Bounds.Top, e.Bounds.Right, e.Bounds.Bottom);
            }

            if (e.Font != null && e.Header != null)
                using (SolidBrush foreBrush = new(foreColor))
                {
                    var format = new StringFormat
                    {
                        Alignment = (StringAlignment)e.Header.TextAlign,
                        LineAlignment = StringAlignment.Center
                    };

                    var paddedBounds = new Rectangle(e.Bounds.X + 2, e.Bounds.Y + 2, e.Bounds.Width - 4, e.Bounds.Height - 4);

                    e.Graphics.DrawString(e.Header?.Text, e.Font, foreBrush, paddedBounds, format);
                }
        }

        private static void BodyDraw(object? _, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void PluginList_Resize(object sender, EventArgs e)
        {
            FitColumns();
        }

        private void FitColumns()
        {
            // Might be uninitialised if user hasn't visited the Core tab this session.
            if (PluginList.Columns.Count > 0)
            {
                int totalWidth = 0;
                foreach (ColumnHeader col in PluginList.Columns)
                    totalWidth += col.Width;

                PluginList.Columns[3].Width += PluginList.Width - totalWidth; // - SystemInformation.VerticalScrollBarWidth;
            }
        }

        private void ReadAllButton_Click(object sender, EventArgs e)
        {
            var readAllDialogue = new ReadAllForm();
            ThemeManager.GetInstance.RegisterControl(readAllDialogue);
            readAllDialogue.StartPosition = FormStartPosition.Manual;
            readAllDialogue.Location = Point.Add(Location, new Size(100, 100));
            SuspendDrawing(this);
            SuspendSorting();
            readAllDialogue.ShowDialog();
            ResumeSorting();
            ResumeDrawing(this);
        }

        private Dictionary<PluginListView, object> PluginComparer;

        private void SuspendSorting()
        {
            PluginComparer = [];
            foreach (TabPage tab in CoreTabControl.TabPages)
            {
                foreach (var control in tab.Controls)
                {
                    if (control?.GetType() == typeof(PluginListView))
                    {
                        var listView = (PluginListView)control;
                        PluginComparer.Add(listView, listView.ListViewItemSorter);
                        listView.ListViewItemSorter = null;
                    }
                }
            }
        }

        private void ResumeSorting()
        {
            if (PluginComparer.Count != 0)
                foreach (var panel in PluginComparer.Keys)
                {
                    panel.ListViewItemSorter = (IObservatoryComparer)PluginComparer[panel];
                }
            PluginComparer?.Clear();
        }

        private NativeNotification.NativePopup? nativePopup;
        private NativeNotification.NativeVoice? nativeVoice;

        private void CheckUpdate()
        {
            try
            {
                string releasesResponse;

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://api.github.com/repos/xjph/ObservatoryCore/releases"),
                    Headers = { { "User-Agent", "Xjph/ObservatoryCore" } }
                };

                releasesResponse = Utils.HttpClient.SendRequest(request).Content.ReadAsStringAsync().Result;

                if (!string.IsNullOrEmpty(releasesResponse))
                {
                    var releases = System.Text.Json.JsonDocument.Parse(releasesResponse).RootElement.EnumerateArray();

                    Version? latestVersion = null;
                    string latestVersionUrl = string.Empty;

                    foreach (var release in releases)
                    {
                        var tag = release.GetProperty("tag_name").ToString();
                        var verstrings = tag[1..].Split('.');
                        var ver = verstrings.Select(verString => { _ = int.TryParse(verString, out int ver); return ver; }).ToArray();
                        if (ver.Length == 3 || ver.Length == 4)
                        {
                            Version githubVersion = new(ver[0], ver[1], ver[2], ver.Length == 3 ? 0 : ver[3]);
                            if (latestVersion == null || githubVersion > latestVersion)
                            {
                                latestVersion = githubVersion;
                                latestVersionUrl = release.GetProperty("html_url").ToString();
                            }
                        }
                    }

                    if (latestVersion > System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version)
                    {
                        UpdateLink.Visible = true;
                        UpdateLink.Enabled = true;
                        UpdateLink.LinkClicked += (_, _) =>
                        {
                            OpenURL(latestVersionUrl);
                        };
                    }
                }
            }
            catch
            {
                // log?
            }
        }

        private void AboutCore_Click(object sender, EventArgs e)
        {
            OpenAbout(_aboutCore);
        }

        // Also used for plugins.
        internal void OpenAbout(AboutInfo aboutInfo)
        {
            if (aboutInfo != null)
            {
                var aboutForm = new AboutForm(aboutInfo);
                ThemeManager.GetInstance.RegisterControl(aboutForm);
                aboutForm.Show();
            }
        }

        private static void OpenURL(string url)
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            // Find currently selected item for export
            if (CoreTabControl.SelectedTab != null && pluginList.ContainsKey(CoreTabControl.SelectedTab))
            {
                var selectedItem = pluginList[CoreTabControl.SelectedTab];
                PluginExport(selectedItem);
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            if (CoreTabControl.SelectedTab != null && pluginList.ContainsKey(CoreTabControl.SelectedTab))
            {
                var selectedItem = pluginList[CoreTabControl.SelectedTab];
                PluginClear(selectedItem);
            }
        }

        private void CoreForm_Shown(object sender, EventArgs e)
        {
            PluginManager.GetInstance.ObservatoryReady();


            if (Properties.Core.Default.StartReadAll)
                ReadAllButton_Click(ReadAllButton, EventArgs.Empty);

            if (Properties.Core.Default.StartMonitor)
                ToggleMonitorButton_Click(ToggleMonitorButton, EventArgs.Empty);
        }

        private void PluginFolderButton_Click(object sender, EventArgs e)
        {
            var pluginDir = Application.StartupPath + "plugins";

            if (!Directory.Exists(pluginDir))
            {
                Directory.CreateDirectory(pluginDir);
            }

            var fileExplorerInfo = new ProcessStartInfo() { FileName = pluginDir, UseShellExecute = true };
            Process.Start(fileExplorerInfo);
        }

        private void CoreForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Save location
            Properties.Core.Default.MainWindowPosition = Location;
            Properties.Core.Default.MainWindowSize = Size;
            Properties.Core.Default.CoreSplitterDistance = CoreSplitter.SplitterDistance;
            SettingsManager.Save();
        }

        private void CoreForm_Load(object sender, EventArgs e)
        {
            CoreSplitter.SplitterDistance = Math.Clamp(
                Properties.Core.Default.CoreSplitterDistance,
                20,
                Math.Max(CoreSplitter.Height - 20, 20)); // Edge case exception.
            var savedLocation = Properties.Core.Default.MainWindowPosition;
            var savedSize = Properties.Core.Default.MainWindowSize;

            // Ensure we're on screen
            bool onscreen = false;
            // Shrink the bounds slightly to allow window to touch edges.
            var formBounds = new Rectangle(
                savedLocation.X + 20,
                savedLocation.Y + 20,
                // Should never be this small, preventing edge case exception
                Math.Max(savedSize.Width - 40, 1),
                Math.Max(savedSize.Height - 40, 1));
            foreach (var screen in Screen.AllScreens)
            {
                onscreen = onscreen || screen.WorkingArea.Contains(formBounds);
            }

            if (onscreen)
            {
                Location = savedLocation;
                Size = savedSize;
            }
        }

        private void RestoreSavedTab()
        {
            CoreTabControl.SelectedIndex = Properties.Core.Default.LastTabIndex < CoreTabControl.TabPages.Count
                ? Properties.Core.Default.LastTabIndex
                : 0;
            CoreTabControl_SelectedIndexChanged(CoreTabControl, EventArgs.Empty);
        }

        private void CoreTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedTab = CoreTabControl.SelectedTab;
            if (selectedTab != null)
            {
                pluginList.TryGetValue(selectedTab, out var plugin);

                // Named bools for clarity
                bool notCoreTab = selectedTab != CoreTabPage;
                bool hasExportMethod = notCoreTab && HasCustomExport(plugin);
                bool isBasicUI = notCoreTab && plugin?.PluginUI.PluginUIType == Framework.PluginUI.UIType.Basic;

                bool canExport = isBasicUI || hasExportMethod;
                bool canClear = isBasicUI;
                ExportButton.Enabled = canExport;
                ClearButton.Enabled = canClear;
                Properties.Core.Default.LastTabIndex = CoreTabControl.SelectedIndex;
                SettingsManager.Save();
            }
        }

        private static bool HasCustomExport(IObservatoryPlugin? plugin) => ((Delegate)plugin.ExportContent).Method != typeof(IObservatoryPlugin).GetMethod("ExportContent");

        private void CoreForm_ResizeBegin(object sender, EventArgs e)
        {
            SuspendLayout();
        }

        private void CoreForm_ResizeEnd(object sender, EventArgs e)
        {
            ResumeLayout();
        }

        #region Plugins
        private Dictionary<ListViewItem, IObservatoryPlugin>? ListedPlugins;
        private bool loading = true; // Suppress settings updates due to initializing the listview.

        private void PopulatePluginList()
        {
            ListedPlugins = new();

            foreach (var (plugin, signed) in PluginManager.GetInstance.EnabledWorkerPlugins)
            {
                if (!ListedPlugins.ContainsValue(plugin))
                {
                    var aboutInfo = plugin.AboutInfo;
                    ListViewItem item = new ListViewItem(new[]
                    {
                        aboutInfo?.FullName ?? plugin.Name,
                        aboutInfo?.AuthorName ?? string.Empty,
                        "Worker",
                        plugin.Version,
                        PluginStatusString(signed)
                    });
                    ListedPlugins.Add(item, plugin);
                    var lvItem = PluginList.Items.Add(item);
                    lvItem.Checked = true; // Start with enabled, let settings disable things.
                }
            }

            foreach (var (plugin, signed) in PluginManager.GetInstance.EnabledNotifyPlugins)
            {
                if (!ListedPlugins.ContainsValue(plugin))
                {
                    var aboutInfo = plugin.AboutInfo;
                    ListViewItem item = new ListViewItem(new[]
                    {
                        aboutInfo?.FullName ?? plugin.Name,
                        aboutInfo?.AuthorName ?? string.Empty,
                        "Notifier",
                        plugin.Version,
                        PluginStatusString(signed)
                    });
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
                    return "";

                case PluginManager.PluginStatus.SigCheckDisabled:
                    return "Signature Checks Disabled";

                case PluginManager.PluginStatus.AllowedSignature:
                    return "Signed, allowed by user";

                case PluginManager.PluginStatus.SignedThirdParty:
                    return "Signed by a trusted third-party";

                default:
                    return string.Empty;
            }
        }

        private void CreatePluginTabs()
        {
            var uiPlugins = PluginManager.GetInstance.AllUIPlugins;
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
                    SettingsManager.Save();
                }
            }

            PluginHelper.CreatePluginTabs(CoreTabControl, uiPlugins, pluginList, columnSizing ?? []);
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

        internal void AboutPluginButton_Click(object sender, EventArgs e)
        {
            if (ListedPlugins != null && PluginList.SelectedItems.Count != 0)
            {
                var plugin = ListedPlugins[PluginList.SelectedItems[0]];
                OpenAbout(plugin.AboutInfo);
            }
        }

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

        private void PluginDataDirButton_Click(object sender, EventArgs e)
        {
            // Default to the root plugin data dir.
            string storageDir = PluginManager.GetInstance.Core.GetStorageFolderForPlugin();

            if (ListedPlugins != null && PluginList.SelectedItems.Count != 0)
            {
                var plugin = ListedPlugins[PluginList.SelectedItems[0]];
                storageDir = PluginManager.GetInstance.Core.GetStorageFolderForPlugin(plugin.GetType().Assembly.GetName().Name ?? "");

            }

            if (string.IsNullOrWhiteSpace(storageDir) || !Directory.Exists(storageDir))
            {
                return; // Unexpected; but do nothing.
            }

            var fileExplorerInfo = new ProcessStartInfo() { FileName = storageDir, UseShellExecute = true };
            Process.Start(fileExplorerInfo);
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

        private void CoreTabControl_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                for (int i = 0; i < pluginList.Count; i++)
                {
                    if (CoreTabControl.GetTabRect(i + 1).Contains(e.Location))
                    {
                        var pluginPanel = CoreTabControl.TabPages[i + 1];
                        var clickedPlugin = pluginList[pluginPanel];

                        ContextMenuStrip pluginContextMenu = new();
                        ToolStripMenuItem popout = new()
                        {
                            Text = $"Popout {clickedPlugin.ShortName}"
                        };
                        ToolStripMenuItem settings = new()
                        {
                            Text = $"{clickedPlugin.ShortName} Settings"
                        };
                        pluginContextMenu.Items.Add(popout);
                        pluginContextMenu.Items.Add(settings);

                        pluginContextMenu.ItemClicked += (o, e) =>
                        {
                            if (e.ClickedItem == popout)
                            {
                                var popoutForm = new PopoutForm(pluginPanel, clickedPlugin.Name);
                                ThemeManager.GetInstance.RegisterControl(popoutForm);
                                popoutForm.Show();
                            }
                        };

                        pluginContextMenu.Show((Control)sender, e.Location);

                    }
                }
            }
        }
        #endregion

        #region Settings
        private void ColourButton_Click(object _, EventArgs e)
        {
            var selectionResult = PopupColour.ShowDialog();
            if (selectionResult == DialogResult.OK)
            {
                ColourButton.BackColor = PopupColour.Color;
                Properties.Core.Default.NativeNotifyColour = (uint)PopupColour.Color.ToArgb();
                SettingsManager.Save();
            }
        }

        private void PopupCheckbox_CheckedChanged(object _, EventArgs e)
        {
            Properties.Core.Default.NativeNotify = PopupCheckbox.Checked;
            SettingsManager.Save();
        }

        private void DurationSpinner_ValueChanged(object _, EventArgs e)
        {
            Properties.Core.Default.NativeNotifyTimeout = (int)DurationSpinner.Value;
            SettingsManager.Save();
        }

        private void ScaleSpinner_ValueChanged(object _, EventArgs e)
        {
            Properties.Core.Default.NativeNotifyScale = (int)ScaleSpinner.Value;
            SettingsManager.Save();
        }

        private void FontScaleSpinner_ValueChanged(object sender, EventArgs e)
        {
            Properties.Core.Default.NativeNotifyFontScale = (int)FontScaleSpinner.Value;
            SettingsManager.Save();
        }

        private void FontDropdown_SelectedIndexChanged(object _, EventArgs e)
        {
            Properties.Core.Default.NativeNotifyFont = FontDropdown.SelectedItem?.ToString();
            SettingsManager.Save();
        }

        private void CornerDropdown_SelectedIndexChanged(object _, EventArgs e)
        {
            Properties.Core.Default.NativeNotifyCorner = CornerDropdown.SelectedIndex;
            SettingsManager.Save();
        }

        private void DisplayDropdown_SelectedIndexChanged(object _, EventArgs e)
        {
            Properties.Core.Default.NativeNotifyScreen = DisplayDropdown.SelectedIndex - 1;
            SettingsManager.Save();
        }

        private void PopupTransparentCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Core.Default.NativeNotifyTransparent = PopupTransparentCheckBox.Checked;
            SettingsManager.Save();
        }

        private void AudioVolumeSlider_Scroll(object _, EventArgs e)
        {
            Properties.Core.Default.VoiceVolume = Math.Clamp(AudioVolumeSlider.Value, 0, 100);
            Properties.Core.Default.AudioVolume = Math.Clamp(AudioVolumeSlider.Value / 100.0f, 0.0f, 1.0f);
            SettingsManager.Save();
        }

        private void VoiceSpeedSlider_Scroll(object _, EventArgs e)
        {
            Properties.Core.Default.VoiceRate = VoiceSpeedSlider.Value;
            SettingsManager.Save();
        }

        private void VoiceCheckbox_CheckedChanged(object _, EventArgs e)
        {
            Properties.Core.Default.VoiceNotify = VoiceCheckbox.Checked;
            SettingsManager.Save();
        }

        private void VoiceDropdown_SelectedIndexChanged(object _, EventArgs e)
        {
            Properties.Core.Default.VoiceSelected = VoiceDropdown.SelectedItem?.ToString();
            SettingsManager.Save();
        }

        private void AudioDeviceDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AudioDeviceDropdown.SelectedItem == null)
                // Shouldn't happen but default to the Windows built-in device (always exists at -1)
                Properties.Core.Default.AudioDevice = AudioHandler.GetFirstDevice();
            else
                // Stores the current selected device
                Properties.Core.Default.AudioDevice = AudioDeviceDropdown.SelectedItem.ToString();
            SettingsManager.Save();
        }
        private void AudioDeviceDropdown_Focused(object sender, EventArgs e)
        {
            AudioDeviceDropdown.Items.Clear();
            foreach (var device in AudioHandler.GetDevices())
                AudioDeviceDropdown.Items.Add(device);
            AudioDeviceDropdown.SelectedIndex = AudioHandler.GetDeviceIndex(Properties.Core.Default.AudioDevice);
        }

        private void PopulateDropdownOptions()
        {
            var fonts = new System.Drawing.Text.InstalledFontCollection().Families;
            FontDropdown.Items.AddRange(fonts.Select(f => f.Name).ToArray());

            DisplayDropdown.Items.Add("Primary");
            if (Screen.AllScreens.Length > 1)
                for (int i = 0; i < Screen.AllScreens.Length; i++)
                    DisplayDropdown.Items.Add((i + 1).ToString());
#if !PROTON
            var voices = new System.Speech.Synthesis.SpeechSynthesizer().GetInstalledVoices();
            foreach (var voice in voices.Select(v => v.VoiceInfo.Name))
                VoiceDropdown.Items.Add(voice);

            foreach (var device in AudioHandler.GetDevices())
                AudioDeviceDropdown.Items.Add(device);
            var deviceIndex = AudioHandler.GetDeviceIndex(Properties.Core.Default.AudioDevice);
            // Select first device if not found.
            AudioDeviceDropdown.SelectedIndex = Math.Max(0, deviceIndex);
#endif
        }

        private void PopulateNativeSettings()
        {
            var settings = Properties.Core.Default;

            TryLoadSetting(DisplayDropdown, "SelectedIndex", settings.NativeNotifyScreen + 1, 0);
            TryLoadSetting(CornerDropdown, "SelectedIndex", settings.NativeNotifyCorner, 0);
            TryLoadSetting(FontDropdown, "SelectedItem", settings.NativeNotifyFont);
            TryLoadSetting(ScaleSpinner, "Value", (decimal)Math.Clamp(settings.NativeNotifyScale, 1, 500), 100);
            TryLoadSetting(DurationSpinner, "Value", (decimal)Math.Clamp(settings.NativeNotifyTimeout, 100, 60000), 5000);
            TryLoadSetting(ColourButton, "BackColor", Color.FromArgb((int)settings.NativeNotifyColour));
            TryLoadSetting(PopupCheckbox, "Checked", settings.NativeNotify);
            TryLoadSetting(AudioVolumeSlider, "Value", Math.Clamp(settings.VoiceVolume, 0, 100), 100); // Also controls AudioVolume setting
            TryLoadSetting(VoiceSpeedSlider, "Value", Math.Clamp(settings.VoiceRate, -10, 10));
            TryLoadSetting(VoiceDropdown, "SelectedItem", settings.VoiceSelected);
            TryLoadSetting(VoiceCheckbox, "Checked", settings.VoiceNotify);
            TryLoadSetting(LabelJournalPath, "Text", LogMonitor.GetJournalFolder().FullName);
            TryLoadSetting(StartMonitorCheckbox, "Checked", settings.StartMonitor);
            TryLoadSetting(StartReadallCheckbox, "Checked", settings.StartReadAll);
            TryLoadSetting(ExportFormatDropdown, "SelectedIndex", settings.ExportFormat);
            TryLoadSetting(PopupTransparentCheckBox, "Checked", settings.NativeNotifyTransparent);
            TryLoadSetting(FontScaleSpinner, "Value", (decimal)Math.Clamp(settings.NativeNotifyFontScale, 1, 500), 100);

#if PROTON
            VoiceCheckbox.Checked = false;
            VoiceCheckbox.Enabled = false;
            VoiceSpeedSlider.Enabled = false;
            VoiceDropdown.Enabled = false;
            VoiceTestButton.Enabled = false;
            VoiceDisabledPanel.Visible = true;
            VoiceDisabledLabel.Text = "Native voice notifications not available in this build.";
            VoiceDisabledPanel.BringToFront();
#endif
#if !DEBUG
            CoreConfigFolder.Visible = false;
#endif
        }

        static private void TryLoadSetting(Control control, string property, object newValue, object? defaultValue = null)
        {
            try
            {
                (control.GetType().GetProperty(property)?.GetSetMethod())?.Invoke(control, [newValue]);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to load all settings ({control.Name}), some values may have been cleared.\r\nError: {ex.InnerException?.Message}");
                if (defaultValue != null)
                    (control.GetType().GetProperty(property)?.GetSetMethod())?.Invoke(control, [defaultValue]);
            }
        }

        private void TestButton_Click(object sender, EventArgs e)
        {
            NotificationArgs args = new()
            {
                Title = "Test Popup Notification",
                Detail = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec at elit maximus, ornare dui nec, accumsan velit. Vestibulum fringilla elit."
            };

            nativePopup ??= new Observatory.NativeNotification.NativePopup();

            nativePopup.InvokeNativeNotification(args);
        }


        private void VoiceTestButton_Click(object sender, EventArgs e)
        {
            NotificationArgs args = new()
            {
                Title = "Test Voice Notification",
                Detail = "This is a test of native voice notifications."
            };
            AudioHandler audioHandler = new AudioHandler();
            nativeVoice ??= new(audioHandler);

            nativeVoice.AudioHandlerEnqueue(args);
        }

        private void ThemeDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            themeManager.CurrentTheme = ThemeDropdown.SelectedItem?.ToString() ?? themeManager.CurrentTheme;
            Properties.Core.Default.Theme = themeManager.CurrentTheme;
            foreach (var (plugin, _) in PluginManager.GetInstance.AllUIPlugins)
            {
                plugin.ThemeChanged(themeManager.CurrentTheme, themeManager.CurrentThemeDetails);
            }
            SettingsManager.Save();
        }

        private void ButtonAddTheme_Click(object sender, EventArgs e)
        {
            var fileBrowse = new OpenFileDialog()
            {
                Filter = "Elite Observatory Theme (*.eot)|*.eot|All files (*.*)|*.*"
            };
            var result = fileBrowse.ShowDialog();
            if (result == DialogResult.OK)
            {
                try
                {
                    var fileContent = File.ReadAllText(fileBrowse.FileName);
                    var themeName = themeManager.AddTheme(fileContent);
                    ThemeDropdown.Items.Add(themeName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        ex.Message,
                        "Error Reading Theme",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void LabelJournalPath_DoubleClick(object sender, EventArgs e)
        {
            var folderBrowse = new FolderBrowserDialog()
            {
                Description = "Select Elite Dangerous Journal Location",
                InitialDirectory = LogMonitor.GetJournalFolder().FullName,
                UseDescriptionForTitle = true
            };
            var result = folderBrowse.ShowDialog(this);

            Properties.Core.Default.JournalFolder =
                result == DialogResult.OK
                ? folderBrowse.SelectedPath
                : string.Empty;

            SettingsManager.Save();
            LabelJournalPath.Text = LogMonitor.GetJournalFolder().FullName;
        }

        private void StartMonitorCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Core.Default.StartMonitor = StartMonitorCheckbox.Checked;
            SettingsManager.Save();
        }

        private void StartReadallCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Core.Default.StartReadAll = StartReadallCheckbox.Checked;
            SettingsManager.Save();
        }

        private void ExportFormatDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Core.Default.ExportFormat = ExportFormatDropdown.SelectedIndex;
            SettingsManager.Save();
        }
        private void CoreConfigFolder_Click(object sender, EventArgs e)
        {
#if PORTABLE
            string? observatoryLocation = System.Diagnostics.Process.GetCurrentProcess()?.MainModule?.FileName;
            var configDir = new FileInfo(observatoryLocation ?? String.Empty).DirectoryName;
#else
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            var fileInfo = new FileInfo(config.FilePath);
            var configDir = fileInfo.DirectoryName;
#endif

            if (string.IsNullOrWhiteSpace(configDir) || !Directory.Exists(configDir))
            {
                return;
            }

            var fileExplorerInfo = new ProcessStartInfo() { FileName = configDir, UseShellExecute = true };
            Process.Start(fileExplorerInfo);
        }
        #endregion


        private CoreSettings _coreSettings = new();

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            if (_coreSettings == null || _coreSettings.IsDisposed)
            {
                _coreSettings = new();
            }
            ThemeManager.GetInstance.RegisterControl(_coreSettings);

            _coreSettings.Show();
            _coreSettings.Activate();
        }
    }
}