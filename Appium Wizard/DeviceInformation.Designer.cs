namespace Appium_Wizard
{
    partial class DeviceInformation
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeviceInformation));
            DeviceInfolistView = new ListView();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            Cancel = new Button();
            Add = new Button();
            SuspendLayout();
            // 
            // DeviceInfolistView
            // 
            DeviceInfolistView.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2 });
            DeviceInfolistView.Location = new Point(0, 2);
            DeviceInfolistView.Name = "DeviceInfolistView";
            DeviceInfolistView.Size = new Size(584, 332);
            DeviceInfolistView.TabIndex = 0;
            DeviceInfolistView.UseCompatibleStateImageBehavior = false;
            DeviceInfolistView.View = View.Details;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "";
            columnHeader1.Width = 250;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "";
            columnHeader2.Width = 320;
            // 
            // Cancel
            // 
            Cancel.Location = new Point(144, 340);
            Cancel.Name = "Cancel";
            Cancel.Size = new Size(112, 34);
            Cancel.TabIndex = 1;
            Cancel.Text = "Cancel";
            Cancel.UseVisualStyleBackColor = true;
            Cancel.Click += Cancel_Click;
            // 
            // Add
            // 
            Add.Location = new Point(321, 340);
            Add.Name = "Add";
            Add.Size = new Size(112, 34);
            Add.TabIndex = 2;
            Add.Text = "Add";
            Add.UseVisualStyleBackColor = true;
            Add.Click += Add_Click;
            // 
            // DeviceInformation
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = SystemColors.ControlLightLight;
            ClientSize = new Size(573, 406);
            Controls.Add(Add);
            Controls.Add(Cancel);
            Controls.Add(DeviceInfolistView);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "DeviceInformation";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Device Information";
            Shown += DeviceInformation_Shown;
            ResumeLayout(false);
        }

        #endregion

        private ListView DeviceInfolistView;
        private Button Cancel;
        private Button Add;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
    }
}