namespace Observatory.UI
{
    partial class NotificationForm
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
            Title = new Label();
            Body = new Label();
            SuspendLayout();
            // 
            // Title
            // 
            Title.Font = new Font("Segoe UI", 24F, FontStyle.Regular, GraphicsUnit.Point);
            Title.ForeColor = Color.OrangeRed;
            Title.Location = new Point(5, 5);
            Title.MaximumSize = new Size(345, 45);
            Title.Name = "Title";
            Title.Size = new Size(338, 45);
            Title.TabIndex = 0;
            Title.Text = "Title";
            Title.UseCompatibleTextRendering = true;
            // 
            // Body
            // 
            Body.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            Body.AutoSize = true;
            Body.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            Body.ForeColor = Color.OrangeRed;
            Body.Location = new Point(12, 45);
            Body.MaximumSize = new Size(320, 85);
            Body.Name = "Body";
            Body.Size = new Size(51, 31);
            Body.TabIndex = 1;
            Body.Text = "Body";
            Body.UseCompatibleTextRendering = true;
            // 
            // NotificationForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(64, 64, 64);
            ClientSize = new Size(355, 145);
            ControlBox = false;
            Controls.Add(Body);
            Controls.Add(Title);
            Enabled = false;
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "NotificationForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "NotificationForm";
            TransparencyKey = Color.FromArgb(64, 64, 64);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label Title;
        private Label Body;
    }
}