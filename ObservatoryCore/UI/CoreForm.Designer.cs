﻿namespace Observatory.UI
{
    partial class CoreForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CoreForm));
            CoreMenu = new MenuStrip();
            coreToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripMenuItem();
            CorePanel = new Panel();
            CoreSettingsLabel = new Label();
            CoreSettingsPanel = new Panel();
            StartReadallCheckbox = new CheckBox();
            StartMonitorCheckbox = new CheckBox();
            LabelJournal = new Label();
            ThemeLabel = new Label();
            LabelJournalPath = new Label();
            ThemeDropdown = new ComboBox();
            ButtonAddTheme = new Button();
            AudioLabel = new Label();
            PopupLabel = new Label();
            PluginSettingsButton = new Button();
            VoiceSettingsPanel = new Panel();
            VoiceSpeedSlider = new TrackBar();
            VoiceVolumeSlider = new TrackBar();
            VoiceTestButton = new Button();
            VoiceCheckbox = new CheckBox();
            VoiceDropdown = new ComboBox();
            VoiceLabel = new Label();
            VoiceSpeedLabel = new Label();
            VoiceVolumeLabel = new Label();
            VoiceDisabledPanel = new Panel();
            VoiceDisabledLabel = new Label();
            PopupSettingsPanel = new Panel();
            DurationSpinner = new NumericUpDown();
            ScaleSpinner = new NumericUpDown();
            LabelColour = new Label();
            TestButton = new Button();
            ColourButton = new Button();
            PopupCheckbox = new CheckBox();
            LabelDuration = new Label();
            LabelScale = new Label();
            FontDropdown = new ComboBox();
            LabelFont = new Label();
            CornerDropdown = new ComboBox();
            DisplayDropdown = new ComboBox();
            CornerLabel = new Label();
            DisplayLabel = new Label();
            PopupDisabledPanel = new Panel();
            PopupDisabledLabel = new Label();
            PluginFolderButton = new Button();
            PluginList = new NoHScrollList();
            NameColumn = new ColumnHeader();
            TypeColumn = new ColumnHeader();
            VersionColumn = new ColumnHeader();
            StatusColumn = new ColumnHeader();
            ReadAllButton = new Button();
            ToggleMonitorButton = new Button();
            ClearButton = new Button();
            ExportButton = new Button();
            GithubLink = new LinkLabel();
            DonateLink = new LinkLabel();
            PopupColour = new ColorDialog();
            OverrideTooltip = new ToolTip(components);
            CoreMenu.SuspendLayout();
            CorePanel.SuspendLayout();
            CoreSettingsPanel.SuspendLayout();
            VoiceSettingsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)VoiceSpeedSlider).BeginInit();
            ((System.ComponentModel.ISupportInitialize)VoiceVolumeSlider).BeginInit();
            VoiceDisabledPanel.SuspendLayout();
            PopupSettingsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DurationSpinner).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ScaleSpinner).BeginInit();
            PopupDisabledPanel.SuspendLayout();
            SuspendLayout();
            // 
            // CoreMenu
            // 
            CoreMenu.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            CoreMenu.AutoSize = false;
            CoreMenu.Dock = DockStyle.None;
            CoreMenu.Items.AddRange(new ToolStripItem[] { coreToolStripMenuItem, toolStripMenuItem1 });
            CoreMenu.LayoutStyle = ToolStripLayoutStyle.VerticalStackWithOverflow;
            CoreMenu.Location = new Point(0, 0);
            CoreMenu.Name = "CoreMenu";
            CoreMenu.Size = new Size(120, 762);
            CoreMenu.TabIndex = 0;
            // 
            // coreToolStripMenuItem
            // 
            coreToolStripMenuItem.Font = new Font("Segoe UI", 18F);
            coreToolStripMenuItem.Name = "coreToolStripMenuItem";
            coreToolStripMenuItem.Size = new Size(113, 36);
            coreToolStripMenuItem.Text = "Core";
            coreToolStripMenuItem.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Alignment = ToolStripItemAlignment.Right;
            toolStripMenuItem1.Font = new Font("Segoe UI", 18F);
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(113, 36);
            toolStripMenuItem1.Text = "<";
            toolStripMenuItem1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // CorePanel
            // 
            CorePanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            CorePanel.AutoScroll = true;
            CorePanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            CorePanel.Controls.Add(CoreSettingsLabel);
            CorePanel.Controls.Add(CoreSettingsPanel);
            CorePanel.Controls.Add(AudioLabel);
            CorePanel.Controls.Add(PopupLabel);
            CorePanel.Controls.Add(PluginSettingsButton);
            CorePanel.Controls.Add(VoiceSettingsPanel);
            CorePanel.Controls.Add(PopupSettingsPanel);
            CorePanel.Controls.Add(PluginFolderButton);
            CorePanel.Controls.Add(PluginList);
            CorePanel.Location = new Point(123, 12);
            CorePanel.Name = "CorePanel";
            CorePanel.Size = new Size(665, 750);
            CorePanel.TabIndex = 1;
            // 
            // CoreSettingsLabel
            // 
            CoreSettingsLabel.AutoSize = true;
            CoreSettingsLabel.Location = new Point(5, 614);
            CoreSettingsLabel.Name = "CoreSettingsLabel";
            CoreSettingsLabel.Size = new Size(77, 15);
            CoreSettingsLabel.TabIndex = 15;
            CoreSettingsLabel.Text = "Core Settings";
            // 
            // CoreSettingsPanel
            // 
            CoreSettingsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            CoreSettingsPanel.BorderStyle = BorderStyle.FixedSingle;
            CoreSettingsPanel.Controls.Add(StartReadallCheckbox);
            CoreSettingsPanel.Controls.Add(StartMonitorCheckbox);
            CoreSettingsPanel.Controls.Add(LabelJournal);
            CoreSettingsPanel.Controls.Add(ThemeLabel);
            CoreSettingsPanel.Controls.Add(LabelJournalPath);
            CoreSettingsPanel.Controls.Add(ThemeDropdown);
            CoreSettingsPanel.Controls.Add(ButtonAddTheme);
            CoreSettingsPanel.Location = new Point(3, 621);
            CoreSettingsPanel.Name = "CoreSettingsPanel";
            CoreSettingsPanel.Size = new Size(659, 124);
            CoreSettingsPanel.TabIndex = 14;
            CoreSettingsPanel.Tag = "";
            // 
            // StartReadallCheckbox
            // 
            StartReadallCheckbox.AutoSize = true;
            StartReadallCheckbox.Location = new Point(121, 94);
            StartReadallCheckbox.Name = "StartReadallCheckbox";
            StartReadallCheckbox.Size = new Size(130, 19);
            StartReadallCheckbox.TabIndex = 15;
            StartReadallCheckbox.Text = "Read All On Launch";
            StartReadallCheckbox.UseVisualStyleBackColor = true;
            StartReadallCheckbox.CheckedChanged += StartReadallCheckbox_CheckedChanged;
            // 
            // StartMonitorCheckbox
            // 
            StartMonitorCheckbox.AutoSize = true;
            StartMonitorCheckbox.Location = new Point(121, 69);
            StartMonitorCheckbox.Name = "StartMonitorCheckbox";
            StartMonitorCheckbox.Size = new Size(157, 19);
            StartMonitorCheckbox.TabIndex = 14;
            StartMonitorCheckbox.Text = "Start Monitor On Launch";
            StartMonitorCheckbox.UseVisualStyleBackColor = true;
            StartMonitorCheckbox.CheckedChanged += StartMonitorCheckbox_CheckedChanged;
            // 
            // LabelJournal
            // 
            LabelJournal.AutoSize = true;
            LabelJournal.Location = new Point(30, 15);
            LabelJournal.Name = "LabelJournal";
            LabelJournal.Size = new Size(84, 15);
            LabelJournal.TabIndex = 12;
            LabelJournal.Text = "Journal Folder:";
            // 
            // ThemeLabel
            // 
            ThemeLabel.AutoSize = true;
            ThemeLabel.Location = new Point(68, 43);
            ThemeLabel.Name = "ThemeLabel";
            ThemeLabel.Size = new Size(46, 15);
            ThemeLabel.TabIndex = 9;
            ThemeLabel.Text = "Theme:";
            ThemeLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LabelJournalPath
            // 
            LabelJournalPath.Font = new Font("Segoe UI", 8.25F);
            LabelJournalPath.Location = new Point(120, 16);
            LabelJournalPath.Name = "LabelJournalPath";
            LabelJournalPath.Size = new Size(526, 13);
            LabelJournalPath.TabIndex = 13;
            LabelJournalPath.Text = "X:\\Journal";
            LabelJournalPath.DoubleClick += LabelJournalPath_DoubleClick;
            // 
            // ThemeDropdown
            // 
            ThemeDropdown.DropDownStyle = ComboBoxStyle.DropDownList;
            ThemeDropdown.FormattingEnabled = true;
            ThemeDropdown.Location = new Point(120, 40);
            ThemeDropdown.Name = "ThemeDropdown";
            ThemeDropdown.Size = new Size(121, 23);
            ThemeDropdown.TabIndex = 10;
            ThemeDropdown.SelectedIndexChanged += ThemeDropdown_SelectedIndexChanged;
            // 
            // ButtonAddTheme
            // 
            ButtonAddTheme.FlatAppearance.BorderSize = 0;
            ButtonAddTheme.FlatStyle = FlatStyle.Flat;
            ButtonAddTheme.Location = new Point(247, 40);
            ButtonAddTheme.Name = "ButtonAddTheme";
            ButtonAddTheme.Size = new Size(88, 23);
            ButtonAddTheme.TabIndex = 11;
            ButtonAddTheme.Text = "Add Theme";
            ButtonAddTheme.UseVisualStyleBackColor = true;
            ButtonAddTheme.Click += ButtonAddTheme_Click;
            // 
            // AudioLabel
            // 
            AudioLabel.AutoSize = true;
            AudioLabel.Location = new Point(5, 432);
            AudioLabel.Name = "AudioLabel";
            AudioLabel.Size = new Size(106, 15);
            AudioLabel.TabIndex = 8;
            AudioLabel.Text = "Voice Notifications";
            // 
            // PopupLabel
            // 
            PopupLabel.AutoSize = true;
            PopupLabel.Location = new Point(5, 215);
            PopupLabel.Name = "PopupLabel";
            PopupLabel.Size = new Size(113, 15);
            PopupLabel.TabIndex = 7;
            PopupLabel.Text = "Popup Notifications";
            // 
            // PluginSettingsButton
            // 
            PluginSettingsButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            PluginSettingsButton.FlatAppearance.BorderSize = 0;
            PluginSettingsButton.FlatStyle = FlatStyle.Flat;
            PluginSettingsButton.Location = new Point(406, 193);
            PluginSettingsButton.Name = "PluginSettingsButton";
            PluginSettingsButton.Size = new Size(120, 23);
            PluginSettingsButton.TabIndex = 6;
            PluginSettingsButton.Text = "Plugin Settings";
            PluginSettingsButton.UseVisualStyleBackColor = false;
            PluginSettingsButton.Click += PluginSettingsButton_Click;
            // 
            // VoiceSettingsPanel
            // 
            VoiceSettingsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            VoiceSettingsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            VoiceSettingsPanel.BorderStyle = BorderStyle.FixedSingle;
            VoiceSettingsPanel.Controls.Add(VoiceSpeedSlider);
            VoiceSettingsPanel.Controls.Add(VoiceVolumeSlider);
            VoiceSettingsPanel.Controls.Add(VoiceTestButton);
            VoiceSettingsPanel.Controls.Add(VoiceCheckbox);
            VoiceSettingsPanel.Controls.Add(VoiceDropdown);
            VoiceSettingsPanel.Controls.Add(VoiceLabel);
            VoiceSettingsPanel.Controls.Add(VoiceSpeedLabel);
            VoiceSettingsPanel.Controls.Add(VoiceVolumeLabel);
            VoiceSettingsPanel.Controls.Add(VoiceDisabledPanel);
            VoiceSettingsPanel.Location = new Point(3, 441);
            VoiceSettingsPanel.Name = "VoiceSettingsPanel";
            VoiceSettingsPanel.Size = new Size(659, 170);
            VoiceSettingsPanel.TabIndex = 5;
            // 
            // VoiceSpeedSlider
            // 
            VoiceSpeedSlider.Location = new Point(121, 51);
            VoiceSpeedSlider.Minimum = 1;
            VoiceSpeedSlider.Name = "VoiceSpeedSlider";
            VoiceSpeedSlider.Size = new Size(120, 45);
            VoiceSpeedSlider.TabIndex = 15;
            VoiceSpeedSlider.TickStyle = TickStyle.Both;
            VoiceSpeedSlider.Value = 10;
            VoiceSpeedSlider.Scroll += VoiceSpeedSlider_Scroll;
            // 
            // VoiceVolumeSlider
            // 
            VoiceVolumeSlider.LargeChange = 10;
            VoiceVolumeSlider.Location = new Point(120, 0);
            VoiceVolumeSlider.Maximum = 100;
            VoiceVolumeSlider.Name = "VoiceVolumeSlider";
            VoiceVolumeSlider.Size = new Size(121, 45);
            VoiceVolumeSlider.TabIndex = 14;
            VoiceVolumeSlider.TickFrequency = 10;
            VoiceVolumeSlider.TickStyle = TickStyle.Both;
            VoiceVolumeSlider.Value = 100;
            VoiceVolumeSlider.Scroll += VoiceVolumeSlider_Scroll;
            // 
            // VoiceTestButton
            // 
            VoiceTestButton.FlatAppearance.BorderSize = 0;
            VoiceTestButton.FlatStyle = FlatStyle.Flat;
            VoiceTestButton.Location = new Point(190, 131);
            VoiceTestButton.Name = "VoiceTestButton";
            VoiceTestButton.Size = new Size(51, 23);
            VoiceTestButton.TabIndex = 13;
            VoiceTestButton.Text = "Test";
            VoiceTestButton.UseVisualStyleBackColor = false;
            VoiceTestButton.Click += VoiceTestButton_Click;
            // 
            // VoiceCheckbox
            // 
            VoiceCheckbox.AutoSize = true;
            VoiceCheckbox.Location = new Point(120, 134);
            VoiceCheckbox.Name = "VoiceCheckbox";
            VoiceCheckbox.Size = new Size(68, 19);
            VoiceCheckbox.TabIndex = 11;
            VoiceCheckbox.Text = "Enabled";
            VoiceCheckbox.UseVisualStyleBackColor = true;
            VoiceCheckbox.CheckedChanged += VoiceCheckbox_CheckedChanged;
            // 
            // VoiceDropdown
            // 
            VoiceDropdown.DropDownStyle = ComboBoxStyle.DropDownList;
            VoiceDropdown.FormattingEnabled = true;
            VoiceDropdown.Location = new Point(121, 102);
            VoiceDropdown.Name = "VoiceDropdown";
            VoiceDropdown.Size = new Size(121, 23);
            VoiceDropdown.TabIndex = 5;
            VoiceDropdown.SelectedIndexChanged += VoiceDropdown_SelectedIndexChanged;
            // 
            // VoiceLabel
            // 
            VoiceLabel.AutoSize = true;
            VoiceLabel.Location = new Point(77, 105);
            VoiceLabel.Name = "VoiceLabel";
            VoiceLabel.Size = new Size(38, 15);
            VoiceLabel.TabIndex = 4;
            VoiceLabel.Text = "Voice:";
            VoiceLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // VoiceSpeedLabel
            // 
            VoiceSpeedLabel.AutoSize = true;
            VoiceSpeedLabel.Location = new Point(73, 63);
            VoiceSpeedLabel.Name = "VoiceSpeedLabel";
            VoiceSpeedLabel.Size = new Size(42, 15);
            VoiceSpeedLabel.TabIndex = 1;
            VoiceSpeedLabel.Text = "Speed:";
            VoiceSpeedLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // VoiceVolumeLabel
            // 
            VoiceVolumeLabel.AutoSize = true;
            VoiceVolumeLabel.Location = new Point(64, 12);
            VoiceVolumeLabel.Name = "VoiceVolumeLabel";
            VoiceVolumeLabel.Size = new Size(50, 15);
            VoiceVolumeLabel.TabIndex = 0;
            VoiceVolumeLabel.Text = "Volume:";
            VoiceVolumeLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // VoiceDisabledPanel
            // 
            VoiceDisabledPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            VoiceDisabledPanel.Controls.Add(VoiceDisabledLabel);
            VoiceDisabledPanel.Enabled = false;
            VoiceDisabledPanel.Location = new Point(3, 3);
            VoiceDisabledPanel.Name = "VoiceDisabledPanel";
            VoiceDisabledPanel.Size = new Size(651, 162);
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
            // PopupSettingsPanel
            // 
            PopupSettingsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            PopupSettingsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            PopupSettingsPanel.BorderStyle = BorderStyle.FixedSingle;
            PopupSettingsPanel.Controls.Add(DurationSpinner);
            PopupSettingsPanel.Controls.Add(ScaleSpinner);
            PopupSettingsPanel.Controls.Add(LabelColour);
            PopupSettingsPanel.Controls.Add(TestButton);
            PopupSettingsPanel.Controls.Add(ColourButton);
            PopupSettingsPanel.Controls.Add(PopupCheckbox);
            PopupSettingsPanel.Controls.Add(LabelDuration);
            PopupSettingsPanel.Controls.Add(LabelScale);
            PopupSettingsPanel.Controls.Add(FontDropdown);
            PopupSettingsPanel.Controls.Add(LabelFont);
            PopupSettingsPanel.Controls.Add(CornerDropdown);
            PopupSettingsPanel.Controls.Add(DisplayDropdown);
            PopupSettingsPanel.Controls.Add(CornerLabel);
            PopupSettingsPanel.Controls.Add(DisplayLabel);
            PopupSettingsPanel.Controls.Add(PopupDisabledPanel);
            PopupSettingsPanel.Location = new Point(3, 224);
            PopupSettingsPanel.Name = "PopupSettingsPanel";
            PopupSettingsPanel.Size = new Size(659, 207);
            PopupSettingsPanel.TabIndex = 3;
            // 
            // DurationSpinner
            // 
            DurationSpinner.Increment = new decimal(new int[] { 25, 0, 0, 0 });
            DurationSpinner.Location = new Point(121, 123);
            DurationSpinner.Maximum = new decimal(new int[] { 60000, 0, 0, 0 });
            DurationSpinner.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
            DurationSpinner.Name = "DurationSpinner";
            DurationSpinner.Size = new Size(120, 23);
            DurationSpinner.TabIndex = 15;
            DurationSpinner.Value = new decimal(new int[] { 8000, 0, 0, 0 });
            DurationSpinner.ValueChanged += DurationSpinner_ValueChanged;
            // 
            // ScaleSpinner
            // 
            ScaleSpinner.Location = new Point(121, 94);
            ScaleSpinner.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            ScaleSpinner.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            ScaleSpinner.Name = "ScaleSpinner";
            ScaleSpinner.Size = new Size(120, 23);
            ScaleSpinner.TabIndex = 14;
            ScaleSpinner.Value = new decimal(new int[] { 100, 0, 0, 0 });
            ScaleSpinner.ValueChanged += ScaleSpinner_ValueChanged;
            // 
            // LabelColour
            // 
            LabelColour.AutoSize = true;
            LabelColour.Location = new Point(68, 156);
            LabelColour.Name = "LabelColour";
            LabelColour.Size = new Size(46, 15);
            LabelColour.TabIndex = 13;
            LabelColour.Text = "Colour:";
            LabelColour.TextAlign = ContentAlignment.MiddleRight;
            // 
            // TestButton
            // 
            TestButton.FlatAppearance.BorderSize = 0;
            TestButton.FlatStyle = FlatStyle.Flat;
            TestButton.Location = new Point(190, 152);
            TestButton.Name = "TestButton";
            TestButton.Size = new Size(51, 23);
            TestButton.TabIndex = 12;
            TestButton.Text = "Test";
            TestButton.UseVisualStyleBackColor = false;
            TestButton.Click += TestButton_Click;
            // 
            // ColourButton
            // 
            ColourButton.FlatStyle = FlatStyle.Flat;
            ColourButton.Location = new Point(121, 152);
            ColourButton.Name = "ColourButton";
            ColourButton.Size = new Size(51, 23);
            ColourButton.TabIndex = 11;
            ColourButton.UseVisualStyleBackColor = true;
            ColourButton.Click += ColourButton_Click;
            // 
            // PopupCheckbox
            // 
            PopupCheckbox.AutoSize = true;
            PopupCheckbox.Location = new Point(120, 181);
            PopupCheckbox.Name = "PopupCheckbox";
            PopupCheckbox.Size = new Size(68, 19);
            PopupCheckbox.TabIndex = 10;
            PopupCheckbox.Text = "Enabled";
            PopupCheckbox.UseVisualStyleBackColor = true;
            PopupCheckbox.CheckedChanged += PopupCheckbox_CheckedChanged;
            // 
            // LabelDuration
            // 
            LabelDuration.AutoSize = true;
            LabelDuration.Location = new Point(32, 125);
            LabelDuration.Name = "LabelDuration";
            LabelDuration.Size = new Size(83, 15);
            LabelDuration.TabIndex = 9;
            LabelDuration.Text = "Duration (ms):";
            LabelDuration.TextAlign = ContentAlignment.MiddleRight;
            // 
            // LabelScale
            // 
            LabelScale.AutoSize = true;
            LabelScale.Location = new Point(57, 96);
            LabelScale.Name = "LabelScale";
            LabelScale.Size = new Size(58, 15);
            LabelScale.TabIndex = 7;
            LabelScale.Text = "Scale (%):";
            LabelScale.TextAlign = ContentAlignment.MiddleRight;
            // 
            // FontDropdown
            // 
            FontDropdown.DropDownStyle = ComboBoxStyle.DropDownList;
            FontDropdown.FormattingEnabled = true;
            FontDropdown.Location = new Point(120, 65);
            FontDropdown.Name = "FontDropdown";
            FontDropdown.Size = new Size(242, 23);
            FontDropdown.TabIndex = 5;
            FontDropdown.SelectedIndexChanged += FontDropdown_SelectedIndexChanged;
            // 
            // LabelFont
            // 
            LabelFont.AutoSize = true;
            LabelFont.Location = new Point(80, 68);
            LabelFont.Name = "LabelFont";
            LabelFont.Size = new Size(34, 15);
            LabelFont.TabIndex = 4;
            LabelFont.Text = "Font:";
            LabelFont.TextAlign = ContentAlignment.MiddleRight;
            // 
            // CornerDropdown
            // 
            CornerDropdown.DropDownStyle = ComboBoxStyle.DropDownList;
            CornerDropdown.FormattingEnabled = true;
            CornerDropdown.Items.AddRange(new object[] { "Bottom-Right", "Bottom-Left", "Top-Right", "Top-Left" });
            CornerDropdown.Location = new Point(120, 36);
            CornerDropdown.Name = "CornerDropdown";
            CornerDropdown.Size = new Size(121, 23);
            CornerDropdown.TabIndex = 3;
            CornerDropdown.SelectedIndexChanged += CornerDropdown_SelectedIndexChanged;
            // 
            // DisplayDropdown
            // 
            DisplayDropdown.DropDownStyle = ComboBoxStyle.DropDownList;
            DisplayDropdown.FormattingEnabled = true;
            DisplayDropdown.Location = new Point(120, 7);
            DisplayDropdown.Name = "DisplayDropdown";
            DisplayDropdown.Size = new Size(121, 23);
            DisplayDropdown.TabIndex = 2;
            DisplayDropdown.SelectedIndexChanged += DisplayDropdown_SelectedIndexChanged;
            // 
            // CornerLabel
            // 
            CornerLabel.AutoSize = true;
            CornerLabel.Location = new Point(68, 39);
            CornerLabel.Name = "CornerLabel";
            CornerLabel.Size = new Size(46, 15);
            CornerLabel.TabIndex = 1;
            CornerLabel.Text = "Corner:";
            CornerLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // DisplayLabel
            // 
            DisplayLabel.AutoSize = true;
            DisplayLabel.Location = new Point(66, 10);
            DisplayLabel.Name = "DisplayLabel";
            DisplayLabel.Size = new Size(48, 15);
            DisplayLabel.TabIndex = 0;
            DisplayLabel.Text = "Display:";
            DisplayLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // PopupDisabledPanel
            // 
            PopupDisabledPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            PopupDisabledPanel.Controls.Add(PopupDisabledLabel);
            PopupDisabledPanel.Enabled = false;
            PopupDisabledPanel.Location = new Point(3, 3);
            PopupDisabledPanel.Name = "PopupDisabledPanel";
            PopupDisabledPanel.Size = new Size(651, 199);
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
            // PluginFolderButton
            // 
            PluginFolderButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            PluginFolderButton.FlatAppearance.BorderSize = 0;
            PluginFolderButton.FlatStyle = FlatStyle.Flat;
            PluginFolderButton.Location = new Point(532, 193);
            PluginFolderButton.Name = "PluginFolderButton";
            PluginFolderButton.Size = new Size(130, 23);
            PluginFolderButton.TabIndex = 1;
            PluginFolderButton.Text = "Open Plugin Folder";
            PluginFolderButton.UseVisualStyleBackColor = false;
            PluginFolderButton.Click += PluginFolderButton_Click;
            // 
            // PluginList
            // 
            PluginList.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            PluginList.BorderStyle = BorderStyle.None;
            PluginList.CheckBoxes = true;
            PluginList.Columns.AddRange(new ColumnHeader[] { NameColumn, TypeColumn, VersionColumn, StatusColumn });
            PluginList.FullRowSelect = true;
            PluginList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            PluginList.ImeMode = ImeMode.NoControl;
            PluginList.Location = new Point(3, 3);
            PluginList.MultiSelect = false;
            PluginList.Name = "PluginList";
            PluginList.OwnerDraw = true;
            PluginList.Scrollable = false;
            PluginList.Size = new Size(659, 184);
            PluginList.TabIndex = 0;
            PluginList.UseCompatibleStateImageBehavior = false;
            PluginList.View = View.Details;
            PluginList.ItemChecked += PluginList_ItemChecked;
            PluginList.Resize += PluginList_Resize;
            // 
            // NameColumn
            // 
            NameColumn.Text = "Plugin";
            NameColumn.Width = 180;
            // 
            // TypeColumn
            // 
            TypeColumn.Text = "Type";
            TypeColumn.Width = 120;
            // 
            // VersionColumn
            // 
            VersionColumn.Text = "Version";
            VersionColumn.Width = 120;
            // 
            // StatusColumn
            // 
            StatusColumn.Text = "Status";
            // 
            // ReadAllButton
            // 
            ReadAllButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            ReadAllButton.FlatAppearance.BorderSize = 0;
            ReadAllButton.FlatStyle = FlatStyle.Flat;
            ReadAllButton.Location = new Point(713, 769);
            ReadAllButton.Name = "ReadAllButton";
            ReadAllButton.Size = new Size(75, 23);
            ReadAllButton.TabIndex = 2;
            ReadAllButton.Text = "Read All";
            ReadAllButton.UseVisualStyleBackColor = false;
            ReadAllButton.Click += ReadAllButton_Click;
            // 
            // ToggleMonitorButton
            // 
            ToggleMonitorButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            ToggleMonitorButton.FlatAppearance.BorderSize = 0;
            ToggleMonitorButton.FlatStyle = FlatStyle.Flat;
            ToggleMonitorButton.Location = new Point(610, 769);
            ToggleMonitorButton.Name = "ToggleMonitorButton";
            ToggleMonitorButton.Size = new Size(97, 23);
            ToggleMonitorButton.TabIndex = 3;
            ToggleMonitorButton.Text = "Start Monitor";
            ToggleMonitorButton.UseVisualStyleBackColor = false;
            ToggleMonitorButton.Click += ToggleMonitorButton_Click;
            // 
            // ClearButton
            // 
            ClearButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            ClearButton.FlatAppearance.BorderSize = 0;
            ClearButton.FlatStyle = FlatStyle.Flat;
            ClearButton.Location = new Point(529, 769);
            ClearButton.Name = "ClearButton";
            ClearButton.Size = new Size(75, 23);
            ClearButton.TabIndex = 4;
            ClearButton.Text = "Clear";
            ClearButton.UseVisualStyleBackColor = false;
            // 
            // ExportButton
            // 
            ExportButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            ExportButton.FlatAppearance.BorderSize = 0;
            ExportButton.FlatStyle = FlatStyle.Flat;
            ExportButton.Location = new Point(448, 769);
            ExportButton.Name = "ExportButton";
            ExportButton.Size = new Size(75, 23);
            ExportButton.TabIndex = 5;
            ExportButton.Text = "Export";
            ExportButton.UseVisualStyleBackColor = false;
            ExportButton.Click += ExportButton_Click;
            // 
            // GithubLink
            // 
            GithubLink.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            GithubLink.AutoSize = true;
            GithubLink.Location = new Point(12, 765);
            GithubLink.Name = "GithubLink";
            GithubLink.Size = new Size(42, 15);
            GithubLink.TabIndex = 6;
            GithubLink.TabStop = true;
            GithubLink.Text = "github";
            GithubLink.LinkClicked += GithubLink_LinkClicked;
            // 
            // DonateLink
            // 
            DonateLink.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            DonateLink.AutoSize = true;
            DonateLink.Location = new Point(12, 780);
            DonateLink.Name = "DonateLink";
            DonateLink.Size = new Size(45, 15);
            DonateLink.TabIndex = 7;
            DonateLink.TabStop = true;
            DonateLink.Text = "Donate";
            DonateLink.LinkClicked += DonateLink_LinkClicked;
            // 
            // CoreForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 804);
            Controls.Add(DonateLink);
            Controls.Add(GithubLink);
            Controls.Add(ExportButton);
            Controls.Add(ClearButton);
            Controls.Add(ToggleMonitorButton);
            Controls.Add(ReadAllButton);
            Controls.Add(CorePanel);
            Controls.Add(CoreMenu);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = CoreMenu;
            MinimumSize = new Size(600, 300);
            Name = "CoreForm";
            Text = "Elite Observatory Core";
            FormClosing += CoreForm_FormClosing;
            Load += CoreForm_Load;
            Shown += CoreForm_Shown;
            Resize += CoreForm_Resize;
            CoreMenu.ResumeLayout(false);
            CoreMenu.PerformLayout();
            CorePanel.ResumeLayout(false);
            CorePanel.PerformLayout();
            CoreSettingsPanel.ResumeLayout(false);
            CoreSettingsPanel.PerformLayout();
            VoiceSettingsPanel.ResumeLayout(false);
            VoiceSettingsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)VoiceSpeedSlider).EndInit();
            ((System.ComponentModel.ISupportInitialize)VoiceVolumeSlider).EndInit();
            VoiceDisabledPanel.ResumeLayout(false);
            VoiceDisabledPanel.PerformLayout();
            PopupSettingsPanel.ResumeLayout(false);
            PopupSettingsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)DurationSpinner).EndInit();
            ((System.ComponentModel.ISupportInitialize)ScaleSpinner).EndInit();
            PopupDisabledPanel.ResumeLayout(false);
            PopupDisabledPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip CoreMenu;
        private ToolStripMenuItem coreToolStripMenuItem;
        private Panel CorePanel;
        private Button ReadAllButton;
        private Button ToggleMonitorButton;
        private Button ClearButton;
        private Button ExportButton;
        private LinkLabel GithubLink;
        private LinkLabel DonateLink;
        private NoHScrollList PluginList;
        private ColumnHeader NameColumn;
        private ColumnHeader TypeColumn;
        private ColumnHeader VersionColumn;
        private ColumnHeader StatusColumn;
        private Button PluginFolderButton;
        private Panel PopupSettingsPanel;
        private ComboBox CornerDropdown;
        private ComboBox DisplayDropdown;
        private Label CornerLabel;
        private Label DisplayLabel;
        private NumericUpDown DurationSpinner;
        private NumericUpDown ScaleSpinner;
        private Label LabelColour;
        private Button TestButton;
        private Button ColourButton;
        private CheckBox PopupCheckbox;
        private Label LabelDuration;
        private Label LabelScale;
        private ComboBox FontDropdown;
        private Label LabelFont;
        private ColorDialog PopupColour;
        private ToolStripMenuItem toolStripMenuItem1;
        private Panel VoiceSettingsPanel;
        private TrackBar VoiceSpeedSlider;
        private TrackBar VoiceVolumeSlider;
        private Button VoiceTestButton;
        private CheckBox VoiceCheckbox;
        private ComboBox VoiceDropdown;
        private Label VoiceLabel;
        private Label VoiceSpeedLabel;
        private Label VoiceVolumeLabel;
        private Button PluginSettingsButton;
        private ToolTip OverrideTooltip;
        private Label AudioLabel;
        private Label PopupLabel;
        private Label ThemeLabel;
        private ComboBox ThemeDropdown;
        private Button ButtonAddTheme;
        private Label LabelJournal;
        private Label LabelJournalPath;
        private Panel VoiceDisabledPanel;
        private Label VoiceDisabledLabel;
        private Panel PopupDisabledPanel;
        private Label PopupDisabledLabel;
        private Label CoreSettingsLabel;
        private Panel CoreSettingsPanel;
        private CheckBox StartReadallCheckbox;
        private CheckBox StartMonitorCheckbox;
    }
}