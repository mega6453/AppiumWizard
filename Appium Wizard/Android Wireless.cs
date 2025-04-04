namespace Appium_Wizard
{
    public partial class AndroidWireless : Form
    {
        string selectedDevice = string.Empty;
        string selectedAddress = string.Empty;
        ListViewItem selectedItem = new ListViewItem();
        Dictionary<string, Tuple<string, string>> listOfDevices = new Dictionary<string, Tuple<string, string>>();
        public AndroidWireless()
        {
            InitializeComponent();
            int listViewWidth = listView1.Width;
            int columnWidth = listViewWidth / 2;
            listView1.Columns[0].Width = columnWidth;
            listView1.Columns[1].Width = columnWidth;
        }

        private void FindDeviceButton_Click(object sender, EventArgs e)
        {
            FindDevices();
        }

        private async void FindDevices()
        {
            int retryCount = 0;
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Android Wireless device", "Looking for Android devices over Wi-Fi...",10);
            listOfDevices.Clear();
            await Task.Run(() => {
                listOfDevices = AndroidMethods.GetInstance().FindPairingReadyDevicesOverWiFi();
            });
            commonProgress.UpdateStepLabel("Android Wireless device", "Looking for Android devices over Wi-Fi...",30);
            while (listOfDevices.Count == 0 && retryCount <= 5)
            {
                await Task.Run(() => {
                    listOfDevices = AndroidMethods.GetInstance().FindPairingReadyDevicesOverWiFi();
                });
                await Task.Delay(1000);                
                retryCount++;
                commonProgress.UpdateStepLabel("Android Wireless device", "Looking for Android devices over Wi-Fi...", 30+(retryCount*10));
            }
            listView1.Items.Clear();
            foreach (var pair in listOfDevices)
            {
                string deviceName = pair.Key;
                string pairingAddress = pair.Value.Item1;
                string connectAddress = pair.Value.Item2;
                if (!string.IsNullOrEmpty(pairingAddress))
                {
                    ListViewItem item = new ListViewItem(deviceName);
                    item.SubItems.Add(pairingAddress);
                    listView1.Items.Add(item);
                }
                else if (!string.IsNullOrEmpty(connectAddress))
                {
                    ListViewItem item = new ListViewItem(deviceName);
                    item.SubItems.Add(connectAddress);
                    listView1.Items.Add(item);
                }
            }
            commonProgress.Close();
            if (listOfDevices.Count == 0)
            {
                var result = MessageBox.Show("No device found. Do you want to try adding the device manually?", "Add Android Device Over Wi-Fi", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                {
                    AndroidWirelessManual androidWirelessManual = new AndroidWirelessManual(this);
                    androidWirelessManual.ShowDialog();
                }
            }
        }

        private async void PairButton_Click(object sender, EventArgs e)
        {
            string pairingAddress = listOfDevices[selectedDevice].Item1;
            string connectAddress = listOfDevices[selectedDevice].Item2;
            if (string.IsNullOrEmpty(connectAddress))
            {
                listOfDevices.Clear();
                await Task.Run(() => {
                    listOfDevices = AndroidMethods.GetInstance().FindPairingReadyDevicesOverWiFi();
                });
                connectAddress = listOfDevices[selectedDevice].Item2;
            }
            if (string.IsNullOrEmpty(pairingAddress) & !string.IsNullOrEmpty(connectAddress))
            {
                CommonProgress commonProgress = new CommonProgress();
                commonProgress.Show();
                commonProgress.UpdateStepLabel("Android Wireless device", "Connecting to Android device over Wi-Fi...");
                await GetDeviceInformation(commonProgress, this, connectAddress);
            }
            if (!string.IsNullOrEmpty(pairingAddress) & !string.IsNullOrEmpty(connectAddress))
            {
                PairingCodePrompt pairingCodePrompt = new PairingCodePrompt(this, selectedDevice, selectedAddress, connectAddress);
                pairingCodePrompt.ShowDialog();
            }
            GoogleAnalytics.SendEvent("PairButton_Click");
        }

        public void RemoveFromList()
        {
            if (listView1.InvokeRequired)
            {
                listView1.Invoke(new Action(() => listView1.Items.Remove(selectedItem)));
                listView1.Invoke(new Action(() => listView1.Refresh()));
            }
            else
            {
                listView1.Items.Remove(selectedItem);
                listView1.Refresh();
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                selectedItem = listView1.SelectedItems[0];
                selectedDevice = selectedItem.SubItems[0].Text;
                selectedAddress = selectedItem.SubItems[1].Text;
                PairButton.Enabled = true;
            }
            else
            {
                PairButton.Enabled = false;
            }
        }

        public async Task GetDeviceInformation(CommonProgress commonProgress, AndroidWireless androidWireless, string connectIPAddress)
        {
            string connectOutput = string.Empty;
            await Task.Run(() => {
                connectOutput = AndroidMethods.GetInstance().ConnectToAndroidWirelessly(connectIPAddress);
            });
            commonProgress.UpdateStepLabel("Android Wireless device", "Connecting to Android device over Wi-Fi...", 50);
            if (connectOutput.Contains("failed to connect"))
            {
                commonProgress.Close();
                MessageBox.Show("1.In the phone, Go to Developer options > Wireless debugging > Pair device with pairing code.\n2.Here, Click Find Devices again> Select the pairing IP > Pair.", "Add Android Device Over Wi-Fi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                Dictionary<string, string> deviceInfo = new Dictionary<string, string>();
                await Task.Run(() => {
                    deviceInfo = AndroidAsyncMethods.GetInstance().GetDeviceInformation(connectIPAddress);
                });
                commonProgress.UpdateStepLabel("Android Wireless device", "Connecting to Android device over Wi-Fi...", 75);
                DeviceInformation deviceInformation = new DeviceInformation();
                if (deviceInfo.ContainsKey("ro.serialno"))
                {
                    string udid = deviceInfo["ro.serialno"]?.ToString() ?? "";
                    string DeviceName = deviceInfo["deviceName"]?.ToString() ?? "";
                    if (!MainScreen.isDeviceAlreadyAdded(udid))
                    {
                        if (deviceInfo.Count > 0)
                        {
                            //DeviceName = deviceInfo["ro.product.model"]?.ToString() ?? "";
                            string OSVersion = "";
                            string OSType = "Android";
                            string Model = "";
                            string Width = deviceInfo["Width"];
                            string Height = deviceInfo["Height"];
                            string Brand = string.Empty;
                            if (deviceInfo.ContainsKey("deviceName"))
                            {
                                DeviceName = deviceInfo["deviceName"]?.ToString() ?? "";
                            }
                            else
                            {
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
                            string[] ScreenWidth = { "Width", Width };
                            string[] ScreenHeight = { "Height", Height };
                            string[] Connection = { "Connection Type", "Wi-Fi" };
                            string[] IPAddress = { "IP Address", connectIPAddress };
                            deviceInformation.infoListView.Items.Add(new ListViewItem(name));
                            deviceInformation.infoListView.Items.Add(new ListViewItem(DeviceModel));
                            deviceInformation.infoListView.Items.Add(new ListViewItem(os));
                            deviceInformation.infoListView.Items.Add(new ListViewItem(version));
                            deviceInformation.infoListView.Items.Add(new ListViewItem(UniqueDeviceID));
                            deviceInformation.infoListView.Items.Add(new ListViewItem(ScreenWidth));
                            deviceInformation.infoListView.Items.Add(new ListViewItem(ScreenHeight));
                            deviceInformation.infoListView.Items.Add(new ListViewItem(Connection));
                            deviceInformation.infoListView.Items.Add(new ListViewItem(IPAddress));
                            commonProgress.Close();
                            deviceInformation.ShowDialog();
                            GoogleAnalytics.SendEvent("DeviceInformation_Android_Wireless");
                            if (MainScreen.isDeviceAlreadyAdded(udid))
                            {
                                RemoveFromList();
                                MessageBox.Show(DeviceName.Replace("\n", "") + " added over Wi-Fi.", "Android Device Over Wi-Fi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                FindDevices();
                            }
                        }
                    }
                    else
                    {
                        commonProgress.Close();
                        MessageBox.Show(DeviceName.Replace("\n", "") + " already exist in the Devices list.", "Android Device Over Wi-Fi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    commonProgress.Close();
                    MessageBox.Show("No Android Device available.\nPlease check if the device is on the same network as this PC.\nGo to Developer options > Wireless debugging > Pair device with pairing code > Find Devices again.", "Add Android Device Over Wi-Fi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void AndroidWireless_Shown(object sender, EventArgs e)
        {
            GoogleAnalytics.SendEvent("AndroidWireless_Shown");
        }

        private void ManualButton_Click(object sender, EventArgs e)
        {
            AndroidWirelessManual androidWirelessManual = new AndroidWirelessManual(this);
            androidWirelessManual.ShowDialog();
        }
    }
}
