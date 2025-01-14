namespace Appium_Wizard
{
    public partial class iOS_Proxy : Form
    {
        public string udid;
        List<Dictionary<string, string>> devicesList;
        public static string selectediOSProxyMethod = "go";
        public iOS_Proxy()
        {
            InitializeComponent();

            ToolTip toolTip = new ToolTip();

            // Set the tooltip text
            toolTip.SetToolTip(goRadioButtonAuto, "Using Go-iOS");
            toolTip.SetToolTip(pyRadioButtonAuto, "Using Pymobiledevice3");
            toolTip.SetToolTip(iProxyRadioButtonAuto, "Using iProxy");
            toolTip.SetToolTip(goRadioButton, "Using Go-iOS");
            toolTip.SetToolTip(pyRadioButton, "Using Pymobiledevice3");
            toolTip.SetToolTip(iProxyRadioButton, "Using iProxy");
        }

        private async void startProxyButton_Click(object sender, EventArgs e)
        {
            if (deviceListComboBox.SelectedIndex.Equals(-1) || string.IsNullOrEmpty(portTextBox.Text)
                || (!goRadioButton.Checked && !pyRadioButton.Checked && !iProxyRadioButton.Checked))
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
                    string method = "go";
                    if (goRadioButton.Checked)
                    {
                        method = "go";
                    }
                    else if (pyRadioButton.Checked)
                    {
                        method = "py";
                    }
                    else
                    {
                        method = "ip";
                    }
                    int port = int.Parse(portTextBox.Text);
                    bool isPortBusy = true;
                    CommonProgress commonProgress = new CommonProgress();
                    commonProgress.Show();
                    commonProgress.Owner = this;
                    commonProgress.UpdateStepLabel("Start iOS Proxy Server", "Please wait while checking the port number " + port + " availability...", 20);
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
                        var dialogResult = MessageBox.Show(processName + " is running in Port number " + port + ".\nDo you want to kill that process and Start the iOS Proxy server on that port?", "Kill Process?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogResult.Equals(DialogResult.Yes))
                        {
                            commonProgress.UpdateStepLabel("Start iOS Proxy Server", "Please wait while killing the process " + processName + " running in the port number " + port + "...", 40);
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

            var output = Database.QueryDataFromiOSProxyTable();
            if (output.Contains("go"))
            {
                goRadioButtonAuto.Checked = true;
            }
            else if (output.Contains("py"))
            {
                pyRadioButtonAuto.Checked = true;
            }
            else
            {
                iProxyRadioButtonAuto.Checked = true;
            }
        }


        private async Task StartProxyServer(string method, int localPort)
        {
            if (method.Equals("go"))
            {
                iOSAsyncMethods.GetInstance().StartiOSProxyServer(udid, localPort, 8100, iOSAsyncMethods.iOSExecutable.go);
            }
            else if (method.Equals("py"))
            {
                iOSAsyncMethods.GetInstance().StartiOSProxyServer(udid, localPort, 8100, iOSAsyncMethods.iOSExecutable.py);
            }
            else if (method.Equals("ip"))
            {
                iOSAsyncMethods.GetInstance().StartiProxyServer(udid, localPort, 8100);
            }
            GoogleAnalytics.SendEvent("StartProxyServer_"+method);
            await Task.Delay(3000);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            if (goRadioButtonAuto.Checked)
            {
                Database.UpdateDataIntoiOSProxyTable("go");
                selectediOSProxyMethod = "go";
            }
            else if (pyRadioButtonAuto.Checked)
            {
                Database.UpdateDataIntoiOSProxyTable("py");
                selectediOSProxyMethod = "py";
            }
            else
            {
                Database.UpdateDataIntoiOSProxyTable("iproxy");
                selectediOSProxyMethod = "iproxy";
            }
            Close();
        }
    }
}
