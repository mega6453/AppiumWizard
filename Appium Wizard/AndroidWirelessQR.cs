using QRCoder;
using System.Security.Cryptography;

namespace Appium_Wizard
{
    public partial class AndroidWirelessQR : Form
    {
        readonly AndroidWireless androidWireless;
        readonly CancellationTokenSource cts = new CancellationTokenSource();
        string serviceName = string.Empty;
        string password = string.Empty;

        public AndroidWirelessQR(AndroidWireless androidWireless)
        {
            this.androidWireless = androidWireless;
            InitializeComponent();
            Icon = androidWireless.Icon;
        }

        private void cancelButton_Click(object? sender, EventArgs e)
        {
            Close();
        }

        private void AndroidWirelessQR_FormClosing(object? sender, FormClosingEventArgs e)
        {
            cts.Cancel();
        }

        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789";
            char[] result = new char[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = chars[RandomNumberGenerator.GetInt32(chars.Length)];
            }
            return new string(result);
        }

        private async void AndroidWirelessQR_Shown(object? sender, EventArgs e)
        {
            serviceName = "studio-" + RandomString(10);
            password = RandomString(12);
            string payload = "WIFI:T:ADB;S:" + serviceName + ";P:" + password + ";;";
            using (QRCodeGenerator generator = new QRCodeGenerator())
            using (QRCodeData data = generator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.M))
            {
                byte[] png = new PngByteQRCode(data).GetGraphic(10);
                using MemoryStream ms = new MemoryStream(png);
                using Image img = Image.FromStream(ms);
                qrPictureBox.Image = new Bitmap(img);
            }
            await PairAndConnect();
        }

        private async Task PairAndConnect()
        {
            var token = cts.Token;
            string pairingAddress = string.Empty;
            DateTime startTime = DateTime.Now;
            DateTime deadline = startTime.AddMinutes(2);
            bool hintShown = false;
            while (string.IsNullOrEmpty(pairingAddress) && DateTime.Now < deadline && !token.IsCancellationRequested)
            {
                pairingAddress = await Task.Run(() => AndroidMethods.GetInstance().FindPairingAddressByServiceName(serviceName));
                if (string.IsNullOrEmpty(pairingAddress))
                {
                    if (!hintShown && DateTime.Now - startTime > TimeSpan.FromSeconds(20))
                    {
                        hintShown = true;
                        statusLabel.Text = "Still waiting. If the phone shows 'Pairing device...' for long, check the network (same Wi-Fi, no client isolation).";
                    }
                    try { await Task.Delay(1000, token); } catch (TaskCanceledException) { }
                }
            }
            if (token.IsCancellationRequested || IsDisposed)
            {
                return;
            }
            if (string.IsNullOrEmpty(pairingAddress))
            {
                MessageBox.Show("The device was not found in time. Please try the following:\n" +
                    "1.Scan the QR code from Developer options > Wireless debugging > Pair device with QR code.\n" +
                    "2.Make sure the phone and this PC are on the same Wi-Fi network.\n" +
                    "3.Set this PC's Wi-Fi network profile to Private (Windows blocks device discovery on Public networks).\n" +
                    "4.Allow adb.exe in Windows Firewall.\n" +
                    "5.Some routers (guest/office Wi-Fi) block device-to-device discovery. Try another network or a phone hotspot.\n" +
                    "6.Restart Wireless debugging or the phone, then try again.\n" +
                    "Alternatively, use 'Pair Manually'.", "Android Device Over Wi-Fi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
                return;
            }

            statusLabel.Text = "Pairing...";
            string output = await Task.Run(() => AndroidMethods.GetInstance().PairAndroidWirelessly(pairingAddress, password));
            if (token.IsCancellationRequested || IsDisposed)
            {
                return;
            }
            if (!output.Contains("Successfully paired"))
            {
                MessageBox.Show("Unable to pair the device.\n1.Make sure the device is on the same network as this PC.\n2.Restart Wireless debugging and try again.", "Android Device Over Wi-Fi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }

            statusLabel.Text = "Paired. Looking for the device address...";
            string ip = pairingAddress.Split(':')[0];
            string connectAddress = string.Empty;
            for (int i = 0; i < 15 && string.IsNullOrEmpty(connectAddress) && !token.IsCancellationRequested; i++)
            {
                List<string> addresses = await Task.Run(() => AndroidMethods.GetInstance().FindConnectReadyDevicesOverWiFi());
                connectAddress = addresses.FirstOrDefault(a => a.StartsWith(ip + ":")) ?? string.Empty;
                if (string.IsNullOrEmpty(connectAddress))
                {
                    try { await Task.Delay(1000, token); } catch (TaskCanceledException) { }
                }
            }
            if (token.IsCancellationRequested || IsDisposed)
            {
                return;
            }
            if (string.IsNullOrEmpty(connectAddress))
            {
                MessageBox.Show("Paired, but unable to get the Wi-Fi address to connect. Turn Wireless debugging off and on, then use 'Find automatically'.", "Android Device Over Wi-Fi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
                return;
            }

            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Android Wireless device", "Connecting to Android device over Wi-Fi...", 40);
            Hide();
            await androidWireless.GetDeviceInformation(commonProgress, androidWireless, connectAddress);
            Close();
        }
    }
}
