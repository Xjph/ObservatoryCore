using Observatory.Assets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Observatory.UI
{
    public partial class PopoutForm : Form
    {
        private TabPage _tab;
        private Panel? _panel;
        private Label _label;

        public PopoutForm(TabPage tab, string pluginName)
        {
            _tab = tab;
            _panel = tab.Controls.OfType<Panel>().FirstOrDefault();
            if (_panel != null)
            {
                InitializeComponent();
                Text = pluginName;
                Icon = Resources.EOCIcon_Presized;
                _panel.Location = new(0, 0);
                _panel.Size = PopoutPanel.Size;
                PopoutPanel.Controls.Add(_panel);
                _label = new Label() 
                { 
                    Text = pluginName + " is currently in a different window."
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
            }
            else
            {
                MessageBox.Show("No UI panel found.");
                Close();
            }
        }

        private void PopoutForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_panel != null)
            {
                ThemeManager.GetInstance.UnregisterControl(_label);
                _tab.Controls.Clear();
                _tab.Controls.Add(_panel);
                _panel.Size = _tab.ClientSize;
                ThemeManager.GetInstance.UnregisterControl(this);
            }
        }
    }
}
