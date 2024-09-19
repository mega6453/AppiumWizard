namespace Appium_Wizard
{
    partial class About
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
            pictureBox1 = new PictureBox();
            tabControl1 = new TabControl();
            AboutTabPage = new TabPage();
            linkLabel1 = new LinkLabel();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            LicenseTabPage = new TabPage();
            LicenseRichTextBox = new RichTextBox();
            ThanksToTabPage = new TabPage();
            ThanksToRichTextBox = new RichTextBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            tabControl1.SuspendLayout();
            AboutTabPage.SuspendLayout();
            LicenseTabPage.SuspendLayout();
            ThanksToTabPage.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Dock = DockStyle.Top;
            pictureBox1.Image = Properties.Resources.appium_wizard_small;
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Margin = new Padding(2);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(396, 65);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(AboutTabPage);
            tabControl1.Controls.Add(LicenseTabPage);
            tabControl1.Controls.Add(ThanksToTabPage);
            tabControl1.Location = new Point(0, 70);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(396, 141);
            tabControl1.TabIndex = 5;
            // 
            // AboutTabPage
            // 
            AboutTabPage.Controls.Add(linkLabel1);
            AboutTabPage.Controls.Add(label3);
            AboutTabPage.Controls.Add(label2);
            AboutTabPage.Controls.Add(label1);
            AboutTabPage.Location = new Point(4, 24);
            AboutTabPage.Name = "AboutTabPage";
            AboutTabPage.Padding = new Padding(3);
            AboutTabPage.Size = new Size(388, 113);
            AboutTabPage.TabIndex = 0;
            AboutTabPage.Text = "About";
            AboutTabPage.UseVisualStyleBackColor = true;
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize = true;
            linkLabel1.Location = new Point(87, 77);
            linkLabel1.Margin = new Padding(2, 0, 2, 0);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(252, 15);
            linkLabel1.TabIndex = 8;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "https://github.com/mega6453/AppiumWizard";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(13, 77);
            label3.Margin = new Padding(2, 0, 2, 0);
            label3.Name = "label3";
            label3.Size = new Size(72, 15);
            label3.TabIndex = 7;
            label3.Text = "Repository : ";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(13, 51);
            label2.Margin = new Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new Size(166, 15);
            label2.TabIndex = 6;
            label2.Text = "Developed By : Meganathan C";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(13, 21);
            label1.Margin = new Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new Size(54, 15);
            label1.TabIndex = 5;
            label1.Text = "Version : ";
            // 
            // LicenseTabPage
            // 
            LicenseTabPage.Controls.Add(LicenseRichTextBox);
            LicenseTabPage.Location = new Point(4, 24);
            LicenseTabPage.Name = "LicenseTabPage";
            LicenseTabPage.Padding = new Padding(3);
            LicenseTabPage.Size = new Size(388, 113);
            LicenseTabPage.TabIndex = 1;
            LicenseTabPage.Text = "License";
            LicenseTabPage.UseVisualStyleBackColor = true;
            // 
            // LicenseRichTextBox
            // 
            LicenseRichTextBox.Location = new Point(0, 0);
            LicenseRichTextBox.Name = "LicenseRichTextBox";
            LicenseRichTextBox.ReadOnly = true;
            LicenseRichTextBox.Size = new Size(392, 113);
            LicenseRichTextBox.TabIndex = 0;
            LicenseRichTextBox.Text = "https://github.com/mega6453/AppiumWizard/blob/master/LICENSE";
            // 
            // ThanksToTabPage
            // 
            ThanksToTabPage.Controls.Add(ThanksToRichTextBox);
            ThanksToTabPage.Location = new Point(4, 24);
            ThanksToTabPage.Name = "ThanksToTabPage";
            ThanksToTabPage.Padding = new Padding(3);
            ThanksToTabPage.Size = new Size(388, 113);
            ThanksToTabPage.TabIndex = 2;
            ThanksToTabPage.Text = "Thanks To";
            ThanksToTabPage.UseVisualStyleBackColor = true;
            // 
            // ThanksToRichTextBox
            // 
            ThanksToRichTextBox.Location = new Point(0, 0);
            ThanksToRichTextBox.Name = "ThanksToRichTextBox";
            ThanksToRichTextBox.ReadOnly = true;
            ThanksToRichTextBox.Size = new Size(388, 113);
            ThanksToRichTextBox.TabIndex = 0;
            ThanksToRichTextBox.Text = "https://github.com/mega6453/AppiumWizard/blob/master/README.md#thanks-to";
            // 
            // About
            // 
            AccessibleRole = AccessibleRole.None;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLightLight;
            BackgroundImageLayout = ImageLayout.None;
            ClientSize = new Size(396, 209);
            Controls.Add(pictureBox1);
            Controls.Add(tabControl1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(2);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "About";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "About";
            Load += About_Load;
            Shown += About_Shown;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            tabControl1.ResumeLayout(false);
            AboutTabPage.ResumeLayout(false);
            AboutTabPage.PerformLayout();
            LicenseTabPage.ResumeLayout(false);
            ThanksToTabPage.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private PictureBox pictureBox1;
        private TabControl tabControl1;
        private TabPage AboutTabPage;
        private TabPage LicenseTabPage;
        private RichTextBox LicenseRichTextBox;
        private TabPage ThanksToTabPage;
        private RichTextBox ThanksToRichTextBox;
        private LinkLabel linkLabel1;
        private Label label3;
        private Label label2;
        private Label label1;
    }
}