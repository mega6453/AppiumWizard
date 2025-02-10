using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;

namespace Appium_Wizard
{
    public partial class Object_Spy : Form
    {
        int port, width, height;
        Image screenshot;
        string xmlContent;
        bool isAndroid;
        public Object_Spy(string os, int port, int width, int height)
        {
            this.port = port;
            this.width = width;
            this.height = height;
            InitializeComponent();
            if (os.Equals("Android"))
            {
                isAndroid = true;
            }
            else
            {
                isAndroid = false;
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            float xScale = (float)screenshot.Width / pictureBox1.ClientSize.Width;
            float yScale = (float)screenshot.Height / pictureBox1.ClientSize.Height;

            // Calculate the actual coordinates on the image
            int actualX = (int)(e.X * xScale);
            int actualY = (int)(e.Y * yScale);

            // Draw the rectangle on the image using the actual coordinates
            DrawRectangleOnImage(screenshot, actualX, actualY, 100, 100);
        }

        private async void Object_Spy_Load(object sender, EventArgs e)
        {
            FetchScreen();
        }

        private async void FetchScreen()
        {
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Owner = this;
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Object Spy", "Please wait while fetching screen...", 20);
            pictureBox1.Size = new Size(width, height);
            string url = "http://localhost:" + port;
            await Task.Run(() =>
            {
                if (isAndroid)
                {
                    screenshot = AndroidAPIMethods.TakeScreenshot(port);
                }
                else
                {
                    screenshot = iOSAPIMethods.TakeScreenshot(url);
                }
            });
            commonProgress.UpdateStepLabel("Object Spy", "Please wait while fetching screen...", 50);
            if (screenshot == null)
            {
                MessageBox.Show("Failed to retreive screenshot.", "Object Spy", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                pictureBox1.Image = screenshot;
                string xml = "empty";
                await Task.Run(() =>
                {
                    if (isAndroid)
                    {
                        xml = AndroidAPIMethods.GetPageSource(port);
                    }
                    else
                    {
                        xml = iOSAPIMethods.GetPageSource(port);
                    }
                });
                commonProgress.UpdateStepLabel("Object Spy", "Please wait while fetching screen...", 70);
                if (xml.Equals("empty"))
                {
                    MessageBox.Show("Failed to fetch page source.", "Object Spy", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    await Task.Run(() =>
                    {
                        LoadXmlToTreeView(xml);
                    });
                    commonProgress.UpdateStepLabel("Object Spy", "Please wait while fetching screen...", 90);
                    listView1.Items.Clear();
                    treeView1.ExpandAll();
                }
            }
            commonProgress.Close();
        }


        private void LoadXmlToTreeView(string xmlContent)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlContent);

            TreeNode rootNode = new TreeNode(xmlDoc.DocumentElement.Name);
            if (treeView1.InvokeRequired)
            {
                treeView1.Invoke(new Action(() => treeView1.Nodes.Clear()));
                treeView1.Invoke(new Action(() => treeView1.Nodes.Add(rootNode)));
            }
            else
            {
                treeView1.Nodes.Clear();
                treeView1.Nodes.Add(rootNode);
            }
            AddXmlNodeToTreeNode(xmlDoc.DocumentElement, rootNode);
        }

        private void AddXmlNodeToTreeNode(XmlNode xmlNode, TreeNode treeNode)
        {
            foreach (XmlNode childNode in xmlNode.ChildNodes)
            {
                if (childNode.NodeType == XmlNodeType.Element)
                {
                    // Assign the XmlNode to the Tag property of the TreeNode
                    TreeNode childTreeNode = new TreeNode(childNode.Name) { Tag = childNode };
                    // Ensure the node addition happens on the UI thread
                    if (treeNode.TreeView.InvokeRequired)
                    {
                        treeNode.TreeView.Invoke(new Action(() => treeNode.Nodes.Add(childTreeNode)));
                    }
                    else
                    {
                        treeNode.Nodes.Add(childTreeNode);
                    }
                    AddXmlNodeToTreeNode(childNode, childTreeNode);
                }
            }
        }


        private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Access the XmlNode stored in the Tag property
            if (e.Node.Tag is XmlNode selectedNode)
            {
                PopulateListView(selectedNode);
                if (screenshot != null)
                {
                    float xScale = (float)screenshot.Width / pictureBox1.ClientSize.Width;
                    float yScale = (float)screenshot.Height / pictureBox1.ClientSize.Height;

                    // Calculate the actual coordinates on the image
                    int actualX = (int)(elementX * xScale);
                    int actualY = (int)(elementY * yScale);

                    int actualWidth = (int)(elementWidth * xScale);
                    int actualHeight = (int)(elementHeight * yScale);

                    // Reset to the original image before drawing
                    DrawRectangleOnImage(screenshot, actualX, actualY, actualWidth, actualHeight);
                }
            }
            else
            {
                listView1.Items.Clear(); // Clear the ListView if no valid node is selected
            }
        }


