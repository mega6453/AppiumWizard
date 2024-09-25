using Newtonsoft.Json;
using System.Diagnostics;
using System.Reflection;
using File = System.IO.File;

namespace Appium_Wizard
{
    public partial class MainScreen : Form
    {
        string udid, DeviceName, OSVersion, OSType, selectedUDID, Model, screenWidth, screenHeight;
        public static MainScreen main;
        string selectedDeviceName, selectedOS, selectedDeviceStatus, selectedDeviceVersion, selectedDeviceIP, selectedDeviceConnection, selectedDeviceCapability;
        public static List<int> runningProcesses = new List<int>();
        public static List<int> runningProcessesPortNumbers = new List<int>();
        private int labelStartPosition; bool isUpdateAvailable;
        string latestVersion;
        Dictionary<string, string> releaseInfo = new Dictionary<string, string>();
        public static Dictionary<string, Tuple<string, string>> DeviceInfo = new Dictionary<string, Tuple<string, string>>();
        public static Dictionary<string, int> udidProxyPort = new Dictionary<string, int>();
        public static Dictionary<string, int> udidScreenPort = new Dictionary<string, int>();

        public MainScreen()
        {
            InitializeComponent();
            main = this;
            RefreshDeviceListView();
            this.Text = "Appium Wizard " + VersionInfo.VersionNumber;
            USBWatcher usb = new USBWatcher(listView1);
            usb.Start();
            labelStartPosition = this.Width - label1.Width;
            if (Common.isInternetAvailable())
            {
                releaseInfo = Common.GetLatestReleaseInfo();
            }
        }
        private void onFormLoad(object sender, EventArgs e)
        {
            try
            {
                if (releaseInfo.ContainsKey("tag_name"))
                {
                    latestVersion = releaseInfo["tag_name"];
                    Version thisAppVersion = new Version(VersionInfo.VersionNumber);
                    Version latestVersionObj = new Version(latestVersion.Substring(1));
                    isUpdateAvailable = latestVersionObj > thisAppVersion;
                    if (isUpdateAvailable)
                    {
                        tableLayoutPanel1.Visible = true;
                        label1.Text = "Appium Wizard new version " + latestVersion + " is available for update. Go to \"Help -> Check for updates\" to open the download page.";
                        label1.AutoSize = true;
                        label1.Anchor = AnchorStyles.None;
                        Button closeButton = new Button();
                        closeButton.Text = "X";
                        closeButton.AutoSize = true;
                        closeButton.FlatStyle = FlatStyle.Flat;
                        closeButton.FlatAppearance.BorderSize = 0;
                        tableLayoutPanel1.Width = this.Width - 50;
                        tableLayoutPanel1.Controls.Add(label1, 0, 0);
                        tableLayoutPanel1.Controls.Add(closeButton, 1, 0);

                        closeButton.Click += (sender, e) =>
                        {
                            Controls.Remove(tableLayoutPanel1);
                        };

                        Controls.Add(tableLayoutPanel1);
                    }
                }
            }
            catch (Exception ex)
            {
                GoogleAnalytics.SendExceptionEvent("Check_Software_update_On_Load", ex.Message);
            }

            using (Graphics g = this.CreateGraphics())
            {
                // Get the current DPI scaling factor
                float dpiX = g.DpiX / 96.0f;

                // Calculate the new width based on the scaling factor
                int baseListViewWidth = 405;
                int newListViewWidth = (int)(baseListViewWidth * dpiX);

                // Adjust column widths and calculate total column width
                int totalColumnWidth = (int)((150 + 50 + 80 + 60 + 60) * dpiX);

                // Ensure ListView width is enough to fit all columns without scroll bar
                listView1.Width = Math.Max(newListViewWidth, totalColumnWidth);

                // Adjust Form width accordingly
                this.Width = listView1.Width + (this.Width - this.ClientSize.Width);

                // Set column widths
                listView1.Columns[0].Width = (int)(150 * dpiX);
                listView1.Columns[1].Width = (int)(50 * dpiX);
                listView1.Columns[2].Width = (int)(80 * dpiX);
                listView1.Columns[3].Width = (int)(60 * dpiX);
                listView1.Columns[4].Width = (int)(0 * dpiX);
                listView1.Columns[5].Width = (int)(60 * dpiX);
                listView1.Columns[6].Width = (int)(0 * dpiX);
                var size = new Size(this.Width - totalColumnWidth - 100, this.Height - 150);
                tabControl1.Left = listView1.Width + 50;
                tabControl1.Size = size;
                richTextBox1.Size = size;
                richTextBox2.Size = size;
                richTextBox3.Size = size;
                richTextBox4.Size = size;
                richTextBox5.Size = size;

                int checkBoxX = tabControl1.Right - checkBox1.Width;
                int checkBoxY = tabControl1.Top;

                // Set the CheckBox location
                checkBox1.Location = new System.Drawing.Point(checkBoxX, checkBoxY);
            }
            GoogleAnalytics.SendEvent("App_Version", VersionInfo.VersionNumber);
        }

        DateTime lastWriteTime1 = new DateTime();
        DateTime lastWriteTime2 = new DateTime();
        DateTime lastWriteTime3 = new DateTime();
        DateTime lastWriteTime4 = new DateTime();
        DateTime lastWriteTime5 = new DateTime();
        private void MainScreen_Shown(object sender, EventArgs e)
        {
            if (!LoadingScreen.isServerStarted)
            {
                var result = MessageBox.Show("Port " + LoadingScreen.appiumPort + " is being used by " + Common.RunNetstatAndFindProcessByPort(LoadingScreen.appiumPort).Item2 + ".\nDo you want to kill that process and start appium server in that port? ", "Error on Starting Server", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                if (result == DialogResult.Yes)
                {
                    Common.KillProcessByPortNumber(LoadingScreen.appiumPort);
                    AppiumServerSetup serverSetup = new AppiumServerSetup();
                    serverSetup.StartAppiumServer(LoadingScreen.appiumPort, 1, "appium --port " + LoadingScreen.appiumPort + " --allow-cors --allow-insecure=adb_shell");
                }
                else
                {
                    ServerConfig serverConfig = new ServerConfig();
                    serverConfig.ShowDialog();
                }
            }
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        if (AppiumServerSetup.portServerNumberAndFilePath.ContainsKey(1))
                        {
                            DateTime currentWriteTime = File.GetLastWriteTime(AppiumServerSetup.portServerNumberAndFilePath[1].Item2);
                            if (currentWriteTime != lastWriteTime1)
                            {
                                lastWriteTime1 = currentWriteTime;
                                UpdateRichTextbox(1);
                            }
                        }
                        if (AppiumServerSetup.portServerNumberAndFilePath.ContainsKey(2))
                        {
                            DateTime currentWriteTime = File.GetLastWriteTime(AppiumServerSetup.portServerNumberAndFilePath[2].Item2);
                            if (currentWriteTime != lastWriteTime2)
                            {
                                lastWriteTime2 = currentWriteTime;
                                UpdateRichTextbox(2);
                            }
                        }
                        if (AppiumServerSetup.portServerNumberAndFilePath.ContainsKey(3))
                        {
                            DateTime currentWriteTime = File.GetLastWriteTime(AppiumServerSetup.portServerNumberAndFilePath[3].Item2);
                            if (currentWriteTime != lastWriteTime3)
                            {
                                lastWriteTime3 = currentWriteTime;
                                UpdateRichTextbox(3);
                            }
                        }
                        if (AppiumServerSetup.portServerNumberAndFilePath.ContainsKey(4))
                        {
                            DateTime currentWriteTime = File.GetLastWriteTime(AppiumServerSetup.portServerNumberAndFilePath[4].Item2);
                            if (currentWriteTime != lastWriteTime4)
                            {
                                lastWriteTime4 = currentWriteTime;
                                UpdateRichTextbox(4);
                            }
                        }
                        if (AppiumServerSetup.portServerNumberAndFilePath.ContainsKey(5))
                        {
                            DateTime currentWriteTime = File.GetLastWriteTime(AppiumServerSetup.portServerNumberAndFilePath[5].Item2);
                            if (currentWriteTime != lastWriteTime5)
                            {
                                lastWriteTime5 = currentWriteTime;
                                UpdateRichTextbox(5);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    Thread.Sleep(1000);
                }
            });
            GoogleAnalytics.SendEvent(MethodBase.GetCurrentMethod().Name);
        }

