namespace Appium_Wizard
{
    partial class CommonProgress
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CommonProgress));
            commonProgressLabel = new Label();
            progressBar1 = new ProgressBar();
            SuspendLayout();
            // 
            // commonProgressLabel
            // 
            commonProgressLabel.AutoSize = true;
            commonProgressLabel.ForeColor = SystemColors.ControlText;
            commonProgressLabel.Location = new Point(19, 63);
            commonProgressLabel.Name = "commonProgressLabel";
            commonProgressLabel.Size = new Size(101, 25);
            commonProgressLabel.TabIndex = 0;
            commonProgressLabel.Text = "In Progress";
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(25, 26);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(673, 34);
            progressBar1.TabIndex = 3;
            // 
            // CommonProgress
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLightLight;
            ClientSize = new Size(728, 176);
            ControlBox = false;
            Controls.Add(progressBar1);
            Controls.Add(commonProgressLabel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CommonProgress";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "In Progress";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label commonProgressLabel;
        private ProgressBar progressBar1;
    }
}