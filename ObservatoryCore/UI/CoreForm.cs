using Observatory.Framework.Interfaces;
using Observatory.PluginManagement;
using Observatory.Utils;

namespace Observatory.UI
{
    public partial class CoreForm : Form
    {
        private Dictionary<object, Panel> uiPanels;

        public CoreForm()
        {
            InitializeComponent();

            PopulateDropdownOptions();
            PopulateNativeSettings();

            ColourListHeader(ref PluginList, Color.DarkSlateGray, Color.LightGray);
            PopulatePluginList();
            FitColumns();
            string version = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "0";
            Text += $" - v{version}";
            CoreMenu.SizeChanged += CoreMenu_SizeChanged;
            uiPanels = new();
            uiPanels.Add(coreToolStripMenuItem, CorePanel);
            pluginList = new Dictionary<string, ToolStripMenuItem>();
            CreatePluginTabs();
            CreatePluginSettings();
            CoreMenu.ItemClicked += CoreMenu_ItemClicked;

            PreCollapsePanels();
        }

        private void PreCollapsePanels()
        {
            AdjustPanelsBelow(VoiceSettingsPanel, AdjustmentDirection.Up);
            AdjustPanelsBelow(PopupSettingsPanel, AdjustmentDirection.Up);
        }

        private void PopulateDropdownOptions()
        {
            var fonts = new System.Drawing.Text.InstalledFontCollection().Families;
            FontDropdown.Items.AddRange(fonts.Select(f => f.Name).ToArray());

            DisplayDropdown.Items.Add("Primary");
            if (Screen.AllScreens.Length > 1)
                for (int i = 0; i < Screen.AllScreens.Length; i++)
                    DisplayDropdown.Items.Add((i + 1).ToString());

            var voices = new System.Speech.Synthesis.SpeechSynthesizer().GetInstalledVoices();
            foreach (var voice in voices.Select(v => v.VoiceInfo.Name))
                VoiceDropdown.Items.Add(voice);
            
        }

        private void PopulateNativeSettings()
        {
            var settings = Properties.Core.Default;

            DisplayDropdown.SelectedIndex = settings.NativeNotifyScreen + 1;
            CornerDropdown.SelectedIndex = settings.NativeNotifyCorner;
            FontDropdown.SelectedItem = settings.NativeNotifyFont;
            ScaleSpinner.Value = settings.NativeNotifyScale;
            DurationSpinner.Value = settings.NativeNotifyTimeout;
            ColourButton.BackColor = Color.FromArgb((int)settings.NativeNotifyColour);
            PopupCheckbox.Checked = settings.NativeNotify;
            VoiceVolumeSlider.Value = settings.VoiceVolume;
            VoiceSpeedSlider.Value = settings.VoiceRate;
            VoiceDropdown.SelectedItem = settings.VoiceSelected;
            VoiceCheckbox.Checked = settings.VoiceNotify;
        }


        private void CoreMenu_SizeChanged(object? sender, EventArgs e)
        {
            CorePanel.Location = new Point(12 + CoreMenu.Width, 12);
            CorePanel.Width = Width - CoreMenu.Width - 40;

        }

        private Dictionary<string, ToolStripMenuItem> pluginList;

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
            panel.Header.Location = new Point(PopupNotificationLabel.Location.X, lowestPoint);
            panel.Header.Width = PopupNotificationLabel.Width;
            panel.Header.Font = PopupNotificationLabel.Font;
            panel.Header.ForeColor = PopupNotificationLabel.ForeColor;
            panel.Header.BackColor = PopupNotificationLabel.BackColor;
            panel.Header.TextAlign = PopupNotificationLabel.TextAlign;
            panel.Location = new Point(PopupNotificationLabel.Location.X, lowestPoint + panel.Header.Height);
            panel.Width = PopupSettingsPanel.Width;
            CorePanel.Controls.Add(panel.Header);
            CorePanel.Controls.Add(panel);
        }

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