        private void UpdateRichTextbox(int tabNumber)
        {
            using (var fileStream = new FileStream(AppiumServerSetup.portServerNumberAndFilePath[tabNumber].Item2, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var streamReader = new StreamReader(fileStream))
            {
                var fileContent = streamReader.ReadToEnd();
                if (tabNumber == 1)
                {
                    Invoke(new Action(() => richTextBox1.Text = fileContent));
                }
                else if (tabNumber == 2)
                {
                    Invoke(new Action(() => richTextBox2.Text = fileContent));
                }
                else if (tabNumber == 3)
                {
                    Invoke(new Action(() => richTextBox3.Text = fileContent));
                }
                else if (tabNumber == 4)
                {
                    Invoke(new Action(() => richTextBox4.Text = fileContent));
                }
                else if (tabNumber == 5)
                {
                    Invoke(new Action(() => richTextBox5.Text = fileContent));
                }
            }
        }


        public void UpdateDeviceStatus()
        {
            listView1.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                foreach (ListViewItem item in listView1.Items)
                {
                    string udidFromList = item.SubItems[4].Text;
                    string OS = item.SubItems[2].Text;
                    if (OS.Equals("Android"))
                    {
                        var connectedList = AndroidMethods.GetInstance().GetListOfDevicesUDID();
                        if (connectedList.Contains(udidFromList))
                        {
                            item.SubItems[3].Text = "Online";
                        }
                        else
                        {
                            item.SubItems[3].Text = "Offline";
                        }
                    }
                    else
                    {
                        var connectedList = iOSMethods.GetInstance().GetListOfDevicesUDID();
                        if (connectedList.Contains(udidFromList))
                        {
                            item.SubItems[3].Text = "Online";
                        }
                        else
                        {
                            item.SubItems[3].Text = "Offline";
                        }
                    }
                }
            });
        }


        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = listView1.SelectedItems[0];
                selectedDeviceName = selectedItem.SubItems[0].Text;
                selectedDeviceVersion = selectedItem.SubItems[1].Text;
                selectedOS = selectedItem.SubItems[2].Text;
                selectedDeviceStatus = selectedItem.SubItems[3].Text;
                selectedUDID = getDeviceUdidByName(selectedDeviceName);
                selectedDeviceConnection = selectedItem.SubItems[5].Text;
                selectedDeviceIP = selectedItem.SubItems[6].Text;
                Open.Enabled = true;
                MoreButton.Enabled = true;
                DeleteDevice.Enabled = true;
                if (selectedOS.Equals("iOS"))
                {
                    Version deviceVersion = new Version(selectedDeviceVersion);
                    Version version17Plus = new Version("17.0.0");
                    if (deviceVersion >= version17Plus) // >=17  -- Setting as true, as go-ios supports iOS 17+ now.
                    {
                        iOSMethods.isGo = true;
                        iOSAsyncMethods.isGo = true;
                        iOSAsyncMethods.is17Plus = true;
                    }
                    else // <17
                    {
                        iOSMethods.isGo = true;
                        iOSAsyncMethods.isGo = true;
                        iOSAsyncMethods.is17Plus = true;
                    }
                }
                if (selectedDeviceStatus.Equals("Online"))
                {
                    ShowCapability();
                }
                mandatorymsglabel.Visible = true;
                if (selectedDeviceConnection.Equals("Wi-Fi"))
                {
                    mandatorymsglabel.Text = "Note : It's mandatory to open the device before starting automation.\nDevice connected over Wi-Fi may be slower when compared to USB.";
                }
                else
                {
                    mandatorymsglabel.Text = "Note : It's mandatory to open the device before starting automation.";
                }
                if (selectedDeviceStatus.Equals("Offline"))
                {
                    panel1.Visible = false;
                    capabilityLabel.Visible = false;
                    Open.Enabled = false;
                    contextMenuStrip4.Items[0].Enabled = false;
                    contextMenuStrip4.Items[1].Enabled = false;
                    contextMenuStrip4.Items[2].Enabled = true; // Refresh
                    contextMenuStrip4.Items[3].Enabled = false;
                    contextMenuStrip4.Items[4].Enabled = false;
                    mandatorymsglabel.Visible = false;
                }
                else
                {
                    Open.Enabled = true;
                    foreach (var item in contextMenuStrip4.Items)
                    {
                        if (item is ToolStripItem toolStripItem)
                        {
                            toolStripItem.Enabled = true;
                        }
                    }
                }
            }
            else
            {
                panel1.Visible = false;
                capabilityLabel.Visible = false;
                Open.Enabled = false;
                DeleteDevice.Enabled = false;
                MoreButton.Enabled = false;
                mandatorymsglabel.Visible = false;
            }
        }

        private void ShowCapability()
        {
            panel1.Visible = true;
            capabilityLabel.Visible = true;
            string automationName = string.Empty;
            string jsonString = string.Empty;
            if (selectedOS.Equals("Android"))
            {
                automationName = "UiAutomator2";
                if (string.IsNullOrEmpty(selectedDeviceIP))
                {
                    jsonString = $@"{{
                                    ""platformName"": ""{selectedOS}"",
                                    ""appium:platformVersion"": ""{selectedDeviceVersion}"",
                                    ""appium:automationName"": ""{automationName}"",
                                    ""appium:udid"": ""{selectedUDID}""
                                }}";
                }
                else
                {
                    jsonString = $@"{{
                                    ""platformName"": ""{selectedOS}"",
                                    ""appium:platformVersion"": ""{selectedDeviceVersion}"",
                                    ""appium:automationName"": ""{automationName}"",
                                    ""appium:udid"": ""{selectedDeviceIP}""
                                }}";
                }
            }
            else
            {
                automationName = "XCUITest";
                jsonString = $@"{{
                                ""platformName"": ""{selectedOS}"",
                                ""appium:platformVersion"": ""{selectedDeviceVersion}"",
                                ""appium:automationName"": ""{automationName}"",
                                ""appium:udid"": ""{selectedUDID}""
                              }}";
            }

            selectedDeviceCapability = FormatJsonString(jsonString);
            richTextBox6.Text = selectedDeviceCapability;
        }

        private string FormatJsonString(string jsonString)
        {
            dynamic parsedJson = JsonConvert.DeserializeObject(jsonString);
            return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
        }
        private async void Open_Click(object sender, EventArgs e)
        {
            if (selectedDeviceStatus.Equals("Online"))
            {
                foreach (ScreenControl screenForm in Application.OpenForms.OfType<ScreenControl>())
                {                                           // brings foreground if opened already
                    if (screenForm.Name.Equals(selectedUDID, StringComparison.InvariantCultureIgnoreCase) | screenForm.Name.Equals(selectedDeviceIP, StringComparison.InvariantCultureIgnoreCase))
                    {
                        screenForm.Activate();
                        return;
                    }
                }
                OpenDevice openDevice = new OpenDevice(selectedUDID, selectedOS, selectedDeviceVersion, selectedDeviceName, selectedDeviceConnection, selectedDeviceIP);
                var isStarted = await openDevice.StartBackgroundTasks();
                if (isStarted)
                {
                    ShowCapability();
                }
                foreach (ScreenControl screenForm in Application.OpenForms.OfType<ScreenControl>())
                {
                    //Open screen in progress class and brings foreground if opened already
                    if (screenForm.Name.Equals(selectedUDID, StringComparison.InvariantCultureIgnoreCase) | screenForm.Name.Equals(selectedDeviceIP, StringComparison.InvariantCultureIgnoreCase))
                    {
                        screenForm.Activate();
                        return;
                    }
                }
            }
            else
            {
                GoogleAnalytics.SendEvent("OpenDevice_Device_Offline");
                MessageBox.Show(selectedDeviceName + " Device Offline. Please check device connectivity...", "Open Device", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            GoogleAnalytics.SendEvent("OpenDevice_Clicked");
        }

        public void addToList(string DeviceName, string OSVersion, string udid, string OS, string Model, string status, string connection, string IPAddress)
        {
            if (OS.Equals("iPhone OS", StringComparison.InvariantCultureIgnoreCase))
            {
                OS = "iOS";
            }
            if (DeviceName != null | DeviceName != string.Empty)
            {
                string[] item1 = { DeviceName ?? "", OSVersion, OS, status, udid, connection, IPAddress };
                if (listView1.InvokeRequired)
                {
                    listView1.Invoke(new Action(() => listView1.Items.Add(new ListViewItem(item1))));
                }
                else
                {
                    listView1.Items.Add(new ListViewItem(item1));
                }
                if (string.IsNullOrEmpty(IPAddress))
                {
                    if (!DeviceInfo.ContainsKey(udid))
                    {
                        DeviceInfo.Add(udid, Tuple.Create(DeviceName, OS));
                    }
                }
                else
                {
                    if (!DeviceInfo.ContainsKey(IPAddress))
                    {
                        DeviceInfo.Add(IPAddress, Tuple.Create(DeviceName, OS));
                    }
                }

            }
            GoogleAnalytics.SendEvent("Device_Added_To_List");
        }
        public void RefreshDeviceListView()
        {
            try
            {
                var devicesList = Database.QueryDataFromDevicesTable();
                string status = string.Empty;
                if (devicesList != null)
                {
                    listView1.Items.Clear();
                    foreach (var device in devicesList)
                    {
                        if (device["OS"].Equals("Android"))
                        {
                            var connectedList = AndroidMethods.GetInstance().GetListOfDevicesUDID();
                            if (connectedList.Contains(device["UDID"]) | connectedList.Contains(device["IPAddress"]))
                            {
                                status = "Online";
                            }
                            else
                            {
                                status = "Offline";
                            }
                        }
                        else
                        {
                            var connectedList = iOSMethods.GetInstance().GetListOfDevicesUDID();
                            if (connectedList.Contains(device["UDID"]))
                            {
                                status = "Online";
                            }
                            else
                            {
                                status = "Offline";
                            }
                        }
                        string[] item1 = { device["Name"], device["Version"], device["OS"], status, device["UDID"], device["Connection"], device["IPAddress"] };
                        listView1.Items.Add(new ListViewItem(item1));
                        if (!DeviceInfo.ContainsKey(device["UDID"]))
                        {
                            DeviceInfo.Add(device["UDID"], Tuple.Create(device["Name"], device["OS"]));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                GoogleAnalytics.SendExceptionEvent("RefreshDeviceListView", e.Message);
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Point screenPoint = AddDevice.PointToScreen(new Point(0, AddDevice.Height));
            contextMenuStrip1.Show(screenPoint);
            //GoogleAnalytics.SendEvent("AddDevice_Clicked");
        }

        public static bool isDeviceAlreadyAdded(string udid)
        {
            bool isDeviceAvailable = false;
            var devicesList = Database.QueryDataFromDevicesTable();
            foreach (var dictionary in devicesList)
            {
                if (dictionary.ContainsValue(udid))
                {
                    isDeviceAvailable = true;
                    break;
                }
            }
            return isDeviceAvailable;
        }

        public string getDeviceUdidByName(string name)
        {
            var devicesList = Database.QueryDataFromDevicesTable();
            foreach (var dictionary in devicesList)
            {
                if (dictionary["Name"].Equals(name))
                {
                    return dictionary["UDID"];
                }
            }
            return "";
        }

        private void DeleteDevice_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Deleting device will interrupt any running tests.\nAre you sure you want to delete " + selectedDeviceName + " device?", "Delete Device", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {
                bool valueExists = false;
                var devicesList = Database.QueryDataFromDevicesTable();
                foreach (var dictionary in devicesList)
                {
                    if (dictionary.ContainsValue(selectedUDID))
                    {
                        valueExists = true;
                        break;
                    }
                }
                if (valueExists)
                {
                    Database.DeleteDataFromDevicesTable(selectedUDID);
                    if (listView1.SelectedItems.Count > 0)
                    {
                        ListViewItem selectedItem = listView1.SelectedItems[0];
                        listView1.Items.Remove(selectedItem);
                        DeviceInfo.Remove(selectedUDID);
                        OpenDevice.deviceDetails.Remove(selectedUDID);
                    }
                    if (selectedOS.Equals("Android"))
                    {
                        AndroidMethods.GetInstance().DisconnectAndroidWireless(selectedDeviceIP);
                    }
                    else
                    {
                        if (udidProxyPort.ContainsKey(selectedUDID))
                        {
                            Common.KillProcessByPortNumber(udidProxyPort[selectedUDID]);
                            udidProxyPort.Remove(selectedUDID);
                        }
                        if (udidScreenPort.ContainsKey(selectedUDID))
                        {
                            Common.KillProcessByPortNumber(udidScreenPort[selectedUDID]);
                            udidScreenPort.Remove(selectedUDID);
                        }
                    }
                    if (ScreenControl.udidScreenControl.ContainsKey(selectedUDID))
                    {
                        ScreenControl.udidScreenControl[selectedUDID].Close();
                        ScreenControl.udidScreenControl.Remove(selectedUDID);
                    }
                    DeleteDevice.Enabled = false;
                    Open.Enabled = false;
                    MessageBox.Show(selectedDeviceName + " removed successfully.", "Delete Device", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    GoogleAnalytics.SendEvent("Device_Deleted");
                }
                else
                {
                    MessageBox.Show("Device not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    GoogleAnalytics.SendEvent("DeleteDevice_Device_Not_Found_Error");
                }
            }
        }


        private async void iOSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Owner = this;
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Detect iOS Device", "Looking for iOS device......", 10);
            List<string> deviceList = new List<string>();
            Dictionary<string, object> deviceInfo = new Dictionary<string, object>();
            try
            {
                await Task.Run(() =>
                {
                    deviceList = iOSMethods.GetInstance().GetListOfDevicesUDID();
                });

                if (deviceList.Contains("ITunes not installed"))
                {
                    commonProgress.Close();
                    var result = MessageBox.Show("Apple Mobile Device Service required for iOS automation. Please Install ITunes and try again.\n\nDo you want to download now?", "Add iOS Device", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    GoogleAnalytics.SendEvent("iTunes_Not_Installed_Error");
                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            ProcessStartInfo psInfo = new ProcessStartInfo
                            {
                                FileName = "https://support.apple.com/en-in/HT210384",
                                UseShellExecute = true
                            };
                            Process.Start(psInfo);
                            GoogleAnalytics.SendEvent("Download_iTunes", "Yes");
                        }
                        catch (Exception exception)
                        {
                            GoogleAnalytics.SendExceptionEvent("Download_iTunes", exception.Message);
                        }
                    }
                    else
                    {
                        GoogleAnalytics.SendEvent("Download_iTunes", "No");
                    }
                }
                else
                {
                    int count = deviceList.Count;
                    if (count == 0)
                    {
                        commonProgress.Close();
                        MessageBox.Show("No iOS Device available. Please check device connectivity.", "Add iOS Device", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        commonProgress.UpdateStepLabel("Detect iOS Device", "Getting iOS device information......", 50);
                        for (int i = 0; i < count; i++)
                        {
                            if (!isDeviceAlreadyAdded(deviceList[i]))
                            {
                                DeviceInformation deviceInformation = new DeviceInformation(main);
                                await Task.Run(() =>
                                {
                                    deviceInfo = iOSMethods.GetInstance().GetDeviceInformation(deviceList[i]);
                                });
                                commonProgress.UpdateStepLabel("Detect iOS Device", "Getting iOS device information......", 70);
                                if (deviceInfo.Count > 0)
                                {
                                    try
                                    {
                                        await Task.Run(() =>
                                        {
                                            Model = iOSMethods.GetInstance().GetDeviceModel(deviceInfo["ProductType"]?.ToString() ?? "");
                                        });
                                    }
                                    catch (Exception)
                                    {
                                        Model = deviceInfo["ProductType"]?.ToString() ?? "";
                                    }
                                    commonProgress.UpdateStepLabel("Detect iOS Device", "Getting iOS device information......", 90);
                                    DeviceName = deviceInfo["DeviceName"]?.ToString().Replace("â€™", "'") ?? "";
                                    OSVersion = deviceInfo["ProductVersion"]?.ToString() ?? "";
                                    udid = deviceInfo["UniqueDeviceID"]?.ToString() ?? "";
                                    OSType = "iOS";
                                    string connectedVia = string.Empty;
                                    if (deviceInfo.ContainsKey("HostAttached"))
                                    {
                                        connectedVia = iOSMethods.iOSConnectedVia((bool)deviceInfo["HostAttached"]);
                                    }
                                    else if (deviceInfo.ContainsKey("ConnectionType"))
                                    {
                                        connectedVia = deviceInfo["ConnectionType"]?.ToString() ?? "";
                                    }
                                    else
                                    {
                                        connectedVia = "";
                                    }
                                    string[] name = { "Name", DeviceName };
                                    string[] version = { "OS", OSType };
                                    string[] os = { "Version", OSVersion };
                                    string[] UniqueDeviceID = { "Udid", udid };
                                    string[] DeviceModel = { "Model", Model };
                                    string[] Connection = { "Connection Type", connectedVia };
                                    deviceInformation.infoListView.Items.Add(new ListViewItem(name));
                                    deviceInformation.infoListView.Items.Add(new ListViewItem(os));
                                    deviceInformation.infoListView.Items.Add(new ListViewItem(version));
                                    deviceInformation.infoListView.Items.Add(new ListViewItem(DeviceModel));
                                    deviceInformation.infoListView.Items.Add(new ListViewItem(UniqueDeviceID));
                                    deviceInformation.infoListView.Items.Add(new ListViewItem(Connection));
                                    commonProgress.Close();
                                    deviceInformation.ShowDialog();
                                    GoogleAnalytics.SendEvent("DeviceInformation_iOS");
                                    break;
                                }
                            }
                            else
                            {
                                if (i == count - 1)
                                {
                                    commonProgress.Close();
                                    MessageBox.Show("No New iOS Device available. Please check device connectivity.", "Add iOS Device", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    GoogleAnalytics.SendEvent("No_iOS_Device_Available");
                                }
                            }
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                commonProgress.Close();
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                GoogleAnalytics.SendExceptionEvent("AddDevice_iOS_Clicked", ex.Message);
            }
        }



        private async void androidToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Owner = this;
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Detect Android Device", "Looking for Android device......", 10);
            List<string> deviceList = new List<string>();
            Dictionary<string, string> deviceInfo = new Dictionary<string, string>();
            try
            {
                await Task.Run(() =>
                {
                    deviceList = AndroidMethods.GetInstance().GetListOfDevicesUDID();
                });
                int count = deviceList.Count;
                if (count == 0)
                {
                    commonProgress.Close();
                    MessageBox.Show("No Android Device available. Please check device connectivity.\nMake sure USB debugging option enabled in your phone's Developer options.", "Add Android Device", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    commonProgress.UpdateStepLabel("Detect Android Device", "Getting Android device information......", 50);
                    for (int i = 0; i < count; i++)
                    {
                        if (!isDeviceAlreadyAdded(deviceList[i]))
                        {
                            DeviceInformation deviceInformation = new DeviceInformation(main);
                            await Task.Run(() =>
                            {
                                deviceInfo = AndroidAsyncMethods.GetInstance().GetDeviceInformation(deviceList[i]);
                            });
                            if (deviceInfo.Count > 0)
                            {
                                commonProgress.UpdateStepLabel("Detect Android Device", "Getting Android device information......", 75);
                                udid = deviceInfo["ro.serialno"]?.ToString() ?? "";
                                OSType = "Android";
                                screenWidth = deviceInfo["Width"];
                                screenHeight = deviceInfo["Height"];
                                string Brand = string.Empty;
                                if (deviceInfo.ContainsKey("deviceName"))
                                {
                                    DeviceName = deviceInfo["deviceName"]?.ToString() ?? "";
                                }
                                else
                                {
                                    //DeviceName = deviceInfo["ro.product.model"]?.ToString() ?? "";
                                    DeviceName = "No information";
                                }
                                if (deviceInfo.ContainsKey("ro.build.version.release"))
                                {
                                    OSVersion = deviceInfo["ro.build.version.release"]?.ToString() ?? "";
                                }
                                else
                                {
                                    OSVersion = "No information";
                                }
                                if (deviceInfo.ContainsKey("ro.product.model"))
                                {
                                    Model = deviceInfo["ro.product.model"]?.ToString() ?? "";
                                }
                                else
                                {
                                    Model = "No information";
                                }
                                if (deviceInfo.ContainsKey("ro.product.product.brand"))
                                {
                                    Brand = deviceInfo["ro.product.product.brand"]?.ToString() ?? "";
                                }
                                else if (deviceInfo.ContainsKey("ro.product.vendor.brand"))
                                {
                                    Brand = deviceInfo["ro.product.vendor.brand"]?.ToString() ?? "";
                                }
                                else
                                {
                                    Brand = "No information";
                                }
                                string BrandPlusModel = Brand.Replace("\n", "") + " " + Model.Replace("\n", "");
                                string[] name = { "Name", DeviceName.Replace("\n", "") };
                                string[] os = { "OS", OSType.Replace("\n", "") };
                                string[] version = { "Version", OSVersion.Replace("\n", "") };
                                string[] UniqueDeviceID = { "Udid", udid.Replace("\n", "") };
                                BrandPlusModel = BrandPlusModel.Substring(0, 1).ToUpper() + BrandPlusModel.Substring(1);
                                string[] DeviceModel = { "Model", BrandPlusModel };
                                string[] ScreenWidth = { "Width", screenWidth };
                                string[] ScreenHeight = { "Height", screenHeight };
                                string[] Connection = { "Connection Type", "USB" };
                                deviceInformation.infoListView.Items.Add(new ListViewItem(name));
                                deviceInformation.infoListView.Items.Add(new ListViewItem(DeviceModel));
                                deviceInformation.infoListView.Items.Add(new ListViewItem(os));
                                deviceInformation.infoListView.Items.Add(new ListViewItem(version));
                                deviceInformation.infoListView.Items.Add(new ListViewItem(UniqueDeviceID));
                                deviceInformation.infoListView.Items.Add(new ListViewItem(ScreenWidth));
                                deviceInformation.infoListView.Items.Add(new ListViewItem(ScreenHeight));
                                deviceInformation.infoListView.Items.Add(new ListViewItem(Connection));
                                commonProgress.Close();
                                deviceInformation.ShowDialog();
                                GoogleAnalytics.SendEvent("DeviceInformation_Android");
                                break;
                            }
                        }
                        else
                        {
                            if (i == count - 1)
                            {
                                commonProgress.Close();
                                MessageBox.Show("No New Android Device available. Please check device connectivity.", "Add Android Device", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                GoogleAnalytics.SendEvent("No_Android_Device_Available");
                            }
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                commonProgress.Close();
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                GoogleAnalytics.SendExceptionEvent("AddDevice_Android_Clicked", ex.Message);
            }
        }

        bool isMessageDisplayed = false;
        private void MoreButton_Click(object sender, EventArgs e)
        {
            if (selectedOS.Equals("iOS") && !isMessageDisplayed)
            {
                isMessageDisplayed = true;
                MessageBox.Show("Make sure to open the iOS device before performing any operation from More section or some operations may not work.\n\nOpening device won't be required in future releases.\n\nThis is a one time message for an application lifecycle.", "More Operations", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            Point screenPoint = MoreButton.PointToScreen(new Point(0, MoreButton.Height));
            contextMenuStrip4.Show(screenPoint);
        }


        public static async Task InstalliOSApp(string selectedDeviceName, string selectedUDID, string filePath)
        {
            CommonProgress commonProgress = new CommonProgress();
            string fileName = Path.GetFileName(filePath);
            commonProgress.Owner = main;
            commonProgress.Show();
            Dictionary<string, string> IPAInfo = new Dictionary<string, string>();
            string bundleId = "", appName = "", version = "";
            bool isPListRead = false;
            try
            {
                await Task.Run(() =>
                {
                    commonProgress.UpdateStepLabel("Install App", "Getting IPA information...", 10);
                    IPAInfo = iOSMethods.GetInstance().GetIPAInformation(filePath);
                    commonProgress.UpdateStepLabel("Install App", "Getting IPA information...", 30);
                    bundleId = IPAInfo["CFBundleIdentifier"];
                    appName = IPAInfo["CFBundleExecutable"];
                    version = IPAInfo["CFBundleShortVersionString"] + "[" + IPAInfo["CFBundleVersion"] + "]";
                    isPListRead = true;
                });
            }
            catch (Exception)
            {
                isPListRead = false;
            }
            bool completed = false; int percent = 5; int countPercentageforPreparing = 31;
            _ = Task.Run(async () =>
            {
                commonProgress.UpdateStepLabel("Install App", "Installing " + appName + "(" + version + ")" + " into " + selectedDeviceName + "\nPercentage Completion: " + iOSAsyncMethods.installationProgress + " % ", percent);
                while (!completed)
                {
                    try
                    {
                        percent = int.Parse(iOSAsyncMethods.installationProgress);
                    }
                    catch (Exception)
                    {
                    }

                    await commonProgress.Invoke(async () =>
                    {
                        if (percent == 0)
                        {
                            commonProgress.UpdateStepLabel("Install App", "Preparing for installation. Please wait, this may take some time depending on the file size...", countPercentageforPreparing);
                            countPercentageforPreparing++;
                            await Task.Delay(1000);
                        }
                        else
                        {
                            if (isPListRead)
                            {
                                commonProgress.UpdateStepLabel("Install App", "Installing " + appName + "(" + version + ")" + " into " + selectedDeviceName + "\nPercentage Completion: " + iOSAsyncMethods.installationProgress + " % ", percent);
                            }
                            else
                            {
                                commonProgress.UpdateStepLabel("Install App", "Installing " + fileName + " into " + selectedDeviceName + "\nPercentage Completion: " + iOSAsyncMethods.installationProgress + " % ", percent);
                            }
                        }
                    });
                }
            });
            await Task.Run(() =>
            {
                iOSAsyncMethods.GetInstance().InstallApp(selectedUDID, filePath);
                completed = true;
            });
            if (iOSAsyncMethods.installationProgress.Contains("installation successful") | iOSAsyncMethods.installationProgress == "100")
            {
                commonProgress.Close();
                if (isPListRead)
                {
                    DialogResult result = MessageBox.Show("Installation Successful. Would you like to launch the app?", "Launch App", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (result == DialogResult.Yes)
                    {
                        var output = iOSMethods.GetInstance().LaunchApp(selectedUDID, bundleId);
                        if (output.Contains("profile has not been explicitly trusted by the user"))
                        {
                            MessageBox.Show("Unable to launch " + appName + " because it has an invalid code signature, inadequate entitlements or its profile has not been explicitly trusted by the user.\n\nTo trust a profile on the device, go to Settings-> General-> VPN & Device Management-> Select Profile-> Trust.", "Launch Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            GoogleAnalytics.SendEvent("Launch_iOS_App_Profile_Not_Trusted");
                        }
                        else if (output.Contains("Process started successfully"))
                        {
                            GoogleAnalytics.SendEvent("Install_iOS_App_Launched");
                        }
                        GoogleAnalytics.SendEvent("Launch_Installed_iOS_App", "Yes");
                    }
                    else
                    {
                        GoogleAnalytics.SendEvent("Launch_Installed_iOS_App", "No");
                    }
                }
                else
                {
                    DialogResult result = MessageBox.Show("Installation Successful. Launch option not available. Please launch the app manually.", "Install App", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    GoogleAnalytics.SendEvent("Install_iOS_App_Launch_Option_NA");
                }
                GoogleAnalytics.SendEvent("Install_iOS_App_Success");

            }
            else
            {
                commonProgress.Close();
                if (iOSAsyncMethods.installationProgress.Contains("MismatchedApplicationIdentifierEntitlement"))
                {
                    MessageBox.Show("Uninstall the existing " + appName + " app and Try again...", "Intallation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    GoogleAnalytics.SendEvent("Install_iOS_App_Failed", "MismatchedApplicationIdentifierEntitlement");
                }
                else
                {
                    MessageBox.Show(iOSAsyncMethods.installationProgress, "Intallation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    GoogleAnalytics.SendEvent("Install_iOS_App_Failed");
                }
            }
            iOSAsyncMethods.installationProgress = "0";
            commonProgress.Close();
        }


        private async void serverSetupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Common.IsNodeInstalled())
            {
                ServerConfig serverSetup = new ServerConfig();
                serverSetup.Owner = this;
                await serverSetup.isServerRunning(main);
                serverSetup.ShowDialog();
            }
            else
            {
                var result = MessageBox.Show("NodeJS not installed OR not added into environment variables.\nPlease Install NodeJs and then try again.\n\nClick OK to open the Troubleshooter to fix the issues.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (result == DialogResult.OK)
                {
                    TroubleShooter troubleShooter = new TroubleShooter();
                    await troubleShooter.FindIssues(main);
                    troubleShooter.ShowDialog();
                    GoogleAnalytics.SendEvent("Server_Node_Not_Installed_Show_Trouble");
                }
                GoogleAnalytics.SendEvent("Server_Config_Node_Not_Installed");
                //var result = MessageBox.Show("NodeJS not installed OR not added into environment variables.\nPlease Install NodeJs and then try again.\n\nDo you want to download now?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                //if (result == DialogResult.Yes)
                //{
                //    try
                //    {
                //        ProcessStartInfo psInfo = new ProcessStartInfo
                //        {
                //            FileName = "https://nodejs.org/en/download",
                //            UseShellExecute = true
                //        };
                //        Process.Start(psInfo);
                //    }
                //    catch (Exception exception)
                //    {
                //        Console.WriteLine("Exception" + exception);
                //    }
                //}
            }
            GoogleAnalytics.SendEvent("Server_Configuration_Clicked");
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }

        private void inspectorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessStartInfo psInfo = new ProcessStartInfo
                {
                    FileName = "https://inspector.appiumpro.com/",
                    UseShellExecute = true
                };
                Process.Start(psInfo);
                GoogleAnalytics.SendEvent("InspectorToolStripMenuItem_Click");
            }
            catch (Exception exception)
            {
                GoogleAnalytics.SendExceptionEvent("InspectorToolStripMenuItem_Click", exception.Message);
            }
        }

        private void xCUITestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessStartInfo psInfo = new ProcessStartInfo
                {
                    FileName = "https://appium.github.io/appium-xcuitest-driver/latest/reference/scripts/",
                    UseShellExecute = true
                };
                Process.Start(psInfo);
                GoogleAnalytics.SendEvent("XCUITestToolStripMenuItem_Click");
            }
            catch (Exception exception)
            {
                GoogleAnalytics.SendEvent("XCUITestToolStripMenuItem_Click", exception.Message);
            }
        }

        private void uIAutomatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessStartInfo psInfo = new ProcessStartInfo
                {
                    FileName = "https://github.com/appium/appium-uiautomator2-driver",
                    UseShellExecute = true
                };
                Process.Start(psInfo);
                GoogleAnalytics.SendEvent("UIAutomatorToolStripMenuItem_Click");
            }
            catch (Exception exception)
            {
                GoogleAnalytics.SendExceptionEvent("UIAutomatorToolStripMenuItem_Click", exception.Message);
            }
        }

        private void fAQToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string faqFilePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\Resources\\Docs\\Appium Wizard Troubleshooting Guide.html";
            try
            {
                ProcessStartInfo psInfo = new ProcessStartInfo
                {
                    FileName = faqFilePath,
                    UseShellExecute = true
                };
                Process.Start(psInfo);
                GoogleAnalytics.SendEvent("FAQToolStripMenuItem_Click");
            }
            catch (Exception exception)
            {
                GoogleAnalytics.SendExceptionEvent("FAQToolStripMenuItem_Click", exception.Message);
            }
        }

        private void iOSProfileManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            iOSProfileManagement iOSProfileManagement = new iOSProfileManagement();
            iOSProfileManagement.ShowDialog();
        }


        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
            }
            else
            {
                richTextBox1.ScrollBars = RichTextBoxScrollBars.Both;
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
            }
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                richTextBox2.SelectionStart = richTextBox2.Text.Length;
                richTextBox2.ScrollToCaret();
            }
            else
            {
                richTextBox2.ScrollBars = RichTextBoxScrollBars.Both;
                richTextBox2.SelectionStart = richTextBox2.Text.Length;
            }
        }

        private void richTextBox3_TextChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                richTextBox3.SelectionStart = richTextBox3.Text.Length;
                richTextBox3.ScrollToCaret();
            }
            else
            {
                richTextBox3.ScrollBars = RichTextBoxScrollBars.Both;
                richTextBox3.SelectionStart = richTextBox3.Text.Length;
            }
        }


        private void richTextBox4_TextChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                richTextBox4.SelectionStart = richTextBox4.Text.Length;
                richTextBox4.ScrollToCaret();
            }
            else
            {
                richTextBox4.ScrollBars = RichTextBoxScrollBars.Both;
                richTextBox4.SelectionStart = richTextBox4.Text.Length;
            }
        }


        private void richTextBox5_TextChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                richTextBox5.SelectionStart = richTextBox5.Text.Length;
                richTextBox5.ScrollToCaret();
            }
            else
            {
                richTextBox5.ScrollBars = RichTextBoxScrollBars.Both;
                richTextBox5.SelectionStart = richTextBox5.Text.Length;
            }
        }

        private void listView1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listView1.FocusedItem.Bounds.Contains(e.Location))
                {
                    contextMenuStrip2.Show(Cursor.Position);
                }
            }
        }

        private void copyUDIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                string udid = listView1.SelectedItems[0].SubItems[4].Text;
                Clipboard.SetText(udid);
                GoogleAnalytics.SendEvent("Copy_UDID");
            }
        }

        private async void fixInstallationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TroubleShooter troubleShooter = new TroubleShooter();
            await troubleShooter.FindIssues(main);
            troubleShooter.ShowDialog(this);
        }

        private void AutoScrollCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                GoogleAnalytics.SendEvent("Auto_Scroll_CheckBox_Checked");
            }
            else
            {
                GoogleAnalytics.SendEvent("Auto_Scroll_CheckBox_Unchecked");
            }
        }

        private void androidWiFiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AndroidWireless androidWireless = new AndroidWireless(main);
            androidWireless.ShowDialog();
        }

        private void label3_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip3.Show(Cursor.Position);
            }
        }

        private void rebootDeviceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to Reboot the following device?\n\n" + selectedDeviceName, "Reboot Device", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                var finalconfirm = MessageBox.Show("After rebooting, you will need to manually enter the passcode/trust the computer/allow permission to use the device with Appium Wizard. Therefore, only reboot the device if you are nearby and able to perform these operations.\n\nStill do you want to Reboot the following device?\n" + selectedDeviceName, "Reboot Device", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (finalconfirm == DialogResult.Yes)
                {
                    if (selectedOS.Equals("iOS"))
                    {
                        iOSMethods.GetInstance().RebootDevice(selectedUDID);
                        GoogleAnalytics.SendExceptionEvent("Reboot_iOS");
                    }
                    else
                    {
                        AndroidMethods.GetInstance().RebootDevice(selectedUDID);
                        GoogleAnalytics.SendExceptionEvent("Reboot_Android");
                    }
                    MessageBox.Show("Reboot Initiated for " + selectedDeviceName + ".", "Reboot Device", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    GoogleAnalytics.SendExceptionEvent("Reboot_Cancelled_SecondPopup");
                }
            }
            else
            {
                GoogleAnalytics.SendExceptionEvent("Reboot_Cancelled_FirstPopup");
            }
        }

        private async void installAppToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedDeviceStatus.Equals("Online"))
            {
                OpenFileDialog dialog = new OpenFileDialog();
                if (selectedOS.Equals("iOS"))
                {
                    dialog.Filter = "IPA files (*.ipa)|*.ipa";
                    dialog.Title = "Select an IPA file";

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = dialog.FileName;
                        string fileName = Path.GetFileName(filePath);
                        InstalliOSApp installApp = new InstalliOSApp(selectedDeviceName, selectedUDID, filePath, fileName);
                        installApp.ShowDialog();
                    }
                }
                else
                {
                    CommonProgress commonProgress = new CommonProgress();
                    dialog.Filter = "APK files (*.apk)|*.apk";
                    dialog.Title = "Select an APK file";

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = dialog.FileName;
                        string fileName = Path.GetFileName(filePath);
                        commonProgress.Text = "Install App";
                        commonProgress.Owner = this;
                        commonProgress.Show();
                        commonProgress.UpdateStepLabel("Install App", "Getting APK Information...", 20);
                        bool completed = false;
                        Dictionary<string, string> APKInfo = new Dictionary<string, string>();
                        string appName = "", packageName = "", activityName = "", version = "";
                        bool isAPKInfoRead;
                        try
                        {
                            await Task.Run(() =>
                            {
                                APKInfo = AndroidMethods.GetInstance().GetApkInformation(filePath);
                            });
                            appName = APKInfo["AppName"];
                            packageName = APKInfo["PackageName"];
                            activityName = APKInfo["ActivityName"];
                            version = APKInfo["Version"];
                            isAPKInfoRead = true;
                        }
                        catch (Exception ex)
                        {
                            GoogleAnalytics.SendExceptionEvent("GetApkInformation", ex.Message);
                            isAPKInfoRead = false;
                        }
                        bool isInstalled = false;
                        int countPercentageforPreparing = 20;
                        _ = Task.Run(async () =>
                        {
                            while (!completed)
                            {
                                await commonProgress.Invoke(async () =>
                                {
                                    if (isAPKInfoRead)
                                    {
                                        commonProgress.UpdateStepLabel("Install App", "Installing " + appName + "(" + version + ") into " + selectedDeviceName, countPercentageforPreparing);
                                    }
                                    else
                                    {
                                        commonProgress.UpdateStepLabel("Install App", "Installing " + fileName + " into " + selectedDeviceName, countPercentageforPreparing);
                                    }
                                    countPercentageforPreparing = countPercentageforPreparing + 5;
                                    await Task.Delay(1000);
                                });
                            }
                        });

                        await Task.Run(() =>
                        {
                            isInstalled = AndroidMethods.GetInstance().InstallApp(selectedUDID, filePath);
                            completed = true;
                        });

                        if (isInstalled)
                        {
                            if (isAPKInfoRead)
                            {
                                DialogResult result = MessageBox.Show("Installation Successful. Would you like to launch the app?", "Launch App", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                                if (result == DialogResult.Yes)
                                {
                                    await Task.Run(() =>
                                    {
                                        AndroidMethods.GetInstance().LaunchApp(selectedUDID, packageName, activityName);
                                    });
                                    GoogleAnalytics.SendEvent("Launch_Installed_Android_App", "Yes");
                                }
                                else
                                {
                                    GoogleAnalytics.SendEvent("Launch_Installed_Android_App", "No");
                                }
                            }
                            else
                            {
                                DialogResult result = MessageBox.Show("Installation Successful. Launch option not available. Please launch the app manually.", "Install App", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                GoogleAnalytics.SendEvent("Install_Android_App_Launch_Option_NA");
                            }
                            GoogleAnalytics.SendEvent("Install_Android_App_Success");
                        }
                        commonProgress.Close();
                    }
                }
            }
            else
            {
                GoogleAnalytics.SendEvent("InstallApp_Device_Offline");
                MessageBox.Show("Please check device connectivity...", "Device Offline", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            GoogleAnalytics.SendEvent("InstallApp_Clicked");
        }

        private void refreshStatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshDeviceListView();
            GoogleAnalytics.SendEvent("Refresh_Status");
        }

        private async void launchAppToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InstalledAppsList installedAppsList = new InstalledAppsList(selectedOS, selectedUDID, selectedDeviceName);
            await installedAppsList.GetInstalledAppsList(main);
            installedAppsList.ShowDialog();
        }

        private async void takeScreenshotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Files (*.png)|*.png";
            saveFileDialog.Title = "Save PNG File";
            saveFileDialog.ShowDialog();

            if (selectedOS.Equals("Android"))
            {
                if (saveFileDialog.FileName != "")
                {
                    CommonProgress commonProgress = new CommonProgress();
                    commonProgress.UpdateStepLabel("Take Screenshot", "Please wait while taking screenshot of " + selectedDeviceName + "...");
                    commonProgress.Owner = this;
                    commonProgress.Show();
                    string filePath = saveFileDialog.FileName;
                    filePath = "\"" + filePath + "\"";
                    try
                    {
                        await Task.Run(() =>
                        {
                            if (selectedDeviceConnection.Equals("Wi-Fi"))
                            {
                                AndroidMethods.GetInstance().TakeScreenshot(selectedDeviceIP, filePath);
                            }
                            else
                            {
                                AndroidMethods.GetInstance().TakeScreenshot(selectedUDID, filePath);
                            }
                        });
                        commonProgress.Close();
                    }
                    catch (Exception)
                    {
                        commonProgress.Close();
                        MessageBox.Show("Failed to Take Screenshot", "Take Screenshot", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    commonProgress.Close();
                    GoogleAnalytics.SendEvent("Android_TakeScreenshot");
                }
            }
            else
            {
                if (saveFileDialog.FileName != "")
                {
                    CommonProgress commonProgress = new CommonProgress();
                    commonProgress.UpdateStepLabel("Take Screenshot", "Please wait while taking screenshot of " + selectedDeviceName + "...");
                    commonProgress.Owner = this;
                    commonProgress.Show();
                    string filePath = saveFileDialog.FileName;
                    filePath = "\"" + filePath + "\"";
                    try
                    {
                        await Task.Run(() =>
                        {
                            iOSMethods.GetInstance().TakeScreenshot(selectedUDID, filePath);
                        });
                        commonProgress.Close();
                    }
                    catch (Exception)
                    {
                        commonProgress.Close();
                        MessageBox.Show("Failed to Take Screenshot", "Take Screenshot", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    GoogleAnalytics.SendEvent("iOS_TakeScreenshot");
                }
            }
        }

        private void signIPAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var profilesList = iOSProfileManagement.FetchProfiles();
            if (profilesList.Count == 0)
            {
                MessageBox.Show("Provisioning Profiles not found. First Import profile in Tools->iOS Profile Management and then try again.", "Provisioning Profiles not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                GoogleAnalytics.SendEvent("Profiles_Not_Available_Popup");
            }
            else
            {
                SignIPA signIPA = new SignIPA(profilesList);
                signIPA.ShowDialog();
            }
        }

        private void reportAnIssueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessStartInfo psInfo = new ProcessStartInfo
                {
                    FileName = "https://github.com/mega6453/AppiumWizard/issues/new/choose",
                    UseShellExecute = true
                };
                Process.Start(psInfo);
                GoogleAnalytics.SendEvent("ReportAnIssueToolStripMenuItem_Click");
            }
            catch (Exception exception)
            {
                GoogleAnalytics.SendExceptionEvent("ReportAnIssueToolStripMenuItem_Click", exception.Message);
            }
        }

        private void startADiscussionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessStartInfo psInfo = new ProcessStartInfo
                {
                    FileName = "https://github.com/mega6453/AppiumWizard/discussions/new/choose",
                    UseShellExecute = true
                };
                Process.Start(psInfo);
                GoogleAnalytics.SendEvent("StartADiscussionToolStripMenuItem_Click");
            }
            catch (Exception exception)
            {
                GoogleAnalytics.SendExceptionEvent("StartADiscussionToolStripMenuItem_Click", exception.Message);
            }
        }

        private async void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> releaseInfo = new Dictionary<string, string>();
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Owner = this;
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Check for Appium Wizard updates", "Please wait while checking for updates...");
            GoogleAnalytics.SendEvent("checkForUpdatesToolStripMenuItem_Click");
            bool isInternetAvailable = false;
            await Task.Run(() =>
            {
                isInternetAvailable = Common.isInternetAvailable();
            });
            if (isInternetAvailable)
            {
                try
                {
                    await Task.Run(() =>
                    {
                        releaseInfo = Common.GetLatestReleaseInfo();
                    });
                    string tagName = releaseInfo["tag_name"];
                    string releaseNotes = releaseInfo["body"].Trim();
                    Version thisAppVersion = new Version(VersionInfo.VersionNumber);
                    Version latestVersionObj = new Version(tagName.Substring(1));

                    bool isUpdateAvailable = latestVersionObj > thisAppVersion;

                    if (isUpdateAvailable)
                    {
                        commonProgress.Close();
                        var result = MessageBox.Show("Appium Wizard new version " + tagName + " is available.\n\nRelease Notes:\n" + releaseNotes + " \n\nWould you like to open the download page now?", "Check for Updates...", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            try
                            {
                                ProcessStartInfo psInfo = new ProcessStartInfo
                                {
                                    FileName = "https://github.com/mega6453/AppiumWizard/releases/latest",
                                    UseShellExecute = true
                                };
                                Process.Start(psInfo);
                                GoogleAnalytics.SendEvent("DownloadUpdate_Yes");
                            }
                            catch (Exception exception)
                            {
                                GoogleAnalytics.SendExceptionEvent("DownloadUpdate_Yes", exception.Message);
                            }
                        }
                        else
                        {
                            GoogleAnalytics.SendEvent("DownloadUpdate_No");
                        }
                    }
                    else
                    {
                        commonProgress.Close();
                        MessageBox.Show("No new updates available at this moment. Please check again later.", "Check for Updates...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                }
                catch (Exception ex)
                {
                    commonProgress.Close();
                    MessageBox.Show("Unhandled Exception", "Check for Updates...");
                    GoogleAnalytics.SendExceptionEvent("checkForUpdatesToolStripMenuItem_Click", ex.Message);
                }
            }
            else
            {
                commonProgress.Close();
                MessageBox.Show("Internet connection not available. Please connect to internet and try again.", "Check for Updates...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                GoogleAnalytics.SendEvent("checkForUpdatesToolStripMenuItem_Click", "No_Internet");
            }
            commonProgress.Close();
        }

        private async void updaterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Updater updater = new Updater();
            await updater.GetVersionInformation(main);
            updater.ShowDialog();
        }

        private void capabilityCopyButton_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(selectedDeviceCapability);
            capabilityCopyButton.BackgroundImage = Properties.Resources.tick;
            timer1.Start();
            GoogleAnalytics.SendEvent("Copy_Capability");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            capabilityCopyButton.BackgroundImage = Properties.Resources.files; // Replace with your image resource
            timer1.Stop();
        }

        private void serverSecurityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessStartInfo psInfo = new ProcessStartInfo
                {
                    FileName = "https://appium.io/docs/en/latest/guides/security/",
                    UseShellExecute = true
                };
                Process.Start(psInfo);
                GoogleAnalytics.SendEvent("serverSecurityToolStripMenuItem_Click");
            }
            catch (Exception exception)
            {
                GoogleAnalytics.SendExceptionEvent("serverSecurityToolStripMenuItem_Click", exception.Message);
            }
        }

        private void cLIArgumentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessStartInfo psInfo = new ProcessStartInfo
                {
                    FileName = "https://appium.io/docs/en/latest/cli/args/",
                    UseShellExecute = true
                };
                Process.Start(psInfo);
                GoogleAnalytics.SendEvent("cLIArgumentsToolStripMenuItem_Click");
            }
            catch (Exception exception)
            {
                GoogleAnalytics.SendExceptionEvent("cLIArgumentsToolStripMenuItem_Click", exception.Message);
            }
        }

        private void sessionCapsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessStartInfo psInfo = new ProcessStartInfo
                {
                    FileName = "https://appium.io/docs/en/latest/guides/caps/",
                    UseShellExecute = true
                };
                Process.Start(psInfo);
                GoogleAnalytics.SendEvent("sessionCapsToolStripMenuItem_Click");
            }
            catch (Exception exception)
            {
                GoogleAnalytics.SendEvent("sessionCapsToolStripMenuItem_Click", exception.Message);
            }
        }

        private void xCUITestCapsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessStartInfo psInfo = new ProcessStartInfo
                {
                    FileName = "https://appium.github.io/appium-xcuitest-driver/latest/reference/capabilities/",
                    UseShellExecute = true
                };
                Process.Start(psInfo);
                GoogleAnalytics.SendEvent("xCUITestCapsToolStripMenuItem_Click");
            }
            catch (Exception exception)
            {
                GoogleAnalytics.SendEvent("xCUITestCapsToolStripMenuItem_Click", exception.Message);
            }
        }

        private void uIAutomator2CapsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessStartInfo psInfo = new ProcessStartInfo
                {
                    FileName = "https://github.com/appium/appium-uiautomator2-driver?tab=readme-ov-file#capabilities",
                    UseShellExecute = true
                };
                Process.Start(psInfo);
                GoogleAnalytics.SendEvent("uIAutomator2CapsToolStripMenuItem_Click");
            }
            catch (Exception exception)
            {
                GoogleAnalytics.SendEvent("uIAutomator2CapsToolStripMenuItem_Click", exception.Message);
            }
        }

        private void sessionSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessStartInfo psInfo = new ProcessStartInfo
                {
                    FileName = "https://appium.io/docs/en/latest/guides/settings/",
                    UseShellExecute = true
                };
                Process.Start(psInfo);
                GoogleAnalytics.SendEvent("sessionSettingsToolStripMenuItem_Click");
            }
            catch (Exception exception)
            {
                GoogleAnalytics.SendExceptionEvent("sessionSettingsToolStripMenuItem_Click", exception.Message);
            }
        }

        private void xCUITestSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessStartInfo psInfo = new ProcessStartInfo
                {
                    FileName = "https://appium.github.io/appium-xcuitest-driver/latest/reference/settings/",
                    UseShellExecute = true
                };
                Process.Start(psInfo);
                GoogleAnalytics.SendEvent("xCUITestSettingsToolStripMenuItem_Click");
            }
            catch (Exception exception)
            {
                GoogleAnalytics.SendEvent("xCUITestSettingsToolStripMenuItem_Click", exception.Message);
            }
        }

        private void uIAutomator2SettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessStartInfo psInfo = new ProcessStartInfo
                {
                    FileName = "https://github.com/appium/appium-uiautomator2-driver?tab=readme-ov-file#settings-api",
                    UseShellExecute = true
                };
                Process.Start(psInfo);
                GoogleAnalytics.SendEvent("uIAutomator2SettingsToolStripMenuItem_Click");
            }
            catch (Exception exception)
            {
                GoogleAnalytics.SendEvent("uIAutomator2SettingsToolStripMenuItem_Click", exception.Message);
            }
        }


        private void onFormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                CommonProgress commonProgress = new CommonProgress();
                commonProgress.Owner = this;
                commonProgress.Show();
                commonProgress.UpdateStepLabel("Exiting", "Please wait while closing all resources and exiting...");
                List<Form> childFormsToClose = new List<Form>();
                foreach (var item in runningProcesses)
                {
                    Common.KillProcessById(item);
                }
                foreach (var item in runningProcessesPortNumbers)
                {
                    Common.KillProcessByPortNumber(item);
                }
                foreach (var item in udidProxyPort)
                {
                    Common.KillProcessByPortNumber(item.Value);
                }
                foreach (var item in udidScreenPort)
                {
                    Common.KillProcessByPortNumber(item.Value);
                }
                foreach (Form form in Application.OpenForms)
                {
                    if (form != this && form != commonProgress)
                    {
                        childFormsToClose.Add(form);
                    }
                }
                foreach (Form formToClose in childFormsToClose)
                {
                    formToClose.Close();
                }
                GoogleAnalytics.SendEvent("App_Closed", "Closed");
                commonProgress.Close();
            }
            catch (Exception ex)
            {
                GoogleAnalytics.SendExceptionEvent("Exception_While_Closing_App", ex.Message);
            }
        }

        private void iOSNativeAppsBundleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessStartInfo psInfo = new ProcessStartInfo
                {
                    FileName = "https://support.apple.com/en-in/guide/deployment/depece748c41/web",
                    UseShellExecute = true
                };
                Process.Start(psInfo);
                GoogleAnalytics.SendEvent("iOSNativeAppsBundleToolStripMenuItem_Click");
            }
            catch (Exception exception)
            {
                GoogleAnalytics.SendEvent("iOSNativeAppsBundleToolStripMenuItem_Click", exception.Message);
            }
        }
    }
}
