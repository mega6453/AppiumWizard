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
            saveAsButton = new Button();
            filePathToolTip = new ToolTip(components);
            ((System.ComponentModel.ISupportInitialize)commandGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)propertyGridView).BeginInit();
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
            commandGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            commandGridView.Size = new Size(668, 332);
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
            checkboxColumn.HeaderText = "Check";
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
            propertyGridView.Location = new Point(700, 41);
            propertyGridView.Name = "propertyGridView";
            propertyGridView.RowHeadersVisible = false;
            propertyGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            propertyGridView.Size = new Size(363, 161);
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
            comboBoxActions.Items.AddRange(new object[] { "Set Device", "Click Element", "Send Text", "Wait for element visible", "Wait for element to vanish", "Sleep", "Install App", "Launch App", "Kill App", "Uninstall App", "Execute Script", "Take Screenshot", "Device Action" });
            comboBoxActions.Location = new Point(398, 12);
            comboBoxActions.Name = "comboBoxActions";
            comboBoxActions.Size = new Size(282, 23);
            comboBoxActions.TabIndex = 2;
            comboBoxActions.SelectedIndexChanged += ComboBoxActions_SelectedIndexChanged;
            // 
            // repeatButton
            // 
            repeatButton.AutoSize = true;
            repeatButton.Location = new Point(536, 391);
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
            runOnceButton.Location = new Point(398, 391);
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
            saveButton.Location = new Point(39, 12);
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
            loadButton.Location = new Point(201, 12);
            loadButton.Name = "loadButton";
            loadButton.Size = new Size(76, 25);
            loadButton.TabIndex = 7;
            loadButton.Text = "Load Script";
            loadButton.UseVisualStyleBackColor = true;
            loadButton.Click += loadButton_Click;
            // 
            // helpButton
            // 
            helpButton.Location = new Point(988, 3);
            helpButton.Name = "helpButton";
            helpButton.Size = new Size(75, 23);
            helpButton.TabIndex = 8;
            helpButton.Text = "Help";
            helpButton.UseVisualStyleBackColor = true;
            helpButton.Click += helpButton_Click;
            // 
            // saveAsButton
            // 
            saveAsButton.AutoSize = true;
            saveAsButton.Location = new Point(120, 12);
            saveAsButton.Name = "saveAsButton";
            saveAsButton.Size = new Size(75, 25);
            saveAsButton.TabIndex = 9;
            saveAsButton.Text = "Save As";
            saveAsButton.UseVisualStyleBackColor = true;
            saveAsButton.Click += saveAsButton_Click;
            // 
            // TestRunner
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLightLight;
            ClientSize = new Size(1075, 424);
            Controls.Add(saveAsButton);
            Controls.Add(helpButton);
            Controls.Add(loadButton);
            Controls.Add(saveButton);
            Controls.Add(repeatButton);
            Controls.Add(runOnceButton);
            Controls.Add(comboBoxActions);
            Controls.Add(propertyGridView);
            Controls.Add(commandGridView);
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
    }
}