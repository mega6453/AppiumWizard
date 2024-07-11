namespace Appium_Wizard
{
    partial class AndroidWirelessManual
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AndroidWirelessManual));
            PairingIPTextBox = new TextBox();
            ConnectIPTextBox = new TextBox();
            PairingCodeTextbox = new TextBox();
            label1 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label7 = new Label();
            label8 = new Label();
            PairButton = new Button();
            CancelButton = new Button();
            PairPortNumberTextbox = new TextBox();
            ConnectPortNumber = new TextBox();
            label2 = new Label();
            label6 = new Label();
            CancelButton2 = new Button();
            ConnectButton = new Button();
            label9 = new Label();
            label10 = new Label();
            label11 = new Label();
            panel1 = new Panel();
            panel2 = new Panel();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // PairingIPTextBox
            // 
            PairingIPTextBox.Location = new Point(112, 115);
            PairingIPTextBox.Margin = new Padding(2);
            PairingIPTextBox.Name = "PairingIPTextBox";
            PairingIPTextBox.PlaceholderText = "IP Address";
            PairingIPTextBox.Size = new Size(124, 23);
            PairingIPTextBox.TabIndex = 0;
            PairingIPTextBox.TextChanged += PairingIPTextBox_TextChanged;
            // 
            // ConnectIPTextBox
            // 
            ConnectIPTextBox.Enabled = false;
            ConnectIPTextBox.Location = new Point(108, 307);
            ConnectIPTextBox.Margin = new Padding(2);
            ConnectIPTextBox.Name = "ConnectIPTextBox";
            ConnectIPTextBox.PlaceholderText = "IP Address";
            ConnectIPTextBox.Size = new Size(124, 23);
            ConnectIPTextBox.TabIndex = 2;
            // 
            // PairingCodeTextbox
            // 
            PairingCodeTextbox.Location = new Point(112, 150);
            PairingCodeTextbox.Margin = new Padding(2);
            PairingCodeTextbox.Name = "PairingCodeTextbox";
            PairingCodeTextbox.PlaceholderText = "6 digit code";
            PairingCodeTextbox.Size = new Size(89, 23);
            PairingCodeTextbox.TabIndex = 4;
            PairingCodeTextbox.TextChanged += PairingCodeTextbox_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(29, 115);
            label1.Margin = new Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new Size(62, 15);
            label1.TabIndex = 5;
            label1.Text = "IP Address";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(29, 179);
            label3.Margin = new Padding(2, 0, 2, 0);
            label3.Name = "label3";
            label3.Size = new Size(0, 15);
            label3.TabIndex = 7;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(29, 150);
            label4.Margin = new Padding(2, 0, 2, 0);
            label4.Name = "label4";
            label4.Size = new Size(75, 15);
            label4.TabIndex = 8;
            label4.Text = "Pairing Code";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Enabled = false;
            label5.Location = new Point(24, 307);
            label5.Margin = new Padding(2, 0, 2, 0);
            label5.Name = "label5";
            label5.Size = new Size(62, 15);
            label5.TabIndex = 9;
            label5.Text = "IP Address";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 9F, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point);
            label7.Location = new Point(29, 90);
            label7.Margin = new Padding(2, 0, 2, 0);
            label7.Name = "label7";
            label7.Size = new Size(87, 15);
            label7.TabIndex = 11;
            label7.Text = "Pairing details:";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Enabled = false;
            label8.Font = new Font("Segoe UI", 9F, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point);
            label8.Location = new Point(24, 283);
            label8.Margin = new Padding(2, 0, 2, 0);
            label8.Name = "label8";
            label8.Size = new Size(112, 15);
            label8.TabIndex = 12;
            label8.Text = "Connecting details:";
            // 
            // PairButton
            // 
            PairButton.AutoSize = true;
            PairButton.Enabled = false;
            PairButton.Location = new Point(274, 165);
            PairButton.Margin = new Padding(2);
            PairButton.Name = "PairButton";
            PairButton.Size = new Size(78, 25);
            PairButton.TabIndex = 13;
            PairButton.Text = "Pair";
            PairButton.UseVisualStyleBackColor = true;
            PairButton.Click += PairButton_Click;
            // 
            // CancelButton
            // 
            CancelButton.AutoSize = true;
            CancelButton.Location = new Point(176, 165);
            CancelButton.Margin = new Padding(2);
            CancelButton.Name = "CancelButton";
            CancelButton.Size = new Size(78, 25);
            CancelButton.TabIndex = 14;
            CancelButton.Text = "Cancel";
            CancelButton.UseVisualStyleBackColor = true;
            CancelButton.Click += CancelButton_Click;
            // 
            // PairPortNumberTextbox
            // 
            PairPortNumberTextbox.Location = new Point(246, 92);
            PairPortNumberTextbox.Margin = new Padding(2);
            PairPortNumberTextbox.Name = "PairPortNumberTextbox";
            PairPortNumberTextbox.PlaceholderText = "Port number";
            PairPortNumberTextbox.Size = new Size(89, 23);
            PairPortNumberTextbox.TabIndex = 15;
            PairPortNumberTextbox.TextChanged += PairPortNumberTextbox_TextChanged;
            // 
            // ConnectPortNumber
            // 
            ConnectPortNumber.Enabled = false;
            ConnectPortNumber.Location = new Point(245, 307);
            ConnectPortNumber.Margin = new Padding(2);
            ConnectPortNumber.Name = "ConnectPortNumber";
            ConnectPortNumber.PlaceholderText = "Port number";
            ConnectPortNumber.Size = new Size(89, 23);
            ConnectPortNumber.TabIndex = 16;
            ConnectPortNumber.TextChanged += ConnectPortNumber_TextChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label2.Location = new Point(236, 95);
            label2.Margin = new Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new Size(10, 15);
            label2.TabIndex = 17;
            label2.Text = ":";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Enabled = false;
            label6.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label6.Location = new Point(234, 309);
            label6.Margin = new Padding(2, 0, 2, 0);
            label6.Name = "label6";
            label6.Size = new Size(10, 15);
            label6.TabIndex = 18;
            label6.Text = ":";
            // 
            // CancelButton2
            // 
            CancelButton2.AutoSize = true;
            CancelButton2.Enabled = false;
            CancelButton2.Location = new Point(176, 137);
            CancelButton2.Margin = new Padding(2);
            CancelButton2.Name = "CancelButton2";
            CancelButton2.Size = new Size(78, 25);
            CancelButton2.TabIndex = 20;
            CancelButton2.Text = "Cancel";
            CancelButton2.UseVisualStyleBackColor = true;
            CancelButton2.Click += CancelButton2_Click;
            // 
            // ConnectButton
            // 
            ConnectButton.AutoSize = true;
            ConnectButton.Enabled = false;
            ConnectButton.Location = new Point(274, 137);
            ConnectButton.Margin = new Padding(2);
            ConnectButton.Name = "ConnectButton";
            ConnectButton.Size = new Size(78, 25);
            ConnectButton.TabIndex = 19;
            ConnectButton.Text = "Connect";
            ConnectButton.UseVisualStyleBackColor = true;
            ConnectButton.Click += ConnectButton_Click;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(29, 29);
            label9.Margin = new Padding(2, 0, 2, 0);
            label9.Name = "label9";
            label9.Size = new Size(478, 45);
            label9.TabIndex = 21;
            label9.Text = resources.GetString("label9.Text");
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label10.Location = new Point(29, 5);
            label10.Margin = new Padding(2, 0, 2, 0);
            label10.Name = "label10";
            label10.Size = new Size(404, 15);
            label10.TabIndex = 22;
            label10.Text = "Follow the below steps to Add Android 11+ Device manually over Wi-Fi:";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Enabled = false;
            label11.Location = new Point(24, 228);
            label11.Margin = new Padding(2, 0, 2, 0);
            label11.Name = "label11";
            label11.Size = new Size(459, 45);
            label11.TabIndex = 23;
            label11.Text = "1. Once the device paired, the pair popup will be closed.\r\n2. Now find the IP address and port displaying in the Wireless debugging main screen.\r\n3. Fill the details below and Click Connect button.";
            // 
            // panel1
            // 
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel1.Controls.Add(CancelButton2);
            panel1.Controls.Add(ConnectButton);
            panel1.Location = new Point(2, 220);
            panel1.Margin = new Padding(2);
            panel1.Name = "panel1";
            panel1.Size = new Size(553, 176);
            panel1.TabIndex = 24;
            // 
            // panel2
            // 
            panel2.BorderStyle = BorderStyle.FixedSingle;
            panel2.Controls.Add(CancelButton);
            panel2.Controls.Add(PairButton);
            panel2.Controls.Add(PairPortNumberTextbox);
            panel2.Controls.Add(label2);
            panel2.Location = new Point(2, 22);
            panel2.Margin = new Padding(2);
            panel2.Name = "panel2";
            panel2.Size = new Size(553, 195);
            panel2.TabIndex = 0;
            // 
            // AndroidWirelessManual
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLightLight;
            ClientSize = new Size(556, 399);
            Controls.Add(label11);
            Controls.Add(label10);
            Controls.Add(label9);
            Controls.Add(label6);
            Controls.Add(ConnectPortNumber);
            Controls.Add(label8);
            Controls.Add(label7);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label1);
            Controls.Add(PairingCodeTextbox);
            Controls.Add(ConnectIPTextBox);
            Controls.Add(PairingIPTextBox);
            Controls.Add(panel1);
            Controls.Add(panel2);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(2);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AndroidWirelessManual";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Android Wireless Manual";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox PairingIPTextBox;
        private TextBox ConnectIPTextBox;
        private TextBox PairingCodeTextbox;
        private Label label1;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label7;
        private Label label8;
        private Button PairButton;
        private Button CancelButton;
        private TextBox PairPortNumberTextbox;
        private TextBox ConnectPortNumber;
        private Label label2;
        private Label label6;
        private Button CancelButton2;
        private Button ConnectButton;
        private Label label9;
        private Label label10;
        private Label label11;
        private Panel panel1;
        private Panel panel2;
    }
}