using Microsoft.Web.WebView2.WinForms;
using System.Diagnostics;
using Timer = System.Windows.Forms.Timer;

namespace Appium_Wizard
{
    public partial class ScreenControl : Form
    {
        public string sessionId = "", tempSessionId = "", URL;
        int pressX; int pressY; int moveToX; int moveToY;
        int x, y, width, height;
        private Point pressStartPoint;
        private DateTime pressStartTime;
        private const int PressThresholdMilliseconds = 500;
        public string IPAddress = "127.0.0.1";
        string deviceName, udid, OSType;
        int screenPort, proxyPort;
        public static ScreenControl? screenControl;
        Dictionary<string,string> deviceSessionId = new Dictionary<string,string>();
        public ScreenControl(string os, string udid, int width, int height, string session, string selectedDeviceName, int proxyPort, int screenPort)
        {
            this.OSType = os;
            this.udid = udid;
            this.width = width;
            this.height = height;
            sessionId = session;
            tempSessionId = session;
            this.deviceName = selectedDeviceName;
            this.proxyPort = proxyPort;
            this.screenPort = screenPort;
            URL = "http://" + IPAddress + ":" + proxyPort;
            InitializeComponent();
            ScreenWebView = new WebView2();
            deviceSessionId.Add(udid,session);
            Task.Run(() =>
            {
                while (true)
                {
                    sessionId = GetSessionID();
                    BeginInvoke(new Action(() =>
                    {
                        if (tempSessionId != sessionId)
                        {
                            ScreenWebView.Reload();
                            tempSessionId = sessionId;
                        }
                    }));
                    Thread.Sleep(1000);
                }
            });
            screenControl = this;
        }

        public void UpdateStatusLabel(string actualText)
        {
            string truncatedText = string.Empty;
            try
            {
                if (actualText.Length > 45)
                {
                    truncatedText = actualText.Substring(0, 45) + "...";
                    statusLabel.Text = truncatedText;
                }
                else
                {
                    statusLabel.Text = actualText;
                }
                statusLabel.ToolTipText = actualText;
                toolStrip2.Refresh();
            }
            catch (Exception)
            {
            }
        }

        private async void ScreenControl_Load(object sender, EventArgs e)
        {
            this.ClientSize = new Size(width, height + toolStrip1.Height + toolStrip2.Height);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Text = deviceName;
            InitializeWebView();
            if (OSType.Equals("iOS"))
            {
                BackButton.Visible = false;
            }
            toolStrip1.Refresh();
            toolStrip2.Refresh();
            Activate();
            await ScreenWebView.EnsureCoreWebView2Async();
            LoadScreen(ScreenWebView);
        }

        private void LoadScreen(WebView2 ScreenWebView)
        {
            string imageUrl = "http://" + IPAddress + ":" + screenPort;
            int imageWidth = width;
            int imageHeight = height;
            var htmlContent = $@"
                                    <html>
                                        <head>
                                            <style>
                                                body {{
                                                    margin: 0;
                                                    padding: 0;
                                                }}
                                                img {{
                                                    display: block;
                                                    max-width: 100%;
                                                    height: auto;
                                                    margin: 0;
                                                    padding: 0;
                                                }}
                                            </style>
                                        </head>
                                        <body>
                                            <img src=""{imageUrl}"" alt=""Screen Mirroring failed"" width={imageWidth} height={imageHeight}>
                                        </body>
                                    </html>
                                ";

            try
            {
                ScreenWebView.NavigateToString(htmlContent);
            }
            catch (Exception)
            {
                string tempFilePath = Path.GetTempFileName() + ".html";
                File.WriteAllText(tempFilePath, htmlContent);
                ScreenWebView.CoreWebView2.Navigate(tempFilePath);
            }

        }

        private void InitializeWebView()
        {
            SetWebViewSize(width, height);
            Controls.Add(ScreenWebView);
        }

        private void SetWebViewSize(int width, int height)
        {
            ScreenWebView.Width = width;
            ScreenWebView.Height = height;
        }

        private void GetMouseCoordinate(object sender, MouseEventArgs e)
        {
            x = e.X;
            y = e.Y;
        }


