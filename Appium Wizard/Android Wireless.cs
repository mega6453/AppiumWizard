using System.Diagnostics;

namespace Appium_Wizard
{
    public partial class AndroidWireless : Form
    {
        MainScreen mainScreen;
        public AndroidWireless(MainScreen mainScreen)
        {
            this.mainScreen = mainScreen;
            InitializeComponent();
            IPAddressTextBox.TextChanged += TextBox_TextChanged;
            PortTextBox.TextChanged += TextBox_TextChanged;
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(IPAddressTextBox.Text) && !string.IsNullOrWhiteSpace(PortTextBox.Text))
            {
                FindDeviceButton.Enabled = true;
            }
            else
            {
                FindDeviceButton.Enabled = false;
            }
        }

        private void FindDeviceButton_Click(object sender, EventArgs e)
        {
            DeviceLookUp deviceLookUp = new DeviceLookUp("Looking for Android device over Wi-Fi...");
            deviceLookUp.Show();
            AndroidMethods.GetInstance().ConnectToAndroidWirelessly(IPAddressTextBox.Text, PortTextBox.Text);
            string address = IPAddressTextBox.Text + ":" + PortTextBox.Text;
            Dictionary<string, string> deviceInfo = AndroidAsyncMethods.GetInstance().GetDeviceInformation(address);
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
                        string[] IPAddress = { "IP Address", address };
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
                        Close();
                    }
                }
                else
                {
                    deviceLookUp.Close();
                    MessageBox.Show(DeviceName.Replace("\n", "") + " already added to the list.", "Add Android Device Over Wi-Fi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                deviceLookUp.Close();
                MessageBox.Show("No Android Device available. Please check if the device is on the same network as this PC.\nMake sure Wireless debugging option enabled in your phone's Developer options.\nMake sure you've entered valid IP Address and Port number.", "Add Android Device Over Wi-Fi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void TryAlternative_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                ProcessStartInfo psInfo = new ProcessStartInfo
                {
                    FileName = "https://play.google.com/work/apps/details?id=moe.haruue.wadb&hl=en&gl=US",
                    UseShellExecute = true
                };
                Process.Start(psInfo);
                GoogleAnalytics.SendEvent("TryAlternative_LinkClicked");
            }
            catch (Exception exception)
            {
                GoogleAnalytics.SendExceptionEvent("TryAlternative_LinkClicked", exception.Message);
            }
        }
    }
}
