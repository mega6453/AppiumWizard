namespace Appium_Wizard
{
    partial class iOSProfileManagement
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(iOSProfileManagement));
            button1 = new Button();
            listView1 = new ListView();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            columnHeader3 = new ColumnHeader();
            columnHeader4 = new ColumnHeader();
            columnHeader6 = new ColumnHeader();
            button2 = new Button();
            deleteProfileButton = new Button();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(12, 12);
            button1.Name = "button1";
            button1.Size = new Size(142, 34);
            button1.TabIndex = 0;
            button1.Text = "Import Profile";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // listView1
            // 
            listView1.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3, columnHeader4, columnHeader6 });
            listView1.FullRowSelect = true;
            listView1.Location = new Point(12, 52);
            listView1.Name = "listView1";
            listView1.Size = new Size(804, 331);
            listView1.TabIndex = 1;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Details;
            listView1.SelectedIndexChanged += listView1_SelectedIndexChanged;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Name";
            columnHeader1.Width = 300;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Expiration";
            columnHeader2.Width = 200;
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "AppId";
            columnHeader3.Width = 150;
            // 
            // columnHeader4
            // 
            columnHeader4.Text = "TeamId";
            columnHeader4.Width = 150;
            // 
            // columnHeader6
            // 
            columnHeader6.Width = 0;
            // 
            // button2
            // 
            button2.Location = new Point(1058, 404);
            button2.Name = "button2";
            button2.Size = new Size(112, 34);
            button2.TabIndex = 2;
            button2.Text = "Close";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // deleteProfileButton
            // 
            deleteProfileButton.Enabled = false;
            deleteProfileButton.Location = new Point(179, 12);
            deleteProfileButton.Name = "deleteProfileButton";
            deleteProfileButton.Size = new Size(139, 34);
            deleteProfileButton.TabIndex = 3;
            deleteProfileButton.Text = "Delete Profile";
            deleteProfileButton.UseVisualStyleBackColor = true;
            deleteProfileButton.Click += button3_Click;
            // 
            // iOSProfileManagement
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(819, 388);
            Controls.Add(deleteProfileButton);
            Controls.Add(button2);
            Controls.Add(listView1);
            Controls.Add(button1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "iOSProfileManagement";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "iOS Profile Management";
            Load += iOSProfileManagement_Load;
            ResumeLayout(false);
        }

        #endregion

        private Button button1;
        private ListView listView1;
        private Button button2;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
        private ColumnHeader columnHeader4;
        private Button deleteProfileButton;
        private ColumnHeader columnHeader6;
    }
}