namespace Observatory.UI
{
    partial class SettingsForm
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
            PluginSettingsCloseButton = new Button();
            SettingsFlowPanel = new FlowLayoutPanel();
            SuspendLayout();
            // 
            // PluginSettingsCloseButton
            // 
            PluginSettingsCloseButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            PluginSettingsCloseButton.FlatAppearance.BorderSize = 0;
            PluginSettingsCloseButton.FlatStyle = FlatStyle.Flat;
            PluginSettingsCloseButton.Location = new Point(302, 101);
            PluginSettingsCloseButton.Name = "PluginSettingsCloseButton";
            PluginSettingsCloseButton.Size = new Size(75, 23);
            PluginSettingsCloseButton.TabIndex = 0;
            PluginSettingsCloseButton.Text = "Close";
            PluginSettingsCloseButton.UseVisualStyleBackColor = true;
            PluginSettingsCloseButton.Click += PluginSettingsCloseButton_Click;
            // 
            // SettingsFlowPanel
            // 
            SettingsFlowPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            SettingsFlowPanel.AutoScroll = true;
            SettingsFlowPanel.Location = new Point(12, 12);
            SettingsFlowPanel.Name = "SettingsFlowPanel";
            SettingsFlowPanel.Size = new Size(365, 83);
            SettingsFlowPanel.TabIndex = 1;
            // 
            // SettingsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(389, 136);
            Controls.Add(SettingsFlowPanel);
            Controls.Add(PluginSettingsCloseButton);
            Name = "SettingsForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "SettingsForm";
            ResumeLayout(false);
        }

        #endregion

        private Button PluginSettingsCloseButton;
        private FlowLayoutPanel SettingsFlowPanel;
    }
}