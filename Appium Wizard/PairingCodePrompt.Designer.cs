namespace Appium_Wizard
{
    partial class PairingCodePrompt
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PairingCodePrompt));
            label1 = new Label();
            label2 = new Label();
            PairingCodeTextBox = new TextBox();
            button1 = new Button();
            button2 = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(15, 13);
            label1.Margin = new Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new Size(271, 15);
            label1.TabIndex = 0;
            label1.Text = "Enter the 6 digit code shown on the device to pair.";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(15, 40);
            label2.Margin = new Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new Size(0, 15);
            label2.TabIndex = 1;
            // 
            // PairingCodeTextBox
            // 
            PairingCodeTextBox.Location = new Point(55, 73);
            PairingCodeTextBox.Margin = new Padding(2);
            PairingCodeTextBox.Name = "PairingCodeTextBox";
            PairingCodeTextBox.Size = new Size(192, 23);
            PairingCodeTextBox.TabIndex = 2;
            PairingCodeTextBox.TextChanged += PairingCodeTextBox_TextChanged;
            // 
            // button1
            // 
            button1.AutoSize = true;
            button1.Enabled = false;
            button1.Location = new Point(169, 113);
            button1.Margin = new Padding(2);
            button1.Name = "button1";
            button1.Size = new Size(78, 25);
            button1.TabIndex = 3;
            button1.Text = "Pair";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.AutoSize = true;
            button2.Location = new Point(55, 113);
            button2.Margin = new Padding(2);
            button2.Name = "button2";
            button2.Size = new Size(78, 25);
            button2.TabIndex = 4;
            button2.Text = "Cancel";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // PairingCodePrompt
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLightLight;
            ClientSize = new Size(316, 152);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(PairingCodeTextBox);
            Controls.Add(label2);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(2);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PairingCodePrompt";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Enter Pairing Code";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private TextBox PairingCodeTextBox;
        private Button button1;
        private Button button2;
    }
}