        public void WebView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                pressStartPoint = e.Location;
                pressStartTime = DateTime.Now;
                pressX = pressStartPoint.X;
                pressY = pressStartPoint.Y;
            }
        }


        public void WebView_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                moveToX = e.Location.X;
                moveToY = e.Location.Y;
                if (e.Button == MouseButtons.Left)
                {
                    TimeSpan pressDuration = DateTime.Now - pressStartTime;
                    if (pressDuration.TotalMilliseconds < PressThresholdMilliseconds)
                    {
                        //UpdateStatusLabel("Click - "+"x: "+pressX+" y: "+pressY);
                        Console.WriteLine("Short press");
                        if (OSType.Equals("Android"))
                        {
                            AndroidMethods.GetInstance().Tap(udid, pressX, pressY);
                        }
                        else
                        {
                            iOSAPIMethods.Tap(URL, sessionId, pressX, pressY);
                        }
                    }
                    else
                    {
                        //UpdateStatusLabel("Swipe - from: ("+ pressX+","+pressY+") to: ("+ moveToX + "," + moveToY + ")");
                        Console.WriteLine("Press and hold");
                        int waitDuration = 500;
                        if (OSType.Equals("Android"))
                        {
                            AndroidMethods.GetInstance().Swipe(udid, pressX, pressY, moveToX, moveToY, waitDuration);
                        }
                        else
                        {
                            iOSAPIMethods.Swipe(URL, sessionId, pressX, pressY, moveToX, moveToY, waitDuration);
                        }
                    }
                }

            }
            catch (Exception)
            {
            }
        }

        public void SendKeys(object sender, KeyPressEventArgs e)
        {
            string text = e.KeyChar.ToString();
            //UpdateStatusLabel(text);
            if (OSType.Equals("iOS"))
            {
                iOSAPIMethods.SendText(URL, sessionId, text);
            }
            else
            {
                AndroidMethods.GetInstance().SendText(udid, text);
            }
        }

        public string GetSessionID()
        {
            if (OSType.Equals("iOS"))
            {
                try
                {
                    string sessionId = iOSAPIMethods.GetWDASessionID(URL);
                    if (sessionId.Equals("nosession"))
                    {
                        return CreateSession();
                    }
                    else
                    {
                        return sessionId;
                    }
                }
                catch (Exception)
                {
                    try
                    {
                        return CreateSession();
                    }
                    catch (Exception)
                    {
                        return "nosession";
                    }
                }
            }
            else
            {
                try
                {
                    sessionId = AndroidAPIMethods.GetSessionID(proxyPort);
                    if (sessionId.Equals("nosession"))
                    {
                        bool isRunning = AndroidMethods.GetInstance().IsUIAutomatorRunning(udid);
                        if (!isRunning)
                        {
                            AndroidAsyncMethods.GetInstance().StartUIAutomatorServer(udid);
                        }
                        AndroidMethods.GetInstance().StartAndroidProxyServer(proxyPort, 6790, udid);
                        AndroidMethods.GetInstance().StartAndroidProxyServer(screenPort, 7810, udid);
                        sessionId = CreateSession();
                        return sessionId;
                    }
                    else
                    {
                        return sessionId;
                    }
                }
                catch (Exception)
                {
                    try
                    {
                        return CreateSession();
                    }
                    catch (Exception)
                    {
                        return "nosession";
                    }
                }
            }
        }

        public string CreateSession()
        {
            if (OSType.Equals("iOS"))
            {
                return iOSAPIMethods.CreateWDASession(proxyPort);
            }
            else
            {
                return AndroidAPIMethods.CreateSession(proxyPort, screenPort);
            }
        }

        private void Home(object sender, EventArgs e)
        {
            if (OSType.Equals("iOS"))
            {
                iOSAPIMethods.GoToHome(URL);
            }
            else
            {
                AndroidMethods.GetInstance().GoToHome(udid);
            }
        }
        private void BackButton_Click(object sender, EventArgs e)
        {
            AndroidMethods.GetInstance().Back(udid);
        }
        private void buttonAlwaysOnTop_Click(object sender, EventArgs e)
        {
            this.TopMost = !this.TopMost;
            //buttonAlwaysOnTop.Text = this.TopMost ? "Disable Always on Top" : "Enable Always on Top";
            buttonAlwaysOnTop.BackColor = this.TopMost ? Color.DarkGreen : SystemColors.Control;
        }

        bool isControlCenterOpen = false;
        private void controlCenter_Click(object sender, EventArgs e)
        {
            if (OSType.Equals("iOS"))
            {
                if (isControlCenterOpen == false)
                {
                    iOSAPIMethods.OpenControlCenter(URL, sessionId, width, height);
                    //controlCenter.Text = "Close Control Center";
                    isControlCenterOpen = true;
                }
                else
                {
                    iOSAPIMethods.CloseControlCenter(URL, sessionId, width);
                    //controlCenter.Text = "Open Control Center";
                    isControlCenterOpen = false;
                }

            }
            else
            {
                if (isControlCenterOpen == false)
                {
                    AndroidMethods.GetInstance().Swipe(udid, 0, 0, 0, 300, 100);
                    //controlCenter.Text = "Close Control Center";
                    isControlCenterOpen = true;
                }
                else
                {
                    AndroidMethods.GetInstance().Swipe(udid, 0, 300, 0, 0, 100);
                    //controlCenter.Text = "Open Control Center";
                    isControlCenterOpen = false;
                }
            }
        }

        private void ObjectSpy_Click(object sender, EventArgs e)
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

        private void ScreenControl_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Hide();
            //try
            //{
            //    bool isItValidSession = AppiumServerSetup.isExpectedDataAvailableInSessionDetails(screenPort.ToString());
            //    if (!isItValidSession)
            //    {
            //        AndroidAPIMethods.DeleteSession(proxyPort, OpenDevice.deviceSessionId[udid]);
                    OpenDevice.deviceSessionId.Remove(udid);
            //        AndroidMethods.GetInstance().StopAndroidProxyServer(udid, proxyPort);
            //        AndroidMethods.GetInstance().StopAndroidProxyServer(udid, screenPort);
            //        AndroidMethods.GetInstance().StopUIAutomator(udid);
            //    }
            //}
            //catch (Exception)
            //{
            //}
            //AndroidMethods.GetInstance().StopAndroidProxyServer(udid, proxyPort);
            //AndroidMethods.GetInstance().StopAndroidProxyServer(udid, screenPort);
            //Close();
            //Task.Run(() =>
            //{
            //    try
            //    {
            //        //Common.KillProcessByPortNumber(proxyPort);
            //        //Common.KillProcessByPortNumber(screenPort);
            //        if (OSType.Equals("iOS"))
            //        {
            //            // if we kill, then it will create issue when automation running..
            //            //Common.KillProcessById(iOSAsyncMethods.PortProcessId[proxyPort]);
            //            //Common.KillProcessById(iOSAsyncMethods.PortProcessId[screenPort]);
            //        }
            //        else
            //        {
            //            //AndroidAPIMethods.DeleteSession(proxyPort);
            //            //Common.KillProcessById(AndroidMethods.PortProcessId[proxyPort]);
            //            //Common.KillProcessById(AndroidMethods.PortProcessId[screenPort]);
            //            //AndroidMethods.PortProcessId.Remove(proxyPort);
            //            //AndroidMethods.PortProcessId.Remove(screenPort);  
            //        }
            //    }
            //    catch (Exception)
            //    {
            //    }

            //});
            //foreach (Process proc in Process.GetProcessesByName("msedgewebview2"))
            //{
            //    try
            //    {
            //        if (proc.MainWindowHandle == ScreenWebView.Handle)
            //        {
            //            proc.Kill();
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        // Handle exception
            //    }
            //}
            //foreach (Process proc in Process.GetProcessesByName("msedge"))
            //{
            //    try
            //    {
            //        if (proc.MainWindowHandle == ScreenWebView.Handle)
            //        {
            //            proc.Kill();
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        // Handle exception
            //    }
            //}
            //foreach (Process proc in Process.GetProcessesByName("adb"))
            //{
            //    try
            //    {
            //        if (proc.MainWindowHandle == ScreenWebView.Handle)
            //        {
            //            proc.Kill();
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        // Handle exception
            //    }
            //}
        }


        private void UnlockButton_Click(object sender, EventArgs e)
        {

            //if (OSType.Equals("iOS"))
            //{
            //    iOSAPIMethods.SendText(URL, sessionId, "");
            //}
            //else
            //{
            //    //AndroidMethods.GetInstance().SendText(udid, text);
            //}
        }

        private void unlockToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void ScreenControl_FormClosed(object sender, FormClosedEventArgs e)
        {
            //AndroidAPIMethods.DeleteSession(proxyPort, OpenDevice.deviceSessionId[udid]);
            //OpenDevice.deviceSessionId.Remove(udid);
            //AndroidMethods.GetInstance().StopAndroidProxyServer(udid, proxyPort);
            //AndroidMethods.GetInstance().StopAndroidProxyServer(udid, screenPort);
        }
    }
}