        private void CoreMenu_ItemClicked(object? _, ToolStripItemClickedEventArgs e)
        {
            
            if (e.ClickedItem.Text == "<")
            {
                foreach (KeyValuePair<string, ToolStripMenuItem> menuItem in pluginList)
                {
                    if (menuItem.Value.Text == "<")
                        menuItem.Value.Text = ">";
                    else
                        menuItem.Value.Text = menuItem.Key[..1];
                }
                CoreMenu.Width = 40;
                CorePanel.Location = new Point(43, 12);
                // CorePanel.Width += 40;
            }
            else if (e.ClickedItem.Text == ">")
            {
                foreach (KeyValuePair<string, ToolStripMenuItem> menuItem in pluginList)
                {
                    if (menuItem.Value.Text == ">")
                        menuItem.Value.Text = "<";
                    else
                        menuItem.Value.Text = menuItem.Key;
                }
                CoreMenu.Width = 120;
                CorePanel.Location = new Point(123, 12);
                // CorePanel.Width -= 40;
            }
            else
            {
                foreach (var panel in uiPanels.Where(p => p.Key != e.ClickedItem))
                {
                    panel.Value.Visible = false;
                }

                if (!Controls.Contains(uiPanels[e.ClickedItem]))
                {
                    uiPanels[e.ClickedItem].Location = CorePanel.Location;
                    uiPanels[e.ClickedItem].Size = CorePanel.Size;
                    uiPanels[e.ClickedItem].BackColor = CorePanel.BackColor;
                    Controls.Add(uiPanels[e.ClickedItem]);
                }
                uiPanels[e.ClickedItem].Visible = true;
            }
            
        }

        private static void ColourListHeader(ref ListView list, Color backColor, Color foreColor)
        {
            list.OwnerDraw = true;
            
            list.DrawColumnHeader +=
                new DrawListViewColumnHeaderEventHandler
                (
                    (sender, e) => headerDraw(sender, e, backColor, foreColor)
                );
            list.DrawItem += new DrawListViewItemEventHandler(bodyDraw);
        }

        private static void headerDraw(object? _, DrawListViewColumnHeaderEventArgs e, Color backColor, Color foreColor)
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
                    var format = new StringFormat();
                    format.Alignment = (StringAlignment)e.Header.TextAlign;
                    format.LineAlignment = StringAlignment.Center;
                    
                    var paddedBounds = new Rectangle(e.Bounds.X + 2, e.Bounds.Y + 2, e.Bounds.Width - 4, e.Bounds.Height - 4);

