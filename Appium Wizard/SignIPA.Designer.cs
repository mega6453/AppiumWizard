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
            cancelButton = new Button();
            OutputPathTextBox = new TextBox();
            OutputPathButton = new Button();
            label5 = new Label();
            IPAFilePathTextBox = new TextBox();
            IPAFileButton = new Button();
            label4 = new Label();
            UDIDTextbox = new TextBox();
            label1 = new Label();
            newBundleIdTextBox = new TextBox();
            label3 = new Label();
            newBundleNameTextBox = new TextBox();
            label6 = new Label();
            newBundleVersionTextBox = new TextBox();
            label7 = new Label();
            SuspendLayout();
            // 
            // profilesListComboBox
            // 
            profilesListComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            profilesListComboBox.FormattingEnabled = true;
            profilesListComboBox.Location = new Point(93, 36);
            profilesListComboBox.Margin = new Padding(2);
            profilesListComboBox.Name = "profilesListComboBox";
            profilesListComboBox.Size = new Size(466, 23);
            profilesListComboBox.TabIndex = 27;
            // 
            // selectProfileLabel
            // 
            selectProfileLabel.AutoSize = true;
            selectProfileLabel.Location = new Point(4, 36);
            selectProfileLabel.Margin = new Padding(2, 0, 2, 0);
            selectProfileLabel.Name = "selectProfileLabel";
            selectProfileLabel.Size = new Size(80, 15);
            selectProfileLabel.TabIndex = 26;
            selectProfileLabel.Text = "Select Profile*";
            // 
            // SignButton
            // 
            SignButton.AutoSize = true;
            SignButton.Location = new Point(302, 303);
            SignButton.Margin = new Padding(2);
            SignButton.Name = "SignButton";
            SignButton.Size = new Size(78, 25);
            SignButton.TabIndex = 29;
            SignButton.Text = "Sign";
            SignButton.UseVisualStyleBackColor = true;
            SignButton.Click += SignButton_Click;
            // 
            // cancelButton
            // 
            cancelButton.AutoSize = true;
            cancelButton.Location = new Point(182, 303);
            cancelButton.Margin = new Padding(2);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(78, 25);
            cancelButton.TabIndex = 28;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += CancelButton_Click;
            // 
            // OutputPathTextBox
            // 
            OutputPathTextBox.Enabled = false;
            OutputPathTextBox.Location = new Point(93, 115);
            OutputPathTextBox.Margin = new Padding(2);
            OutputPathTextBox.Name = "OutputPathTextBox";
            OutputPathTextBox.PlaceholderText = "Select the path where you want to save the Signed IPA file";
            OutputPathTextBox.Size = new Size(435, 23);
            OutputPathTextBox.TabIndex = 35;
            // 
            // OutputPathButton
            // 
            OutputPathButton.AutoSize = true;
            OutputPathButton.Location = new Point(532, 113);
            OutputPathButton.Margin = new Padding(2);
            OutputPathButton.Name = "OutputPathButton";
            OutputPathButton.Size = new Size(26, 25);
            OutputPathButton.TabIndex = 34;
            OutputPathButton.Text = "...";
            OutputPathButton.UseVisualStyleBackColor = true;
            OutputPathButton.Click += OutputPathButton_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(4, 118);
            label5.Margin = new Padding(2, 0, 2, 0);
            label5.Name = "label5";
            label5.Size = new Size(77, 15);
            label5.TabIndex = 33;
            label5.Text = "Output Path*";
            // 
            // IPAFilePathTextBox
            // 
            IPAFilePathTextBox.Enabled = false;
            IPAFilePathTextBox.Location = new Point(93, 76);
            IPAFilePathTextBox.Margin = new Padding(2);
            IPAFilePathTextBox.Name = "IPAFilePathTextBox";
            IPAFilePathTextBox.PlaceholderText = "Select an IPA file which you want to Sign";
            IPAFilePathTextBox.Size = new Size(435, 23);
            IPAFilePathTextBox.TabIndex = 32;
            // 
            // IPAFileButton
            // 
            IPAFileButton.AutoSize = true;
            IPAFileButton.Location = new Point(532, 74);
            IPAFileButton.Margin = new Padding(2);
            IPAFileButton.Name = "IPAFileButton";
            IPAFileButton.Size = new Size(26, 25);
            IPAFileButton.TabIndex = 31;
            IPAFileButton.Text = "...";
            IPAFileButton.UseVisualStyleBackColor = true;
            IPAFileButton.Click += IPAFileButton_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(4, 79);
            label4.Margin = new Padding(2, 0, 2, 0);
            label4.Name = "label4";
            label4.Size = new Size(48, 15);
            label4.TabIndex = 30;
            label4.Text = "IPA file*";
            // 
            // UDIDTextbox
            // 
            UDIDTextbox.Location = new Point(93, 165);
            UDIDTextbox.Margin = new Padding(2);
            UDIDTextbox.Name = "UDIDTextbox";
            UDIDTextbox.PlaceholderText = "Optional - Verify the profile has this UDID or not";
            UDIDTextbox.Size = new Size(435, 23);
            UDIDTextbox.TabIndex = 37;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(4, 169);
            label1.Margin = new Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new Size(72, 15);
            label1.TabIndex = 36;
            label1.Text = "Device UDID";
            // 
            // newBundleIdTextBox
            // 
            newBundleIdTextBox.Location = new Point(93, 198);
            newBundleIdTextBox.Margin = new Padding(2);
            newBundleIdTextBox.Name = "newBundleIdTextBox";
            newBundleIdTextBox.PlaceholderText = "Optional - New Bundle ID to change";
            newBundleIdTextBox.Size = new Size(435, 23);
            newBundleIdTextBox.TabIndex = 40;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(4, 201);
            label3.Margin = new Padding(2, 0, 2, 0);
            label3.Name = "label3";
            label3.Size = new Size(58, 15);
            label3.TabIndex = 39;
            label3.Text = "Bundle ID";
            // 
            // newBundleNameTextBox
            // 
            newBundleNameTextBox.Location = new Point(93, 231);
            newBundleNameTextBox.Margin = new Padding(2);
            newBundleNameTextBox.Name = "newBundleNameTextBox";
            newBundleNameTextBox.PlaceholderText = "Optional - New Bundle name to change";
            newBundleNameTextBox.Size = new Size(435, 23);
            newBundleNameTextBox.TabIndex = 42;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(4, 236);
            label6.Margin = new Padding(2, 0, 2, 0);
            label6.Name = "label6";
            label6.Size = new Size(79, 15);
            label6.TabIndex = 41;
            label6.Text = "Bundle Name";
            // 
            // newBundleVersionTextBox
            // 
            newBundleVersionTextBox.Location = new Point(93, 267);
            newBundleVersionTextBox.Margin = new Padding(2);
            newBundleVersionTextBox.Name = "newBundleVersionTextBox";
            newBundleVersionTextBox.PlaceholderText = "Optional - New Bundle version to change";
            newBundleVersionTextBox.Size = new Size(435, 23);
            newBundleVersionTextBox.TabIndex = 44;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(4, 275);
            label7.Margin = new Padding(2, 0, 2, 0);
            label7.Name = "label7";
            label7.Size = new Size(85, 15);
            label7.TabIndex = 43;
            label7.Text = "Bundle Version";
            // 
            // SignIPA
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLightLight;
            ClientSize = new Size(570, 335);
            Controls.Add(newBundleVersionTextBox);
            Controls.Add(label7);
            Controls.Add(newBundleNameTextBox);
            Controls.Add(label6);
            Controls.Add(newBundleIdTextBox);
            Controls.Add(label3);
            Controls.Add(UDIDTextbox);
            Controls.Add(label1);
            Controls.Add(OutputPathTextBox);
            Controls.Add(OutputPathButton);
            Controls.Add(label5);
            Controls.Add(IPAFilePathTextBox);
            Controls.Add(IPAFileButton);
            Controls.Add(label4);
            Controls.Add(SignButton);
            Controls.Add(cancelButton);
            Controls.Add(profilesListComboBox);
            Controls.Add(selectProfileLabel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(2);
            MaximizeBox = false;
            Name = "SignIPA";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "IPA Signer";
            Load += SignIPA_Load;
            Shown += SignIPA_Shown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox profilesListComboBox;
        private Label selectProfileLabel;
        private Button SignButton;
        private Button cancelButton;
        private TextBox OutputPathTextBox;
        private Button OutputPathButton;
        private Label label5;
        private TextBox IPAFilePathTextBox;
        private Button IPAFileButton;
        private Label label4;
        private TextBox UDIDTextbox;
        private Label label1;
        private TextBox newBundleIdTextBox;
        private Label label3;
        private TextBox newBundleNameTextBox;
        private Label label6;
        private TextBox newBundleVersionTextBox;
        private Label label7;
    }
}