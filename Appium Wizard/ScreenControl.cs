﻿using Appium_Wizard.Properties;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Reflection;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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
        public static Dictionary<string, ScreenControl> udidScreenControl = new Dictionary<string, ScreenControl>();
        Dictionary<string, string> deviceSessionId = new Dictionary<string, string>();
        public static Dictionary<string, WebView2> webview2 = new Dictionary<string, WebView2>();
        public static Dictionary<string, Tuple<int, int>> devicePorts = new Dictionary<string, Tuple<int, int>>();
        string canvasFunction = string.Empty;
        string canvasFunctionID = string.Empty;
        string color = ColorTranslator.ToHtml(Color.Red);
        int lineWidth = 2;
        bool isAndroid;
        public ScreenControl(string os, string Version, string udid, int width, int height, string session, string selectedDeviceName, int proxyPort, int screenPort)
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
                        try
                        {
                            BeginInvoke(new Action(() =>
                            {
                                if (tempSessionId != sessionId)
                                {
                                    try
                                    {
                                        ScreenWebView.EnsureCoreWebView2Async();
                                        ScreenWebView.Reload();
                                        tempSessionId = sessionId;
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                            }));
                        }
                        catch (Exception)
                        {
                        }
                    }
                    Thread.Sleep(1000);
                }
            });
            udidScreenControl.Add(udid, this);
            if (OSType.Equals("Android"))
            {
                isAndroid = true;
            }
            else
            {
                isAndroid = false;
            }
        }

        public void UpdateStatusLabel(ScreenControl screenControl, string actualText)
        {
            try
            {
                BeginInvoke(new Action(() =>
                {
                    toolStripStatusLabel.Text = actualText;
                    toolStripStatusLabel.ToolTipText = actualText;
                }));
            }
            catch (Exception)
            {
            }
        }


        public async Task SetupScreenSharing()
        {
            if (MainScreen.alwaysOnTop)
            {
                this.TopMost = true;
            }
            else
            {
                this.TopMost = false;
            }
            AlwaysOnTopToolStripButton.BackColor = this.TopMost ? Color.DarkGreen : SystemColors.Control;

            this.ClientSize = new Size(width, height + toolStrip1.Height + statusStrip1.Height);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Text = deviceName + "[v" + OSVersion + "]";
            InitializeWebView();
            if (OSType.Equals("iOS"))
            {
                BackToolStripButton.Visible = false;
            }
            toolStrip1.Refresh();
            statusStrip1.Refresh();
            Activate();

            // Await the asynchronous LoadScreen method
            await LoadScreen(udid, screenPort);
        }

        public async Task LoadScreen(string udid, int screenPort)
        {
            ScreenWebView = webview2[udid];
            if (Controls.Contains(ScreenWebView))
            {
                Controls.Remove(ScreenWebView);
                ScreenWebView = new WebView2();
                InitializeWebView();
                webview2[udid] = ScreenWebView;
            }

            string imageUrl = $"http://{IPAddress}:{screenPort}";
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

            var tcs = new TaskCompletionSource<bool>();

            ScreenWebView.CoreWebView2.NavigationCompleted += (sender, args) =>
            {
                if (args.IsSuccess)
                {
                    tcs.TrySetResult(true);
                }
                else
                {
                    tcs.TrySetException(new Exception($"Navigation failed with error code {args.WebErrorStatus}"));
                }
            };

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
            try
            {
                await tcs.Task; // Wait for navigation to complete
            }
            catch (Exception)
            {
                screenControlButtons(false);
                return;
            }

            screenControlButtons(true);
            GoogleAnalytics.SendEvent("LoadScreen");
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
            screenControlButtons(false);
            GoogleAnalytics.SendEvent("LoadDeviceDisconnected");
        }

        private void screenControlButtons(bool enable)
        {
            BackToolStripButton.Enabled = enable;
            ControlCenterToolStripButton.Enabled = enable;
            HomeToolStripButton.Enabled = enable;
            ScreenshotToolStripButton.Enabled = enable;
            SettingsToolStripButton.Enabled = enable;
            MoreToolStripButton.Enabled = enable;
            RecordButton.Enabled = enable;
            objectSpyButton.Enabled = enable;
            recentAppsToolStripButton.Enabled = enable;
        }

        private async void InitializeWebView()
        {
            SetWebViewSize(width, height);
            canvasFunction = JavaScripts.DrawRectangleOnCanvas();
            var env = await CoreWebView2Environment.CreateAsync(null, null, null);
            ScreenWebView.CoreWebView2InitializationCompleted += InitializationCompleted;
            await ScreenWebView.EnsureCoreWebView2Async(env);
            Controls.Add(ScreenWebView);
        }

        private async void InitializationCompleted(object? sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                var canvasFunctionID = await
                    ScreenWebView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(canvasFunction);
            }
        }

        private void SetWebViewSize(int width, int height)
        {
            ScreenWebView.Width = width;
            ScreenWebView.Height = height + toolStrip1.Height + statusStrip1.Height;
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
            moveToX = e.Location.X;
            moveToY = e.Location.Y;
            try
            {
                int swipeThreshold = 50; // Minimum distance in pixels to qualify as a swipe

                // Calculate deltas
                int deltaX = moveToX - pressX;
                int deltaY = moveToY - pressY;

                // Check if the movement qualifies as a swipe
                if (Math.Abs(deltaX) < swipeThreshold && Math.Abs(deltaY) < swipeThreshold)
                {
                    if (isAndroid)
                    {
                        AndroidMethods.GetInstance().Tap(udid, pressX, pressY);
                        GoogleAnalytics.SendEvent("Tap_Screen", "Android");
                    }
                    else
                    {
                        iOSAPIMethods.Tap(URL, sessionId, pressX, pressY);
                        GoogleAnalytics.SendEvent("Tap_Screen", "iOS");
                    }
                    return;
                }

                // Calculate screen-based swipe coordinates
                int horizontalSwipeStartX = (int)(width * 0.1); // Start swipe at 10% of screen width
                int horizontalSwipeEndX = (int)(width * 0.9);   // End swipe at 90% of screen width
                int verticalSwipeStartY = (int)(height * 0.5);  // Start swipe at 50% of screen height
                int verticalSwipeEndY = (int)(height * 0.9);    // End swipe at 90% of screen height
                if (Math.Abs(deltaX) > Math.Abs(deltaY))
                {
                    int waitDuration = 100;
                    if (deltaX > 0)
                    {   // Swipe right
                        if (isAndroid)
                        {
                            AndroidMethods.GetInstance().Swipe(udid, horizontalSwipeStartX, height / 2, horizontalSwipeEndX, height / 2, waitDuration);
                        }
                        else
                        {
                            iOSAPIMethods.Swipe(URL, sessionId, horizontalSwipeStartX, height / 2, horizontalSwipeEndX, height / 2, waitDuration);
                        }
                    }
                    else
                    {
                        // Swipe left
                        if (isAndroid)
                        {
                            AndroidMethods.GetInstance().Swipe(udid, horizontalSwipeEndX, height / 2, horizontalSwipeStartX, height / 2, waitDuration);
                        }
                        else
                        {
                            iOSAPIMethods.Swipe(URL, sessionId, horizontalSwipeEndX, height / 2, horizontalSwipeStartX, height / 2, waitDuration);
                        }
                    }
                }
                else
                {
                    int waitDuration = 1000;
                    if (deltaY > 0)
                    {
                        // Swipe down
                        if (isAndroid)
                        {
                            AndroidMethods.GetInstance().Swipe(udid, width / 2, verticalSwipeStartY, width / 2, verticalSwipeEndY, waitDuration);
                        }
                        else
                        {
                            iOSAPIMethods.Swipe(URL, sessionId, width / 2, verticalSwipeStartY, width / 2, verticalSwipeEndY, waitDuration);
                        }
                    }
                    else
                    {
                        // Swipe up
                        if (isAndroid)
                        {
                            AndroidMethods.GetInstance().Swipe(udid, width / 2, verticalSwipeEndY, width / 2, verticalSwipeStartY, waitDuration);
                        }
                        else
                        {
                            iOSAPIMethods.Swipe(URL, sessionId, width / 2, verticalSwipeEndY, width / 2, verticalSwipeStartY, waitDuration);
                        }
                    }
                }
                if (isAndroid)
                {
                    GoogleAnalytics.SendEvent("SwipeScreen", "Android");
                }
                else
                {
                    GoogleAnalytics.SendEvent("SwipeScreen", "iOS");
                }
            }
            catch (Exception)
            {
            }
        }

        public void SendKeys(object sender, KeyPressEventArgs e)
        {
            string text = e.KeyChar.ToString();
            if (OSType.Equals("iOS"))
            {
                iOSAPIMethods.SendText(URL, sessionId, text);
                GoogleAnalytics.SendEvent("SendKeys", "iOS");
            }
            else
            {
                AndroidMethods.GetInstance().SendText(udid, text);
                GoogleAnalytics.SendEvent("SendKeys", "Android");
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
                            AndroidMethods.GetInstance().StartAndroidProxyServer(proxyPort, 6790, udid);
                            AndroidMethods.GetInstance().StartAndroidProxyServer(screenPort, 7810, udid);
                        }
                        else
                        {
                            AndroidMethods.GetInstance().StartAndroidProxyServer(proxyPort, 6790, udid);
                            AndroidMethods.GetInstance().StartAndroidProxyServer(screenPort, 7810, udid);
                            sessionId = AndroidAPIMethods.GetSessionID(proxyPort);
                        }
                        if (sessionId.Equals("nosession"))
                        {
                            sessionId = CreateSession();
                        }
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
                return AndroidAPIMethods.CreateSession(proxyPort);
            }
        }

        private void HomeButton_Click(object sender, EventArgs e)
        {
            if (OSType.Equals("iOS"))
            {
                iOSAPIMethods.GoToHome(proxyPort);
                GoogleAnalytics.SendEvent(MethodBase.GetCurrentMethod().Name, "iOS");
            }
            else
            {
                AndroidMethods.GetInstance().GoToHome(udid);
                GoogleAnalytics.SendEvent(MethodBase.GetCurrentMethod().Name, "Android");
            }
        }
        private void BackButton_Click(object sender, EventArgs e)
        {
            AndroidMethods.GetInstance().Back(udid);
            GoogleAnalytics.SendEvent("BackButton_Click");
        }
        private void AlwaysOnTop_Click(object sender, EventArgs e)
        {
            this.TopMost = !this.TopMost;
            //buttonAlwaysOnTop.Text = this.TopMost ? "Disable Always on Top" : "Enable Always on Top";
            AlwaysOnTopToolStripButton.BackColor = this.TopMost ? Color.DarkGreen : SystemColors.Control;
            GoogleAnalytics.SendEvent("AlwaysOnTop_Click");
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
                GoogleAnalytics.SendEvent("controlCenter_Click", "iOS");
            }
            else
            {
                if (isControlCenterOpen == false)
                {
                    AndroidMethods.GetInstance().OpenNotification(udid);
                    //controlCenter.Text = "Close Control Center";
                    isControlCenterOpen = true;
                }
                else
                {
                    AndroidMethods.GetInstance().CloseNotification(udid);
                    //controlCenter.Text = "Open Control Center";
                    isControlCenterOpen = false;
                }
                GoogleAnalytics.SendEvent("controlCenter_Click", "Android");
            }
        }

        private void ScreenControl_FormClosing(object sender, FormClosingEventArgs e)
        {
            udidScreenControl.Remove(udid);
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

        private async void ScreenControl_FormClosed(object sender, FormClosedEventArgs e)
        {
            //AndroidAPIMethods.DeleteSession(proxyPort, OpenDevice.deviceSessionId[udid]);
            //OpenDevice.deviceSessionId.Remove(udid);
            //AndroidMethods.GetInstance().StopAndroidProxyServer(udid, proxyPort);
            //AndroidMethods.GetInstance().StopAndroidProxyServer(udid, screenPort);
            if (ScreenWebView != null)
            {
                ScreenWebView.Dispose();
            }
            await Task.Run(() =>
            {
                try
                {
                    if (MainScreen.udidProxyPort.ContainsKey(udid))
                    {
                        Common.KillProcessByPortNumber(MainScreen.udidProxyPort[udid]);
                        MainScreen.udidProxyPort.Remove(udid);
                    }
                    if (MainScreen.udidScreenPort.ContainsKey(udid))
                    {
                        Common.KillProcessByPortNumber(MainScreen.udidScreenPort[udid]);
                        MainScreen.udidScreenPort.Remove(udid);
                    }
                    if (Common.screenRecordingUDIDProcessId.ContainsKey(udid))
                    {
                        Common.KillProcessById(Common.screenRecordingUDIDProcessId[udid]);
                        Common.screenRecordingUDIDProcess.Remove(udid);
                        Common.screenRecordingUDIDProcessId.Remove(udid);
                    }
                    if (Common.screenRecordingUDIDProcess.ContainsKey(udid))
                    {
                        Common.screenRecordingUDIDProcess[udid].Kill();
                        Common.screenRecordingUDIDProcess.Remove(udid);
                        Common.screenRecordingUDIDProcessId.Remove(udid);
                    }
                }
                catch (Exception)
                {
                }
            });

            GoogleAnalytics.SendEvent("ScreenControl_FormClosed");
        }

        private void ScreenControl_Shown(object sender, EventArgs e)
        {
            string OS = OSType + " " + OSVersion;
            GoogleAnalytics.SendEvent(MethodBase.GetCurrentMethod().Name, OS);
        }

        public void DrawRectangle(ScreenControl screenControl, int x, int y, int width, int height)
        {
            try
            {
                BeginInvoke(new Action(() =>
                {
                    screenControl.ScreenWebView.ExecuteScriptAsync($"drawRectangle({x}, {y}, {width}, {height}, {lineWidth}, '{color}', true)");
                    //screenControl.ScreenWebView.ExecuteScriptAsync("drawRectangle(0, 0, 0, 0, 0, '', true)");
                }));
            }
            catch (Exception)
            {
            }
        }


        public void DrawArrow(ScreenControl screenControl, int startX, int startY, int endX, int endY)
        {
            try
            {
                BeginInvoke(new Action(() =>
                {
                    screenControl.ScreenWebView.ExecuteScriptAsync($"drawArrow({startX}, {startY}, {endX}, {endY}, 10, '{color}', true)");
                }));
            }
            catch (Exception)
            {
            }
        }

        public void DrawDot(ScreenControl screenControl, int x, int y)
        {
            try
            {
                BeginInvoke(new Action(() =>
                {
                    screenControl.ScreenWebView.ExecuteScriptAsync($"drawDot({x}, {y}, 5, 'red', true)");
                    //screenControl.ScreenWebView.ExecuteScriptAsync($"drawDot(0, 0, 0, '',true)");

                }));
            }
            catch (Exception)
            {
            }
        }

        static bool isMessageDisplayed = false;
        private async void Screenshot_Click(object sender, EventArgs e)
        {
            string downloadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            string timestamp = DateTime.Now.ToString("dd-MMM-yyyy_hh.mmtt");
            string filePath = Path.Combine(downloadPath, $"Screenshot_{deviceName}_{timestamp}.png");
            filePath = "\"" + filePath + "\"";
            if (OSType.Equals("Android"))
            {
                try
                {
                    await Task.Run(() =>
                    {
                        AndroidMethods.GetInstance().TakeScreenshot(udid, filePath);
                    });
                }
                catch (Exception)
                {
                    MessageBox.Show("Failed to Take Screenshot", "Take Screenshot", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                GoogleAnalytics.SendEvent("Android_TakeScreenshot_ScreenControl");
            }
            else
            {
                try
                {
                    await Task.Run(() =>
                    {
                        iOSMethods.GetInstance().TakeScreenshot(udid, filePath);
                    });
                }
                catch (Exception)
                {
                    MessageBox.Show("Failed to Take Screenshot", "Take Screenshot", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                GoogleAnalytics.SendEvent("iOS_TakeScreenshot_ScreenControl");
            }
            if (MainScreen.ScreenshotNotification)
            {
                Common.ShowNotification("Take Screenshot", "Screenshot saved in Downloads folder.");
            }
            //if (!isMessageDisplayed)
            //{
            //    isMessageDisplayed = true;
            //    MessageBox.Show("Screenshot saved in Downloads folder.\nThis is a one time message for an application lifecycle.", "Take Screenshot", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
        }

        private void SettingsToolStripButton_Click(object sender, EventArgs e)
        {
            if (OSType.Equals("Android"))
            {
                AndroidMethods.GetInstance().LaunchSettings(udid);
                GoogleAnalytics.SendEvent("Android_SettingsToolStripButton_Click");
            }
            else
            {
                string url = "http://localhost:" + proxyPort;
                iOSAPIMethods.LaunchApp(url, sessionId, "com.apple.Preferences");
                GoogleAnalytics.SendEvent("iOS_SettingsToolStripButton_Click");
            }
        }

        private void UnlockScreen_Click(object sender, EventArgs e)
        {
            EnterPassword enterPassword = new EnterPassword(OSType, udid, deviceName);
            enterPassword.ShowDialog();
            GoogleAnalytics.SendEvent("UnlockScreen_Click");
        }

        bool isRecording = false;
        private DateTime recordingStartTime;
        private const int MinimumRecordingDuration = 30;
        private async void RecordButton_Click(object sender, EventArgs e)
        {
            if (!isRecording)
            {
                RecordButton.Image = Resources.record_inprogress;
                recordingStartTime = DateTime.Now;
                isRecording = true;

                try
                {
                    Common common = new Common();
                    await common.StartScreenRecording(udid, deviceName);
                }
                catch (Exception)
                {
                    MessageBox.Show("Failed to Record Scren", "Record Screen", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                TimeSpan recordingDuration = DateTime.Now - recordingStartTime;
                if (recordingDuration.TotalSeconds >= MinimumRecordingDuration)
                {
                    RecordButton.Enabled = false;
                    isRecording = false;
                    try
                    {
                        Common common = new Common();
                        await common.StopScreenRecording(udid);
                    }
                    catch (Exception)
                    {
                    }
                    RecordButton.Image = Resources.record_button;
                    RecordButton.Enabled = true;
                    //MessageBox.Show("Screen Recording saved in Downloads folder.", "Record Screen", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (MainScreen.ScreenRecordingNotification)
                    {
                        Common.ShowNotification("Record Screen", "Screen Recording saved in Downloads folder.");
                    }
                }
                else
                {
                    MessageBox.Show($"Please record for at least {MinimumRecordingDuration} seconds before stopping.", "Record Screen", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            GoogleAnalytics.SendEvent("RecordButton_Click");
        }

        private InstalledAppsList installedAppsListForm;
        private async void manageAppsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GoogleAnalytics.SendEvent("Manage_Apps_Click_ScreenControl");
            if (installedAppsListForm == null || installedAppsListForm.IsDisposed)
            {
                installedAppsListForm = new InstalledAppsList(OSType, udid, deviceName);
                await installedAppsListForm.GetInstalledAppsList(this);
                installedAppsListForm.Show();
            }
            else
            {
                if (installedAppsListForm.WindowState == FormWindowState.Minimized)
                {
                    installedAppsListForm.WindowState = FormWindowState.Normal;
                }
                installedAppsListForm.BringToFront();
            }
        }

        private void inspectorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessStartInfo psInfo = new ProcessStartInfo
                {
                    FileName = "https://inspector.appiumpro.com/",
                    UseShellExecute = true
                };
                Process.Start(psInfo);
                GoogleAnalytics.SendEvent("ObjectSpy_Click");
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception" + exception);
                GoogleAnalytics.SendExceptionEvent("ObjectSpy_Click", exception.Message);
            }
        }

        private void objectSpyButton_Click(object sender, EventArgs e)
        {
            Object_Spy object_Spy = new Object_Spy(OSType, proxyPort, width, height);
            object_Spy.Show();
        }

        private void recentAppsToolStripButton_Click(object sender, EventArgs e)
        {
            if (isAndroid)
            {
                AndroidMethods.GetInstance().ShowRecentApps(udid);
            }
            else
            {
                int startX = width / 2, startY = height; int endX = startX, endY = (int)(height - (height * 0.1)); ;
                iOSAPIMethods.Swipe(URL, sessionId, startX, startY, endX, endY, 500);
            }
        }
    }
}
