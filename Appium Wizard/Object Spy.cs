using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Appium_Wizard
{
    public partial class Object_Spy : Form
    {
        int port, width, height;
        Image screenshot;
        string xmlContent;
        public Object_Spy(int port, int width, int height)
        {
            this.port = port;
            this.width = width;
            this.height = height;
            InitializeComponent();
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

        private void Object_Spy_Load(object sender, EventArgs e)
        {
            pictureBox1.Size = new Size(width, height);
            string url = "http://localhost:" + port;
            screenshot = iOSAPIMethods.TakeScreenshot(url);

            if (screenshot == null)
            {
                MessageBox.Show("Failed to retreive screenshot.", "Opening Object Spy", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                //image = screenshot;
                pictureBox1.Image = screenshot;
                xmlContent = iOSAPIMethods.GetPageSource(port);
                LoadXmlToTreeView(xmlContent);
                treeView1.ExpandAll();
            }
        }

        private void LoadXmlToTreeView(string xmlContent)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlContent);

            TreeNode rootNode = new TreeNode(xmlDoc.DocumentElement.Name);
            if (treeView1.InvokeRequired)
            {
                treeView1.Invoke(new Action(() => treeView1.Nodes.Add(rootNode)));
            }
            else
            {
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
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Refresh Screen", "Please wait while fetching screen...", 20);
            string url = "http://localhost:" + port;
            await Task.Run(() =>
            {
                screenshot = iOSAPIMethods.TakeScreenshot(url);
            });
            commonProgress.UpdateStepLabel("Refresh Screen", "Please wait while fetching screen...", 50);
            if (screenshot == null)
            {
                MessageBox.Show("Failed to retreive screenshot.", "Refresh Screen", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                pictureBox1.Image = screenshot;
                string xml = "empty";
                await Task.Run(() =>
                {
                    xml = iOSAPIMethods.GetPageSource(port);
                });
                commonProgress.UpdateStepLabel("Refresh Screen", "Please wait while fetching screen...", 70);
                if (xml.Equals("empty"))
                {
                    MessageBox.Show("Failed to fetch page source.", "Refresh Screen", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    await Task.Run(() =>
                    {
                        LoadXmlToTreeView(xml);
                    });
                    commonProgress.UpdateStepLabel("Refresh Screen", "Please wait while fetching screen...", 90);
                    listView1.Items.Clear();
                    treeView1.ExpandAll();
                }
            }
            commonProgress.Close();
        }


        //private void xpathTextbox_TextChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        XmlDocument xmlDoc = new XmlDocument();
        //        xmlDoc.LoadXml(xmlContent);

        //        // Attempt to select a node using the XPath expression
        //        XmlNode selectedXmlNode = xmlDoc.SelectSingleNode(xpathTextbox.Text);

        //        if (selectedXmlNode != null)
        //        {
        //            // Find the corresponding TreeNode
        //            TreeNode matchingNode = FindTreeNodeByXmlNode(treeView1.Nodes, selectedXmlNode);

        //            if (matchingNode != null)
        //            {
        //                xpathTextbox.ForeColor = Color.Black;
        //                treeView1.SelectedNode = matchingNode;
        //                treeView1.SelectedNode.EnsureVisible();
        //            }
        //            else
        //            {
        //                xpathTextbox.ForeColor = Color.Red;
        //            }
        //        }
        //        else
        //        {
        //            xpathTextbox.ForeColor = Color.Red;
        //        }
        //    }
        //    catch (XPathException)
        //    {
        //        xpathTextbox.ForeColor = Color.Red;
        //    }
        //    catch (XmlException)
        //    {
        //        xpathTextbox.ForeColor = Color.Red;
        //    }
        //}


        private List<TreeNode> matchingNodes = new List<TreeNode>();
        private int currentIndex = -1;

        private void xpathTextbox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlContent);

                // Attempt to select nodes using the XPath expression
                XmlNodeList selectedXmlNodes = xmlDoc.SelectNodes(xpathTextbox.Text);

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

                    xpathTextbox.ForeColor = Color.Black;
                    TotalElementCount.Text = $"{matchingNodes.Count}";

                    if (matchingNodes.Count > 0)
                    {
                        currentIndex = 0;
                        HighlightCurrentNode();
                    }
                }
                else
                {
                    xpathTextbox.ForeColor = Color.Red;
                    TotalElementCount.Text = "0";
                }
            }
            catch (XPathException)
            {
                xpathTextbox.ForeColor = Color.Red;
                TotalElementCount.Text = "0";
            }
            catch (XmlException)
            {
                xpathTextbox.ForeColor = Color.Red;
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
