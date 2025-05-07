namespace Appium_Wizard
{
    partial class MainForm
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
            commandGridView = new DataGridView();
            Command = new DataGridViewTextBoxColumn();
            propertyGridView = new DataGridView();
            Property = new DataGridViewTextBoxColumn();
            Value = new DataGridViewTextBoxColumn();
            comboBoxActions = new ComboBox();
            repeatButton = new Button();
            runOnceButton = new Button();
            ((System.ComponentModel.ISupportInitialize)commandGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)propertyGridView).BeginInit();
            SuspendLayout();
            // 
            // commandGridView
            // 
            commandGridView.AllowUserToAddRows = false;
            commandGridView.AllowUserToResizeColumns = false;
            commandGridView.AllowUserToResizeRows = false;
            commandGridView.BackgroundColor = SystemColors.ControlLight;
            commandGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            commandGridView.Columns.AddRange(new DataGridViewColumn[] { Command });
            commandGridView.Location = new Point(12, 41);
            commandGridView.Name = "commandGridView";
            commandGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            commandGridView.Size = new Size(788, 332);
            commandGridView.TabIndex = 0;
            commandGridView.SelectionChanged += DataGridView1_SelectionChanged;
            commandGridView.UserDeletingRow += DataGridView1_UserDeletingRow;
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
            propertyGridView.AllowUserToResizeColumns = false;
            propertyGridView.AllowUserToResizeRows = false;
            propertyGridView.BackgroundColor = SystemColors.ControlLight;
            propertyGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            propertyGridView.Columns.AddRange(new DataGridViewColumn[] { Property, Value });
            propertyGridView.EditMode = DataGridViewEditMode.EditOnEnter;
            propertyGridView.Location = new Point(823, 41);
            propertyGridView.Name = "propertyGridView";
            propertyGridView.RowHeadersVisible = false;
            propertyGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            propertyGridView.Size = new Size(240, 213);
            propertyGridView.TabIndex = 1;
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
            comboBoxActions.Items.AddRange(new object[] { "Set Device", "Click Element", "Send Text", "Wait for element visible", "Wait for element to vanish", "Wait in seconds", "Install App", "Launch App", "Uninstall App", "Execute Script", "Take Screenshot", "Device Action" });
            comboBoxActions.Location = new Point(518, 12);
            comboBoxActions.Name = "comboBoxActions";
            comboBoxActions.Size = new Size(282, 23);
            comboBoxActions.TabIndex = 2;
            comboBoxActions.SelectedIndexChanged += ComboBoxActions_SelectedIndexChanged;
            // 
            // repeatButton
            // 
            repeatButton.Location = new Point(536, 391);
            repeatButton.Name = "repeatButton";
            repeatButton.Size = new Size(75, 23);
            repeatButton.TabIndex = 4;
            repeatButton.Text = "Repeat";
            repeatButton.UseVisualStyleBackColor = true;
            repeatButton.Click += repeatButton_Click;
            // 
            // runOnceButton
            // 
            runOnceButton.Location = new Point(398, 391);
            runOnceButton.Name = "runOnceButton";
            runOnceButton.Size = new Size(75, 23);
            runOnceButton.TabIndex = 3;
            runOnceButton.Text = "Run Once";
            runOnceButton.UseVisualStyleBackColor = true;
            runOnceButton.Click += runOnceButton_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLightLight;
            ClientSize = new Size(1075, 450);
            Controls.Add(repeatButton);
            Controls.Add(runOnceButton);
            Controls.Add(comboBoxActions);
            Controls.Add(propertyGridView);
            Controls.Add(commandGridView);
            Name = "MainForm";
            Text = "MainForm";
            Load += MainForm_Load;
            ((System.ComponentModel.ISupportInitialize)commandGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)propertyGridView).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView commandGridView;
        private DataGridView propertyGridView;
        private ComboBox comboBoxActions;
        private Button repeatButton;
        private Button runOnceButton;
        private DataGridViewTextBoxColumn Property;
        private DataGridViewTextBoxColumn Value;
        private DataGridViewTextBoxColumn Command;
    }
}