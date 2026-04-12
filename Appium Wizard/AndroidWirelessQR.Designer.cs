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
            label1 = new Label();
            pictureBoxQR = new PictureBox();
            lblInstructions = new Label();
            label2 = new Label();
            lblPairingCode = new Label();
            label3 = new Label();
            lblIPAddress = new Label();
            lblStatus = new Label();
            btnCancel = new Button();
            panel1 = new Panel();
            btnCopyDetails = new Button();
            btnPairManually = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBoxQR).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            //
            // label1
            //
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(312, 21);
            label1.TabIndex = 0;
            label1.Text = "Pair Android Device with QR Code";
            //
            // pictureBoxQR
            //
            pictureBoxQR.BorderStyle = BorderStyle.FixedSingle;
            pictureBoxQR.Location = new Point(3, 3);
            pictureBoxQR.Name = "pictureBoxQR";
            pictureBoxQR.Size = new Size(300, 300);
            pictureBoxQR.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxQR.TabIndex = 1;
            pictureBoxQR.TabStop = false;
            //
            // lblInstructions
            //
            lblInstructions.Location = new Point(12, 360);
            lblInstructions.Name = "lblInstructions";
            lblInstructions.Size = new Size(460, 110);
            lblInstructions.TabIndex = 2;
            lblInstructions.Text = "Loading instructions...";
            //
            // label2
            //
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label2.Location = new Point(12, 480);
            label2.Name = "label2";
            label2.Size = new Size(85, 15);
            label2.TabIndex = 3;
            label2.Text = "Pairing Code:";
            //
            // lblPairingCode
            //
            lblPairingCode.AutoSize = true;
            lblPairingCode.Font = new Font("Consolas", 14F, FontStyle.Bold, GraphicsUnit.Point);
            lblPairingCode.ForeColor = Color.Blue;
            lblPairingCode.Location = new Point(103, 475);
            lblPairingCode.Name = "lblPairingCode";
            lblPairingCode.Size = new Size(80, 22);
            lblPairingCode.TabIndex = 4;
            lblPairingCode.Text = "------";
            //
            // label3
            //
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label3.Location = new Point(12, 505);
            label3.Name = "label3";
            label3.Size = new Size(70, 15);
            label3.TabIndex = 5;
            label3.Text = "IP Address:";
            //
            // lblIPAddress
            //
            lblIPAddress.AutoSize = true;
            lblIPAddress.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            lblIPAddress.Location = new Point(103, 505);
            lblIPAddress.Name = "lblIPAddress";
            lblIPAddress.Size = new Size(98, 14);
            lblIPAddress.TabIndex = 6;
            lblIPAddress.Text = "-------------";
            //
            // lblStatus
            //
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Segoe UI", 9F, FontStyle.Italic, GraphicsUnit.Point);
            lblStatus.ForeColor = Color.Gray;
            lblStatus.Location = new Point(12, 532);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(172, 15);
            lblStatus.TabIndex = 7;
            lblStatus.Text = "Waiting for device to scan QR...";
            //
            // btnCancel
            //
            btnCancel.Location = new Point(397, 560);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 27);
            btnCancel.TabIndex = 8;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            //
            // btnCopyDetails
            //
            btnCopyDetails.Location = new Point(150, 560);
            btnCopyDetails.Name = "btnCopyDetails";
            btnCopyDetails.Size = new Size(100, 27);
            btnCopyDetails.TabIndex = 10;
            btnCopyDetails.Text = "Copy Details";
            btnCopyDetails.UseVisualStyleBackColor = true;
            btnCopyDetails.Click += btnCopyDetails_Click;
            //
            // btnPairManually
            //
            btnPairManually.Location = new Point(260, 560);
            btnPairManually.Name = "btnPairManually";
            btnPairManually.Size = new Size(120, 27);
            btnPairManually.TabIndex = 11;
            btnPairManually.Text = "Pair Manually";
            btnPairManually.UseVisualStyleBackColor = true;
            btnPairManually.Click += btnPairManually_Click;
            //
            // panel1
            //
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel1.Controls.Add(pictureBoxQR);
            panel1.Location = new Point(81, 40);
            panel1.Name = "panel1";
            panel1.Size = new Size(308, 308);
            panel1.TabIndex = 9;
            //
            // AndroidWirelessQR
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLightLight;
            ClientSize = new Size(484, 600);
            Controls.Add(btnPairManually);
            Controls.Add(btnCopyDetails);
            Controls.Add(panel1);
            Controls.Add(btnCancel);
            Controls.Add(lblStatus);
            Controls.Add(lblIPAddress);
            Controls.Add(label3);
            Controls.Add(lblPairingCode);
            Controls.Add(label2);
            Controls.Add(lblInstructions);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AndroidWirelessQR";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Android QR Code Pairing";
            FormClosing += AndroidWirelessQR_FormClosing;
            Load += AndroidWirelessQR_Load;
            Shown += AndroidWirelessQR_Shown;
            ((System.ComponentModel.ISupportInitialize)pictureBoxQR).EndInit();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private PictureBox pictureBoxQR;
        private Label lblInstructions;
        private Label label2;
        private Label lblPairingCode;
        private Label label3;
        private Label lblIPAddress;
        private Label lblStatus;
        private Button btnCancel;
        private Panel panel1;
        private Button btnCopyDetails;
        private Button btnPairManually;
    }
}
