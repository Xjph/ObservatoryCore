﻿using System.Diagnostics;
using Observatory.Assets;

namespace Observatory.UI
{
    public partial class DonateForm : Form
    {
        public DonateForm()
        {
            Icon = Resources.EOCIcon_Presized;
            InitializeComponent();
        }

        private void PaypalButtonClick(object sender, EventArgs e)
        {
            OpenURL("https://www.paypal.com/donate/?hosted_button_id=XYQWYQ337TBP4");
            Close();
        }

        private void PatreonButtonClick(object sender, EventArgs e)
        {
            OpenURL("https://www.patreon.com/vithigar");
            Close();
        }

        private void OpenURL(string url)
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
    }
}
