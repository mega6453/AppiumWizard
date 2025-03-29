namespace Appium_Wizard
{
    partial class TroubleShooter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TroubleShooter));
            NodeJSLabel = new Label();
            AppiumLabel = new Label();
            XCUITestLabel = new Label();
            ComponentsLabel = new Label();
            StatusLabel = new Label();
            FixLabel = new Label();
            checkForIssues = new Button();
            UIAutomatorLabel = new Label();
            FixWDAButton = new Button();
            label1 = new Label();
            FixUIAutomatorButton = new Button();
            FixXCUITestButton = new Button();
            UIAutomatorStatusLabel = new Label();
            XCUITestStatusLabel = new Label();
            AppiumStatusLabel = new Label();
            NodeJSStatusLabel = new Label();
            FixAppiumButton = new Button();
            FixNodeJSButton = new Button();
            tableLayoutPanel1 = new TableLayoutPanel();
            WDAStatusLabel = new Label();
            reInstallAllButton = new Button();
            showProgresscheckBox = new CheckBox();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // NodeJSLabel
            // 
            NodeJSLabel.Anchor = AnchorStyles.None;
            NodeJSLabel.AutoSize = true;
            NodeJSLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            NodeJSLabel.Location = new Point(65, 45);
            NodeJSLabel.Margin = new Padding(2, 0, 2, 0);
            NodeJSLabel.Name = "NodeJSLabel";
            NodeJSLabel.Size = new Size(49, 15);
            NodeJSLabel.TabIndex = 6;
            NodeJSLabel.Text = "NodeJS";
            // 
            // AppiumLabel
            // 
            AppiumLabel.Anchor = AnchorStyles.None;
            AppiumLabel.AutoSize = true;
            AppiumLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            AppiumLabel.Location = new Point(64, 80);
            AppiumLabel.Margin = new Padding(2, 0, 2, 0);
            AppiumLabel.Name = "AppiumLabel";
            AppiumLabel.Size = new Size(50, 15);
            AppiumLabel.TabIndex = 7;
            AppiumLabel.Text = "Appium";
            AppiumLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // XCUITestLabel
            // 
            XCUITestLabel.Anchor = AnchorStyles.None;
            XCUITestLabel.AutoSize = true;
            XCUITestLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            XCUITestLabel.Location = new Point(60, 115);
            XCUITestLabel.Margin = new Padding(2, 0, 2, 0);
            XCUITestLabel.Name = "XCUITestLabel";
            XCUITestLabel.Size = new Size(58, 15);
            XCUITestLabel.TabIndex = 8;
            XCUITestLabel.Text = "XCUITest";
            XCUITestLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // ComponentsLabel
            // 
            ComponentsLabel.Anchor = AnchorStyles.None;
            ComponentsLabel.AutoSize = true;
            ComponentsLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            ComponentsLabel.Location = new Point(51, 10);
            ComponentsLabel.Margin = new Padding(2, 0, 2, 0);
            ComponentsLabel.Name = "ComponentsLabel";
            ComponentsLabel.Size = new Size(77, 15);
            ComponentsLabel.TabIndex = 10;
            ComponentsLabel.Text = "Components";
            // 
            // StatusLabel
            // 
            StatusLabel.Anchor = AnchorStyles.None;
            StatusLabel.AutoSize = true;
            StatusLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            StatusLabel.Location = new Point(246, 10);
            StatusLabel.Margin = new Padding(2, 0, 2, 0);
            StatusLabel.Name = "StatusLabel";
            StatusLabel.Size = new Size(42, 15);
            StatusLabel.TabIndex = 11;
            StatusLabel.Text = "Status";
            // 
            // FixLabel
            // 
            FixLabel.Anchor = AnchorStyles.None;
            FixLabel.AutoSize = true;
            FixLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            FixLabel.Location = new Point(433, 10);
            FixLabel.Margin = new Padding(2, 0, 2, 0);
            FixLabel.Name = "FixLabel";
            FixLabel.Size = new Size(23, 15);
            FixLabel.TabIndex = 17;
            FixLabel.Text = "Fix";
            // 
            // checkForIssues
            // 
            checkForIssues.AutoSize = true;
            checkForIssues.Location = new Point(24, 226);
            checkForIssues.Margin = new Padding(2);
            checkForIssues.Name = "checkForIssues";
            checkForIssues.Size = new Size(106, 25);
            checkForIssues.TabIndex = 18;
            checkForIssues.Text = "Check for Issues";
            checkForIssues.UseVisualStyleBackColor = true;
            checkForIssues.Click += checkForIssues_Click;
            // 
            // UIAutomatorLabel
            // 
            UIAutomatorLabel.Anchor = AnchorStyles.None;
            UIAutomatorLabel.AutoSize = true;
            UIAutomatorLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            UIAutomatorLabel.Location = new Point(45, 150);
            UIAutomatorLabel.Margin = new Padding(2, 0, 2, 0);
            UIAutomatorLabel.Name = "UIAutomatorLabel";
            UIAutomatorLabel.Size = new Size(88, 15);
            UIAutomatorLabel.TabIndex = 9;
            UIAutomatorLabel.Text = "UIAutomator2";
            UIAutomatorLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // FixWDAButton
            // 
            FixWDAButton.Anchor = AnchorStyles.None;
            FixWDAButton.AutoSize = true;
            FixWDAButton.Enabled = false;
            FixWDAButton.Location = new Point(407, 181);
            FixWDAButton.Name = "FixWDAButton";
            FixWDAButton.Size = new Size(75, 25);
            FixWDAButton.TabIndex = 21;
            FixWDAButton.Text = "Fix WDA";
            FixWDAButton.UseVisualStyleBackColor = true;
            FixWDAButton.Click += FixWDAButton_Click;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.None;
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label1.Location = new Point(17, 186);
            label1.Name = "label1";
            label1.Size = new Size(144, 15);
            label1.TabIndex = 20;
            label1.Text = "WebDriverAgentRunner";
            // 
            // FixUIAutomatorButton
            // 
            FixUIAutomatorButton.Anchor = AnchorStyles.None;
            FixUIAutomatorButton.AutoSize = true;
            FixUIAutomatorButton.Enabled = false;
            FixUIAutomatorButton.Location = new Point(384, 144);
            FixUIAutomatorButton.Margin = new Padding(2);
            FixUIAutomatorButton.Name = "FixUIAutomatorButton";
            FixUIAutomatorButton.Size = new Size(120, 28);
            FixUIAutomatorButton.TabIndex = 4;
            FixUIAutomatorButton.Text = "Fix UIAutomator";
            FixUIAutomatorButton.UseVisualStyleBackColor = true;
            FixUIAutomatorButton.Click += FixUIAutomatorButton_Click;
            // 
            // FixXCUITestButton
            // 
            FixXCUITestButton.Anchor = AnchorStyles.None;
            FixXCUITestButton.AutoSize = true;
            FixXCUITestButton.Enabled = false;
            FixXCUITestButton.Location = new Point(386, 110);
            FixXCUITestButton.Margin = new Padding(2);
            FixXCUITestButton.Name = "FixXCUITestButton";
            FixXCUITestButton.Size = new Size(117, 25);
            FixXCUITestButton.TabIndex = 3;
            FixXCUITestButton.Text = "Fix XCUITest";
            FixXCUITestButton.UseVisualStyleBackColor = true;
            FixXCUITestButton.Click += FixXCUITestButton_Click;
            // 
            // UIAutomatorStatusLabel
            // 
            UIAutomatorStatusLabel.Anchor = AnchorStyles.None;
            UIAutomatorStatusLabel.AutoSize = true;
            UIAutomatorStatusLabel.Location = new Point(267, 150);
            UIAutomatorStatusLabel.Margin = new Padding(2, 0, 2, 0);
            UIAutomatorStatusLabel.Name = "UIAutomatorStatusLabel";
            UIAutomatorStatusLabel.Size = new Size(0, 15);
            UIAutomatorStatusLabel.TabIndex = 16;
            UIAutomatorStatusLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // XCUITestStatusLabel
            // 
            XCUITestStatusLabel.Anchor = AnchorStyles.None;
            XCUITestStatusLabel.AutoSize = true;
            XCUITestStatusLabel.Location = new Point(267, 115);
            XCUITestStatusLabel.Margin = new Padding(2, 0, 2, 0);
            XCUITestStatusLabel.Name = "XCUITestStatusLabel";
            XCUITestStatusLabel.Size = new Size(0, 15);
            XCUITestStatusLabel.TabIndex = 15;
            XCUITestStatusLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // AppiumStatusLabel
            // 
            AppiumStatusLabel.Anchor = AnchorStyles.None;
            AppiumStatusLabel.AutoSize = true;
            AppiumStatusLabel.Location = new Point(267, 80);
            AppiumStatusLabel.Margin = new Padding(2, 0, 2, 0);
            AppiumStatusLabel.Name = "AppiumStatusLabel";
            AppiumStatusLabel.Size = new Size(0, 15);
            AppiumStatusLabel.TabIndex = 14;
            AppiumStatusLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // NodeJSStatusLabel
            // 
            NodeJSStatusLabel.Anchor = AnchorStyles.None;
            NodeJSStatusLabel.AutoSize = true;
            NodeJSStatusLabel.Location = new Point(267, 45);
            NodeJSStatusLabel.Margin = new Padding(2, 0, 2, 0);
            NodeJSStatusLabel.Name = "NodeJSStatusLabel";
            NodeJSStatusLabel.Size = new Size(0, 15);
            NodeJSStatusLabel.TabIndex = 13;
            NodeJSStatusLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // FixAppiumButton
            // 
            FixAppiumButton.Anchor = AnchorStyles.None;
            FixAppiumButton.AutoSize = true;
            FixAppiumButton.Enabled = false;
            FixAppiumButton.Location = new Point(388, 75);
            FixAppiumButton.Margin = new Padding(2);
            FixAppiumButton.Name = "FixAppiumButton";
            FixAppiumButton.Size = new Size(112, 25);
            FixAppiumButton.TabIndex = 2;
            FixAppiumButton.Text = "Fix Appium";
            FixAppiumButton.UseVisualStyleBackColor = true;
            FixAppiumButton.Click += FixAppiumButton_Click;
            // 
            // FixNodeJSButton
            // 
            FixNodeJSButton.Anchor = AnchorStyles.None;
            FixNodeJSButton.AutoSize = true;
            FixNodeJSButton.Enabled = false;
            FixNodeJSButton.Location = new Point(390, 40);
            FixNodeJSButton.Margin = new Padding(2);
            FixNodeJSButton.Name = "FixNodeJSButton";
            FixNodeJSButton.Size = new Size(108, 25);
            FixNodeJSButton.TabIndex = 0;
            FixNodeJSButton.Text = "Fix NodeJS";
            FixNodeJSButton.UseVisualStyleBackColor = true;
            FixNodeJSButton.Click += FixNodeJSButton_Click;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = AnchorStyles.None;
            tableLayoutPanel1.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 174F));
            tableLayoutPanel1.Controls.Add(FixNodeJSButton, 2, 1);
            tableLayoutPanel1.Controls.Add(AppiumLabel, 0, 2);
            tableLayoutPanel1.Controls.Add(FixLabel, 2, 0);
            tableLayoutPanel1.Controls.Add(FixAppiumButton, 2, 2);
            tableLayoutPanel1.Controls.Add(StatusLabel, 1, 0);
            tableLayoutPanel1.Controls.Add(NodeJSStatusLabel, 1, 1);
            tableLayoutPanel1.Controls.Add(XCUITestLabel, 0, 3);
            tableLayoutPanel1.Controls.Add(AppiumStatusLabel, 1, 2);
            tableLayoutPanel1.Controls.Add(XCUITestStatusLabel, 1, 3);
            tableLayoutPanel1.Controls.Add(NodeJSLabel, 0, 1);
            tableLayoutPanel1.Controls.Add(UIAutomatorStatusLabel, 1, 4);
            tableLayoutPanel1.Controls.Add(FixXCUITestButton, 2, 3);
            tableLayoutPanel1.Controls.Add(FixUIAutomatorButton, 2, 4);
            tableLayoutPanel1.Controls.Add(label1, 0, 5);
            tableLayoutPanel1.Controls.Add(FixWDAButton, 2, 5);
            tableLayoutPanel1.Controls.Add(ComponentsLabel, 0, 0);
            tableLayoutPanel1.Controls.Add(UIAutomatorLabel, 0, 4);
            tableLayoutPanel1.Controls.Add(WDAStatusLabel, 1, 5);
            tableLayoutPanel1.Location = new Point(2, 5);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 6;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 16.666666F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 16.666666F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 16.666666F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 16.666666F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 16.666666F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 16.666666F));
            tableLayoutPanel1.Size = new Size(533, 212);
            tableLayoutPanel1.TabIndex = 19;
            // 
            // WDAStatusLabel
            // 
            WDAStatusLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            WDAStatusLabel.AutoSize = true;
            WDAStatusLabel.Location = new Point(182, 176);
            WDAStatusLabel.Name = "WDAStatusLabel";
            WDAStatusLabel.Size = new Size(171, 35);
            WDAStatusLabel.TabIndex = 22;
            WDAStatusLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // reInstallAllButton
            // 
            reInstallAllButton.AutoSize = true;
            reInstallAllButton.BackColor = Color.FromArgb(255, 224, 192);
            reInstallAllButton.Location = new Point(386, 226);
            reInstallAllButton.Name = "reInstallAllButton";
            reInstallAllButton.Size = new Size(140, 25);
            reInstallAllButton.TabIndex = 20;
            reInstallAllButton.Text = "Re-Install Everything";
            reInstallAllButton.UseVisualStyleBackColor = false;
            reInstallAllButton.Click += reInstallAllButton_Click;
            reInstallAllButton.MouseHover += reInstallAllButton_MouseHover;
            // 
            // showProgresscheckBox
            // 
            showProgresscheckBox.AutoSize = true;
            showProgresscheckBox.Location = new Point(207, 232);
            showProgresscheckBox.Name = "showProgresscheckBox";
            showProgresscheckBox.Size = new Size(148, 19);
            showProgresscheckBox.TabIndex = 21;
            showProgresscheckBox.Text = "Show progress window";
            showProgresscheckBox.UseVisualStyleBackColor = true;
            showProgresscheckBox.CheckedChanged += showProgresscheckBox_CheckedChanged;
            // 
            // TroubleShooter
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLightLight;
            ClientSize = new Size(538, 256);
            Controls.Add(showProgresscheckBox);
            Controls.Add(reInstallAllButton);
            Controls.Add(checkForIssues);
            Controls.Add(tableLayoutPanel1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(2);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "TroubleShooter";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Troubleshooter";
            Shown += TroubleShooter_Shown;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label NodeJSLabel;
        private Label AppiumLabel;
        private Label XCUITestLabel;
        private Label ComponentsLabel;
        private Label StatusLabel;
        private Label FixLabel;
        private Button checkForIssues;
        private Label UIAutomatorLabel;
        private Button FixWDAButton;
        private Label label1;
        private Button FixUIAutomatorButton;
        private Button FixXCUITestButton;
        private Label UIAutomatorStatusLabel;
        private Label XCUITestStatusLabel;
        private Label AppiumStatusLabel;
        private Label NodeJSStatusLabel;
        private Button FixAppiumButton;
        private Button FixNodeJSButton;
        private TableLayoutPanel tableLayoutPanel1;
        private Label WDAStatusLabel;
        private Button reInstallAllButton;
        private CheckBox showProgresscheckBox;
    }
}