namespace Appium_Wizard
{
    partial class Object_Spy
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
            pictureBox1 = new PictureBox();
            treeView1 = new TreeView();
            listView1 = new ListView();
            Property = new ColumnHeader();
            Value = new ColumnHeader();
            richTextBox1 = new RichTextBox();
            refreshButton = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(13, 35);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(275, 426);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.MouseClick += pictureBox1_MouseClick;
            // 
            // treeView1
            // 
            treeView1.Location = new Point(327, 35);
            treeView1.Name = "treeView1";
            treeView1.Size = new Size(313, 426);
            treeView1.TabIndex = 1;
            treeView1.AfterSelect += TreeView_AfterSelect;
            // 
            // listView1
            // 
            listView1.Columns.AddRange(new ColumnHeader[] { Property, Value });
            listView1.Location = new Point(657, 35);
            listView1.Name = "listView1";
            listView1.Size = new Size(297, 426);
            listView1.TabIndex = 2;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Details;
            // 
            // Property
            // 
            Property.Text = "Property";
            Property.Width = 100;
            // 
            // Value
            // 
            Value.Text = "Value";
            Value.Width = 500;
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(328, 467);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(627, 56);
            richTextBox1.TabIndex = 4;
            richTextBox1.Text = "";
            // 
            // refreshButton
            // 
            refreshButton.Location = new Point(191, 6);
            refreshButton.Name = "refreshButton";
            refreshButton.Size = new Size(97, 23);
            refreshButton.TabIndex = 5;
            refreshButton.Text = "Refresh Screen";
            refreshButton.UseVisualStyleBackColor = true;
            refreshButton.Click += refreshButton_Click;
            // 
            // Object_Spy
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(969, 537);
            Controls.Add(refreshButton);
            Controls.Add(richTextBox1);
            Controls.Add(listView1);
            Controls.Add(treeView1);
            Controls.Add(pictureBox1);
            Name = "Object_Spy";
            Text = "Object Spy";
            Load += Object_Spy_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pictureBox1;
        private TreeView treeView1;
        private ListView listView1;
        private ColumnHeader Property;
        private ColumnHeader Value;
        private RichTextBox richTextBox1;
        private Button refreshButton;
    }
}