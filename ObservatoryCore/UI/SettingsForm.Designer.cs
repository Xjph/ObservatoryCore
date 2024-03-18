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
            SuspendLayout();
            // 
            // PluginSettingsCloseButton
            // 
            PluginSettingsCloseButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            PluginSettingsCloseButton.Location = new Point(339, 5);
            PluginSettingsCloseButton.Name = "PluginSettingsCloseButton";
            PluginSettingsCloseButton.Size = new Size(75, 23);
            PluginSettingsCloseButton.TabIndex = 0;
            PluginSettingsCloseButton.Text = "Close";
            PluginSettingsCloseButton.UseVisualStyleBackColor = true;
            PluginSettingsCloseButton.Click += PluginSettingsCloseButton_Click;
            // 
            // SettingsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(426, 40);
            Controls.Add(PluginSettingsCloseButton);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "SettingsForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "SettingsForm";
            ResumeLayout(false);
        }

        #endregion

        private Button PluginSettingsCloseButton;
    }
}