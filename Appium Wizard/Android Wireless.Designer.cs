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
            ManualButton = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Location = new Point(8, 13);
            label1.Margin = new Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new Size(354, 15);
            label1.TabIndex = 0;
            label1.Text = "Follow the below steps to Add Android 11+ Device Over Wi-Fi:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(8, 36);
            label2.Margin = new Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new Size(434, 45);
            label2.TabIndex = 1;
            label2.Text = "1. Connect your Android 11+ Phone and your PC to same network.\r\n2. Go to Developer options > Wireless debugging > Pair device with pairing code.\r\n3. Click any one of the below options.";
            label2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // FindDeviceButton
            // 
            FindDeviceButton.AutoSize = true;
            FindDeviceButton.Location = new Point(112, 95);
            FindDeviceButton.Margin = new Padding(2);
            FindDeviceButton.Name = "FindDeviceButton";
            FindDeviceButton.Size = new Size(118, 25);
            FindDeviceButton.TabIndex = 4;
            FindDeviceButton.Text = "Find automatically";
            FindDeviceButton.UseVisualStyleBackColor = true;
            FindDeviceButton.Click += FindDeviceButton_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Italic, GraphicsUnit.Point);
            label3.ForeColor = Color.Red;
            label3.Location = new Point(8, 254);
            label3.Margin = new Padding(2, 0, 2, 0);
            label3.Name = "label3";
            label3.Size = new Size(393, 15);
            label3.TabIndex = 6;
            label3.Text = "Note : Device connected over Wi-Fi may be slower when compared to USB.";
            // 
            // listView1
            // 
            listView1.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2 });
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.Location = new Point(61, 126);
            listView1.Margin = new Padding(2);
            listView1.Name = "listView1";
            listView1.Size = new Size(352, 93);
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
            PairButton.AutoSize = true;
            PairButton.Enabled = false;
            PairButton.Location = new Point(191, 223);
            PairButton.Margin = new Padding(2);
            PairButton.Name = "PairButton";
            PairButton.Size = new Size(92, 25);
            PairButton.TabIndex = 8;
            PairButton.Text = "Pair";
            PairButton.UseVisualStyleBackColor = true;
            PairButton.Click += PairButton_Click;
            // 
            // ManualButton
            // 
            ManualButton.AutoSize = true;
            ManualButton.Location = new Point(246, 95);
            ManualButton.Margin = new Padding(2);
            ManualButton.Name = "ManualButton";
            ManualButton.Size = new Size(102, 25);
            ManualButton.TabIndex = 9;
            ManualButton.Text = "Pair Manually";
            ManualButton.UseVisualStyleBackColor = true;
            ManualButton.Click += ManualButton_Click;
            // 
            // AndroidWireless
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLightLight;
            ClientSize = new Size(489, 275);
            Controls.Add(ManualButton);
            Controls.Add(PairButton);
            Controls.Add(listView1);
            Controls.Add(label3);
            Controls.Add(FindDeviceButton);
            Controls.Add(label2);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(2);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AndroidWireless";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Android Wireless";
            Shown += AndroidWireless_Shown;
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
        private Button ManualButton;
    }
}