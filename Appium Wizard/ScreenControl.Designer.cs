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
            buttonAlwaysOnTop = new ToolStripButton();
            toolStripSeparator4 = new ToolStripSeparator();
            toolStripSeparator3 = new ToolStripSeparator();
            ObjectSpy = new ToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            toolStripSeparator1 = new ToolStripSeparator();
            toolStripButton1 = new ToolStripButton();
            BackButton = new ToolStripButton();
            controlCenter = new ToolStripButton();
            toolStripDropDownButton1 = new ToolStripDropDownButton();
            unlockToolStripMenuItem = new ToolStripMenuItem();
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
            ScreenWebView.Name = "ScreenWebView";
            ScreenWebView.Size = new Size(371, 572);
            ScreenWebView.TabIndex = 0;
            ScreenWebView.ZoomFactor = 1D;
            ScreenWebView.KeyPress += SendKeys;
            ScreenWebView.MouseDown += WebView_MouseDown;
            ScreenWebView.MouseMove += GetMouseCoordinate;
            ScreenWebView.MouseUp += WebView_MouseUp;
            // 
            // toolStrip1
            // 
            toolStrip1.Dock = DockStyle.Bottom;
            toolStrip1.ImageScalingSize = new Size(24, 24);
            toolStrip1.Items.AddRange(new ToolStripItem[] { buttonAlwaysOnTop, toolStripSeparator4, toolStripSeparator3, ObjectSpy, toolStripSeparator2, toolStripSeparator1, toolStripButton1, BackButton, controlCenter, toolStripDropDownButton1 });
            toolStrip1.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            toolStrip1.Location = new Point(0, 572);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.RenderMode = ToolStripRenderMode.System;
            toolStrip1.Size = new Size(371, 33);
            toolStrip1.TabIndex = 1;
            toolStrip1.Text = "toolStrip1";
            // 
            // buttonAlwaysOnTop
            // 
            buttonAlwaysOnTop.Alignment = ToolStripItemAlignment.Right;
            buttonAlwaysOnTop.DisplayStyle = ToolStripItemDisplayStyle.Image;
            buttonAlwaysOnTop.Image = Properties.Resources.pin;
            buttonAlwaysOnTop.ImageTransparentColor = Color.Magenta;
            buttonAlwaysOnTop.Name = "buttonAlwaysOnTop";
            buttonAlwaysOnTop.Size = new Size(34, 28);
            buttonAlwaysOnTop.Click += buttonAlwaysOnTop_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Alignment = ToolStripItemAlignment.Right;
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(6, 33);
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Alignment = ToolStripItemAlignment.Right;
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(6, 33);
            // 
            // ObjectSpy
            // 
            ObjectSpy.Alignment = ToolStripItemAlignment.Right;
            ObjectSpy.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ObjectSpy.Image = Properties.Resources.search;
            ObjectSpy.ImageTransparentColor = Color.Magenta;
            ObjectSpy.Name = "ObjectSpy";
            ObjectSpy.Size = new Size(34, 28);
            ObjectSpy.Click += ObjectSpy_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Alignment = ToolStripItemAlignment.Right;
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 33);
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Alignment = ToolStripItemAlignment.Right;
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 33);
            // 
            // toolStripButton1
            // 
            toolStripButton1.Alignment = ToolStripItemAlignment.Right;
            toolStripButton1.BackColor = SystemColors.HighlightText;
            toolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButton1.Image = Properties.Resources.home;
            toolStripButton1.ImageTransparentColor = Color.Magenta;
            toolStripButton1.Name = "toolStripButton1";
            toolStripButton1.Size = new Size(34, 28);
            toolStripButton1.Click += Home;
            // 
            // BackButton
            // 
            BackButton.Alignment = ToolStripItemAlignment.Right;
            BackButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            BackButton.Image = Properties.Resources.left_arrow;
            BackButton.ImageTransparentColor = Color.Magenta;
            BackButton.Name = "BackButton";
            BackButton.Size = new Size(34, 28);
            BackButton.Click += BackButton_Click;
            // 
            // controlCenter
            // 
            controlCenter.Alignment = ToolStripItemAlignment.Right;
            controlCenter.BackColor = SystemColors.ControlLightLight;
            controlCenter.CheckOnClick = true;
            controlCenter.DisplayStyle = ToolStripItemDisplayStyle.Image;
            controlCenter.Image = Properties.Resources.toggle_button;
            controlCenter.ImageTransparentColor = Color.Magenta;
            controlCenter.Name = "controlCenter";
            controlCenter.Size = new Size(34, 28);
            controlCenter.Click += controlCenter_Click;
            // 
            // toolStripDropDownButton1
            // 
            toolStripDropDownButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripDropDownButton1.DropDownItems.AddRange(new ToolStripItem[] { unlockToolStripMenuItem });
            toolStripDropDownButton1.Image = (Image)resources.GetObject("toolStripDropDownButton1.Image");
            toolStripDropDownButton1.ImageTransparentColor = Color.Magenta;
            toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            toolStripDropDownButton1.Size = new Size(42, 28);
            toolStripDropDownButton1.Text = "toolStripDropDownButton1";
            toolStripDropDownButton1.Visible = false;
            // 
            // unlockToolStripMenuItem
            // 
            unlockToolStripMenuItem.Image = Properties.Resources.Unlock;
            unlockToolStripMenuItem.Name = "unlockToolStripMenuItem";
            unlockToolStripMenuItem.Size = new Size(168, 34);
            unlockToolStripMenuItem.Text = "Unlock";
            unlockToolStripMenuItem.Click += unlockToolStripMenuItem_Click;
            // 
            // toolStrip2
            // 
            toolStrip2.Dock = DockStyle.Bottom;
            toolStrip2.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip2.ImageScalingSize = new Size(24, 24);
            toolStrip2.Items.AddRange(new ToolStripItem[] { statusLabel });
            toolStrip2.Location = new Point(0, 547);
            toolStrip2.Name = "toolStrip2";
            toolStrip2.RenderMode = ToolStripRenderMode.System;
            toolStrip2.Size = new Size(371, 25);
            toolStrip2.TabIndex = 2;
            toolStrip2.Text = "toolStrip2";
            // 
            // statusLabel
            // 
            statusLabel.AutoToolTip = true;
            statusLabel.BackgroundImageLayout = ImageLayout.None;
            statusLabel.DisplayStyle = ToolStripItemDisplayStyle.Text;
            statusLabel.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(0, 20);
            // 
            // ScreenControl
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(371, 605);
            Controls.Add(toolStrip2);
            Controls.Add(ScreenWebView);
            Controls.Add(toolStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "ScreenControl";
            StartPosition = FormStartPosition.WindowsDefaultBounds;
            Text = "ScreenControl";
            FormClosing += ScreenControl_FormClosing;
            FormClosed += ScreenControl_FormClosed;
            Load += ScreenControl_Load;
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
        private ToolStripButton toolStripButton1;
        private ToolStripButton buttonAlwaysOnTop;
        private ToolStripButton controlCenter;
        private ToolStripButton ObjectSpy;
        private ToolStrip toolStrip2;
        private ToolStripLabel statusLabel;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton BackButton;
        private ToolStripDropDownButton toolStripDropDownButton1;
        private ToolStripMenuItem unlockToolStripMenuItem;
    }
}