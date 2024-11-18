namespace Observatory.UI
{
    partial class PopoutForm
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
            PopoutPanel = new Panel();
            ClearButton = new Button();
            ExportButton = new Button();
            SettingsButton = new Button();
            SuspendLayout();
            // 
            // PopoutPanel
            // 
            PopoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            PopoutPanel.Location = new Point(1, 0);
            PopoutPanel.Name = "PopoutPanel";
            PopoutPanel.Size = new Size(799, 409);
            PopoutPanel.TabIndex = 0;
            // 
            // ClearButton
            // 
            ClearButton.Location = new Point(713, 415);
            ClearButton.Name = "ClearButton";
            ClearButton.Size = new Size(75, 23);
            ClearButton.TabIndex = 1;
            ClearButton.Text = "Clear";
            ClearButton.UseVisualStyleBackColor = true;
            // 
            // ExportButton
            // 
            ExportButton.Location = new Point(632, 415);
            ExportButton.Name = "ExportButton";
            ExportButton.Size = new Size(75, 23);
            ExportButton.TabIndex = 2;
            ExportButton.Text = "Export";
            ExportButton.UseVisualStyleBackColor = true;
            // 
            // SettingsButton
            // 
            SettingsButton.Location = new Point(12, 415);
            SettingsButton.Name = "SettingsButton";
            SettingsButton.Size = new Size(75, 23);
            SettingsButton.TabIndex = 3;
            SettingsButton.Text = "Settings";
            SettingsButton.UseVisualStyleBackColor = true;
            // 
            // PopoutForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(SettingsButton);
            Controls.Add(ExportButton);
            Controls.Add(ClearButton);
            Controls.Add(PopoutPanel);
            Name = "PopoutForm";
            Text = "PopoutForm";
            FormClosing += PopoutForm_FormClosing;
            ResumeLayout(false);
        }

        #endregion

        private Panel PopoutPanel;
        private Button ClearButton;
        private Button ExportButton;
        private Button SettingsButton;
    }
}