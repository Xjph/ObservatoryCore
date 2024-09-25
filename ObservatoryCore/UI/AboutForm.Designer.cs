namespace Observatory.UI
{
    partial class AboutForm
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
            lblFullNameTitle = new Label();
            lblFullName = new Label();
            lblShortNameTitle = new Label();
            lblShortName = new Label();
            lblDescriptionTitle = new Label();
            lblAuthorNameTitle = new Label();
            lblAuthorName = new Label();
            lnk1 = new LinkLabel();
            lnk2 = new LinkLabel();
            lnk3 = new LinkLabel();
            txtDescription = new TextBox();
            lnk4 = new LinkLabel();
            SuspendLayout();
            // 
            // lblFullNameTitle
            // 
            lblFullNameTitle.Location = new Point(8, 8);
            lblFullNameTitle.Name = "lblFullNameTitle";
            lblFullNameTitle.Size = new Size(152, 15);
            lblFullNameTitle.TabIndex = 0;
            lblFullNameTitle.Text = "Full name:";
            // 
            // lblFullName
            // 
            lblFullName.AutoEllipsis = true;
            lblFullName.Location = new Point(176, 8);
            lblFullName.Name = "lblFullName";
            lblFullName.Size = new Size(328, 15);
            lblFullName.TabIndex = 1;
            // 
            // lblShortNameTitle
            // 
            lblShortNameTitle.Location = new Point(8, 32);
            lblShortNameTitle.Name = "lblShortNameTitle";
            lblShortNameTitle.Size = new Size(152, 15);
            lblShortNameTitle.TabIndex = 2;
            lblShortNameTitle.Text = "Short name:";
            // 
            // lblShortName
            // 
            lblShortName.AutoEllipsis = true;
            lblShortName.Location = new Point(176, 32);
            lblShortName.Name = "lblShortName";
            lblShortName.Size = new Size(328, 15);
            lblShortName.TabIndex = 3;
            // 
            // lblDescriptionTitle
            // 
            lblDescriptionTitle.Location = new Point(8, 80);
            lblDescriptionTitle.Name = "lblDescriptionTitle";
            lblDescriptionTitle.Size = new Size(152, 15);
            lblDescriptionTitle.TabIndex = 6;
            lblDescriptionTitle.Text = "Description:";
            // 
            // lblAuthorNameTitle
            // 
            lblAuthorNameTitle.Location = new Point(8, 56);
            lblAuthorNameTitle.Name = "lblAuthorNameTitle";
            lblAuthorNameTitle.Size = new Size(152, 15);
            lblAuthorNameTitle.TabIndex = 4;
            lblAuthorNameTitle.Text = "Author:";
            // 
            // lblAuthorName
            // 
            lblAuthorName.AutoEllipsis = true;
            lblAuthorName.Location = new Point(176, 56);
            lblAuthorName.Name = "lblAuthorName";
            lblAuthorName.Size = new Size(328, 15);
            lblAuthorName.TabIndex = 5;
            // 
            // lnk1
            // 
            lnk1.AutoEllipsis = true;
            lnk1.Location = new Point(8, 216);
            lnk1.Name = "lnk1";
            lnk1.Size = new Size(240, 16);
            lnk1.TabIndex = 8;
            lnk1.TabStop = true;
            lnk1.Text = "<link>";
            lnk1.TextAlign = ContentAlignment.MiddleCenter;
            lnk1.LinkClicked += OnLinkClicked;
            // 
            // lnk2
            // 
            lnk2.AutoEllipsis = true;
            lnk2.Location = new Point(264, 216);
            lnk2.Name = "lnk2";
            lnk2.Size = new Size(240, 16);
            lnk2.TabIndex = 9;
            lnk2.TabStop = true;
            lnk2.Text = "<link>";
            lnk2.TextAlign = ContentAlignment.MiddleCenter;
            lnk2.LinkClicked += OnLinkClicked;
            // 
            // lnk3
            // 
            lnk3.AutoEllipsis = true;
            lnk3.Location = new Point(8, 240);
            lnk3.Name = "lnk3";
            lnk3.Size = new Size(240, 16);
            lnk3.TabIndex = 10;
            lnk3.TabStop = true;
            lnk3.Text = "<link>";
            lnk3.TextAlign = ContentAlignment.MiddleCenter;
            lnk3.LinkClicked += OnLinkClicked;
            // 
            // txtDescription
            // 
            txtDescription.Location = new Point(176, 80);
            txtDescription.Multiline = true;
            txtDescription.Name = "txtDescription";
            txtDescription.ReadOnly = true;
            txtDescription.ScrollBars = ScrollBars.Vertical;
            txtDescription.Size = new Size(328, 128);
            txtDescription.TabIndex = 7;
            // 
            // lnk4
            // 
            lnk4.AutoEllipsis = true;
            lnk4.Location = new Point(264, 240);
            lnk4.Name = "lnk4";
            lnk4.Size = new Size(240, 16);
            lnk4.TabIndex = 11;
            lnk4.TabStop = true;
            lnk4.Text = "<link>";
            lnk4.TextAlign = ContentAlignment.MiddleCenter;
            lnk4.LinkClicked += OnLinkClicked;
            // 
            // AboutForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(514, 263);
            Controls.Add(lnk4);
            Controls.Add(txtDescription);
            Controls.Add(lnk3);
            Controls.Add(lnk2);
            Controls.Add(lnk1);
            Controls.Add(lblAuthorName);
            Controls.Add(lblAuthorNameTitle);
            Controls.Add(lblDescriptionTitle);
            Controls.Add(lblShortName);
            Controls.Add(lblShortNameTitle);
            Controls.Add(lblFullName);
            Controls.Add(lblFullNameTitle);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AboutForm";
            ShowIcon = false;
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "About ...";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblFullNameTitle;
        private Label lblFullName;
        private Label lblShortNameTitle;
        private Label lblShortName;
        private Label lblDescriptionTitle;
        private Label lblAuthorNameTitle;
        private Label lblAuthorName;
        private LinkLabel lnk1;
        private LinkLabel lnk2;
        private LinkLabel lnk3;
        private TextBox txtDescription;
        private LinkLabel lnk4;
    }
}