using QRCoder;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace Appium_Wizard
{
    public partial class AndroidWirelessQR : Form
    {
        MainScreen mainScreen;
        AndroidWireless androidWireless;
        string pairingCode = string.Empty;
        string pairingAddress = string.Empty;
        int pairingPort = 0;
        System.Windows.Forms.Timer? checkPairingTimer = null;
        bool isPaired = false;

        public AndroidWirelessQR(MainScreen mainScreen, AndroidWireless androidWireless)
        {
            this.mainScreen = mainScreen;
            this.androidWireless = androidWireless;
            InitializeComponent();
        }

        private void AndroidWirelessQR_Load(object sender, EventArgs e)
        {
            StartQRPairing();
        }

        private async void StartQRPairing()
        {
            try
            {
                // Get local IP address
                string localIP = GetLocalIPAddress();
                if (localIP == "NoValidAddress")
                {
                    MessageBox.Show("Could not detect your PC's IP address. Please ensure you're connected to Wi-Fi.", "QR Code Pairing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                // Generate random 6-digit pairing code
                Random random = new Random();
                pairingCode = random.Next(100000, 999999).ToString();

                // Find available port for pairing (typically ADB uses ports in 30000-40000 range)
                pairingPort = GetAvailablePort();

                // Start ADB pairing server
                bool started = await StartADBPairingServer(pairingPort);
                if (!started)
                {
                    MessageBox.Show("Failed to start ADB pairing server. Please try again.", "QR Code Pairing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                pairingAddress = $"{localIP}:{pairingPort}";

                // Generate QR code data in Android's expected format
                string qrData = GenerateQRData(localIP, pairingPort, pairingCode);

                // Create and display QR code
                GenerateQRCode(qrData);

                // Update UI
                lblPairingCode.Text = pairingCode;
                lblIPAddress.Text = pairingAddress;
                lblInstructions.Text = "How to pair:\n\n" +
                                      "Option 1 - Use ADB WiFi Pairing App:\n" +
                                      "  • Install 'LADB' or 'WiFi ADB' app from Play Store\n" +
                                      "  • Scan this QR code\n\n" +
                                      "Option 2 - Manual Pairing (Recommended):\n" +
                                      "  • Click 'Copy Details' button below\n" +
                                      "  • On device: Settings > Developer Options >\n" +
                                      "    Wireless Debugging > Pair with pairing code\n" +
                                      "  • Enter the pairing code and IP shown below";

                // Start monitoring for successful pairing
                StartPairingMonitor();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting up QR pairing: {ex.Message}", "QR Code Pairing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private string GetLocalIPAddress()
        {
            try
            {
                // First try Wi-Fi
                foreach (NetworkInterface netInterface in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 &&
                        netInterface.OperationalStatus == OperationalStatus.Up)
                    {
                        IPInterfaceProperties ipProps = netInterface.GetIPProperties();
                        foreach (UnicastIPAddressInformation addr in ipProps.UnicastAddresses)
                        {
                            if (addr.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                return addr.Address.ToString();
                            }
                        }
                    }
                }

                // Fallback to Ethernet
                foreach (NetworkInterface netInterface in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                        netInterface.OperationalStatus == OperationalStatus.Up)
                    {
                        IPInterfaceProperties ipProps = netInterface.GetIPProperties();
                        foreach (UnicastIPAddressInformation addr in ipProps.UnicastAddresses)
                        {
                            if (addr.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                return addr.Address.ToString();
                            }
                        }
                    }
                }

                return "NoValidAddress";
            }
            catch (Exception)
            {
                return "NoValidAddress";
            }
        }

        private int GetAvailablePort()
        {
            // Use a random port in the range ADB typically uses
            Random random = new Random();
            return random.Next(37000, 40000);
        }

        private async Task<bool> StartADBPairingServer(int port)
        {
            try
            {
                // Start ADB server first
                await Task.Run(() => AndroidMethods.GetInstance().ExecuteCommand("start-server"));
                await Task.Delay(500);

                // Start ADB pairing server - just specify port to make it LISTEN
                // Format: adb pair {port} {password}
                string pairCommand = $"pair {port} {pairingCode}";

                // Run pairing in background using ProcessStartInfo for better control
                var startInfo = new ProcessStartInfo
                {
                    FileName = FilesPath.adbFilePath,
                    Arguments = pairCommand,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                var pairingProcess = new Process { StartInfo = startInfo };

                pairingProcess.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        NLog.LogManager.GetCurrentClassLogger().Info($"ADB Pair Output: {e.Data}");

                        // Check if pairing was successful
                        if (e.Data.Contains("Successfully paired"))
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                lblStatus.Text = "✓ Pairing successful! Looking for device...";
                                lblStatus.ForeColor = Color.Green;
                            });
                        }
                    }
                };

                pairingProcess.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        NLog.LogManager.GetCurrentClassLogger().Error($"ADB Pair Error: {e.Data}");
                    }
                };

                pairingProcess.Start();
                pairingProcess.BeginOutputReadLine();
                pairingProcess.BeginErrorReadLine();

                NLog.LogManager.GetCurrentClassLogger().Info($"Started ADB pairing server on port {port} with code {pairingCode}");

                return true;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, "Failed to start ADB pairing server");
                return false;
            }
        }

        private string GenerateQRData(string ipAddress, int port, string password)
        {
            // Android wireless debugging QR code format
            // Format: WIFI:T:ADB;S:<ip>:<port>;P:<password>;;
            return $"WIFI:T:ADB;S:{ipAddress}:{port};P:{password};;";
        }

        private void GenerateQRCode(string data)
        {
            try
            {
                using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                {
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.M);
                    using (QRCode qrCode = new QRCode(qrCodeData))
                    {
                        // Generate larger QR code for better scanning
                        Bitmap qrCodeImage = qrCode.GetGraphic(10, Color.Black, Color.White, true);
                        pictureBoxQR.Image = qrCodeImage;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating QR code: {ex.Message}", "QR Code Pairing", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StartPairingMonitor()
        {
            checkPairingTimer = new System.Windows.Forms.Timer();
            checkPairingTimer.Interval = 2000; // Check every 2 seconds
            checkPairingTimer.Tick += CheckPairingStatus;
            checkPairingTimer.Start();
        }

        private async void CheckPairingStatus(object? sender, EventArgs e)
        {
            try
            {
                if (isPaired) return;

                // Check if any device has connected via mDNS
                var devices = await Task.Run(() => AndroidMethods.GetInstance().FindConnectReadyDevicesOverWiFi());

                if (devices.Count > 0)
                {
                    isPaired = true;
                    checkPairingTimer?.Stop();

                    lblStatus.Text = "✓ Pairing successful! Connecting to device...";
                    lblStatus.ForeColor = Color.Green;

                    // Connect to the device
                    string connectAddress = devices[0];
                    await ConnectAndAddDevice(connectAddress);
                }
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, "Error checking pairing status");
            }
        }

        private async Task ConnectAndAddDevice(string connectAddress)
        {
            try
            {
                CommonProgress commonProgress = new CommonProgress();
                commonProgress.Show();
                commonProgress.UpdateStepLabel("Android Wireless QR", "Connecting to device...", 50);

                await androidWireless.GetDeviceInformation(commonProgress, androidWireless, connectAddress);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting to device: {ex.Message}", "QR Code Pairing", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AndroidWirelessQR_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                checkPairingTimer?.Stop();
                checkPairingTimer?.Dispose();
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, "Error cleaning up QR pairing");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AndroidWirelessQR_Shown(object sender, EventArgs e)
        {
            GoogleAnalytics.SendEvent("AndroidWirelessQR_Shown");
        }

        private void btnCopyDetails_Click(object sender, EventArgs e)
        {
            try
            {
                string details = $"Pairing Code: {pairingCode}\nIP Address: {pairingAddress}";
                Clipboard.SetText(details);
                MessageBox.Show("Pairing details copied to clipboard!", "Copied", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to copy to clipboard: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPairManually_Click(object sender, EventArgs e)
        {
            try
            {
                // Extract IP and port from pairingAddress (format: "192.168.1.100:38523")
                string[] parts = pairingAddress.Split(':');
                if (parts.Length == 2)
                {
                    string ip = parts[0];
                    string port = parts[1];

                    // Stop the timer while manual dialog is open
                    checkPairingTimer?.Stop();

                    // Open manual pairing dialog with pre-filled values
                    AndroidWirelessManual manualDialog = new AndroidWirelessManual(androidWireless, ip, port, pairingCode);
                    this.Hide();
                    manualDialog.ShowDialog();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening manual pairing: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
