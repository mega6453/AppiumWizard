﻿using NLog;
using System.Xml;

namespace Appium_Wizard
{
    public partial class Object_Spy : Form
    {
        int port, width, height;
        Image screenshot;
        string xmlContent;
        bool isAndroid;
        string os, sessionId;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();


        public Object_Spy(string os, int port, int width, int height, string sessionId)
        {
            this.os = os;
            this.port = port;
            this.width = width;
            this.height = height;
            this.sessionId = sessionId;
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
            await FetchScreen();
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

        //private XmlNode FindElementByCoordinatesRecursive(XmlNode node, int x, int y)
        //{
        //    if (node == null) return null;

        //    // Check all children first to find the deepest matching node
        //    XmlNode mostSpecificNode = null;
        //    foreach (XmlNode childNode in node.ChildNodes)
        //    {
        //        XmlNode foundNode = FindElementByCoordinatesRecursive(childNode, x, y);
        //        if (foundNode != null)
        //        {
        //            mostSpecificNode = foundNode;
        //            break; // Exit the loop as soon as a node is found
        //        }
        //    }

        //    // If no more specific child node contains the point, check the current node
        //    if (mostSpecificNode == null && IsPointInElementBounds(node, x, y))
        //    //if (IsPointInElementBounds(node, x, y))
        //    {
        //        Console.WriteLine($"Node {node.Name} contains the point: ({x}, {y})");
        //        return node;
        //    }

        //    return mostSpecificNode;
        //}

        private XmlNode FindElementByCoordinatesRecursive(XmlNode node, int x, int y)
        {
            if (node == null) return null;

            XmlNode bestMatch = null;
            int smallestArea = int.MaxValue;

            foreach (XmlNode childNode in node.ChildNodes)
            {
                XmlNode foundNode = FindElementByCoordinatesRecursive(childNode, x, y);
                if (foundNode != null)
                {
                    int area = GetArea(foundNode);
                    if (area < smallestArea)
                    {
                        bestMatch = foundNode;
                        smallestArea = area;
                    }
                }
            }

            if (bestMatch == null && IsPointInElementBounds(node, x, y))
            {
                return node;
            }

            return bestMatch;
        }

        private int GetArea(XmlNode node)
        {
            int.TryParse(node.Attributes["x"]?.Value, out int x);
            int.TryParse(node.Attributes["y"]?.Value, out int y);
            int.TryParse(node.Attributes["width"]?.Value, out int width);
            int.TryParse(node.Attributes["height"]?.Value, out int height);
            return width * height;
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



        private async Task FetchScreen()
        {
            string messageTitle = "Object Spy - BETA";
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Owner = this;
            commonProgress.Show();
            commonProgress.UpdateStepLabel(messageTitle, "Please wait while fetching screen...", 20);
            pictureBox1.Size = new Size(width, height);
            string url = "http://localhost:" + port;
            await Task.Run(() => {
                if (isAndroid)
                {
                    screenshot = AndroidAPIMethods.TakeScreenshotWithSessionId(port, sessionId);
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
                MessageBox.Show("Failed to retrieve screenshot.", messageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }
            else
            {
                pictureBox1.Image = screenshot;
                await Task.Run(() => {
                    if (isAndroid)
                    {
                        xmlContent = AndroidAPIMethods.GetPageSource(port, sessionId);
                    }
                    else
                    {
                        xmlContent = iOSAPIMethods.GetPageSource(port, sessionId);
                    }
                });

                commonProgress.UpdateStepLabel(messageTitle, "Please wait while fetching screen...", 75);
                if (xmlContent.Equals("Invalid session") | xmlContent.Equals("empty") | xmlContent.Contains("Exception while getting page source") | xmlContent.Contains("Failed to create a new session."))
                {
                    Logger.Info("xmlContent : "+xmlContent);
                    commonProgress.Close();
                    MessageBox.Show("Failed to fetch page source.", messageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                    return;
                }
                commonProgress.UpdateStepLabel(messageTitle, "Please wait while fetching screen...", 85);
                // Load XML content into the tree view
                await Task.Run(() =>
                {
                    LoadXmlToTreeView(xmlContent);
                });

                commonProgress.UpdateStepLabel(messageTitle, "Please wait while fetching screen...", 95);
                listView1.Items.Clear();
                treeView1.ExpandAll();
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
            await FetchScreen();
        }

        private List<TreeNode> matchingNodes = new List<TreeNode>();
        private int currentIndex = -1;

        private void xpathTextbox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlContent);

                string filterText = filterTextbox.Text;

                // Check if the filterText is empty or whitespace
                if (string.IsNullOrWhiteSpace(filterText))
                {
                    // Clear the matching nodes and UI
                    matchingNodes.Clear();
                    currentIndex = -1;
                    filterTextbox.ForeColor = Color.Black;
                    TotalElementCount.Text = "0";
                    elementNumberTextbox.Text = "0";
                    return;
                }
                XmlNodeList selectedXmlNodes;
                try
                {
                     selectedXmlNodes = xmlDoc.SelectNodes(filterText);
                }
                catch (Exception ex)
                {
                    filterText = filterText.Replace("='", "=\"").Replace("']", "\"]").Replace("['", "[\"").Replace("' and ", "\" and ");
                    selectedXmlNodes = xmlDoc.SelectNodes(filterText);
                }
               

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
                    elementNumberTextbox.Text = "0";
                }
            }
            catch (Exception)
            {
                filterTextbox.ForeColor = Color.Red;
                TotalElementCount.Text = "0";
                elementNumberTextbox.Text = "0";
            }
        }

        private void HighlightCurrentNode()
        {
            try
            {
                if (currentIndex >= 0 && currentIndex < matchingNodes.Count)
                {
                    TreeNode currentNode = matchingNodes[currentIndex];

                    // Ensure the UI update happens on the correct thread
                    if (treeView1.InvokeRequired)
                    {
                        treeView1.Invoke(new Action(() =>
                        {
                            treeView1.SelectedNode = currentNode;
                            treeView1.SelectedNode.EnsureVisible();
                        }));
                    }
                    else
                    {
                        treeView1.SelectedNode = currentNode;
                        treeView1.SelectedNode.EnsureVisible();
                    }

                    elementNumberTextbox.Text = (currentIndex + 1).ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HighlightCurrentNode: {ex.Message}");
            }
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            if (matchingNodes.Count > 0)
            {
                // Increment index and wrap around if necessary
                currentIndex = (currentIndex + 1) % matchingNodes.Count;

                // Ensure the UI updates correctly
                HighlightCurrentNode();
            }
        }

        private void previousButton_Click(object sender, EventArgs e)
        {
            if (matchingNodes.Count > 0)
            {
                // Decrement index and wrap around if necessary
                currentIndex = (currentIndex - 1 + matchingNodes.Count) % matchingNodes.Count;

                // Ensure the UI updates correctly
                HighlightCurrentNode();
            }
        }

        private TreeNode FindTreeNodeByXmlNode(TreeNodeCollection nodes, XmlNode xmlNode)
        {
            try
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
            catch (Exception)
            {
                return null;
            }
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
                if (elementNumber == 0)
                {
                    // Special case: 0 is valid, but no element corresponds to it
                    elementNumberTextbox.ForeColor = Color.Black;
                    return;
                }

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

        public static string GenerateUniqueXPath(XmlNode node)
        {
            try
            {

                // Start with the base XPath using the node name
                string baseXPath = $"//{node.Name}";

                // Relevant attributes to consider for XPath generation
                string[] importantAttributes = { 
                                                // Android attributes
                                                "resource-id",
                                                "text",
                                                "content-desc",
                                                "class",    
                                                // iOS attributes
                                                "name",
                                                "label",
                                                "value",
                                                "type"
                                            };

                // Step 1: Test each attribute individually
                foreach (string attributeName in importantAttributes)
                {
                    if (node.Attributes[attributeName] != null)
                    {
                        string testXPath = $"{baseXPath}[@{attributeName}='{node.Attributes[attributeName].Value}']";
                        XmlNodeList matchingNodes = node.OwnerDocument.SelectNodes(testXPath);

                        // If the XPath with this single attribute is unique, return it
                        if (matchingNodes.Count == 1)
                        {
                            return testXPath;
                        }

                        // If multiple nodes match, skip further combinations and go to indexing
                        if (matchingNodes.Count > 1)
                        {
                            return AddIndexToXPath(testXPath, matchingNodes, node);
                        }
                    }
                }

                // Step 2: Incrementally combine attributes (skipped if Step 1 detects multiple matches)
                var attributeConditions = new System.Text.StringBuilder();
                foreach (string attributeName in importantAttributes)
                {
                    if (node.Attributes[attributeName] != null)
                    {
                        if (attributeConditions.Length > 0)
                        {
                            attributeConditions.Append(" and ");
                        }
                        attributeConditions.Append($"@{attributeName}='{node.Attributes[attributeName].Value}'");

                        string testXPath = $"{baseXPath}[{attributeConditions}]";
                        XmlNodeList matchingNodes = node.OwnerDocument.SelectNodes(testXPath);

                        // If the XPath with the combined attributes is unique, return it
                        if (matchingNodes.Count == 1)
                        {
                            return testXPath;
                        }

                        // If multiple nodes match, skip further combinations and go to indexing
                        if (matchingNodes.Count > 1)
                        {
                            return AddIndexToXPath(testXPath, matchingNodes, node);
                        }
                    }
                }

                // Step 3: Add indexing to the final XPath
                string finalXPath = $"{baseXPath}[{attributeConditions}]";
                XmlNodeList finalMatchingNodes = node.OwnerDocument.SelectNodes(finalXPath);
                return AddIndexToXPath(finalXPath, finalMatchingNodes, node);
            }
            catch (Exception)
            {
                return "";
            }
        }

        private static string AddIndexToXPath(string xpath, XmlNodeList matchingNodes, XmlNode targetNode)
        {
            int index = 1;
            foreach (XmlNode matchingNode in matchingNodes)
            {
                if (matchingNode == targetNode)
                {
                    return $"({xpath})[{index}]";
                }
                index++;
            }
            return xpath; // Fallback (shouldn't happen if indexing is applied correctly)
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
            GoogleAnalytics.SendEvent("Object_Spy_Shown", os);
        }

        private void helpButton_Click(object sender, EventArgs e)
        {
            string message = @"
                This tool helps you inspect UI elements and generate XPath expressions for automation.

                How to use:
                1. Image View: Right-click on an element in the screenshot to copy its XPath or add it to the filter.
                    - Hover over the image to see the coordinates of your cursor.
                    - Click on an element to highlight it and view its details.

                2. Tree View: Right-click on a node in the tree structure to copy its XPath or add it to the filter.
                    - The tree displays the XML structure of the page source.
                    - Selecting a node will highlight the corresponding element in the image.

                3. List View: Select one or more properties from the list to generate and copy a stable XPath.
                    - Combine multiple properties for a more specific XPath.

                4. Filter Textbox: Use XPath expressions to filter elements.
                    - Type an XPath query in the filter textbox to locate elements.
                    - Matching nodes will be highlighted in the tree view.

                5. Navigation: Use 'Next' and 'Previous' buttons to navigate through matching elements.
                    - The total count of matching elements is displayed.

                Note:
                - This feature is in Beta. If you encounter any issues, please report them on the GitHub page.";

            MessageBox.Show(message, "Object Spy - BETA", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
