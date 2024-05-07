using Observatory.Framework;
using Observatory.Framework.Interfaces;
using Observatory.PluginManagement;
using Observatory.Utils;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Observatory.UI
{
    public partial class CoreForm : Form
    {
        private readonly Dictionary<object, Panel> uiPanels;
        private ThemeManager themeManager;

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);
        private const int WM_SETREDRAW = 11;
        private static void SuspendDrawing(Control control)
        {
            SendMessage(control.Handle, WM_SETREDRAW, false, 0);
        }

        private static void ResumeDrawing(Control control)
        {
            SendMessage(control.Handle, WM_SETREDRAW, true, 0);
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
            string version = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "0";
            Text += $" - v{version}";
            CoreMenu.SizeChanged += CoreMenu_SizeChanged;
            uiPanels = new()
            {
                { coreToolStripMenuItem, CorePanel }
            };


            pluginList = new Dictionary<string, ToolStripMenuItem>();

            DisableOverriddenNotification();
            CoreMenu.ItemClicked += CoreMenu_ItemClicked;

            themeManager = ThemeManager.GetInstance;
            themeManager.RegisterControl(this);

            foreach (var theme in themeManager.GetThemes)
            {
                ThemeDropdown.Items.Add(theme);
            }
            ThemeDropdown.SelectedItem = themeManager.CurrentTheme;
            CreatePluginTabs();
        }

        public void FocusPlugin(string pluginShortName)
        {
            var pluginItem = FindMenuItemForPlugin(pluginShortName);
            if (pluginItem != null)
            {
                SuspendDrawing(this);
                ActivatePluginTab(pluginItem);
                ResumeDrawing(this);
            }
        }

        private ToolStripItem? FindMenuItemForPlugin(string pluginShortName)
        {
            foreach (ToolStripItem item in CoreMenu.Items)
            {
                if (item.Text == pluginShortName)
                {
                    return item;
                }
            }
            return null;
        }

        private void CoreMenu_SizeChanged(object? sender, EventArgs e)
        {
            CorePanel.Location = new Point(12 + CoreMenu.Width, 12);
            CorePanel.Width = Width - CoreMenu.Width - 40;
        }

        private readonly Dictionary<string, ToolStripMenuItem> pluginList;

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

        private void ResizePanels(Point location, int widthChange)
        {
            CorePanel.Location = location;
            CorePanel.Width += widthChange;
            foreach (var panel in uiPanels)
            {
                if (Controls.Contains(panel.Value))
                {
                    panel.Value.Location = CorePanel.Location;
                    panel.Value.Size = CorePanel.Size;
                }
            }
        }

        private void CoreMenu_ItemClicked(object? _, ToolStripItemClickedEventArgs e)
        {
            SuspendDrawing(this);
            if (e.ClickedItem.Text == "<")
            {
                foreach (KeyValuePair<string, ToolStripMenuItem> menuItem in pluginList)
                {
                    if (menuItem.Value.Text == "<")
                        menuItem.Value.Text = ">";
                    else
                        menuItem.Value.Text = menuItem.Key[..3];
                }
                CoreMenu.Width = 70;
                ResizePanels(new Point(CoreMenu.Width + 3, 12), 0);
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
                CoreMenu.Width = GetExpandedMenuWidth();
                ResizePanels(new Point(CoreMenu.Width + 3, 12), 0);
            }
            else
            {
                ActivatePluginTab(e.ClickedItem);
            }
            ResumeDrawing(this);
        }

        private void ActivatePluginTab(ToolStripItem item)
        {
            foreach (var panel in uiPanels.Where(p => p.Key != item))
            {
                panel.Value.Visible = false;
            }

            if (!Controls.Contains(uiPanels[item]))
            {
                uiPanels[item].Location = CorePanel.Location;
                uiPanels[item].Size = CorePanel.Size;
                uiPanels[item].BackColor = CorePanel.BackColor;
                uiPanels[item].Parent = CorePanel.Parent;
                Controls.Add(uiPanels[item]);
            }
            uiPanels[item].Visible = true;

            SetClickedItem(item);
        }

        private void SetClickedItem(ToolStripItem item)
        {
            foreach (ToolStripItem menuItem in CoreMenu.Items)
            {
                bool bold = menuItem == item;
                menuItem.Font = new Font(menuItem.Font, bold ? FontStyle.Bold : FontStyle.Regular);
            }
        }

        private static void ColourListHeader(ref ListView list, Color backColor, Color foreColor)
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
            int totalWidth = 0;
            foreach (ColumnHeader col in PluginList.Columns)
                totalWidth += col.Width;

            PluginList.Columns[3].Width += PluginList.Width - totalWidth; // - SystemInformation.VerticalScrollBarWidth;
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
            PluginComparer = new();
            foreach (var panel in uiPanels.Values)
            {
                foreach (var control in panel.Controls)
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
            if (PluginComparer?.Any() ?? false)
                foreach (var panel in PluginComparer.Keys)
                {
                    panel.ListViewItemSorter = (IObservatoryComparer)PluginComparer[panel];
                }
            PluginComparer?.Clear();
        }

        private Observatory.NativeNotification.NativePopup? nativePopup;



        private void GithubLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenURL("https://github.com/Xjph/ObservatoryCore");
        }

        private void DonateLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenURL("https://www.paypal.com/donate/?hosted_button_id=XYQWYQ337TBP4");
        }

        private void OpenURL(string url)
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }


    }
}