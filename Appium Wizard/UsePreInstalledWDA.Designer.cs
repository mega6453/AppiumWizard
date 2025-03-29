namespace Appium_Wizard
{
    partial class UsePreInstalledWDA
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UsePreInstalledWDA));
            bundleIdTextbox = new TextBox();
            ApplyButton = new Button();
            label1 = new Label();
            label2 = new Label();
            deviceNameLabel = new Label();
            defaultRadioButton = new RadioButton();
            customRadioButton = new RadioButton();
            cancelButton = new Button();
            dontUseRadioButton = new RadioButton();
            SuspendLayout();
            // 
            // bundleIdTextbox
            // 
            bundleIdTextbox.BorderStyle = BorderStyle.FixedSingle;
            bundleIdTextbox.Location = new Point(32, 180);
            bundleIdTextbox.Name = "bundleIdTextbox";
            bundleIdTextbox.Size = new Size(364, 23);
            bundleIdTextbox.TabIndex = 0;
            // 
            // ApplyButton
            // 
            ApplyButton.AutoSize = true;
            ApplyButton.Location = new Point(206, 209);
            ApplyButton.Name = "ApplyButton";
            ApplyButton.Size = new Size(93, 25);
            ApplyButton.TabIndex = 2;
            ApplyButton.Text = "Apply && Close";
            ApplyButton.UseVisualStyleBackColor = true;
            ApplyButton.Click += ApplyButton_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(29, 162);
            label1.Name = "label1";
            label1.Size = new Size(233, 15);
            label1.TabIndex = 3;
            label1.Text = "Enter Pre-Installed custom WDA Bundle ID:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(10, 13);
            label2.Name = "label2";
            label2.Size = new Size(51, 15);
            label2.TabIndex = 4;
            label2.Text = "Device : ";
            // 
            // deviceNameLabel
            // 
            deviceNameLabel.AutoSize = true;
            deviceNameLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            deviceNameLabel.Location = new Point(70, 13);
            deviceNameLabel.Name = "deviceNameLabel";
            deviceNameLabel.Size = new Size(0, 15);
            deviceNameLabel.TabIndex = 5;
            // 
            // defaultRadioButton
            // 
            defaultRadioButton.AutoSize = true;
            defaultRadioButton.Location = new Point(12, 82);
            defaultRadioButton.Name = "defaultRadioButton";
            defaultRadioButton.Size = new Size(291, 34);
            defaultRadioButton.TabIndex = 7;
            defaultRadioButton.TabStop = true;
            defaultRadioButton.Text = "Use Pre-Installed appium's original WDA \r\n(com.facebook.WebDriverAgentRunner.xctrunner)";
            defaultRadioButton.UseVisualStyleBackColor = true;
            // 
            // customRadioButton
            // 
            customRadioButton.AutoSize = true;
            customRadioButton.Location = new Point(12, 138);
            customRadioButton.Name = "customRadioButton";
            customRadioButton.Size = new Size(186, 19);
            customRadioButton.TabIndex = 8;
            customRadioButton.TabStop = true;
            customRadioButton.Text = "Use Pre-Installed custom WDA";
            customRadioButton.UseVisualStyleBackColor = true;
            customRadioButton.CheckedChanged += customRadioButton_CheckedChanged;
            // 
            // cancelButton
            // 
            cancelButton.AutoSize = true;
            cancelButton.Location = new Point(98, 209);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(75, 25);
            cancelButton.TabIndex = 9;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += cancelButton_Click;
            // 
            // dontUseRadioButton
            // 
            dontUseRadioButton.AutoSize = true;
            dontUseRadioButton.Location = new Point(12, 45);
            dontUseRadioButton.Name = "dontUseRadioButton";
            dontUseRadioButton.Size = new Size(174, 19);
            dontUseRadioButton.TabIndex = 10;
            dontUseRadioButton.TabStop = true;
            dontUseRadioButton.Text = "Don't use Pre-Installed WDA";
            dontUseRadioButton.UseVisualStyleBackColor = true;
            // 
            // UsePreInstalledWDA
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLightLight;
            ClientSize = new Size(408, 247);
            Controls.Add(dontUseRadioButton);
            Controls.Add(cancelButton);
            Controls.Add(customRadioButton);
            Controls.Add(defaultRadioButton);
            Controls.Add(deviceNameLabel);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(ApplyButton);
            Controls.Add(bundleIdTextbox);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "UsePreInstalledWDA";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Use Pre-Installed WDA";
            Load += UsePreInstalledWDA_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox bundleIdTextbox;
        private Button ApplyButton;
        private Label label1;
        private Label label2;
        private Label deviceNameLabel;
        private RadioButton defaultRadioButton;
        private RadioButton customRadioButton;
        private Button cancelButton;
        private RadioButton dontUseRadioButton;
    }
}