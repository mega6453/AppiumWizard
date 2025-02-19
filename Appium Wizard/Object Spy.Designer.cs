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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Object_Spy));
            pictureBox1 = new PictureBox();
            treeView1 = new TreeView();
            listView1 = new ListView();
            Property = new ColumnHeader();
            Value = new ColumnHeader();
            filterTextbox = new RichTextBox();
            refreshButton = new Button();
            elementNumberTextbox = new TextBox();
            label1 = new Label();
            TotalElementCount = new Label();
            previousButton = new Button();
            nextButton = new Button();
            listViewContextMenuStrip = new ContextMenuStrip(components);
            copyXpathToolStripMenuItem = new ToolStripMenuItem();
            addToFilterToolStripMenuItem = new ToolStripMenuItem();
            treeViewContextMenuStrip = new ContextMenuStrip(components);
            copyUniqueXpathToolStripMenuItem = new ToolStripMenuItem();
            addUniqueXpathToFilterToolStripMenuItem = new ToolStripMenuItem();
            filterLabel = new Label();
            pictureBoxContextMenuStrip = new ContextMenuStrip(components);
            copyUniqueXPathToolStripMenuItem1 = new ToolStripMenuItem();
            addUniqueXpathToFilterToolStripMenuItem1 = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            listViewContextMenuStrip.SuspendLayout();
            treeViewContextMenuStrip.SuspendLayout();
            pictureBoxContextMenuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(12, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(275, 459);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.MouseClick += pictureBox1_MouseClick;
            pictureBox1.MouseUp += pictureBox1_MouseUp;
            // 
            // treeView1
            // 
            treeView1.Location = new Point(326, 35);
            treeView1.Name = "treeView1";
            treeView1.Size = new Size(483, 426);
            treeView1.TabIndex = 1;
            treeView1.AfterSelect += TreeView_AfterSelect;
            treeView1.MouseUp += treeView1_MouseUp;
            // 
            // listView1
            // 
            listView1.Columns.AddRange(new ColumnHeader[] { Property, Value });
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            listView1.Location = new Point(815, 35);
            listView1.Name = "listView1";
            listView1.Size = new Size(354, 426);
            listView1.TabIndex = 2;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Details;
            listView1.MouseUp += listView1_MouseUp;
            // 
            // Property
            // 
            Property.Text = "Property";
            Property.Width = 150;
            // 
            // Value
            // 
            Value.Text = "Value";
            Value.Width = 500;
            // 
            // filterTextbox
            // 
            filterTextbox.Location = new Point(328, 484);
            filterTextbox.Name = "filterTextbox";
            filterTextbox.Size = new Size(642, 50);
            filterTextbox.TabIndex = 4;
            filterTextbox.Text = "";
            filterTextbox.TextChanged += xpathTextbox_TextChanged;
            // 
            // refreshButton
            // 
            refreshButton.AutoSize = true;
            refreshButton.Location = new Point(326, 7);
            refreshButton.Name = "refreshButton";
            refreshButton.Size = new Size(97, 25);
            refreshButton.TabIndex = 5;
            refreshButton.Text = "Refresh Screen";
            refreshButton.UseVisualStyleBackColor = true;
            refreshButton.Click += refreshButton_Click;
            // 
            // elementNumberTextbox
            // 
            elementNumberTextbox.BorderStyle = BorderStyle.None;
            elementNumberTextbox.Location = new Point(976, 485);
            elementNumberTextbox.Name = "elementNumberTextbox";
            elementNumberTextbox.Size = new Size(26, 16);
            elementNumberTextbox.TabIndex = 6;
            elementNumberTextbox.TextChanged += elementNumberTextbox_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(1003, 486);
            label1.Name = "label1";
            label1.Size = new Size(12, 15);
            label1.TabIndex = 7;
            label1.Text = "/";
            // 
            // TotalElementCount
            // 
            TotalElementCount.AutoSize = true;
            TotalElementCount.Location = new Point(1011, 486);
            TotalElementCount.Name = "TotalElementCount";
            TotalElementCount.Size = new Size(12, 15);
            TotalElementCount.TabIndex = 8;
            TotalElementCount.Text = "-";
            // 
            // previousButton
            // 
            previousButton.AutoSize = true;
            previousButton.Location = new Point(1038, 480);
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
            nextButton.Location = new Point(1064, 480);
            nextButton.Name = "nextButton";
            nextButton.Size = new Size(25, 25);
            nextButton.TabIndex = 10;
            nextButton.Text = ">";
            nextButton.UseVisualStyleBackColor = true;
            nextButton.Click += nextButton_Click;
            // 
            // listViewContextMenuStrip
            // 
            listViewContextMenuStrip.Items.AddRange(new ToolStripItem[] { copyXpathToolStripMenuItem, addToFilterToolStripMenuItem });
            listViewContextMenuStrip.Name = "contextMenuStrip1";
            listViewContextMenuStrip.Size = new Size(140, 48);
            // 
            // copyXpathToolStripMenuItem
            // 
            copyXpathToolStripMenuItem.Name = "copyXpathToolStripMenuItem";
            copyXpathToolStripMenuItem.Size = new Size(139, 22);
            copyXpathToolStripMenuItem.Text = "Copy XPath";
            copyXpathToolStripMenuItem.Click += copyXpathToolStripMenuItem_Click;
            // 
            // addToFilterToolStripMenuItem
            // 
            addToFilterToolStripMenuItem.Name = "addToFilterToolStripMenuItem";
            addToFilterToolStripMenuItem.Size = new Size(139, 22);
            addToFilterToolStripMenuItem.Text = "Add to Filter";
            addToFilterToolStripMenuItem.Click += addToFilterToolStripMenuItem_Click;
            // 
            // treeViewContextMenuStrip
            // 
            treeViewContextMenuStrip.Items.AddRange(new ToolStripItem[] { copyUniqueXpathToolStripMenuItem, addUniqueXpathToFilterToolStripMenuItem });
            treeViewContextMenuStrip.Name = "treeViewContextMenuStrip";
            treeViewContextMenuStrip.Size = new Size(215, 48);
            // 
            // copyUniqueXpathToolStripMenuItem
            // 
            copyUniqueXpathToolStripMenuItem.Name = "copyUniqueXpathToolStripMenuItem";
            copyUniqueXpathToolStripMenuItem.Size = new Size(214, 22);
            copyUniqueXpathToolStripMenuItem.Text = "Copy Unique Xpath";
            copyUniqueXpathToolStripMenuItem.Click += copyUniqueXpathToolStripMenuItem_Click;
            // 
            // addUniqueXpathToFilterToolStripMenuItem
            // 
            addUniqueXpathToFilterToolStripMenuItem.Name = "addUniqueXpathToFilterToolStripMenuItem";
            addUniqueXpathToFilterToolStripMenuItem.Size = new Size(214, 22);
            addUniqueXpathToFilterToolStripMenuItem.Text = "Add Unique Xpath to Filter";
            addUniqueXpathToFilterToolStripMenuItem.Click += addUniqueXpathToFilterToolStripMenuItem_Click;
            // 
            // filterLabel
            // 
            filterLabel.AutoSize = true;
            filterLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            filterLabel.Location = new Point(326, 468);
            filterLabel.Name = "filterLabel";
            filterLabel.Size = new Size(132, 15);
            filterLabel.TabIndex = 13;
            filterLabel.Text = "Filter (XPath Validator)";
            // 
            // pictureBoxContextMenuStrip
            // 
            pictureBoxContextMenuStrip.Items.AddRange(new ToolStripItem[] { copyUniqueXPathToolStripMenuItem1, addUniqueXpathToFilterToolStripMenuItem1 });
            pictureBoxContextMenuStrip.Name = "pictureBoxContextMenuStrip";
            pictureBoxContextMenuStrip.Size = new Size(215, 70);
            // 
            // copyUniqueXPathToolStripMenuItem1
            // 
            copyUniqueXPathToolStripMenuItem1.Name = "copyUniqueXPathToolStripMenuItem1";
            copyUniqueXPathToolStripMenuItem1.Size = new Size(214, 22);
            copyUniqueXPathToolStripMenuItem1.Text = "Copy Unique XPath";
            copyUniqueXPathToolStripMenuItem1.Click += copyUniqueXPathToolStripMenuItem1_Click;
            // 
            // addUniqueXpathToFilterToolStripMenuItem1
            // 
            addUniqueXpathToFilterToolStripMenuItem1.Name = "addUniqueXpathToFilterToolStripMenuItem1";
            addUniqueXpathToFilterToolStripMenuItem1.Size = new Size(214, 22);
            addUniqueXpathToFilterToolStripMenuItem1.Text = "Add Unique Xpath to Filter";
            addUniqueXpathToFilterToolStripMenuItem1.Click += addUniqueXpathToFilterToolStripMenuItem1_Click;
            // 
            // Object_Spy
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1181, 562);
            Controls.Add(filterLabel);
            Controls.Add(nextButton);
            Controls.Add(previousButton);
            Controls.Add(TotalElementCount);
            Controls.Add(label1);
            Controls.Add(elementNumberTextbox);
            Controls.Add(refreshButton);
            Controls.Add(filterTextbox);
            Controls.Add(listView1);
            Controls.Add(treeView1);
            Controls.Add(pictureBox1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Object_Spy";
            Text = "Object Spy";
            WindowState = FormWindowState.Maximized;
            Load += Object_Spy_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            listViewContextMenuStrip.ResumeLayout(false);
            treeViewContextMenuStrip.ResumeLayout(false);
            pictureBoxContextMenuStrip.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private TreeView treeView1;
        private ListView listView1;
        private ColumnHeader Property;
        private ColumnHeader Value;
        private RichTextBox filterTextbox;
        private Button refreshButton;
        private TextBox elementNumberTextbox;
        private Label label1;
        private Label TotalElementCount;
        private Button previousButton;
        private Button nextButton;
        private ContextMenuStrip listViewContextMenuStrip;
        private ToolStripMenuItem addToFilterToolStripMenuItem;
        private ToolStripMenuItem copyXpathToolStripMenuItem;
        private ContextMenuStrip treeViewContextMenuStrip;
        private ToolStripMenuItem copyUniqueXpathToolStripMenuItem;
        private ToolStripMenuItem addUniqueXpathToFilterToolStripMenuItem;
        private Label filterLabel;
        private ContextMenuStrip pictureBoxContextMenuStrip;
        private ToolStripMenuItem copyUniqueXPathToolStripMenuItem1;
        private ToolStripMenuItem addUniqueXpathToFilterToolStripMenuItem1;
    }
}