namespace Appium_Wizard
{
    partial class AndroidWirelessQR
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AndroidWirelessQR));
            instructionsLabel = new Label();
            qrPictureBox = new PictureBox();
            statusLabel = new Label();
            cancelButton = new Button();
            ((System.ComponentModel.ISupportInitialize)qrPictureBox).BeginInit();
            SuspendLayout();
            // 
            // instructionsLabel
            // 
            instructionsLabel.Location = new Point(12, 10);
            instructionsLabel.Margin = new Padding(2, 0, 2, 0);
            instructionsLabel.Name = "instructionsLabel";
            instructionsLabel.Size = new Size(433, 47);
            instructionsLabel.TabIndex = 0;
            instructionsLabel.Text = "On the phone, go to Developer options > Wireless debugging > Pair device with QR code, then scan the code below.";
            // 
            // qrPictureBox
            // 
            qrPictureBox.Location = new Point(89, 72);
            qrPictureBox.Margin = new Padding(2);
            qrPictureBox.Name = "qrPictureBox";
            qrPictureBox.Size = new Size(260, 260);
            qrPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            qrPictureBox.TabIndex = 1;
            qrPictureBox.TabStop = false;
            // 
            // statusLabel
            // 
            statusLabel.Location = new Point(69, 334);
            statusLabel.Margin = new Padding(2, 0, 2, 0);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(316, 45);
            statusLabel.TabIndex = 2;
            statusLabel.Text = "Waiting for the QR code to be scanned...";
            statusLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // cancelButton
            // 
            cancelButton.AutoSize = true;
            cancelButton.Location = new Point(169, 381);
            cancelButton.Margin = new Padding(2);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(80, 25);
            cancelButton.TabIndex = 3;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += cancelButton_Click;
            // 
            // AndroidWirelessQR
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLightLight;
            ClientSize = new Size(456, 430);
            Controls.Add(cancelButton);
            Controls.Add(statusLabel);
            Controls.Add(qrPictureBox);
            Controls.Add(instructionsLabel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(2);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AndroidWirelessQR";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Pair with QR code";
            FormClosing += AndroidWirelessQR_FormClosing;
            Shown += AndroidWirelessQR_Shown;
            ((System.ComponentModel.ISupportInitialize)qrPictureBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label instructionsLabel;
        private PictureBox qrPictureBox;
        private Label statusLabel;
        private Button cancelButton;
    }
}
