using Microsoft.VisualBasic.Devices;
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
        string os;
        public Object_Spy(string os, int port, int width, int height)
        {
            this.os = os;
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

        private async void Object_Spy_Load(object sender, EventArgs e)
        {
            FetchScreen();
        }


        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            float xScale = (float)screenshot.Width / pictureBox1.ClientSize.Width;
            float yScale = (float)screenshot.Height / pictureBox1.ClientSize.Height;
            int actualX = (int)(e.X * xScale);
            int actualY = (int)(e.Y * yScale);

            XmlNode clickedElement;
            if (isAndroid)
            {
                clickedElement = FindElementByCoordinates(actualX, actualY);
            }
            else
            {
                clickedElement = FindElementByCoordinates(e.X, e.Y);
            }

            //int elementX, elementY, elementWidth, elementHeight;
            if (clickedElement != null)
            {
                TreeNode matchingNode = FindTreeNodeByXmlNode(treeView1.Nodes, clickedElement);

                if (matchingNode != null)
                {
                    // Select the TreeNode
                    treeView1.SelectedNode = matchingNode;
                    matchingNode.EnsureVisible();
                }

                // Extract the bounds of the element
                if (isAndroid)
                {
                    string bounds = clickedElement.Attributes["bounds"]?.Value;
                    if (!string.IsNullOrEmpty(bounds))
                    {
                        var parts = bounds.Split(new[] { '[', ']', ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length == 4)
                        {
                            int.TryParse(parts[0], out elementX);
                            int.TryParse(parts[1], out elementY);
                            int.TryParse(parts[2], out int x2);
                            int.TryParse(parts[3], out int y2);

                            elementWidth = x2 - elementX;
                            elementHeight = y2 - elementY;
                            DrawRectangleOnImage(screenshot, elementX, elementY, elementWidth, elementHeight);
                        }
                    }
                }
                else
                {
                    int.TryParse(clickedElement.Attributes["x"]?.Value, out elementX);
                    int.TryParse(clickedElement.Attributes["y"]?.Value, out elementY);
                    int.TryParse(clickedElement.Attributes["width"]?.Value, out elementWidth);
                    int.TryParse(clickedElement.Attributes["height"]?.Value, out elementHeight);


                    actualX = (int)(elementX * xScale);
                    actualY = (int)(elementY * yScale);

                    int actualWidth = (int)(elementWidth * xScale);
                    int actualHeight = (int)(elementHeight * yScale);
                    DrawRectangleOnImage(screenshot, actualX, actualY, actualWidth, actualHeight);
                }
            }
        }

        private XmlNode FindElementByCoordinates(int x, int y)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlContent);

            var r = FindElementByCoordinatesRecursive(xmlDoc.DocumentElement, x, y);
            return r;
        }

        private XmlNode FindElementByCoordinatesRecursive(XmlNode node, int x, int y)
        {
            if (node == null) return null;

            // Check all children first to find the deepest matching node
            XmlNode mostSpecificNode = null;
            foreach (XmlNode childNode in node.ChildNodes)
            {
                XmlNode foundNode = FindElementByCoordinatesRecursive(childNode, x, y);
                if (foundNode != null)
                {
                    mostSpecificNode = foundNode;
                    break; // Exit the loop as soon as a node is found
                }
            }

            // If no more specific child node contains the point, check the current node
            if (mostSpecificNode == null && IsPointInElementBounds(node, x, y))
            //if (IsPointInElementBounds(node, x, y))
            {
                Console.WriteLine($"Node {node.Name} contains the point: ({x}, {y})");
                return node;
            }

            return mostSpecificNode;
        }


        private bool IsPointInElementBounds(XmlNode node, int x, int y)
        {
            int elementX = 0, elementY = 0, elementWidth = 0, elementHeight = 0;

            if (isAndroid)
            {
                string bounds = node.Attributes["bounds"]?.Value;
                if (!string.IsNullOrEmpty(bounds))
                {
                    var parts = bounds.Split(new[] { '[', ']', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 4)
                    {
                        int.TryParse(parts[0], out elementX);
                        int.TryParse(parts[1], out elementY);
                        int.TryParse(parts[2], out int x2);
                        int.TryParse(parts[3], out int y2);

                        elementWidth = x2 - elementX;
                        elementHeight = y2 - elementY;

                        // Debug: Log bounds and point
                        Console.WriteLine($"Checking Android node: {node.Name}, Bounds: [{elementX}, {elementY}, {x2}, {y2}], Point: ({x}, {y})");

                        // Validate bounds
                        if (elementX > 0 && elementY > 0 && elementHeight > 0 && elementHeight > 0)
                        {
                            return x >= elementX && x <= (elementX + elementWidth) && y >= elementY && y <= (elementY + elementHeight);
                        }
                    }
                }
                return false;
            }
            else
            {
                // Check if the node has the necessary attributes
                if (node.Attributes["x"] != null &&
                    node.Attributes["y"] != null &&
                    node.Attributes["width"] != null &&
                    node.Attributes["height"] != null)
                {
                    int.TryParse(node.Attributes["x"].Value, out elementX);
                    int.TryParse(node.Attributes["y"].Value, out elementY);
                    int.TryParse(node.Attributes["width"].Value, out elementWidth);
                    int.TryParse(node.Attributes["height"].Value, out elementHeight);

                    // Debug: Log bounds and point
                    Console.WriteLine($"Checking node: {node.Name}, Bounds: [{elementX}, {elementY}, {elementWidth}, {elementHeight}], Point: ({x}, {y})");

                    // Validate bounds
                    if (elementX > 0 && elementY > 0 && elementHeight > 0 && elementHeight > 0)
                    {
                        return x >= elementX && x <= (elementX + elementWidth) && y >= elementY && y <= (elementY + elementHeight);
                    }
                }

                // If bounds are not valid or attributes are missing, return false
                return false;
            }
        }



        private async void FetchScreen()
        {
            string messageTitle = "Object Spy - BETA";
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Owner = this;
            commonProgress.Show();
            commonProgress.UpdateStepLabel(messageTitle, "Please wait while fetching screen...", 20);
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
            commonProgress.UpdateStepLabel(messageTitle, "Please wait while fetching screen...", 50);
            if (screenshot == null)
            {
                commonProgress.Close();
                MessageBox.Show("Failed to retreive screenshot.", messageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
            else
            {
                pictureBox1.Image = screenshot;
                await Task.Run(() =>
                {
                    if (isAndroid)
                    {
                        xmlContent = AndroidAPIMethods.GetPageSource(port);
                    }
                    else
                    {
                        xmlContent = iOSAPIMethods.GetPageSource(port);
                    }
                });
                commonProgress.UpdateStepLabel(messageTitle, "Please wait while fetching screen...", 70);
                if (xmlContent.Equals("empty"))
                {
                    commonProgress.Close();
                    MessageBox.Show("Failed to fetch page source.", messageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                }
                else
                {
                    await Task.Run(() =>
                    {
                        LoadXmlToTreeView(xmlContent);
                    });
                    commonProgress.UpdateStepLabel(messageTitle, "Please wait while fetching screen...", 90);
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
                    if (isAndroid)
                    {
                        DrawRectangleOnImage(screenshot, elementX, elementY, elementWidth, elementHeight);
                    }
                    else
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

            if (isAndroid)
            {
                string bounds = xmlNode.Attributes["bounds"]?.Value;
                if (!string.IsNullOrEmpty(bounds))
                {
                    // Split the bounds string correctly
                    var parts = bounds.Split(new[] { '[', ']', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 4)
                    {
                        int.TryParse(parts[0], out int x1);
                        int.TryParse(parts[1], out int y1);
                        int.TryParse(parts[2], out int x2);
                        int.TryParse(parts[3], out int y2);

                        elementX = x1;
                        elementY = y1;
                        elementWidth = x2 - x1;
                        elementHeight = y2 - y1;

                        // Add parsed bounds to ListView
                        ListViewItem boundsItem = new ListViewItem("Bounds");
                        boundsItem.SubItems.Add($"[{elementX},{elementY}][{x2},{y2}]");
                        listView1.Items.Add(boundsItem);

                        ListViewItem xItem = new ListViewItem("X");
                        xItem.SubItems.Add(elementX.ToString());
                        listView1.Items.Add(xItem);

                        ListViewItem yItem = new ListViewItem("Y");
                        yItem.SubItems.Add(elementY.ToString());
                        listView1.Items.Add(yItem);

                        ListViewItem widthItem = new ListViewItem("Width");
                        widthItem.SubItems.Add(elementWidth.ToString());
                        listView1.Items.Add(widthItem);

                        ListViewItem heightItem = new ListViewItem("Height");
                        heightItem.SubItems.Add(elementHeight.ToString());
                        listView1.Items.Add(heightItem);
                    }
                }
            }
            else
            {
                int.TryParse(xmlNode.Attributes["x"]?.Value, out elementX);
                int.TryParse(xmlNode.Attributes["y"]?.Value, out elementY);
                int.TryParse(xmlNode.Attributes["width"]?.Value, out elementWidth);
                int.TryParse(xmlNode.Attributes["height"]?.Value, out elementHeight);
            }
        }



        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }


        private void DrawRectangleOnImage(Image originalImage, int x, int y, int width, int height)
        {
            try
            {
                // Create a new bitmap from the original image to avoid altering the original
                Bitmap imageCopy = new Bitmap(originalImage);

                using (Graphics g = Graphics.FromImage(imageCopy))
                {
                    // Clear the existing drawings by filling with a transparent color, if needed
                    // g.Clear(Color.Transparent); // Uncomment if you want to clear with a specific color

                    // Create a pen to draw the rectangle
                    using (Pen pen = new Pen(Color.Red, 7))
                    {
                        // Draw the rectangle
                        g.DrawRectangle(pen, x, y, width, height);
                    }
                }

                // Assign the modified image to the PictureBox
                pictureBox1.Image = imageCopy;

                // Refresh the PictureBox to show the updated image
                pictureBox1.Refresh();
            }
            catch (Exception ex)
            {
                // Handle exceptions if needed
                Console.WriteLine(ex.Message);
            }
        }

        private async void refreshButton_Click(object sender, EventArgs e)
        {
            GoogleAnalytics.SendEvent("Object_Spy_Refresh", os);
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
            //GoogleAnalytics.SendEvent("Object_Spy_ListView_CopyXPath", os);
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
            //GoogleAnalytics.SendEvent("Object_Spy_ListView_AddToFilter", os);
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
            //GoogleAnalytics.SendEvent("Object_Spy_TreeView_CopyXPath", os);
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

        //private string GenerateUniqueXPath(XmlNode node)
        //{
        //    List<string> preferredAttributes = new List<string>();
        //    if (node.Attributes != null)
        //    {
        //        foreach (XmlAttribute attribute in node.Attributes)
        //        {
        //            preferredAttributes.Add(attribute.Name);
        //        }
        //    }
        //    preferredAttributes.Remove("index");
        //    preferredAttributes.Remove("x");
        //    preferredAttributes.Remove("y");
        //    preferredAttributes.Remove("width");
        //    preferredAttributes.Remove("height");

        //    string xpath = $"//*";

        //    // Check each preferred attribute individually
        //    foreach (string attr in preferredAttributes)
        //    {
        //        if (node.Attributes[attr] != null)
        //        {
        //            string singleAttributeXPath = $"{xpath}[@{attr}='{node.Attributes[attr].Value}']";
        //            XmlNodeList nodes = node.OwnerDocument.SelectNodes(singleAttributeXPath);
        //            if (nodes.Count == 1)
        //            {
        //                return singleAttributeXPath;
        //            }
        //        }
        //    }

        //    // Check combined attributes
        //    string combinedAttributesXPath = xpath;
        //    foreach (string attr in preferredAttributes)
        //    {
        //        if (node.Attributes[attr] != null)
        //        {
        //            combinedAttributesXPath += $"[@{attr}='{node.Attributes[attr].Value}']";
        //        }
        //    }
        //    XmlNodeList combinedNodes = node.OwnerDocument.SelectNodes(combinedAttributesXPath);
        //    if (combinedNodes.Count == 1)
        //    {
        //        return combinedAttributesXPath;
        //    }

        //    // If no unique XPath is found with attributes, check for inner text
        //    if (!string.IsNullOrWhiteSpace(node.InnerText))
        //    {
        //        string textXPath = $"{xpath}[text()='{node.InnerText.Trim()}']";
        //        XmlNodeList textNodes = node.OwnerDocument.SelectNodes(textXPath);
        //        if (textNodes.Count == 1)
        //        {
        //            return textXPath;
        //        }
        //    }

        //    // If still not unique, wrap in parentheses and append index
        //    while (node.ParentNode != null)
        //    {
        //        XmlNode parentNode = node.ParentNode;
        //        int index = 1;

        //        foreach (XmlNode sibling in parentNode.ChildNodes)
        //        {
        //            if (sibling == node)
        //            {
        //                break;
        //            }
        //            if (sibling.Name == node.Name)
        //            {
        //                index++;
        //            }
        //        }

        //        xpath = $"/{parentNode.Name}({node.Name})[{index}]" + xpath;
        //        node = parentNode;
        //    }

        //    return xpath;
        //}


        private string GenerateUniqueXPath(XmlNode node)
        {
            List<string> preferredAttributes = new List<string>();
            if (node.Attributes != null)
            {
                // Iterate over each attribute in the node
                foreach (XmlAttribute attribute in node.Attributes)
                {
                    // Add the attribute name to the list
                    preferredAttributes.Add(attribute.Name);
                }
            }
            // List of attributes to consider for creating a unique XPath
            //string[] preferredAttributes = { "id", "name", "value", "label", "text", "class", "type", "enabled", "visible", "accessible" };

            preferredAttributes.Remove("index");
            preferredAttributes.Remove("x");
            preferredAttributes.Remove("y");
            preferredAttributes.Remove("width");
            preferredAttributes.Remove("height");
            // Start with the node name
            //string xpath = "//" + node.Name;
            string xpath = "//*";
            int attCount = preferredAttributes.Count;
            int counter = 1;
            foreach (string attr in preferredAttributes)
            {
                if (node.Attributes[attr] != null)
                {
                    string singleAttributeXPath = $"{xpath}[@{attr}='{node.Attributes[attr].Value}']";
                    XmlNodeList nodes = node.OwnerDocument.SelectNodes(singleAttributeXPath);
                    if (nodes.Count == 1)
                    {
                        return singleAttributeXPath;
                    }
                    if (attCount == counter)
                    {
                        int index = 1;
                        foreach (XmlNode sibling in nodes)
                        {
                            if (sibling == node)
                            {
                                xpath = $"({xpath})[{index}]";
                                return xpath;
                            }
                            index++;
                        }
                    }
                    counter++;
                }
            }


            // Check each preferred attribute
            foreach (string attr in preferredAttributes)
            {
                if (node.Attributes[attr] != null)
                {
                    xpath += $"[@{attr}='{node.Attributes[attr].Value}']";

                    // Check if this XPath is unique
                    XmlNodeList nodes = node.OwnerDocument.SelectNodes(xpath);
                    if (nodes.Count == 1)
                    {
                        return xpath;
                    }
                    else
                    {
                        // If not unique, wrap in parentheses and append index
                        int index = 1;
                        foreach (XmlNode sibling in nodes)
                        {
                            if (sibling == node)
                            {
                                xpath = $"({xpath})[{index}]";
                                return xpath;
                            }
                            index++;
                        }
                    }
                }
            }

            // If no preferred attributes are found, check for inner text
            if (!string.IsNullOrWhiteSpace(node.InnerText))
            {
                xpath += $"[text()='{node.InnerText.Trim()}']";

                // Check if this XPath is unique
                XmlNodeList nodes = node.OwnerDocument.SelectNodes(xpath);
                if (nodes.Count == 1)
                {
                    return xpath;
                }
                else
                {
                    // If not unique, wrap in parentheses and append index
                    int index = 1;
                    foreach (XmlNode sibling in nodes)
                    {
                        if (sibling == node)
                        {
                            xpath = $"({xpath})[{index}]";
                            return xpath;
                        }
                        index++;
                    }
                }
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

                // Wrap the XPath in parentheses and append the index
                xpath = $"/{parentNode.Name}/({node.Name})[{index}]" + xpath;

                // Move to the parent node
                node = parentNode;
            }

            return xpath;
        }

        private string GenerateUniqueXPathOld(XmlNode node)
        {
            List<string> preferredAttributes = new List<string>();
            if (node.Attributes != null)
            {
                // Iterate over each attribute in the node
                foreach (XmlAttribute attribute in node.Attributes)
                {
                    // Add the attribute name to the list
                    preferredAttributes.Add(attribute.Name);
                }
            }

            // List of attributes to consider for creating a unique XPath
            //string[] preferredAttributes = { "id", "name", "value", "label", "text", "class", "type", "enabled", "visible", "accessible" };

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
            //GoogleAnalytics.SendEvent("Object_Spy_TreeView_AddToFilter", os);
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Tag is XmlNode selectedXmlNode)
            {
                string uniqueXPath = GenerateUniqueXPath(selectedXmlNode);
                filterTextbox.Text = uniqueXPath;
                treeView1.SelectedNode = treeView1.SelectedNode;
            }
        }



        int clickedX; int clickedY;
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                clickedX = e.X;
                clickedY = e.Y;
                pictureBoxContextMenuStrip.Show(pictureBox1, e.Location);
            }
        }

        private void copyUniqueXPathToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //GoogleAnalytics.SendEvent("Object_Spy_PictureBox_CopyXPath", os);
            float xScale = (float)screenshot.Width / pictureBox1.ClientSize.Width;
            float yScale = (float)screenshot.Height / pictureBox1.ClientSize.Height;
            int actualX = (int)(clickedX * xScale);
            int actualY = (int)(clickedY * yScale);

            XmlNode clickedElement;
            if (isAndroid)
            {
                clickedElement = FindElementByCoordinates(actualX, actualY);
            }
            else
            {
                clickedElement = FindElementByCoordinates(clickedX, clickedY);
            }
            string uniqueXPath = GenerateUniqueXPath(clickedElement);
            Clipboard.SetText(uniqueXPath);
        }

        private void addUniqueXpathToFilterToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //GoogleAnalytics.SendEvent("Object_Spy_PictureBox_AddToFilter", os);
            float xScale = (float)screenshot.Width / pictureBox1.ClientSize.Width;
            float yScale = (float)screenshot.Height / pictureBox1.ClientSize.Height;
            int actualX = (int)(clickedX * xScale);
            int actualY = (int)(clickedY * yScale);

            XmlNode clickedElement;
            if (isAndroid)
            {
                clickedElement = FindElementByCoordinates(actualX, actualY);
            }
            else
            {
                clickedElement = FindElementByCoordinates(clickedX, clickedY);
            }
            string uniqueXPath = GenerateUniqueXPath(clickedElement);
            filterTextbox.Text = uniqueXPath;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            coordLabel.Text = $"X: {e.X}, Y: {e.Y}";
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            coordLabel.Text = string.Empty;
        }

        private void Object_Spy_Shown(object sender, EventArgs e)
        {
            GoogleAnalytics.SendEvent("Object_Spy_Shown",os);
        }
    }
}
