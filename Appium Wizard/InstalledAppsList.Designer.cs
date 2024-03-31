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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstalledAppsList));
            listView1 = new ListView();
            columnHeader1 = new ColumnHeader();
            LaunchButton = new Button();
            UninstallButton = new Button();
            textBoxSearch = new TextBox();
            SuspendLayout();
            // 
            // listView1
            // 
            listView1.Columns.AddRange(new ColumnHeader[] { columnHeader1 });
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.HeaderStyle = ColumnHeaderStyle.None;
            listView1.Location = new Point(0, 40);
            listView1.Name = "listView1";
            listView1.Size = new Size(506, 332);
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
            LaunchButton.Enabled = false;
            LaunchButton.Location = new Point(107, 378);
            LaunchButton.Name = "LaunchButton";
            LaunchButton.Size = new Size(126, 34);
            LaunchButton.TabIndex = 1;
            LaunchButton.Text = "Launch App";
            LaunchButton.UseVisualStyleBackColor = true;
            LaunchButton.Click += LaunchButton_Click;
            // 
            // UninstallButton
            // 
            UninstallButton.Enabled = false;
            UninstallButton.Location = new Point(273, 378);
            UninstallButton.Name = "UninstallButton";
            UninstallButton.Size = new Size(138, 34);
            UninstallButton.TabIndex = 2;
            UninstallButton.Text = "Uninstall App";
            UninstallButton.UseVisualStyleBackColor = true;
            UninstallButton.Click += UninstallButton_Click;
            // 
            // textBoxSearch
            // 
            textBoxSearch.Location = new Point(0, 8);
            textBoxSearch.Name = "textBoxSearch";
            textBoxSearch.PlaceholderText = "Search Package Name";
            textBoxSearch.Size = new Size(506, 31);
            textBoxSearch.TabIndex = 3;
            textBoxSearch.TextChanged += textBox1_TextChanged;
            // 
            // InstalledAppsList
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(508, 418);
            Controls.Add(textBoxSearch);
            Controls.Add(UninstallButton);
            Controls.Add(LaunchButton);
            Controls.Add(listView1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "InstalledAppsList";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Installed Apps List";
            Load += InstalledAppsList_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListView listView1;
        private ColumnHeader columnHeader1;
        private Button LaunchButton;
        private Button UninstallButton;
        private TextBox textBoxSearch;
    }
}