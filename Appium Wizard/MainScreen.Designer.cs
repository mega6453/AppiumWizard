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
            contextMenuStrip1 = new ContextMenuStrip(components);
            androidToolStripMenuItem = new ToolStripMenuItem();
            iOSToolStripMenuItem = new ToolStripMenuItem();
            AddDevice = new Button();
            DeleteDevice = new Button();
            richTextBox1 = new RichTextBox();
            checkBox1 = new CheckBox();
            label1 = new Label();
            InstallButton = new Button();
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
            contextMenuStrip1.SuspendLayout();
            menuStrip1.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            tabPage3.SuspendLayout();
            tabPage4.SuspendLayout();
            tabPage5.SuspendLayout();
            contextMenuStrip2.SuspendLayout();
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
            listView1.Columns.AddRange(new ColumnHeader[] { columnHeader4, columnHeader2, columnHeader3, columnHeader1, columnHeader5 });
            listView1.FullRowSelect = true;
            listView1.Location = new Point(30, 122);
            listView1.Name = "listView1";
            listView1.Scrollable = false;
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
            columnHeader4.Width = 300;
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
            // contextMenuStrip1
            // 
            contextMenuStrip1.ImageScalingSize = new Size(24, 24);
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { androidToolStripMenuItem, iOSToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(158, 68);
            // 
            // androidToolStripMenuItem
            // 
            androidToolStripMenuItem.Image = Properties.Resources.android;
            androidToolStripMenuItem.Name = "androidToolStripMenuItem";
            androidToolStripMenuItem.Size = new Size(157, 32);
            androidToolStripMenuItem.Text = "Android";
            androidToolStripMenuItem.Click += androidToolStripMenuItem_Click;
            // 
            // iOSToolStripMenuItem
            // 
            iOSToolStripMenuItem.Image = Properties.Resources.apple;
            iOSToolStripMenuItem.Name = "iOSToolStripMenuItem";
            iOSToolStripMenuItem.Size = new Size(157, 32);
            iOSToolStripMenuItem.Text = "iOS";
            iOSToolStripMenuItem.Click += iOSToolStripMenuItem_Click;
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
            // InstallButton
            // 
            InstallButton.Enabled = false;
            InstallButton.Location = new Point(501, 70);
            InstallButton.Name = "InstallButton";
            InstallButton.Size = new Size(112, 34);
            InstallButton.TabIndex = 9;
            InstallButton.Text = "Install App";
            InstallButton.UseVisualStyleBackColor = true;
            InstallButton.Click += InstallButton_Click;
            // 
            // menuStrip1
            // 
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
            inspectorToolStripMenuItem1.Click += inspectorToolStripMenuItem1_Click;
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
            capabilitiesToolStripMenuItem.Size = new Size(270, 34);
            capabilitiesToolStripMenuItem.Text = "Capabilities";
            capabilitiesToolStripMenuItem.Click += capabilitiesToolStripMenuItem_Click;
            // 
            // xCUITestToolStripMenuItem
            // 
            xCUITestToolStripMenuItem.Image = Properties.Resources.link;
            xCUITestToolStripMenuItem.Name = "xCUITestToolStripMenuItem";
            xCUITestToolStripMenuItem.Size = new Size(270, 34);
            xCUITestToolStripMenuItem.Text = "XCUITest";
            xCUITestToolStripMenuItem.Click += xCUITestToolStripMenuItem_Click;
            // 
            // uIAutomatorToolStripMenuItem
            // 
            uIAutomatorToolStripMenuItem.Image = Properties.Resources.link;
            uIAutomatorToolStripMenuItem.Name = "uIAutomatorToolStripMenuItem";
            uIAutomatorToolStripMenuItem.Size = new Size(270, 34);
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
            // MainScreen
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1407, 631);
            Controls.Add(tabControl1);
            Controls.Add(menuStrip1);
            Controls.Add(InstallButton);
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
            Shown += afterFormShown;
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
        private Button InstallButton;
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
    }
}