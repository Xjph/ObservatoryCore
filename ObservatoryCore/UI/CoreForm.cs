using Observatory.Framework;
using Observatory.Framework.Interfaces;
using Observatory.PluginManagement;
using Observatory.Utils;
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
                + "If you like this tool, consider making a one-time donation via PayPal, or ongoing via Patreon!"
                + Environment.NewLine
                + Environment.NewLine
                + "With special thanks to my Patrons:"
                + Environment.NewLine
                + string.Join(Environment.NewLine, _patrons),
            AuthorName = "Vithigar",
            Links = new()
            {
                new AboutLink("github", "https://github.com/Xjph/ObservatoryCore"),
                new AboutLink("Documentation", "https://observatory.xjph.net/"),
                new AboutLink("Donate via Paypal", "https://www.paypal.com/donate/?hosted_button_id=XYQWYQ337TBP4"),
                new AboutLink("Donate via Patreon", "https://www.patreon.com/vithigar"),
            }
        };

        private static string[] _patrons =
        [
            "  Arx",
            "  Doctor Nozimo",
            "  KPTRamius",
            "  Markus H",
            "  McMuttons",
            "  NirevS",
            "  ObediahJoel",
            "  PaganPilot",
            "  pepčok",
            "  Slegnor"
        ];

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

            string version = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version?.ToString(3) ?? "0";
            Text += $" - v{version}";

            themeManager = ThemeManager.GetInstance;
            themeManager.CurrentTheme = Properties.Core.Default.Theme;
            themeManager.RegisterControl(this);

            CreatePluginTabs();
            CreatePluginList();
            RestoreSavedTab();
            RestoreSavedPopouts();
            CheckUpdate();

            LogMonitor.GetInstance.SetLastEventLabel(LastEvent);
            LogMonitor.GetInstance.SetTotalEventLabel(TotalEvents);
        }

        public void FocusPlugin(string pluginShortName)
        {
            var pluginTab = FindTabPageForPlugin(pluginShortName);
            if (pluginTab != null)
            {
                SuspendDrawing(this);
                CoreTabControl.SelectedTab = pluginTab;
                ResumeDrawing(this);
            }
        }

        private void CreatePluginList()
        {
            var pluginList = new PluginList(PluginManager.GetInstance.AllPlugins);

            pluginList.Location = new(0, 0);
            pluginList.Size = CoreTabPanel.Size;
            pluginList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pluginList.AutoScroll = true;
            ThemeManager.GetInstance.RegisterControl(pluginList);
            CoreTabPanel.Controls.Add(pluginList);
        }

        private TabPage? FindTabPageForPlugin(string pluginShortName)
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
                MonitorStatus.Text = "Current Monitor Status: Stopped";
            }
            else
            {
                LogMonitor.GetInstance.Start();
                ToggleMonitorButton.Text = "Stop Monitor";
                MonitorStatus.Text = "Current Monitor Status: Active";
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

        private Dictionary<PluginUIGrid, object> PluginComparer;

        private void SuspendSorting()
        {
            PluginComparer = [];
            foreach (TabPage tab in CoreTabControl.TabPages)
            {
                foreach (var control in tab.Controls)
                {
                    if (control?.GetType() == typeof(PluginUIGrid))
                    {
                        var listView = (PluginUIGrid)control;
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
            FormsManager.OpenAboutForm(_aboutCore);
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
            // Async task to allow UI draw to finish before doing work.
            Task.Run(() =>
            {
                Thread.Sleep(500);
                PluginManager.GetInstance.ObservatoryReady();

                if (Properties.Core.Default.StartReadAll)
                    Invoke(()=>ReadAllButton_Click(ReadAllButton, EventArgs.Empty));

                if (Properties.Core.Default.StartMonitor)
                    Invoke(()=>ToggleMonitorButton_Click(ToggleMonitorButton, EventArgs.Empty));
            });
        }

        private void CoreForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Save location
            Properties.Core.Default.MainWindowPosition = Location;
            Properties.Core.Default.MainWindowSize = Size;
            SettingsManager.Save();

            // Create new collection to iterate while modifying OpenForms
            var openWindows = Application.OpenForms.Cast<Form>().ToList();

            List<string> openPopouts = [];

            // Call close event of popouts so locations get saved
            foreach (Form window in openWindows)
            {
                if (window != this)
                {
                    if (window is PopoutForm)
                    {
                        openPopouts.Add(window.Text);
                    }
                    window.Close();
                }
            }

            Properties.Core.Default.OpenPopouts = JsonSerializer.Serialize(openPopouts);
            SettingsManager.Save();
        }

        private void CoreForm_Load(object sender, EventArgs e)
        {
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
            List<string> tabOrder;
            try
            {
                tabOrder = JsonSerializer.Deserialize<List<string>>(Properties.Core.Default.TabOrder) ?? [];
            }
            catch
            {
                tabOrder = [];
            }

            int currentIndex = 0;

            foreach (var tabName in tabOrder)
            {
                var tab = FindTabPageForPlugin(tabName);
                if (tab != null && CoreTabControl.TabPages.Count > currentIndex)
                {
                    CoreTabControl.SwapTabs(tab, CoreTabControl.TabPages[currentIndex]);
                    currentIndex++;
                }
            }

            CoreTabControl.SelectedIndex = Properties.Core.Default.LastTabIndex < CoreTabControl.TabPages.Count
                ? Properties.Core.Default.LastTabIndex
                : 0;
            CoreTabControl_SelectedIndexChanged(CoreTabControl, EventArgs.Empty);
        }

        private void RestoreSavedPopouts()
        {
            try
            {
                var openPopouts = Properties.Core.Default.OpenPopouts;
                List<string>? popoutsToRestore = JsonSerializer.Deserialize<List<string>>(openPopouts);
                List<IObservatoryWorker> plugins = PluginManager.GetInstance.AllUIPlugins;

                foreach (var name in popoutsToRestore ?? [])
                {
                    var pluginTabKV = pluginList.Where(p => p.Value.Name == name).FirstOrDefault();
                    if (pluginTabKV.Key != null)
                    {
                        FormsManager.OpenPluginPopoutForm(pluginTabKV.Value, pluginTabKV.Key);
                    }
                }
            }
            catch
            {
                // do nothing
            }

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
                    if (CoreTabControl.GetTabRect(i).Contains(e.Location))
                    {
                        var pluginPanel = CoreTabControl.TabPages[i];
                        // The core tab may not be at index 0 and is not contained in the plugin list.
                        if (pluginList.ContainsKey(pluginPanel))
                        {
                            var clickedPlugin = pluginList[pluginPanel];

                            PluginContextMenu pluginContextMenu = new(clickedPlugin, pluginPanel);
                            pluginContextMenu.Show((Control)sender, e.Location);
                        }
                        else
                        {
                            // Assume the core tab for the moment.
                            CoreContextMenu coreContextMenu = new(_aboutCore, OpenCoreSettings, OpenPluginsFolder);
                            coreContextMenu.Show((Control)sender, e.Location);
                        }
                    }
                }
            }
        }
        #endregion

        private CoreSettings? _coreSettings;

        public void OpenCoreSettings()
        {
            if (_coreSettings == null || _coreSettings.IsDisposed)
            {
                _coreSettings = new();
                ThemeManager.GetInstance.RegisterControl(_coreSettings);
            }

            _coreSettings.Show();
            _coreSettings.Activate();
        }

        public void OpenPluginsFolder()
        {
            var pluginDir = Application.StartupPath + "plugins";

            if (!Directory.Exists(pluginDir))
            {
                Directory.CreateDirectory(pluginDir);
            }

            var fileExplorerInfo = new ProcessStartInfo() { FileName = pluginDir, UseShellExecute = true };
            Process.Start(fileExplorerInfo);
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            OpenCoreSettings();
        }

        private void PluginFolderButton_Click(object sender, EventArgs e)
        {
            OpenPluginsFolder();
        }
    }
}