namespace Appium_Wizard
{
    partial class Server_Settings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Server_Settings));
            ServerArgsRichTextBox = new RichTextBox();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            DefaultCapabilitiesRichTextBox = new RichTextBox();
            label6 = new Label();
            label7 = new Label();
            ServerArgsLink = new LinkLabel();
            FinalCommandRichTextBox = new RichTextBox();
            cancelButton = new Button();
            applyButton = new Button();
            label8 = new Label();
            resetButton = new Button();
            label5 = new Label();
            contextMenuStrip1 = new ContextMenuStrip(components);
            sessionCapabilityToolStripMenuItem = new ToolStripMenuItem();
            xCUITestCapabilityToolStripMenuItem = new ToolStripMenuItem();
            uIAutomator2CapabilityToolStripMenuItem = new ToolStripMenuItem();
            DefaultCapsLinkLabel = new LinkLabel();
            groupBox1 = new GroupBox();
            debugRadioButton = new RadioButton();
            infoRadioButton = new RadioButton();
            errorRadioButton = new RadioButton();
            contextMenuStrip1.SuspendLayout();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // ServerArgsRichTextBox
            // 
            ServerArgsRichTextBox.Location = new Point(10, 102);
            ServerArgsRichTextBox.Margin = new Padding(2);
            ServerArgsRichTextBox.Name = "ServerArgsRichTextBox";
            ServerArgsRichTextBox.Size = new Size(719, 45);
            ServerArgsRichTextBox.TabIndex = 1;
            ServerArgsRichTextBox.Text = "";
            ServerArgsRichTextBox.TextChanged += ServerArgs_TextChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label2.Location = new Point(10, 240);
            label2.Margin = new Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new Size(97, 15);
            label2.TabIndex = 2;
            label2.Text = "Final Command :";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label3.Location = new Point(8, 378);
            label3.Margin = new Padding(2, 0, 2, 0);
            label3.Name = "label3";
            label3.Size = new Size(260, 15);
            label3.TabIndex = 4;
            label3.Text = "Mandatory args (will be added automatically): ";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(8, 397);
            label4.Margin = new Padding(2, 0, 2, 0);
            label4.Name = "label4";
            label4.Size = new Size(401, 30);
            label4.TabIndex = 5;
            label4.Text = "1. Appium Port number --> To start the server in the given port number.\r\n2. webDriverAgentUrl as default capability --> For iOS automation support.";
            // 
            // DefaultCapabilitiesRichTextBox
            // 
            DefaultCapabilitiesRichTextBox.Location = new Point(10, 166);
            DefaultCapabilitiesRichTextBox.Margin = new Padding(2);
            DefaultCapabilitiesRichTextBox.Name = "DefaultCapabilitiesRichTextBox";
            DefaultCapabilitiesRichTextBox.Size = new Size(719, 67);
            DefaultCapabilitiesRichTextBox.TabIndex = 6;
            DefaultCapabilitiesRichTextBox.Text = "";
            DefaultCapabilitiesRichTextBox.TextChanged += DefaultCapabilities_TextChanged;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(10, 85);
            label6.Margin = new Padding(2, 0, 2, 0);
            label6.Name = "label6";
            label6.Size = new Size(69, 15);
            label6.TabIndex = 8;
            label6.Text = "Server Args:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(8, 149);
            label7.Margin = new Padding(2, 0, 2, 0);
            label7.Name = "label7";
            label7.Size = new Size(109, 15);
            label7.TabIndex = 9;
            label7.Text = "Default Capabilites:";
            // 
            // ServerArgsLink
            // 
            ServerArgsLink.AutoSize = true;
            ServerArgsLink.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            ServerArgsLink.Location = new Point(90, 85);
            ServerArgsLink.Margin = new Padding(2, 0, 2, 0);
            ServerArgsLink.Name = "ServerArgsLink";
            ServerArgsLink.Size = new Size(60, 15);
            ServerArgsLink.TabIndex = 10;
            ServerArgsLink.TabStop = true;
            ServerArgsLink.Text = "Refer here";
            ServerArgsLink.LinkClicked += ServerArgsLink_LinkClicked;
            // 
            // FinalCommandRichTextBox
            // 
            FinalCommandRichTextBox.DetectUrls = false;
            FinalCommandRichTextBox.Location = new Point(10, 257);
            FinalCommandRichTextBox.Margin = new Padding(2);
            FinalCommandRichTextBox.Name = "FinalCommandRichTextBox";
            FinalCommandRichTextBox.ReadOnly = true;
            FinalCommandRichTextBox.Size = new Size(717, 75);
            FinalCommandRichTextBox.TabIndex = 11;
            FinalCommandRichTextBox.Text = "";
            // 
            // cancelButton
            // 
            cancelButton.AutoSize = true;
            cancelButton.Location = new Point(292, 341);
            cancelButton.Margin = new Padding(2);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(78, 25);
            cancelButton.TabIndex = 12;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += cancelButton_Click;
            // 
            // applyButton
            // 
            applyButton.AutoSize = true;
            applyButton.Location = new Point(396, 341);
            applyButton.Margin = new Padding(2);
            applyButton.Name = "applyButton";
            applyButton.Size = new Size(95, 25);
            applyButton.TabIndex = 13;
            applyButton.Text = "Apply && Close";
            applyButton.UseVisualStyleBackColor = true;
            applyButton.Click += applyButton_Click;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.ForeColor = Color.Chocolate;
            label8.Location = new Point(195, 149);
            label8.Margin = new Padding(2, 0, 2, 0);
            label8.Name = "label8";
            label8.Size = new Size(509, 15);
            label8.TabIndex = 15;
            label8.Text = "Enter valid json string below. E.g. {\"appium:newCommandTimeout\": 0, \"appium:noReset\": true}";
            // 
            // resetButton
            // 
            resetButton.AutoSize = true;
            resetButton.Location = new Point(162, 342);
            resetButton.Margin = new Padding(2);
            resetButton.Name = "resetButton";
            resetButton.Size = new Size(103, 25);
            resetButton.TabIndex = 16;
            resetButton.Text = "Reset to Default";
            resetButton.UseVisualStyleBackColor = true;
            resetButton.Click += resetButton_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label5.ForeColor = Color.IndianRed;
            label5.Location = new Point(8, 5);
            label5.Margin = new Padding(2, 0, 2, 0);
            label5.Name = "label5";
            label5.Size = new Size(658, 30);
            label5.TabIndex = 7;
            label5.Text = "NOTE : THERE IS NO VALIDATION DONE ON THE GIVEN COMMAND, SO MAKE SURE YOU ARE ENTERING VALID ARGS.\r\nWHILE STARTING THE SERVER, IF IT FAILS THEN FIX THE COMMAND.";
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.ImageScalingSize = new Size(24, 24);
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { sessionCapabilityToolStripMenuItem, xCUITestCapabilityToolStripMenuItem, uIAutomator2CapabilityToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(214, 94);
            // 
            // sessionCapabilityToolStripMenuItem
            // 
            sessionCapabilityToolStripMenuItem.Image = Properties.Resources.link;
            sessionCapabilityToolStripMenuItem.Name = "sessionCapabilityToolStripMenuItem";
            sessionCapabilityToolStripMenuItem.Size = new Size(213, 30);
            sessionCapabilityToolStripMenuItem.Text = "Session Capability";
            sessionCapabilityToolStripMenuItem.Click += sessionCapabilityToolStripMenuItem_Click;
            // 
            // xCUITestCapabilityToolStripMenuItem
            // 
            xCUITestCapabilityToolStripMenuItem.Image = Properties.Resources.link;
            xCUITestCapabilityToolStripMenuItem.Name = "xCUITestCapabilityToolStripMenuItem";
            xCUITestCapabilityToolStripMenuItem.Size = new Size(213, 30);
            xCUITestCapabilityToolStripMenuItem.Text = "XCUITest Capability";
            xCUITestCapabilityToolStripMenuItem.Click += xCUITestCapabilityToolStripMenuItem_Click;
            // 
            // uIAutomator2CapabilityToolStripMenuItem
            // 
            uIAutomator2CapabilityToolStripMenuItem.Image = Properties.Resources.link;
            uIAutomator2CapabilityToolStripMenuItem.Name = "uIAutomator2CapabilityToolStripMenuItem";
            uIAutomator2CapabilityToolStripMenuItem.Size = new Size(213, 30);
            uIAutomator2CapabilityToolStripMenuItem.Text = "UIAutomator2 Capability";
            uIAutomator2CapabilityToolStripMenuItem.Click += uIAutomator2CapabilityToolStripMenuItem_Click;
            // 
            // DefaultCapsLinkLabel
            // 
            DefaultCapsLinkLabel.AutoSize = true;
            DefaultCapsLinkLabel.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            DefaultCapsLinkLabel.Location = new Point(127, 149);
            DefaultCapsLinkLabel.Margin = new Padding(2, 0, 2, 0);
            DefaultCapsLinkLabel.Name = "DefaultCapsLinkLabel";
            DefaultCapsLinkLabel.Size = new Size(60, 15);
            DefaultCapsLinkLabel.TabIndex = 18;
            DefaultCapsLinkLabel.TabStop = true;
            DefaultCapsLinkLabel.Text = "Refer here";
            DefaultCapsLinkLabel.LinkClicked += DefaultCapsLinkLabel_LinkClicked;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(debugRadioButton);
            groupBox1.Controls.Add(infoRadioButton);
            groupBox1.Controls.Add(errorRadioButton);
            groupBox1.Location = new Point(10, 44);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(328, 38);
            groupBox1.TabIndex = 19;
            groupBox1.TabStop = false;
            groupBox1.Text = "Log Level";
            // 
            // debugRadioButton
            // 
            debugRadioButton.AutoSize = true;
            debugRadioButton.Location = new Point(259, 13);
            debugRadioButton.Name = "debugRadioButton";
            debugRadioButton.Size = new Size(59, 19);
            debugRadioButton.TabIndex = 22;
            debugRadioButton.TabStop = true;
            debugRadioButton.Text = "debug";
            debugRadioButton.UseVisualStyleBackColor = true;
            debugRadioButton.CheckedChanged += debugRadioButton_CheckedChanged;
            // 
            // infoRadioButton
            // 
            infoRadioButton.AutoSize = true;
            infoRadioButton.Location = new Point(117, 13);
            infoRadioButton.Name = "infoRadioButton";
            infoRadioButton.Size = new Size(46, 19);
            infoRadioButton.TabIndex = 20;
            infoRadioButton.TabStop = true;
            infoRadioButton.Text = "info";
            infoRadioButton.UseVisualStyleBackColor = true;
            infoRadioButton.CheckedChanged += infoRadioButton_CheckedChanged;
            // 
            // errorRadioButton
            // 
            errorRadioButton.AutoSize = true;
            errorRadioButton.Location = new Point(185, 13);
            errorRadioButton.Name = "errorRadioButton";
            errorRadioButton.Size = new Size(50, 19);
            errorRadioButton.TabIndex = 21;
            errorRadioButton.TabStop = true;
            errorRadioButton.Text = "error";
            errorRadioButton.UseVisualStyleBackColor = true;
            errorRadioButton.CheckedChanged += errorRadioButton_CheckedChanged;
            // 
            // Server_Settings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLightLight;
            ClientSize = new Size(734, 432);
            Controls.Add(groupBox1);
            Controls.Add(DefaultCapsLinkLabel);
            Controls.Add(resetButton);
            Controls.Add(label8);
            Controls.Add(applyButton);
            Controls.Add(cancelButton);
            Controls.Add(FinalCommandRichTextBox);
            Controls.Add(ServerArgsLink);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(DefaultCapabilitiesRichTextBox);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(ServerArgsRichTextBox);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(2);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Server_Settings";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Server Settings";
            Load += Server_Settings_Load;
            contextMenuStrip1.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private RichTextBox ServerArgsRichTextBox;
        private Label label2;
        private Label label3;
        private Label label4;
        private RichTextBox DefaultCapabilitiesRichTextBox;
        private Label label6;
        private Label label7;
        private LinkLabel ServerArgsLink;
        private RichTextBox FinalCommandRichTextBox;
        private Button cancelButton;
        private Button applyButton;
        private Label label8;
        private Button resetButton;
        private Label label5;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem sessionCapabilityToolStripMenuItem;
        private ToolStripMenuItem xCUITestCapabilityToolStripMenuItem;
        private ToolStripMenuItem uIAutomator2CapabilityToolStripMenuItem;
        private LinkLabel DefaultCapsLinkLabel;
        private GroupBox groupBox1;
        private RadioButton debugRadioButton;
        private RadioButton infoRadioButton;
        private RadioButton errorRadioButton;
    }
}