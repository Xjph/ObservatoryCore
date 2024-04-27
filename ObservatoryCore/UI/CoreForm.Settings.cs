using Observatory.Framework;
using Observatory.Utils;

namespace Observatory.UI
{
    partial class CoreForm
    {
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

        private void FontDropdown_SelectedIndexChanged(object _, EventArgs e)
        {
            Properties.Core.Default.NativeNotifyFont = FontDropdown.SelectedItem.ToString();
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

        private void VoiceVolumeSlider_Scroll(object _, EventArgs e)
        {
            Properties.Core.Default.VoiceVolume = VoiceVolumeSlider.Value;
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
            Properties.Core.Default.VoiceSelected = VoiceDropdown.SelectedItem.ToString();
            SettingsManager.Save();
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

        private void ThemeDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            themeManager.CurrentTheme = ThemeDropdown.SelectedItem.ToString() ?? themeManager.CurrentTheme;
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
    }
}