using Observatory.Framework;
using Observatory.Utils;
using System.Reflection;

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
#if !PROTON
            var voices = new System.Speech.Synthesis.SpeechSynthesizer().GetInstalledVoices();
            foreach (var voice in voices.Select(v => v.VoiceInfo.Name))
                VoiceDropdown.Items.Add(voice);
#endif 
        }

        private void PopulateNativeSettings()
        {
            var settings = Properties.Core.Default;

            TryLoadSetting(DisplayDropdown, "SelectedIndex", settings.NativeNotifyScreen + 1);
            TryLoadSetting(CornerDropdown, "SelectedIndex", settings.NativeNotifyCorner);
            TryLoadSetting(FontDropdown, "SelectedItem", settings.NativeNotifyFont);
            TryLoadSetting(ScaleSpinner, "Value", (decimal)Math.Clamp(settings.NativeNotifyScale, 1, 500));
            TryLoadSetting(DurationSpinner, "Value", (decimal)Math.Clamp(settings.NativeNotifyTimeout, 100, 60000));
            TryLoadSetting(ColourButton, "BackColor", Color.FromArgb((int)settings.NativeNotifyColour));
            TryLoadSetting(PopupCheckbox, "Checked", settings.NativeNotify);
            TryLoadSetting(VoiceVolumeSlider, "Value", Math.Clamp(settings.VoiceVolume, 0, 100));
            TryLoadSetting(VoiceSpeedSlider, "Value", Math.Clamp(settings.VoiceRate, 0, 100));
            TryLoadSetting(VoiceDropdown, "SelectedItem", settings.VoiceSelected);
            TryLoadSetting(VoiceCheckbox, "Checked", settings.VoiceNotify);
            TryLoadSetting(LabelJournalPath, "Text", LogMonitor.GetJournalFolder().FullName);

#if PROTON
            VoiceCheckbox.Checked = false;
            VoiceCheckbox.Enabled = false;
            VoiceVolumeSlider.Enabled = false;
            VoiceSpeedSlider.Enabled = false;
            VoiceDropdown.Enabled = false;
            VoiceTestButton.Enabled = false;
            VoiceDisabledPanel.Visible = true;
            VoiceDisabledLabel.Text = "Native voice notifications not available in this build.";
            VoiceDisabledPanel.BringToFront();
#endif
        }

        static private void TryLoadSetting(object control, string property, object newValue)
        {
            try
            {
                (control.GetType().GetProperty(property)?.GetSetMethod())?.Invoke(control, [newValue]);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to load all settings, some values may have been cleared.\r\nError: {ex.InnerException?.Message}");
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
    }
}