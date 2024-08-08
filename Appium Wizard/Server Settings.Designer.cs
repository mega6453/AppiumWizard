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
            label1 = new Label();
            ServerArgs = new RichTextBox();
            label2 = new Label();
            finalCommandLabel = new Label();
            label3 = new Label();
            label4 = new Label();
            DefaultCapabilities = new RichTextBox();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            ServerArgsLink = new LinkLabel();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(423, 25);
            label1.TabIndex = 0;
            label1.Text = "Enter the required Server Command-Line Argument.\r\n";
            // 
            // ServerArgs
            // 
            ServerArgs.Location = new Point(12, 169);
            ServerArgs.Name = "ServerArgs";
            ServerArgs.Size = new Size(1025, 72);
            ServerArgs.TabIndex = 1;
            ServerArgs.Text = "";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label2.Location = new Point(12, 419);
            label2.Name = "label2";
            label2.Size = new Size(153, 25);
            label2.TabIndex = 2;
            label2.Text = "Final Command :";
            // 
            // finalCommandLabel
            // 
            finalCommandLabel.AutoSize = true;
            finalCommandLabel.Location = new Point(12, 444);
            finalCommandLabel.Name = "finalCommandLabel";
            finalCommandLabel.Size = new Size(991, 25);
            finalCommandLabel.TabIndex = 3;
            finalCommandLabel.Text = "appium --port {{portNumber}} --allow-cors --default-capabilities \"{\\\"appium:webDriverAgentUrl\\\":\\\"http://localhost:56254\\\"}";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label3.Location = new Point(12, 502);
            label3.Name = "label3";
            label3.Size = new Size(602, 25);
            label3.TabIndex = 4;
            label3.Text = "Following args will be added automatically while starting the server : ";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 544);
            label4.Name = "label4";
            label4.Size = new Size(607, 75);
            label4.TabIndex = 5;
            label4.Text = resources.GetString("label4.Text");
            // 
            // DefaultCapabilities
            // 
            DefaultCapabilities.Location = new Point(12, 311);
            DefaultCapabilities.Name = "DefaultCapabilities";
            DefaultCapabilities.Size = new Size(1026, 72);
            DefaultCapabilities.TabIndex = 6;
            DefaultCapabilities.Text = "";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label5.ForeColor = Color.IndianRed;
            label5.Location = new Point(12, 44);
            label5.Name = "label5";
            label5.Size = new Size(1026, 50);
            label5.TabIndex = 7;
            label5.Text = "NOTE : THERE IS NO VALIDATION DONE ON THE GIVEN COMMAND, SO MAKE SURE YOU ARE ENTERING VALID ARGS.\r\nWHILE STARTING THE SERVER, IF IT FAILS THEN FIX THE ARGS.";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(12, 141);
            label6.Name = "label6";
            label6.Size = new Size(103, 25);
            label6.TabIndex = 8;
            label6.Text = "Server Args";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(12, 283);
            label7.Name = "label7";
            label7.Size = new Size(160, 25);
            label7.TabIndex = 9;
            label7.Text = "Deafult Capabilites";
            // 
            // ServerArgsLink
            // 
            ServerArgsLink.AutoSize = true;
            ServerArgsLink.Location = new Point(486, 9);
            ServerArgsLink.Name = "ServerArgsLink";
            ServerArgsLink.Size = new Size(342, 25);
            ServerArgsLink.TabIndex = 10;
            ServerArgsLink.TabStop = true;
            ServerArgsLink.Text = "https://appium.io/docs/en/latest/cli/args/";
            ServerArgsLink.LinkClicked += ServerArgsLink_LinkClicked;
            // 
            // Server_Settings
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1049, 637);
            Controls.Add(ServerArgsLink);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(DefaultCapabilities);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(finalCommandLabel);
            Controls.Add(label2);
            Controls.Add(ServerArgs);
            Controls.Add(label1);
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

        private Label label1;
        private RichTextBox ServerArgs;
        private Label label2;
        private Label finalCommandLabel;
        private Label label3;
        private Label label4;
        private RichTextBox DefaultCapabilities;
        private Label label5;
        private Label label6;
        private Label label7;
        private LinkLabel ServerArgsLink;
    }
}