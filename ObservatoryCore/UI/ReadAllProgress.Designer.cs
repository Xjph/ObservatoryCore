namespace Observatory.UI
{
    partial class ReadAllForm
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
            ReadAllProgress = new ProgressBar();
            JournalLabel = new Label();
            CancelButton = new Button();
            SuspendLayout();
            // 
            // ReadAllProgress
            // 
            ReadAllProgress.Location = new Point(12, 27);
            ReadAllProgress.Name = "ReadAllProgress";
            ReadAllProgress.Size = new Size(371, 23);
            ReadAllProgress.Step = 1;
            ReadAllProgress.TabIndex = 0;
            // 
            // JournalLabel
            // 
            JournalLabel.AutoSize = true;
            JournalLabel.Location = new Point(12, 9);
            JournalLabel.Name = "JournalLabel";
            JournalLabel.Size = new Size(45, 15);
            JournalLabel.TabIndex = 1;
            JournalLabel.Text = "foo.log";
            // 
            // CancelButton
            // 
            CancelButton.Location = new Point(308, 56);
            CancelButton.Name = "CancelButton";
            CancelButton.Size = new Size(75, 23);
            CancelButton.TabIndex = 2;
            CancelButton.Text = "Cancel";
            CancelButton.UseVisualStyleBackColor = true;
            CancelButton.Click += CancelButton_Click;
            // 
            // ReadAllForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(395, 86);
            Controls.Add(CancelButton);
            Controls.Add(JournalLabel);
            Controls.Add(ReadAllProgress);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "ReadAllForm";
            Text = "Read All In Progress...";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ProgressBar ReadAllProgress;
        private Label JournalLabel;
        private Button CancelButton;
    }
}