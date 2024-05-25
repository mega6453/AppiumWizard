namespace Appium_Wizard
{
    partial class SignIPA
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SignIPA));
            profilesListComboBox = new ComboBox();
            selectProfileLabel = new Label();
            SignButton = new Button();
            CancelButton = new Button();
            OutputPathTextBox = new TextBox();
            OutputPathButton = new Button();
            label5 = new Label();
            IPAFilePathTextBox = new TextBox();
            IPAFileButton = new Button();
            label4 = new Label();
            UDIDTextbox = new TextBox();
            label1 = new Label();
            label2 = new Label();
            SuspendLayout();
            // 
            // profilesListComboBox
            // 
            profilesListComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            profilesListComboBox.FormattingEnabled = true;
            profilesListComboBox.Location = new Point(124, 60);
            profilesListComboBox.Name = "profilesListComboBox";
            profilesListComboBox.Size = new Size(664, 33);
            profilesListComboBox.TabIndex = 27;
            // 
            // selectProfileLabel
            // 
            selectProfileLabel.AutoSize = true;
            selectProfileLabel.Location = new Point(5, 60);
            selectProfileLabel.Name = "selectProfileLabel";
            selectProfileLabel.Size = new Size(121, 25);
            selectProfileLabel.TabIndex = 26;
            selectProfileLabel.Text = "Select Profile*";
            // 
            // SignButton
            // 
            SignButton.Location = new Point(425, 338);
            SignButton.Name = "SignButton";
            SignButton.Size = new Size(112, 34);
            SignButton.TabIndex = 29;
            SignButton.Text = "Sign";
            SignButton.UseVisualStyleBackColor = true;
            SignButton.Click += SignButton_Click;
            // 
            // CancelButton
            // 
            CancelButton.Location = new Point(254, 338);
            CancelButton.Name = "CancelButton";
            CancelButton.Size = new Size(112, 34);
            CancelButton.TabIndex = 28;
            CancelButton.Text = "Cancel";
            CancelButton.UseVisualStyleBackColor = true;
            CancelButton.Click += CancelButton_Click;
            // 
            // OutputPathTextBox
            // 
            OutputPathTextBox.Enabled = false;
            OutputPathTextBox.Location = new Point(124, 191);
            OutputPathTextBox.Name = "OutputPathTextBox";
            OutputPathTextBox.PlaceholderText = "Select the path where you want to save the Signed IPA file";
            OutputPathTextBox.Size = new Size(620, 31);
            OutputPathTextBox.TabIndex = 35;
            // 
            // OutputPathButton
            // 
            OutputPathButton.Location = new Point(751, 188);
            OutputPathButton.Name = "OutputPathButton";
            OutputPathButton.Size = new Size(37, 34);
            OutputPathButton.TabIndex = 34;
            OutputPathButton.Text = "...";
            OutputPathButton.UseVisualStyleBackColor = true;
            OutputPathButton.Click += OutputPathButton_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(5, 197);
            label5.Name = "label5";
            label5.Size = new Size(116, 25);
            label5.TabIndex = 33;
            label5.Text = "Output Path*";
            // 
            // IPAFilePathTextBox
            // 
            IPAFilePathTextBox.Enabled = false;
            IPAFilePathTextBox.Location = new Point(124, 126);
            IPAFilePathTextBox.Name = "IPAFilePathTextBox";
            IPAFilePathTextBox.PlaceholderText = "Select an IPA file which you want to Sign";
            IPAFilePathTextBox.Size = new Size(620, 31);
            IPAFilePathTextBox.TabIndex = 32;
            // 
            // IPAFileButton
            // 
            IPAFileButton.Location = new Point(751, 127);
            IPAFileButton.Name = "IPAFileButton";
            IPAFileButton.Size = new Size(37, 34);
            IPAFileButton.TabIndex = 31;
            IPAFileButton.Text = "...";
            IPAFileButton.UseVisualStyleBackColor = true;
            IPAFileButton.Click += IPAFileButton_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(5, 132);
            label4.Name = "label4";
            label4.Size = new Size(74, 25);
            label4.TabIndex = 30;
            label4.Text = "IPA file*";
            // 
            // UDIDTextbox
            // 
            UDIDTextbox.Location = new Point(124, 250);
            UDIDTextbox.Name = "UDIDTextbox";
            UDIDTextbox.PlaceholderText = "Optional";
            UDIDTextbox.Size = new Size(620, 31);
            UDIDTextbox.TabIndex = 37;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(5, 256);
            label1.Name = "label1";
            label1.Size = new Size(112, 25);
            label1.TabIndex = 36;
            label1.Text = "Device UDID";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Italic, GraphicsUnit.Point);
            label2.ForeColor = Color.Chocolate;
            label2.Location = new Point(124, 284);
            label2.Name = "label2";
            label2.Size = new Size(599, 25);
            label2.TabIndex = 38;
            label2.Text = "If you enter an UDID, it will verify if the selected profile contains this UDID.";
            // 
            // SignIPA
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLightLight;
            ClientSize = new Size(800, 384);
            Controls.Add(label2);
            Controls.Add(UDIDTextbox);
            Controls.Add(label1);
            Controls.Add(OutputPathTextBox);
            Controls.Add(OutputPathButton);
            Controls.Add(label5);
            Controls.Add(IPAFilePathTextBox);
            Controls.Add(IPAFileButton);
            Controls.Add(label4);
            Controls.Add(SignButton);
            Controls.Add(CancelButton);
            Controls.Add(profilesListComboBox);
            Controls.Add(selectProfileLabel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SignIPA";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Sign IPA";
            Load += SignIPA_Load;
            Shown += SignIPA_Shown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox profilesListComboBox;
        private Label selectProfileLabel;
        private Button SignButton;
        private Button CancelButton;
        private TextBox OutputPathTextBox;
        private Button OutputPathButton;
        private Label label5;
        private TextBox IPAFilePathTextBox;
        private Button IPAFileButton;
        private Label label4;
        private TextBox UDIDTextbox;
        private Label label1;
        private Label label2;
    }
}