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
            goRadioButtonAuto = new RadioButton();
            pyRadioButtonAuto = new RadioButton();
            iProxyRadioButtonAuto = new RadioButton();
            label5 = new Label();
            ApplyButton = new Button();
            cancelButton = new Button();
            groupBox1 = new GroupBox();
            label7 = new Label();
            groupBox2 = new GroupBox();
            label8 = new Label();
            label6 = new Label();
            iProxyRadioButton = new RadioButton();
            pyRadioButton = new RadioButton();
            goRadioButton = new RadioButton();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label1.Location = new Point(31, 94);
            label1.Name = "label1";
            label1.Size = new Size(113, 15);
            label1.TabIndex = 0;
            label1.Text = "Enter Port Number";
            // 
            // portTextBox
            // 
            portTextBox.BorderStyle = BorderStyle.FixedSingle;
            portTextBox.Location = new Point(154, 90);
            portTextBox.Name = "portTextBox";
            portTextBox.Size = new Size(179, 23);
            portTextBox.TabIndex = 1;
            // 
            // startProxyButton
            // 
            startProxyButton.AutoSize = true;
            startProxyButton.Location = new Point(239, 156);
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
            deviceListComboBox.Location = new Point(155, 52);
            deviceListComboBox.Name = "deviceListComboBox";
            deviceListComboBox.Size = new Size(179, 23);
            deviceListComboBox.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label2.Location = new Point(31, 55);
            label2.Name = "label2";
            label2.Size = new Size(84, 15);
            label2.TabIndex = 4;
            label2.Text = "Select Device";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            label3.ForeColor = Color.Sienna;
            label3.Location = new Point(102, 117);
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
            // goRadioButtonAuto
            // 
            goRadioButtonAuto.AutoSize = true;
            goRadioButtonAuto.Location = new Point(156, 22);
            goRadioButtonAuto.Name = "goRadioButtonAuto";
            goRadioButtonAuto.Size = new Size(76, 19);
            goRadioButtonAuto.TabIndex = 7;
            goRadioButtonAuto.TabStop = true;
            goRadioButtonAuto.Text = "Method 1";
            goRadioButtonAuto.UseVisualStyleBackColor = true;
            // 
            // pyRadioButtonAuto
            // 
            pyRadioButtonAuto.AutoSize = true;
            pyRadioButtonAuto.Location = new Point(238, 22);
            pyRadioButtonAuto.Name = "pyRadioButtonAuto";
            pyRadioButtonAuto.Size = new Size(76, 19);
            pyRadioButtonAuto.TabIndex = 8;
            pyRadioButtonAuto.TabStop = true;
            pyRadioButtonAuto.Text = "Method 2";
            pyRadioButtonAuto.UseVisualStyleBackColor = true;
            // 
            // iProxyRadioButtonAuto
            // 
            iProxyRadioButtonAuto.AutoSize = true;
            iProxyRadioButtonAuto.Location = new Point(320, 22);
            iProxyRadioButtonAuto.Name = "iProxyRadioButtonAuto";
            iProxyRadioButtonAuto.Size = new Size(76, 19);
            iProxyRadioButtonAuto.TabIndex = 9;
            iProxyRadioButtonAuto.TabStop = true;
            iProxyRadioButtonAuto.Text = "Method 3";
            iProxyRadioButtonAuto.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label5.Location = new Point(38, 24);
            label5.Name = "label5";
            label5.Size = new Size(89, 15);
            label5.TabIndex = 10;
            label5.Text = "Select Method";
            // 
            // ApplyButton
            // 
            ApplyButton.AutoSize = true;
            ApplyButton.Location = new Point(174, 51);
            ApplyButton.Name = "ApplyButton";
            ApplyButton.Size = new Size(93, 25);
            ApplyButton.TabIndex = 12;
            ApplyButton.Text = "Apply && Close";
            ApplyButton.UseVisualStyleBackColor = true;
            ApplyButton.Click += ApplyButton_Click;
            // 
            // cancelButton
            // 
            cancelButton.AutoSize = true;
            cancelButton.Location = new Point(122, 155);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(75, 25);
            cancelButton.TabIndex = 11;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += cancelButton_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label7);
            groupBox1.Controls.Add(ApplyButton);
            groupBox1.Controls.Add(goRadioButtonAuto);
            groupBox1.Controls.Add(pyRadioButtonAuto);
            groupBox1.Controls.Add(iProxyRadioButtonAuto);
            groupBox1.Controls.Add(label5);
            groupBox1.Location = new Point(17, 66);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(439, 101);
            groupBox1.TabIndex = 15;
            groupBox1.TabStop = false;
            groupBox1.Text = "Automatic";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.ForeColor = Color.IndianRed;
            label7.Location = new Point(38, 82);
            label7.Name = "label7";
            label7.Size = new Size(353, 15);
            label7.TabIndex = 0;
            label7.Text = "This setting will be applied when a new appium session is created.";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(label8);
            groupBox2.Controls.Add(label6);
            groupBox2.Controls.Add(startProxyButton);
            groupBox2.Controls.Add(iProxyRadioButton);
            groupBox2.Controls.Add(label1);
            groupBox2.Controls.Add(pyRadioButton);
            groupBox2.Controls.Add(portTextBox);
            groupBox2.Controls.Add(goRadioButton);
            groupBox2.Controls.Add(deviceListComboBox);
            groupBox2.Controls.Add(cancelButton);
            groupBox2.Controls.Add(label2);
            groupBox2.Controls.Add(label3);
            groupBox2.Location = new Point(17, 183);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(439, 204);
            groupBox2.TabIndex = 16;
            groupBox2.TabStop = false;
            groupBox2.Text = "Manual";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.ForeColor = Color.IndianRed;
            label8.Location = new Point(53, 184);
            label8.Name = "label8";
            label8.Size = new Size(326, 15);
            label8.TabIndex = 13;
            label8.Text = "This starts the iOS proxy immediately for the selected device.";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label6.Location = new Point(30, 22);
            label6.Name = "label6";
            label6.Size = new Size(89, 15);
            label6.TabIndex = 20;
            label6.Text = "Select Method";
            // 
            // iProxyRadioButton
            // 
            iProxyRadioButton.AutoSize = true;
            iProxyRadioButton.Location = new Point(317, 20);
            iProxyRadioButton.Name = "iProxyRadioButton";
            iProxyRadioButton.Size = new Size(76, 19);
            iProxyRadioButton.TabIndex = 19;
            iProxyRadioButton.TabStop = true;
            iProxyRadioButton.Text = "Method 3";
            iProxyRadioButton.UseVisualStyleBackColor = true;
            // 
            // pyRadioButton
            // 
            pyRadioButton.AutoSize = true;
            pyRadioButton.Location = new Point(235, 20);
            pyRadioButton.Name = "pyRadioButton";
            pyRadioButton.Size = new Size(76, 19);
            pyRadioButton.TabIndex = 18;
            pyRadioButton.TabStop = true;
            pyRadioButton.Text = "Method 2";
            pyRadioButton.UseVisualStyleBackColor = true;
            // 
            // goRadioButton
            // 
            goRadioButton.AutoSize = true;
            goRadioButton.Location = new Point(153, 20);
            goRadioButton.Name = "goRadioButton";
            goRadioButton.Size = new Size(76, 19);
            goRadioButton.TabIndex = 17;
            goRadioButton.TabStop = true;
            goRadioButton.Text = "Method 1";
            goRadioButton.UseVisualStyleBackColor = true;
            // 
            // iOS_Proxy
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLightLight;
            ClientSize = new Size(468, 392);
            Controls.Add(label4);
            Controls.Add(groupBox1);
            Controls.Add(groupBox2);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "iOS_Proxy";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "iOS Proxy";
            Load += iOS_Proxy_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
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
        private RadioButton goRadioButtonAuto;
        private RadioButton pyRadioButtonAuto;
        private RadioButton iProxyRadioButtonAuto;
        private Label label5;
        private Button ApplyButton;
        private Button cancelButton;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Label label6;
        private RadioButton iProxyRadioButton;
        private RadioButton pyRadioButton;
        private RadioButton goRadioButton;
        private Label label7;
        private Label label8;
    }
}