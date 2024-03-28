using Microsoft.Web.WebView2.WinForms;
using System;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Reflection;
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
        string deviceName, udid, OSType, OSVersion;
        int screenPort, proxyPort;
        public static ScreenControl? screenControl;
        Dictionary<string, string> deviceSessionId = new Dictionary<string, string>();
        public static Dictionary<string, WebView2> webview2 = new Dictionary<string, WebView2>();
        public static Dictionary<string, Tuple<int, int>> devicePorts = new Dictionary<string, Tuple<int, int>>();
        public ScreenControl(string os,string Version, string udid, int width, int height, string session, string selectedDeviceName, int proxyPort, int screenPort)
        {
            this.OSType = os;
            this.OSVersion = Version;
            this.udid = udid;
            this.width = width;
            this.height = height;
            sessionId = session;
            tempSessionId = session;
            this.deviceName = selectedDeviceName;
            this.proxyPort = proxyPort;
            this.screenPort = screenPort;
            if (devicePorts.ContainsKey(udid))
            {
                devicePorts[udid] = new Tuple<int, int>(screenPort, proxyPort);
            }
            else
            {
                devicePorts.Add(udid, new Tuple<int, int>(screenPort, proxyPort));
            }
            URL = "http://" + IPAddress + ":" + proxyPort;
            InitializeComponent();
            ScreenWebView = new WebView2();
            deviceSessionId.Add(udid, session);
            if (!webview2.ContainsKey(udid))
            {
                webview2.Add(udid, ScreenWebView);
            }
            else
            {
                webview2[udid] = ScreenWebView;
            }
            Task.Run(() =>
            {
                while (true)
                {
                    sessionId = GetSessionID();
                    if (OSType.Equals("Android"))
                    {
                        BeginInvoke(new Action(() =>
                        {
                            if (tempSessionId != sessionId)
                            {
                                ScreenWebView.EnsureCoreWebView2Async();
                                ScreenWebView.Reload();
                                tempSessionId = sessionId;
                            }
                        }));
                    }
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
                if (actualText.Length > 55)
                {
                    truncatedText = actualText.Substring(0, 55) + "...";
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

        private void ScreenControl_Load(object sender, EventArgs e)
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
            LoadScreen(udid, screenPort);
        }

        public async void LoadScreen(string udid, int screenPort)
        {
            //ScreenWebView = webview2[udid];
            //if (!ScreenWebView.Equals(webview2[udid]))
            //{
            //    ScreenWebView = webview2[udid];
            //}
            ScreenWebView = webview2[udid];
            //if (Controls.Contains(ScreenWebView))
            //{
            //    Controls.Remove(ScreenWebView);
            //    Controls.Add(ScreenWebView);
            //}
            string imageUrl = "http://" + IPAddress + ":" + screenPort;
            int imageWidth = width;
            int imageHeight = height;
            var htmlContent = $@"
                                <html>
                                    <head>
                                        <meta http-equiv='cache-control' content='no-cache, no-store, must-revalidate' />
                                        <meta http-equiv='pragma' content='no-cache' />
                                        <meta http-equiv='expires' content='0' />
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
            await ScreenWebView.EnsureCoreWebView2Async();
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
            GoogleAnalytics.SendEvent(MethodBase.GetCurrentMethod().Name);
        }

        public async void LoadDeviceDisconnected(string udid)
        {
            ScreenWebView = webview2[udid];
            string htmlContent = @"<!DOCTYPE html>
                                    <html>
                                    <head>
                                        <style>
                                            body {
                                                display: flex;
                                                justify-content: center;
                                                align-items: center;
                                                height: 100vh;
                                                margin: 0;
                                                background-color: #f0f0f0;
                                            }
                                            h3 {
                                                font-size: 24px;
                                                font-weight: bold;
                                                animation: colorChange 2s infinite;
                                            }
                                            @keyframes colorChange {
                                                0% { color: red; }
                                                50% { color: blue; }
                                                100% { color: green; }
                                            }
                                        </style>
                                    </head>
                                    <body>
                                        <h3>Device disconnected</h3>
                                    </body>
                                    </html>";

            await ScreenWebView.EnsureCoreWebView2Async();
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
            GoogleAnalytics.SendEvent(MethodBase.GetCurrentMethod().Name);
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
                            GoogleAnalytics.SendEvent("Tap_Screen", "Android");
                        }
                        else
                        {
                            iOSAPIMethods.Tap(URL, sessionId, pressX, pressY);
                            GoogleAnalytics.SendEvent("Tap_Screen", "iOS");
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
                            GoogleAnalytics.SendEvent("Press_Hold_Screen", "Android");
                        }
                        else
                        {
                            iOSAPIMethods.Swipe(URL, sessionId, pressX, pressY, moveToX, moveToY, waitDuration);
                            GoogleAnalytics.SendEvent("Press_Hold_Screen", "iOS");
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
                GoogleAnalytics.SendEvent(MethodBase.GetCurrentMethod().Name, "iOS");
            }
            else
            {
                AndroidMethods.GetInstance().SendText(udid, text);
                GoogleAnalytics.SendEvent(MethodBase.GetCurrentMethod().Name, "Android");
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

        private void HomeButton_Click(object sender, EventArgs e)
        {
            if (OSType.Equals("iOS"))
            {
                iOSAPIMethods.GoToHome(URL);
                GoogleAnalytics.SendEvent(MethodBase.GetCurrentMethod().Name,"iOS");
            }
            else
            {
                AndroidMethods.GetInstance().GoToHome(udid);
                GoogleAnalytics.SendEvent(MethodBase.GetCurrentMethod().Name,"Android");
            }
        }
        private void BackButton_Click(object sender, EventArgs e)
        {
            AndroidMethods.GetInstance().Back(udid);
            GoogleAnalytics.SendEvent(MethodBase.GetCurrentMethod().Name);
        }
        private void AlwaysOnTop_Click(object sender, EventArgs e)
        {
            this.TopMost = !this.TopMost;
            //buttonAlwaysOnTop.Text = this.TopMost ? "Disable Always on Top" : "Enable Always on Top";
            buttonAlwaysOnTop.BackColor = this.TopMost ? Color.DarkGreen : SystemColors.Control;
            GoogleAnalytics.SendEvent(MethodBase.GetCurrentMethod().Name);
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
                GoogleAnalytics.SendEvent(MethodBase.GetCurrentMethod().Name,"iOS");
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
                GoogleAnalytics.SendEvent(MethodBase.GetCurrentMethod().Name,"Android");
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
                GoogleAnalytics.SendEvent(MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception" + exception);
                GoogleAnalytics.SendExceptionEvent(MethodBase.GetCurrentMethod().Name,exception.Message);
            }
        }

        private void ScreenControl_FormClosing(object sender, FormClosingEventArgs e)
        {
            webview2.Remove(udid);
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
            GoogleAnalytics.SendEvent(MethodBase.GetCurrentMethod().Name);
        }

        private void ScreenControl_Shown(object sender, EventArgs e)
        {
            string OS = OSType + " " + OSVersion;
            GoogleAnalytics.SendEvent(MethodBase.GetCurrentMethod().Name,OS);
        }
    }
}
