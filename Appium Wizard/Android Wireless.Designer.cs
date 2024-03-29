namespace Appium_Wizard
{
    partial class AndroidWireless
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AndroidWireless));
            label1 = new Label();
            label2 = new Label();
            IPAddressTextBox = new TextBox();
            PortTextBox = new TextBox();
            FindDeviceButton = new Button();
            TryAlternativeLinkLabel = new LinkLabel();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Location = new Point(12, 22);
            label1.Name = "label1";
            label1.Size = new Size(504, 25);
            label1.TabIndex = 0;
            label1.Text = "Follow the below steps to Add Android Device Over Wi-Fi:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 69);
            label2.Name = "label2";
            label2.Size = new Size(575, 100);
            label2.TabIndex = 1;
            label2.Text = resources.GetString("label2.Text");
            // 
            // IPAddressTextBox
            // 
            IPAddressTextBox.Location = new Point(39, 196);
            IPAddressTextBox.Name = "IPAddressTextBox";
            IPAddressTextBox.PlaceholderText = "IP Address";
            IPAddressTextBox.Size = new Size(257, 31);
            IPAddressTextBox.TabIndex = 2;
            // 
            // PortTextBox
            // 
            PortTextBox.Location = new Point(302, 196);
            PortTextBox.Name = "PortTextBox";
            PortTextBox.PlaceholderText = "Port";
            PortTextBox.Size = new Size(150, 31);
            PortTextBox.TabIndex = 3;
            // 
            // FindDeviceButton
            // 
            FindDeviceButton.Enabled = false;
            FindDeviceButton.Location = new Point(234, 262);
            FindDeviceButton.Name = "FindDeviceButton";
            FindDeviceButton.Size = new Size(112, 34);
            FindDeviceButton.TabIndex = 4;
            FindDeviceButton.Text = "Find Device";
            FindDeviceButton.UseVisualStyleBackColor = true;
            FindDeviceButton.Click += FindDeviceButton_Click;
            // 
            // TryAlternativeLinkLabel
            // 
            TryAlternativeLinkLabel.AutoSize = true;
            TryAlternativeLinkLabel.Location = new Point(465, 117);
            TryAlternativeLinkLabel.Name = "TryAlternativeLinkLabel";
            TryAlternativeLinkLabel.Size = new Size(123, 25);
            TryAlternativeLinkLabel.TabIndex = 5;
            TryAlternativeLinkLabel.TabStop = true;
            TryAlternativeLinkLabel.Text = "Try Alternative";
            TryAlternativeLinkLabel.LinkClicked += TryAlternative_LinkClicked;
            // 
            // AndroidWireless
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(611, 310);
            Controls.Add(TryAlternativeLinkLabel);
            Controls.Add(FindDeviceButton);
            Controls.Add(PortTextBox);
            Controls.Add(IPAddressTextBox);
            Controls.Add(label2);
            Controls.Add(label1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AndroidWireless";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Android Wireless";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private TextBox IPAddressTextBox;
        private TextBox PortTextBox;
        private Button FindDeviceButton;
        private LinkLabel TryAlternativeLinkLabel;
    }
}