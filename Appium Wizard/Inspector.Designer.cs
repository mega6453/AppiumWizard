namespace Appium_Wizard
{
    partial class Inspector
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Inspector));
            inspectorWebView = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)inspectorWebView).BeginInit();
            SuspendLayout();
            // 
            // inspectorWebView
            // 
            inspectorWebView.AllowExternalDrop = true;
            inspectorWebView.CreationProperties = null;
            inspectorWebView.DefaultBackgroundColor = Color.White;
            inspectorWebView.Dock = DockStyle.Fill;
            inspectorWebView.Location = new Point(0, 0);
            inspectorWebView.Name = "inspectorWebView";
            inspectorWebView.Size = new Size(800, 450);
            inspectorWebView.TabIndex = 0;
            inspectorWebView.ZoomFactor = 1D;
            // 
            // Inspector
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(inspectorWebView);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Inspector";
            Text = "Inspector";
            WindowState = FormWindowState.Maximized;
            Load += Inspector_Load;
            Shown += Inspector_Shown;
            ((System.ComponentModel.ISupportInitialize)inspectorWebView).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Microsoft.Web.WebView2.WinForms.WebView2 inspectorWebView;
    }
}