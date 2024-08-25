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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Server_Settings));
            ServerArgs = new RichTextBox();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            DefaultCapabilities = new RichTextBox();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            ServerArgsLink = new LinkLabel();
            FinalCommandRichTextBox = new RichTextBox();
            cancelButton = new Button();
            applyButton = new Button();
            defaultCapLinkLabel = new LinkLabel();
            label8 = new Label();
            SuspendLayout();
            // 
            // ServerArgs
            // 
            ServerArgs.Location = new Point(12, 95);
            ServerArgs.Name = "ServerArgs";
            ServerArgs.Size = new Size(1025, 72);
            ServerArgs.TabIndex = 1;
            ServerArgs.Text = "";
            ServerArgs.TextChanged += ServerArgs_TextChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label2.Location = new Point(15, 320);
            label2.Name = "label2";
            label2.Size = new Size(153, 25);
            label2.TabIndex = 2;
            label2.Text = "Final Command :";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label3.Location = new Point(12, 528);
            label3.Name = "label3";
            label3.Size = new Size(602, 25);
            label3.TabIndex = 4;
            label3.Text = "Following args will be added automatically while starting the server : ";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 570);
            label4.Name = "label4";
            label4.Size = new Size(607, 75);
            label4.TabIndex = 5;
            label4.Text = resources.GetString("label4.Text");
            // 
            // DefaultCapabilities
            // 
            DefaultCapabilities.Location = new Point(15, 208);
            DefaultCapabilities.Name = "DefaultCapabilities";
            DefaultCapabilities.Size = new Size(1026, 109);
            DefaultCapabilities.TabIndex = 6;
            DefaultCapabilities.Text = "";
            DefaultCapabilities.TextChanged += DefaultCapabilities_TextChanged;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label5.ForeColor = Color.IndianRed;
            label5.Location = new Point(12, 9);
            label5.Name = "label5";
            label5.Size = new Size(1026, 50);
            label5.TabIndex = 7;
            label5.Text = "NOTE : THERE IS NO VALIDATION DONE ON THE GIVEN COMMAND, SO MAKE SURE YOU ARE ENTERING VALID ARGS.\r\nWHILE STARTING THE SERVER, IF IT FAILS THEN FIX THE ARGS.";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(12, 67);
            label6.Name = "label6";
            label6.Size = new Size(107, 25);
            label6.TabIndex = 8;
            label6.Text = "Server Args:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(11, 180);
            label7.Name = "label7";
            label7.Size = new Size(164, 25);
            label7.TabIndex = 9;
            label7.Text = "Default Capabilites:";
            // 
            // ServerArgsLink
            // 
            ServerArgsLink.AutoSize = true;
            ServerArgsLink.Font = new Font("Segoe UI", 9F, FontStyle.Italic, GraphicsUnit.Point);
            ServerArgsLink.Location = new Point(125, 67);
            ServerArgsLink.Name = "ServerArgsLink";
            ServerArgsLink.Size = new Size(90, 25);
            ServerArgsLink.TabIndex = 10;
            ServerArgsLink.TabStop = true;
            ServerArgsLink.Text = "Refer here";
            ServerArgsLink.LinkClicked += ServerArgsLink_LinkClicked;
            // 
            // FinalCommandRichTextBox
            // 
            FinalCommandRichTextBox.DetectUrls = false;
            FinalCommandRichTextBox.Location = new Point(15, 348);
            FinalCommandRichTextBox.Name = "FinalCommandRichTextBox";
            FinalCommandRichTextBox.ReadOnly = true;
            FinalCommandRichTextBox.Size = new Size(1023, 123);
            FinalCommandRichTextBox.TabIndex = 11;
            FinalCommandRichTextBox.Text = "";
            // 
            // cancelButton
            // 
            cancelButton.AutoSize = true;
            cancelButton.Location = new Point(362, 477);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(112, 35);
            cancelButton.TabIndex = 12;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += cancelButton_Click;
            // 
            // applyButton
            // 
            applyButton.AutoSize = true;
            applyButton.Location = new Point(524, 477);
            applyButton.Name = "applyButton";
            applyButton.Size = new Size(136, 35);
            applyButton.TabIndex = 13;
            applyButton.Text = "Apply && Close";
            applyButton.UseVisualStyleBackColor = true;
            applyButton.Click += applyButton_Click;
            // 
            // defaultCapLinkLabel
            // 
            defaultCapLinkLabel.AutoSize = true;
            defaultCapLinkLabel.Font = new Font("Segoe UI", 9F, FontStyle.Italic, GraphicsUnit.Point);
            defaultCapLinkLabel.Location = new Point(181, 180);
            defaultCapLinkLabel.Name = "defaultCapLinkLabel";
            defaultCapLinkLabel.Size = new Size(90, 25);
            defaultCapLinkLabel.TabIndex = 14;
            defaultCapLinkLabel.TabStop = true;
            defaultCapLinkLabel.Text = "Refer here";
            defaultCapLinkLabel.LinkClicked += defaultCapLinkLabel_LinkClicked;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.ForeColor = Color.Chocolate;
            label8.Location = new Point(278, 180);
            label8.Name = "label8";
            label8.Size = new Size(757, 25);
            label8.TabIndex = 15;
            label8.Text = "Enter valid json string below. E.g. {\"appium:newCommandTimeout\": 0, \"appium:noReset\": true}";
            // 
            // Server_Settings
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLightLight;
            ClientSize = new Size(1048, 656);
            Controls.Add(label8);
            Controls.Add(defaultCapLinkLabel);
            Controls.Add(applyButton);
            Controls.Add(cancelButton);
            Controls.Add(FinalCommandRichTextBox);
            Controls.Add(ServerArgsLink);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(DefaultCapabilities);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(ServerArgs);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Server_Settings";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Server Settings";
            Load += Server_Settings_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private RichTextBox ServerArgs;
        private Label label2;
        private Label label3;
        private Label label4;
        private RichTextBox DefaultCapabilities;
        private Label label5;
        private Label label6;
        private Label label7;
        private LinkLabel ServerArgsLink;
        private RichTextBox FinalCommandRichTextBox;
        private Button cancelButton;
        private Button applyButton;
        private LinkLabel defaultCapLinkLabel;
        private Label label8;
    }
}