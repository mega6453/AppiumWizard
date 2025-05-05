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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestRunner));
            dataGridView1 = new DataGridView();
            ActionsColumn = new DataGridViewComboBoxColumn();
            InputColumn = new DataGridViewTextBoxColumn();
            runOnceButton = new Button();
            repeatButton = new Button();
            comboBox1 = new ComboBox();
            label1 = new Label();
            helpButton = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { ActionsColumn, InputColumn });
            dataGridView1.Location = new Point(0, 41);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(902, 372);
            dataGridView1.TabIndex = 0;
            dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;
            dataGridView1.EditingControlShowing += dataGridView1_EditingControlShowing;
            // 
            // ActionsColumn
            // 
            ActionsColumn.FillWeight = 5.07614136F;
            ActionsColumn.HeaderText = "Actions";
            ActionsColumn.Items.AddRange(new object[] { "Click Element", "Send Text", "Wait for element visible", "Wait for element to vanish", "Wait in seconds" });
            ActionsColumn.Name = "ActionsColumn";
            ActionsColumn.Resizable = DataGridViewTriState.True;
            ActionsColumn.SortMode = DataGridViewColumnSortMode.Automatic;
            ActionsColumn.Width = 200;
            // 
            // InputColumn
            // 
            InputColumn.FillWeight = 194.923859F;
            InputColumn.HeaderText = "Input";
            InputColumn.Name = "InputColumn";
            InputColumn.Width = 1000;
            // 
            // runOnceButton
            // 
            runOnceButton.Location = new Point(398, 419);
            runOnceButton.Name = "runOnceButton";
            runOnceButton.Size = new Size(75, 23);
            runOnceButton.TabIndex = 1;
            runOnceButton.Text = "Run Once";
            runOnceButton.UseVisualStyleBackColor = true;
            runOnceButton.Click += runOnceButton_Click;
            // 
            // repeatButton
            // 
            repeatButton.Location = new Point(536, 419);
            repeatButton.Name = "repeatButton";
            repeatButton.Size = new Size(75, 23);
            repeatButton.TabIndex = 2;
            repeatButton.Text = "Repeat";
            repeatButton.UseVisualStyleBackColor = true;
            repeatButton.Click += repeatButton_Click;
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(133, 12);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(247, 23);
            comboBox1.TabIndex = 3;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(40, 15);
            label1.Name = "label1";
            label1.Size = new Size(76, 15);
            label1.TabIndex = 4;
            label1.Text = "Select Device";
            // 
            // helpButton
            // 
            helpButton.Location = new Point(926, 11);
            helpButton.Name = "helpButton";
            helpButton.Size = new Size(75, 23);
            helpButton.TabIndex = 5;
            helpButton.Text = "Help";
            helpButton.UseVisualStyleBackColor = true;
            helpButton.Click += helpButton_Click;
            // 
            // TestRunner
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLightLight;
            ClientSize = new Size(1013, 450);
            Controls.Add(helpButton);
            Controls.Add(label1);
            Controls.Add(comboBox1);
            Controls.Add(repeatButton);
            Controls.Add(runOnceButton);
            Controls.Add(dataGridView1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "TestRunner";
            Text = "Test Runner";
            Load += TestRunner_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dataGridView1;
        private Button runOnceButton;
        private Button repeatButton;
        private ComboBox comboBox1;
        private Label label1;
        private Button helpButton;
        private DataGridViewComboBoxColumn ActionsColumn;
        private DataGridViewTextBoxColumn InputColumn;
    }
}