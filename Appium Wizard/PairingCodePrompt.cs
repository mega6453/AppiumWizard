namespace Appium_Wizard
{
    public partial class PairingCodePrompt : Form
    {
        string pairingAddress, connectIPAddress, deviceName;
        AndroidWireless androidWireless;
        public PairingCodePrompt(AndroidWireless androidWireless, string deviceName, string pairingAddress, string connectAddress)
        {
            this.androidWireless = androidWireless;
            this.deviceName = deviceName;
            this.pairingAddress = pairingAddress;
            this.connectIPAddress = connectAddress;
            InitializeComponent();
            label2.Text = pairingAddress;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            Dictionary<string, Tuple<string, string>> listOfDevices = new Dictionary<string, Tuple<string, string>>();
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Android Wireless device", "Pairing Android device over Wi-Fi...", 10);
            string output = string.Empty;
            await Task.Run(() =>
            {
                output = AndroidMethods.GetInstance().PairAndroidWirelessly(pairingAddress, PairingCodeTextBox.Text);
            });            
            if (output.Contains("Successfully paired"))
            {
                commonProgress.UpdateStepLabel("Android Wireless device", "Pairing Android device over Wi-Fi...", 20);
                int retryCount = 1;
                if (string.IsNullOrEmpty(connectIPAddress))
                {
                    await Task.Run(() => {
                        listOfDevices = AndroidMethods.GetInstance().FindPairingReadyDevicesOverWiFi();
                    });
                    if (listOfDevices.ContainsKey(deviceName))
                    {
                        connectIPAddress = listOfDevices[deviceName].Item2;
                    }
                    commonProgress.UpdateStepLabel("Android Wireless device", "Pairing Android device over Wi-Fi...", 30);
                    if (string.IsNullOrEmpty(connectIPAddress))
                    {
                        MessageBox.Show("Please turn off Wireless debugging > turn on > then click below OK button...", "Android Device Over Wi-Fi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        while (string.IsNullOrEmpty(connectIPAddress) && retryCount <= 5)
                        {
                            await Task.Run(() => {
                                listOfDevices = AndroidMethods.GetInstance().FindPairingReadyDevicesOverWiFi();
                            });
                            connectIPAddress = listOfDevices[deviceName].Item2;
                            if (string.IsNullOrEmpty(connectIPAddress))
                            {
                                await Task.Delay(2000);
                            }
                            retryCount++;
                        }
                    }
                }
                if (string.IsNullOrEmpty(connectIPAddress))
                {
                    commonProgress.Close();
                    MessageBox.Show("Unable to get Wi-Fi address to connect to the device. Please try again by restarting device Wi-Fi or rebooting device.", "Android Device Over Wi-Fi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    commonProgress.UpdateStepLabel("Android Wireless device", "Pairing Android device over Wi-Fi...", 40);
                    await androidWireless.GetDeviceInformation(commonProgress, androidWireless, connectIPAddress);
                    Close();
                }
            }
            else if (output.Contains("Wrong password or connection was dropped"))
            {
                commonProgress.Close();
                MessageBox.Show("Wrong pairing code or connection was dropped.\n1.Please enter correct pairing code.\n2.Make sure device is connected to same network.", "Android Device Over Wi-Fi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                commonProgress.Close();
                MessageBox.Show("Unable to Pair the device. Please try the following steps:\n1.Restart Wireless debugging\n2.Restart Mobile Wi-Fi\n3.Restart device", "Android Device Over Wi-Fi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PairingCodeTextBox_TextChanged(object sender, EventArgs e)
        {
            if (PairingCodeTextBox.Text.Length == 6)
            {
                button1.Enabled = true;
            }
            else
            {
                button1.Enabled = false;
            }
        }
    }
}
