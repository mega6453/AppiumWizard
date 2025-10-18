using Appium_Wizard.Appium_Wizard;
using Appium_Wizard.Properties;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using NLog;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;

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
        public string deviceName, udid, OSType, OSVersion, deviceModel;
        public int screenPort, proxyPort;
        public static Dictionary<string, ScreenControl> udidScreenControl = new Dictionary<string, ScreenControl>();
        public static Dictionary<string, string> deviceSessionId = new Dictionary<string, string>();
        public static Dictionary<string, WebView2> webview2 = new Dictionary<string, WebView2>();
        public static Dictionary<string, Tuple<int, int>> devicePorts = new Dictionary<string, Tuple<int, int>>();
        string canvasFunction = string.Empty;
        string canvasFunctionID = string.Empty;
        string color = "#FF0000";
        int lineWidth = 2;
        public bool isAndroid;
        private List<ScreenAction> recordedActions = new List<ScreenAction>();
        private bool isRecordingSteps = false;
        public string sessionURL = string.Empty;
        public int screenDensity = 0;
        public string deviceSerialNumber;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public bool useScrcpy = false;
        public ScreenControl(string os, string Version, string udid, int width, int height, string selectedDeviceName, string deviceModel)
        {
            InitializeComponent();
            this.OSType = os;
            this.OSVersion = Version;
            this.udid = udid;
            this.width = width;
            this.height = height;
            this.deviceModel = deviceModel;
            this.deviceName = selectedDeviceName;
            isAndroid = true;
            screenDensity = (int)AndroidMethods.GetInstance().GetScreenDensity(udid);
            deviceSerialNumber = udid;
            udidScreenControl.Add(udid, this);
            useScrcpy = true;
            infoToolStripMenuItem.Visible = false;
        }

        public ScreenControl(string os, string Version, string udid, int width, int height, string session, string selectedDeviceName, int proxyPort, int screenPort, string deviceModel)
        {
            useScrcpy = false;
            this.OSType = os;
            this.OSVersion = Version;
            this.udid = udid;
            this.width = width;
            this.height = height;
            sessionId = session;
            tempSessionId = session;
            this.deviceModel = deviceModel;
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
            if (deviceSessionId.ContainsKey(udid))
            {
                deviceSessionId[udid] = session;
            }
            else
            {
                deviceSessionId.Add(udid, session);
            }
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
                    sessionURL = URL + "/session/" + sessionId;
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
                screenDensity = (int)AndroidMethods.GetInstance().GetScreenDensity(udid);
                isAndroid = true;
                deviceSerialNumber = udid;
            }
            else
            {
                isAndroid = false;
                deviceSerialNumber = iOSMethods.GetInstance().GetDeviceSerialNumber(udid);
            }
            Logger.Info("Initialization completed");
            this.KeyPreview = true;
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
            Logger.Info("Before loading screen...");
            // Await the asynchronous LoadScreen method
            await LoadScreen(udid, screenPort);
        }

        public async Task LoadScreen(string udid, int screenPort)
        {
            if (useScrcpy)
            {
                await InitializeScrcpy();
                return;
            }
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
                Logger.Info("Starting NavigateToString...");
                ScreenWebView.NavigateToString(htmlContent);
            }
            catch (Exception ex)
            {
                string tempFilePath = Path.GetTempFileName() + ".html";
                File.WriteAllText(tempFilePath, htmlContent);
                ScreenWebView.CoreWebView2.Navigate(tempFilePath);
                Logger.Error(ex, "NavigateToString exception");
            }
            try
            {
                await tcs.Task; // Wait for navigation to complete
            }
            catch (Exception ex)
            {
                screenControlButtons(false);
                Logger.Error(ex, "Wait for navigation to complete exception");
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
            catch (Exception ex)
            {
                string tempFilePath = Path.GetTempFileName() + ".html";
                File.WriteAllText(tempFilePath, htmlContent);
                ScreenWebView.CoreWebView2.Navigate(tempFilePath);
                Logger.Error(ex, "NavigateToString exception");
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
            RecordAndStopRecordingSteps.Enabled = enable;
        }

        private async void InitializeWebView()
        {
            if (useScrcpy)
            {
                return;
            }
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
                if (isRecordingSteps)
                {
                    recordedActions.Add(new ScreenAction
                    {
                        ActionType = "Click on coordinates",
                        X = pressX,
                        Y = pressY
                    });
                }
            }
        }

        public async void WebView_MouseUp(object sender, MouseEventArgs e)
        {
            moveToX = e.Location.X;
            moveToY = e.Location.Y;

            try
            {
                int swipeThreshold = 50; // Minimum distance in pixels to qualify as a swipe or drag-drop
                int longPressThresholdMs = 800; // Duration in ms to qualify as long press
                int dragDropThresholdMs = 1000; // Duration in ms to qualify as drag-drop

                // Calculate deltas
                int deltaX = moveToX - pressX;
                int deltaY = moveToY - pressY;

                // Calculate press duration
                var pressDuration = (DateTime.Now - pressStartTime).TotalMilliseconds;

                // Calculate distance moved (Euclidean)
                double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

                if (distance < swipeThreshold)
                {
                    // Movement small enough for tap or long press
                    if (pressDuration >= longPressThresholdMs)
                    {
                        // Long press
                        if (isAndroid)
                        {
                            await Task.Run(() =>
                            {
                                AndroidMethods.GetInstance().LongPress(udid, pressX, pressY);
                            });
                            GoogleAnalytics.SendEvent("LongPress_Screen", "Android");
                        }
                        else
                        {
                            await Task.Run(() =>
                            {
                                iOSAPIMethods.LongPress(URL, sessionId, pressX, pressY);
                            });
                            GoogleAnalytics.SendEvent("LongPress_Screen", "iOS");
                        }
                    }
                    else
                    {
                        // Tap
                        if (isAndroid)
                        {
                            await Task.Run(() =>
                            {
                                AndroidMethods.GetInstance().Tap(udid, pressX, pressY);
                            });
                            GoogleAnalytics.SendEvent("Tap_Screen", "Android");
                        }
                        else
                        {
                            await Task.Run(() =>
                            {
                                iOSAPIMethods.Tap(URL, sessionId, pressX, pressY);
                            });
                            GoogleAnalytics.SendEvent("Tap_Screen", "iOS");
                        }
                    }
                    return;
                }
                else
                {
                    // Movement exceeds swipe threshold - decide swipe or drag-drop based on duration

                    if (pressDuration >= dragDropThresholdMs)
                    {
                        // Treat as drag-drop
                        if (isAndroid)
                        {
                            await Task.Run(() =>
                            {
                                AndroidAPIMethods.DragDrop(udid, sessionId, proxyPort, pressX, pressY, moveToX, moveToY, speed: 1000);
                            });
                            GoogleAnalytics.SendEvent("DragDrop_Screen", "Android");
                        }
                        else
                        {
                            await Task.Run(() =>
                            {
                                iOSAPIMethods.DragDrop(URL, sessionId, pressX, pressY, moveToX, moveToY);
                            });
                            GoogleAnalytics.SendEvent("DragDrop_Screen", "iOS");
                        }
                    }
                    else
                    {
                        // Treat as swipe
                        int waitDuration = 300;

                        if (isAndroid)
                        {
                            await Task.Run(() =>
                            {
                                AndroidMethods.GetInstance().SwipeForScreenControl(udid, pressX, pressY, moveToX, moveToY, waitDuration);
                            });
                            GoogleAnalytics.SendEvent("SwipeScreen", "Android");
                        }
                        else
                        {
                            await Task.Run(() =>
                            {
                                iOSAPIMethods.Swipe(URL, sessionId, pressX, pressY, moveToX, moveToY, waitDuration);
                            });
                            GoogleAnalytics.SendEvent("SwipeScreen", "iOS");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception in WebView_MouseUp");
            }
        }

        public async void SendKeys(object sender, KeyPressEventArgs e)
        {
            try
            {
                string text = e.KeyChar.ToString();
                if (isRecordingSteps)
                {
                    recordedActions.Add(new ScreenAction
                    {
                        ActionType = "Send Text Without Element",
                        Text = text
                    });
                }
                if (OSType.Equals("iOS"))
                {
                    await Task.Run(() =>
                    {
                        iOSAPIMethods.SendText(URL, sessionId, text);
                    });
                    GoogleAnalytics.SendEvent("SendKeys", "iOS");
                }
                else
                {
                    await Task.Run(() =>
                    {
                        AndroidMethods.GetInstance().SendText(udid, text);
                    });
                    GoogleAnalytics.SendEvent("SendKeys", "Android");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception in SendKeys");
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
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "Exception in CreateSession");
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
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "Exception in CreateSession");
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

        private async void HomeButton_Click(object sender, EventArgs e)
        {
            if (isRecordingSteps)
            {
                recordedActions.Add(new ScreenAction
                {
                    ActionType = "Home",
                });
            }

            await Task.Run(() =>
            {
                if (OSType.Equals("iOS"))
                {
                    iOSAPIMethods.GoToHome(proxyPort);
                    GoogleAnalytics.SendEvent("HomeButton_Click", "iOS");
                }
                else
                {
                    AndroidMethods.GetInstance().GoToHome(udid);
                    GoogleAnalytics.SendEvent("HomeButton_Click", "Android");
                }
            });
        }
        private async void BackButton_Click(object sender, EventArgs e)
        {
            if (isRecordingSteps)
            {
                recordedActions.Add(new ScreenAction
                {
                    ActionType = "Back",
                });
            }
            await Task.Run(() =>
            {
                AndroidMethods.GetInstance().Back(udid);
            });
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
        private async void controlCenter_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
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
            });
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

        public async void DrawRectangle(ScreenControl screenControl, int x, int y, int width, int height)
        {
            try
            {
                if (useScrcpy)
                {
                    DrawRectangleOnScrcpy(x, y, width, height);
                    await Task.Delay(1000);
                    ClearDrawing();
                }
                else
                {
                    BeginInvoke(new Action(() =>
                    {
                        screenControl.ScreenWebView.ExecuteScriptAsync($"drawRectangle({x}, {y}, {width}, {height}, {lineWidth}, '{color}', true)");
                        //screenControl.ScreenWebView.ExecuteScriptAsync("drawRectangle(0, 0, 0, 0, 0, '', true)");
                    }));
                }
            }
            catch (Exception)
            {
            }
        }


        public async void DrawArrow(ScreenControl screenControl, int startX, int startY, int endX, int endY)
        {
            try
            {
                if (useScrcpy) 
                {
                    DrawArrowOnScrcpy(startX, startY, endX, endY, 10);
                    await Task.Delay(1000);
                    ClearDrawing();
                }
                else
                {
                    BeginInvoke(new Action(() =>
                    {
                        screenControl.ScreenWebView.ExecuteScriptAsync($"drawArrow({startX}, {startY}, {endX}, {endY}, 10, '{color}', true)");
                    }));
                }
            }
            catch (Exception)
            {
            }
        }

        public async void DrawDot(ScreenControl screenControl, int x, int y)
        {
            try
            {
                if (useScrcpy)
                {
                    DrawDotOnScrcpy(x, y, 5);
                    await Task.Delay(500);
                    ClearDrawing();
                }
                else
                {
                    BeginInvoke(new Action(() =>
                    {
                        screenControl.ScreenWebView.ExecuteScriptAsync($"drawDot({x}, {y}, 5, 'red', true)");
                        //screenControl.ScreenWebView.ExecuteScriptAsync($"drawDot(0, 0, 0, '',true)");
                    }));
                }
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

        private async void SettingsToolStripButton_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
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
            });
        }

        private void UnlockScreen_Click(object sender, EventArgs e)
        {
            EnterPassword enterPassword = new EnterPassword(OSType, udid, deviceName);
            enterPassword.ShowDialog();
            GoogleAnalytics.SendEvent("UnlockScreen_Click");
        }

        bool isRecordingScreen = false;
        private DateTime recordingStartTime;
        private const int MinimumRecordingDuration = 30;
        private async void RecordButton_Click(object sender, EventArgs e)
        {
            if (!isRecordingScreen)
            {
                RecordButton.Image = Resources.record_inprogress;
                recordingStartTime = DateTime.Now;
                isRecordingScreen = true;

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
                    isRecordingScreen = false;
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
            try
            {
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
                GoogleAnalytics.SendEvent("Manage_Apps_Click_ScreenControl");
            }
            catch (Exception exception)
            {
                GoogleAnalytics.SendExceptionEvent("Manage_Apps_Click_ScreenControl", exception.Message);
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
                GoogleAnalytics.SendExceptionEvent("ObjectSpy_Click", exception.Message);
            }
        }

        private void objectSpyButton_Click(object sender, EventArgs e)
        {
            Object_Spy object_Spy = new Object_Spy(OSType, proxyPort, width, height, sessionId);
            object_Spy.Show();
        }

        private async void recentAppsToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                await Task.Run(() =>
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
                });
                GoogleAnalytics.SendEvent("recentAppsToolStripButton_Click", OSType);
            }
            catch (Exception exception)
            {
                GoogleAnalytics.SendExceptionEvent("recentAppsToolStripButton_Click", exception.Message);
            }
        }

        private void SaveRecordedStepsToJson(string filePath)
        {
            try
            {
                // Transform the recorded actions into the desired format
                var formattedActions = recordedActions.Select(action =>
                {
                    var item = new Dictionary<string, object>();

                    switch (action.ActionType)
                    {
                        case "Set Device":
                            item["Item1"] = "Set Device";
                            item["Item2"] = new Dictionary<string, object>
                                    {
                                        { "Device Name", deviceName }
                                    };
                            break;

                        case "Click on coordinates":
                            item["Item1"] = "Click on coordinates";
                            item["Item2"] = new Dictionary<string, object>
                                            {
                                                { "X", action.X},
                                                { "Y", action.Y}
                                            };
                            break;

                        case "Send Text Without Element":
                            item["Item1"] = "Send Text Without Element";
                            item["Item2"] = new Dictionary<string, object>
                                            {
                                                { "Text to Enter", action.Text }
                                            };
                            break;
                        case "Home":
                            item["Item1"] = "Device Action";
                            item["Item2"] = new Dictionary<string, object>
                                            {
                                                { "Action", "Home" }
                                            };
                            break;
                        case "Back":
                            item["Item1"] = "Device Action";
                            item["Item2"] = new Dictionary<string, object>
                                            {
                                                { "Action", "Back" }
                                            };
                            break;

                        default:
                            item["Item1"] = "Unknown Action";
                            item["Item2"] = new Dictionary<string, object>();
                            break;
                    }

                    return item;
                }).ToList();

                // Serialize the formatted actions to JSON
                string json = JsonSerializer.Serialize(formattedActions, new JsonSerializerOptions { WriteIndented = true });

                // Save the JSON to a file
                //string timestamp = DateTime.Now.ToString("dd-MMM-yyyy_hh.mm.ss_tt");
                //string filePath = Path.Combine(scriptPath, $"StepsRecorder_{deviceName}_{timestamp}.json");
                File.WriteAllText(filePath, json);

                MessageBox.Show($"Steps saved to {filePath}", "Save Steps", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save steps: {ex.Message}", "Save Steps Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void playStepsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isRecordingSteps)
            {
                MessageBox.Show("Recording is in progress. Please stop the recording and then play.", "Play Steps", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (recordedActions.Count <= 1)
            {
                MessageBox.Show("No steps recorded to play.", "Play Steps", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            playStepsToolStripMenuItem.Enabled = false;
            await Task.Run(() =>
            {
                foreach (var action in recordedActions)
                {
                    switch (action.ActionType)
                    {
                        case "Click on coordinates":
                            if (isAndroid)
                            {
                                AndroidMethods.GetInstance().Tap(udid, action.X, action.Y);
                            }
                            else
                            {
                                iOSAPIMethods.Tap(URL, sessionId, action.X, action.Y);
                            }
                            break;

                        case "Send Text Without Element":
                            if (isAndroid)
                            {
                                AndroidMethods.GetInstance().SendText(udid, action.Text);
                            }
                            else
                            {
                                iOSAPIMethods.SendText(URL, sessionId, action.Text);
                            }
                            break;

                        case "Home":
                            if (isAndroid)
                            {
                                AndroidMethods.GetInstance().GoToHome(udid);
                            }
                            else
                            {
                                iOSAPIMethods.GoToHome(proxyPort);
                            }
                            break;

                        case "Back":
                            if (isAndroid)
                            {
                                AndroidMethods.GetInstance().Back(udid);
                            }
                            break;
                    }

                    //await Task.Delay(500); // Add delay between actions
                }
            });
            MessageBox.Show("Steps playback completed.", "Play Steps", MessageBoxButtons.OK, MessageBoxIcon.Information);
            GoogleAnalytics.SendEvent("Steps playback completed", OSType);
            playStepsToolStripMenuItem.Enabled = true;
        }

        private void RecordAndStopRecordingSteps_ButtonClick(object sender, EventArgs e)
        {
            isRecordingSteps = !isRecordingSteps;
            if (isRecordingSteps)
            {
                RecordAndStopRecordingSteps.Image = Resources.RecordStepsGif;
                recordedActions.Clear();
                recordedActions.Add(new ScreenAction
                {
                    ActionType = "Set Device",
                });
                MessageBox.Show("Recording steps started.", "Record Steps", MessageBoxButtons.OK, MessageBoxIcon.Information);
                GoogleAnalytics.SendEvent("Recording steps started", OSType);
            }
            else
            {
                RecordAndStopRecordingSteps.Image = Resources.RecordSteps;
                if (recordedActions.Count <= 1)
                {
                    MessageBox.Show("Recording steps have stopped. But no actions performed on the screen. So, nothing to save or play.", "Record Steps", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                var result = MessageBox.Show("Recording steps have stopped. You can press the play steps button to execute the recorded steps any number of times until you close this screen. If you want to execute it in the future(with Tools->Test Runner), you can save it.\n\nDo you want to save the script ? ", "Record Steps", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                GoogleAnalytics.SendEvent("Recording steps stopped", OSType);
                if (result == DialogResult.Yes)
                {
                    using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                    {
                        saveFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
                        saveFileDialog.Title = "Save Recorded Steps";
                        saveFileDialog.DefaultExt = "json";
                        saveFileDialog.AddExtension = true;
                        string timestamp = DateTime.Now.ToString("dd-MMM-yyyy_hh.mm.ss_tt");
                        saveFileDialog.FileName = $"StepsRecorder_{deviceName}_{timestamp}.json";

                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            // Call the method to save recorded steps and pass the selected file path
                            SaveRecordedStepsToJson(saveFileDialog.FileName);
                            GoogleAnalytics.SendEvent("Recording steps saved in file", OSType);
                        }
                    }
                }
            }
        }

        private void readMeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string message = @"Record and Playback Feature:

                                - This feature allows you to capture your interactions with the mobile device and replay them later.
                                
                                - You can play the recorded steps by clicking Play steps, until this window closed.                                

                                - You can save the steps as a script and use them later in the Test Runner feature which is available under Tools.

                                - It's ideal for automating repetitive tasks and testing workflows efficiently.";

            MessageBox.Show(message, "Record and Playback - Read Me", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void copyProxyPortToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(proxyPort.ToString());
        }

        private void copyScreenPortToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(screenPort.ToString());
        }

        private void copySessionIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(sessionId);
        }

        private void copySessionURLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(sessionURL);
        }

        private void copySerialNumberToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(deviceSerialNumber);
        }

        private void copyModelNumberToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(deviceModel);
        }

        private void copyOSVersionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(OSType + " " + OSVersion);
        }

        private void copyAllInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string deviceName = "Device Name - " + this.deviceName;
            string deviceOS = "Device OS - " + OSType;
            string deviceOSVersion = "OS Version - " + OSVersion;
            string deviceModel = "Model - " + this.deviceModel;
            string deviceSerialNumber = "Serial Number - " + this.deviceSerialNumber;
            string udid = "UDID - " + this.udid;
            string deviceDetails = deviceName + "\n" + deviceOS + "\n" + deviceOSVersion + "\n" + deviceModel + "\n" + deviceSerialNumber + "\n" + udid;
            Clipboard.SetText(deviceDetails);
        }

        private void copyUDIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(this.udid);
        }

        private void ScreenWebView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
            {
                if (OSType.Equals("iOS"))
                {
                    iOSAPIMethods.BackSpace(URL, sessionId);
                }
                else
                {
                    AndroidMethods.GetInstance().BackSpace(udid);
                }
            }
            else if (e.KeyCode == Keys.Delete)
            {
                if (OSType.Equals("Android"))
                {
                    AndroidMethods.GetInstance().Delete(udid);
                }
                //iOS don't have delete option
            }
            else if (e.KeyCode == Keys.Up)
            {
                if (OSType.Equals("Android"))
                {
                    AndroidMethods.GetInstance().UpArrow(udid);
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (OSType.Equals("Android"))
                {
                    AndroidMethods.GetInstance().DownArrow(udid);
                }
            }
            else if (e.KeyCode == Keys.Left)
            {
                if (OSType.Equals("Android"))
                {
                    AndroidMethods.GetInstance().LeftArrow(udid);
                }
            }
            else if (e.KeyCode == Keys.Right)
            {
                if (OSType.Equals("Android"))
                {
                    AndroidMethods.GetInstance().RightArrow(udid);
                }
            }
        }

        private void ScreenWebView_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            Debug.WriteLine($"PreviewKeyDown: {e.KeyCode}");
            if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete ||
                e.KeyCode == Keys.Up || e.KeyCode == Keys.Down ||
                e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                e.IsInputKey = true;
            }
        }

        private void showDeviceInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string deviceName = "Device Name - " + this.deviceName;
            string deviceOS = "Device OS - " + OSType;
            string deviceOSVersion = "OS Version - " + OSVersion;
            string deviceModel = "Model - " + this.deviceModel;
            string deviceSerialNumber = "Serial Number - " + this.deviceSerialNumber;
            string udid = "UDID - " + this.udid;
            string deviceDetails = deviceName + "\n" + deviceOS + "\n" + deviceOSVersion + "\n" + deviceModel + "\n" + deviceSerialNumber + "\n" + udid;
            MessageBox.Show(deviceDetails,"Device Info - "+ this.deviceName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        //<<<------------------------------------------scrcpy embedding methods------------------------------------------>>>
        private ScrcpyEmbedder scrcpyEmbedder;
        private OverlayForm scrcpyOverlayForm;
        private async Task InitializeScrcpy()
        {
            try
            {
                Logger.Info("Initializing scrcpy for Android device...");

                // Create scrcpy embedder
                scrcpyEmbedder = new ScrcpyEmbedder(FilesPath.scrcpy);

                // Create the form but don't show it yet
                this.CreateHandle(); // Force handle creation without showing

                // Configure the host panel
                int topOffset = 0;
                int bottomOffset = 0;

                // Position the panel in the available client area
                scrcpyEmbedder.HostPanel.Location = new Point(0, topOffset);
                scrcpyEmbedder.HostPanel.Size = new Size(width, height - bottomOffset);
                scrcpyEmbedder.HostPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
                scrcpyEmbedder.HostPanel.BackColor = Color.Black;

                // Add to form
                this.Controls.Add(scrcpyEmbedder.HostPanel);

                // Force panel handle creation
                var panelHandle = scrcpyEmbedder.HostPanel.Handle;

                // Add event handlers
                scrcpyEmbedder.HostPanel.MouseDown += WebView_MouseDown;
                scrcpyEmbedder.HostPanel.MouseUp += WebView_MouseUp;
                scrcpyEmbedder.HostPanel.MouseMove += GetMouseCoordinate;

                // Create and show the overlay (top-level window) to avoid flicker
                CreateAndShowOverlay();
                // Position overlay to cover the scrcpy panel
                UpdateOverlayBounds();
                // Initialize overlay with a blank rectangle (none yet)
                scrcpyOverlayForm?.SetRectangle(null);

                // Ensure proper Z-order
                scrcpyEmbedder.HostPanel.BringToFront();

                // Now start scrcpy (this happens hidden)
                bool scrcpyStarted = await scrcpyEmbedder.StartAsync();
                if (!scrcpyStarted)
                {
                    Logger.Error("Failed to start scrcpy");
                    this.Show(); // Show form even on error
                    MessageBox.Show("Failed to start screen mirroring. Go to Menu->Settings->Android Screen Mirroring->Use UiAutomator2", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // small delay to stabilize
                await Task.Delay(1000);

                screenControlButtons(true);
                Logger.Info("Scrcpy started successfully");

                // NOW show the form after everything is ready
                this.Show();
                this.Activate();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error initializing scrcpy");
                this.Show(); // Show form even on error
                MessageBox.Show($"Error starting screen mirroring: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateAndShowOverlay()
        {
            if (scrcpyOverlayForm == null || scrcpyOverlayForm.IsDisposed)
            {
                scrcpyOverlayForm = new OverlayForm
                {
                    LineColor = Color.Red,
                    LineWidth = 2,
                    DebugVisible = false  // <-- CHANGE THIS TO FALSE!
                };

                // Position BEFORE showing
                AttachOverlayPositioning();
                UpdateOverlayBounds();

                // Now show
                scrcpyOverlayForm.Show(this);
                scrcpyOverlayForm.BringToFront();
            }
        }

        private void UpdateOverlayBounds()
        {
            if (scrcpyOverlayForm == null || scrcpyOverlayForm.IsDisposed) return;

            var panelScreenPos = scrcpyEmbedder.HostPanel.PointToScreen(Point.Empty);
            scrcpyOverlayForm.Bounds = new Rectangle(panelScreenPos, scrcpyEmbedder.HostPanel.Size);
            scrcpyOverlayForm.BringToFront();
        }

        private void AttachOverlayPositioning()
        {
            if (scrcpyEmbedder != null && scrcpyEmbedder.HostPanel != null)
            {
                scrcpyEmbedder.HostPanel.SizeChanged += (s, e) => UpdateOverlayBounds();
                scrcpyEmbedder.HostPanel.LocationChanged += (s, e) => UpdateOverlayBounds();
            }
            this.Move += (s, e) => UpdateOverlayBounds();
            this.Resize += (s, e) => UpdateOverlayBounds();
        }

        private Color GetCurrentColor()
        {
            try
            {
                // This handles both hex codes (#FF0000) and named colors (Red)
                return ColorTranslator.FromHtml(color);
            }
            catch
            {
                // If conversion fails, use pure red
                return Color.FromArgb(255, 0, 0); // Pure red: R=255, G=0, B=0
            }
        }

        public void DrawRectangleOnScrcpy(int x, int y, int w, int h)
        {
            if (scrcpyOverlayForm == null || scrcpyOverlayForm.IsDisposed)
                return;

            var rect = new Rectangle(x, y, w, h);
            var drawColor = GetCurrentColor(); // Use same color as WebView2
            if (scrcpyOverlayForm.InvokeRequired)
            {
                scrcpyOverlayForm.BeginInvoke(new Action(() =>
                {
                    scrcpyOverlayForm.LineColor = drawColor;      // Same color as WebView2
                    scrcpyOverlayForm.LineWidth = lineWidth;      // Same lineWidth as WebView2 (should be 2)
                    scrcpyOverlayForm.SetRectangle(rect);
                }));
            }
            else
            {
                scrcpyOverlayForm.LineColor = drawColor;
                scrcpyOverlayForm.LineWidth = lineWidth;
                scrcpyOverlayForm.SetRectangle(rect);
            }
        }

        // Update DrawDotOnScrcpy to use the color field
        public void DrawDotOnScrcpy(int x, int y, int radius = 5)
        {
            if (scrcpyOverlayForm == null || scrcpyOverlayForm.IsDisposed)
                return;

            var point = new Point(x, y);
            var drawColor = GetCurrentColor(); // Use the same color as WebView2

            if (scrcpyOverlayForm.InvokeRequired)
            {
                scrcpyOverlayForm.BeginInvoke(new Action(() =>
                {
                    scrcpyOverlayForm.LineColor = drawColor;
                    scrcpyOverlayForm.SetDot(point, radius);
                }));
            }
            else
            {
                scrcpyOverlayForm.LineColor = drawColor;
                scrcpyOverlayForm.SetDot(point, radius);
            }
        }

        // Update DrawArrowOnScrcpy to use the color field
        public void DrawArrowOnScrcpy(int startX, int startY, int endX, int endY, int arrowHeadSize = 10)
        {
            if (scrcpyOverlayForm == null || scrcpyOverlayForm.IsDisposed)
                return;

            var drawColor = GetCurrentColor(); // Use the same color as WebView2

            if (scrcpyOverlayForm.InvokeRequired)
            {
                scrcpyOverlayForm.BeginInvoke(new Action(() =>
                {
                    scrcpyOverlayForm.LineColor = drawColor;
                    scrcpyOverlayForm.LineWidth = lineWidth; // Use the lineWidth field
                    scrcpyOverlayForm.SetArrow(startX, startY, endX, endY, arrowHeadSize);
                }));
            }
            else
            {
                scrcpyOverlayForm.LineColor = drawColor;
                scrcpyOverlayForm.LineWidth = lineWidth;
                scrcpyOverlayForm.SetArrow(startX, startY, endX, endY, arrowHeadSize);
            }
        }

        public void ClearDrawing()
        {
            if (scrcpyOverlayForm == null || scrcpyOverlayForm.IsDisposed)
                return;

            if (scrcpyOverlayForm.InvokeRequired)
            {
                scrcpyOverlayForm.BeginInvoke(new Action(() =>
                {
                    scrcpyOverlayForm.SetRectangle(null);
                }));
            }
            else
            {
                scrcpyOverlayForm.SetRectangle(null);
            }
        }
        //<<<------------------------------------------------------------------------------------------------------------>>>
    }

    public class ScreenAction
    {
        public string ActionType { get; set; } // "tap", "swipe", "sendkeys"
        public int X { get; set; }
        public int Y { get; set; }
        public string Text { get; set; } // For send keys
    }

    public class OverlayForm : Form
    {
        private Rectangle? rectToDraw;
        private Point? dotToDraw;
        private Arrow? arrowToDraw;

        public Color LineColor { get; set; } = Color.Red;
        public int LineWidth { get; set; } = 2;
        public int DotRadius { get; set; } = 5;
        public bool DebugVisible { get; set; } = false;

        public OverlayForm()
        {
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            DoubleBuffered = true;
            TopMost = true;

            BackColor = Color.Lime;
            TransparencyKey = Color.Lime;

            this.Load += (s, e) => ApplyClickThroughStyles();
        }

        private void ApplyClickThroughStyles()
        {
            const int WS_EX_TRANSPARENT = 0x20;
            const int WS_EX_LAYERED = 0x00080000;

            IntPtr hWnd = this.Handle;
            IntPtr ex = NativeMethods.GetWindowLongPtrFallback(hWnd, NativeMethods.GWL_EXSTYLE);
            long exLong = ex == IntPtr.Zero ? 0 : ex.ToInt64();
            exLong |= (WS_EX_TRANSPARENT | WS_EX_LAYERED);
            NativeMethods.SetWindowLongPtrFallback(hWnd, NativeMethods.GWL_EXSTYLE, new IntPtr(exLong));
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= NativeMethods.WS_EX_TRANSPARENT | NativeMethods.WS_EX_LAYERED;
                return cp;
            }
        }

        public void SetRectangle(Rectangle? r)
        {
            rectToDraw = r;
            dotToDraw = null;
            arrowToDraw = null;
            Invalidate();
            Update();
        }

        public void SetDot(Point? p, int radius = 5)
        {
            dotToDraw = p;
            DotRadius = radius;
            rectToDraw = null;
            arrowToDraw = null;
            Invalidate();
            Update();
        }

        public void SetArrow(int startX, int startY, int endX, int endY, int arrowHeadSize = 10)
        {
            arrowToDraw = new Arrow
            {
                StartX = startX,
                StartY = startY,
                EndX = endX,
                EndY = endY,
                ArrowHeadSize = arrowHeadSize
            };
            rectToDraw = null;
            dotToDraw = null;
            Invalidate();
            Update();
        }

        public void ClearAll()
        {
            rectToDraw = null;
            dotToDraw = null;
            arrowToDraw = null;
            Invalidate();
            Update();
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.Clear(BackColor);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            // Add these for better rendering
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

            if (DebugVisible)
            {
                using (var br = new SolidBrush(Color.FromArgb(80, Color.Yellow)))
                {
                    e.Graphics.FillRectangle(br, ClientRectangle);
                }
            }

            // Draw Rectangle
            if (rectToDraw.HasValue)
            {
                var r = rectToDraw.Value;
                r.Intersect(new Rectangle(0, 0, Width, Height));

                if (r.Width > 0 && r.Height > 0)
                {
                    using (var pen = new Pen(LineColor, LineWidth))
                    {
                        // REMOVE Inset alignment - this was causing the darker color!
                        // pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;

                        // Use Center alignment for clearer, brighter color
                        pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Center;
                        e.Graphics.DrawRectangle(pen, r);
                    }
                }
            }

            // Draw Dot
            if (dotToDraw.HasValue)
            {
                var p = dotToDraw.Value;
                if (p.X >= 0 && p.X < Width && p.Y >= 0 && p.Y < Height)
                {
                    using (var brush = new SolidBrush(LineColor))
                    {
                        int diameter = DotRadius * 2;
                        e.Graphics.FillEllipse(brush, p.X - DotRadius, p.Y - DotRadius, diameter, diameter);
                    }
                }
            }

            // Draw Arrow
            if (arrowToDraw.HasValue)
            {
                var arrow = arrowToDraw.Value;
                using (var pen = new Pen(LineColor, LineWidth))
                {
                    pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Center; // Same as rectangle

                    // Draw the main line
                    e.Graphics.DrawLine(pen, arrow.StartX, arrow.StartY, arrow.EndX, arrow.EndY);

                    // Calculate arrow head
                    double angle = Math.Atan2(arrow.EndY - arrow.StartY, arrow.EndX - arrow.StartX);
                    double arrowAngle = Math.PI / 6;

                    int arrowHead1X = (int)(arrow.EndX - arrow.ArrowHeadSize * Math.Cos(angle - arrowAngle));
                    int arrowHead1Y = (int)(arrow.EndY - arrow.ArrowHeadSize * Math.Sin(angle - arrowAngle));
                    int arrowHead2X = (int)(arrow.EndX - arrow.ArrowHeadSize * Math.Cos(angle + arrowAngle));
                    int arrowHead2Y = (int)(arrow.EndY - arrow.ArrowHeadSize * Math.Sin(angle + arrowAngle));

                    e.Graphics.DrawLine(pen, arrow.EndX, arrow.EndY, arrowHead1X, arrowHead1Y);
                    e.Graphics.DrawLine(pen, arrow.EndX, arrow.EndY, arrowHead2X, arrowHead2Y);
                }
            }
        }

    
        public struct Arrow
        {
            public int StartX;
            public int StartY;
            public int EndX;
            public int EndY;
            public int ArrowHeadSize;
        }

        internal static class NativeMethods
        {
            public const int GWL_EXSTYLE = -20;
            public const int WS_EX_TRANSPARENT = 0x20;
            public const int WS_EX_LAYERED = 0x00080000;

            [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", SetLastError = true)]
            public static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

            [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
            public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

            [DllImport("user32.dll", EntryPoint = "GetWindowLong", SetLastError = true)]
            public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

            [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
            public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

            public static IntPtr GetWindowLongPtrFallback(IntPtr hWnd, int nIndex)
            {
                return IntPtr.Size == 8 ? GetWindowLongPtr(hWnd, nIndex) : GetWindowLong(hWnd, nIndex);
            }

            public static IntPtr SetWindowLongPtrFallback(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
            {
                return IntPtr.Size == 8 ? SetWindowLongPtr(hWnd, nIndex, dwNewLong) : SetWindowLong(hWnd, nIndex, dwNewLong);
            }
        }
    }
}
