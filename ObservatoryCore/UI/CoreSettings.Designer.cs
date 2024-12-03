namespace Observatory.UI
{
    partial class CoreSettings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            PopupSettingsPanel = new Panel();
            FontScaleSpinner = new NumericUpDown();
            PopupTransparentCheckBox = new CheckBox();
            LabelFontScale = new Label();
            DurationSpinner = new NumericUpDown();
            DisplayLabel = new Label();
            CornerLabel = new Label();
            LabelFont = new Label();
            LabelScale = new Label();
            LabelDuration = new Label();
            LabelColour = new Label();
            ScaleSpinner = new NumericUpDown();
            PopupLabel = new Label();
            FontDropdown = new ComboBox();
            TestButton = new Button();
            CornerDropdown = new ComboBox();
            ColourButton = new Button();
            DisplayDropdown = new ComboBox();
            PopupCheckbox = new CheckBox();
            PopupDisabledPanel = new Panel();
            PopupDisabledLabel = new Label();
            CoreSettingsPanel = new Panel();
            AudioDeviceLabel = new Label();
            AudioDeviceDropdown = new ComboBox();
            ExportFormatLabel = new Label();
            CoreConfigFolder = new Button();
            AudioVolumeSlider = new TrackBar();
            ExportFormatDropdown = new ComboBox();
            CoreSettingsLabel = new Label();
            StartReadallCheckbox = new CheckBox();
            StartMonitorCheckbox = new CheckBox();
            LabelJournal = new Label();
            ThemeLabel = new Label();
            VoiceVolumeLabel = new Label();
            LabelJournalPath = new Label();
            ThemeDropdown = new ComboBox();
            ButtonAddTheme = new Button();
            VoiceSettingsPanel = new Panel();
            VoiceTestButton = new Button();
            AudioLabel = new Label();
            VoiceCheckbox = new CheckBox();
            VoiceSpeedSlider = new TrackBar();
            AudioTypeLabel = new Label();
            VoiceSpeedLabel = new Label();
            AudioTypeDropdown = new ComboBox();
            VoiceLabel = new Label();
            VoiceDropdown = new ComboBox();
            VoiceDisabledPanel = new Panel();
            VoiceDisabledLabel = new Label();
            CoreSettingsOK = new Button();
            PopupColour = new ColorDialog();
            PopupSettingsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)FontScaleSpinner).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DurationSpinner).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ScaleSpinner).BeginInit();
            PopupDisabledPanel.SuspendLayout();
            CoreSettingsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)AudioVolumeSlider).BeginInit();
            VoiceSettingsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)VoiceSpeedSlider).BeginInit();
            VoiceDisabledPanel.SuspendLayout();
            SuspendLayout();
            // 
            // PopupSettingsPanel
            // 
            PopupSettingsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            PopupSettingsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            PopupSettingsPanel.BorderStyle = BorderStyle.FixedSingle;
            PopupSettingsPanel.Controls.Add(FontScaleSpinner);
            PopupSettingsPanel.Controls.Add(PopupTransparentCheckBox);
            PopupSettingsPanel.Controls.Add(LabelFontScale);
            PopupSettingsPanel.Controls.Add(DurationSpinner);
            PopupSettingsPanel.Controls.Add(DisplayLabel);
            PopupSettingsPanel.Controls.Add(CornerLabel);
            PopupSettingsPanel.Controls.Add(LabelFont);
            PopupSettingsPanel.Controls.Add(LabelScale);
            PopupSettingsPanel.Controls.Add(LabelDuration);
            PopupSettingsPanel.Controls.Add(LabelColour);
            PopupSettingsPanel.Controls.Add(ScaleSpinner);
            PopupSettingsPanel.Controls.Add(PopupLabel);
            PopupSettingsPanel.Controls.Add(FontDropdown);
            PopupSettingsPanel.Controls.Add(TestButton);
            PopupSettingsPanel.Controls.Add(CornerDropdown);
            PopupSettingsPanel.Controls.Add(ColourButton);
            PopupSettingsPanel.Controls.Add(DisplayDropdown);
            PopupSettingsPanel.Controls.Add(PopupCheckbox);
            PopupSettingsPanel.Controls.Add(PopupDisabledPanel);
            PopupSettingsPanel.Location = new Point(13, 248);
            PopupSettingsPanel.Name = "PopupSettingsPanel";
            PopupSettingsPanel.Size = new Size(467, 226);
            PopupSettingsPanel.TabIndex = 30;
            // 
            // FontScaleSpinner
            // 
            FontScaleSpinner.Location = new Point(292, 109);
            FontScaleSpinner.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            FontScaleSpinner.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            FontScaleSpinner.Name = "FontScaleSpinner";
            FontScaleSpinner.Size = new Size(67, 23);
            FontScaleSpinner.TabIndex = 12;
            FontScaleSpinner.Value = new decimal(new int[] { 100, 0, 0, 0 });
            FontScaleSpinner.ValueChanged += FontScaleSpinner_ValueChanged;
            // 
            // PopupTransparentCheckBox
            // 
            PopupTransparentCheckBox.AutoSize = true;
            PopupTransparentCheckBox.Location = new Point(201, 140);
            PopupTransparentCheckBox.Name = "PopupTransparentCheckBox";
            PopupTransparentCheckBox.Size = new Size(95, 19);
            PopupTransparentCheckBox.TabIndex = 13;
            PopupTransparentCheckBox.Text = "Transparency";
            PopupTransparentCheckBox.UseVisualStyleBackColor = true;
            PopupTransparentCheckBox.CheckedChanged += PopupTransparentCheckBox_CheckedChanged;
            // 
            // LabelFontScale
            // 
            LabelFontScale.AutoSize = true;
            LabelFontScale.Location = new Point(201, 111);
            LabelFontScale.Name = "LabelFontScale";
            LabelFontScale.Size = new Size(85, 15);
            LabelFontScale.TabIndex = 11;
            LabelFontScale.Text = "Font Scale (%):";
            LabelFontScale.TextAlign = ContentAlignment.MiddleRight;
            // 
            // DurationSpinner
            // 
            DurationSpinner.Increment = new decimal(new int[] { 25, 0, 0, 0 });
            DurationSpinner.Location = new Point(118, 138);
            DurationSpinner.Maximum = new decimal(new int[] { 60000, 0, 0, 0 });
            DurationSpinner.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
            DurationSpinner.Name = "DurationSpinner";
            DurationSpinner.Size = new Size(67, 23);
            DurationSpinner.TabIndex = 11;
            DurationSpinner.Value = new decimal(new int[] { 8000, 0, 0, 0 });
            DurationSpinner.ValueChanged += DurationSpinner_ValueChanged;
            // 
            // DisplayLabel
            // 
            DisplayLabel.AutoSize = true;
            DisplayLabel.Location = new Point(55, 25);
            DisplayLabel.Name = "DisplayLabel";
            DisplayLabel.Size = new Size(48, 15);
            DisplayLabel.TabIndex = 0;
            DisplayLabel.Text = "Display:";
            DisplayLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // CornerLabel
            // 
            CornerLabel.AutoSize = true;
            CornerLabel.Location = new Point(57, 54);
            CornerLabel.Name = "CornerLabel";
            CornerLabel.Size = new Size(46, 15);
            CornerLabel.TabIndex = 1;
            CornerLabel.Text = "Corner:";
            CornerLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // LabelFont
            // 
            LabelFont.AutoSize = true;
            LabelFont.Location = new Point(69, 83);
            LabelFont.Name = "LabelFont";
            LabelFont.Size = new Size(34, 15);
            LabelFont.TabIndex = 4;
            LabelFont.Text = "Font:";
            LabelFont.TextAlign = ContentAlignment.MiddleRight;
            // 
            // LabelScale
            // 
            LabelScale.AutoSize = true;
            LabelScale.Location = new Point(45, 111);
            LabelScale.Name = "LabelScale";
            LabelScale.Size = new Size(58, 15);
            LabelScale.TabIndex = 7;
            LabelScale.Text = "Scale (%):";
            LabelScale.TextAlign = ContentAlignment.MiddleRight;
            // 
            // LabelDuration
            // 
            LabelDuration.AutoSize = true;
            LabelDuration.Location = new Point(20, 140);
            LabelDuration.Name = "LabelDuration";
            LabelDuration.Size = new Size(83, 15);
            LabelDuration.TabIndex = 9;
            LabelDuration.Text = "Duration (ms):";
            LabelDuration.TextAlign = ContentAlignment.MiddleRight;
            // 
            // LabelColour
            // 
            LabelColour.AutoSize = true;
            LabelColour.Location = new Point(57, 171);
            LabelColour.Name = "LabelColour";
            LabelColour.Size = new Size(46, 15);
            LabelColour.TabIndex = 13;
            LabelColour.Text = "Colour:";
            LabelColour.TextAlign = ContentAlignment.MiddleRight;
            // 
            // ScaleSpinner
            // 
            ScaleSpinner.Location = new Point(118, 109);
            ScaleSpinner.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            ScaleSpinner.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            ScaleSpinner.Name = "ScaleSpinner";
            ScaleSpinner.Size = new Size(67, 23);
            ScaleSpinner.TabIndex = 10;
            ScaleSpinner.Value = new decimal(new int[] { 100, 0, 0, 0 });
            ScaleSpinner.ValueChanged += ScaleSpinner_ValueChanged;
            // 
            // PopupLabel
            // 
            PopupLabel.AutoSize = true;
            PopupLabel.Location = new Point(0, -1);
            PopupLabel.Name = "PopupLabel";
            PopupLabel.Size = new Size(113, 15);
            PopupLabel.TabIndex = 30;
            PopupLabel.Text = "Popup Notifications";
            // 
            // FontDropdown
            // 
            FontDropdown.DropDownStyle = ComboBoxStyle.DropDownList;
            FontDropdown.FormattingEnabled = true;
            FontDropdown.Location = new Point(117, 80);
            FontDropdown.Name = "FontDropdown";
            FontDropdown.Size = new Size(242, 23);
            FontDropdown.TabIndex = 9;
            FontDropdown.SelectedIndexChanged += FontDropdown_SelectedIndexChanged;
            // 
            // TestButton
            // 
            TestButton.FlatAppearance.BorderSize = 0;
            TestButton.FlatStyle = FlatStyle.Flat;
            TestButton.Location = new Point(187, 167);
            TestButton.Name = "TestButton";
            TestButton.Size = new Size(51, 23);
            TestButton.TabIndex = 13;
            TestButton.Text = "Test";
            TestButton.UseVisualStyleBackColor = false;
            TestButton.Click += TestButton_Click;
            // 
            // CornerDropdown
            // 
            CornerDropdown.DropDownStyle = ComboBoxStyle.DropDownList;
            CornerDropdown.FormattingEnabled = true;
            CornerDropdown.Items.AddRange(new object[] { "Bottom-Right", "Bottom-Left", "Top-Right", "Top-Left", "Center-Top", "Center-Bottom", "Center-Left", "Center-Right" });
            CornerDropdown.Location = new Point(117, 51);
            CornerDropdown.Name = "CornerDropdown";
            CornerDropdown.Size = new Size(121, 23);
            CornerDropdown.TabIndex = 8;
            CornerDropdown.SelectedIndexChanged += CornerDropdown_SelectedIndexChanged;
            // 
            // ColourButton
            // 
            ColourButton.FlatStyle = FlatStyle.Flat;
            ColourButton.Location = new Point(118, 167);
            ColourButton.Name = "ColourButton";
            ColourButton.Size = new Size(51, 23);
            ColourButton.TabIndex = 12;
            ColourButton.UseVisualStyleBackColor = true;
            ColourButton.Click += ColourButton_Click;
            // 
            // DisplayDropdown
            // 
            DisplayDropdown.DropDownStyle = ComboBoxStyle.DropDownList;
            DisplayDropdown.FormattingEnabled = true;
            DisplayDropdown.Location = new Point(117, 22);
            DisplayDropdown.Name = "DisplayDropdown";
            DisplayDropdown.Size = new Size(121, 23);
            DisplayDropdown.TabIndex = 7;
            DisplayDropdown.SelectedIndexChanged += DisplayDropdown_SelectedIndexChanged;
            // 
            // PopupCheckbox
            // 
            PopupCheckbox.AutoSize = true;
            PopupCheckbox.Location = new Point(117, 196);
            PopupCheckbox.Name = "PopupCheckbox";
            PopupCheckbox.Size = new Size(68, 19);
            PopupCheckbox.TabIndex = 14;
            PopupCheckbox.Text = "Enabled";
            PopupCheckbox.UseVisualStyleBackColor = true;
            PopupCheckbox.CheckedChanged += PopupCheckbox_CheckedChanged;
            // 
            // PopupDisabledPanel
            // 
            PopupDisabledPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            PopupDisabledPanel.Controls.Add(PopupDisabledLabel);
            PopupDisabledPanel.Enabled = false;
            PopupDisabledPanel.Location = new Point(3, 17);
            PopupDisabledPanel.Name = "PopupDisabledPanel";
            PopupDisabledPanel.Size = new Size(459, 204);
            PopupDisabledPanel.TabIndex = 16;
            PopupDisabledPanel.Visible = false;
            // 
            // PopupDisabledLabel
            // 
            PopupDisabledLabel.AutoSize = true;
            PopupDisabledLabel.Location = new Point(19, 12);
            PopupDisabledLabel.Name = "PopupDisabledLabel";
            PopupDisabledLabel.Size = new Size(141, 15);
            PopupDisabledLabel.TabIndex = 0;
            PopupDisabledLabel.Text = "Placeholder Disabled Text";
            // 
            // CoreSettingsPanel
            // 
            CoreSettingsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            CoreSettingsPanel.BorderStyle = BorderStyle.FixedSingle;
            CoreSettingsPanel.Controls.Add(AudioDeviceLabel);
            CoreSettingsPanel.Controls.Add(AudioDeviceDropdown);
            CoreSettingsPanel.Controls.Add(ExportFormatLabel);
            CoreSettingsPanel.Controls.Add(CoreConfigFolder);
            CoreSettingsPanel.Controls.Add(AudioVolumeSlider);
            CoreSettingsPanel.Controls.Add(ExportFormatDropdown);
            CoreSettingsPanel.Controls.Add(CoreSettingsLabel);
            CoreSettingsPanel.Controls.Add(StartReadallCheckbox);
            CoreSettingsPanel.Controls.Add(StartMonitorCheckbox);
            CoreSettingsPanel.Controls.Add(LabelJournal);
            CoreSettingsPanel.Controls.Add(ThemeLabel);
            CoreSettingsPanel.Controls.Add(VoiceVolumeLabel);
            CoreSettingsPanel.Controls.Add(LabelJournalPath);
            CoreSettingsPanel.Controls.Add(ThemeDropdown);
            CoreSettingsPanel.Controls.Add(ButtonAddTheme);
            CoreSettingsPanel.Location = new Point(12, 12);
            CoreSettingsPanel.Name = "CoreSettingsPanel";
            CoreSettingsPanel.Size = new Size(468, 230);
            CoreSettingsPanel.TabIndex = 34;
            CoreSettingsPanel.Tag = "";
            // 
            // AudioDeviceLabel
            // 
            AudioDeviceLabel.AutoSize = true;
            AudioDeviceLabel.Location = new Point(23, 153);
            AudioDeviceLabel.Name = "AudioDeviceLabel";
            AudioDeviceLabel.Size = new Size(80, 15);
            AudioDeviceLabel.TabIndex = 37;
            AudioDeviceLabel.Text = "Audio Device:";
            // 
            // AudioDeviceDropdown
            // 
            AudioDeviceDropdown.DropDownStyle = ComboBoxStyle.DropDownList;
            AudioDeviceDropdown.FormattingEnabled = true;
            AudioDeviceDropdown.Location = new Point(117, 150);
            AudioDeviceDropdown.Name = "AudioDeviceDropdown";
            AudioDeviceDropdown.Size = new Size(214, 23);
            AudioDeviceDropdown.TabIndex = 25;
            AudioDeviceDropdown.SelectedIndexChanged += AudioDeviceDropdown_SelectedIndexChanged;
            AudioDeviceDropdown.Click += AudioDeviceDropdown_Focused;
            // 
            // ExportFormatLabel
            // 
            ExportFormatLabel.AutoSize = true;
            ExportFormatLabel.Location = new Point(18, 50);
            ExportFormatLabel.Name = "ExportFormatLabel";
            ExportFormatLabel.Size = new Size(85, 15);
            ExportFormatLabel.TabIndex = 35;
            ExportFormatLabel.Text = "Export Format:";
            // 
            // CoreConfigFolder
            // 
            CoreConfigFolder.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            CoreConfigFolder.FlatAppearance.BorderSize = 0;
            CoreConfigFolder.FlatStyle = FlatStyle.Flat;
            CoreConfigFolder.Location = new Point(373, 200);
            CoreConfigFolder.Name = "CoreConfigFolder";
            CoreConfigFolder.Size = new Size(90, 23);
            CoreConfigFolder.TabIndex = 19;
            CoreConfigFolder.Text = "Config Folder";
            CoreConfigFolder.UseVisualStyleBackColor = true;
            CoreConfigFolder.Click += CoreConfigFolder_Click;
            // 
            // AudioVolumeSlider
            // 
            AudioVolumeSlider.LargeChange = 10;
            AudioVolumeSlider.Location = new Point(117, 105);
            AudioVolumeSlider.Maximum = 100;
            AudioVolumeSlider.Name = "AudioVolumeSlider";
            AudioVolumeSlider.Size = new Size(214, 45);
            AudioVolumeSlider.TabIndex = 24;
            AudioVolumeSlider.TickFrequency = 10;
            AudioVolumeSlider.TickStyle = TickStyle.Both;
            AudioVolumeSlider.Value = 100;
            AudioVolumeSlider.Scroll += AudioVolumeSlider_Scroll;
            // 
            // ExportFormatDropdown
            // 
            ExportFormatDropdown.DropDownStyle = ComboBoxStyle.DropDownList;
            ExportFormatDropdown.FormattingEnabled = true;
            ExportFormatDropdown.Items.AddRange(new object[] { "Tab-Separated Values (csv)", "Office Open XML (xlsx)" });
            ExportFormatDropdown.Location = new Point(117, 47);
            ExportFormatDropdown.Name = "ExportFormatDropdown";
            ExportFormatDropdown.Size = new Size(214, 23);
            ExportFormatDropdown.TabIndex = 21;
            ExportFormatDropdown.SelectedIndexChanged += ExportFormatDropdown_SelectedIndexChanged;
            // 
            // CoreSettingsLabel
            // 
            CoreSettingsLabel.AutoSize = true;
            CoreSettingsLabel.Location = new Point(0, 0);
            CoreSettingsLabel.Name = "CoreSettingsLabel";
            CoreSettingsLabel.Size = new Size(77, 15);
            CoreSettingsLabel.TabIndex = 33;
            CoreSettingsLabel.Text = "Core Settings";
            // 
            // StartReadallCheckbox
            // 
            StartReadallCheckbox.AutoSize = true;
            StartReadallCheckbox.Location = new Point(117, 204);
            StartReadallCheckbox.Name = "StartReadallCheckbox";
            StartReadallCheckbox.Size = new Size(130, 19);
            StartReadallCheckbox.TabIndex = 27;
            StartReadallCheckbox.Text = "Read All On Launch";
            StartReadallCheckbox.UseVisualStyleBackColor = true;
            StartReadallCheckbox.CheckedChanged += StartReadallCheckbox_CheckedChanged;
            // 
            // StartMonitorCheckbox
            // 
            StartMonitorCheckbox.AutoSize = true;
            StartMonitorCheckbox.Location = new Point(117, 179);
            StartMonitorCheckbox.Name = "StartMonitorCheckbox";
            StartMonitorCheckbox.Size = new Size(157, 19);
            StartMonitorCheckbox.TabIndex = 26;
            StartMonitorCheckbox.Text = "Start Monitor On Launch";
            StartMonitorCheckbox.UseVisualStyleBackColor = true;
            StartMonitorCheckbox.CheckedChanged += StartMonitorCheckbox_CheckedChanged;
            // 
            // LabelJournal
            // 
            LabelJournal.AutoSize = true;
            LabelJournal.Location = new Point(19, 25);
            LabelJournal.Name = "LabelJournal";
            LabelJournal.Size = new Size(84, 15);
            LabelJournal.TabIndex = 12;
            LabelJournal.Text = "Journal Folder:";
            // 
            // ThemeLabel
            // 
            ThemeLabel.AutoSize = true;
            ThemeLabel.Location = new Point(57, 79);
            ThemeLabel.Name = "ThemeLabel";
            ThemeLabel.Size = new Size(46, 15);
            ThemeLabel.TabIndex = 9;
            ThemeLabel.Text = "Theme:";
            ThemeLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // VoiceVolumeLabel
            // 
            VoiceVolumeLabel.AutoSize = true;
            VoiceVolumeLabel.Location = new Point(53, 118);
            VoiceVolumeLabel.Name = "VoiceVolumeLabel";
            VoiceVolumeLabel.Size = new Size(50, 15);
            VoiceVolumeLabel.TabIndex = 0;
            VoiceVolumeLabel.Text = "Volume:";
            VoiceVolumeLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // LabelJournalPath
            // 
            LabelJournalPath.Font = new Font("Segoe UI", 8.25F);
            LabelJournalPath.Location = new Point(117, 25);
            LabelJournalPath.Name = "LabelJournalPath";
            LabelJournalPath.Size = new Size(327, 13);
            LabelJournalPath.TabIndex = 20;
            LabelJournalPath.Text = "X:\\Journal";
            LabelJournalPath.DoubleClick += LabelJournalPath_DoubleClick;
            // 
            // ThemeDropdown
            // 
            ThemeDropdown.DropDownStyle = ComboBoxStyle.DropDownList;
            ThemeDropdown.FormattingEnabled = true;
            ThemeDropdown.Location = new Point(117, 76);
            ThemeDropdown.Name = "ThemeDropdown";
            ThemeDropdown.Size = new Size(214, 23);
            ThemeDropdown.TabIndex = 22;
            ThemeDropdown.SelectedIndexChanged += ThemeDropdown_SelectedIndexChanged;
            // 
            // ButtonAddTheme
            // 
            ButtonAddTheme.FlatAppearance.BorderSize = 0;
            ButtonAddTheme.FlatStyle = FlatStyle.Flat;
            ButtonAddTheme.Location = new Point(344, 74);
            ButtonAddTheme.Name = "ButtonAddTheme";
            ButtonAddTheme.Size = new Size(88, 23);
            ButtonAddTheme.TabIndex = 23;
            ButtonAddTheme.Text = "Add Theme";
            ButtonAddTheme.UseVisualStyleBackColor = true;
            ButtonAddTheme.Click += ButtonAddTheme_Click;
            // 
            // VoiceSettingsPanel
            // 
            VoiceSettingsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            VoiceSettingsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            VoiceSettingsPanel.BorderStyle = BorderStyle.FixedSingle;
            VoiceSettingsPanel.Controls.Add(VoiceTestButton);
            VoiceSettingsPanel.Controls.Add(AudioLabel);
            VoiceSettingsPanel.Controls.Add(VoiceCheckbox);
            VoiceSettingsPanel.Controls.Add(VoiceSpeedSlider);
            VoiceSettingsPanel.Controls.Add(AudioTypeLabel);
            VoiceSettingsPanel.Controls.Add(VoiceSpeedLabel);
            VoiceSettingsPanel.Controls.Add(AudioTypeDropdown);
            VoiceSettingsPanel.Controls.Add(VoiceLabel);
            VoiceSettingsPanel.Controls.Add(VoiceDropdown);
            VoiceSettingsPanel.Controls.Add(VoiceDisabledPanel);
            VoiceSettingsPanel.Location = new Point(12, 480);
            VoiceSettingsPanel.Name = "VoiceSettingsPanel";
            VoiceSettingsPanel.Size = new Size(468, 173);
            VoiceSettingsPanel.TabIndex = 35;
            // 
            // VoiceTestButton
            // 
            VoiceTestButton.FlatAppearance.BorderSize = 0;
            VoiceTestButton.FlatStyle = FlatStyle.Flat;
            VoiceTestButton.Location = new Point(188, 130);
            VoiceTestButton.Name = "VoiceTestButton";
            VoiceTestButton.Size = new Size(51, 23);
            VoiceTestButton.TabIndex = 13;
            VoiceTestButton.Text = "Test";
            VoiceTestButton.UseVisualStyleBackColor = false;
            VoiceTestButton.Click += VoiceTestButton_Click;
            // 
            // AudioLabel
            // 
            AudioLabel.AutoSize = true;
            AudioLabel.Location = new Point(0, 0);
            AudioLabel.Name = "AudioLabel";
            AudioLabel.Size = new Size(110, 15);
            AudioLabel.TabIndex = 31;
            AudioLabel.Text = "Audio Notifications";
            // 
            // VoiceCheckbox
            // 
            VoiceCheckbox.AutoSize = true;
            VoiceCheckbox.Location = new Point(118, 133);
            VoiceCheckbox.Name = "VoiceCheckbox";
            VoiceCheckbox.Size = new Size(68, 19);
            VoiceCheckbox.TabIndex = 11;
            VoiceCheckbox.Text = "Enabled";
            VoiceCheckbox.UseVisualStyleBackColor = true;
            VoiceCheckbox.CheckedChanged += VoiceCheckbox_CheckedChanged;
            // 
            // VoiceSpeedSlider
            // 
            VoiceSpeedSlider.Location = new Point(117, 85);
            VoiceSpeedSlider.Minimum = -10;
            VoiceSpeedSlider.Name = "VoiceSpeedSlider";
            VoiceSpeedSlider.Size = new Size(214, 45);
            VoiceSpeedSlider.TabIndex = 15;
            VoiceSpeedSlider.TickStyle = TickStyle.Both;
            VoiceSpeedSlider.Value = 10;
            VoiceSpeedSlider.Scroll += VoiceSpeedSlider_Scroll;
            // 
            // AudioTypeLabel
            // 
            AudioTypeLabel.AutoSize = true;
            AudioTypeLabel.Location = new Point(69, 27);
            AudioTypeLabel.Name = "AudioTypeLabel";
            AudioTypeLabel.Size = new Size(34, 15);
            AudioTypeLabel.TabIndex = 1;
            AudioTypeLabel.Text = "Type:";
            // 
            // VoiceSpeedLabel
            // 
            VoiceSpeedLabel.AutoSize = true;
            VoiceSpeedLabel.Location = new Point(62, 98);
            VoiceSpeedLabel.Name = "VoiceSpeedLabel";
            VoiceSpeedLabel.Size = new Size(42, 15);
            VoiceSpeedLabel.TabIndex = 1;
            VoiceSpeedLabel.Text = "Speed:";
            VoiceSpeedLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // AudioTypeDropdown
            // 
            AudioTypeDropdown.DropDownStyle = ComboBoxStyle.DropDownList;
            AudioTypeDropdown.FormattingEnabled = true;
            AudioTypeDropdown.Items.AddRange(new object[] { "Voice", "Chime" });
            AudioTypeDropdown.Location = new Point(118, 24);
            AudioTypeDropdown.Name = "AudioTypeDropdown";
            AudioTypeDropdown.Size = new Size(213, 23);
            AudioTypeDropdown.TabIndex = 2;
            AudioTypeDropdown.SelectedIndexChanged += AudioTypeDropdown_SelectedIndexChanged;
            // 
            // VoiceLabel
            // 
            VoiceLabel.AutoSize = true;
            VoiceLabel.Location = new Point(65, 56);
            VoiceLabel.Name = "VoiceLabel";
            VoiceLabel.Size = new Size(38, 15);
            VoiceLabel.TabIndex = 4;
            VoiceLabel.Text = "Voice:";
            VoiceLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // VoiceDropdown
            // 
            VoiceDropdown.DropDownStyle = ComboBoxStyle.DropDownList;
            VoiceDropdown.FormattingEnabled = true;
            VoiceDropdown.Location = new Point(117, 53);
            VoiceDropdown.Name = "VoiceDropdown";
            VoiceDropdown.Size = new Size(214, 23);
            VoiceDropdown.TabIndex = 16;
            VoiceDropdown.SelectedIndexChanged += VoiceDropdown_SelectedIndexChanged;
            // 
            // VoiceDisabledPanel
            // 
            VoiceDisabledPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            VoiceDisabledPanel.Controls.Add(VoiceDisabledLabel);
            VoiceDisabledPanel.Enabled = false;
            VoiceDisabledPanel.Location = new Point(4, 18);
            VoiceDisabledPanel.Name = "VoiceDisabledPanel";
            VoiceDisabledPanel.Size = new Size(460, 150);
            VoiceDisabledPanel.TabIndex = 16;
            VoiceDisabledPanel.Visible = false;
            // 
            // VoiceDisabledLabel
            // 
            VoiceDisabledLabel.AutoSize = true;
            VoiceDisabledLabel.Location = new Point(19, 12);
            VoiceDisabledLabel.Name = "VoiceDisabledLabel";
            VoiceDisabledLabel.Size = new Size(141, 15);
            VoiceDisabledLabel.TabIndex = 0;
            VoiceDisabledLabel.Text = "Placeholder Disabled Text";
            // 
            // CoreSettingsOK
            // 
            CoreSettingsOK.FlatAppearance.BorderSize = 0;
            CoreSettingsOK.FlatStyle = FlatStyle.Flat;
            CoreSettingsOK.Location = new Point(405, 659);
            CoreSettingsOK.Name = "CoreSettingsOK";
            CoreSettingsOK.Size = new Size(75, 23);
            CoreSettingsOK.TabIndex = 36;
            CoreSettingsOK.Text = "Close";
            CoreSettingsOK.UseVisualStyleBackColor = false;
            CoreSettingsOK.Click += CoreSettingsOK_Click;
            // 
            // CoreSettings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(492, 695);
            Controls.Add(CoreSettingsOK);
            Controls.Add(VoiceSettingsPanel);
            Controls.Add(CoreSettingsPanel);
            Controls.Add(PopupSettingsPanel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CoreSettings";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Observatory Core Settings";
            PopupSettingsPanel.ResumeLayout(false);
            PopupSettingsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)FontScaleSpinner).EndInit();
            ((System.ComponentModel.ISupportInitialize)DurationSpinner).EndInit();
            ((System.ComponentModel.ISupportInitialize)ScaleSpinner).EndInit();
            PopupDisabledPanel.ResumeLayout(false);
            PopupDisabledPanel.PerformLayout();
            CoreSettingsPanel.ResumeLayout(false);
            CoreSettingsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)AudioVolumeSlider).EndInit();
            VoiceSettingsPanel.ResumeLayout(false);
            VoiceSettingsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)VoiceSpeedSlider).EndInit();
            VoiceDisabledPanel.ResumeLayout(false);
            VoiceDisabledPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel PopupSettingsPanel;
        private NumericUpDown FontScaleSpinner;
        private CheckBox PopupTransparentCheckBox;
        private Label LabelFontScale;
        private NumericUpDown DurationSpinner;
        private Label DisplayLabel;
        private Label CornerLabel;
        private Label LabelFont;
        private Label LabelScale;
        private Label LabelDuration;
        private Label LabelColour;
        private NumericUpDown ScaleSpinner;
        private Label PopupLabel;
        private ComboBox FontDropdown;
        private Button TestButton;
        private ComboBox CornerDropdown;
        private Button ColourButton;
        private ComboBox DisplayDropdown;
        private CheckBox PopupCheckbox;
        private Panel PopupDisabledPanel;
        private Label PopupDisabledLabel;
        private Panel CoreSettingsPanel;
        private Label AudioDeviceLabel;
        private ComboBox AudioDeviceDropdown;
        private Label ExportFormatLabel;
        private Button CoreConfigFolder;
        private TrackBar AudioVolumeSlider;
        private ComboBox ExportFormatDropdown;
        private Label CoreSettingsLabel;
        private CheckBox StartReadallCheckbox;
        private CheckBox StartMonitorCheckbox;
        private Label LabelJournal;
        private Label ThemeLabel;
        private Label VoiceVolumeLabel;
        private Label LabelJournalPath;
        private ComboBox ThemeDropdown;
        private Button ButtonAddTheme;
        private Panel VoiceSettingsPanel;
        private TrackBar VoiceSpeedSlider;
        private Button VoiceTestButton;
        private CheckBox VoiceCheckbox;
        private Label VoiceLabel;
        private Label VoiceSpeedLabel;
        private Label AudioLabel;
        private ComboBox VoiceDropdown;
        private Panel VoiceDisabledPanel;
        private Label VoiceDisabledLabel;
        private Button CoreSettingsOK;
        private ColorDialog PopupColour;
        private ComboBox AudioTypeDropdown;
        private Label AudioTypeLabel;
    }
}