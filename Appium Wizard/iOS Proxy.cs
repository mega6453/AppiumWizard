namespace Appium_Wizard
{
    public partial class iOS_Proxy : Form
    {
        public string udid;
        List<Dictionary<string, string>> devicesList;
        public iOS_Proxy()
        {
            InitializeComponent();
        }

        private async void startProxyButton_Click(object sender, EventArgs e)
        {
            if (deviceListComboBox.SelectedIndex.Equals(-1) || string.IsNullOrEmpty(portTextBox.Text))
            {
                MessageBox.Show("Select the device and Enter port number to Start the iOS Proxy.", "Missing fields", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                var selectedDeviceName = deviceListComboBox.SelectedItem;
                foreach (var device in devicesList)
                {
                    if (device.TryGetValue("Name", out string deviceName) && deviceName == selectedDeviceName)
                    {
                        if (device.TryGetValue("UDID", out string udid))
                        {
                            this.udid = udid;
                        }
                    }
                }
                if (string.IsNullOrEmpty(udid))
                {
                    MessageBox.Show("Failed to Start iOS Proxy Server. Please retry after restarting Appium Wizard.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    int port = int.Parse(portTextBox.Text);
                    bool isPortBusy = Common.IsPortBeingUsed(port);
                    if (isPortBusy)
                    {
                        var dialogResult = MessageBox.Show("Are you sure you want to kill the following process running in port number " + port + " and Start the iOS Proxy server on that port?\n\nRunning Process - " + Common.RunNetstatAndFindProcessByPort(port).Item2, "Kill Process?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogResult.Equals(DialogResult.Yes))
                        {
                            CommonProgress commonProgress = new CommonProgress();
                            commonProgress.Show();
                            commonProgress.Owner = this;
                            commonProgress.UpdateStepLabel("Start iOS Proxy Server", "Please wait while killing existing process...", 40);
                            await Task.Run(() =>
                            {
                                Common.KillProcessByPortNumber(port);
                            });
                            commonProgress.UpdateStepLabel("Start iOS Proxy Server", "Please wait while starting iOS Proxy server on port " + port + " for device " + selectedDeviceName + "...", 70);
                            isPortBusy = Common.IsPortBeingUsed(port);
                            if (isPortBusy)
                            {
                                MessageBox.Show("Failed to Start iOS Proxy Server on port number " + port + ",Please try again.", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                iOSAsyncMethods.GetInstance().StartiProxyServer(udid, port, 8100);
                                commonProgress.UpdateStepLabel("Start iOS Proxy Server", "Please wait while starting iOS Proxy server on port " + port + " for device " + selectedDeviceName + "...", 80);
                                isPortBusy = Common.IsPortBeingUsed(port);
                                commonProgress.UpdateStepLabel("Start iOS Proxy Server", "Please wait while starting iOS Proxy server on port " + port + " for device " + selectedDeviceName + "...", 90);
                                if (isPortBusy)
                                {
                                    MessageBox.Show("Proxy Started Successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                {
                                    MessageBox.Show("Failed to Start iOS Proxy Server on port number " + port + ",Please try again.", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            commonProgress.Close();
                        }
                    }
                    else
                    {
                        CommonProgress commonProgress = new CommonProgress();
                        commonProgress.Show();
                        commonProgress.Owner = this;
                        commonProgress.UpdateStepLabel("Start iOS Proxy Server", "Please wait while starting iOS Proxy server on port " + port + " for device " + selectedDeviceName + "...");
                        iOSAsyncMethods.GetInstance().StartiProxyServer(udid, port, 8100);
                        commonProgress.UpdateStepLabel("Start iOS Proxy Server", "Please wait while starting iOS Proxy server on port " + port + " for device " + selectedDeviceName + "...", 70);
                        isPortBusy = Common.IsPortBeingUsed(port);
                        commonProgress.UpdateStepLabel("Start iOS Proxy Server", "Please wait while starting iOS Proxy server on port " + port + " for device " + selectedDeviceName + "...", 80);
                        if (isPortBusy)
                        {
                            MessageBox.Show("Proxy Started Successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Failed to Start iOS Proxy Server on port number " + port + ",Please try again.", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        commonProgress.Close();
                    }
                }
            }
        }

        private void iOS_Proxy_Load(object sender, EventArgs e)
        {
            devicesList = Database.QueryDataFromDevicesTable();
            foreach (var device in devicesList)
            {
                if (device["OS"].Equals("iOS"))
                {
                    deviceListComboBox.Items.Add(device["Name"]);
                }
            }

        }
    }
}