        private int elementX;
        private int elementY;
        private int elementWidth;
        private int elementHeight;
        private void PopulateListView(XmlNode xmlNode)
        {
            listView1.Items.Clear();

            // Add element name
            ListViewItem nameItem = new ListViewItem("Name");
            nameItem.SubItems.Add(xmlNode.Name);
            listView1.Items.Add(nameItem);

            // Add inner text
            ListViewItem textItem = new ListViewItem("InnerText");
            textItem.SubItems.Add(xmlNode.InnerText.Trim());
            listView1.Items.Add(textItem);

            // Add attributes
            if (xmlNode.Attributes != null)
            {
                foreach (XmlAttribute attribute in xmlNode.Attributes)
                {
                    ListViewItem attributeItem = new ListViewItem(attribute.Name);
                    attributeItem.SubItems.Add(attribute.Value);
                    listView1.Items.Add(attributeItem);
                }
            }

            // Specifically extract x, y, width, and height if they exist
            int.TryParse(xmlNode.Attributes["x"]?.Value, out elementX);
            int.TryParse(xmlNode.Attributes["y"]?.Value, out elementY);
            int.TryParse(xmlNode.Attributes["width"]?.Value, out elementWidth);
            int.TryParse(xmlNode.Attributes["height"]?.Value, out elementHeight);
        }



        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        Graphics g;
        private void DrawRectangleOnImage(Image image, int x, int y, int width, int height)
        {
            try
            {
                //this.Invalidate();
                using (g = Graphics.FromImage(image))
                {
                    // Create a pen to draw the rectangle
                    using (Pen pen = new Pen(Color.Red, 7))
                    {
                        // Draw the rectangle
                        g.DrawRectangle(pen, x, y, width, height);
                    }
                }
                pictureBox1.Image = image;
                // Refresh the PictureBox to show the updated image
                pictureBox1.Refresh();
            }
            catch (Exception)
            {
            }

        }
        //private Rectangle rectangleToDraw;
        //private Image image;
        //private void DrawRectangleOnImage(Image img, int x, int y, int width, int height)
        //{
        //    //pictureBox1.Invalidate();
        //    pictureBox1.Image = img;
        //    //image = img;
        //    //rectangleToDraw = new Rectangle(x, y, width, height);

        //}
        private async void refreshButton_Click(object sender, EventArgs e)
        {
            FetchScreen();
        }

        private List<TreeNode> matchingNodes = new List<TreeNode>();
        private int currentIndex = -1;

        private void xpathTextbox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlContent);

                // Attempt to select nodes using the XPath expression
                XmlNodeList selectedXmlNodes = xmlDoc.SelectNodes(filterTextbox.Text);

                matchingNodes.Clear();
                currentIndex = -1;

