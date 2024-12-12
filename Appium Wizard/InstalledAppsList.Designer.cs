namespace Appium_Wizard
{
    partial class InstalledAppsList
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstalledAppsList));
            listView1 = new ListView();
            columnHeader1 = new ColumnHeader();
            LaunchButton = new Button();
            UninstallButton = new Button();
            textBoxSearch = new TextBox();
            KillAppButton = new Button();
            DropDownButton = new Button();
            contextMenuStrip1 = new ContextMenuStrip(components);
            clearAppDataToolStripMenuItem = new ToolStripMenuItem();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // listView1
            // 
            listView1.Columns.AddRange(new ColumnHeader[] { columnHeader1 });
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.HeaderStyle = ColumnHeaderStyle.None;
            listView1.Location = new Point(0, 24);
            listView1.Margin = new Padding(2);
            listView1.Name = "listView1";
            listView1.Size = new Size(355, 201);
            listView1.TabIndex = 0;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Details;
            listView1.SelectedIndexChanged += listView1_SelectedIndexChanged;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Package Names";
            columnHeader1.Width = 500;
            // 
            // LaunchButton
            // 
            LaunchButton.AutoSize = true;
            LaunchButton.Enabled = false;
            LaunchButton.Location = new Point(19, 228);
            LaunchButton.Margin = new Padding(2);
            LaunchButton.Name = "LaunchButton";
            LaunchButton.Size = new Size(88, 25);
            LaunchButton.TabIndex = 1;
            LaunchButton.Text = "Launch App";
            LaunchButton.UseVisualStyleBackColor = true;
            LaunchButton.Click += LaunchButton_Click;
            // 
            // UninstallButton
            // 
            UninstallButton.AutoSize = true;
            UninstallButton.Enabled = false;
            UninstallButton.Location = new Point(222, 228);
            UninstallButton.Margin = new Padding(2);
            UninstallButton.Name = "UninstallButton";
            UninstallButton.Size = new Size(97, 25);
            UninstallButton.TabIndex = 2;
            UninstallButton.Text = "Uninstall App";
            UninstallButton.UseVisualStyleBackColor = true;
            UninstallButton.Click += UninstallButton_Click;
            // 
            // textBoxSearch
            // 
            textBoxSearch.Location = new Point(0, 5);
            textBoxSearch.Margin = new Padding(2);
            textBoxSearch.Name = "textBoxSearch";
            textBoxSearch.PlaceholderText = "Search Package Name";
            textBoxSearch.Size = new Size(355, 23);
            textBoxSearch.TabIndex = 3;
            textBoxSearch.TextChanged += textBox1_TextChanged;
            // 
            // KillAppButton
            // 
            KillAppButton.AutoSize = true;
            KillAppButton.Enabled = false;
            KillAppButton.Location = new Point(127, 228);
            KillAppButton.Margin = new Padding(2);
            KillAppButton.Name = "KillAppButton";
            KillAppButton.Size = new Size(78, 25);
            KillAppButton.TabIndex = 4;
            KillAppButton.Text = "Kill App";
            KillAppButton.UseVisualStyleBackColor = true;
            KillAppButton.Click += KillAppButton_Click;
            // 
            // DropDownButton
            // 
            DropDownButton.AutoSize = true;
            DropDownButton.Enabled = false;
            DropDownButton.Location = new Point(318, 228);
            DropDownButton.Name = "DropDownButton";
            DropDownButton.Size = new Size(27, 25);
            DropDownButton.TabIndex = 5;
            DropDownButton.Text = "▼";
            DropDownButton.UseVisualStyleBackColor = true;
            DropDownButton.Click += DropDownButton_Click;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { clearAppDataToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(181, 48);
            // 
            // clearAppDataToolStripMenuItem
            // 
            clearAppDataToolStripMenuItem.Image = Properties.Resources.clean_code;
            clearAppDataToolStripMenuItem.Name = "clearAppDataToolStripMenuItem";
            clearAppDataToolStripMenuItem.Size = new Size(180, 22);
            clearAppDataToolStripMenuItem.Text = "Clear App Data";
            clearAppDataToolStripMenuItem.Click += clearAppDataToolStripMenuItem_Click;
            // 
            // InstalledAppsList
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(356, 260);
            Controls.Add(DropDownButton);
            Controls.Add(KillAppButton);
            Controls.Add(textBoxSearch);
            Controls.Add(UninstallButton);
            Controls.Add(LaunchButton);
            Controls.Add(listView1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(2);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "InstalledAppsList";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Manage Apps";
            Load += InstalledAppsList_Load;
            Shown += InstalledAppsList_Shown;
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListView listView1;
        private ColumnHeader columnHeader1;
        private Button LaunchButton;
        private Button UninstallButton;
        private TextBox textBoxSearch;
        private Button KillAppButton;
        private Button DropDownButton;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem clearAppDataToolStripMenuItem;
    }
}