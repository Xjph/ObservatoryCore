﻿using Observatory.Assets;
using Observatory.Framework;
using Observatory.Framework.Interfaces;
using Observatory.Utils;

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
            var savedPositions = Properties.Core.Default.PopoutLocation;
            if (savedPositions != null)
            {
                foreach (var position in savedPositions)
                {
                    if (position?.StartsWith(Text) ?? false)
                    {
                        var values = position.Split('\u001f');
                        if (values.Length == 5
                            && int.TryParse(values[1], out int locationX)
                            && int.TryParse(values[2], out int locationY)
                            && int.TryParse(values[3], out int sizeW)
                            && int.TryParse(values[4], out int sizeH))
                        {

                            Point savedLocation = new(locationX, locationY);
                            Size savedSize = new(sizeW, sizeH);
                            if (WithinScreenBounds(savedLocation, savedSize))
                            {
                                Size = savedSize;
                                Location = savedLocation;
                            }
                        }
                        break;
                    }
                }
            }
        }

        private bool WithinScreenBounds(Point location, Size size)
        {
            var windowRect = new Rectangle(location, size);

            // Shrink slightly to give some wiggle room
            windowRect.Height -= 10;
            windowRect.Width -= 10;
            windowRect.Offset(5, 5);

            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.Bounds.Contains(windowRect))
                {
                    return true;
                }
            }
            return false;
        }

        private void SavePosition()
        {
            var positionString = $"{Text}\u001f{Location.X}\u001f{Location.Y}\u001f{Width}\u001f{Height}";
            bool found = false;
            var savedPositions = Properties.Core.Default.PopoutLocation;
            for (int i = 0; i < savedPositions?.Count; i++)
            {
                if (savedPositions[i]?.StartsWith(Text) ?? false)
                {
                    savedPositions[i] = positionString;
                    found = true;
                }
            }
            if (!found)
            {
                savedPositions ??= [];
                savedPositions.Add(positionString);
            }
            Properties.Core.Default.PopoutLocation = savedPositions;
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
            var settingsTitle = $"{_plugin.Name} Settings";
            var form = GetFormByTitle(settingsTitle);
            if (form != null)
            {
                form.Activate();
            }
            else
            {
                var settingsForm = new SettingsForm(_plugin);
                settingsForm.Show();
            }
        }

        private static Form? GetFormByTitle(string title)
        {
            foreach (Form form in Application.OpenForms)
            {
                if (form.Text == title)
                    return form;
            }
            return null;
        }
    }
}
