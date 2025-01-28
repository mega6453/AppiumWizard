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
            closeButton = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(12, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(275, 426);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.MouseClick += pictureBox1_MouseClick;
            // 
            // treeView1
            // 
            treeView1.Location = new Point(295, 12);
            treeView1.Name = "treeView1";
            treeView1.Size = new Size(313, 426);
            treeView1.TabIndex = 1;
            treeView1.AfterSelect += TreeView_AfterSelect;
            // 
            // listView1
            // 
            listView1.Columns.AddRange(new ColumnHeader[] { Property, Value });
            listView1.Location = new Point(625, 12);
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
            // closeButton
            // 
            closeButton.Location = new Point(847, 477);
            closeButton.Name = "closeButton";
            closeButton.Size = new Size(75, 23);
            closeButton.TabIndex = 3;
            closeButton.Text = "Close";
            closeButton.UseVisualStyleBackColor = true;
            closeButton.Click += closeButton_Click;
            // 
            // Object_Spy
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(947, 512);
            Controls.Add(closeButton);
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
        private Button closeButton;
    }
}