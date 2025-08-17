namespace Appium_Wizard
{
    partial class ScreenControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScreenControl));
            ScreenWebView = new Microsoft.Web.WebView2.WinForms.WebView2();
            toolStrip1 = new ToolStrip();
            AlwaysOnTopToolStripButton = new ToolStripButton();
            toolStripSeparator3 = new ToolStripSeparator();
            toolStripSeparator1 = new ToolStripSeparator();
            BackToolStripButton = new ToolStripButton();
            HomeToolStripButton = new ToolStripButton();
            recentAppsToolStripButton = new ToolStripButton();
            ControlCenterToolStripButton = new ToolStripButton();
            SettingsToolStripButton = new ToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            RecordButton = new ToolStripButton();
            ScreenshotToolStripButton = new ToolStripButton();
            MoreToolStripButton = new ToolStripDropDownButton();
            UnlockScreen = new ToolStripMenuItem();
            manageAppsToolStripMenuItem = new ToolStripMenuItem();
            deviceInfoToolStripMenuItem = new ToolStripMenuItem();
            copySerialNumberToolStripMenuItem = new ToolStripMenuItem();
            copyUDIDToolStripMenuItem = new ToolStripMenuItem();
            copyModelNumberToolStripMenuItem = new ToolStripMenuItem();
            copyOSVersionToolStripMenuItem = new ToolStripMenuItem();
            copyAllInfoToolStripMenuItem = new ToolStripMenuItem();
            infoToolStripMenuItem = new ToolStripMenuItem();
            copyProxyPortToolStripMenuItem1 = new ToolStripMenuItem();
            copyScreenPortToolStripMenuItem1 = new ToolStripMenuItem();
            copySessionIDToolStripMenuItem1 = new ToolStripMenuItem();
            copySessionURLToolStripMenuItem1 = new ToolStripMenuItem();
            toolStripSeparator4 = new ToolStripSeparator();
            objectSpyButton = new ToolStripButton();
            RecordAndStopRecordingSteps = new ToolStripSplitButton();
            playStepsToolStripMenuItem = new ToolStripMenuItem();
            readMeToolStripMenuItem = new ToolStripMenuItem();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel = new ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)ScreenWebView).BeginInit();
            toolStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // ScreenWebView
            // 
            ScreenWebView.AllowExternalDrop = true;
            ScreenWebView.CreationProperties = null;
            ScreenWebView.DefaultBackgroundColor = Color.White;
            ScreenWebView.Dock = DockStyle.Fill;
            ScreenWebView.Location = new Point(0, 0);
            ScreenWebView.Margin = new Padding(2);
            ScreenWebView.Name = "ScreenWebView";
            ScreenWebView.Size = new Size(452, 346);
            ScreenWebView.TabIndex = 0;
            ScreenWebView.ZoomFactor = 1D;
            ScreenWebView.KeyDown += ScreenWebView_KeyDown;
            ScreenWebView.KeyPress += SendKeys;
            ScreenWebView.MouseDown += WebView_MouseDown;
            ScreenWebView.MouseMove += GetMouseCoordinate;
            ScreenWebView.MouseUp += WebView_MouseUp;
            ScreenWebView.PreviewKeyDown += ScreenWebView_PreviewKeyDown;
            // 
            // toolStrip1
            // 
            toolStrip1.BackColor = SystemColors.ControlLightLight;
            toolStrip1.Dock = DockStyle.Bottom;
            toolStrip1.ImageScalingSize = new Size(24, 24);
            toolStrip1.Items.AddRange(new ToolStripItem[] { AlwaysOnTopToolStripButton, toolStripSeparator3, toolStripSeparator1, BackToolStripButton, HomeToolStripButton, recentAppsToolStripButton, ControlCenterToolStripButton, SettingsToolStripButton, toolStripSeparator2, RecordButton, ScreenshotToolStripButton, MoreToolStripButton, toolStripSeparator4, objectSpyButton, RecordAndStopRecordingSteps });
            toolStrip1.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            toolStrip1.Location = new Point(0, 346);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.RenderMode = ToolStripRenderMode.System;
            toolStrip1.Size = new Size(452, 31);
            toolStrip1.TabIndex = 1;
            toolStrip1.Text = "toolStrip1";
            // 
            // AlwaysOnTopToolStripButton
            // 
            AlwaysOnTopToolStripButton.Alignment = ToolStripItemAlignment.Right;
            AlwaysOnTopToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            AlwaysOnTopToolStripButton.Image = Properties.Resources.pin;
            AlwaysOnTopToolStripButton.ImageTransparentColor = Color.Magenta;
            AlwaysOnTopToolStripButton.Name = "AlwaysOnTopToolStripButton";
            AlwaysOnTopToolStripButton.Size = new Size(28, 28);
            AlwaysOnTopToolStripButton.ToolTipText = "Always on Top";
            AlwaysOnTopToolStripButton.Click += AlwaysOnTop_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Alignment = ToolStripItemAlignment.Right;
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(6, 31);
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Alignment = ToolStripItemAlignment.Right;
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 31);
            // 
            // BackToolStripButton
            // 
            BackToolStripButton.Alignment = ToolStripItemAlignment.Right;
            BackToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            BackToolStripButton.Image = Properties.Resources.left_arrow;
            BackToolStripButton.ImageTransparentColor = Color.Magenta;
            BackToolStripButton.Name = "BackToolStripButton";
            BackToolStripButton.Size = new Size(28, 28);
            BackToolStripButton.ToolTipText = "Back";
            BackToolStripButton.Click += BackButton_Click;
            // 
            // HomeToolStripButton
            // 
            HomeToolStripButton.Alignment = ToolStripItemAlignment.Right;
            HomeToolStripButton.BackColor = SystemColors.HighlightText;
            HomeToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            HomeToolStripButton.Image = Properties.Resources.home;
            HomeToolStripButton.ImageTransparentColor = Color.Magenta;
            HomeToolStripButton.Name = "HomeToolStripButton";
            HomeToolStripButton.Size = new Size(28, 28);
            HomeToolStripButton.ToolTipText = "Home";
            HomeToolStripButton.Click += HomeButton_Click;
            // 
            // recentAppsToolStripButton
            // 
            recentAppsToolStripButton.Alignment = ToolStripItemAlignment.Right;
            recentAppsToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            recentAppsToolStripButton.Image = Properties.Resources.recentApps;
            recentAppsToolStripButton.ImageTransparentColor = Color.Magenta;
            recentAppsToolStripButton.Name = "recentAppsToolStripButton";
            recentAppsToolStripButton.Size = new Size(28, 28);
            recentAppsToolStripButton.ToolTipText = "Recent Apps";
            recentAppsToolStripButton.Click += recentAppsToolStripButton_Click;
            // 
            // ControlCenterToolStripButton
            // 
            ControlCenterToolStripButton.Alignment = ToolStripItemAlignment.Right;
            ControlCenterToolStripButton.BackColor = SystemColors.ControlLightLight;
            ControlCenterToolStripButton.CheckOnClick = true;
            ControlCenterToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ControlCenterToolStripButton.Image = Properties.Resources.toggle_button;
            ControlCenterToolStripButton.ImageTransparentColor = Color.Magenta;
            ControlCenterToolStripButton.Name = "ControlCenterToolStripButton";
            ControlCenterToolStripButton.Size = new Size(28, 28);
            ControlCenterToolStripButton.ToolTipText = "Control Center/Notifications";
            ControlCenterToolStripButton.Click += controlCenter_Click;
            // 
            // SettingsToolStripButton
            // 
            SettingsToolStripButton.Alignment = ToolStripItemAlignment.Right;
            SettingsToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            SettingsToolStripButton.Image = Properties.Resources.settings;
            SettingsToolStripButton.ImageTransparentColor = Color.Magenta;
            SettingsToolStripButton.Name = "SettingsToolStripButton";
            SettingsToolStripButton.Size = new Size(28, 28);
            SettingsToolStripButton.ToolTipText = "Settings";
            SettingsToolStripButton.Click += SettingsToolStripButton_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Alignment = ToolStripItemAlignment.Right;
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 31);
            // 
            // RecordButton
            // 
            RecordButton.Alignment = ToolStripItemAlignment.Right;
            RecordButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            RecordButton.Image = Properties.Resources.record_button;
            RecordButton.ImageTransparentColor = Color.Magenta;
            RecordButton.Name = "RecordButton";
            RecordButton.Size = new Size(28, 28);
            RecordButton.ToolTipText = "Record Screen";
            RecordButton.Click += RecordButton_Click;
            // 
            // ScreenshotToolStripButton
            // 
            ScreenshotToolStripButton.Alignment = ToolStripItemAlignment.Right;
            ScreenshotToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ScreenshotToolStripButton.Image = Properties.Resources.screenshot;
            ScreenshotToolStripButton.ImageTransparentColor = Color.Magenta;
            ScreenshotToolStripButton.Name = "ScreenshotToolStripButton";
            ScreenshotToolStripButton.Size = new Size(28, 28);
            ScreenshotToolStripButton.ToolTipText = "Take Screenshot";
            ScreenshotToolStripButton.Click += Screenshot_Click;
            // 
            // MoreToolStripButton
            // 
            MoreToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            MoreToolStripButton.DropDownItems.AddRange(new ToolStripItem[] { UnlockScreen, manageAppsToolStripMenuItem, deviceInfoToolStripMenuItem, infoToolStripMenuItem });
            MoreToolStripButton.Image = Properties.Resources.ellipsis;
            MoreToolStripButton.ImageTransparentColor = Color.Magenta;
            MoreToolStripButton.Name = "MoreToolStripButton";
            MoreToolStripButton.Size = new Size(37, 28);
            MoreToolStripButton.ToolTipText = "More...";
            // 
            // UnlockScreen
            // 
            UnlockScreen.Image = Properties.Resources.Unlock;
            UnlockScreen.Name = "UnlockScreen";
            UnlockScreen.Size = new Size(149, 22);
            UnlockScreen.Text = "Unlock Screen";
            UnlockScreen.Click += UnlockScreen_Click;
            // 
            // manageAppsToolStripMenuItem
            // 
            manageAppsToolStripMenuItem.Image = Properties.Resources.application;
            manageAppsToolStripMenuItem.Name = "manageAppsToolStripMenuItem";
            manageAppsToolStripMenuItem.Size = new Size(149, 22);
            manageAppsToolStripMenuItem.Text = "Manage Apps";
            manageAppsToolStripMenuItem.Click += manageAppsToolStripMenuItem_Click;
            // 
            // deviceInfoToolStripMenuItem
            // 
            deviceInfoToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { copySerialNumberToolStripMenuItem, copyUDIDToolStripMenuItem, copyModelNumberToolStripMenuItem, copyOSVersionToolStripMenuItem, copyAllInfoToolStripMenuItem });
            deviceInfoToolStripMenuItem.Image = Properties.Resources.information;
            deviceInfoToolStripMenuItem.Name = "deviceInfoToolStripMenuItem";
            deviceInfoToolStripMenuItem.Size = new Size(149, 22);
            deviceInfoToolStripMenuItem.Text = "Device Info";
            // 
            // copySerialNumberToolStripMenuItem
            // 
            copySerialNumberToolStripMenuItem.Name = "copySerialNumberToolStripMenuItem";
            copySerialNumberToolStripMenuItem.Size = new Size(186, 22);
            copySerialNumberToolStripMenuItem.Text = "Copy Serial Number";
            copySerialNumberToolStripMenuItem.Click += copySerialNumberToolStripMenuItem_Click;
            // 
            // copyUDIDToolStripMenuItem
            // 
            copyUDIDToolStripMenuItem.Name = "copyUDIDToolStripMenuItem";
            copyUDIDToolStripMenuItem.Size = new Size(186, 22);
            copyUDIDToolStripMenuItem.Text = "Copy UDID";
            copyUDIDToolStripMenuItem.Click += copyUDIDToolStripMenuItem_Click;
            // 
            // copyModelNumberToolStripMenuItem
            // 
            copyModelNumberToolStripMenuItem.Name = "copyModelNumberToolStripMenuItem";
            copyModelNumberToolStripMenuItem.Size = new Size(186, 22);
            copyModelNumberToolStripMenuItem.Text = "Copy Model Number";
            copyModelNumberToolStripMenuItem.Click += copyModelNumberToolStripMenuItem_Click;
            // 
            // copyOSVersionToolStripMenuItem
            // 
            copyOSVersionToolStripMenuItem.Name = "copyOSVersionToolStripMenuItem";
            copyOSVersionToolStripMenuItem.Size = new Size(186, 22);
            copyOSVersionToolStripMenuItem.Text = "Copy OS Version";
            copyOSVersionToolStripMenuItem.Click += copyOSVersionToolStripMenuItem_Click;
            // 
            // copyAllInfoToolStripMenuItem
            // 
            copyAllInfoToolStripMenuItem.Name = "copyAllInfoToolStripMenuItem";
            copyAllInfoToolStripMenuItem.Size = new Size(186, 22);
            copyAllInfoToolStripMenuItem.Text = "Copy All Info";
            copyAllInfoToolStripMenuItem.Click += copyAllInfoToolStripMenuItem_Click;
            // 
            // infoToolStripMenuItem
            // 
            infoToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { copyProxyPortToolStripMenuItem1, copyScreenPortToolStripMenuItem1, copySessionIDToolStripMenuItem1, copySessionURLToolStripMenuItem1 });
            infoToolStripMenuItem.Image = Properties.Resources.information;
            infoToolStripMenuItem.Name = "infoToolStripMenuItem";
            infoToolStripMenuItem.Size = new Size(149, 22);
            infoToolStripMenuItem.Text = "Other Info";
            // 
            // copyProxyPortToolStripMenuItem1
            // 
            copyProxyPortToolStripMenuItem1.Name = "copyProxyPortToolStripMenuItem1";
            copyProxyPortToolStripMenuItem1.Size = new Size(168, 22);
            copyProxyPortToolStripMenuItem1.Text = "Copy Proxy Port";
            copyProxyPortToolStripMenuItem1.Click += copyProxyPortToolStripMenuItem_Click;
            // 
            // copyScreenPortToolStripMenuItem1
            // 
            copyScreenPortToolStripMenuItem1.Name = "copyScreenPortToolStripMenuItem1";
            copyScreenPortToolStripMenuItem1.Size = new Size(168, 22);
            copyScreenPortToolStripMenuItem1.Text = "Copy Screen Port";
            copyScreenPortToolStripMenuItem1.Click += copyScreenPortToolStripMenuItem_Click;
            // 
            // copySessionIDToolStripMenuItem1
            // 
            copySessionIDToolStripMenuItem1.Name = "copySessionIDToolStripMenuItem1";
            copySessionIDToolStripMenuItem1.Size = new Size(168, 22);
            copySessionIDToolStripMenuItem1.Text = "Copy Session ID";
            copySessionIDToolStripMenuItem1.Click += copySessionIDToolStripMenuItem_Click;
            // 
            // copySessionURLToolStripMenuItem1
            // 
            copySessionURLToolStripMenuItem1.Name = "copySessionURLToolStripMenuItem1";
            copySessionURLToolStripMenuItem1.Size = new Size(168, 22);
            copySessionURLToolStripMenuItem1.Text = "Copy Session URL";
            copySessionURLToolStripMenuItem1.Click += copySessionURLToolStripMenuItem_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Alignment = ToolStripItemAlignment.Right;
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(6, 31);
            // 
            // objectSpyButton
            // 
            objectSpyButton.Alignment = ToolStripItemAlignment.Right;
            objectSpyButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            objectSpyButton.Image = Properties.Resources.search;
            objectSpyButton.ImageTransparentColor = Color.Magenta;
            objectSpyButton.Name = "objectSpyButton";
            objectSpyButton.Size = new Size(28, 28);
            objectSpyButton.ToolTipText = "Object Spy";
            objectSpyButton.Click += objectSpyButton_Click;
            // 
            // RecordAndStopRecordingSteps
            // 
            RecordAndStopRecordingSteps.Alignment = ToolStripItemAlignment.Right;
            RecordAndStopRecordingSteps.DisplayStyle = ToolStripItemDisplayStyle.Image;
            RecordAndStopRecordingSteps.DropDownItems.AddRange(new ToolStripItem[] { playStepsToolStripMenuItem, readMeToolStripMenuItem });
            RecordAndStopRecordingSteps.Image = Properties.Resources.RecordSteps;
            RecordAndStopRecordingSteps.ImageTransparentColor = Color.Magenta;
            RecordAndStopRecordingSteps.Name = "RecordAndStopRecordingSteps";
            RecordAndStopRecordingSteps.Size = new Size(40, 28);
            RecordAndStopRecordingSteps.ToolTipText = "Record and Playback";
            RecordAndStopRecordingSteps.ButtonClick += RecordAndStopRecordingSteps_ButtonClick;
            // 
            // playStepsToolStripMenuItem
            // 
            playStepsToolStripMenuItem.Image = Properties.Resources.play;
            playStepsToolStripMenuItem.Name = "playStepsToolStripMenuItem";
            playStepsToolStripMenuItem.Size = new Size(127, 22);
            playStepsToolStripMenuItem.Text = "Play Steps";
            playStepsToolStripMenuItem.Click += playStepsToolStripMenuItem_Click;
            // 
            // readMeToolStripMenuItem
            // 
            readMeToolStripMenuItem.Image = Properties.Resources.readme;
            readMeToolStripMenuItem.Name = "readMeToolStripMenuItem";
            readMeToolStripMenuItem.Size = new Size(127, 22);
            readMeToolStripMenuItem.Text = "Read me";
            readMeToolStripMenuItem.Click += readMeToolStripMenuItem_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.BackColor = SystemColors.Control;
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel });
            statusStrip1.Location = new Point(0, 324);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.ShowItemToolTips = true;
            statusStrip1.Size = new Size(452, 22);
            statusStrip1.TabIndex = 3;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            toolStripStatusLabel.AutoToolTip = true;
            toolStripStatusLabel.BackColor = Color.Transparent;
            toolStripStatusLabel.BorderStyle = Border3DStyle.Adjust;
            toolStripStatusLabel.Font = new Font("Arial Narrow", 8F);
            toolStripStatusLabel.Name = "toolStripStatusLabel";
            toolStripStatusLabel.Size = new Size(437, 17);
            toolStripStatusLabel.Spring = true;
            toolStripStatusLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // ScreenControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(452, 377);
            Controls.Add(statusStrip1);
            Controls.Add(ScreenWebView);
            Controls.Add(toolStrip1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(2);
            MaximizeBox = false;
            Name = "ScreenControl";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "ScreenControl";
            FormClosing += ScreenControl_FormClosing;
            FormClosed += ScreenControl_FormClosed;
            Shown += ScreenControl_Shown;
            ((System.ComponentModel.ISupportInitialize)ScreenWebView).EndInit();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Microsoft.Web.WebView2.WinForms.WebView2 ScreenWebView;
        private ToolStrip toolStrip1;
        private ToolStripButton HomeToolStripButton;
        private ToolStripButton AlwaysOnTopToolStripButton;
        private ToolStripButton ControlCenterToolStripButton;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton BackToolStripButton;
        private ToolStripDropDownButton toolStripDropDownButton1;
        private ToolStripMenuItem unlockToolStripMenuItem;
        private ToolStripButton ScreenshotToolStripButton;
        private ToolStripButton SettingsToolStripButton;
        private ToolStripDropDownButton MoreToolStripButton;
        private ToolStripMenuItem UnlockScreen;
        private ToolStripButton RecordButton;
        private ToolStripMenuItem manageAppsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripButton objectSpyButton;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripButton recentAppsToolStripButton;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel;
        private ToolStripSplitButton RecordAndStopRecordingSteps;
        private ToolStripMenuItem playStepsToolStripMenuItem;
        private ToolStripMenuItem readMeToolStripMenuItem;
        private ToolStripMenuItem infoToolStripMenuItem;
        private ToolStripMenuItem copyProxyPortToolStripMenuItem1;
        private ToolStripMenuItem copyScreenPortToolStripMenuItem1;
        private ToolStripMenuItem copySessionIDToolStripMenuItem1;
        private ToolStripMenuItem copySessionURLToolStripMenuItem1;
        private ToolStripMenuItem deviceInfoToolStripMenuItem;
        private ToolStripMenuItem copySerialNumberToolStripMenuItem;
        private ToolStripMenuItem copyModelNumberToolStripMenuItem;
        private ToolStripMenuItem copyOSVersionToolStripMenuItem;
        private ToolStripMenuItem copyAllInfoToolStripMenuItem;
        private ToolStripMenuItem copyUDIDToolStripMenuItem;
    }
}