namespace Appium_Wizard
{
    partial class AddRemoteDevice
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddRemoteDevice));
            IPTextBox = new TextBox();
            label1 = new Label();
            label2 = new Label();
            portTextBox = new TextBox();
            findDevicesButton = new Button();
            SuspendLayout();
            // 
            // IPTextBox
            // 
            IPTextBox.BorderStyle = BorderStyle.FixedSingle;
            IPTextBox.Location = new Point(150, 55);
            IPTextBox.Name = "IPTextBox";
            IPTextBox.Size = new Size(200, 23);
            IPTextBox.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(34, 59);
            label1.Name = "label1";
            label1.Size = new Size(91, 15);
            label1.TabIndex = 1;
            label1.Text = "Enter Remote IP";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(34, 115);
            label2.Name = "label2";
            label2.Size = new Size(106, 15);
            label2.TabIndex = 2;
            label2.Text = "Enter Port Number";
            // 
            // portTextBox
            // 
            portTextBox.BorderStyle = BorderStyle.FixedSingle;
            portTextBox.Location = new Point(150, 111);
            portTextBox.Name = "portTextBox";
            portTextBox.Size = new Size(100, 23);
            portTextBox.TabIndex = 3;
            // 
            // findDevicesButton
            // 
            findDevicesButton.AutoSize = true;
            findDevicesButton.Location = new Point(150, 157);
            findDevicesButton.Name = "findDevicesButton";
            findDevicesButton.Size = new Size(107, 25);
            findDevicesButton.TabIndex = 4;
            findDevicesButton.Text = "Find Devices";
            findDevicesButton.UseVisualStyleBackColor = true;
            findDevicesButton.Click += findDevicesButton_Click;
            // 
            // AddRemoteDevice
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(412, 226);
            Controls.Add(findDevicesButton);
            Controls.Add(portTextBox);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(IPTextBox);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AddRemoteDevice";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Add Remote Device";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox IPTextBox;
        private Label label1;
        private Label label2;
        private TextBox portTextBox;
        private Button findDevicesButton;
    }
}