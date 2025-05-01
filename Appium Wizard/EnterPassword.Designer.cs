namespace Appium_Wizard
{
    partial class EnterPassword
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EnterPassword));
            PasswordTextbox = new TextBox();
            cancelButton = new Button();
            label2 = new Label();
            Unlock = new Button();
            NoteForiOSLabel = new Label();
            SuspendLayout();
            // 
            // PasswordTextbox
            // 
            PasswordTextbox.Location = new Point(93, 42);
            PasswordTextbox.Name = "PasswordTextbox";
            PasswordTextbox.Size = new Size(169, 23);
            PasswordTextbox.TabIndex = 1;
            // 
            // cancelButton
            // 
            cancelButton.AutoSize = true;
            cancelButton.Location = new Point(93, 90);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(75, 25);
            cancelButton.TabIndex = 2;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += CancelButton_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(2, 46);
            label2.Name = "label2";
            label2.Size = new Size(87, 15);
            label2.TabIndex = 3;
            label2.Text = "Enter Password";
            // 
            // Unlock
            // 
            Unlock.AutoSize = true;
            Unlock.Location = new Point(187, 90);
            Unlock.Name = "Unlock";
            Unlock.Size = new Size(75, 25);
            Unlock.TabIndex = 4;
            Unlock.Text = "Unlock";
            Unlock.UseVisualStyleBackColor = true;
            Unlock.Click += Unlock_Click;
            // 
            // NoteForiOSLabel
            // 
            NoteForiOSLabel.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            NoteForiOSLabel.ForeColor = Color.IndianRed;
            NoteForiOSLabel.Location = new Point(4, 135);
            NoteForiOSLabel.Name = "NoteForiOSLabel";
            NoteForiOSLabel.Size = new Size(303, 36);
            NoteForiOSLabel.TabIndex = 5;
            NoteForiOSLabel.Text = "Note : This works only if the WebDriverAgent is already running in the iPhone.";
            NoteForiOSLabel.Visible = false;
            // 
            // EnterPassword
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLightLight;
            ClientSize = new Size(312, 184);
            Controls.Add(NoteForiOSLabel);
            Controls.Add(Unlock);
            Controls.Add(label2);
            Controls.Add(cancelButton);
            Controls.Add(PasswordTextbox);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "EnterPassword";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Unlock Screen";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TextBox PasswordTextbox;
        private Button cancelButton;
        private Label label2;
        private Button Unlock;
        private Label NoteForiOSLabel;
    }
}