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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CoreForm));
            this.CoreMenu = new System.Windows.Forms.MenuStrip();
            this.coreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.CorePanel = new System.Windows.Forms.Panel();
            this.VoiceSettingsPanel = new System.Windows.Forms.Panel();
            this.VoiceSpeedSlider = new System.Windows.Forms.TrackBar();
            this.VoiceVolumeSlider = new System.Windows.Forms.TrackBar();
            this.VoiceTestButton = new System.Windows.Forms.Button();
            this.VoiceCheckbox = new System.Windows.Forms.CheckBox();
            this.VoiceDropdown = new System.Windows.Forms.ComboBox();
            this.VoiceLabel = new System.Windows.Forms.Label();
            this.VoiceSpeedLabel = new System.Windows.Forms.Label();
            this.VoiceVolumeLabel = new System.Windows.Forms.Label();
            this.VoiceNotificationLabel = new System.Windows.Forms.Label();
            this.PopupSettingsPanel = new System.Windows.Forms.Panel();
            this.DurationSpinner = new System.Windows.Forms.NumericUpDown();
            this.ScaleSpinner = new System.Windows.Forms.NumericUpDown();
            this.LabelColour = new System.Windows.Forms.Label();
            this.TestButton = new System.Windows.Forms.Button();
            this.ColourButton = new System.Windows.Forms.Button();
            this.PopupCheckbox = new System.Windows.Forms.CheckBox();
            this.LabelDuration = new System.Windows.Forms.Label();
            this.LabelScale = new System.Windows.Forms.Label();
            this.FontDropdown = new System.Windows.Forms.ComboBox();
            this.LabelFont = new System.Windows.Forms.Label();
            this.CornerDropdown = new System.Windows.Forms.ComboBox();
            this.DisplayDropdown = new System.Windows.Forms.ComboBox();
            this.CornerLabel = new System.Windows.Forms.Label();
            this.DisplayLabel = new System.Windows.Forms.Label();
            this.PopupNotificationLabel = new System.Windows.Forms.Label();
            this.PluginFolderButton = new System.Windows.Forms.Button();
            this.PluginList = new System.Windows.Forms.ListView();
            this.NameColumn = new System.Windows.Forms.ColumnHeader();
            this.TypeColumn = new System.Windows.Forms.ColumnHeader();
            this.VersionColumn = new System.Windows.Forms.ColumnHeader();
            this.StatusColumn = new System.Windows.Forms.ColumnHeader();
            this.ReadAllButton = new System.Windows.Forms.Button();
            this.ToggleMonitorButton = new System.Windows.Forms.Button();
            this.ClearButton = new System.Windows.Forms.Button();
            this.ExportButton = new System.Windows.Forms.Button();
            this.GithubLink = new System.Windows.Forms.LinkLabel();
            this.DonateLink = new System.Windows.Forms.LinkLabel();
            this.PopupColour = new System.Windows.Forms.ColorDialog();
            this.CoreMenu.SuspendLayout();
            this.CorePanel.SuspendLayout();
            this.VoiceSettingsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.VoiceSpeedSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.VoiceVolumeSlider)).BeginInit();
            this.PopupSettingsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DurationSpinner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ScaleSpinner)).BeginInit();
            this.SuspendLayout();
            // 
            // CoreMenu
            // 
            this.CoreMenu.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.CoreMenu.AutoSize = false;
            this.CoreMenu.BackColor = System.Drawing.Color.Black;
            this.CoreMenu.Dock = System.Windows.Forms.DockStyle.None;
            this.CoreMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.coreToolStripMenuItem,
            this.toolStripMenuItem1});
            this.CoreMenu.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.CoreMenu.Location = new System.Drawing.Point(0, 0);
            this.CoreMenu.Name = "CoreMenu";
            this.CoreMenu.Size = new System.Drawing.Size(120, 691);
            this.CoreMenu.TabIndex = 0;
            // 
            // coreToolStripMenuItem
            // 
            this.coreToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.coreToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.coreToolStripMenuItem.Name = "coreToolStripMenuItem";
            this.coreToolStripMenuItem.Size = new System.Drawing.Size(113, 36);
            this.coreToolStripMenuItem.Text = "Core";
            this.coreToolStripMenuItem.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripMenuItem1.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.toolStripMenuItem1.ForeColor = System.Drawing.Color.Gainsboro;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(113, 36);
            this.toolStripMenuItem1.Text = "<";
            this.toolStripMenuItem1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CorePanel
            // 
            this.CorePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CorePanel.AutoScroll = true;
            this.CorePanel.Controls.Add(this.VoiceSettingsPanel);
            this.CorePanel.Controls.Add(this.VoiceNotificationLabel);
            this.CorePanel.Controls.Add(this.PopupSettingsPanel);
            this.CorePanel.Controls.Add(this.PopupNotificationLabel);
            this.CorePanel.Controls.Add(this.PluginFolderButton);
            this.CorePanel.Controls.Add(this.PluginList);
            this.CorePanel.Location = new System.Drawing.Point(123, 12);
            this.CorePanel.Name = "CorePanel";
            this.CorePanel.Size = new System.Drawing.Size(665, 679);
            this.CorePanel.TabIndex = 1;
            // 
            // VoiceSettingsPanel
            // 
            this.VoiceSettingsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.VoiceSettingsPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.VoiceSettingsPanel.Controls.Add(this.VoiceSpeedSlider);
            this.VoiceSettingsPanel.Controls.Add(this.VoiceVolumeSlider);
            this.VoiceSettingsPanel.Controls.Add(this.VoiceTestButton);
            this.VoiceSettingsPanel.Controls.Add(this.VoiceCheckbox);
            this.VoiceSettingsPanel.Controls.Add(this.VoiceDropdown);
            this.VoiceSettingsPanel.Controls.Add(this.VoiceLabel);
            this.VoiceSettingsPanel.Controls.Add(this.VoiceSpeedLabel);
            this.VoiceSettingsPanel.Controls.Add(this.VoiceVolumeLabel);
            this.VoiceSettingsPanel.Location = new System.Drawing.Point(3, 426);
            this.VoiceSettingsPanel.Name = "VoiceSettingsPanel";
            this.VoiceSettingsPanel.Size = new System.Drawing.Size(659, 177);
            this.VoiceSettingsPanel.TabIndex = 5;
            this.VoiceSettingsPanel.Visible = false;
            // 
            // VoiceSpeedSlider
            // 
            this.VoiceSpeedSlider.Location = new System.Drawing.Point(121, 51);
            this.VoiceSpeedSlider.Maximum = 100;
            this.VoiceSpeedSlider.Name = "VoiceSpeedSlider";
            this.VoiceSpeedSlider.Size = new System.Drawing.Size(120, 45);
            this.VoiceSpeedSlider.TabIndex = 15;
            this.VoiceSpeedSlider.TickFrequency = 10;
            this.VoiceSpeedSlider.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.VoiceSpeedSlider.Value = 50;
            this.VoiceSpeedSlider.Scroll += new System.EventHandler(this.VoiceSpeedSlider_Scroll);
            // 
            // VoiceVolumeSlider
            // 
            this.VoiceVolumeSlider.LargeChange = 10;
            this.VoiceVolumeSlider.Location = new System.Drawing.Point(120, 0);
            this.VoiceVolumeSlider.Maximum = 100;
            this.VoiceVolumeSlider.Name = "VoiceVolumeSlider";
            this.VoiceVolumeSlider.Size = new System.Drawing.Size(121, 45);
            this.VoiceVolumeSlider.TabIndex = 14;
            this.VoiceVolumeSlider.TickFrequency = 10;
            this.VoiceVolumeSlider.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.VoiceVolumeSlider.Value = 100;
            this.VoiceVolumeSlider.Scroll += new System.EventHandler(this.VoiceVolumeSlider_Scroll);
            // 
            // VoiceTestButton
            // 
            this.VoiceTestButton.BackColor = System.Drawing.Color.DimGray;
            this.VoiceTestButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VoiceTestButton.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.VoiceTestButton.Location = new System.Drawing.Point(190, 131);
            this.VoiceTestButton.Name = "VoiceTestButton";
            this.VoiceTestButton.Size = new System.Drawing.Size(51, 23);
            this.VoiceTestButton.TabIndex = 13;
            this.VoiceTestButton.Text = "Test";
            this.VoiceTestButton.UseVisualStyleBackColor = false;
            // 
            // VoiceCheckbox
            // 
            this.VoiceCheckbox.AutoSize = true;
            this.VoiceCheckbox.ForeColor = System.Drawing.Color.Gainsboro;
            this.VoiceCheckbox.Location = new System.Drawing.Point(120, 134);
            this.VoiceCheckbox.Name = "VoiceCheckbox";
            this.VoiceCheckbox.Size = new System.Drawing.Size(68, 19);
            this.VoiceCheckbox.TabIndex = 11;
            this.VoiceCheckbox.Text = "Enabled";
            this.VoiceCheckbox.UseVisualStyleBackColor = true;
            this.VoiceCheckbox.CheckedChanged += new System.EventHandler(this.VoiceCheckbox_CheckedChanged);
            // 
            // VoiceDropdown
            // 
            this.VoiceDropdown.FormattingEnabled = true;
            this.VoiceDropdown.Location = new System.Drawing.Point(121, 102);
            this.VoiceDropdown.Name = "VoiceDropdown";
            this.VoiceDropdown.Size = new System.Drawing.Size(121, 23);
            this.VoiceDropdown.TabIndex = 5;
            this.VoiceDropdown.SelectedIndexChanged += new System.EventHandler(this.VoiceDropdown_SelectedIndexChanged);
            // 
            // VoiceLabel
            // 
            this.VoiceLabel.AutoSize = true;
            this.VoiceLabel.ForeColor = System.Drawing.Color.Gainsboro;
            this.VoiceLabel.Location = new System.Drawing.Point(77, 105);
            this.VoiceLabel.Name = "VoiceLabel";
            this.VoiceLabel.Size = new System.Drawing.Size(38, 15);
            this.VoiceLabel.TabIndex = 4;
            this.VoiceLabel.Text = "Voice:";
            this.VoiceLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // VoiceSpeedLabel
            // 
            this.VoiceSpeedLabel.AutoSize = true;
            this.VoiceSpeedLabel.ForeColor = System.Drawing.Color.Gainsboro;
            this.VoiceSpeedLabel.Location = new System.Drawing.Point(73, 63);
            this.VoiceSpeedLabel.Name = "VoiceSpeedLabel";
            this.VoiceSpeedLabel.Size = new System.Drawing.Size(42, 15);
            this.VoiceSpeedLabel.TabIndex = 1;
            this.VoiceSpeedLabel.Text = "Speed:";
            this.VoiceSpeedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // VoiceVolumeLabel
            // 
            this.VoiceVolumeLabel.AutoSize = true;
            this.VoiceVolumeLabel.ForeColor = System.Drawing.Color.Gainsboro;
            this.VoiceVolumeLabel.Location = new System.Drawing.Point(64, 12);
            this.VoiceVolumeLabel.Name = "VoiceVolumeLabel";
            this.VoiceVolumeLabel.Size = new System.Drawing.Size(50, 15);
            this.VoiceVolumeLabel.TabIndex = 0;
            this.VoiceVolumeLabel.Text = "Volume:";
            this.VoiceVolumeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // VoiceNotificationLabel
            // 
            this.VoiceNotificationLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.VoiceNotificationLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.VoiceNotificationLabel.ForeColor = System.Drawing.Color.LightGray;
            this.VoiceNotificationLabel.Location = new System.Drawing.Point(3, 403);
            this.VoiceNotificationLabel.Name = "VoiceNotificationLabel";
            this.VoiceNotificationLabel.Size = new System.Drawing.Size(659, 23);
            this.VoiceNotificationLabel.TabIndex = 4;
            this.VoiceNotificationLabel.Text = "❯ Voice Notifications";
            this.VoiceNotificationLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.VoiceNotificationLabel.Click += new System.EventHandler(this.VoiceNotificationLabel_Click);
            // 
            // PopupSettingsPanel
            // 
            this.PopupSettingsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PopupSettingsPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.PopupSettingsPanel.Controls.Add(this.DurationSpinner);
            this.PopupSettingsPanel.Controls.Add(this.ScaleSpinner);
            this.PopupSettingsPanel.Controls.Add(this.LabelColour);
            this.PopupSettingsPanel.Controls.Add(this.TestButton);
            this.PopupSettingsPanel.Controls.Add(this.ColourButton);
            this.PopupSettingsPanel.Controls.Add(this.PopupCheckbox);
            this.PopupSettingsPanel.Controls.Add(this.LabelDuration);
            this.PopupSettingsPanel.Controls.Add(this.LabelScale);
            this.PopupSettingsPanel.Controls.Add(this.FontDropdown);
            this.PopupSettingsPanel.Controls.Add(this.LabelFont);
            this.PopupSettingsPanel.Controls.Add(this.CornerDropdown);
            this.PopupSettingsPanel.Controls.Add(this.DisplayDropdown);
            this.PopupSettingsPanel.Controls.Add(this.CornerLabel);
            this.PopupSettingsPanel.Controls.Add(this.DisplayLabel);
            this.PopupSettingsPanel.Location = new System.Drawing.Point(3, 195);
            this.PopupSettingsPanel.Name = "PopupSettingsPanel";
            this.PopupSettingsPanel.Size = new System.Drawing.Size(659, 208);
            this.PopupSettingsPanel.TabIndex = 3;
            this.PopupSettingsPanel.Visible = false;
            // 
            // DurationSpinner
            // 
            this.DurationSpinner.BackColor = System.Drawing.Color.DimGray;
            this.DurationSpinner.ForeColor = System.Drawing.Color.Gainsboro;
            this.DurationSpinner.Increment = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.DurationSpinner.Location = new System.Drawing.Point(121, 119);
            this.DurationSpinner.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.DurationSpinner.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.DurationSpinner.Name = "DurationSpinner";
            this.DurationSpinner.Size = new System.Drawing.Size(120, 23);
            this.DurationSpinner.TabIndex = 15;
            this.DurationSpinner.Value = new decimal(new int[] {
            8000,
            0,
            0,
            0});
            this.DurationSpinner.ValueChanged += new System.EventHandler(this.DurationSpinner_ValueChanged);
            // 
            // ScaleSpinner
            // 
            this.ScaleSpinner.BackColor = System.Drawing.Color.DimGray;
            this.ScaleSpinner.ForeColor = System.Drawing.Color.Gainsboro;
            this.ScaleSpinner.Location = new System.Drawing.Point(121, 90);
            this.ScaleSpinner.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.ScaleSpinner.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ScaleSpinner.Name = "ScaleSpinner";
            this.ScaleSpinner.Size = new System.Drawing.Size(120, 23);
            this.ScaleSpinner.TabIndex = 14;
            this.ScaleSpinner.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.ScaleSpinner.ValueChanged += new System.EventHandler(this.ScaleSpinner_ValueChanged);
            // 
            // LabelColour
            // 
            this.LabelColour.AutoSize = true;
            this.LabelColour.ForeColor = System.Drawing.Color.Gainsboro;
            this.LabelColour.Location = new System.Drawing.Point(68, 152);
            this.LabelColour.Name = "LabelColour";
            this.LabelColour.Size = new System.Drawing.Size(46, 15);
            this.LabelColour.TabIndex = 13;
            this.LabelColour.Text = "Colour:";
            this.LabelColour.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TestButton
            // 
            this.TestButton.BackColor = System.Drawing.Color.DimGray;
            this.TestButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.TestButton.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.TestButton.Location = new System.Drawing.Point(190, 148);
            this.TestButton.Name = "TestButton";
            this.TestButton.Size = new System.Drawing.Size(51, 23);
            this.TestButton.TabIndex = 12;
            this.TestButton.Text = "Test";
            this.TestButton.UseVisualStyleBackColor = false;
            this.TestButton.Click += new System.EventHandler(this.TestButton_Click);
            // 
            // ColourButton
            // 
            this.ColourButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ColourButton.Location = new System.Drawing.Point(121, 148);
            this.ColourButton.Name = "ColourButton";
            this.ColourButton.Size = new System.Drawing.Size(51, 23);
            this.ColourButton.TabIndex = 11;
            this.ColourButton.UseVisualStyleBackColor = true;
            this.ColourButton.Click += new System.EventHandler(this.ColourButton_Click);
            // 
            // PopupCheckbox
            // 
            this.PopupCheckbox.AutoSize = true;
            this.PopupCheckbox.ForeColor = System.Drawing.Color.Gainsboro;
            this.PopupCheckbox.Location = new System.Drawing.Point(120, 177);
            this.PopupCheckbox.Name = "PopupCheckbox";
            this.PopupCheckbox.Size = new System.Drawing.Size(68, 19);
            this.PopupCheckbox.TabIndex = 10;
            this.PopupCheckbox.Text = "Enabled";
            this.PopupCheckbox.UseVisualStyleBackColor = true;
            this.PopupCheckbox.CheckedChanged += new System.EventHandler(this.PopupCheckbox_CheckedChanged);
            // 
            // LabelDuration
            // 
            this.LabelDuration.AutoSize = true;
            this.LabelDuration.ForeColor = System.Drawing.Color.Gainsboro;
            this.LabelDuration.Location = new System.Drawing.Point(32, 121);
            this.LabelDuration.Name = "LabelDuration";
            this.LabelDuration.Size = new System.Drawing.Size(83, 15);
            this.LabelDuration.TabIndex = 9;
            this.LabelDuration.Text = "Duration (ms):";
            this.LabelDuration.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LabelScale
            // 
            this.LabelScale.AutoSize = true;
            this.LabelScale.ForeColor = System.Drawing.Color.Gainsboro;
            this.LabelScale.Location = new System.Drawing.Point(57, 92);
            this.LabelScale.Name = "LabelScale";
            this.LabelScale.Size = new System.Drawing.Size(58, 15);
            this.LabelScale.TabIndex = 7;
            this.LabelScale.Text = "Scale (%):";
            this.LabelScale.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FontDropdown
            // 
            this.FontDropdown.FormattingEnabled = true;
            this.FontDropdown.Location = new System.Drawing.Point(120, 61);
            this.FontDropdown.Name = "FontDropdown";
            this.FontDropdown.Size = new System.Drawing.Size(121, 23);
            this.FontDropdown.TabIndex = 5;
            this.FontDropdown.SelectedIndexChanged += new System.EventHandler(this.FontDropdown_SelectedIndexChanged);
            // 
            // LabelFont
            // 
            this.LabelFont.AutoSize = true;
            this.LabelFont.ForeColor = System.Drawing.Color.Gainsboro;
            this.LabelFont.Location = new System.Drawing.Point(80, 64);
            this.LabelFont.Name = "LabelFont";
            this.LabelFont.Size = new System.Drawing.Size(34, 15);
            this.LabelFont.TabIndex = 4;
            this.LabelFont.Text = "Font:";
            this.LabelFont.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CornerDropdown
            // 
            this.CornerDropdown.FormattingEnabled = true;
            this.CornerDropdown.Items.AddRange(new object[] {
            "Bottom-Right",
            "Bottom-Left",
            "Top-Right",
            "Top-Left"});
            this.CornerDropdown.Location = new System.Drawing.Point(120, 32);
            this.CornerDropdown.Name = "CornerDropdown";
            this.CornerDropdown.Size = new System.Drawing.Size(121, 23);
            this.CornerDropdown.TabIndex = 3;
            this.CornerDropdown.SelectedIndexChanged += new System.EventHandler(this.CornerDropdown_SelectedIndexChanged);
            // 
            // DisplayDropdown
            // 
            this.DisplayDropdown.FormattingEnabled = true;
            this.DisplayDropdown.Location = new System.Drawing.Point(120, 3);
            this.DisplayDropdown.Name = "DisplayDropdown";
            this.DisplayDropdown.Size = new System.Drawing.Size(121, 23);
            this.DisplayDropdown.TabIndex = 2;
            this.DisplayDropdown.SelectedIndexChanged += new System.EventHandler(this.DisplayDropdown_SelectedIndexChanged);
            // 
            // CornerLabel
            // 
            this.CornerLabel.AutoSize = true;
            this.CornerLabel.ForeColor = System.Drawing.Color.Gainsboro;
            this.CornerLabel.Location = new System.Drawing.Point(68, 35);
            this.CornerLabel.Name = "CornerLabel";
            this.CornerLabel.Size = new System.Drawing.Size(46, 15);
            this.CornerLabel.TabIndex = 1;
            this.CornerLabel.Text = "Corner:";
            this.CornerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // DisplayLabel
            // 
            this.DisplayLabel.AutoSize = true;
            this.DisplayLabel.ForeColor = System.Drawing.Color.Gainsboro;
            this.DisplayLabel.Location = new System.Drawing.Point(66, 6);
            this.DisplayLabel.Name = "DisplayLabel";
            this.DisplayLabel.Size = new System.Drawing.Size(48, 15);
            this.DisplayLabel.TabIndex = 0;
            this.DisplayLabel.Text = "Display:";
            this.DisplayLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // PopupNotificationLabel
            // 
            this.PopupNotificationLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PopupNotificationLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PopupNotificationLabel.ForeColor = System.Drawing.Color.LightGray;
            this.PopupNotificationLabel.Location = new System.Drawing.Point(3, 172);
            this.PopupNotificationLabel.Name = "PopupNotificationLabel";
            this.PopupNotificationLabel.Size = new System.Drawing.Size(659, 23);
            this.PopupNotificationLabel.TabIndex = 2;
            this.PopupNotificationLabel.Text = "❯ Popup Notifications";
            this.PopupNotificationLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.PopupNotificationLabel.Click += new System.EventHandler(this.PopupNotificationLabel_Click);
            // 
            // PluginFolderButton
            // 
            this.PluginFolderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PluginFolderButton.BackColor = System.Drawing.Color.DimGray;
            this.PluginFolderButton.FlatAppearance.BorderSize = 0;
            this.PluginFolderButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.PluginFolderButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.PluginFolderButton.Location = new System.Drawing.Point(532, 140);
            this.PluginFolderButton.Name = "PluginFolderButton";
            this.PluginFolderButton.Size = new System.Drawing.Size(130, 23);
            this.PluginFolderButton.TabIndex = 1;
            this.PluginFolderButton.Text = "Open Plugin Folder";
            this.PluginFolderButton.UseVisualStyleBackColor = false;
            // 
            // PluginList
            // 
            this.PluginList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PluginList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.PluginList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.PluginList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.NameColumn,
            this.TypeColumn,
            this.VersionColumn,
            this.StatusColumn});
            this.PluginList.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.PluginList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.PluginList.Location = new System.Drawing.Point(3, 3);
            this.PluginList.MultiSelect = false;
            this.PluginList.Name = "PluginList";
            this.PluginList.OwnerDraw = true;
            this.PluginList.Scrollable = false;
            this.PluginList.Size = new System.Drawing.Size(659, 137);
            this.PluginList.TabIndex = 0;
            this.PluginList.UseCompatibleStateImageBehavior = false;
            this.PluginList.View = System.Windows.Forms.View.Details;
            this.PluginList.Resize += new System.EventHandler(this.PluginList_Resize);
            // 
            // NameColumn
            // 
            this.NameColumn.Text = "Plugin";
            this.NameColumn.Width = 180;
            // 
            // TypeColumn
            // 
            this.TypeColumn.Text = "Type";
            this.TypeColumn.Width = 120;
            // 
            // VersionColumn
            // 
            this.VersionColumn.Text = "Version";
            this.VersionColumn.Width = 120;
            // 
            // StatusColumn
            // 
            this.StatusColumn.Text = "Status";
            // 
            // ReadAllButton
            // 
            this.ReadAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ReadAllButton.BackColor = System.Drawing.Color.DimGray;
            this.ReadAllButton.FlatAppearance.BorderSize = 0;
            this.ReadAllButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ReadAllButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ReadAllButton.Location = new System.Drawing.Point(713, 698);
            this.ReadAllButton.Name = "ReadAllButton";
            this.ReadAllButton.Size = new System.Drawing.Size(75, 23);
            this.ReadAllButton.TabIndex = 2;
            this.ReadAllButton.Text = "Read All";
            this.ReadAllButton.UseVisualStyleBackColor = false;
            this.ReadAllButton.Click += new System.EventHandler(this.ReadAllButton_Click);
            // 
            // ToggleMonitorButton
            // 
            this.ToggleMonitorButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ToggleMonitorButton.BackColor = System.Drawing.Color.DimGray;
            this.ToggleMonitorButton.FlatAppearance.BorderSize = 0;
            this.ToggleMonitorButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ToggleMonitorButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ToggleMonitorButton.Location = new System.Drawing.Point(610, 698);
            this.ToggleMonitorButton.Name = "ToggleMonitorButton";
            this.ToggleMonitorButton.Size = new System.Drawing.Size(97, 23);
            this.ToggleMonitorButton.TabIndex = 3;
            this.ToggleMonitorButton.Text = "Start Monitor";
            this.ToggleMonitorButton.UseVisualStyleBackColor = false;
            this.ToggleMonitorButton.Click += new System.EventHandler(this.ToggleMonitorButton_Click);
            // 
            // ClearButton
            // 
            this.ClearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ClearButton.BackColor = System.Drawing.Color.DimGray;
            this.ClearButton.FlatAppearance.BorderSize = 0;
            this.ClearButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ClearButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClearButton.Location = new System.Drawing.Point(529, 698);
            this.ClearButton.Name = "ClearButton";
            this.ClearButton.Size = new System.Drawing.Size(75, 23);
            this.ClearButton.TabIndex = 4;
            this.ClearButton.Text = "Clear";
            this.ClearButton.UseVisualStyleBackColor = false;
            // 
            // ExportButton
            // 
            this.ExportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ExportButton.BackColor = System.Drawing.Color.DimGray;
            this.ExportButton.FlatAppearance.BorderSize = 0;
            this.ExportButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ExportButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ExportButton.Location = new System.Drawing.Point(448, 698);
            this.ExportButton.Name = "ExportButton";
            this.ExportButton.Size = new System.Drawing.Size(75, 23);
            this.ExportButton.TabIndex = 5;
            this.ExportButton.Text = "Export";
            this.ExportButton.UseVisualStyleBackColor = false;
            // 
            // GithubLink
            // 
            this.GithubLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.GithubLink.AutoSize = true;
            this.GithubLink.LinkColor = System.Drawing.Color.White;
            this.GithubLink.Location = new System.Drawing.Point(12, 694);
            this.GithubLink.Name = "GithubLink";
            this.GithubLink.Size = new System.Drawing.Size(42, 15);
            this.GithubLink.TabIndex = 6;
            this.GithubLink.TabStop = true;
            this.GithubLink.Text = "github";
            // 
            // DonateLink
            // 
            this.DonateLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.DonateLink.AutoSize = true;
            this.DonateLink.LinkColor = System.Drawing.Color.White;
            this.DonateLink.Location = new System.Drawing.Point(12, 709);
            this.DonateLink.Name = "DonateLink";
            this.DonateLink.Size = new System.Drawing.Size(45, 15);
            this.DonateLink.TabIndex = 7;
            this.DonateLink.TabStop = true;
            this.DonateLink.Text = "Donate";
            // 
            // CoreForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(800, 733);
            this.Controls.Add(this.DonateLink);
            this.Controls.Add(this.GithubLink);
            this.Controls.Add(this.ExportButton);
            this.Controls.Add(this.ClearButton);
            this.Controls.Add(this.ToggleMonitorButton);
            this.Controls.Add(this.ReadAllButton);
            this.Controls.Add(this.CorePanel);
            this.Controls.Add(this.CoreMenu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.CoreMenu;
            this.Name = "CoreForm";
            this.Text = "Elite Observatory Core";
            this.CoreMenu.ResumeLayout(false);
            this.CoreMenu.PerformLayout();
            this.CorePanel.ResumeLayout(false);
            this.VoiceSettingsPanel.ResumeLayout(false);
            this.VoiceSettingsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.VoiceSpeedSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.VoiceVolumeSlider)).EndInit();
            this.PopupSettingsPanel.ResumeLayout(false);
            this.PopupSettingsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DurationSpinner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ScaleSpinner)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private ListView PluginList;
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
        private Label PopupNotificationLabel;
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
        private Label VoiceNotificationLabel;
    }
}