using Observatory.Assets;
using Observatory.Framework;
using Observatory.NativeNotification;
using Observatory.PluginManagement;
using Observatory.Utils;
using System.Configuration;
using System.Data;
using System.Diagnostics;

namespace Observatory.UI
{
    public partial class CoreSettings : Form
    {
        private NativePopup? nativePopup;
        private NativeVoice? nativeVoice;
        private bool loading = false;

        public CoreSettings()
        {
            InitializeComponent();
            PopulateDropdownOptions();
            PopulateNativeSettings();
            Icon = Resources.EOCIcon_Presized;
        }


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

        private void FontScaleSpinner_ValueChanged(object sender, EventArgs e)
        {
            Properties.Core.Default.NativeNotifyFontScale = (int)FontScaleSpinner.Value;
            SettingsManager.Save();
        }

        private void FontDropdown_SelectedIndexChanged(object _, EventArgs e)
        {
            Properties.Core.Default.NativeNotifyFont = FontDropdown.SelectedItem?.ToString();
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

        private void PopupTransparentCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Core.Default.NativeNotifyTransparent = PopupTransparentCheckBox.Checked;
            SettingsManager.Save();
        }

        private void AudioVolumeSlider_Scroll(object _, EventArgs e)
        {
            Properties.Core.Default.VoiceVolume = Math.Clamp(AudioVolumeSlider.Value, 0, 100);
            Properties.Core.Default.AudioVolume = Math.Clamp(AudioVolumeSlider.Value / 100.0f, 0.0f, 1.0f);
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
            if (Properties.Core.Default.ChimeEnabled)
            {
                Properties.Core.Default.ChimeSelected = VoiceDropdown.SelectedIndex;
            }
            else
            {
                Properties.Core.Default.VoiceSelected = VoiceDropdown.SelectedItem?.ToString();
            }
            SettingsManager.Save();
        }

