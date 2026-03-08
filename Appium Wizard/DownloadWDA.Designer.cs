namespace Appium_Wizard
{
    partial class DownloadWDA
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DownloadWDA));
            downloadLatestButton = new Button();
            versionTextBox = new TextBox();
            downloadSpecificButton = new Button();
            label1 = new Label();
            SuspendLayout();
            //
            // downloadLatestButton
            //
            downloadLatestButton.AutoSize = true;
            downloadLatestButton.Location = new Point(12, 12);
            downloadLatestButton.Name = "downloadLatestButton";
            downloadLatestButton.Size = new Size(360, 30);
            downloadLatestButton.TabIndex = 0;
            downloadLatestButton.Text = "Download latest version of WDA";
            downloadLatestButton.UseVisualStyleBackColor = true;
            downloadLatestButton.Click += DownloadLatestButton_Click;
            //
            // versionTextBox
            //
            versionTextBox.Location = new Point(12, 87);
            versionTextBox.Name = "versionTextBox";
            versionTextBox.PlaceholderText = "Enter version (e.g., 8.11.1)";
            versionTextBox.Size = new Size(360, 23);
            versionTextBox.TabIndex = 1;
            //
            // downloadSpecificButton
            //
            downloadSpecificButton.AutoSize = true;
            downloadSpecificButton.Location = new Point(12, 116);
            downloadSpecificButton.Name = "downloadSpecificButton";
            downloadSpecificButton.Size = new Size(360, 30);
            downloadSpecificButton.TabIndex = 2;
            downloadSpecificButton.Text = "Download specific version";
            downloadSpecificButton.UseVisualStyleBackColor = true;
            downloadSpecificButton.Click += DownloadSpecificButton_Click;
            //
            // label1
            //
            label1.AutoSize = true;
            label1.Location = new Point(12, 60);
            label1.Name = "label1";
            label1.Size = new Size(148, 15);
            label1.TabIndex = 3;
            label1.Text = "Or specify a specific version:";
            //
            // DownloadWDA
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLightLight;
            ClientSize = new Size(384, 161);
            Controls.Add(label1);
            Controls.Add(downloadSpecificButton);
            Controls.Add(versionTextBox);
            Controls.Add(downloadLatestButton);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "DownloadWDA";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Download WebDriverAgent";
            Load += DownloadWDA_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button downloadLatestButton;
        private TextBox versionTextBox;
        private Button downloadSpecificButton;
        private Label label1;
    }
}
