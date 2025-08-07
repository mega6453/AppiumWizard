namespace Appium_Wizard
{
    partial class TestRunner
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestRunner));
            commandGridView = new DataGridView();
            checkboxColumn = new DataGridViewCheckBoxColumn();
            Command = new DataGridViewTextBoxColumn();
            propertyGridView = new DataGridView();
            Property = new DataGridViewTextBoxColumn();
            Value = new DataGridViewTextBoxColumn();
            comboBoxActions = new ComboBox();
            repeatButton = new Button();
            runOnceButton = new Button();
            saveButton = new Button();
            loadButton = new Button();
            helpButton = new Button();
            filePathToolTip = new ToolTip(components);
            label5 = new Label();
            repeatCountLabel = new Label();
            openReportButton = new Button();
            DropDownButton = new Button();
            contextMenuStrip1 = new ContextMenuStrip(components);
            openReportFolderToolStripMenuItem = new ToolStripMenuItem();
            saveDownButton = new Button();
            contextMenuStrip2 = new ContextMenuStrip(components);
            saveAsToolStripMenuItem = new ToolStripMenuItem();
            newButton = new Button();
            propertyInfo = new Label();
            panel1 = new Panel();
            panel2 = new Panel();
            currentExecution = new Label();
            label2 = new Label();
            ((System.ComponentModel.ISupportInitialize)commandGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)propertyGridView).BeginInit();
            contextMenuStrip1.SuspendLayout();
            contextMenuStrip2.SuspendLayout();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // commandGridView
            // 
            commandGridView.AllowDrop = true;
            commandGridView.AllowUserToAddRows = false;
            commandGridView.AllowUserToResizeColumns = false;
            commandGridView.AllowUserToResizeRows = false;
            commandGridView.BackgroundColor = SystemColors.ControlLight;
            commandGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            commandGridView.Columns.AddRange(new DataGridViewColumn[] { checkboxColumn, Command });
            commandGridView.Location = new Point(12, 41);
            commandGridView.Name = "commandGridView";
            commandGridView.RowHeadersVisible = false;
            commandGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            commandGridView.Size = new Size(632, 332);
            commandGridView.TabIndex = 0;
            commandGridView.CellValueChanged += commandGridView_CellValueChanged;
            commandGridView.SelectionChanged += DataGridView1_SelectionChanged;
            commandGridView.UserDeletingRow += DataGridView1_UserDeletingRow;
            commandGridView.DragDrop += commandGridView_DragDrop;
            commandGridView.DragOver += commandGridView_DragOver;
            commandGridView.MouseDown += commandGridView_MouseDown;
            // 
            // checkboxColumn
            // 
            checkboxColumn.HeaderText = "Y/N";
            checkboxColumn.Name = "checkboxColumn";
            checkboxColumn.Resizable = DataGridViewTriState.False;
            checkboxColumn.Width = 50;
            // 
            // Command
            // 
            Command.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Command.HeaderText = "Command";
            Command.Name = "Command";
            Command.ReadOnly = true;
            Command.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // propertyGridView
            // 
            propertyGridView.AllowUserToAddRows = false;
            propertyGridView.AllowUserToDeleteRows = false;
            propertyGridView.AllowUserToResizeColumns = false;
            propertyGridView.AllowUserToResizeRows = false;
            propertyGridView.BackgroundColor = SystemColors.ControlLight;
            propertyGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            propertyGridView.Columns.AddRange(new DataGridViewColumn[] { Property, Value });
            propertyGridView.EditMode = DataGridViewEditMode.EditOnEnter;
            propertyGridView.Location = new Point(663, 41);
            propertyGridView.Name = "propertyGridView";
            propertyGridView.RowHeadersVisible = false;
            propertyGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            propertyGridView.Size = new Size(439, 161);
            propertyGridView.TabIndex = 1;
            propertyGridView.CellBeginEdit += dataGridView1_CellBeginEdit;
            propertyGridView.CellValueChanged += DataGridView2_CellValueChanged;
            // 
            // Property
            // 
            Property.HeaderText = "Property";
            Property.Name = "Property";
            Property.ReadOnly = true;
            Property.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // Value
            // 
            Value.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Value.HeaderText = "Value";
            Value.Name = "Value";
            // 
            // comboBoxActions
            // 
            comboBoxActions.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxActions.FormattingEnabled = true;
            comboBoxActions.Items.AddRange(new object[] { "Set Device", "Click Element", "Click on coordinates", "Send Text", "Send Text With Random Values", "Send Text Without Element", "Wait for element visible", "Wait for element to vanish", "Sleep", "Install App", "Launch App", "Kill App", "Uninstall App", "Take Screenshot", "Device Action" });
            comboBoxActions.Location = new Point(439, 14);
            comboBoxActions.Name = "comboBoxActions";
            comboBoxActions.Size = new Size(205, 23);
            comboBoxActions.TabIndex = 2;
            comboBoxActions.SelectedIndexChanged += ComboBoxActions_SelectedIndexChanged;
            // 
            // repeatButton
            // 
            repeatButton.AutoSize = true;
            repeatButton.Location = new Point(486, 437);
            repeatButton.Name = "repeatButton";
            repeatButton.Size = new Size(75, 25);
            repeatButton.TabIndex = 4;
            repeatButton.Text = "Repeat";
            repeatButton.UseVisualStyleBackColor = true;
            repeatButton.Click += repeatButton_Click;
            // 
            // runOnceButton
            // 
            runOnceButton.AutoSize = true;
            runOnceButton.Location = new Point(395, 437);
            runOnceButton.Name = "runOnceButton";
            runOnceButton.Size = new Size(75, 25);
            runOnceButton.TabIndex = 3;
            runOnceButton.Text = "Run Once";
            runOnceButton.UseVisualStyleBackColor = true;
            runOnceButton.Click += runOnceButton_Click;
            // 
            // saveButton
            // 
            saveButton.AutoSize = true;
            saveButton.Location = new Point(93, 12);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(75, 25);
            saveButton.TabIndex = 6;
            saveButton.Text = "Save Script";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Click += saveButton_Click;
            saveButton.MouseHover += saveButton_MouseHover;
            // 
            // loadButton
            // 
            loadButton.AutoSize = true;
            loadButton.Location = new Point(206, 12);
            loadButton.Name = "loadButton";
            loadButton.Size = new Size(76, 25);
            loadButton.TabIndex = 7;
            loadButton.Text = "Load Script";
            loadButton.UseVisualStyleBackColor = true;
            loadButton.Click += loadButton_Click;
            // 
            // helpButton
            // 
            helpButton.AutoSize = true;
            helpButton.Location = new Point(1027, 5);
            helpButton.Name = "helpButton";
            helpButton.Size = new Size(75, 25);
            helpButton.TabIndex = 8;
            helpButton.Text = "Help";
            helpButton.UseVisualStyleBackColor = true;
            helpButton.Click += helpButton_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label5.ForeColor = Color.Brown;
            label5.Location = new Point(663, 342);
            label5.Margin = new Padding(2, 0, 2, 0);
            label5.Name = "label5";
            label5.Size = new Size(364, 30);
            label5.TabIndex = 11;
            label5.Text = "NOTE : THERE IS NO VALIDATION DONE ON THE GIVEN VALUES, \r\nSO MAKE SURE YOU ARE ENTERING VALID VALUES.";
            // 
            // repeatCountLabel
            // 
            repeatCountLabel.AutoSize = true;
            repeatCountLabel.ForeColor = Color.FromArgb(192, 0, 0);
            repeatCountLabel.Location = new Point(567, 442);
            repeatCountLabel.Name = "repeatCountLabel";
            repeatCountLabel.Size = new Size(0, 15);
            repeatCountLabel.TabIndex = 12;
            // 
            // openReportButton
            // 
            openReportButton.AutoSize = true;
            openReportButton.Location = new Point(288, 12);
            openReportButton.Name = "openReportButton";
            openReportButton.Size = new Size(84, 25);
            openReportButton.TabIndex = 13;
            openReportButton.Text = "Open Report";
            openReportButton.UseVisualStyleBackColor = true;
            openReportButton.Click += openReportButton_Click;
            // 
            // DropDownButton
            // 
            DropDownButton.AutoSize = true;
            DropDownButton.Location = new Point(374, 12);
            DropDownButton.Name = "DropDownButton";
            DropDownButton.Size = new Size(27, 25);
            DropDownButton.TabIndex = 14;
            DropDownButton.Text = "▼";
            DropDownButton.UseVisualStyleBackColor = true;
            DropDownButton.Click += DropDownButton_Click;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { openReportFolderToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(181, 26);
            // 
            // openReportFolderToolStripMenuItem
            // 
            openReportFolderToolStripMenuItem.Name = "openReportFolderToolStripMenuItem";
            openReportFolderToolStripMenuItem.Size = new Size(180, 22);
            openReportFolderToolStripMenuItem.Text = "Open Reports folder";
            openReportFolderToolStripMenuItem.Click += openReportFolderToolStripMenuItem_Click;
            // 
            // saveDownButton
            // 
            saveDownButton.AutoSize = true;
            saveDownButton.Location = new Point(171, 12);
            saveDownButton.Name = "saveDownButton";
            saveDownButton.Size = new Size(27, 25);
            saveDownButton.TabIndex = 15;
            saveDownButton.Text = "▼";
            saveDownButton.UseVisualStyleBackColor = true;
            saveDownButton.Click += button1_Click;
            // 
            // contextMenuStrip2
            // 
            contextMenuStrip2.Items.AddRange(new ToolStripItem[] { saveAsToolStripMenuItem });
            contextMenuStrip2.Name = "contextMenuStrip2";
            contextMenuStrip2.Size = new Size(115, 26);
            // 
            // saveAsToolStripMenuItem
            // 
            saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            saveAsToolStripMenuItem.Size = new Size(114, 22);
            saveAsToolStripMenuItem.Text = "Save As";
            saveAsToolStripMenuItem.Click += saveAsToolStripMenuItem_Click;
            // 
            // newButton
            // 
            newButton.AutoSize = true;
            newButton.Location = new Point(12, 12);
            newButton.Name = "newButton";
            newButton.Size = new Size(75, 25);
            newButton.TabIndex = 16;
            newButton.Text = "New";
            newButton.UseVisualStyleBackColor = true;
            newButton.Click += newButton_Click;
            // 
            // propertyInfo
            // 
            propertyInfo.Dock = DockStyle.Fill;
            propertyInfo.Location = new Point(0, 0);
            propertyInfo.Name = "propertyInfo";
            propertyInfo.Size = new Size(437, 133);
            propertyInfo.TabIndex = 17;
            // 
            // panel1
            // 
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel1.Controls.Add(propertyInfo);
            panel1.Location = new Point(663, 205);
            panel1.Name = "panel1";
            panel1.Size = new Size(439, 135);
            panel1.TabIndex = 18;
            // 
            // panel2
            // 
            panel2.BackColor = SystemColors.ControlLightLight;
            panel2.BorderStyle = BorderStyle.FixedSingle;
            panel2.Controls.Add(currentExecution);
            panel2.Location = new Point(12, 392);
            panel2.Name = "panel2";
            panel2.Size = new Size(632, 35);
            panel2.TabIndex = 19;
            // 
            // currentExecution
            // 
            currentExecution.AutoSize = true;
            currentExecution.BackColor = Color.Transparent;
            currentExecution.ForeColor = Color.FromArgb(192, 0, 0);
            currentExecution.Location = new Point(3, 10);
            currentExecution.Name = "currentExecution";
            currentExecution.Size = new Size(0, 15);
            currentExecution.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 376);
            label2.Name = "label2";
            label2.Size = new Size(97, 15);
            label2.TabIndex = 0;
            label2.Text = "Execution Status:";
            // 
            // TestRunner
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLightLight;
            ClientSize = new Size(1114, 474);
            Controls.Add(label2);
            Controls.Add(newButton);
            Controls.Add(saveDownButton);
            Controls.Add(DropDownButton);
            Controls.Add(openReportButton);
            Controls.Add(repeatCountLabel);
            Controls.Add(label5);
            Controls.Add(helpButton);
            Controls.Add(loadButton);
            Controls.Add(saveButton);
            Controls.Add(repeatButton);
            Controls.Add(runOnceButton);
            Controls.Add(comboBoxActions);
            Controls.Add(propertyGridView);
            Controls.Add(commandGridView);
            Controls.Add(panel1);
            Controls.Add(panel2);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "TestRunner";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Test Runner";
            FormClosing += TestRunner_FormClosing;
            Load += MainForm_Load;
            Shown += TestRunner_Shown;
            ((System.ComponentModel.ISupportInitialize)commandGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)propertyGridView).EndInit();
            contextMenuStrip1.ResumeLayout(false);
            contextMenuStrip2.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView commandGridView;
        private DataGridView propertyGridView;
        private ComboBox comboBoxActions;
        private Button repeatButton;
        private Button runOnceButton;
        private DataGridViewTextBoxColumn Property;
        private DataGridViewTextBoxColumn Value;
        private Button saveButton;
        private Button loadButton;
        private Button helpButton;
        private Button saveAsButton;
        private ToolTip filePathToolTip;
        private DataGridViewCheckBoxColumn checkboxColumn;
        private DataGridViewTextBoxColumn Command;
        private Label label5;
        private Label label1;
        private Label repeatCountLabel;
        private Button openReportButton;
        private Button DropDownButton;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem openReportFolderToolStripMenuItem;
        private Button saveDownButton;
        private ContextMenuStrip contextMenuStrip2;
        private ToolStripMenuItem saveAsToolStripMenuItem;
        private Button newButton;
        private Label propertyInfo;
        private Panel panel1;
        private Panel panel2;
        private Label currentExecution;
        private Label label2;
    }
}