                    e.Graphics.DrawString(e.Header?.Text, e.Font, foreBrush, paddedBounds, format);
                }
        }

        private static void bodyDraw(object? _, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void PluginList_Resize(object sender, EventArgs e)
        {
            FitColumns();
        }

        private void FitColumns()
        {
            int totalWidth = 0;
            foreach (ColumnHeader col in PluginList.Columns)
                totalWidth += col.Width;

            PluginList.Columns[3].Width += PluginList.Width - totalWidth;
        }

        private void ReadAllButton_Click(object sender, EventArgs e)
        {
            LogMonitor.GetInstance.ReadAllJournals();
        }

        private void PopupNotificationLabel_Click(object _, EventArgs e)
        {
            CorePanel.SuspendLayout();
            if (PopupNotificationLabel.Text[0] == '❯')
            {
                PopupNotificationLabel.Text = PopupNotificationLabel.Text.Replace('❯', '⌵');
                PopupSettingsPanel.Visible = true;
                AdjustPanelsBelow(PopupSettingsPanel, AdjustmentDirection.Down);
            }
            else
            {
                PopupNotificationLabel.Text = PopupNotificationLabel.Text.Replace('⌵', '❯');
                PopupSettingsPanel.Visible = false;
                AdjustPanelsBelow(PopupSettingsPanel, AdjustmentDirection.Up);
            }
            CorePanel.ResumeLayout();
        }

        private void VoiceNotificationLabel_Click(object sender, EventArgs e)
        {
            CorePanel.SuspendLayout();
            if (VoiceNotificationLabel.Text[0] == '❯')
            {
                VoiceNotificationLabel.Text = VoiceNotificationLabel.Text.Replace('❯', '⌵');
                VoiceSettingsPanel.Visible = true;
                AdjustPanelsBelow(VoiceSettingsPanel, AdjustmentDirection.Down);
            }
            else
            {
                VoiceNotificationLabel.Text = VoiceNotificationLabel.Text.Replace('⌵', '❯');
                VoiceSettingsPanel.Visible = false;
                AdjustPanelsBelow(VoiceSettingsPanel, AdjustmentDirection.Up);
            }
            CorePanel.ResumeLayout();
        }

        private void AdjustPanelsBelow(Control toggledControl, AdjustmentDirection adjustmentDirection)
        {
            var distance = adjustmentDirection == AdjustmentDirection.Down ? toggledControl.Height : -toggledControl.Height;
            foreach (Control control in CorePanel.Controls)
            {
                var loc = control.Location;
                if (loc.Y >= toggledControl.Location.Y && control != toggledControl)
                {
                    loc.Y = control.Location.Y + distance;
                    control.Location = loc;
                }
            }
        }

        internal enum AdjustmentDirection
        {
            Up, Down
        }

        #region Settings Changes

        private void ColourButton_Click(object _, EventArgs e)
        {
            var selectionResult = PopupColour.ShowDialog();
            if (selectionResult == DialogResult.OK)
            {
                ColourButton.BackColor = PopupColour.Color;
                Properties.Core.Default.NativeNotifyColour = (uint)PopupColour.Color.ToArgb();
                Properties.Core.Default.Save();
            }
        }

        private void PopupCheckbox_CheckedChanged(object _, EventArgs e)
        {
            Properties.Core.Default.NativeNotify = PopupCheckbox.Checked;
            Properties.Core.Default.Save();
        }

        private void DurationSpinner_ValueChanged(object _, EventArgs e)
        {
            Properties.Core.Default.NativeNotifyTimeout = (int)DurationSpinner.Value;
            Properties.Core.Default.Save();
        }

        private void ScaleSpinner_ValueChanged(object _, EventArgs e)
        {
            Properties.Core.Default.NativeNotifyScale = (int)ScaleSpinner.Value;
            Properties.Core.Default.Save();
        }

        private void FontDropdown_SelectedIndexChanged(object _, EventArgs e)
        {
            Properties.Core.Default.NativeNotifyFont = FontDropdown.SelectedItem.ToString();
            Properties.Core.Default.Save();
        }

        private void CornerDropdown_SelectedIndexChanged(object _, EventArgs e)
        {
            Properties.Core.Default.NativeNotifyCorner = CornerDropdown.SelectedIndex;
            Properties.Core.Default.Save();
        }

        private void DisplayDropdown_SelectedIndexChanged(object _, EventArgs e)
        {
            Properties.Core.Default.NativeNotifyScreen = DisplayDropdown.SelectedIndex - 1;
            Properties.Core.Default.Save();
        }

        private void VoiceVolumeSlider_Scroll(object _, EventArgs e)
        {
            Properties.Core.Default.VoiceVolume = VoiceVolumeSlider.Value;
            Properties.Core.Default.Save();
        }

        private void VoiceSpeedSlider_Scroll(object _, EventArgs e)
        {
            Properties.Core.Default.VoiceRate = VoiceSpeedSlider.Value;
            Properties.Core.Default.Save();
        }

        private void VoiceCheckbox_CheckedChanged(object _, EventArgs e)
        {
            Properties.Core.Default.VoiceNotify = VoiceCheckbox.Checked;
            Properties.Core.Default.Save();
        }

        private void VoiceDropdown_SelectedIndexChanged(object _, EventArgs e)
        {
            Properties.Core.Default.VoiceSelected = VoiceDropdown.SelectedItem.ToString();
            Properties.Core.Default.Save();
        }


        #endregion


    }
}