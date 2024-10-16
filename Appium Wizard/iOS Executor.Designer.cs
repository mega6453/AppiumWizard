namespace Appium_Wizard
{
    partial class iOS_Executor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(iOS_Executor));
            goRadioButton = new RadioButton();
            pyRadioButton = new RadioButton();
            label1 = new Label();
            label2 = new Label();
            cancelButton = new Button();
            OKButton = new Button();
            AutomaticRadioButton = new RadioButton();
            NoteLabel = new Label();
            SuspendLayout();
            // 
            // goRadioButton
            // 
            goRadioButton.AutoSize = true;
            goRadioButton.Location = new Point(6, 100);
            goRadioButton.Name = "goRadioButton";
            goRadioButton.Size = new Size(76, 19);
            goRadioButton.TabIndex = 1;
            goRadioButton.TabStop = true;
            goRadioButton.Text = "Method 1";
            goRadioButton.UseVisualStyleBackColor = true;
            // 
            // pyRadioButton
            // 
            pyRadioButton.AutoSize = true;
            pyRadioButton.Location = new Point(6, 144);
            pyRadioButton.Name = "pyRadioButton";
            pyRadioButton.Size = new Size(76, 19);
            pyRadioButton.TabIndex = 2;
            pyRadioButton.TabStop = true;
            pyRadioButton.Text = "Method 2";
            pyRadioButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(3, 9);
            label1.Name = "label1";
            label1.Size = new Size(363, 30);
            label1.TabIndex = 3;
            label1.Text = "If you are facing any issue on performing action from More section \r\non iOS device, then try changing the method below :";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            label2.Location = new Point(88, 56);
            label2.Name = "label2";
            label2.Size = new Size(258, 105);
            label2.TabIndex = 4;
            label2.Text = "- Decides automatically which method to use.\r\n\r\n\r\n- Admin privilege not required. Faster execution.\r\n\r\n\r\n- Admin privilege required. Bit slower execution.";
            // 
            // cancelButton
            // 
            cancelButton.AutoSize = true;
            cancelButton.Location = new Point(98, 177);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(75, 25);
            cancelButton.TabIndex = 5;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += cancelButton_Click;
            // 
            // OKButton
            // 
            OKButton.AutoSize = true;
            OKButton.Location = new Point(197, 177);
            OKButton.Name = "OKButton";
            OKButton.Size = new Size(93, 25);
            OKButton.TabIndex = 6;
            OKButton.Text = "Apply && Close";
            OKButton.UseVisualStyleBackColor = true;
            OKButton.Click += OKButton_Click;
            // 
            // AutomaticRadioButton
            // 
            AutomaticRadioButton.AutoSize = true;
            AutomaticRadioButton.Location = new Point(6, 54);
            AutomaticRadioButton.Name = "AutomaticRadioButton";
            AutomaticRadioButton.Size = new Size(81, 19);
            AutomaticRadioButton.TabIndex = 7;
            AutomaticRadioButton.TabStop = true;
            AutomaticRadioButton.Text = "Automatic";
            AutomaticRadioButton.UseVisualStyleBackColor = true;
            // 
            // NoteLabel
            // 
            NoteLabel.AutoSize = true;
            NoteLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            NoteLabel.Location = new Point(6, 206);
            NoteLabel.Name = "NoteLabel";
            NoteLabel.Size = new Size(325, 15);
            NoteLabel.TabIndex = 8;
            NoteLabel.Text = "Note : This setting does not affect automation execution.";
            // 
            // iOS_Executor
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLightLight;
            ClientSize = new Size(392, 225);
            Controls.Add(NoteLabel);
            Controls.Add(AutomaticRadioButton);
            Controls.Add(OKButton);
            Controls.Add(cancelButton);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(pyRadioButton);
            Controls.Add(goRadioButton);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "iOS_Executor";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "iOS Executor";
            Load += iOS_Executor_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private RadioButton goRadioButton;
        private RadioButton pyRadioButton;
        private Label label1;
        private Label label2;
        private Button cancelButton;
        private Button OKButton;
        private RadioButton AutomaticRadioButton;
        private Label NoteLabel;
    }
}