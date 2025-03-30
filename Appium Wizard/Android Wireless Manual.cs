using NLog;

namespace Appium_Wizard
{
    public partial class AndroidWirelessManual : Form
    {
        AndroidWireless androidWireless;
        string networkPortion = string.Empty;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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

        private async void PairButton_Click(object sender, EventArgs e)
        {
            string pairingIPPort = PairingIPTextBox.Text + ":" + PairPortNumberTextbox.Text;
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Android Wireless device", "Pairing Android device over Wi-Fi...", 10);
            string pairOutput = string.Empty;
            await Task.Run(() => {
                pairOutput = AndroidMethods.GetInstance().PairAndroidWirelessly(pairingIPPort, PairingCodeTextbox.Text);
            });            
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
                CancelButton1.Enabled = false;
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
                commonProgress.UpdateStepLabel("Android Wireless device", "Pairing Android device over Wi-Fi...", 90);
            }
            else if (pairOutput.Contains("Wrong password or connection was dropped"))
            {
                commonProgress.Close();
                MessageBox.Show("Wrong pairing code or connection was dropped.\n1.Please enter correct pairing code.\n2.Make sure device is connected to same network.", "Android Device Over Wi-Fi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                commonProgress.Close();
                MessageBox.Show("Unable to Pair the device. Please try the following steps:\n1.Restart Wireless debugging\n2.Restart Mobile Wi-Fi\n3.Restart device", "Android Device Over Wi-Fi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            commonProgress.Close();
        }

        private void CancelButton2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void ConnectButton_Click(object sender, EventArgs e)
        {
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Android Wireless device", "Connecting to Android device over Wi-Fi...", 10);
            string connectIPPort = ConnectIPTextBox.Text + ":" + ConnectPortNumber.Text;
            string connectOutput = string.Empty;
            await Task.Run(() => {
                connectOutput = AndroidMethods.GetInstance().ConnectToAndroidWirelessly(connectIPPort);
            });
            if (connectOutput.Contains("connected to"))
            {
                commonProgress.UpdateStepLabel("Android Wireless device", "Connecting to Android device over Wi-Fi...", 60);
                await androidWireless.GetDeviceInformation(commonProgress, androidWireless, connectIPPort);
                commonProgress.UpdateStepLabel("Android Wireless device", "Connecting to Android device over Wi-Fi...", 90);
                Close();
            }
            else
            {
                commonProgress.Close();
                MessageBox.Show("Please verify entered IP details and Make sure device is connected to same network.", "Android Device Over Wi-Fi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            commonProgress.Close();
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
