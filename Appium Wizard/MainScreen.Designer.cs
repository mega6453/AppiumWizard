using System.IO.Packaging;

namespace Appium_Wizard
{
    partial class MainScreen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainScreen));
            Open = new Button();
            listView1 = new ListView();
            columnHeader4 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            columnHeader3 = new ColumnHeader();
            columnHeader1 = new ColumnHeader();
            columnHeader5 = new ColumnHeader();
            columnHeader6 = new ColumnHeader();
            columnHeader7 = new ColumnHeader();
            contextMenuStrip1 = new ContextMenuStrip(components);
            iOSToolStripMenuItem = new ToolStripMenuItem();
            androidToolStripMenuItem = new ToolStripMenuItem();
            androidWiFiToolStripMenuItem = new ToolStripMenuItem();
            AddDevice = new Button();
            DeleteDevice = new Button();
            richTextBox1 = new RichTextBox();
            checkBox1 = new CheckBox();
            label1 = new Label();
            MoreButton = new Button();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            serverSetupToolStripMenuItem = new ToolStripMenuItem();
            fixInstallationToolStripMenuItem = new ToolStripMenuItem();
            toolsToolStripMenuItem = new ToolStripMenuItem();
            inspectorToolStripMenuItem1 = new ToolStripMenuItem();
            iOSProfileManagementToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            appiumDocsToolStripMenuItem = new ToolStripMenuItem();
            capabilitiesToolStripMenuItem = new ToolStripMenuItem();
            xCUITestToolStripMenuItem = new ToolStripMenuItem();
            uIAutomatorToolStripMenuItem = new ToolStripMenuItem();
            troubleshootToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            tabPage2 = new TabPage();
            richTextBox2 = new RichTextBox();
            tabPage3 = new TabPage();
            richTextBox3 = new RichTextBox();
            tabPage4 = new TabPage();
            richTextBox4 = new RichTextBox();
            tabPage5 = new TabPage();
            richTextBox5 = new RichTextBox();
            contextMenuStrip2 = new ContextMenuStrip(components);
            copyUDIDToolStripMenuItem = new ToolStripMenuItem();
            label2 = new Label();
            panel1 = new Panel();
            label3 = new Label();
            contextMenuStrip3 = new ContextMenuStrip(components);
            copyIPAddressToolStripMenuItem = new ToolStripMenuItem();
            contextMenuStrip4 = new ContextMenuStrip(components);
            installAppToolStripMenuItem = new ToolStripMenuItem();
            launchAppToolStripMenuItem = new ToolStripMenuItem();
            refreshStatusToolStripMenuItem = new ToolStripMenuItem();
            rebootDeviceToolStripMenuItem = new ToolStripMenuItem();
            contextMenuStrip1.SuspendLayout();
            menuStrip1.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            tabPage3.SuspendLayout();
            tabPage4.SuspendLayout();
            tabPage5.SuspendLayout();
            contextMenuStrip2.SuspendLayout();
            panel1.SuspendLayout();
            contextMenuStrip3.SuspendLayout();
            contextMenuStrip4.SuspendLayout();
            SuspendLayout();
            // 
            // Open
            // 
            Open.Enabled = false;
            Open.Location = new Point(339, 70);
            Open.Name = "Open";
            Open.Size = new Size(127, 34);
            Open.TabIndex = 0;
            Open.Text = "Open Device";
            Open.UseVisualStyleBackColor = true;
            Open.Click += Open_Click;
            // 
            // listView1
            // 
            listView1.Columns.AddRange(new ColumnHeader[] { columnHeader4, columnHeader2, columnHeader3, columnHeader1, columnHeader5, columnHeader6, columnHeader7 });
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.Location = new Point(30, 122);
            listView1.Name = "listView1";
            listView1.Size = new Size(583, 181);
            listView1.TabIndex = 2;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Details;
            listView1.SelectedIndexChanged += listView1_SelectedIndexChanged;
            listView1.MouseUp += listView1_MouseUp;
            // 
            // columnHeader4
            // 
            columnHeader4.Text = "Name";
            columnHeader4.Width = 230;
            // 
            // columnHeader2
            // 
            columnHeader2.DisplayIndex = 2;
            columnHeader2.Text = "Version";
            columnHeader2.Width = 80;
            // 
            // columnHeader3
            // 
            columnHeader3.DisplayIndex = 1;
            columnHeader3.Text = "OS";
            columnHeader3.Width = 100;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Status";
            columnHeader1.Width = 100;
            // 
            // columnHeader5
            // 
            columnHeader5.Text = "udid";
            columnHeader5.Width = 0;
            // 
            // columnHeader6
            // 
            columnHeader6.Text = "Connection";
            // 
            // columnHeader7
            // 
            columnHeader7.Text = "IPAddress";
            columnHeader7.Width = 0;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.ImageScalingSize = new Size(24, 24);
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { iOSToolStripMenuItem, androidToolStripMenuItem, androidWiFiToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(204, 100);
            // 
            // iOSToolStripMenuItem
            // 
            iOSToolStripMenuItem.Image = Properties.Resources.apple;
            iOSToolStripMenuItem.Name = "iOSToolStripMenuItem";
            iOSToolStripMenuItem.Size = new Size(203, 32);
            iOSToolStripMenuItem.Text = "iOS";
            iOSToolStripMenuItem.Click += iOSToolStripMenuItem_Click;
            // 
            // androidToolStripMenuItem
            // 
            androidToolStripMenuItem.Image = Properties.Resources.android;
            androidToolStripMenuItem.Name = "androidToolStripMenuItem";
            androidToolStripMenuItem.Size = new Size(203, 32);
            androidToolStripMenuItem.Text = "Android";
            androidToolStripMenuItem.Click += androidToolStripMenuItem_Click;
            // 
            // androidWiFiToolStripMenuItem
            // 
            androidWiFiToolStripMenuItem.Image = (Image)resources.GetObject("androidWiFiToolStripMenuItem.Image");
            androidWiFiToolStripMenuItem.Name = "androidWiFiToolStripMenuItem";
            androidWiFiToolStripMenuItem.Size = new Size(203, 32);
            androidWiFiToolStripMenuItem.Text = "Android Wi-Fi";
            androidWiFiToolStripMenuItem.Click += androidWiFiToolStripMenuItem_Click;
            // 
            // AddDevice
            // 
            AddDevice.Location = new Point(30, 70);
            AddDevice.Name = "AddDevice";
            AddDevice.Size = new Size(112, 34);
            AddDevice.TabIndex = 4;
            AddDevice.Text = "Add Device";
            AddDevice.UseVisualStyleBackColor = true;
            AddDevice.Click += button1_Click;
            // 
            // DeleteDevice
            // 
            DeleteDevice.Enabled = false;
            DeleteDevice.Location = new Point(173, 70);
            DeleteDevice.Name = "DeleteDevice";
            DeleteDevice.Size = new Size(135, 34);
            DeleteDevice.TabIndex = 5;
            DeleteDevice.Text = "Delete Device";
            DeleteDevice.UseVisualStyleBackColor = true;
            DeleteDevice.Click += DeleteDevice_Click;
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(0, 0);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(747, 455);
            richTextBox1.TabIndex = 6;
            richTextBox1.Text = "";
            richTextBox1.TextChanged += richTextBox1_TextChanged;
            // 
            // checkBox1
            // 
            checkBox1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            checkBox1.AutoSize = true;
            checkBox1.Checked = true;
            checkBox1.CheckState = CheckState.Checked;
            checkBox1.Location = new Point(1270, 42);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(125, 29);
            checkBox1.TabIndex = 7;
            checkBox1.Text = "Auto Scroll";
            checkBox1.UseVisualStyleBackColor = true;
            checkBox1.CheckedChanged += AutoScrollCheckbox_CheckedChanged;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top;
            label1.AutoSize = true;
            label1.ForeColor = Color.Tomato;
            label1.Location = new Point(994, 43);
            label1.Name = "label1";
            label1.Size = new Size(173, 25);
            label1.TabIndex = 8;
            label1.Text = "Appium Server Logs";
            // 
            // MoreButton
            // 
            MoreButton.Location = new Point(501, 70);
            MoreButton.Name = "MoreButton";
            MoreButton.Size = new Size(112, 34);
            MoreButton.TabIndex = 9;
            MoreButton.Text = "More...";
            MoreButton.UseVisualStyleBackColor = true;
            MoreButton.Click += MoreButton_Click;
            // 
            // menuStrip1
            // 
            menuStrip1.BackColor = Color.WhiteSmoke;
            menuStrip1.GripStyle = ToolStripGripStyle.Visible;
            menuStrip1.ImageScalingSize = new Size(24, 24);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, toolsToolStripMenuItem, helpToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1407, 33);
            menuStrip1.TabIndex = 10;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { serverSetupToolStripMenuItem, fixInstallationToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(77, 29);
            fileToolStripMenuItem.Text = "Server";
            // 
            // serverSetupToolStripMenuItem
            // 
            serverSetupToolStripMenuItem.Image = Properties.Resources.gear;
            serverSetupToolStripMenuItem.Name = "serverSetupToolStripMenuItem";
            serverSetupToolStripMenuItem.Size = new Size(233, 34);
            serverSetupToolStripMenuItem.Text = "Configuration";
            serverSetupToolStripMenuItem.Click += serverSetupToolStripMenuItem_Click;
            // 
            // fixInstallationToolStripMenuItem
            // 
            fixInstallationToolStripMenuItem.Image = Properties.Resources.troubleshooting;
            fixInstallationToolStripMenuItem.Name = "fixInstallationToolStripMenuItem";
            fixInstallationToolStripMenuItem.Size = new Size(233, 34);
            fixInstallationToolStripMenuItem.Text = "Troubleshooter";
            fixInstallationToolStripMenuItem.Click += fixInstallationToolStripMenuItem_Click;
            // 
            // toolsToolStripMenuItem
            // 
            toolsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { inspectorToolStripMenuItem1, iOSProfileManagementToolStripMenuItem });
            toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            toolsToolStripMenuItem.Size = new Size(69, 29);
            toolsToolStripMenuItem.Text = "Tools";
            // 
            // inspectorToolStripMenuItem1
            // 
            inspectorToolStripMenuItem1.Image = Properties.Resources.inspector;
            inspectorToolStripMenuItem1.Name = "inspectorToolStripMenuItem1";
            inspectorToolStripMenuItem1.Size = new Size(307, 34);
            inspectorToolStripMenuItem1.Text = "Inspector";
            inspectorToolStripMenuItem1.Click += inspectorToolStripMenuItem_Click;
            // 
            // iOSProfileManagementToolStripMenuItem
            // 
            iOSProfileManagementToolStripMenuItem.Image = Properties.Resources.management;
            iOSProfileManagementToolStripMenuItem.Name = "iOSProfileManagementToolStripMenuItem";
            iOSProfileManagementToolStripMenuItem.Size = new Size(307, 34);
            iOSProfileManagementToolStripMenuItem.Text = "iOS Profile Management";
            iOSProfileManagementToolStripMenuItem.Click += iOSProfileManagementToolStripMenuItem_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { appiumDocsToolStripMenuItem, troubleshootToolStripMenuItem, aboutToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(65, 29);
            helpToolStripMenuItem.Text = "Help";
            // 
            // appiumDocsToolStripMenuItem
            // 
            appiumDocsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { capabilitiesToolStripMenuItem, xCUITestToolStripMenuItem, uIAutomatorToolStripMenuItem });
            appiumDocsToolStripMenuItem.Image = Properties.Resources.doc;
            appiumDocsToolStripMenuItem.Name = "appiumDocsToolStripMenuItem";
            appiumDocsToolStripMenuItem.Size = new Size(294, 34);
            appiumDocsToolStripMenuItem.Text = "Appium Docs";
            // 
            // capabilitiesToolStripMenuItem
            // 
            capabilitiesToolStripMenuItem.Image = Properties.Resources.link;
            capabilitiesToolStripMenuItem.Name = "capabilitiesToolStripMenuItem";
            capabilitiesToolStripMenuItem.Size = new Size(218, 34);
            capabilitiesToolStripMenuItem.Text = "Capabilities";
            capabilitiesToolStripMenuItem.Click += capabilitiesToolStripMenuItem_Click;
            // 
            // xCUITestToolStripMenuItem
            // 
            xCUITestToolStripMenuItem.Image = Properties.Resources.link;
            xCUITestToolStripMenuItem.Name = "xCUITestToolStripMenuItem";
            xCUITestToolStripMenuItem.Size = new Size(218, 34);
            xCUITestToolStripMenuItem.Text = "XCUITest";
            xCUITestToolStripMenuItem.Click += xCUITestToolStripMenuItem_Click;
            // 
            // uIAutomatorToolStripMenuItem
            // 
            uIAutomatorToolStripMenuItem.Image = Properties.Resources.link;
            uIAutomatorToolStripMenuItem.Name = "uIAutomatorToolStripMenuItem";
            uIAutomatorToolStripMenuItem.Size = new Size(218, 34);
            uIAutomatorToolStripMenuItem.Text = "UIAutomator";
            uIAutomatorToolStripMenuItem.Click += uIAutomatorToolStripMenuItem_Click;
            // 
            // troubleshootToolStripMenuItem
            // 
            troubleshootToolStripMenuItem.Image = Properties.Resources.guide;
            troubleshootToolStripMenuItem.Name = "troubleshootToolStripMenuItem";
            troubleshootToolStripMenuItem.Size = new Size(294, 34);
            troubleshootToolStripMenuItem.Text = "Troubleshooting Guide";
            troubleshootToolStripMenuItem.Click += fAQToolStripMenuItem_Click;
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Image = Properties.Resources.information;
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(294, 34);
            aboutToolStripMenuItem.Text = "About";
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Controls.Add(tabPage4);
            tabControl1.Controls.Add(tabPage5);
            tabControl1.Location = new Point(640, 70);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(755, 493);
            tabControl1.TabIndex = 11;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(richTextBox1);
            tabPage1.Location = new Point(4, 34);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(747, 455);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "#1";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(richTextBox2);
            tabPage2.Location = new Point(4, 34);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(747, 455);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "#2";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // richTextBox2
            // 
            richTextBox2.Location = new Point(0, 0);
            richTextBox2.Name = "richTextBox2";
            richTextBox2.Size = new Size(747, 459);
            richTextBox2.TabIndex = 0;
            richTextBox2.Text = "";
            richTextBox2.TextChanged += richTextBox2_TextChanged;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(richTextBox3);
            tabPage3.Location = new Point(4, 34);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new Size(747, 455);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "#3";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // richTextBox3
            // 
            richTextBox3.Location = new Point(0, 0);
            richTextBox3.Name = "richTextBox3";
            richTextBox3.Size = new Size(747, 455);
            richTextBox3.TabIndex = 0;
            richTextBox3.Text = "";
            richTextBox3.TextChanged += richTextBox3_TextChanged;
            // 
            // tabPage4
            // 
            tabPage4.Controls.Add(richTextBox4);
            tabPage4.Location = new Point(4, 34);
            tabPage4.Name = "tabPage4";
            tabPage4.Padding = new Padding(3);
            tabPage4.Size = new Size(747, 455);
            tabPage4.TabIndex = 3;
            tabPage4.Text = "#4";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // richTextBox4
            // 
            richTextBox4.Location = new Point(0, 0);
            richTextBox4.Name = "richTextBox4";
            richTextBox4.Size = new Size(747, 455);
            richTextBox4.TabIndex = 0;
            richTextBox4.Text = "";
            richTextBox4.TextChanged += richTextBox4_TextChanged;
            // 
            // tabPage5
            // 
            tabPage5.Controls.Add(richTextBox5);
            tabPage5.Location = new Point(4, 34);
            tabPage5.Name = "tabPage5";
            tabPage5.Padding = new Padding(3);
            tabPage5.Size = new Size(747, 455);
            tabPage5.TabIndex = 4;
            tabPage5.Text = "#5";
            tabPage5.UseVisualStyleBackColor = true;
            // 
            // richTextBox5
            // 
            richTextBox5.Location = new Point(0, 0);
            richTextBox5.Name = "richTextBox5";
            richTextBox5.Size = new Size(747, 459);
            richTextBox5.TabIndex = 0;
            richTextBox5.Text = "";
            richTextBox5.TextChanged += richTextBox5_TextChanged;
            // 
            // contextMenuStrip2
            // 
            contextMenuStrip2.ImageScalingSize = new Size(24, 24);
            contextMenuStrip2.Items.AddRange(new ToolStripItem[] { copyUDIDToolStripMenuItem });
            contextMenuStrip2.Name = "contextMenuStrip2";
            contextMenuStrip2.Size = new Size(183, 36);
            // 
            // copyUDIDToolStripMenuItem
            // 
            copyUDIDToolStripMenuItem.Image = Properties.Resources.files;
            copyUDIDToolStripMenuItem.Name = "copyUDIDToolStripMenuItem";
            copyUDIDToolStripMenuItem.Size = new Size(182, 32);
            copyUDIDToolStripMenuItem.Text = "Copy UDID";
            copyUDIDToolStripMenuItem.Click += copyUDIDToolStripMenuItem_Click;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label2.AutoSize = true;
            label2.ForeColor = Color.IndianRed;
            label2.Location = new Point(3, 12);
            label2.Name = "label2";
            label2.Size = new Size(0, 25);
            label2.TabIndex = 12;
            // 
            // panel1
            // 
            panel1.Controls.Add(label3);
            panel1.Controls.Add(label2);
            panel1.Location = new Point(30, 366);
            panel1.Name = "panel1";
            panel1.Size = new Size(583, 150);
            panel1.TabIndex = 13;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(3, 102);
            label3.Name = "label3";
            label3.Size = new Size(0, 25);
            label3.TabIndex = 14;
            label3.MouseUp += label3_MouseUp;
            // 
            // contextMenuStrip3
            // 
            contextMenuStrip3.ImageScalingSize = new Size(24, 24);
            contextMenuStrip3.Items.AddRange(new ToolStripItem[] { copyIPAddressToolStripMenuItem });
            contextMenuStrip3.Name = "contextMenuStrip3";
            contextMenuStrip3.Size = new Size(225, 36);
            // 
            // copyIPAddressToolStripMenuItem
            // 
            copyIPAddressToolStripMenuItem.Image = Properties.Resources.files;
            copyIPAddressToolStripMenuItem.Name = "copyIPAddressToolStripMenuItem";
            copyIPAddressToolStripMenuItem.Size = new Size(224, 32);
            copyIPAddressToolStripMenuItem.Text = "Copy IP Address";
            copyIPAddressToolStripMenuItem.Click += copyIPAddressToolStripMenuItem_Click;
            // 
            // contextMenuStrip4
            // 
            contextMenuStrip4.ImageScalingSize = new Size(24, 24);
            contextMenuStrip4.Items.AddRange(new ToolStripItem[] { installAppToolStripMenuItem, launchAppToolStripMenuItem, refreshStatusToolStripMenuItem, rebootDeviceToolStripMenuItem });
            contextMenuStrip4.Name = "contextMenuStrip4";
            contextMenuStrip4.Size = new Size(307, 165);
            // 
            // installAppToolStripMenuItem
            // 
            installAppToolStripMenuItem.Image = Properties.Resources.Install;
            installAppToolStripMenuItem.Name = "installAppToolStripMenuItem";
            installAppToolStripMenuItem.Size = new Size(306, 32);
            installAppToolStripMenuItem.Text = "Install App";
            installAppToolStripMenuItem.Click += installAppToolStripMenuItem_Click;
            // 
            // launchAppToolStripMenuItem
            // 
            launchAppToolStripMenuItem.Image = Properties.Resources.Launch;
            launchAppToolStripMenuItem.Name = "launchAppToolStripMenuItem";
            launchAppToolStripMenuItem.Size = new Size(306, 32);
            launchAppToolStripMenuItem.Text = "Launch App | Uninstall App";
            launchAppToolStripMenuItem.Click += launchAppToolStripMenuItem_Click;
            // 
            // refreshStatusToolStripMenuItem
            // 
            refreshStatusToolStripMenuItem.Image = Properties.Resources.Refresh;
            refreshStatusToolStripMenuItem.Name = "refreshStatusToolStripMenuItem";
            refreshStatusToolStripMenuItem.Size = new Size(306, 32);
            refreshStatusToolStripMenuItem.Text = "Refresh Status";
            refreshStatusToolStripMenuItem.Click += refreshStatusToolStripMenuItem_Click;
            // 
            // rebootDeviceToolStripMenuItem
            // 
            rebootDeviceToolStripMenuItem.Image = Properties.Resources.Reboot;
            rebootDeviceToolStripMenuItem.Name = "rebootDeviceToolStripMenuItem";
            rebootDeviceToolStripMenuItem.Size = new Size(306, 32);
            rebootDeviceToolStripMenuItem.Text = "Reboot Device";
            rebootDeviceToolStripMenuItem.Click += rebootDeviceToolStripMenuItem_Click;
            // 
            // MainScreen
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLightLight;
            ClientSize = new Size(1407, 631);
            Controls.Add(panel1);
            Controls.Add(tabControl1);
            Controls.Add(menuStrip1);
            Controls.Add(MoreButton);
            Controls.Add(label1);
            Controls.Add(checkBox1);
            Controls.Add(DeleteDevice);
            Controls.Add(AddDevice);
            Controls.Add(listView1);
            Controls.Add(Open);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Name = "MainScreen";
            Text = "Appium Wizard";
            WindowState = FormWindowState.Maximized;
            FormClosing += onFormClosing;
            Load += onFormLoad;
            Shown += MainScreen_Shown;
            contextMenuStrip1.ResumeLayout(false);
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            tabPage3.ResumeLayout(false);
            tabPage4.ResumeLayout(false);
            tabPage5.ResumeLayout(false);
            contextMenuStrip2.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            contextMenuStrip3.ResumeLayout(false);
            contextMenuStrip4.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button Open;
        private ListView listView1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
        private ColumnHeader columnHeader4;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem androidToolStripMenuItem;
        private ToolStripMenuItem iOSToolStripMenuItem;
        private Button AddDevice;
        private Button DeleteDevice;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader5;
        private RichTextBox richTextBox1;
        private CheckBox checkBox1;
        private Label label1;
        private Button MoreButton;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem serverSetupToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private TabPage tabPage4;
        private TabPage tabPage5;
        private RichTextBox richTextBox2;
        private RichTextBox richTextBox3;
        private RichTextBox richTextBox4;
        private RichTextBox richTextBox5;
        private ToolStripMenuItem appiumDocsToolStripMenuItem;
        private ToolStripMenuItem capabilitiesToolStripMenuItem;
        private ToolStripMenuItem xCUITestToolStripMenuItem;
        private ToolStripMenuItem uIAutomatorToolStripMenuItem;
        private ToolStripMenuItem troubleshootToolStripMenuItem;
        private ToolStripMenuItem toolsToolStripMenuItem;
        private ToolStripMenuItem iOSProfileManagementToolStripMenuItem;
        private ToolStripMenuItem inspectorToolStripMenuItem1;
        private ContextMenuStrip contextMenuStrip2;
        private ToolStripMenuItem copyUDIDToolStripMenuItem;
        private ToolStripMenuItem fixInstallationToolStripMenuItem;
        private ToolStripMenuItem androidWiFiToolStripMenuItem;
        private ColumnHeader columnHeader6;
        private ColumnHeader columnHeader7;
        private Label label2;
        private Panel panel1;
        private Label label3;
        private ContextMenuStrip contextMenuStrip3;
        private ToolStripMenuItem copyIPAddressToolStripMenuItem;
        private ContextMenuStrip contextMenuStrip4;
        private ToolStripMenuItem installAppToolStripMenuItem;
        private ToolStripMenuItem rebootDeviceToolStripMenuItem;
        private ToolStripMenuItem refreshStatusToolStripMenuItem;
        private ToolStripMenuItem launchAppToolStripMenuItem;
    }
}