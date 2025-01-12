using Observatory.Assets;
using Observatory.Framework;
using Observatory.Framework.Interfaces;
using Observatory.Utils;
using System.Text.Json;

namespace Observatory.UI
{
    public partial class PopoutForm : Form
    {
        private TabPage _tab;
        private Panel? _panel;
        private Label _label;
        private IObservatoryPlugin _plugin;


        public PopoutForm(TabPage tab, IObservatoryPlugin plugin)
        {
            _plugin = plugin;
            _tab = tab;
            _panel = tab.Controls.OfType<Panel>().FirstOrDefault();
            if (_panel != null)
            {
                InitializeComponent();
                Text = plugin.Name;
                RestorePosition();
                Icon = Resources.EOCIcon_Presized;
                _panel.Location = new(0, 0);
                _panel.Size = PopoutPanel.Size;
                PopoutPanel.Controls.Add(_panel);
                _label = new Label()
                {
                    Text = plugin.Name + " is currently in a different window."
                    + Environment.NewLine + "You can click this text to switch to it.",
                    Size = _tab.ClientSize,
                    Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                    Padding = new(20),
                    BorderStyle = BorderStyle.FixedSingle
                };
                _label.Click += (o, e) =>
                {
                    if (WindowState == FormWindowState.Minimized)
                        WindowState = FormWindowState.Normal;
                    Activate();
                };
                ThemeManager.GetInstance.RegisterControl(_label);
                _tab.Controls.Add(_label);

                bool hasExportMethod = HasCustomExport(plugin);
                bool isBasicUI = plugin?.PluginUI.PluginUIType == Framework.PluginUI.UIType.Basic;
                bool canExport = isBasicUI || hasExportMethod;
                bool canClear = isBasicUI;
               
                if (!canClear)
                {
                    ClearButton.Enabled = false;
                    ClearButton.Visible = false;
                    ExportButton.Location = ClearButton.Location;
                }

                if (!canExport)
                {
                    ExportButton.Enabled = false;
                    ExportButton.Visible = false;
                }
            }
            else
            {
                MessageBox.Show("No UI panel found.");
                Close();
            }
        }

        private static bool HasCustomExport(IObservatoryPlugin? plugin) => ((Delegate)plugin.ExportContent).Method != typeof(IObservatoryPlugin).GetMethod("ExportContent");


        private void RestorePosition()
        {
            Dictionary<string, PopoutPosition> savedPositions;
            try
            {
                savedPositions = JsonSerializer.Deserialize<Dictionary<string, PopoutPosition>>
                    (Properties.Core.Default.PopoutLocation) ?? [];
            }
            catch
            {
                savedPositions = [];
            }

            if (savedPositions.TryGetValue(Text, out PopoutPosition? value) && value != null)
            {
                Point savedLocation = new(value.X, value.Y);
                Size savedSize = new(value.Width, value.Height);
                if (WithinScreenBounds(savedLocation, savedSize))
                {
                    Size = savedSize;
                    Location = savedLocation;
                }
            }
        }

        private bool WithinScreenBounds(Point location, Size size)
        {
            var windowRect = new Rectangle(location, size);

            // Shrink slightly to give some wiggle room
            windowRect.Height -= 40;
            windowRect.Width -= 40;
            windowRect.Offset(20, 20);

            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.WorkingArea.Contains(windowRect))
                {
                    return true;
                }
            }
            return false;
        }

        private void SavePosition()
        {
            var position = new PopoutPosition(Location.X, Location.Y, Width, Height);

            Dictionary<string, PopoutPosition> savedPositions;
            try
            {
                savedPositions = JsonSerializer.Deserialize<Dictionary<string, PopoutPosition>>
                    (Properties.Core.Default.PopoutLocation) ?? [];
            }
            catch
            {
                savedPositions = [];
            }
            savedPositions[Text] = position;

            Properties.Core.Default.PopoutLocation = JsonSerializer.Serialize(savedPositions);
            SettingsManager.Save();
        }

        private void PopoutForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_panel != null)
            {
                SavePosition();
                ThemeManager.GetInstance.UnregisterControl(_label);
                _tab.Controls.Clear();
                _tab.Controls.Add(_panel);
                _panel.Size = _tab.ClientSize;
                ThemeManager.GetInstance.UnregisterControl(this);
            }
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            // Custom export method handled inside ExportCSV
            if (Properties.Core.Default.ExportFormat == 0 || HasCustomExport(_plugin))
                ExportHandler.ExportCSV(_plugin);
            else
                ExportHandler.ExportXlsx(_plugin);
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            if (_plugin.PluginUI.PluginUIType == PluginUI.UIType.Basic)
            {
                _plugin.PluginUI.DataGrid.Clear();
            }
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            FormsManager.OpenPluginSettingsForm(_plugin);
        }

        private class PopoutPosition
        {
            public PopoutPosition(int x, int y, int width, int height) 
            {
                X = x;
                Y = y;
                Width = width;
                Height = height;
            }

            public int X { get; set; }
            public int Y { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
        }
    }
}
