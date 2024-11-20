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
            ReadAllButton = new Button();
            ToggleMonitorButton = new Button();
            ClearButton = new Button();
            ExportButton = new Button();
            PopupColour = new ColorDialog();
            OverrideTooltip = new ToolTip(components);
            AboutCore = new Button();
            CoreTabControl = new ColourableTabControl();
            CoreTabPage = new TabPage();
            SettingsButton = new Button();
            CoreTabPanel = new Panel();
            UpdateLink = new LinkLabel();
            MonitorStatus = new Label();
            LastEvent = new Label();
            TotalEvents = new Label();
            CoreTabControl.SuspendLayout();
            CoreTabPage.SuspendLayout();
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
            ReadAllButton.TabIndex = 33;
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
            ToggleMonitorButton.TabIndex = 32;
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
            ClearButton.TabIndex = 31;
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
            ExportButton.TabIndex = 30;
            ExportButton.Text = "Export";
            ExportButton.UseVisualStyleBackColor = false;
            ExportButton.Click += ExportButton_Click;
            // 
            // AboutCore
            // 
            AboutCore.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            AboutCore.FlatAppearance.BorderSize = 0;
            AboutCore.FlatStyle = FlatStyle.Flat;
            AboutCore.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            AboutCore.Location = new Point(16, 864);
            AboutCore.Name = "AboutCore";
            AboutCore.Size = new Size(42, 28);
            AboutCore.TabIndex = 35;
            AboutCore.Text = "ℹ️";
            OverrideTooltip.SetToolTip(AboutCore, "About Elite Observatory Core");
            AboutCore.UseVisualStyleBackColor = true;
            AboutCore.Click += AboutCore_Click;
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
            CoreTabControl.TabIndex = 1;
            CoreTabControl.SelectedIndexChanged += CoreTabControl_SelectedIndexChanged;
            CoreTabControl.MouseClick += CoreTabControl_MouseClick;
            // 
            // CoreTabPage
            // 
            CoreTabPage.BackColor = SystemColors.Control;
            CoreTabPage.BorderStyle = BorderStyle.FixedSingle;
            CoreTabPage.Controls.Add(TotalEvents);
            CoreTabPage.Controls.Add(LastEvent);
            CoreTabPage.Controls.Add(MonitorStatus);
            CoreTabPage.Controls.Add(SettingsButton);
            CoreTabPage.Controls.Add(CoreTabPanel);
            CoreTabPage.Location = new Point(4, 24);
            CoreTabPage.Name = "CoreTabPage";
            CoreTabPage.Padding = new Padding(3);
            CoreTabPage.Size = new Size(825, 823);
            CoreTabPage.TabIndex = 0;
            CoreTabPage.Text = "Core";
            // 
            // SettingsButton
            // 
            SettingsButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            SettingsButton.FlatAppearance.BorderSize = 0;
            SettingsButton.FlatStyle = FlatStyle.Flat;
            SettingsButton.Location = new Point(717, 792);
            SettingsButton.Name = "SettingsButton";
            SettingsButton.Size = new Size(100, 23);
            SettingsButton.TabIndex = 36;
            SettingsButton.Text = "Core Settings";
            SettingsButton.UseVisualStyleBackColor = true;
            SettingsButton.Click += SettingsButton_Click;
            // 
            // CoreTabPanel
            // 
            CoreTabPanel.Dock = DockStyle.Top;
            CoreTabPanel.Location = new Point(3, 3);
            CoreTabPanel.Name = "CoreTabPanel";
            CoreTabPanel.Size = new Size(817, 764);
            CoreTabPanel.TabIndex = 0;
            // 
            // UpdateLink
            // 
            UpdateLink.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            UpdateLink.AutoSize = true;
            UpdateLink.Enabled = false;
            UpdateLink.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            UpdateLink.Location = new Point(60, 865);
            UpdateLink.Name = "UpdateLink";
            UpdateLink.Size = new Size(207, 32);
            UpdateLink.TabIndex = 34;
            UpdateLink.TabStop = true;
            UpdateLink.Text = "Update Available";
            UpdateLink.Visible = false;
            // 
            // MonitorStatus
            // 
            MonitorStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            MonitorStatus.AutoSize = true;
            MonitorStatus.Location = new Point(6, 770);
            MonitorStatus.Name = "MonitorStatus";
            MonitorStatus.Size = new Size(178, 15);
            MonitorStatus.TabIndex = 37;
            MonitorStatus.Text = "Current Monitor Status: Stopped";
            // 
            // LastEvent
            // 
            LastEvent.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            LastEvent.AutoSize = true;
            LastEvent.Location = new Point(6, 785);
            LastEvent.Name = "LastEvent";
            LastEvent.Size = new Size(192, 15);
            LastEvent.TabIndex = 38;
            LastEvent.Text = "Last Journal Event Processed: None";
            // 
            // TotalEvents
            // 
            TotalEvents.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            TotalEvents.AutoSize = true;
            TotalEvents.Location = new Point(6, 800);
            TotalEvents.Name = "TotalEvents";
            TotalEvents.Size = new Size(171, 15);
            TotalEvents.TabIndex = 39;
            TotalEvents.Text = "Total Journal Lines Processed: 0";
            // 
            // CoreForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(857, 904);
            Controls.Add(AboutCore);
            Controls.Add(UpdateLink);
            Controls.Add(CoreTabControl);
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
            ResizeBegin += CoreForm_ResizeBegin;
            ResizeEnd += CoreForm_ResizeEnd;
            CoreTabControl.ResumeLayout(false);
            CoreTabPage.ResumeLayout(false);
            CoreTabPage.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button ReadAllButton;
        private Button ToggleMonitorButton;
        private Button ClearButton;
        private Button ExportButton;
        private ColorDialog PopupColour;
        private ToolTip OverrideTooltip;
        private ColourableTabControl CoreTabControl;
        private TabPage CoreTabPage;
        private LinkLabel UpdateLink;
        private Button AboutCore;
        private Button SettingsButton;
        private Panel CoreTabPanel;
        private Label TotalEvents;
        private Label LastEvent;
        private Label MonitorStatus;
    }
}