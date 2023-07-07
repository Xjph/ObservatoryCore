using Observatory.Framework;
using Observatory.Framework.Interfaces;
using Observatory.PluginManagement;
using Observatory.Utils;
using System.Text;
using System.Windows.Forms;

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

        private void CoreMenu_SizeChanged(object? sender, EventArgs e)
        {
            CorePanel.Location = new Point(12 + CoreMenu.Width, 12);
            CorePanel.Width = Width - CoreMenu.Width - 40;

        }

        private Dictionary<string, ToolStripMenuItem> pluginList;

        private static void DuplicateControlVisuals(Control source, Control target, bool applyHeight = true)
        {
            if (applyHeight) target.Height = source.Height;
            target.Width = source.Width;
            target.Font = source.Font;
            target.ForeColor = source.ForeColor;
            target.BackColor = source.BackColor;
            target.Anchor = source.Anchor;
        }

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

        private void TestButton_Click(object sender, EventArgs e)
        {
            NotificationArgs args = new()
            {
                Title = "Test Notification",
                Detail = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec at elit maximus, ornare dui nec, accumsan velit. Vestibulum fringilla elit."
            };
            var testNotify = new NotificationForm(new Guid(), args);
            testNotify.Show();

        }
    }
}