namespace Appium_Wizard
{
    partial class iOS_Proxy
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(iOS_Proxy));
            label1 = new Label();
            portTextBox = new TextBox();
            startProxyButton = new Button();
            deviceListComboBox = new ComboBox();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            methodRadioButton1 = new RadioButton();
            methodRadioButton2 = new RadioButton();
            methodRadioButton3 = new RadioButton();
            label5 = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label1.Location = new Point(54, 130);
            label1.Name = "label1";
            label1.Size = new Size(113, 15);
            label1.TabIndex = 0;
            label1.Text = "Enter Port Number";
            // 
            // portTextBox
            // 
            portTextBox.Location = new Point(170, 126);
            portTextBox.Name = "portTextBox";
            portTextBox.Size = new Size(179, 23);
            portTextBox.TabIndex = 1;
            // 
            // startProxyButton
            // 
            startProxyButton.AutoSize = true;
            startProxyButton.Location = new Point(196, 238);
            startProxyButton.Name = "startProxyButton";
            startProxyButton.Size = new Size(75, 25);
            startProxyButton.TabIndex = 2;
            startProxyButton.Text = "Start Proxy";
            startProxyButton.UseVisualStyleBackColor = true;
            startProxyButton.Click += startProxyButton_Click;
            // 
            // deviceListComboBox
            // 
            deviceListComboBox.FormattingEnabled = true;
            deviceListComboBox.Location = new Point(170, 73);
            deviceListComboBox.Name = "deviceListComboBox";
            deviceListComboBox.Size = new Size(179, 23);
            deviceListComboBox.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label2.Location = new Point(54, 76);
            label2.Name = "label2";
            label2.Size = new Size(84, 15);
            label2.TabIndex = 4;
            label2.Text = "Select Device";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            label3.ForeColor = Color.IndianRed;
            label3.Location = new Point(120, 152);
            label3.Name = "label3";
            label3.Size = new Size(292, 30);
            label3.TabIndex = 5;
            label3.Text = "Enter the port number of 'appium:webDriverAgentUrl' \r\nas displayed in the Appium server logs at startup.";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            label4.Location = new Point(12, 7);
            label4.Name = "label4";
            label4.Size = new Size(424, 45);
            label4.TabIndex = 6;
            label4.Text = "Use this feature if you encounter communication issues with your iPhone during\r\nautomation execution,such as a 'Could not proxy command to the remote server'\r\nerror in the Appium server logs.";
            // 
            // methodRadioButton1
            // 
            methodRadioButton1.AutoSize = true;
            methodRadioButton1.Location = new Point(172, 205);
            methodRadioButton1.Name = "methodRadioButton1";
            methodRadioButton1.Size = new Size(76, 19);
            methodRadioButton1.TabIndex = 7;
            methodRadioButton1.TabStop = true;
            methodRadioButton1.Text = "Method 1";
            methodRadioButton1.UseVisualStyleBackColor = true;
            // 
            // methodRadioButton2
            // 
            methodRadioButton2.AutoSize = true;
            methodRadioButton2.Location = new Point(254, 205);
            methodRadioButton2.Name = "methodRadioButton2";
            methodRadioButton2.Size = new Size(76, 19);
            methodRadioButton2.TabIndex = 8;
            methodRadioButton2.TabStop = true;
            methodRadioButton2.Text = "Method 2";
            methodRadioButton2.UseVisualStyleBackColor = true;
            // 
            // methodRadioButton3
            // 
            methodRadioButton3.AutoSize = true;
            methodRadioButton3.Location = new Point(336, 205);
            methodRadioButton3.Name = "methodRadioButton3";
            methodRadioButton3.Size = new Size(76, 19);
            methodRadioButton3.TabIndex = 9;
            methodRadioButton3.TabStop = true;
            methodRadioButton3.Text = "Method 3";
            methodRadioButton3.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label5.Location = new Point(54, 205);
            label5.Name = "label5";
            label5.Size = new Size(89, 15);
            label5.TabIndex = 10;
            label5.Text = "Select Method";
            // 
            // iOS_Proxy
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLightLight;
            ClientSize = new Size(468, 278);
            Controls.Add(label5);
            Controls.Add(methodRadioButton3);
            Controls.Add(methodRadioButton2);
            Controls.Add(methodRadioButton1);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(deviceListComboBox);
            Controls.Add(startProxyButton);
            Controls.Add(portTextBox);
            Controls.Add(label1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "iOS_Proxy";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "iOS Proxy - Start Proxy Manually";
            Load += iOS_Proxy_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox portTextBox;
        private Button startProxyButton;
        private ComboBox deviceListComboBox;
        private Label label2;
        private Label label3;
        private Label label4;
        private RadioButton methodRadioButton1;
        private RadioButton methodRadioButton2;
        private RadioButton methodRadioButton3;
        private Label label5;
    }
}