                if (selectedXmlNodes != null && selectedXmlNodes.Count > 0)
                {
                    foreach (XmlNode xmlNode in selectedXmlNodes)
                    {
                        // Find the corresponding TreeNode for each matching XmlNode
                        TreeNode matchingNode = FindTreeNodeByXmlNode(treeView1.Nodes, xmlNode);

                        if (matchingNode != null)
                        {
                            matchingNodes.Add(matchingNode);
                        }
                    }

                    filterTextbox.ForeColor = Color.Black;
                    TotalElementCount.Text = $"{matchingNodes.Count}";

                    if (matchingNodes.Count > 0)
                    {
                        currentIndex = 0;
                        HighlightCurrentNode();
                    }
                }
                else
                {
                    filterTextbox.ForeColor = Color.Red;
                    TotalElementCount.Text = "0";
                }
            }
            catch (XPathException)
            {
                filterTextbox.ForeColor = Color.Red;
                TotalElementCount.Text = "0";
            }
            catch (XmlException)
            {
                filterTextbox.ForeColor = Color.Red;
                TotalElementCount.Text = "0";
            }
        }

        private void HighlightCurrentNode()
        {
            if (currentIndex >= 0 && currentIndex < matchingNodes.Count)
            {
                TreeNode currentNode = matchingNodes[currentIndex];
                treeView1.SelectedNode = currentNode;
                treeView1.SelectedNode.EnsureVisible();
                elementNumberTextbox.Text = (currentIndex + 1).ToString();
            }
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            if (matchingNodes.Count > 0)
            {
                currentIndex = (currentIndex + 1) % matchingNodes.Count;
                HighlightCurrentNode();
            }
        }

        private void previousButton_Click(object sender, EventArgs e)
        {
            if (matchingNodes.Count > 0)
            {
                currentIndex = (currentIndex - 1 + matchingNodes.Count) % matchingNodes.Count;
                HighlightCurrentNode();
            }
        }

        // Ensure your FindTreeNodeByXmlNode and AreNodesEquivalent methods are defined as before


        private TreeNode FindTreeNodeByXmlNode(TreeNodeCollection nodes, XmlNode xmlNode)
        {
            foreach (TreeNode treeNode in nodes)
            {
                if (treeNode.Tag is XmlNode node)
                {
                    // Compare the node names and attributes to determine equivalence
                    if (AreNodesEquivalent(node, xmlNode))
                    {
                        return treeNode;
                    }
                }
                // Recursively search in child nodes
                TreeNode foundNode = FindTreeNodeByXmlNode(treeNode.Nodes, xmlNode);
                if (foundNode != null)
                {
                    return foundNode;
                }
            }
            return null;
        }

        private bool AreNodesEquivalent(XmlNode node1, XmlNode node2)
        {
            if (node1.Name != node2.Name)
            {
                return false;
            }

            if (node1.Attributes.Count != node2.Attributes.Count)
            {
                return false;
            }

            for (int i = 0; i < node1.Attributes.Count; i++)
            {
                XmlAttribute attr1 = node1.Attributes[i];
                XmlAttribute attr2 = node2.Attributes[attr1.Name];

                if (attr2 == null || attr1.Value != attr2.Value)
                {
                    return false;
                }
            }
            return true;
        }

        private void elementNumberTextbox_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(elementNumberTextbox.Text, out int elementNumber))
            {
                // Convert to zero-based index
                int index = elementNumber - 1;

                if (index >= 0 && index < matchingNodes.Count)
                {
                    currentIndex = index;
                    HighlightCurrentNode();
                    elementNumberTextbox.ForeColor = Color.Black;
                }
                else
                {
                    elementNumberTextbox.ForeColor = Color.Red; // Invalid number
                }
            }
            else
            {
                elementNumberTextbox.ForeColor = Color.Red; // Non-numeric input
            }
        }
        private void listView1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Check if a ListViewItem is under the cursor
                ListViewItem item = listView1.GetItemAt(e.X, e.Y);
                if (item != null)
                {
                    // Select the item under the cursor
                    item.Selected = true;

                    // Show the context menu at the cursor position
                    listViewContextMenuStrip.Show(listView1, e.Location);
                }
            }
        }


        private void copyXpathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 1) // multiple row selected
            {
                var predicates = new List<string>();

                foreach (ListViewItem selectedItem in listView1.SelectedItems)
                {
                    string property = selectedItem.Text;
                    string value = selectedItem.SubItems[1].Text;
                    string xpathPredicate = $"@{property}='{value}'";
                    predicates.Add(xpathPredicate);
                }

                string combinedPredicate = string.Join(" and ", predicates);
                string xpath = $"//*[{combinedPredicate}]";
                Clipboard.SetText(xpath);
            }
            else
            {
                ListViewItem selectedItem = listView1.SelectedItems[0];
                string property = selectedItem.Text;
                string value = selectedItem.SubItems[1].Text;

                string xpathPredicate = $"@{property}='{value}'";
                string xpath = $"//*[{xpathPredicate}]";
                Clipboard.SetText(xpath);
            }
        }

        private void addToFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 1) // multiple row selected
            {
                var predicates = new List<string>();

                foreach (ListViewItem selectedItem in listView1.SelectedItems)
                {
                    string property = selectedItem.Text;
                    string value = selectedItem.SubItems[1].Text;
                    string xpathPredicate = $"@{property}='{value}'";
                    predicates.Add(xpathPredicate);
                }

                string combinedPredicate = string.Join(" and ", predicates);

                if (!string.IsNullOrWhiteSpace(filterTextbox.Text))
                {
                    // Append the new combined predicates within the same XPath query
                    if (filterTextbox.Text.EndsWith("]"))
                    {
                        filterTextbox.Text = filterTextbox.Text.TrimEnd(']') + $" and {combinedPredicate}]";
                    }
                    else
                    {
                        filterTextbox.Text += $" and {combinedPredicate}";
                    }
                }
                else
                {
                    // Set the initial XPath expression
                    filterTextbox.Text = $"//*[{combinedPredicate}]";
                }
            }
            else
            {
                ListViewItem selectedItem = listView1.SelectedItems[0];
                string property = selectedItem.Text;
                string value = selectedItem.SubItems[1].Text;

                string xpathPredicate = $"@{property}='{value}'";
                if (!string.IsNullOrWhiteSpace(filterTextbox.Text))
                {
                    // Append the new predicate within the same XPath query
                    // Split the existing XPath to remove the last ']' before appending
                    if (filterTextbox.Text.EndsWith("]"))
                    {
                        filterTextbox.Text = filterTextbox.Text.TrimEnd(']') + $" and {xpathPredicate}]";
                    }
                    else
                    {
                        filterTextbox.Text += $" and {xpathPredicate}";
                    }
                }
                else
                {
                    // Set the initial XPath expression
                    filterTextbox.Text = $"//*[{xpathPredicate}]";
                }
            }
        }

        private void copyUniqueXpathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Tag is XmlNode selectedXmlNode)
            {
                string uniqueXPath = GenerateUniqueXPath(selectedXmlNode);
                Clipboard.SetText(uniqueXPath);
            }
        }



        private void treeView1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Select the node under the mouse pointer
                TreeNode selectedNode = treeView1.GetNodeAt(e.X, e.Y);
                if (selectedNode != null)
                {
                    treeView1.SelectedNode = selectedNode;

                    // Show the context menu at the mouse position
                    treeViewContextMenuStrip.Show(treeView1, e.Location);
                }
            }
        }


        private string GenerateUniqueXPath(XmlNode node)
        {
            // List of attributes to consider for creating a unique XPath
            string[] preferredAttributes = { "id", "name", "value", "label", "text", "class", "type", "enabled", "visible", "accessible" };

            // Start with the node name
            string xpath = "//" + node.Name;

            // Check each preferred attribute
            foreach (string attr in preferredAttributes)
            {
                if (node.Attributes[attr] != null)
                {
                    xpath += $"[@{attr}='{node.Attributes[attr].Value}']";
                    return xpath;
                }
            }

            // If no preferred attributes are found, check for inner text
            if (!string.IsNullOrWhiteSpace(node.InnerText))
            {
                xpath += $"[text()='{node.InnerText.Trim()}']";
                return xpath;
            }

            // Traverse up to the root, appending each parent node with predicates to ensure uniqueness
            while (node.ParentNode != null)
            {
                XmlNode parentNode = node.ParentNode;
                int index = 1;

                // Count the index of the node among its siblings with the same name
                foreach (XmlNode sibling in parentNode.ChildNodes)
                {
                    if (sibling == node)
                    {
                        break;
                    }
                    if (sibling.Name == node.Name)
                    {
                        index++;
                    }
                }

                // Append the current node's position to the XPath
                xpath = "/" + parentNode.Name + "/" + node.Name + "[" + index + "]" + xpath;

                // Move to the parent node
                node = parentNode;
            }

            return xpath;
        }

        private void addUniqueXpathToFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Tag is XmlNode selectedXmlNode)
            {
                string uniqueXPath = GenerateUniqueXPath(selectedXmlNode);
                filterTextbox.Text = uniqueXPath;
            }
        }





        //private void pictureBox1_Paint(object sender, PaintEventArgs e)
        //{
        //    if (!image.Equals(null))
        //    {
        //        e.Graphics.DrawImage(image, 0, 0, image.Width, image.Height);

        //        // Create a pen to draw the rectangle
        //        using (Pen pen = new Pen(Color.Red, 7))
        //        {
        //            // Draw the rectangle
        //            e.Graphics.DrawRectangle(pen, rectangleToDraw);
        //        }
        //    }
        //}
    }
}
