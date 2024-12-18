namespace Appium_Wizard
{
    public partial class iOS_Proxy : Form
    {
        public string udid;
        List<Dictionary<string, string>> devicesList;
        public iOS_Proxy()
        {
            InitializeComponent();

            ToolTip toolTip = new ToolTip();

            // Set the tooltip text
            toolTip.SetToolTip(methodRadioButton1, "Using iProxy");
            toolTip.SetToolTip(methodRadioButton2, "Using Go-iOS");
            toolTip.SetToolTip(methodRadioButton3, "Using Pymobiledevice3");
        }

        private async void startProxyButton_Click(object sender, EventArgs e)
        {
            if (deviceListComboBox.SelectedIndex.Equals(-1) || string.IsNullOrEmpty(portTextBox.Text)
                || (!methodRadioButton1.Checked && !methodRadioButton2.Checked && !methodRadioButton3.Checked))
            {
                MessageBox.Show("Select the device, Enter port number and Select a method to Start the iOS Proxy.", "Missing fields", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    string method = "method1";
                    if (methodRadioButton1.Checked)
                    {
                        method = "method1";
                    }
                    else if (methodRadioButton2.Checked)
                    {
                        method = "method2";
                    }
                    else
                    {
                        method = "method3";
                    }
                    int port = int.Parse(portTextBox.Text);
                    bool isPortBusy = true;
                    CommonProgress commonProgress = new CommonProgress();
                    commonProgress.Show();
                    commonProgress.Owner = this;
                    commonProgress.UpdateStepLabel("Start iOS Proxy Server", "Please wait while checking the port number "+port+" availability...", 20);
                    await Task.Run(() =>
                    {
                        isPortBusy = Common.IsPortBeingUsed(port);
                    });
                    if (isPortBusy)
                    {
                        string processName = Common.RunNetstatAndFindProcessByPort(port).Item2;
                        if (processName.ToLower().Contains("iproxy") || processName.ToLower().Contains("iosserver"))
                        {
                            processName = processName + " (By Appium Wizard)";
                        }
                        var dialogResult = MessageBox.Show(processName+ " is running in Port number "+port+".\nDo you want to kill that process and Start the iOS Proxy server on that port?", "Kill Process?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogResult.Equals(DialogResult.Yes))
                        {
                            commonProgress.UpdateStepLabel("Start iOS Proxy Server", "Please wait while killing the process "+processName+" running in the port number "+port+"...", 40);
                            await Task.Run(() =>
                            {
                                Common.KillProcessByPortNumber(port);
                            });
                            commonProgress.UpdateStepLabel("Start iOS Proxy Server", "Please wait while starting iOS Proxy server on port " + port + " for device " + selectedDeviceName + "...", 50);
                            await Task.Run(() =>
                            {
                                isPortBusy = Common.IsPortBeingUsed(port);
                            });
                            if (isPortBusy)
                            {
                                commonProgress.Close();
                                MessageBox.Show("Failed to Start iOS Proxy Server on port number " + port + ",Please try again.", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                        else
                        {
                            commonProgress.Close();
                            return;
                        }
                    }
                    commonProgress.UpdateStepLabel("Start iOS Proxy Server", "Please wait while starting iOS Proxy server on port " + port + " for device " + selectedDeviceName + "...", 60);
                    await StartProxyServer(method, port);
                    commonProgress.UpdateStepLabel("Start iOS Proxy Server", "Please wait while starting iOS Proxy server on port " + port + " for device " + selectedDeviceName + "...", 70);
                    await Task.Run(() =>
                    {
                        isPortBusy = Common.IsPortBeingUsed(port);
                    });
                    commonProgress.UpdateStepLabel("Start iOS Proxy Server", "Please wait while starting iOS Proxy server on port " + port + " for device " + selectedDeviceName + "...", 80);
                    if (isPortBusy)
                    {
                        commonProgress.Close();
                        MessageBox.Show("Proxy Started Successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Failed to Start iOS Proxy Server on port number " + port + ",Please try again.", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    commonProgress.Close();
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


        private async Task StartProxyServer(string method, int localPort)
        {
            if (method.Equals("method1"))
            {
                iOSAsyncMethods.GetInstance().StartiProxyServer(udid, localPort, 8100);
            }
            else if (method.Equals("method2"))
            {
                iOSAsyncMethods.GetInstance().StartiOSProxyServer(udid, localPort, 8100, iOSAsyncMethods.iOSExecutable.go);
            }
            else if (method.Equals("method3"))
            {
               iOSAsyncMethods.GetInstance().StartiOSProxyServer(udid, localPort, 8100, iOSAsyncMethods.iOSExecutable.py);
            }
            await Task.Delay(3000);
        }
    }
}
