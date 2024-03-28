namespace Appium_Wizard
{
    partial class LoadingScreen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoadingScreen));
            statusLabel = new Label();
            productVersion = new Label();
            firstTimeRunLabel = new Label();
            SuspendLayout();
            // 
            // statusLabel
            // 
            statusLabel.AutoSize = true;
            statusLabel.BackColor = Color.Transparent;
            statusLabel.Location = new Point(30, 498);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(0, 25);
            statusLabel.TabIndex = 1;
            // 
            // productVersion
            // 
            productVersion.AutoSize = true;
            productVersion.BackColor = Color.Transparent;
            productVersion.Location = new Point(830, 505);
            productVersion.Name = "productVersion";
            productVersion.Size = new Size(0, 25);
            productVersion.TabIndex = 2;
            // 
            // firstTimeRunLabel
            // 
            firstTimeRunLabel.AutoSize = true;
            firstTimeRunLabel.BackColor = Color.Transparent;
            firstTimeRunLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            firstTimeRunLabel.Location = new Point(30, 465);
            firstTimeRunLabel.Name = "firstTimeRunLabel";
            firstTimeRunLabel.Size = new Size(0, 25);
            firstTimeRunLabel.TabIndex = 3;
            // 
            // LoadingScreen
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.AppiumWizardSplashScreen;
            BackgroundImageLayout = ImageLayout.Zoom;
            ClientSize = new Size(984, 539);
            Controls.Add(firstTimeRunLabel);
            Controls.Add(productVersion);
            Controls.Add(statusLabel);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "LoadingScreen";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "LoadingScreen";
            Load += LoadingScreen_Load;
            Shown += LoadingScreen_Shown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label statusLabel;
        private Label productVersion;
        private Label firstTimeRunLabel;
    }
}