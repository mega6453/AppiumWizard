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
        public string IPAddress = "localhost";
        string deviceName, udid, OSType;
        int screenPort, proxyPort;
        public static ScreenControl? screenControl;
        private Timer reloadTimer, reloadTimer1;
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
            Task.Run(() =>
            {
                while (true)
                {
                    sessionId = GetSessionID();
                    //if (os.Equals("Android"))
                    //{
                    //    bool isLoaded = Common.IsLocalhostLoaded("http://localhost:" + screenPort);
                    //    if (!isLoaded)
                    //    {
                    //        Common.KillProcessByPortNumber(screenPort);
                    //        AndroidMethods.GetInstance().StartAndroidProxyServer(screenPort, 7810, udid);
                    //    }
                    //}
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
            if (OSType.Equals("Android"))
            {
                try
                {
                    reloadTimer = new Timer();
                    reloadTimer.Interval = 3000;
                    reloadTimer.Tick += ReloadTimer_Tick;
                    reloadTimer.Start();

                    reloadTimer1 = new Timer();
                    reloadTimer1.Interval = 1000;
                    reloadTimer1.Tick += ReloadTimer_Tick1;
                    reloadTimer1.Start();
                }
                catch (Exception)
                {
                    MessageBox.Show("Please try again...");
                    Close();
                }
            }
        }

        private void ReloadTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                ScreenWebView.Reload();
                Thread.Sleep(500);
                ScreenWebView.Reload();
                Thread.Sleep(500);
                ScreenWebView.Reload();
                Thread.Sleep(500);
                ScreenWebView.Reload();
                Thread.Sleep(500);
                ScreenWebView.Reload();
                Thread.Sleep(500);
                ScreenWebView.Reload();
                //Thread.Sleep(500);
                //ScreenWebView.Reload();
                reloadTimer.Stop();
            }
            catch (Exception)
            {

            }

        }
        private void ReloadTimer_Tick1(object sender, EventArgs e)
        {
            //bool isScreenLoaded = Common.IsLocalhostLoaded("http://localhost:" + screenPort);
            //if (!isScreenLoaded)
            //{
            //    Common.KillProcessByPortNumber(screenPort);
            //    AndroidMethods.GetInstance().StartAndroidProxyServer(screenPort, 7810, udid);
            //}

            try
            {
                //if (OSType.Equals("Android"))
                if (OSType.Equals("Android") && !sessionId.Equals(tempSessionId))
                {
                    tempSessionId = sessionId;
                    ScreenWebView.Reload();
                    Thread.Sleep(200);
                    ScreenWebView.Reload();
                    Thread.Sleep(200);
                    ScreenWebView.Reload();
                    Thread.Sleep(200);
                    ScreenWebView.Reload();
                    Thread.Sleep(500);
                    ScreenWebView.Reload();
                    Thread.Sleep(500);
                    ScreenWebView.Reload();
                    //Thread.Sleep(500);
                    //ScreenWebView.Reload();
                }
            }
            catch (Exception)
            {
            }

        }

        private void InitializeWebView()
        {
            SetWebViewSize(width, height);
            Controls.Add(ScreenWebView);
            ScreenWebView.Source = new Uri("http://" + IPAddress + ":" + screenPort);
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
            //if (OSType.Equals("Android"))
            //{
            //    ScreenWebView.Reload();
            //}
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
                //return AndroidAPIMethods.GetSessionID(proxyPort);
                try
                {
                    bool isRunning = AndroidMethods.GetInstance().IsUIAutomatorRunning(udid);
                    if (!isRunning)
                    {
                        AndroidAsyncMethods.GetInstance().StartUIAutomatorServer(udid);
                    }
                    return string.Empty;
                    //else
                    //{
                    //    sessionId = AndroidAPIMethods.GetSessionID(proxyPort);
                    //}
                    //if (sessionId.Equals("nosession"))
                    //{
                    //    //sessionId = CreateSession();
                    //    //AndroidMethods.GetInstance().StopAndroidProxyServer(udid, proxyPort);
                    //    AndroidMethods.GetInstance().StopAndroidProxyServer(udid, screenPort);
                    //    //AndroidMethods.GetInstance().StartAndroidProxyServer(proxyPort, 6790, udid);
                    //    AndroidMethods.GetInstance().StartAndroidProxyServer(screenPort, 7810, udid);
                    //    return sessionId;
                    //}
                    //else
                    //{
                    //    return sessionId;
                    //}
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

                //bool isUIAutomatorStarted = AndroidAPIMethods.isUIAutomatorSessionStarted(proxyPort);
                //if (!isUIAutomatorStarted) 
                //{
                //    AndroidAsyncMethods.GetInstance().StartUIAutomatorServer(udid);
                //    AndroidAPIMethods.CreateSession(proxyPort, screenPort);
                //}

                //bool isUIAutomatorRunning = AndroidMethods.GetInstance().IsUIAutomatorRunning(udid);
                //if (!isUIAutomatorRunning)
                //{
                //    AndroidAsyncMethods.GetInstance().StartUIAutomatorServer(udid);
                //}
                //return "";
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
                //bool isRunning = AndroidMethods.GetInstance().IsUIAutomatorRunning(udid);
                //if (!isRunning)
                //{
                //    AndroidAsyncMethods.GetInstance().StartUIAutomatorServer(udid);
                //}
                //AndroidAsyncMethods.GetInstance().StopUIAutomator(udid);
                //AndroidAsyncMethods.GetInstance().StartUIAutomatorServer(udid);
                //AndroidMethods.GetInstance().StartAndroidProxyServer(proxyPort, 6790, udid);
                string sessionId = AndroidAPIMethods.CreateSession(proxyPort, screenPort);
                //Common.KillProcessByPortNumber(screenPort);
                //AndroidMethods.GetInstance().StartAndroidProxyServer(screenPort, 7810, udid);
                return sessionId;
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
            buttonAlwaysOnTop.Text = this.TopMost ? "Disable Always on Top" : "Enable Always on Top";
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
                    controlCenter.Text = "Close Control Center";
                    isControlCenterOpen = true;
                }
                else
                {
                    iOSAPIMethods.CloseControlCenter(URL, sessionId, width);
                    controlCenter.Text = "Open Control Center";
                    isControlCenterOpen = false;
                }

            }
            else
            {
                if (isControlCenterOpen == false)
                {
                    AndroidMethods.GetInstance().Swipe(udid, 0, 0, 0, 300, 100);
                    controlCenter.Text = "Close Control Center";
                    isControlCenterOpen = true;
                }
                else
                {
                    AndroidMethods.GetInstance().Swipe(udid, 0, 300, 0, 0, 100);
                    controlCenter.Text = "Open Control Center";
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
            //            //AndroidMethods.GetInstance().StopAndroidProxyServer(udid, proxyPort);
            //            //AndroidMethods.GetInstance().StopAndroidProxyServer(udid, screenPort);
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
    }
}
