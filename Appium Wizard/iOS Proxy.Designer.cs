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
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(58, 130);
            label1.Name = "label1";
            label1.Size = new Size(106, 15);
            label1.TabIndex = 0;
            label1.Text = "Enter Port Number";
            // 
            // portTextBox
            // 
            portTextBox.Location = new Point(170, 126);
            portTextBox.Name = "portTextBox";
            portTextBox.Size = new Size(100, 23);
            portTextBox.TabIndex = 1;
            // 
            // startProxyButton
            // 
            startProxyButton.AutoSize = true;
            startProxyButton.Location = new Point(276, 125);
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
            label2.Location = new Point(60, 76);
            label2.Name = "label2";
            label2.Size = new Size(76, 15);
            label2.TabIndex = 4;
            label2.Text = "Select Device";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            label3.ForeColor = Color.IndianRed;
            label3.Location = new Point(60, 170);
            label3.Name = "label3";
            label3.Size = new Size(323, 30);
            label3.TabIndex = 5;
            label3.Text = "Note: Enter the port number of 'appium:webDriverAgentUrl' \r\nas displayed in the Appium server logs at startup.";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            label4.Location = new Point(12, 7);
            label4.Name = "label4";
            label4.Size = new Size(424, 45);
            label4.TabIndex = 6;
            label4.Text = "Use this feature if you encounter communication issues with your iPhone during\r\nautomation execution,such as a 'Could not proxy command to the remote server'\r\nerror in the Appium server logs.\"";
            // 
            // iOS_Proxy
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(468, 224);
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
            StartPosition = FormStartPosition.CenterScreen;
            Text = "iOS Proxy";
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
    }
}