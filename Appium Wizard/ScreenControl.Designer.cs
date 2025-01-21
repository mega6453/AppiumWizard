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
            HomeToolStripButton = new ToolStripButton();
            BackToolStripButton = new ToolStripButton();
            ControlCenterToolStripButton = new ToolStripButton();
            SettingsToolStripButton = new ToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            ScreenshotToolStripButton = new ToolStripButton();
            MoreToolStripButton = new ToolStripDropDownButton();
            UnlockScreen = new ToolStripMenuItem();
            manageAppsToolStripMenuItem = new ToolStripMenuItem();
            inspectorToolStripMenuItem = new ToolStripMenuItem();
            RecordButton = new ToolStripButton();
            SetOrientationButton = new ToolStripButton();
            toolStrip2 = new ToolStrip();
            statusLabel = new ToolStripLabel();
            ((System.ComponentModel.ISupportInitialize)ScreenWebView).BeginInit();
            toolStrip1.SuspendLayout();
            toolStrip2.SuspendLayout();
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
            ScreenWebView.Size = new Size(398, 346);
            ScreenWebView.TabIndex = 0;
            ScreenWebView.ZoomFactor = 1D;
            ScreenWebView.KeyPress += SendKeys;
            ScreenWebView.MouseDown += WebView_MouseDown;
            ScreenWebView.MouseMove += GetMouseCoordinate;
            ScreenWebView.MouseUp += WebView_MouseUp;
            // 
            // toolStrip1
            // 
            toolStrip1.BackColor = SystemColors.ControlLightLight;
            toolStrip1.Dock = DockStyle.Bottom;
            toolStrip1.ImageScalingSize = new Size(24, 24);
            toolStrip1.Items.AddRange(new ToolStripItem[] { AlwaysOnTopToolStripButton, toolStripSeparator3, toolStripSeparator1, HomeToolStripButton, BackToolStripButton, ControlCenterToolStripButton, SettingsToolStripButton, toolStripSeparator2, ScreenshotToolStripButton, MoreToolStripButton, RecordButton, SetOrientationButton });
            toolStrip1.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            toolStrip1.Location = new Point(0, 346);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.RenderMode = ToolStripRenderMode.System;
            toolStrip1.Size = new Size(398, 31);
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
            // HomeToolStripButton
            // 
            HomeToolStripButton.Alignment = ToolStripItemAlignment.Right;
            HomeToolStripButton.BackColor = SystemColors.ControlLightLight;
            HomeToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            HomeToolStripButton.Image = Properties.Resources.home;
            HomeToolStripButton.ImageTransparentColor = Color.Magenta;
            HomeToolStripButton.Name = "HomeToolStripButton";
            HomeToolStripButton.Size = new Size(28, 28);
            HomeToolStripButton.Click += HomeButton_Click;
            // 
            // BackToolStripButton
            // 
            BackToolStripButton.Alignment = ToolStripItemAlignment.Right;
            BackToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            BackToolStripButton.Image = Properties.Resources.left_arrow;
            BackToolStripButton.ImageTransparentColor = Color.Magenta;
            BackToolStripButton.Name = "BackToolStripButton";
            BackToolStripButton.Size = new Size(28, 28);
            BackToolStripButton.Click += BackButton_Click;
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
            SettingsToolStripButton.Click += SettingsToolStripButton_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Alignment = ToolStripItemAlignment.Right;
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 31);
            // 
            // ScreenshotToolStripButton
            // 
            ScreenshotToolStripButton.Alignment = ToolStripItemAlignment.Right;
            ScreenshotToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ScreenshotToolStripButton.Image = Properties.Resources.screenshot;
            ScreenshotToolStripButton.ImageTransparentColor = Color.Magenta;
            ScreenshotToolStripButton.Name = "ScreenshotToolStripButton";
            ScreenshotToolStripButton.Size = new Size(28, 28);
            ScreenshotToolStripButton.Click += Screenshot_Click;
            // 
            // MoreToolStripButton
            // 
            MoreToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            MoreToolStripButton.DropDownItems.AddRange(new ToolStripItem[] { UnlockScreen, manageAppsToolStripMenuItem, inspectorToolStripMenuItem });
            MoreToolStripButton.Image = Properties.Resources.ellipsis;
            MoreToolStripButton.ImageTransparentColor = Color.Magenta;
            MoreToolStripButton.Name = "MoreToolStripButton";
            MoreToolStripButton.Size = new Size(37, 28);
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
            // inspectorToolStripMenuItem
            // 
            inspectorToolStripMenuItem.Image = Properties.Resources.inspector;
            inspectorToolStripMenuItem.Name = "inspectorToolStripMenuItem";
            inspectorToolStripMenuItem.Size = new Size(149, 22);
            inspectorToolStripMenuItem.Text = "Inspector";
            inspectorToolStripMenuItem.Click += inspectorToolStripMenuItem_Click;
            // 
            // RecordButton
            // 
            RecordButton.Alignment = ToolStripItemAlignment.Right;
            RecordButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            RecordButton.Image = Properties.Resources.record_button;
            RecordButton.ImageTransparentColor = Color.Magenta;
            RecordButton.Name = "RecordButton";
            RecordButton.Size = new Size(28, 28);
            RecordButton.Click += RecordButton_Click;
            // 
            // SetOrientationButton
            // 
            SetOrientationButton.Alignment = ToolStripItemAlignment.Right;
            SetOrientationButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            SetOrientationButton.Image = Properties.Resources.screen_rotate;
            SetOrientationButton.ImageTransparentColor = Color.Magenta;
            SetOrientationButton.Name = "SetOrientationButton";
            SetOrientationButton.Size = new Size(28, 28);
            SetOrientationButton.Click += SetOrientationButton_Click;
            // 
            // toolStrip2
            // 
            toolStrip2.Dock = DockStyle.Bottom;
            toolStrip2.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip2.ImageScalingSize = new Size(24, 24);
            toolStrip2.Items.AddRange(new ToolStripItem[] { statusLabel });
            toolStrip2.Location = new Point(0, 321);
            toolStrip2.Name = "toolStrip2";
            toolStrip2.RenderMode = ToolStripRenderMode.System;
            toolStrip2.Size = new Size(398, 25);
            toolStrip2.TabIndex = 2;
            toolStrip2.TabStop = true;
            toolStrip2.Text = "toolStrip2";
            // 
            // statusLabel
            // 
            statusLabel.BackgroundImageLayout = ImageLayout.None;
            statusLabel.DisplayStyle = ToolStripItemDisplayStyle.Text;
            statusLabel.Font = new Font("Arial Narrow", 9F);
            statusLabel.Name = "statusLabel";
            statusLabel.Overflow = ToolStripItemOverflow.Never;
            statusLabel.Size = new Size(0, 22);
            statusLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // ScreenControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(398, 377);
            Controls.Add(toolStrip2);
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
            Load += ScreenControl_Load;
            Shown += ScreenControl_Shown;
            ((System.ComponentModel.ISupportInitialize)ScreenWebView).EndInit();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            toolStrip2.ResumeLayout(false);
            toolStrip2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Microsoft.Web.WebView2.WinForms.WebView2 ScreenWebView;
        private ToolStrip toolStrip1;
        private ToolStripButton HomeToolStripButton;
        private ToolStripButton AlwaysOnTopToolStripButton;
        private ToolStripButton ControlCenterToolStripButton;
        private ToolStrip toolStrip2;
        private ToolStripLabel statusLabel;
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
        private ToolStripMenuItem inspectorToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripButton SetOrientationButton;
    }
}