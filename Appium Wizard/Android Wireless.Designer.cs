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
            FindDeviceButton = new Button();
            label3 = new Label();
            listView1 = new ListView();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            PairButton = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Location = new Point(12, 22);
            label1.Name = "label1";
            label1.Size = new Size(542, 25);
            label1.TabIndex = 0;
            label1.Text = "Follow the below steps to Add Android 11+ Device Over Wi-Fi:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 69);
            label2.Name = "label2";
            label2.Size = new Size(659, 75);
            label2.TabIndex = 1;
            label2.Text = "1. Connect your Android 11+ Phone and your PC to same network.\r\n2. Go to Developer options > Wireless debugging > Pair device with pairing code.\r\n3. Click on Find Devices.";
            // 
            // FindDeviceButton
            // 
            FindDeviceButton.Location = new Point(273, 158);
            FindDeviceButton.Name = "FindDeviceButton";
            FindDeviceButton.Size = new Size(132, 34);
            FindDeviceButton.TabIndex = 4;
            FindDeviceButton.Text = "Find Devices";
            FindDeviceButton.UseVisualStyleBackColor = true;
            FindDeviceButton.Click += FindDeviceButton_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Italic, GraphicsUnit.Point);
            label3.ForeColor = Color.Red;
            label3.Location = new Point(12, 424);
            label3.Name = "label3";
            label3.Size = new Size(599, 25);
            label3.TabIndex = 6;
            label3.Text = "Note : Device connected over Wi-Fi may be slower when compared to USB.";
            // 
            // listView1
            // 
            listView1.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2 });
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.Location = new Point(87, 210);
            listView1.Name = "listView1";
            listView1.Size = new Size(501, 153);
            listView1.TabIndex = 7;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Details;
            listView1.SelectedIndexChanged += listView1_SelectedIndexChanged;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Device UDID";
            columnHeader1.Width = 250;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "IP Address";
            columnHeader2.Width = 250;
            // 
            // PairButton
            // 
            PairButton.Enabled = false;
            PairButton.Location = new Point(273, 369);
            PairButton.Name = "PairButton";
            PairButton.Size = new Size(132, 34);
            PairButton.TabIndex = 8;
            PairButton.Text = "Pair";
            PairButton.UseVisualStyleBackColor = true;
            PairButton.Click += PairButton_Click;
            // 
            // AndroidWireless
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLightLight;
            ClientSize = new Size(690, 458);
            Controls.Add(PairButton);
            Controls.Add(listView1);
            Controls.Add(label3);
            Controls.Add(FindDeviceButton);
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
        private Button FindDeviceButton;
        private Label label3;
        private ListView listView1;
        private Button PairButton;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
    }
}