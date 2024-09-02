namespace Observatory.UI
{
    partial class DonateForm
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
            DonateLabel = new Label();
            PatreonButton = new Button();
            PaypalButton = new Button();
            SuspendLayout();
            // 
            // DonateLabel
            // 
            DonateLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            DonateLabel.Location = new Point(12, 9);
            DonateLabel.Name = "DonateLabel";
            DonateLabel.Size = new Size(370, 45);
            DonateLabel.TabIndex = 0;
            DonateLabel.Text = "Thank you for considering a donation! Please choose below whether to make a one-time donation via PayPal, or ongoing via Patreon!";
            // 
            // PatreonButton
            // 
            PatreonButton.Location = new Point(81, 57);
            PatreonButton.Name = "PatreonButton";
            PatreonButton.Size = new Size(75, 23);
            PatreonButton.TabIndex = 1;
            PatreonButton.Text = "Patreon";
            PatreonButton.UseVisualStyleBackColor = true;
            PatreonButton.Click += PatreonButtonClick;
            // 
            // PaypalButton
            // 
            PaypalButton.Location = new Point(224, 57);
            PaypalButton.Name = "PaypalButton";
            PaypalButton.Size = new Size(75, 23);
            PaypalButton.TabIndex = 2;
            PaypalButton.Text = "PayPal";
            PaypalButton.UseVisualStyleBackColor = true;
            PaypalButton.Click += PaypalButtonClick;
            // 
            // DonateForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(394, 100);
            Controls.Add(PaypalButton);
            Controls.Add(PatreonButton);
            Controls.Add(DonateLabel);
            Name = "DonateForm";
            Text = "Observatory Donation";
            ResumeLayout(false);
        }

        #endregion

        private Label DonateLabel;
        private Button PatreonButton;
        private Button PaypalButton;
    }
}