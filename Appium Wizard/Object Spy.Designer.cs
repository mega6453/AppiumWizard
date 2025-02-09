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
            components = new System.ComponentModel.Container();
            pictureBox1 = new PictureBox();
            treeView1 = new TreeView();
            listView1 = new ListView();
            Property = new ColumnHeader();
            Value = new ColumnHeader();
            xpathTextbox = new RichTextBox();
            refreshButton = new Button();
            elementNumberTextbox = new TextBox();
            label1 = new Label();
            TotalElementCount = new Label();
            previousButton = new Button();
            nextButton = new Button();
            contextMenuStrip1 = new ContextMenuStrip(components);
            copyXpathToolStripMenuItem = new ToolStripMenuItem();
            addToFilterToolStripMenuItem = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            contextMenuStrip1.SuspendLayout();
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
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            listView1.Location = new Point(657, 35);
            listView1.Name = "listView1";
            listView1.Size = new Size(297, 426);
            listView1.TabIndex = 2;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Details;
            listView1.MouseUp += listView1_MouseUp;
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
            // xpathTextbox
            // 
            xpathTextbox.Location = new Point(328, 467);
            xpathTextbox.Name = "xpathTextbox";
            xpathTextbox.Size = new Size(506, 58);
            xpathTextbox.TabIndex = 4;
            xpathTextbox.Text = "";
            xpathTextbox.TextChanged += xpathTextbox_TextChanged;
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
            // elementNumberTextbox
            // 
            elementNumberTextbox.BorderStyle = BorderStyle.None;
            elementNumberTextbox.Location = new Point(840, 472);
            elementNumberTextbox.Name = "elementNumberTextbox";
            elementNumberTextbox.Size = new Size(26, 16);
            elementNumberTextbox.TabIndex = 6;
            elementNumberTextbox.TextChanged += elementNumberTextbox_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(867, 473);
            label1.Name = "label1";
            label1.Size = new Size(12, 15);
            label1.TabIndex = 7;
            label1.Text = "/";
            // 
            // TotalElementCount
            // 
            TotalElementCount.AutoSize = true;
            TotalElementCount.Location = new Point(875, 473);
            TotalElementCount.Name = "TotalElementCount";
            TotalElementCount.Size = new Size(12, 15);
            TotalElementCount.TabIndex = 8;
            TotalElementCount.Text = "-";
            // 
            // previousButton
            // 
            previousButton.AutoSize = true;
            previousButton.Location = new Point(902, 467);
            previousButton.Name = "previousButton";
            previousButton.Size = new Size(25, 25);
            previousButton.TabIndex = 9;
            previousButton.Text = "<";
            previousButton.UseVisualStyleBackColor = true;
            previousButton.Click += previousButton_Click;
            // 
            // nextButton
            // 
            nextButton.AutoSize = true;
            nextButton.Location = new Point(928, 467);
            nextButton.Name = "nextButton";
            nextButton.Size = new Size(25, 25);
            nextButton.TabIndex = 10;
            nextButton.Text = ">";
            nextButton.UseVisualStyleBackColor = true;
            nextButton.Click += nextButton_Click;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { copyXpathToolStripMenuItem, addToFilterToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(181, 70);
            // 
            // copyXpathToolStripMenuItem
            // 
            copyXpathToolStripMenuItem.Name = "copyXpathToolStripMenuItem";
            copyXpathToolStripMenuItem.Size = new Size(180, 22);
            copyXpathToolStripMenuItem.Text = "Copy XPath";
            copyXpathToolStripMenuItem.Click += copyXpathToolStripMenuItem_Click;
            // 
            // addToFilterToolStripMenuItem
            // 
            addToFilterToolStripMenuItem.Name = "addToFilterToolStripMenuItem";
            addToFilterToolStripMenuItem.Size = new Size(180, 22);
            addToFilterToolStripMenuItem.Text = "Add to Filter";
            addToFilterToolStripMenuItem.Click += addToFilterToolStripMenuItem_Click;
            // 
            // Object_Spy
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(969, 537);
            Controls.Add(nextButton);
            Controls.Add(previousButton);
            Controls.Add(TotalElementCount);
            Controls.Add(label1);
            Controls.Add(elementNumberTextbox);
            Controls.Add(refreshButton);
            Controls.Add(xpathTextbox);
            Controls.Add(listView1);
            Controls.Add(treeView1);
            Controls.Add(pictureBox1);
            Name = "Object_Spy";
            Text = "Object Spy";
            Load += Object_Spy_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private TreeView treeView1;
        private ListView listView1;
        private ColumnHeader Property;
        private ColumnHeader Value;
        private RichTextBox xpathTextbox;
        private Button refreshButton;
        private TextBox elementNumberTextbox;
        private Label label1;
        private Label TotalElementCount;
        private Button previousButton;
        private Button nextButton;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem addToFilterToolStripMenuItem;
        private ToolStripMenuItem copyXpathToolStripMenuItem;
    }
}