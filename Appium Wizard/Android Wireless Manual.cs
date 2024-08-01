namespace Appium_Wizard
{
    public partial class AndroidWirelessManual : Form
    {
        AndroidWireless androidWireless;
        string networkPortion = string.Empty;
        public AndroidWirelessManual(AndroidWireless androidWireless)
        {
            this.androidWireless = androidWireless;
            InitializeComponent();
            networkPortion = Common.GetOnlyNetworkPortion() + ".";
            PairingIPTextBox.Text = networkPortion;
            PairingIPTextBox.Select(PairingIPTextBox.Text.Length, 0);
            ConnectIPTextBox.Text = networkPortion;
            ConnectIPTextBox.Select(ConnectIPTextBox.Text.Length, 0);
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void PairButton_Click(object sender, EventArgs e)
        {
            string pairingIPPort = PairingIPTextBox.Text + ":" + PairPortNumberTextbox.Text;
            DeviceLookUp deviceLookUp = new DeviceLookUp("Pairing Android device over Wi-Fi...");
            deviceLookUp.Show();
            var pairOutput = AndroidMethods.GetInstance().PairAndroidWirelessly(pairingIPPort, PairingCodeTextbox.Text);
            if (pairOutput.Contains("Successfully paired"))
            {
                //Disable Pair section
                label1.Enabled = false;
                label7.Enabled = false;
                label4.Enabled = false;
                label9.Enabled = false;
                PairingIPTextBox.Enabled = false;
                PairPortNumberTextbox.Enabled = false;
                PairingCodeTextbox.Enabled = false;
                CancelButton.Enabled = false;
                PairButton.Enabled = false;
                //Enable Connect section
                ConnectIPTextBox.Text = PairingIPTextBox.Text;
                label5.Enabled = true;
                label8.Enabled = true;
                label11.Enabled = true;
                ConnectIPTextBox.Enabled = false;
                ConnectPortNumber.Enabled = true;
                CancelButton2.Enabled = true;
                ConnectButton.Enabled = true;
            }
            else if (pairOutput.Contains("Wrong password or connection was dropped"))
            {
                deviceLookUp.Close();
                MessageBox.Show("Wrong pairing code or connection was dropped.\n1.Please enter correct pairing code.\n2.Make sure device is connected to same network.", "Android Device Over Wi-Fi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                deviceLookUp.Close();
                MessageBox.Show("Unable to Pair the device. Please try the following steps:\n1.Restart Wireless debugging\n2.Restart Mobile Wi-Fi\n3.Restart device", "Android Device Over Wi-Fi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            deviceLookUp.Close();
        }

        private void CancelButton2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            DeviceLookUp deviceLookUp = new DeviceLookUp("Connecting to Android device over Wi-Fi...");
            deviceLookUp.Show();
            string connectIPPort = ConnectIPTextBox.Text + ":" + ConnectPortNumber.Text;
            var connectOutput = AndroidMethods.GetInstance().ConnectToAndroidWirelessly(connectIPPort);
            if (connectOutput.Contains("connected to"))
            {
                androidWireless.GetDeviceInformation(deviceLookUp, androidWireless, connectIPPort);
                Close();
            }
            else
            {
                deviceLookUp.Close();
                MessageBox.Show("Please verify entered IP details and Make sure device is connected to same network.", "Android Device Over Wi-Fi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            deviceLookUp.Close();
        }

        bool isValidPairingIP = false;
        private void PairingCodeTextbox_TextChanged(object sender, EventArgs e)
        {
            if (PairingCodeTextbox.Text.Length == 6 && PairPortNumberTextbox.Text.Length > 0 && isValidPairingIP)
            {
                PairButton.Enabled = true;
            }
            else
            {
                PairButton.Enabled = false;
            }
        }

        private void PairPortNumberTextbox_TextChanged(object sender, EventArgs e)
        {
            if (PairPortNumberTextbox.Text.Length > 0 && PairingCodeTextbox.Text.Length == 6 && isValidPairingIP)
            {
                PairButton.Enabled = true;
            }
            else
            {
                PairButton.Enabled = false;
            }
        }


        private void PairingIPTextBox_TextChanged(object sender, EventArgs e)
        {
            isValidPairingIP = Common.isValidIPAddress(PairingIPTextBox.Text);
            if (isValidPairingIP && PairPortNumberTextbox.Text.Length > 0 && PairingCodeTextbox.Text.Length == 6)
            {
                PairButton.Enabled = true;
            }
            else
            {
                PairButton.Enabled = false;
            }
        }

        private void ConnectPortNumber_TextChanged(object sender, EventArgs e)
        {
            if (ConnectPortNumber.Text.Length > 0)
            {
                ConnectButton.Enabled = true;
            }
            else
            {
                ConnectButton.Enabled = false;
            }
        }
    }
}
