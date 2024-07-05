namespace Observatory.UI
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
            ReadAllButton = new Button();
            ToggleMonitorButton = new Button();
            ClearButton = new Button();
            ExportButton = new Button();
            GithubLink = new LinkLabel();
            DonateLink = new LinkLabel();
            PopupColour = new ColorDialog();
            OverrideTooltip = new ToolTip(components);
            CoreTabControl = new ColourableTabControl();
            CoreTabPage = new TabPage();
            CoreSplitter = new SplitContainer();
            CoreLayoutPanel = new TableLayoutPanel();
            PluginList = new NoHScrollList();
            NameColumn = new ColumnHeader();
            TypeColumn = new ColumnHeader();
            VersionColumn = new ColumnHeader();
            StatusColumn = new ColumnHeader();
            PluginListButtonsLayoutPanel = new FlowLayoutPanel();
            PluginFolderButton = new Button();
            PluginSettingsButton = new Button();
            CoreSettingsLayoutPanel = new FlowLayoutPanel();
            PopupSettingsPanel = new Panel();
            DurationSpinner = new NumericUpDown();
            ScaleSpinner = new NumericUpDown();
            PopupLabel = new Label();
            FontDropdown = new ComboBox();
            LabelColour = new Label();
            LabelFont = new Label();
            LabelScale = new Label();
            TestButton = new Button();
            CornerDropdown = new ComboBox();
            DisplayLabel = new Label();
            LabelDuration = new Label();
            ColourButton = new Button();
            DisplayDropdown = new ComboBox();
            CornerLabel = new Label();
            PopupCheckbox = new CheckBox();
            PopupDisabledPanel = new Panel();
            PopupDisabledLabel = new Label();
            VoiceSettingsPanel = new Panel();
            VoiceSpeedSlider = new TrackBar();
            VoiceVolumeSlider = new TrackBar();
            VoiceLabel = new Label();
            VoiceTestButton = new Button();
            VoiceSpeedLabel = new Label();
            AudioLabel = new Label();
            VoiceDropdown = new ComboBox();
            VoiceCheckbox = new CheckBox();
            VoiceVolumeLabel = new Label();
            VoiceDisabledPanel = new Panel();
            VoiceDisabledLabel = new Label();
            CoreSettingsPanel = new Panel();
            ExportFormatLabel = new Label();
            ExportFormatDropdown = new ComboBox();
            CoreSettingsLabel = new Label();
            StartReadallCheckbox = new CheckBox();
            StartMonitorCheckbox = new CheckBox();
            LabelJournal = new Label();
            ThemeLabel = new Label();
            LabelJournalPath = new Label();
            ThemeDropdown = new ComboBox();
            ButtonAddTheme = new Button();
            CoreTabControl.SuspendLayout();
            CoreTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)CoreSplitter).BeginInit();
            CoreSplitter.Panel1.SuspendLayout();
            CoreSplitter.Panel2.SuspendLayout();
            CoreSplitter.SuspendLayout();
            CoreLayoutPanel.SuspendLayout();
            PluginListButtonsLayoutPanel.SuspendLayout();
            CoreSettingsLayoutPanel.SuspendLayout();
            PopupSettingsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DurationSpinner).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ScaleSpinner).BeginInit();
            PopupDisabledPanel.SuspendLayout();
            VoiceSettingsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)VoiceSpeedSlider).BeginInit();
            ((System.ComponentModel.ISupportInitialize)VoiceVolumeSlider).BeginInit();
            VoiceDisabledPanel.SuspendLayout();
            CoreSettingsPanel.SuspendLayout();
            SuspendLayout();
            // 
            // ReadAllButton
            // 
            ReadAllButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            ReadAllButton.FlatAppearance.BorderSize = 0;
            ReadAllButton.FlatStyle = FlatStyle.Flat;
            ReadAllButton.Location = new Point(770, 869);
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
            ToggleMonitorButton.Location = new Point(667, 869);
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
            ClearButton.Location = new Point(586, 869);
            ClearButton.Name = "ClearButton";
            ClearButton.Size = new Size(75, 23);
            ClearButton.TabIndex = 4;
            ClearButton.Text = "Clear";
            ClearButton.UseVisualStyleBackColor = false;
            ClearButton.Click += ClearButton_Click;
            // 
            // ExportButton
            // 
            ExportButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            ExportButton.FlatAppearance.BorderSize = 0;
            ExportButton.FlatStyle = FlatStyle.Flat;
            ExportButton.Location = new Point(505, 869);
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
            GithubLink.Location = new Point(12, 865);
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
            DonateLink.Location = new Point(12, 880);
            DonateLink.Name = "DonateLink";
            DonateLink.Size = new Size(45, 15);
            DonateLink.TabIndex = 7;
            DonateLink.TabStop = true;
            DonateLink.Text = "Donate";
            DonateLink.LinkClicked += DonateLink_LinkClicked;
            // 
            // CoreTabControl
            // 
            CoreTabControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            CoreTabControl.Controls.Add(CoreTabPage);
            CoreTabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            CoreTabControl.Location = new Point(12, 12);
            CoreTabControl.Margin = new Padding(0);
            CoreTabControl.Multiline = true;
            CoreTabControl.Name = "CoreTabControl";
            CoreTabControl.SelectedIndex = 0;
            CoreTabControl.SelectedTabColor = Color.Empty;
            CoreTabControl.Size = new Size(833, 851);
            CoreTabControl.TabColor = Color.Empty;
            CoreTabControl.TabIndex = 8;
            CoreTabControl.DrawItem += CoreTabControl_DrawItem;
            CoreTabControl.SelectedIndexChanged += CoreTabControl_SelectedIndexChanged;
            // 
            // CoreTabPage
            // 
            CoreTabPage.BackColor = Color.Transparent;
            CoreTabPage.Controls.Add(CoreSplitter);
            CoreTabPage.Location = new Point(4, 24);
            CoreTabPage.Name = "CoreTabPage";
            CoreTabPage.Padding = new Padding(3);
            CoreTabPage.Size = new Size(825, 823);
            CoreTabPage.TabIndex = 0;
            CoreTabPage.Text = "Core";
            // 
            // CoreSplitter
            // 
            CoreSplitter.BackColor = SystemColors.ControlLight;
            CoreSplitter.Dock = DockStyle.Fill;
            CoreSplitter.FixedPanel = FixedPanel.Panel1;
            CoreSplitter.Location = new Point(3, 3);
            CoreSplitter.Name = "CoreSplitter";
            CoreSplitter.Orientation = Orientation.Horizontal;
            // 
            // CoreSplitter.Panel1
            // 
            CoreSplitter.Panel1.AutoScroll = true;
            CoreSplitter.Panel1.BackColor = SystemColors.Control;
            CoreSplitter.Panel1.Controls.Add(CoreLayoutPanel);
            // 
            // CoreSplitter.Panel2
            // 
            CoreSplitter.Panel2.BackColor = SystemColors.Control;
            CoreSplitter.Panel2.Controls.Add(CoreSettingsLayoutPanel);
            CoreSplitter.Size = new Size(819, 817);
            CoreSplitter.SplitterDistance = 179;
            CoreSplitter.TabIndex = 16;
            // 
            // CoreLayoutPanel
            // 
            CoreLayoutPanel.ColumnCount = 1;
            CoreLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            CoreLayoutPanel.Controls.Add(PluginList, 0, 0);
            CoreLayoutPanel.Controls.Add(PluginListButtonsLayoutPanel, 0, 1);
            CoreLayoutPanel.Dock = DockStyle.Fill;
            CoreLayoutPanel.Location = new Point(0, 0);
            CoreLayoutPanel.Name = "CoreLayoutPanel";
            CoreLayoutPanel.RowCount = 2;
            CoreLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            CoreLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            CoreLayoutPanel.Size = new Size(819, 179);
            CoreLayoutPanel.TabIndex = 35;
            // 
            // PluginList
            // 
            PluginList.BorderStyle = BorderStyle.None;
            PluginList.CheckBoxes = true;
            PluginList.Columns.AddRange(new ColumnHeader[] { NameColumn, TypeColumn, VersionColumn, StatusColumn });
            PluginList.Dock = DockStyle.Fill;
            PluginList.FullRowSelect = true;
            PluginList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            PluginList.ImeMode = ImeMode.NoControl;
            PluginList.Location = new Point(3, 3);
            PluginList.MultiSelect = false;
            PluginList.Name = "PluginList";
            PluginList.OwnerDraw = true;
            PluginList.Size = new Size(813, 133);
            PluginList.TabIndex = 8;
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
            // PluginListButtonsLayoutPanel
            // 
            PluginListButtonsLayoutPanel.Controls.Add(PluginFolderButton);
            PluginListButtonsLayoutPanel.Controls.Add(PluginSettingsButton);
            PluginListButtonsLayoutPanel.Dock = DockStyle.Fill;
            PluginListButtonsLayoutPanel.FlowDirection = FlowDirection.RightToLeft;
            PluginListButtonsLayoutPanel.Location = new Point(3, 142);
            PluginListButtonsLayoutPanel.Name = "PluginListButtonsLayoutPanel";
            PluginListButtonsLayoutPanel.Size = new Size(813, 34);
            PluginListButtonsLayoutPanel.TabIndex = 9;
            // 
            // PluginFolderButton
            // 
            PluginFolderButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            PluginFolderButton.FlatAppearance.BorderSize = 0;
            PluginFolderButton.FlatStyle = FlatStyle.Flat;
            PluginFolderButton.Location = new Point(680, 3);
            PluginFolderButton.Name = "PluginFolderButton";
            PluginFolderButton.Size = new Size(130, 23);
            PluginFolderButton.TabIndex = 10;
            PluginFolderButton.Text = "Open Plugin Folder";
            PluginFolderButton.UseVisualStyleBackColor = false;
            PluginFolderButton.Click += PluginFolderButton_Click;
            // 
            // PluginSettingsButton
            // 
            PluginSettingsButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            PluginSettingsButton.FlatAppearance.BorderSize = 0;
            PluginSettingsButton.FlatStyle = FlatStyle.Flat;
            PluginSettingsButton.Location = new Point(554, 3);
            PluginSettingsButton.Name = "PluginSettingsButton";
            PluginSettingsButton.Size = new Size(120, 23);
            PluginSettingsButton.TabIndex = 11;
            PluginSettingsButton.Text = "Plugin Settings";
            PluginSettingsButton.UseVisualStyleBackColor = false;
            PluginSettingsButton.Click += PluginSettingsButton_Click;
            // 
            // CoreSettingsLayoutPanel
            // 
            CoreSettingsLayoutPanel.AutoScroll = true;
            CoreSettingsLayoutPanel.Controls.Add(PopupSettingsPanel);
            CoreSettingsLayoutPanel.Controls.Add(VoiceSettingsPanel);
            CoreSettingsLayoutPanel.Controls.Add(CoreSettingsPanel);
            CoreSettingsLayoutPanel.Dock = DockStyle.Fill;
            CoreSettingsLayoutPanel.Location = new Point(0, 0);
            CoreSettingsLayoutPanel.Name = "CoreSettingsLayoutPanel";
            CoreSettingsLayoutPanel.Size = new Size(819, 634);
            CoreSettingsLayoutPanel.TabIndex = 34;
            // 
            // PopupSettingsPanel
            // 
            PopupSettingsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            PopupSettingsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            PopupSettingsPanel.BorderStyle = BorderStyle.FixedSingle;
            PopupSettingsPanel.Controls.Add(DurationSpinner);
            PopupSettingsPanel.Controls.Add(ScaleSpinner);
            PopupSettingsPanel.Controls.Add(PopupLabel);
            PopupSettingsPanel.Controls.Add(FontDropdown);
            PopupSettingsPanel.Controls.Add(LabelColour);
            PopupSettingsPanel.Controls.Add(LabelFont);
            PopupSettingsPanel.Controls.Add(LabelScale);
            PopupSettingsPanel.Controls.Add(TestButton);
            PopupSettingsPanel.Controls.Add(CornerDropdown);
            PopupSettingsPanel.Controls.Add(DisplayLabel);
            PopupSettingsPanel.Controls.Add(LabelDuration);
            PopupSettingsPanel.Controls.Add(ColourButton);
            PopupSettingsPanel.Controls.Add(DisplayDropdown);
            PopupSettingsPanel.Controls.Add(CornerLabel);
            PopupSettingsPanel.Controls.Add(PopupCheckbox);
            PopupSettingsPanel.Controls.Add(PopupDisabledPanel);
            PopupSettingsPanel.Location = new Point(3, 3);
            PopupSettingsPanel.Name = "PopupSettingsPanel";
            PopupSettingsPanel.Size = new Size(550, 230);
            PopupSettingsPanel.TabIndex = 29;
            // 
            // DurationSpinner
            // 
            DurationSpinner.Increment = new decimal(new int[] { 25, 0, 0, 0 });
            DurationSpinner.Location = new Point(118, 138);
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
            ScaleSpinner.Location = new Point(118, 109);
            ScaleSpinner.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            ScaleSpinner.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            ScaleSpinner.Name = "ScaleSpinner";
            ScaleSpinner.Size = new Size(120, 23);
            ScaleSpinner.TabIndex = 14;
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
            FontDropdown.TabIndex = 5;
            FontDropdown.SelectedIndexChanged += FontDropdown_SelectedIndexChanged;
            // 
            // LabelColour
            // 
            LabelColour.AutoSize = true;
            LabelColour.Location = new Point(65, 171);
            LabelColour.Name = "LabelColour";
            LabelColour.Size = new Size(46, 15);
            LabelColour.TabIndex = 13;
            LabelColour.Text = "Colour:";
            LabelColour.TextAlign = ContentAlignment.MiddleRight;
            // 
            // LabelFont
            // 
            LabelFont.AutoSize = true;
            LabelFont.Location = new Point(77, 83);
            LabelFont.Name = "LabelFont";
            LabelFont.Size = new Size(34, 15);
            LabelFont.TabIndex = 4;
            LabelFont.Text = "Font:";
            LabelFont.TextAlign = ContentAlignment.MiddleRight;
            // 
            // LabelScale
            // 
            LabelScale.AutoSize = true;
            LabelScale.Location = new Point(54, 111);
            LabelScale.Name = "LabelScale";
            LabelScale.Size = new Size(58, 15);
            LabelScale.TabIndex = 7;
            LabelScale.Text = "Scale (%):";
            LabelScale.TextAlign = ContentAlignment.MiddleRight;
            // 
            // TestButton
            // 
            TestButton.FlatAppearance.BorderSize = 0;
            TestButton.FlatStyle = FlatStyle.Flat;
            TestButton.Location = new Point(187, 167);
            TestButton.Name = "TestButton";
            TestButton.Size = new Size(51, 23);
            TestButton.TabIndex = 12;
            TestButton.Text = "Test";
            TestButton.UseVisualStyleBackColor = false;
            TestButton.Click += TestButton_Click;
            // 
            // CornerDropdown
            // 
            CornerDropdown.DropDownStyle = ComboBoxStyle.DropDownList;
            CornerDropdown.FormattingEnabled = true;
            CornerDropdown.Items.AddRange(new object[] { "Bottom-Right", "Bottom-Left", "Top-Right", "Top-Left" });
            CornerDropdown.Location = new Point(117, 51);
            CornerDropdown.Name = "CornerDropdown";
            CornerDropdown.Size = new Size(121, 23);
            CornerDropdown.TabIndex = 3;
            CornerDropdown.SelectedIndexChanged += CornerDropdown_SelectedIndexChanged;
            // 
            // DisplayLabel
            // 
            DisplayLabel.AutoSize = true;
            DisplayLabel.Location = new Point(63, 25);
            DisplayLabel.Name = "DisplayLabel";
            DisplayLabel.Size = new Size(48, 15);
            DisplayLabel.TabIndex = 0;
            DisplayLabel.Text = "Display:";
            DisplayLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // LabelDuration
            // 
            LabelDuration.AutoSize = true;
            LabelDuration.Location = new Point(29, 140);
            LabelDuration.Name = "LabelDuration";
            LabelDuration.Size = new Size(83, 15);
            LabelDuration.TabIndex = 9;
            LabelDuration.Text = "Duration (ms):";
            LabelDuration.TextAlign = ContentAlignment.MiddleRight;
            // 
            // ColourButton
            // 
            ColourButton.FlatStyle = FlatStyle.Flat;
            ColourButton.Location = new Point(118, 167);
            ColourButton.Name = "ColourButton";
            ColourButton.Size = new Size(51, 23);
            ColourButton.TabIndex = 11;
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
            DisplayDropdown.TabIndex = 2;
            DisplayDropdown.SelectedIndexChanged += DisplayDropdown_SelectedIndexChanged;
            // 
            // CornerLabel
            // 
            CornerLabel.AutoSize = true;
            CornerLabel.Location = new Point(65, 54);
            CornerLabel.Name = "CornerLabel";
            CornerLabel.Size = new Size(46, 15);
            CornerLabel.TabIndex = 1;
            CornerLabel.Text = "Corner:";
            CornerLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // PopupCheckbox
            // 
            PopupCheckbox.AutoSize = true;
            PopupCheckbox.Location = new Point(117, 196);
            PopupCheckbox.Name = "PopupCheckbox";
            PopupCheckbox.Size = new Size(68, 19);
            PopupCheckbox.TabIndex = 10;
            PopupCheckbox.Text = "Enabled";
            PopupCheckbox.UseVisualStyleBackColor = true;
            PopupCheckbox.CheckedChanged += PopupCheckbox_CheckedChanged;
            // 
            // PopupDisabledPanel
            // 
            PopupDisabledPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            PopupDisabledPanel.Controls.Add(PopupDisabledLabel);
            PopupDisabledPanel.Enabled = false;
            PopupDisabledPanel.Location = new Point(3, 17);
            PopupDisabledPanel.Name = "PopupDisabledPanel";
            PopupDisabledPanel.Size = new Size(542, 207);
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
            // VoiceSettingsPanel
            // 
            VoiceSettingsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            VoiceSettingsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            VoiceSettingsPanel.BorderStyle = BorderStyle.FixedSingle;
            VoiceSettingsPanel.Controls.Add(VoiceSpeedSlider);
            VoiceSettingsPanel.Controls.Add(VoiceVolumeSlider);
            VoiceSettingsPanel.Controls.Add(VoiceLabel);
            VoiceSettingsPanel.Controls.Add(VoiceTestButton);
            VoiceSettingsPanel.Controls.Add(VoiceSpeedLabel);
            VoiceSettingsPanel.Controls.Add(AudioLabel);
            VoiceSettingsPanel.Controls.Add(VoiceDropdown);
            VoiceSettingsPanel.Controls.Add(VoiceCheckbox);
            VoiceSettingsPanel.Controls.Add(VoiceVolumeLabel);
            VoiceSettingsPanel.Controls.Add(VoiceDisabledPanel);
            VoiceSettingsPanel.Location = new Point(3, 239);
            VoiceSettingsPanel.Name = "VoiceSettingsPanel";
            VoiceSettingsPanel.Size = new Size(550, 230);
            VoiceSettingsPanel.TabIndex = 30;
            // 
            // VoiceSpeedSlider
            // 
            VoiceSpeedSlider.Location = new Point(109, 70);
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
            VoiceVolumeSlider.Location = new Point(108, 20);
            VoiceVolumeSlider.Maximum = 100;
            VoiceVolumeSlider.Name = "VoiceVolumeSlider";
            VoiceVolumeSlider.Size = new Size(121, 45);
            VoiceVolumeSlider.TabIndex = 14;
            VoiceVolumeSlider.TickFrequency = 10;
            VoiceVolumeSlider.TickStyle = TickStyle.Both;
            VoiceVolumeSlider.Value = 100;
            VoiceVolumeSlider.Scroll += VoiceVolumeSlider_Scroll;
            // 
            // VoiceLabel
            // 
            VoiceLabel.AutoSize = true;
            VoiceLabel.Location = new Point(65, 124);
            VoiceLabel.Name = "VoiceLabel";
            VoiceLabel.Size = new Size(38, 15);
            VoiceLabel.TabIndex = 4;
            VoiceLabel.Text = "Voice:";
            VoiceLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // VoiceTestButton
            // 
            VoiceTestButton.FlatAppearance.BorderSize = 0;
            VoiceTestButton.FlatStyle = FlatStyle.Flat;
            VoiceTestButton.Location = new Point(178, 150);
            VoiceTestButton.Name = "VoiceTestButton";
            VoiceTestButton.Size = new Size(51, 23);
            VoiceTestButton.TabIndex = 13;
            VoiceTestButton.Text = "Test";
            VoiceTestButton.UseVisualStyleBackColor = false;
            VoiceTestButton.Click += VoiceTestButton_Click;
            // 
            // VoiceSpeedLabel
            // 
            VoiceSpeedLabel.AutoSize = true;
            VoiceSpeedLabel.Location = new Point(61, 82);
            VoiceSpeedLabel.Name = "VoiceSpeedLabel";
            VoiceSpeedLabel.Size = new Size(42, 15);
            VoiceSpeedLabel.TabIndex = 1;
            VoiceSpeedLabel.Text = "Speed:";
            VoiceSpeedLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // AudioLabel
            // 
            AudioLabel.AutoSize = true;
            AudioLabel.Location = new Point(0, 0);
            AudioLabel.Name = "AudioLabel";
            AudioLabel.Size = new Size(106, 15);
            AudioLabel.TabIndex = 31;
            AudioLabel.Text = "Voice Notifications";
            // 
            // VoiceDropdown
            // 
            VoiceDropdown.DropDownStyle = ComboBoxStyle.DropDownList;
            VoiceDropdown.FormattingEnabled = true;
            VoiceDropdown.Location = new Point(109, 121);
            VoiceDropdown.Name = "VoiceDropdown";
            VoiceDropdown.Size = new Size(121, 23);
            VoiceDropdown.TabIndex = 5;
            VoiceDropdown.SelectedIndexChanged += VoiceDropdown_SelectedIndexChanged;
            // 
            // VoiceCheckbox
            // 
            VoiceCheckbox.AutoSize = true;
            VoiceCheckbox.Location = new Point(108, 153);
            VoiceCheckbox.Name = "VoiceCheckbox";
            VoiceCheckbox.Size = new Size(68, 19);
            VoiceCheckbox.TabIndex = 11;
            VoiceCheckbox.Text = "Enabled";
            VoiceCheckbox.UseVisualStyleBackColor = true;
            VoiceCheckbox.CheckedChanged += VoiceCheckbox_CheckedChanged;
            // 
            // VoiceVolumeLabel
            // 
            VoiceVolumeLabel.AutoSize = true;
            VoiceVolumeLabel.Location = new Point(52, 31);
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
            VoiceDisabledPanel.Location = new Point(3, 18);
            VoiceDisabledPanel.Name = "VoiceDisabledPanel";
            VoiceDisabledPanel.Size = new Size(542, 203);
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
            // CoreSettingsPanel
            // 
            CoreSettingsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            CoreSettingsPanel.BorderStyle = BorderStyle.FixedSingle;
            CoreSettingsPanel.Controls.Add(ExportFormatLabel);
            CoreSettingsPanel.Controls.Add(ExportFormatDropdown);
            CoreSettingsPanel.Controls.Add(CoreSettingsLabel);
            CoreSettingsPanel.Controls.Add(StartReadallCheckbox);
            CoreSettingsPanel.Controls.Add(StartMonitorCheckbox);
            CoreSettingsPanel.Controls.Add(LabelJournal);
            CoreSettingsPanel.Controls.Add(ThemeLabel);
            CoreSettingsPanel.Controls.Add(LabelJournalPath);
            CoreSettingsPanel.Controls.Add(ThemeDropdown);
            CoreSettingsPanel.Controls.Add(ButtonAddTheme);
            CoreSettingsPanel.Location = new Point(3, 475);
            CoreSettingsPanel.Name = "CoreSettingsPanel";
            CoreSettingsPanel.Size = new Size(550, 230);
            CoreSettingsPanel.TabIndex = 33;
            CoreSettingsPanel.Tag = "";
            // 
            // ExportFormatLabel
            // 
            ExportFormatLabel.AutoSize = true;
            ExportFormatLabel.Location = new Point(30, 57);
            ExportFormatLabel.Name = "ExportFormatLabel";
            ExportFormatLabel.Size = new Size(85, 15);
            ExportFormatLabel.TabIndex = 35;
            ExportFormatLabel.Text = "Export Format:";
            // 
            // ExportFormatDropdown
            // 
            ExportFormatDropdown.DropDownStyle = ComboBoxStyle.DropDownList;
            ExportFormatDropdown.FormattingEnabled = true;
            ExportFormatDropdown.Items.AddRange(new object[] { "Tab-Separated Values (csv)", "Office Open XML (xlsx)" });
            ExportFormatDropdown.Location = new Point(121, 54);
            ExportFormatDropdown.Name = "ExportFormatDropdown";
            ExportFormatDropdown.Size = new Size(214, 23);
            ExportFormatDropdown.TabIndex = 34;
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
            StartReadallCheckbox.Location = new Point(121, 137);
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
            StartMonitorCheckbox.Location = new Point(121, 112);
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
            LabelJournal.Location = new Point(31, 31);
            LabelJournal.Name = "LabelJournal";
            LabelJournal.Size = new Size(84, 15);
            LabelJournal.TabIndex = 12;
            LabelJournal.Text = "Journal Folder:";
            // 
            // ThemeLabel
            // 
            ThemeLabel.AutoSize = true;
            ThemeLabel.Location = new Point(68, 86);
            ThemeLabel.Name = "ThemeLabel";
            ThemeLabel.Size = new Size(46, 15);
            ThemeLabel.TabIndex = 9;
            ThemeLabel.Text = "Theme:";
            ThemeLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LabelJournalPath
            // 
            LabelJournalPath.Font = new Font("Segoe UI", 8.25F);
            LabelJournalPath.Location = new Point(121, 32);
            LabelJournalPath.Name = "LabelJournalPath";
            LabelJournalPath.Size = new Size(424, 13);
            LabelJournalPath.TabIndex = 13;
            LabelJournalPath.Text = "X:\\Journal";
            LabelJournalPath.DoubleClick += LabelJournalPath_DoubleClick;
            // 
            // ThemeDropdown
            // 
            ThemeDropdown.DropDownStyle = ComboBoxStyle.DropDownList;
            ThemeDropdown.FormattingEnabled = true;
            ThemeDropdown.Location = new Point(121, 83);
            ThemeDropdown.Name = "ThemeDropdown";
            ThemeDropdown.Size = new Size(121, 23);
            ThemeDropdown.TabIndex = 10;
            ThemeDropdown.SelectedIndexChanged += ThemeDropdown_SelectedIndexChanged;
            // 
            // ButtonAddTheme
            // 
            ButtonAddTheme.FlatAppearance.BorderSize = 0;
            ButtonAddTheme.FlatStyle = FlatStyle.Flat;
            ButtonAddTheme.Location = new Point(247, 83);
            ButtonAddTheme.Name = "ButtonAddTheme";
            ButtonAddTheme.Size = new Size(88, 23);
            ButtonAddTheme.TabIndex = 11;
            ButtonAddTheme.Text = "Add Theme";
            ButtonAddTheme.UseVisualStyleBackColor = true;
            ButtonAddTheme.Click += ButtonAddTheme_Click;
            // 
            // CoreForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(857, 904);
            Controls.Add(CoreTabControl);
            Controls.Add(DonateLink);
            Controls.Add(GithubLink);
            Controls.Add(ExportButton);
            Controls.Add(ClearButton);
            Controls.Add(ToggleMonitorButton);
            Controls.Add(ReadAllButton);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(600, 300);
            Name = "CoreForm";
            Text = "Elite Observatory Core";
            FormClosing += CoreForm_FormClosing;
            Load += CoreForm_Load;
            Shown += CoreForm_Shown;
            CoreTabControl.ResumeLayout(false);
            CoreTabPage.ResumeLayout(false);
            CoreSplitter.Panel1.ResumeLayout(false);
            CoreSplitter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)CoreSplitter).EndInit();
            CoreSplitter.ResumeLayout(false);
            CoreLayoutPanel.ResumeLayout(false);
            PluginListButtonsLayoutPanel.ResumeLayout(false);
            CoreSettingsLayoutPanel.ResumeLayout(false);
            PopupSettingsPanel.ResumeLayout(false);
            PopupSettingsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)DurationSpinner).EndInit();
            ((System.ComponentModel.ISupportInitialize)ScaleSpinner).EndInit();
            PopupDisabledPanel.ResumeLayout(false);
            PopupDisabledPanel.PerformLayout();
            VoiceSettingsPanel.ResumeLayout(false);
            VoiceSettingsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)VoiceSpeedSlider).EndInit();
            ((System.ComponentModel.ISupportInitialize)VoiceVolumeSlider).EndInit();
            VoiceDisabledPanel.ResumeLayout(false);
            VoiceDisabledPanel.PerformLayout();
            CoreSettingsPanel.ResumeLayout(false);
            CoreSettingsPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button ReadAllButton;
        private Button ToggleMonitorButton;
        private Button ClearButton;
        private Button ExportButton;
        private LinkLabel GithubLink;
        private LinkLabel DonateLink;
        private ColorDialog PopupColour;
        private ToolTip OverrideTooltip;
        private ColourableTabControl CoreTabControl;
        private TabPage CoreTabPage;
        private SplitContainer CoreSplitter;
        private TableLayoutPanel CoreLayoutPanel;
        private NoHScrollList PluginList;
        private ColumnHeader NameColumn;
        private ColumnHeader TypeColumn;
        private ColumnHeader VersionColumn;
        private ColumnHeader StatusColumn;
        private FlowLayoutPanel PluginListButtonsLayoutPanel;
        private Button PluginFolderButton;
        private Button PluginSettingsButton;
        private FlowLayoutPanel CoreSettingsLayoutPanel;
        private Panel PopupSettingsPanel;
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
        private ComboBox CornerDropdown;
        private ComboBox DisplayDropdown;
        private Label CornerLabel;
        private Label DisplayLabel;
        private Panel PopupDisabledPanel;
        private Label PopupDisabledLabel;
        private Label PopupLabel;
        private Panel VoiceSettingsPanel;
        private TrackBar VoiceSpeedSlider;
        private TrackBar VoiceVolumeSlider;
        private Button VoiceTestButton;
        private CheckBox VoiceCheckbox;
        private ComboBox VoiceDropdown;
        private Label VoiceLabel;
        private Label VoiceSpeedLabel;
        private Label VoiceVolumeLabel;
        private Panel VoiceDisabledPanel;
        private Label VoiceDisabledLabel;
        private Label AudioLabel;
        private Panel CoreSettingsPanel;
        private Label CoreSettingsLabel;
        private CheckBox StartReadallCheckbox;
        private CheckBox StartMonitorCheckbox;
        private Label LabelJournal;
        private Label ThemeLabel;
        private Label LabelJournalPath;
        private ComboBox ThemeDropdown;
        private Button ButtonAddTheme;
        private Label ExportFormatLabel;
        private ComboBox ExportFormatDropdown;
    }
}