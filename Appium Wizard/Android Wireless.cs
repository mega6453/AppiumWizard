namespace Appium_Wizard
{
    public partial class AndroidWireless : Form
    {
        MainScreen mainScreen;
        string selectedDevice = string.Empty;
        string selectedAddress = string.Empty;
        ListViewItem selectedItem;
        Dictionary<string, Tuple<string, string>> listOfDevices = new Dictionary<string, Tuple<string, string>>();
        public AndroidWireless(MainScreen mainScreen)
        {
            this.mainScreen = mainScreen;
            InitializeComponent();
        }

        private void FindDeviceButton_Click(object sender, EventArgs e)
        {
            FindDevices();
        }

        private void FindDevices()
        {
            int retryCount = 0;
            DeviceLookUp deviceLookUp = new DeviceLookUp("Looking for Android devices over Wi-Fi...");
            deviceLookUp.Show();
            listOfDevices.Clear();
            listOfDevices = AndroidMethods.GetInstance().FindPairingReadyDevicesOverWiFi();
            while (listOfDevices.Count == 0 && retryCount <= 5)
            {
                listOfDevices = AndroidMethods.GetInstance().FindPairingReadyDevicesOverWiFi();
                Thread.Sleep(1000);
                retryCount++;
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
            deviceLookUp.Close();
        }

        private void PairButton_Click(object sender, EventArgs e)
        {
            string pairingAddress = listOfDevices[selectedDevice].Item1;
            string connectAddress = listOfDevices[selectedDevice].Item2;
            if (string.IsNullOrEmpty(connectAddress))
            {
                listOfDevices.Clear();
                listOfDevices = AndroidMethods.GetInstance().FindPairingReadyDevicesOverWiFi();
                connectAddress = listOfDevices[selectedDevice].Item2;
            }
            if (string.IsNullOrEmpty(pairingAddress) & !string.IsNullOrEmpty(connectAddress))
            {
                DeviceLookUp deviceLookUp = new DeviceLookUp("Connecting to Android device over Wi-Fi...");
                deviceLookUp.Show();
                GetDeviceInformation(deviceLookUp, this, connectAddress);
            }
            if (!string.IsNullOrEmpty(pairingAddress) & !string.IsNullOrEmpty(connectAddress))
            {
                PairingCodePrompt pairingCodePrompt = new PairingCodePrompt(this, mainScreen, selectedDevice, selectedAddress, connectAddress);
                pairingCodePrompt.ShowDialog();
            }
            GoogleAnalytics.SendEvent("PairButton_Click");
        }

        public void RemoveFromList()
        {
            listView1.Items.Remove(selectedItem);
            listView1.Refresh();
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

        public void GetDeviceInformation(DeviceLookUp deviceLookUp, AndroidWireless androidWireless, string connectIPAddress)
        {
            var output = AndroidMethods.GetInstance().ConnectToAndroidWirelessly(connectIPAddress);
            if (output.Contains("failed to connect"))
            {
                deviceLookUp.Close();
                MessageBox.Show("1.In the phone, Go to Developer options > Wireless debugging > Pair device with pairing code.\n2.Here, Click Find Devices again> Select the pairing IP > Pair.", "Add Android Device Over Wi-Fi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                Dictionary<string, string> deviceInfo = AndroidAsyncMethods.GetInstance().GetDeviceInformation(connectIPAddress);
                DeviceInformation deviceInformation = new DeviceInformation(mainScreen);
                if (deviceInfo.ContainsKey("ro.serialno"))
                {
                    string udid = deviceInfo["ro.serialno"]?.ToString() ?? "";
                    string DeviceName = deviceInfo["deviceName"]?.ToString() ?? "";
                    if (!MainScreen.isDeviceAlreadyAdded(udid))
                    {
                        if (deviceInfo.Count > 0)
                        {
                            //DeviceName = deviceInfo["ro.product.model"]?.ToString() ?? "";
                            string OSVersion = deviceInfo["ro.build.version.release"]?.ToString() ?? "";
                            string OSType = "Android";
                            string Model = deviceInfo["ro.product.model"]?.ToString() ?? "";
                            string Width = deviceInfo["Width"];
                            string Height = deviceInfo["Height"];
                            string Brand = string.Empty;
                            if (!deviceInfo.ContainsKey("ro.product.product.brand"))
                            {
                                Brand = deviceInfo["ro.product.product.brand"]?.ToString() ?? "";
                            }
                            else
                            {
                                Brand = deviceInfo["ro.product.vendor.brand"]?.ToString() ?? "";
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
                            deviceLookUp.Hide();
                            deviceInformation.ShowDialog();
                            GoogleAnalytics.SendEvent("DeviceInformation_Android_Wireless");
                            deviceLookUp.Close();
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
                        deviceLookUp.Close();
                        MessageBox.Show(DeviceName.Replace("\n", "") + " already exist in the Devices list.", "Android Device Over Wi-Fi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    deviceLookUp.Close();
                    MessageBox.Show("No Android Device available.\nPlease check if the device is on the same network as this PC.\nGo to Developer options > Wireless debugging > Pair device with pairing code > Find Devices again.", "Add Android Device Over Wi-Fi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void AndroidWireless_Shown(object sender, EventArgs e)
        {
            GoogleAnalytics.SendEvent("AndroidWireless_Shown");
        }
    }
}