        private void AudioDeviceDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AudioDeviceDropdown.SelectedItem == null)
                // Shouldn't happen but default to the Windows built-in device (always exists at -1)
                Properties.Core.Default.AudioDevice = AudioHandler.GetFirstDevice();
            else
                // Stores the current selected device
                Properties.Core.Default.AudioDevice = AudioDeviceDropdown.SelectedItem.ToString();
            SettingsManager.Save();
        }
        private void AudioDeviceDropdown_Focused(object sender, EventArgs e)
        {
            AudioDeviceDropdown.Items.Clear();
            foreach (var device in AudioHandler.GetDevices())
                AudioDeviceDropdown.Items.Add(device);
            AudioDeviceDropdown.SelectedIndex = AudioHandler.GetDeviceIndex(Properties.Core.Default.AudioDevice);
        }

        private void PopulateDropdownOptions(bool voiceOnly = false)
        {
            if (!voiceOnly)
            {
                var fonts = new System.Drawing.Text.InstalledFontCollection().Families;
                FontDropdown.Items.AddRange(fonts.Select(f => f.Name).ToArray());

                DisplayDropdown.Items.Add("Primary");
                if (Screen.AllScreens.Length > 1)
                    for (int i = 0; i < Screen.AllScreens.Length; i++)
                        DisplayDropdown.Items.Add((i + 1).ToString());

                foreach (var theme in ThemeManager.GetInstance.GetThemes)
                {
                    ThemeDropdown.Items.Add(theme);
                }
                ThemeDropdown.SelectedItem = ThemeManager.GetInstance.CurrentTheme;

                var audioDevices = AudioHandler.GetDevices();

                if (audioDevices.Count > 0)
                {
                    foreach (var device in AudioHandler.GetDevices())
                        AudioDeviceDropdown.Items.Add(device);
                    var deviceIndex = AudioHandler.GetDeviceIndex(Properties.Core.Default.AudioDevice);
                    // Select first device if not found.
                    AudioDeviceDropdown.SelectedIndex = Math.Max(0, deviceIndex);
                }
            }

            if (Properties.Core.Default.ChimeEnabled)
            {
                VoiceDropdown.Items.AddRange([1, 2, 3, 4, 5, 6]);
                TryLoadSetting(VoiceDropdown, "SelectedIndex", Properties.Core.Default.ChimeSelected);
            }
            else
            {
#if !PROTON
                var speech = new System.Speech.Synthesis.SpeechSynthesizer();
                try
                {
                    speech.InjectOneCoreVoices();
                }
                catch
                {
                    // injection does some wacky reflection stuff
                    // silently proceed if it fails with original voices
                }

                var voices = speech.GetInstalledVoices();
                foreach (var voice in voices.Select(v => v.VoiceInfo.Name))
                    VoiceDropdown.Items.Add(voice);
                TryLoadSetting(VoiceDropdown, "SelectedItem", Properties.Core.Default.VoiceSelected);
#endif
            }


        }

        private void PopulateNativeSettings()
        {
            var settings = Properties.Core.Default;
            loading = true;
            TryLoadSetting(DisplayDropdown, "SelectedIndex", settings.NativeNotifyScreen + 1, 0);
            TryLoadSetting(CornerDropdown, "SelectedIndex", settings.NativeNotifyCorner, 0);
            TryLoadSetting(FontDropdown, "SelectedItem", settings.NativeNotifyFont);
            TryLoadSetting(ScaleSpinner, "Value", (decimal)Math.Clamp(settings.NativeNotifyScale, 1, 500), 100);
            TryLoadSetting(DurationSpinner, "Value", (decimal)Math.Clamp(settings.NativeNotifyTimeout, 100, 60000), 5000);
            TryLoadSetting(ColourButton, "BackColor", Color.FromArgb((int)settings.NativeNotifyColour));
            TryLoadSetting(PopupCheckbox, "Checked", settings.NativeNotify);
            TryLoadSetting(AudioVolumeSlider, "Value", Math.Clamp(settings.VoiceVolume, 0, 100), 100); // Also controls AudioVolume setting
            TryLoadSetting(VoiceSpeedSlider, "Value", Math.Clamp(settings.VoiceRate, -10, 10));
            TryLoadSetting(VoiceCheckbox, "Checked", settings.VoiceNotify);
            TryLoadSetting(LabelJournalPath, "Text", LogMonitor.GetJournalFolder().FullName);
            TryLoadSetting(StartMonitorCheckbox, "Checked", settings.StartMonitor);
            TryLoadSetting(StartReadallCheckbox, "Checked", settings.StartReadAll);
            TryLoadSetting(ExportFormatDropdown, "SelectedIndex", settings.ExportFormat);
            TryLoadSetting(PopupTransparentCheckBox, "Checked", settings.NativeNotifyTransparent);
            TryLoadSetting(FontScaleSpinner, "Value", (decimal)Math.Clamp(settings.NativeNotifyFontScale, 1, 500), 100);
            TryLoadSetting(AudioTypeDropdown, "SelectedIndex", settings.ChimeEnabled ? 1 : 0);
            TryLoadSetting(AltMonitorCheckbox, "Checked", settings.AltMonitor);

#if PROTON
            VoiceCheckbox.Checked = false;
            VoiceCheckbox.Enabled = false;
            VoiceSpeedSlider.Enabled = false;
            VoiceDropdown.Enabled = false;
            VoiceTestButton.Enabled = false;
            VoiceDisabledPanel.Visible = true;
            VoiceDisabledLabel.Text = "Native voice notifications not available in this build.";
            VoiceDisabledPanel.BringToFront();
#endif
#if !DEBUG
            CoreConfigFolder.Visible = false;
#endif
            loading = false;
        }

        static private void TryLoadSetting(Control control, string property, object newValue, object? defaultValue = null)
        {
            try
            {
                (control.GetType().GetProperty(property)?.GetSetMethod())?.Invoke(control, [newValue]);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to load all settings ({control.Name}), some values may have been cleared.\r\nError: {ex.InnerException?.Message}");
                if (defaultValue != null)
                    (control.GetType().GetProperty(property)?.GetSetMethod())?.Invoke(control, [defaultValue]);
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


        private void VoiceTestButton_Click(object sender, EventArgs e)
        {
            NotificationArgs args = new()
            {
                Title = "Test Voice Notification",
                Detail = "This is a test of native voice notifications."
            };
            AudioHandler audioHandler = new AudioHandler();
            nativeVoice ??= new(audioHandler);

            nativeVoice.AudioHandlerEnqueue(args);
        }

        private void ThemeDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            var themeManager = ThemeManager.GetInstance;
            themeManager.CurrentTheme = ThemeDropdown.SelectedItem?.ToString() ?? themeManager.CurrentTheme;
            Properties.Core.Default.Theme = themeManager.CurrentTheme;
            foreach (var plugin in PluginManager.GetInstance.AllUIPlugins)
            {
                plugin.ThemeChanged(themeManager.CurrentTheme, themeManager.CurrentThemeDetails);
            }
            SettingsManager.Save();
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
                    var themeName = ThemeManager.GetInstance.AddTheme(fileContent);
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

        private void StartMonitorCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Core.Default.StartMonitor = StartMonitorCheckbox.Checked;
            SettingsManager.Save();
        }

        private void StartReadallCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Core.Default.StartReadAll = StartReadallCheckbox.Checked;
            SettingsManager.Save();
        }

        private void ExportFormatDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Core.Default.ExportFormat = ExportFormatDropdown.SelectedIndex;
            SettingsManager.Save();
        }
        private void CoreConfigFolder_Click(object sender, EventArgs e)
        {
#if PORTABLE
            string? observatoryLocation = System.Diagnostics.Process.GetCurrentProcess()?.MainModule?.FileName;
            var configDir = new FileInfo(observatoryLocation ?? String.Empty).DirectoryName;
#else
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            var fileInfo = new FileInfo(config.FilePath);
            var configDir = fileInfo.DirectoryName;
#endif

            if (string.IsNullOrWhiteSpace(configDir) || !Directory.Exists(configDir))
            {
                return;
            }

            var fileExplorerInfo = new ProcessStartInfo() { FileName = configDir, UseShellExecute = true };
            Process.Start(fileExplorerInfo);
        }


        private void CoreSettingsOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void AudioTypeDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Core.Default.ChimeEnabled = AudioTypeDropdown.SelectedItem?.ToString() == "Chime";
            SettingsManager.Save();

            VoiceLabel.Text = Properties.Core.Default.ChimeEnabled ? "Chime:" : "Voice:";
            VoiceSpeedSlider.Enabled = !Properties.Core.Default.ChimeEnabled;
            VoiceDropdown.Items.Clear();
            PopulateDropdownOptions(true);
        }

        private void AltMonitorCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (loading || !AltMonitorCheckbox.Checked
                || MessageBox.Show(
                    "This setting should only be enabled if standard log monitoring is not working correctly.",
                    "Enabling Log Polling",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Exclamation) == DialogResult.OK)
            {
                Properties.Core.Default.AltMonitor = AltMonitorCheckbox.Checked;
                SettingsManager.Save();
            }
        }

        private void TestNotificationRouting_Click(object sender, EventArgs e)
        {
            NotificationRendering r = 0;
            if (PopupNotifCheckbox.Checked) r |= NotificationRendering.NativeVisual;
            if (AudioNotifCheckbox.Checked) r |= NotificationRendering.NativeVocal;
            if (PluginNotifCheckbox.Checked) r |= NotificationRendering.PluginNotifier;

            var args = new NotificationArgs()
            {
                Title = "Notification routing test",
                Detail = "Please check that expected notifications were triggered.",
                ExtendedDetails = "Ensure all expected content is present.",
                Sender = "Core",
                Rendering = r,
            };
            PluginManager.GetInstance.Core.SendNotification(args);
        }
    }
}
