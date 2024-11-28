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
            CancelButton = new Button();
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
            // CancelButton
            // 
            CancelButton.AutoSize = true;
            CancelButton.Location = new Point(93, 90);
            CancelButton.Name = "CancelButton";
            CancelButton.Size = new Size(75, 25);
            CancelButton.TabIndex = 2;
            CancelButton.Text = "Cancel";
            CancelButton.UseVisualStyleBackColor = true;
            CancelButton.Click += CancelButton_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(20, 45);
            label2.Name = "label2";
            label2.Size = new Size(57, 15);
            label2.TabIndex = 3;
            label2.Text = "Password";
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
            NoteForiOSLabel.Location = new Point(20, 135);
            NoteForiOSLabel.Name = "NoteForiOSLabel";
            NoteForiOSLabel.Size = new Size(268, 40);
            NoteForiOSLabel.TabIndex = 5;
            NoteForiOSLabel.Text = "This works only if WebDriverAgent is already \r\nrunning in the iPhone.";
            NoteForiOSLabel.Visible = false;
            // 
            // EnterPassword
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(321, 184);
            Controls.Add(NoteForiOSLabel);
            Controls.Add(Unlock);
            Controls.Add(label2);
            Controls.Add(CancelButton);
            Controls.Add(PasswordTextbox);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "EnterPassword";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Unlock Screen";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TextBox PasswordTextbox;
        private Button CancelButton;
        private Label label2;
        private Button Unlock;
        private Label NoteForiOSLabel;
    }
}