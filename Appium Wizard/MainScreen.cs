using Appium_Wizard.Properties;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Newtonsoft.Json;
using NLog;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace Appium_Wizard
{
    public partial class MainScreen : Form
    {
        string? udid, DeviceName, OSVersion, OSType, selectedUDID, Model, screenWidth, screenHeight;
        public static MainScreen? main;
        string? selectedDeviceName, selectedOS, selectedDeviceStatus, selectedDeviceVersion, selectedDeviceIP, selectedDeviceModel, selectedDeviceConnection, selectedDeviceCapability;
        public static List<int> runningProcesses = new List<int>();
        public static List<int> runningProcessesPortNumbers = new List<int>();
        private int labelStartPosition; bool isUpdateAvailable;
        string? latestVersion;
        Dictionary<string, string> releaseInfo = new Dictionary<string, string>();
        public static Dictionary<string, Tuple<string, string>> DeviceInfo = new Dictionary<string, Tuple<string, string>>();
        public static Dictionary<string, int> udidProxyPort = new Dictionary<string, int>();
        public static Dictionary<string, int> udidScreenPort = new Dictionary<string, int>();
        public static bool DeviceConnectedNotification, DeviceDisconnectedNotification, ScreenshotNotification, ScreenRecordingNotification;
        public static bool alwaysOnTop;
        //public static List<string> UDIDPreInstalledWDA = new List<string>();
        public static Dictionary<string, string> UDIDPreInstalledWDA = new Dictionary<string, string>();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private Timer uncheckTimer; private Timer highlightTimer;
        public static Dictionary<int, WebView2> serverNumberWebView = new Dictionary<int, WebView2>();

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
            InitializeWebViews();
        }
        string defaultText = "Appium Server Not Running. Go to Server->Config to start the server...";
        private async void InitializeWebViews()
        {
            await ConfigureWebView(1, server1WebView, defaultText);
            await ConfigureWebView(2, server2WebView, defaultText);
            await ConfigureWebView(3, server3WebView, defaultText);
            await ConfigureWebView(4, server4WebView, defaultText);
            await ConfigureWebView(5, server5WebView, defaultText);
        }

        private async Task ConfigureWebView(int serverNumber, WebView2 webView, string defaultText)
        {
            serverNumberWebView[serverNumber] = webView;
            // Ensure the WebView2 environment is initialized
            await webView.EnsureCoreWebView2Async();

            // Attach ContextMenuRequested event to disable right-click
            webView.CoreWebView2.ContextMenuRequested += CoreWebView2_ContextMenuRequested;

            // Set default text using NavigateToString
            if (AppiumServerSetup.portServerNumberAndFilePath.ContainsKey(serverNumber))
            {
                StartLogsServer(serverNumber);
            }
            else
            {
                SetDefaultText(webView, defaultText);
            }
        }

        private void SetDefaultText(WebView2 webView, string defaultText)
        {
            string htmlContent = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            display: flex;
                            justify-content: center;
                            align-items: center;
                            height: 100%;
                            margin: 0;
                            background-color: #f0f0f0;
                        }}
                        div {{
                            text-align: center;
                            color: #555;
                            font-size: 20px;
                        }}
                    </style>
                </head>
                <body>
                    <div>{defaultText}</div>
                </body>
                </html>";
            webView.NavigateToString(htmlContent);
        }

        private void CoreWebView2_ContextMenuRequested(object sender, CoreWebView2ContextMenuRequestedEventArgs e)
        {
            // Suppress the context menu
            e.Handled = true;
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
                        string updateMessage = "Appium Wizard new version " + latestVersion + " is available for update. Go to \"Help -> Check for updates\" to open the download page.";
                        ShowMessage(updateMessage);
                    }
                }
                var result = Database.QueryDataFromNotificationsTable();
                DeviceConnectedNotification = result["DeviceConnected"].Equals("Enable");
                DeviceDisconnectedNotification = result["DeviceDisconnected"].Equals("Enable");
                ScreenshotNotification = result["Screenshot"].Equals("Enable");
                ScreenRecordingNotification = result["ScreenRecording"].Equals("Enable");

                alwaysOnTop = Database.QueryDataFromAlwaysOnTopTable().Equals("Yes");
                if (alwaysOnTop)
                {
                    yesToolStripMenuItem.Image = Resources.check_mark;
                }
                else
                {
                    noToolStripMenuItem.Image = Resources.check_mark;
                }

                UDIDPreInstalledWDA = Database.QueryUDIDsAndBundleIdsFromUsePreInstalledWDAList().ToDictionary();
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
                server1WebView.Size = size;
                server2WebView.Size = size;
                server3WebView.Size = size;
                server4WebView.Size = size;
                server5WebView.Size = size;
                openLogsButton.Location = new Point(tabControl1.Right - openLogsButton.Width, tabControl1.Top);
            }
            GoogleAnalytics.SendEvent("App_Version", VersionInfo.VersionNumber);
        }

        private void ShowMessage(string message)
        {
            tableLayoutPanel1.Visible = true;
            label1.Text = message;
            label1.AutoSize = true;
            label1.Anchor = AnchorStyles.None;

            Button closeButton = new Button
            {
                Text = "X",
                AutoSize = true,
                FlatStyle = FlatStyle.Flat
            };
            closeButton.FlatAppearance.BorderSize = 0;

            tableLayoutPanel1.Width = this.Width - 50;
            tableLayoutPanel1.Controls.Clear(); // Ensure no duplicate controls
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(closeButton, 1, 0);

            closeButton.Click += (sender, e) =>
            {
                Controls.Remove(tableLayoutPanel1);
            };

            Controls.Add(tableLayoutPanel1);
        }

        private void MainScreen_Shown(object sender, EventArgs e)
        {
            try
            {
                iOS_Executor.selectediOSExecutor = Database.QueryDataFromiOSExecutorTable();
                iOS_Proxy.selectediOSProxyMethod = Database.QueryDataFromiOSProxyTable();
                Logger.Info("iOS_Executor.selectediOSExecutor - " + iOS_Executor.selectediOSExecutor);
                Logger.Info("iOS_Proxy.selectediOSProxyMethod - " + iOS_Proxy.selectediOSProxyMethod);
            }
            catch (Exception ex)
            {
                iOS_Executor.selectediOSExecutor = "auto";
                iOS_Proxy.selectediOSProxyMethod = "iproxy";
                Logger.Error(ex);
            }
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
            try
            {
                if (server1WebView.CoreWebView2 != null)
                {
                    server1WebView.Reload();
                }
            }
            catch (Exception)
            {
            }
            //tabControl1.SelectedIndex = 0;
            //tabControl1_SelectedIndexChanged(tabControl1, EventArgs.Empty);
            GoogleAnalytics.SendEvent("MainScreen_Shown");
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
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
                    selectedDeviceIP = selectedItem.SubItems.Count > 6 ? selectedItem.SubItems[6].Text : string.Empty;
                    selectedDeviceModel = selectedItem.SubItems.Count > 7 ? selectedItem.SubItems[7].Text : string.Empty;
                    Open.Enabled = true;
                    OpenDropDownButton.Enabled = true;
                    MoreButton.Enabled = true;
                    DeleteDevice.Enabled = true;
                    if (selectedOS.Equals("iOS"))
                    {
                        if (iOS_Executor.selectediOSExecutor.Equals("auto"))
                        {
                            if (IsIt17PlusVersion(selectedDeviceVersion)) // >17
                            {
                                //SetiOSTool(false);
                                //iOSAsyncMethods.is17Plus = true;

                                SetiOSTool(true);
                                iOSAsyncMethods.is17Plus = true;
                            }
                            else // <17
                            {
                                SetiOSTool(true);
                                iOSAsyncMethods.is17Plus = false;
                            }
                        }
                        else if (iOS_Executor.selectediOSExecutor.Equals("go"))
                        {
                            SetiOSTool(true); // use go
                        }
                        else
                        {
                            SetiOSTool(false); // use py
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
                        OpenDropDownButton.Enabled = false;
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
                        OpenDropDownButton.Enabled = true;
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
                    OpenDropDownButton.Enabled = false;
                    DeleteDevice.Enabled = false;
                    MoreButton.Enabled = false;
                    mandatorymsglabel.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public void SetiOSTool(bool useGoiOS)
        {
            if (useGoiOS) //go-iOS
            {
                iOSMethods.isGo = true;
                iOSAsyncMethods.isGo = true;
            }
            else //pymobiledevice3
            {
                iOSMethods.isGo = false;
                iOSAsyncMethods.isGo = false;
            }
        }

        public bool IsIt17PlusVersion(string version)
        {
            Version deviceVersion = new Version(version);
            Version version17Plus = new Version("17.0.0");
            if (deviceVersion >= version17Plus)
            {
                return true;
            }
            return false;
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
                        if (screenForm.WindowState.Equals(FormWindowState.Minimized))
                        {
                            screenForm.WindowState = FormWindowState.Normal;
                        }
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
                        if (screenForm.WindowState.Equals(FormWindowState.Minimized))
                        {
                            screenForm.WindowState = FormWindowState.Normal;
                        }
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
            if (OS.Equals("iOS", StringComparison.InvariantCultureIgnoreCase) | OS.Equals("iPhone OS", StringComparison.InvariantCultureIgnoreCase))
            {
                OS = "iOS";
                if (IsIt17PlusVersion(OSVersion))
                {
                    Task.Run(() =>
                    {
                        iOSAsyncMethods.GetInstance().CreateTunnelGo(false);
                    });
                }
                Task.Run(() =>
                {
                    iOSMethods.GetInstance().MountImage(udid);
                });
            }
            if (DeviceName != null | DeviceName != string.Empty)
            {
                string[] item1 = { DeviceName ?? "", OSVersion, OS, status, udid, connection, IPAddress, Model };
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
                        string[] item1 = { device["Name"], device["Version"], device["OS"], status, device["UDID"], device["Connection"], device["IPAddress"], device["Model"] };
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
                    Database.DeleteUDIDFromUsePreInstalledWDAList(selectedUDID);
                    UDIDPreInstalledWDA.Remove(selectedUDID);
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
            commonProgress.UpdateStepLabel("Detect iOS Device", "Looking for iOS device...", 10);
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
                        MessageBox.Show("No iOS Device available. Please check device connectivity and make sure device is unlocked.", "Add iOS Device", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        commonProgress.UpdateStepLabel("Detect iOS Device", "Getting iOS device information...", 50);
                        for (int i = 0; i < count; i++)
                        {
                            if (!isDeviceAlreadyAdded(deviceList[i]))
                            {
                                DeviceInformation deviceInformation = new DeviceInformation(main);
                                await Task.Run(() =>
                                {
                                    deviceInfo = iOSMethods.GetInstance().GetDeviceInformation(deviceList[i]);
                                });
                                commonProgress.UpdateStepLabel("Detect iOS Device", "Getting iOS device information...", 70);
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
                                    commonProgress.UpdateStepLabel("Detect iOS Device", "Getting iOS device information...", 90);
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
            commonProgress.UpdateStepLabel("Detect Android Device", "Looking for Android device...", 10);
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
                    commonProgress.UpdateStepLabel("Detect Android Device", "Getting Android device information...", 50);
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
                                commonProgress.UpdateStepLabel("Detect Android Device", "Getting Android device information...", 75);
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

        private void MoreButton_Click(object sender, EventArgs e)
        {
            Point screenPoint = MoreButton.PointToScreen(new Point(0, MoreButton.Height));
            contextMenuStrip4.Show(screenPoint);
        }

        private ToolTip moreButtonToolTip = new ToolTip();
        bool isMessageDisplayed = false;
        private void MoreButton_MouseHover(object sender, EventArgs e)
        {
            if (selectedOS.Equals("iOS"))
            {
                moreButtonToolTip.SetToolTip(MoreButton, "Make sure to open the iOS device before performing any operation from More section or some operations may not work or may need admin privilege.");
            }
            else
            {
                moreButtonToolTip.SetToolTip(MoreButton, string.Empty);
            }
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
                    await troubleShooter.FindIssues();
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
                GoogleAnalytics.SendExceptionEvent("XCUITestToolStripMenuItem_Click", exception.Message);
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
            try
            {
                ProcessStartInfo psInfo = new ProcessStartInfo
                {
                    FileName = "https://github.com/mega6453/AppiumWizard/blob/master/TROUBLESHOOTINGGUIDE.md",
                    UseShellExecute = true
                };
                Process.Start(psInfo);
                GoogleAnalytics.SendEvent("fAQToolStripMenuItem_Click");
            }
            catch (Exception exception)
            {
                GoogleAnalytics.SendExceptionEvent("fAQToolStripMenuItem_Click", exception.Message);
            }
        }

        private async void iOSProfileManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Owner = this;
            commonProgress.Show();
            iOSProfileManagement iOSProfileManagement = new iOSProfileManagement();
            await iOSProfileManagement.UpdateProfilesList(commonProgress);
            iOSProfileManagement.ShowDialog();
        }

        private void listView1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listView1.FocusedItem != null && listView1.FocusedItem.Bounds.Contains(e.Location))
                {
                    if (contextMenuStrip2 != null)
                    {
                        contextMenuStrip2.Show(Cursor.Position);
                    }
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
            await troubleShooter.FindIssues();
            troubleShooter.ShowDialog(this);
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
                        bool isRebooted = iOSMethods.GetInstance().RebootDevice(selectedUDID);
                        if (isRebooted)
                        {
                            MessageBox.Show("Reboot Initiated for " + selectedDeviceName + ".", "Reboot Device", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        GoogleAnalytics.SendExceptionEvent("Reboot_iOS");
                    }
                    else
                    {
                        AndroidMethods.GetInstance().RebootDevice(selectedUDID);
                        MessageBox.Show("Reboot Initiated for " + selectedDeviceName + ".", "Reboot Device", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        GoogleAnalytics.SendExceptionEvent("Reboot_Android");
                    }
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

        private Dictionary<string, InstalledAppsList> installedAppsForms = new Dictionary<string, InstalledAppsList>();
        private async void launchAppToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string deviceKey = selectedUDID;
            if (!installedAppsForms.ContainsKey(deviceKey) || installedAppsForms[deviceKey].IsDisposed)
            {
                InstalledAppsList installedAppsList;
                if (selectedOS.Equals("Android") && selectedDeviceConnection.Equals("Wi-Fi"))
                {
                    installedAppsList = new InstalledAppsList(selectedOS, selectedDeviceIP, selectedDeviceName);
                }
                else
                {
                    installedAppsList = new InstalledAppsList(selectedOS, selectedUDID, selectedDeviceName);
                }
                await installedAppsList.GetInstalledAppsList(this);
                installedAppsForms[deviceKey] = installedAppsList;
                installedAppsList.FormClosed += (s, args) => installedAppsForms.Remove(deviceKey);
                installedAppsList.Show();
            }
            else
            {
                if (installedAppsForms[deviceKey].WindowState == FormWindowState.Minimized)
                {
                    installedAppsForms[deviceKey].WindowState = FormWindowState.Normal;
                }
                installedAppsForms[deviceKey].BringToFront();
            }
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

        private SignIPA? signIPA;
        private async void signIPAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (signIPA == null || signIPA.IsDisposed)
            {
                CommonProgress commonProgress = new CommonProgress();
                commonProgress.Owner = this;
                commonProgress.Show();
                commonProgress.UpdateStepLabel("Load Profiles", "Please wait while fetching profiles...");
                List<string[]> profilesList = new List<string[]>();
                await Task.Run(() =>
                {
                    profilesList = iOSProfileManagement.FetchProfiles();
                });
                commonProgress.Close();
                if (profilesList.Count == 0)
                {
                    MessageBox.Show("Provisioning Profiles not found. First Import profile in Tools->iOS Profile Management and then try again.", "Provisioning Profiles not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    GoogleAnalytics.SendEvent("Profiles_Not_Available_Popup");
                }
                else
                {
                    signIPA = new SignIPA(profilesList);
                    signIPA.Owner = this;
                    signIPA.Show();
                }
            }
            else
            {
                if (signIPA.WindowState == FormWindowState.Minimized)
                {
                    signIPA.WindowState = FormWindowState.Normal;
                }
                signIPA.BringToFront();
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
                    MessageBox.Show("Failed to check update - Go to https://github.com/mega6453/AppiumWizard and check manually.", "Check for Updates...");
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
            await updater.GetVersionInformation();
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
                commonProgress.UpdateStepLabel("Closing Appium Wizard", "Please wait while closing all resources and exiting...");
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

        private void iOSExecutorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            iOS_Executor iOS_Executor = new iOS_Executor(listView1);
            iOS_Executor.ShowDialog();
        }

        private void MainScreen_Activated(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = listView1.SelectedItems[0];
                selectedItem.Selected = false;
                selectedItem.Selected = true;
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

        private void iOSProxyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            iOS_Proxy proxy = new iOS_Proxy();
            proxy.ShowDialog();
        }

        private void notificationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Notifications notifications = new Notifications();
            notifications.Owner = this;
            notifications.ShowDialog();
        }

        private void yesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            alwaysOnTop = true;
            Database.UpdateDataIntoAlwaysOnTopTable("Yes");
            noToolStripMenuItem.Image = null;
            yesToolStripMenuItem.Image = Resources.check_mark;
        }

        private void noToolStripMenuItem_Click(object sender, EventArgs e)
        {
            alwaysOnTop = false;
            Database.UpdateDataIntoAlwaysOnTopTable("No");
            yesToolStripMenuItem.Image = null;
            noToolStripMenuItem.Image = Resources.check_mark;
        }

        private void usePreInstalledWDAToolStripMenuItem_Click(object sender, EventArgs e)
        {

            UsePreInstalledWDA usePreInstalledWDA = new UsePreInstalledWDA(selectedUDID, selectedDeviceName);
            usePreInstalledWDA.ShowDialog();
        }

        private void contextMenuStrip2_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (selectedOS.Equals("iOS"))
            {
                usePreInstalledWDAToolStripMenuItem.Visible = true;
                if (UDIDPreInstalledWDA.ContainsKey(selectedUDID))
                {
                    usePreInstalledWDAToolStripMenuItem.Image = Resources.check_mark;
                }
                else
                {
                    usePreInstalledWDAToolStripMenuItem.Image = null;
                }
            }
            else
            {
                usePreInstalledWDAToolStripMenuItem.Visible = false;
            }
        }

        private void pluginsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessStartInfo psInfo = new ProcessStartInfo
                {
                    FileName = "https://appium.io/docs/en/latest/ecosystem/plugins/",
                    UseShellExecute = true
                };
                Process.Start(psInfo);
                GoogleAnalytics.SendEvent("pluginsToolStripMenuItem_Click");
            }
            catch (Exception exception)
            {
                GoogleAnalytics.SendExceptionEvent("pluginsToolStripMenuItem_Click", exception.Message);
            }
        }

        private async void pluginsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Owner = this;
            commonProgress.Show();
            Plugins plugins = new Plugins();
            await plugins.UpdatePluginList(commonProgress);
            plugins.ShowDialog();
        }

        private void openLogsFolderToolstripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\Resources\\Logs");
        }

        private void copyDeviceDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string deviceName = "Device Name - " + selectedDeviceName;
            string deviceOS = "Device OS - " + selectedOS;
            string deviceOSVersion = "OS Version - " + selectedDeviceVersion;
            string deviceModel = "Model - " + selectedDeviceModel;
            string udid = "UDID - " + selectedUDID;
            string deviceDetails = deviceName + "\n" + deviceOS + "\n" + deviceOSVersion + "\n" + deviceModel + "\n" + udid;
            Clipboard.SetText(deviceDetails);
            GoogleAnalytics.SendEvent("copyDeviceDetailsToolStripMenuItem_Click");
        }


        private void testRunnerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TestRunner testRunner = new TestRunner();
            testRunner.Show();
        }

        private void openLogsButton_Click(object sender, EventArgs e)
        {
            try
            {
                TabPage selectedTab = tabControl1.SelectedTab;
                int selectedIndex = tabControl1.SelectedIndex;
                int serverNumber = selectedIndex + 1;
                if (AppiumServerSetup.portServerNumberAndFilePath.ContainsKey(serverNumber))
                {
                    int logsPort = StartServerAndReturnPort(serverNumber);
                    ProcessStartInfo psInfo = new ProcessStartInfo
                    {
                        FileName = "http://localhost:" + logsPort + "/index.html",
                        UseShellExecute = true
                    };
                    Process.Start(psInfo);
                }
                else
                {
                    MessageBox.Show("Appium server is not running in tab " + serverNumber + ". Please start the server and then try again. Go to Server->Config to start server.", "Server Not Running", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occured in Starting Server logs, Please report in github with steps. Exception : " + ex.Message, "Exception in Starting Server logs", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public int getIntervalToLoadLogs(int serverNumber)
        {
            string logLevelFromDB = Database.QueryDataFromlogLevelTable()["Server" + serverNumber];
            switch (logLevelFromDB)
            {
                case "error":
                    return 1000;
                case "info":
                    return 2000;
                case "debug":
                    return 5000;
                default:
                    return 5000;
            }
        }


        public static ConcurrentDictionary<int, bool> serverUrlLoaded = new ConcurrentDictionary<int, bool>();

        public void StartLogsServer(int serverNumber)
        {
            try
            {
                if (AppiumServerSetup.portServerNumberAndFilePath.ContainsKey(serverNumber))
                {
                    int logsPort = StartServerAndReturnPort(serverNumber);
                    string url = "http://localhost:" + logsPort + "/index.html";
                    WaitForServerToStart(url);
                    if (!serverUrlLoaded.GetOrAdd(serverNumber, false))
                    {
                        if (serverNumberWebView.ContainsKey(serverNumber))
                        {
                            LoadUrlInWebView(serverNumberWebView[serverNumber], url);
                            serverUrlLoaded[serverNumber] = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occured in Starting Server logs, Please report in github with steps. Exception : " + ex.Message, "Exception in Starting Server logs", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int StartServerAndReturnPort(int serverNumber)
        {
            int logsPort;
            if (!Common.serverNumberPortNumber.ContainsKey(serverNumber))
            {
                int appiumPortNumber = AppiumServerSetup.portServerNumberAndFilePath[serverNumber].Item1;
                string logFilePath = AppiumServerSetup.portServerNumberAndFilePath[serverNumber].Item2;
                string htmlFilePath = Common.GenerateHtmlWithFilePath(logFilePath, appiumPortNumber, getIntervalToLoadLogs(serverNumber));
                logsPort = Common.GetFreePort();
                _ = Task.Run(() => Common.StartServer(serverNumber, logsPort, htmlFilePath, logFilePath));
            }
            else
            {
                logsPort = Common.serverNumberPortNumber[serverNumber];
            }
            return logsPort;
        }

        private bool WaitForServerToStart(string url, int timeoutMilliseconds = 5000)
        {
            var stopwatch = Stopwatch.StartNew();
            while (stopwatch.ElapsedMilliseconds < timeoutMilliseconds)
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        var response = client.GetAsync(url).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            return true; // Server is ready
                        }
                    }
                }
                catch
                {
                    // Ignore exceptions (e.g., server not started yet)
                }
                Thread.Sleep(100); // Wait for 100ms before retrying
            }
            return false; // Timeout
        }

        public void LoadUrlInWebView(WebView2 webView, string url)
        {
            if (webView.InvokeRequired)
            {
                // Marshal the call back to the UI thread
                webView.Invoke(new Action(() => LoadUrlInWebView(webView, url)));
            }
            else
            {
                try
                {
                    webView.Source = new Uri(url);
                }
                catch (UriFormatException ex)
                {
                    MessageBox.Show($"Invalid URL format: {url}\nError: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int appiumPortNumber, serverNumber = tabControl1.SelectedIndex + 1;
            string text = "Open logs in browser";
            if (AppiumServerSetup.portServerNumberAndFilePath.ContainsKey(serverNumber))
            {
                appiumPortNumber = AppiumServerSetup.portServerNumberAndFilePath[serverNumber].Item1;
                text = "Open logs in browser - " + appiumPortNumber;
                openLogsButton.Visible = true;
            }
            else
            {
                openLogsButton.Visible = false;
            }
            openLogsButton.Location = new Point(tabControl1.Right - openLogsButton.Width, tabControl1.Top);
            openLogsButton.Text = text;
        }


        public void SelectTab(int serverNumber, bool start)
        {
            int tabIndex = serverNumber - 1;
            tabControl1.SelectedIndex = tabIndex;
            UpdateOpenLogsButtonText(serverNumber, start);
        }

        public void UpdateOpenLogsButtonText(int serverNumber, bool start)
        {
            if (openLogsButton.InvokeRequired)
            {
                openLogsButton.Invoke(new Action(() => UpdateOpenLogsButtonText(serverNumber, start)));
            }
            else
            {
                int appiumPortNumber;
                if (start)
                {
                    if (AppiumServerSetup.portServerNumberAndFilePath.ContainsKey(serverNumber))
                    {
                        appiumPortNumber = AppiumServerSetup.portServerNumberAndFilePath[serverNumber].Item1;
                        string text = "Open logs in browser - " + appiumPortNumber;
                        openLogsButton.Text = text;
                        openLogsButton.Visible = true;
                    }
                }
                else
                {
                    openLogsButton.Text = "Open logs in browser";
                    openLogsButton.Visible = false;
                }
            }
        }

        public void UpdateTabText(int serverNumber, int portNumber, bool start)
        {
            int tabIndex = serverNumber - 1;
            if (tabControl1.InvokeRequired)
            {
                tabControl1.Invoke(new Action(() => UpdateTabText(serverNumber, portNumber, start)));
            }
            else
            {
                if (start)
                {
                    tabControl1.TabPages[tabIndex].Text = "#" + serverNumber + " - " + portNumber;
                }
                else
                {
                    tabControl1.TabPages[tabIndex].Text = "#" + serverNumber;
                }
            }
        }

        private void tabControl1_Resize(object sender, EventArgs e)
        {
            openLogsButton.Location = new Point(tabControl1.Right - openLogsButton.Width, tabControl1.Top);
        }

        private void openLogsButton_Resize(object sender, EventArgs e)
        {
            openLogsButton.Location = new Point(tabControl1.Right - openLogsButton.Width, tabControl1.Top);
        }

        public void UpdateWebViewDefaultText(int serverNumber)
        {
            SetDefaultText(serverNumberWebView[serverNumber], defaultText);
        }

        private async void reInitializeDeviceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string description = "Use this option in case if opening device fails. ";
            if (selectedOS.Equals("iOS"))
            {
                description += "This action will reinstall the WebDriverAgent on the " + selectedDeviceName + " and restart the device.\n\nOnce the device rebooted, unlock the device and then try to open the device again.\n\nDo you want to continue?";

            }
            else
            {
                description += "This action will reinstall the UIAutomator on the " + selectedDeviceName + " and restart the device.\n\nOnce the device rebooted, unlock the device and then try to open the device again.\n\nDo you want to continue?";
            }
            var result = MessageBox.Show(description, "Re-Initialize Device", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                CommonProgress commonProgress = new CommonProgress();
                commonProgress.Owner = this;
                commonProgress.Show();
                if (selectedOS.Equals("iOS"))
                {
                    commonProgress.UpdateStepLabel("Re-Initialize Device", "Please wait while checking if provisioning profile available for this device...", 10);
                    if (iOSMethods.GetInstance().isProfileAvailableToSign(selectedUDID).Item1)
                    {
                        commonProgress.UpdateStepLabel("Re-Initialize Device", "Please wait while uninstalling WDA...", 20);
                        await Task.Run(() =>
                        {
                            iOSMethods.GetInstance().UninstallWDA(selectedUDID);
                        });
                        commonProgress.UpdateStepLabel("Re-Initialize Device", "Please wait while installing WDA...", 30);
                        bool isWDAInstalled = false;
                        await Task.Run(() =>
                        {
                            isWDAInstalled = iOSMethods.GetInstance().InstallWDA(selectedUDID);
                        });
                        if (isWDAInstalled)
                        {
                            commonProgress.UpdateStepLabel("Re-Initialize Device", "Please wait while Rebooting device...", 70);
                            await Task.Run(() =>
                            {
                                iOSMethods.GetInstance().RebootDevice(selectedUDID);
                            });
                            MessageBox.Show("Reboot Initiated. Once the device rebooted, unlock the device and then try to open the device again.\n\nIf possible, restart Appium Wizard also and then try opening device.", "Re-Initialize Device", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Failed to install WebDriverAgent. Please check if you have valid profile in Tools->iOS profile management.\n\n Canceling Reboot step.", "Install WDA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Provisioning profile not found for this device. Please check if you have valid profile in Tools->iOS profile management.\n\n Canceling Uninstalling WDA step.\n\nIf you have WDA already installed in this device, restart the device and then try opening the device.", "Re-Initialize Device", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    commonProgress.UpdateStepLabel("Re-Initialize Device", "Please wait while uninstalling uiautomator...", 10);
                    await Task.Run(() =>
                    {
                        AndroidMethods.GetInstance().UninstallUIAutomator(selectedUDID);
                    });
                    commonProgress.UpdateStepLabel("Re-Initialize Device", "Please wait while installing uiautomator...", 30);
                    await Task.Run(() =>
                    {
                        AndroidMethods.GetInstance().InstallUIAutomator(selectedUDID);
                    });
                    commonProgress.UpdateStepLabel("Re-Initialize Device", "Please wait while Rebooting device...", 70);
                    await Task.Run(() =>
                    {
                        AndroidMethods.GetInstance().RebootDevice(selectedUDID);
                    });
                    MessageBox.Show("Reboot Initiated. Once the device rebooted, unlock the device and then try to open the device again.\n\nIf possible, restart Appium Wizard also and then try opening device.", "Re-Initialize Device", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                commonProgress.Close();
            }
        }

        private void OpenDropDownButton_Click(object sender, EventArgs e)
        {
            Point screenPoint = OpenDropDownButton.PointToScreen(new Point(0, OpenDropDownButton.Height));
            openContextMenuStrip.Show(screenPoint);
        }

        private void changeLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessStartInfo psInfo = new ProcessStartInfo
                {
                    FileName = "https://github.com/mega6453/AppiumWizard/blob/master/CHANGELOG.md",
                    UseShellExecute = true
                };
                Process.Start(psInfo);
                GoogleAnalytics.SendEvent("changeLogToolStripMenuItem_Click");
            }
            catch (Exception exception)
            {
                GoogleAnalytics.SendExceptionEvent("changeLogToolStripMenuItem_Click", exception.Message);
            }
        }

        private void readMeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessStartInfo psInfo = new ProcessStartInfo
                {
                    FileName = "https://github.com/mega6453/AppiumWizard/blob/master/README.md",
                    UseShellExecute = true
                };
                Process.Start(psInfo);
                GoogleAnalytics.SendEvent("readMeToolStripMenuItem_Click");
            }
            catch (Exception exception)
            {
                GoogleAnalytics.SendExceptionEvent("readMeToolStripMenuItem_Click", exception.Message);
            }
        }
    }
}
