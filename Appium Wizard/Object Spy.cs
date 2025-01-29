using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Appium_Wizard
{
    public partial class Object_Spy : Form
    {
        int port, width, height;
        Image screenshot;
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
                pictureBox1.Image = screenshot;
                var xml = iOSAPIMethods.GetPageSource(port);
                LoadXmlToTreeView(xml);
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

        private void DrawRectangleOnImage(Image image, int x, int y, int width, int height)
        {
            // Create a Graphics object from the image
            using (Graphics g = Graphics.FromImage(image))
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

        private async void refreshButton_Click(object sender, EventArgs e)
        {
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Refresh Screen","Please wait while fetching screen...",20);
            string url = "http://localhost:" + port;
            await Task.Run(() => {
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
                string xml="empty";
                await Task.Run(() => {
                xml = iOSAPIMethods.GetPageSource(port);                   
                });
                commonProgress.UpdateStepLabel("Refresh Screen", "Please wait while fetching screen...", 70);
                if (xml.Equals("empty"))
                {
                    MessageBox.Show("Failed to fetch page source.", "Refresh Screen", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    await Task.Run(() => {
                        LoadXmlToTreeView(xml);
                    });
                    commonProgress.UpdateStepLabel("Refresh Screen", "Please wait while fetching screen...", 90);
                    listView1.Items.Clear();
                    treeView1.ExpandAll();
                }
            }
            commonProgress.Close();
        }
    }
}
