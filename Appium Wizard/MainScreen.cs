using System.Diagnostics;
using File = System.IO.File;

namespace Appium_Wizard
{
    public partial class MainScreen : Form
    {
        private Process iOSProxyProcess, appiumServerProcess;
        string udid, DeviceName, OSVersion, OSType, selectedUDID, Model, Width, Height;
        public static MainScreen main;
        string selectedDeviceName, selectedOS, selectedDeviceStatus;
        public MainScreen()
        {
            InitializeComponent();
            main = this;
            RefreshDeviceListView();
            this.Text = "Appium Wizard " + ProductVersion;
            USBWatcher usb = new USBWatcher(listView1);
            usb.Start();
        }
        private void onFormLoad(object sender, EventArgs e)
        {
            var screenSize = Screen.PrimaryScreen.WorkingArea.Size;
            var expectedSize = new Size(screenSize.Width * 66 / 100, screenSize.Height * 88 / 100);
            tabControl1.Size = expectedSize;
            richTextBox1.Size = expectedSize;
            richTextBox2.Size = expectedSize;
            richTextBox3.Size = expectedSize;
            richTextBox4.Size = expectedSize;
            richTextBox5.Size = expectedSize;
        }

        DateTime lastWriteTime1 = new DateTime();
        DateTime lastWriteTime2 = new DateTime();
        DateTime lastWriteTime3 = new DateTime();
        DateTime lastWriteTime4 = new DateTime();
        DateTime lastWriteTime5 = new DateTime();
        private void afterFormShown(object sender, EventArgs e)
        {
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
            listView1.Invoke((MethodInvoker)delegate
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
                selectedOS = selectedItem.SubItems[2].Text;
                selectedUDID = getDeviceUdidByName(selectedDeviceName);
                selectedDeviceStatus = selectedItem.SubItems[3].Text;
                Open.Enabled = true;
                InstallButton.Enabled = true;
                DeleteDevice.Enabled = true;
            }
            else
            {
                Open.Enabled = false;
                DeleteDevice.Enabled = false;
                InstallButton.Enabled = false;
            }
        }

