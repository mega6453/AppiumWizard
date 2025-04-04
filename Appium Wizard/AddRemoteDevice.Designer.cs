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
            findDevicesButton = new Button();
            remoteDevicesList = new ListView();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            columnHeader3 = new ColumnHeader();
            columnHeader4 = new ColumnHeader();
            columnHeader5 = new ColumnHeader();
            columnHeader6 = new ColumnHeader();
            columnHeader7 = new ColumnHeader();
            columnHeader8 = new ColumnHeader();
            ReserveButton = new Button();
            closeButton = new Button();
            label2 = new Label();
            SuspendLayout();
            // 
            // IPTextBox
            // 
            IPTextBox.Location = new Point(149, 14);
            IPTextBox.Name = "IPTextBox";
            IPTextBox.PlaceholderText = "IPAddress:PortNumber";
            IPTextBox.Size = new Size(200, 23);
            IPTextBox.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(13, 18);
            label1.Name = "label1";
            label1.Size = new Size(129, 15);
            label1.TabIndex = 1;
            label1.Text = "Enter Remote Address :";
            // 
            // findDevicesButton
            // 
            findDevicesButton.AutoSize = true;
            findDevicesButton.Location = new Point(356, 12);
            findDevicesButton.Name = "findDevicesButton";
            findDevicesButton.Size = new Size(107, 25);
            findDevicesButton.TabIndex = 4;
            findDevicesButton.Text = "Find Devices";
            findDevicesButton.UseVisualStyleBackColor = true;
            findDevicesButton.Click += findDevicesButton_Click;
            // 
            // remoteDevicesList
            // 
            remoteDevicesList.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3, columnHeader4, columnHeader5, columnHeader6, columnHeader7, columnHeader8 });
            remoteDevicesList.FullRowSelect = true;
            remoteDevicesList.GridLines = true;
            remoteDevicesList.Location = new Point(12, 74);
            remoteDevicesList.Name = "remoteDevicesList";
            remoteDevicesList.Size = new Size(695, 287);
            remoteDevicesList.TabIndex = 5;
            remoteDevicesList.UseCompatibleStateImageBehavior = false;
            remoteDevicesList.View = View.Details;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Name";
            columnHeader1.Width = 150;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "OS";
            columnHeader2.Width = 70;
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "Version";
            // 
            // columnHeader4
            // 
            columnHeader4.Text = "Model";
            columnHeader4.Width = 150;
            // 
            // columnHeader5
            // 
            columnHeader5.Text = "UDID";
            columnHeader5.Width = 200;
            // 
            // columnHeader6
            // 
            columnHeader6.Text = "IP Address";
            columnHeader6.Width = 0;
            // 
            // columnHeader7
            // 
            columnHeader7.Text = "Proxy Port";
            columnHeader7.Width = 0;
            // 
            // columnHeader8
            // 
            columnHeader8.Text = "Screen Port";
            columnHeader8.Width = 0;
            // 
            // ReserveButton
            // 
            ReserveButton.AutoSize = true;
            ReserveButton.Location = new Point(358, 376);
            ReserveButton.Name = "ReserveButton";
            ReserveButton.Size = new Size(75, 25);
            ReserveButton.TabIndex = 6;
            ReserveButton.Text = "Reserve";
            ReserveButton.UseVisualStyleBackColor = true;
            ReserveButton.Click += ReserveButton_Click;
            // 
            // closeButton
            // 
            closeButton.AutoSize = true;
            closeButton.Location = new Point(254, 376);
            closeButton.Name = "closeButton";
            closeButton.Size = new Size(75, 25);
            closeButton.TabIndex = 7;
            closeButton.Text = "Close";
            closeButton.UseVisualStyleBackColor = true;
            closeButton.Click += closeButton_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label2.Location = new Point(13, 56);
            label2.Name = "label2";
            label2.Size = new Size(73, 15);
            label2.TabIndex = 8;
            label2.Text = "Devices list:";
            // 
            // AddRemoteDevice
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLightLight;
            ClientSize = new Size(712, 413);
            Controls.Add(label2);
            Controls.Add(closeButton);
            Controls.Add(ReserveButton);
            Controls.Add(remoteDevicesList);
            Controls.Add(findDevicesButton);
            Controls.Add(label1);
            Controls.Add(IPTextBox);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AddRemoteDevice";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Add Remote Device";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox IPTextBox;
        private Label label1;
        private Button findDevicesButton;
        private ListView remoteDevicesList;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
        private ColumnHeader columnHeader4;
        private Button ReserveButton;
        private Button closeButton;
        private ColumnHeader columnHeader6;
        private ColumnHeader columnHeader7;
        private ColumnHeader columnHeader8;
        private Label label2;
        private ColumnHeader columnHeader5;
    }
}