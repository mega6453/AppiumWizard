namespace Appium_Wizard
{
    partial class Plugins
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Plugins));
            listView1 = new ListView();
            columnHeader1 = new ColumnHeader();
            columnHeader3 = new ColumnHeader();
            installButton = new Button();
            uninstallButton = new Button();
            label1 = new Label();
            label2 = new Label();
            textBox1 = new TextBox();
            otherPluginInstallButton = new Button();
            closeButton = new Button();
            pluginsLinkLabel = new LinkLabel();
            showProgressCheckBox1 = new CheckBox();
            showProgressCheckBox2 = new CheckBox();
            updateButton = new Button();
            SuspendLayout();
            // 
            // listView1
            // 
            listView1.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader3 });
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.Location = new Point(12, 21);
            listView1.Name = "listView1";
            listView1.Size = new Size(411, 139);
            listView1.TabIndex = 0;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Details;
            listView1.SelectedIndexChanged += listView1_SelectedIndexChanged;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Plugin";
            columnHeader1.Width = 250;
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "Version";
            columnHeader3.Width = 150;
            // 
            // installButton
            // 
            installButton.AutoSize = true;
            installButton.Enabled = false;
            installButton.Location = new Point(179, 162);
            installButton.Name = "installButton";
            installButton.Size = new Size(75, 25);
            installButton.TabIndex = 1;
            installButton.Text = "Install";
            installButton.UseVisualStyleBackColor = true;
            installButton.Click += installButton_Click;
            // 
            // uninstallButton
            // 
            uninstallButton.AutoSize = true;
            uninstallButton.Enabled = false;
            uninstallButton.Location = new Point(348, 162);
            uninstallButton.Name = "uninstallButton";
            uninstallButton.Size = new Size(75, 25);
            uninstallButton.TabIndex = 2;
            uninstallButton.Text = "Uninstall";
            uninstallButton.UseVisualStyleBackColor = true;
            uninstallButton.Click += uninstallButton_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label1.Location = new Point(12, 3);
            label1.Name = "label1";
            label1.Size = new Size(280, 15);
            label1.TabIndex = 3;
            label1.Text = "Plugins List (Official plugins && all installed plugins)";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label2.Location = new Point(12, 209);
            label2.Name = "label2";
            label2.Size = new Size(121, 15);
            label2.TabIndex = 4;
            label2.Text = "Install Other Plugins:";
            // 
            // textBox1
            // 
            textBox1.Anchor = AnchorStyles.None;
            textBox1.BorderStyle = BorderStyle.FixedSingle;
            textBox1.Location = new Point(17, 227);
            textBox1.Name = "textBox1";
            textBox1.PlaceholderText = "<installation key>";
            textBox1.Size = new Size(341, 23);
            textBox1.TabIndex = 5;
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // otherPluginInstallButton
            // 
            otherPluginInstallButton.AutoSize = true;
            otherPluginInstallButton.Enabled = false;
            otherPluginInstallButton.Location = new Point(179, 256);
            otherPluginInstallButton.Name = "otherPluginInstallButton";
            otherPluginInstallButton.Size = new Size(75, 25);
            otherPluginInstallButton.TabIndex = 6;
            otherPluginInstallButton.Text = "Install";
            otherPluginInstallButton.UseVisualStyleBackColor = true;
            otherPluginInstallButton.Click += otherPluginInstallButton_Click;
            // 
            // closeButton
            // 
            closeButton.AutoSize = true;
            closeButton.Location = new Point(263, 256);
            closeButton.Name = "closeButton";
            closeButton.Size = new Size(75, 25);
            closeButton.TabIndex = 7;
            closeButton.Text = "Close";
            closeButton.UseVisualStyleBackColor = true;
            closeButton.Click += closeButton_Click;
            // 
            // pluginsLinkLabel
            // 
            pluginsLinkLabel.AutoSize = true;
            pluginsLinkLabel.Location = new Point(376, 3);
            pluginsLinkLabel.Name = "pluginsLinkLabel";
            pluginsLinkLabel.Size = new Size(46, 15);
            pluginsLinkLabel.TabIndex = 8;
            pluginsLinkLabel.TabStop = true;
            pluginsLinkLabel.Text = "Plugins";
            pluginsLinkLabel.LinkClicked += pluginsLinkLabel_LinkClicked;
            // 
            // showProgressCheckBox1
            // 
            showProgressCheckBox1.AutoSize = true;
            showProgressCheckBox1.Enabled = false;
            showProgressCheckBox1.Location = new Point(12, 166);
            showProgressCheckBox1.Name = "showProgressCheckBox1";
            showProgressCheckBox1.Size = new Size(148, 19);
            showProgressCheckBox1.TabIndex = 9;
            showProgressCheckBox1.Text = "Show progress window";
            showProgressCheckBox1.UseVisualStyleBackColor = true;
            showProgressCheckBox1.CheckedChanged += showProgressCheckBox1_CheckedChanged;
            // 
            // showProgressCheckBox2
            // 
            showProgressCheckBox2.AutoSize = true;
            showProgressCheckBox2.Enabled = false;
            showProgressCheckBox2.Location = new Point(12, 260);
            showProgressCheckBox2.Name = "showProgressCheckBox2";
            showProgressCheckBox2.Size = new Size(148, 19);
            showProgressCheckBox2.TabIndex = 10;
            showProgressCheckBox2.Text = "Show progress window";
            showProgressCheckBox2.UseVisualStyleBackColor = true;
            showProgressCheckBox2.CheckedChanged += showProgressCheckBox2_CheckedChanged;
            // 
            // updateButton
            // 
            updateButton.AutoSize = true;
            updateButton.Enabled = false;
            updateButton.Location = new Point(263, 162);
            updateButton.Name = "updateButton";
            updateButton.Size = new Size(75, 25);
            updateButton.TabIndex = 11;
            updateButton.Text = "Update";
            updateButton.UseVisualStyleBackColor = true;
            updateButton.Click += updateButton_Click;
            // 
            // Plugins
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLightLight;
            ClientSize = new Size(435, 288);
            Controls.Add(updateButton);
            Controls.Add(showProgressCheckBox2);
            Controls.Add(showProgressCheckBox1);
            Controls.Add(pluginsLinkLabel);
            Controls.Add(closeButton);
            Controls.Add(otherPluginInstallButton);
            Controls.Add(textBox1);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(uninstallButton);
            Controls.Add(installButton);
            Controls.Add(listView1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Plugins";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Plugins Manager";
            Load += Plugins_Load;
            Shown += Plugins_Shown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListView listView1;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader3;
        private Button installButton;
        private Button uninstallButton;
        private Label label1;
        private Label label2;
        private TextBox textBox1;
        private Button otherPluginInstallButton;
        private Button closeButton;
        private LinkLabel pluginsLinkLabel;
        private CheckBox showProgressCheckBox1;
        private CheckBox showProgressCheckBox2;
        private Button updateButton;
    }
}