        private void Open_Click(object sender, EventArgs e)
        {
            if (selectedDeviceStatus.Equals("Online"))
            {
                foreach (ScreenControl screenForm in Application.OpenForms.OfType<ScreenControl>())
                {                                           // brings foreground if opened already
                    if (screenForm.Name.Equals(selectedUDID, StringComparison.InvariantCultureIgnoreCase))
                    {
                        screenForm.Activate();
                        return;
                    }
                }
                OpenDevice openDevice = new OpenDevice(selectedUDID, selectedOS, selectedDeviceName);
                openDevice.StartBackgroundTasks();
                foreach (ScreenControl screenForm in Application.OpenForms.OfType<ScreenControl>())
                {                                           //Open screen in progress class and brings foreground if opened already
                    if (screenForm.Name.Equals(selectedUDID, StringComparison.InvariantCultureIgnoreCase))
                    {
                        screenForm.Activate();
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show(selectedDeviceName + " Device Offline. Please check device connectivity...", "Open Device", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void onFormClosed(object sender, FormClosedEventArgs e)
        {
            if (iOSProxyProcess != null && !iOSProxyProcess.HasExited)
            {
                iOSProxyProcess.Kill();
            }
            if (appiumServerProcess != null && !appiumServerProcess.HasExited)
            {
                appiumServerProcess.Kill();
            }
            var processes = Process.GetProcessesByName("msedgewebview2");
            foreach (var process in processes)
            {
                if (!process.HasExited)
                {
                    process.Kill();
                }
            }
        }



        public void addToList(string DeviceName, string OSVersion, string udid, string OS, string Model, string status)
        {
            if (OS.Equals("iPhone OS", StringComparison.InvariantCultureIgnoreCase))
            {
                OS = "iOS";
            }
            if (DeviceName != null | DeviceName != string.Empty)
            {
                string[] item1 = { DeviceName ?? "", OSVersion, OS, status, udid };
                listView1.Items.Add(new ListViewItem(item1));
            }
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
                            if (connectedList.Contains(device["UDID"]))
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
                        string[] item1 = { device["Name"], device["Version"], device["OS"], status, device["UDID"] };
                        listView1.Items.Add(new ListViewItem(item1));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Point screenPoint = AddDevice.PointToScreen(new Point(0, AddDevice.Height));
            contextMenuStrip1.Show(screenPoint);
        }

        public bool isDeviceAlreadyAdded(string udid)
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

            DialogResult result = MessageBox.Show("Are you sure you want to delete " + selectedDeviceName + " device?", "Delete Device", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
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
                    }
                    DeleteDevice.Enabled = false;
                    Open.Enabled = false;
                    try
                    {
                        //OpenDevice.deviceDetails.Remove(selectedUDID);
                    }
                    catch (Exception)
                    {
                    }
                    MessageBox.Show(selectedDeviceName + " removed successfully.", "Delete Device", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Device not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void iOSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DeviceLookUp deviceLookUp = new DeviceLookUp("Looking for iOS device...");
                deviceLookUp.Show();
                List<string> deviceList = iOSMethods.GetInstance().GetListOfDevicesUDID();
                if (deviceList.Contains("ITunes not installed"))
                {
                    deviceLookUp.Close();
                    var result = MessageBox.Show("Apple Mobile Device Service required for iOS automation. Please Install ITunes and try again.\n\nDo you want to download now?", "Add iOS Device", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
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
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine("Exception" + exception);
                        }
                    }
                }
                else
                {
                    int count = deviceList.Count;
                    if (count == 0)
                    {
                        deviceLookUp.Hide();
                        MessageBox.Show("No iOS Device available. Please check device connectivity.", "Add iOS Device", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        for (int i = 0; i < count; i++)
                        {
                            if (!isDeviceAlreadyAdded(deviceList[i]))
                            {
                                DeviceInformation deviceInformation = new DeviceInformation(main);
                                Dictionary<string, object> deviceInfo = iOSMethods.GetInstance().GetDeviceInformation(deviceList[i]);
                                if (deviceInfo.Count > 0)
                                {
                                    Model = iOSMethods.GetInstance().GetDeviceModel(deviceInfo["ProductType"]?.ToString() ?? "");
                                    DeviceName = deviceInfo["DeviceName"]?.ToString() ?? "";
                                    OSVersion = deviceInfo["ProductVersion"]?.ToString() ?? "";
                                    udid = deviceInfo["UniqueDeviceID"]?.ToString() ?? "";
                                    OSType = "iOS";
                                    string[] name = { "Name", DeviceName };
                                    string[] version = { "OS", OSType };
                                    string[] os = { "Version", OSVersion };
                                    string[] UniqueDeviceID = { "Udid", udid };
                                    string[] DeviceModel = { "Model", Model };
                                    deviceInformation.infoListView.Items.Add(new ListViewItem(name));
                                    deviceInformation.infoListView.Items.Add(new ListViewItem(os));
                                    deviceInformation.infoListView.Items.Add(new ListViewItem(version));
                                    deviceInformation.infoListView.Items.Add(new ListViewItem(DeviceModel));
                                    deviceInformation.infoListView.Items.Add(new ListViewItem(UniqueDeviceID));
                                    deviceLookUp.Hide();
                                    deviceInformation.ShowDialog();
                                    deviceLookUp.Close();
                                    break;
                                }
                            }
                            else
                            {
                                if (i == count - 1)
                                {
                                    deviceLookUp.Hide();
                                    MessageBox.Show("No New iOS Device available. Please check device connectivity.", "Add iOS Device", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    deviceLookUp.Close();
                                }
                            }
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void androidToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeviceLookUp deviceLookUp = new DeviceLookUp("Looking for Android device...");
            try
            {
                deviceLookUp.Show();
                List<string> deviceList = AndroidMethods.GetInstance().GetListOfDevicesUDID();
                int count = deviceList.Count;
                if (count == 0)
                {
                    deviceLookUp.Hide();
                    MessageBox.Show("No Android Device available. Please check device connectivity.\nMake sure USB debugging option enabled in your phone's Developer options.", "Add Android Device", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (!isDeviceAlreadyAdded(deviceList[i]))
                        {
                            DeviceInformation deviceInformation = new DeviceInformation(main);
                            Dictionary<string, string> deviceInfo = AndroidAsyncMethods.GetInstance().GetDeviceInformation(deviceList[i]);
                            if (deviceInfo.Count > 0)
                            {
                                DeviceName = deviceInfo["deviceName"]?.ToString() ?? "";
                                //DeviceName = deviceInfo["ro.product.model"]?.ToString() ?? "";
                                OSVersion = deviceInfo["ro.build.version.release"]?.ToString() ?? "";
                                udid = deviceInfo["ro.serialno"]?.ToString() ?? "";
                                OSType = "Android";
                                Model = deviceInfo["ro.product.model"]?.ToString() ?? "";
                                Width = deviceInfo["Width"];
                                Height = deviceInfo["Height"];
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
                                deviceInformation.infoListView.Items.Add(new ListViewItem(name));
                                deviceInformation.infoListView.Items.Add(new ListViewItem(DeviceModel));
                                deviceInformation.infoListView.Items.Add(new ListViewItem(os));
                                deviceInformation.infoListView.Items.Add(new ListViewItem(version));
                                deviceInformation.infoListView.Items.Add(new ListViewItem(UniqueDeviceID));
                                deviceInformation.infoListView.Items.Add(new ListViewItem(ScreenWidth));
                                deviceInformation.infoListView.Items.Add(new ListViewItem(ScreenHeight));
                                deviceLookUp.Hide();
                                deviceInformation.ShowDialog();
                                break;
                            }
                        }
                        else
                        {
                            if (i == count - 1)
                            {
                                deviceLookUp.Hide();
                                MessageBox.Show("No New Android Device available. Please check device connectivity.", "Add Android Device", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                deviceLookUp.Hide();
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void onFormClosing(object sender, FormClosingEventArgs e)
        {
            Common.TerminateProcess("taskkill /im appiumserver.exe /f");
            Common.TerminateProcess("taskkill /im node.exe /f");
            Common.TerminateProcess("taskkill /im iOSServer.exe /f");
            Common.TerminateProcess("taskkill /im adb.exe /f");
            List<Form> childFormsToClose = new List<Form>();
            foreach (Form form in Application.OpenForms)
            {
                if (form != this)
                {
                    childFormsToClose.Add(form);
                }
            }
            foreach (Form formToClose in childFormsToClose)
            {
                formToClose.Close();
            }

        }

        private void InstallButton_Click(object sender, EventArgs e)
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
                        InstallApp installApp = new InstallApp(selectedDeviceName, selectedUDID, filePath, fileName);
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
                        commonProgress.Show();
                        commonProgress.UpdateStepLabel("Install App", "Starting Installation...");
                        bool completed = false;
                        Dictionary<string, string> APKInfo = new Dictionary<string, string>();
                        string appName = "", packageName = "", activityName = "", version = "";
                        bool isAPKInfoRead;
                        try
                        {
                            APKInfo = AndroidMethods.GetInstance().GetApkInformation(filePath);
                            appName = APKInfo["AppName"];
                            packageName = APKInfo["PackageName"];
                            activityName = APKInfo["ActivityName"];
                            version = APKInfo["Version"];
                            isAPKInfoRead = true;
                        }
                        catch (Exception)
                        {
                            isAPKInfoRead = false;
                        }
                        bool isInstalled = false;
                        Task.Run(() =>
                        {
                            isInstalled = AndroidMethods.GetInstance().InstallApp(selectedUDID, filePath);
                            completed = true;
                        });
                        while (completed == false)
                        {
                            if (isAPKInfoRead)
                            {
                                commonProgress.UpdateStepLabel("Install App", "Installing " + appName + "(" + version + ") into " + selectedDeviceName);
                            }
                            else
                            {
                                commonProgress.UpdateStepLabel("Install App", "Installing " + fileName + " into " + selectedDeviceName);
                            }
                        }
                        if (isInstalled)
                        {
                            if (isAPKInfoRead)
                            {
                                DialogResult result = MessageBox.Show("Installation Successful. Would you like to launch the app?", "Launch App", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                                if (result == DialogResult.Yes)
                                {
                                    AndroidMethods.GetInstance().LaunchApp(selectedUDID, packageName, activityName);
                                }
                            }
                            else
                            {
                                DialogResult result = MessageBox.Show("Installation Successful. Launch option not available. Please launch the app manually.", "Install App", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        commonProgress.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("Please check device connectivity...", "Device Offline", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        public static void InstalliOSApp(string selectedDeviceName, string selectedUDID, string filePath, InstallApp installApp)
        {
            installApp.Close();
            CommonProgress commonProgress = new CommonProgress();
            string fileName = Path.GetFileName(filePath);
            commonProgress.Text = "Installing " + fileName + " in " + selectedDeviceName;
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Install App", "Starting Installation...");
            Dictionary<string, string> IPAInfo = new Dictionary<string, string>();
            string bundleId = "", appName = "", version = "";
            bool isPListRead = false;
            try
            {
                IPAInfo = iOSMethods.GetInstance().GetIPAInformation(filePath);
                bundleId = IPAInfo["CFBundleIdentifier"];
                appName = IPAInfo["CFBundleExecutable"];
                version = IPAInfo["CFBundleShortVersionString"] + "[" + IPAInfo["CFBundleVersion"] + "]";
                isPListRead = true;
            }
            catch (Exception)
            {
                isPListRead = false;
            }
            bool completed = false;
            Task.Run(() =>
            {
                iOSAsyncMethods.GetInstance().InstallApp(selectedUDID, filePath);
                completed = true;
            });
            while (completed == false)
            {
                if (isPListRead)
                {
                    commonProgress.UpdateStepLabel("Install App", "Installing " + appName + "(" + version + ")" + " into " + selectedDeviceName + "\n\nPercentage Completion : " + iOSAsyncMethods.installationProgress + "%");
                }
                else
                {
                    commonProgress.UpdateStepLabel("Install App", "Installing " + fileName + " into " + selectedDeviceName + "\n\nPercentage Completion : " + iOSAsyncMethods.installationProgress + "%");
                }
            }
            if (iOSAsyncMethods.installationProgress.Contains("installation successful") | iOSAsyncMethods.installationProgress == "100")
            {
                if (isPListRead)
                {
                    DialogResult result = MessageBox.Show("Installation Successful. Would you like to launch the app?", "Launch App", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (result == DialogResult.Yes)
                    {
                        var output = iOSMethods.GetInstance().LaunchApp(selectedUDID, bundleId);
                        if (output.Contains("profile has not been explicitly trusted by the user"))
                        {
                            MessageBox.Show("Unable to launch " + appName + " because it has an invalid code signature, inadequate entitlements or its profile has not been explicitly trusted by the user.\n\nTo trust a profile on the device, go to Settings-> General-> VPN & Device Management-> Select Profile-> Trust.", "Launch Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    DialogResult result = MessageBox.Show("Installation Successful. Launch option not available. Please launch the app manually.", "Install App", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                if (iOSAsyncMethods.installationProgress.Contains("MismatchedApplicationIdentifierEntitlement"))
                {
                    MessageBox.Show("Uninstall the existing " + appName + " app and Try again...", "Intallation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(iOSAsyncMethods.installationProgress, "Intallation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            iOSAsyncMethods.installationProgress = "0";
            commonProgress.Close();

        }




        private void serverSetupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Common.IsNodeInstalled())
            {
                ServerConfig serverSetup = new ServerConfig();
                serverSetup.ShowDialog();
            }
            else
            {
                var result = MessageBox.Show("NodeJS not installed OR not added into environment variables.\nPlease Install NodeJs and then try again.\n\nClick OK to open the Troubleshooter to fix the issues.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (result == DialogResult.OK)
                {
                    TroubleShooter troubleShooter = new TroubleShooter();   
                    troubleShooter.ShowDialog();
                }
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
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception" + exception);
            }
        }

        private void capabilitiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessStartInfo psInfo = new ProcessStartInfo
                {
                    FileName = "https://appium.io/docs/en/latest/guides/caps/",
                    UseShellExecute = true
                };
                Process.Start(psInfo);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception" + exception);
            }
        }

        private void xCUITestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessStartInfo psInfo = new ProcessStartInfo
                {
                    FileName = "https://appium.github.io/appium-xcuitest-driver",
                    UseShellExecute = true
                };
                Process.Start(psInfo);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception" + exception);
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
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception" + exception);
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
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception" + exception);
            }
        }

        private void iOSProfileManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            iOSProfileManagement iOSProfileManagement = new iOSProfileManagement();
            iOSProfileManagement.ShowDialog();
        }

        private void inspectorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessStartInfo psInfo = new ProcessStartInfo
                {
                    FileName = "https://inspector.appiumpro.com/",
                    UseShellExecute = true
                };
                Process.Start(psInfo);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception" + exception);
            }
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
            }
        }

        private void fixInstallationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TroubleShooter troubleShooter = new TroubleShooter();
            troubleShooter.ShowDialog();
        }
    }

    public class Device
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string OS { get; set; }
        public string Udid { get; set; }


    }
}
