namespace Appium_Wizard
{
    public class OpenDevice
    {
        CommonProgress commonProgress = new CommonProgress();

        public string WDAsessionId = "", UIAutomatorSessionId = "", URL;
        int width, height, proxyPort, screenServerPort;
        public string IPAddress = "localhost";
        string deviceName, udid, OSType;
        bool isScreenServerStarted = false;
        string title;
        public OpenDevice(string udid, string selectedOS, string selectedDeviceName)
        {
            this.udid = udid;
            this.deviceName = selectedDeviceName;
            this.OSType = selectedOS;
            var screeSize = getDeviceScreenSize(udid);
            this.width = screeSize.Item1;
            this.height = screeSize.Item2;
            title = "Opening " + deviceName;
        }

        public (int, int) getDeviceScreenSize(string udid)
        {
            var devicesList = Database.QueryDataFromDevicesTable();
            foreach (var dictionary in devicesList)
            {
                if (dictionary["UDID"].Equals(udid))
                {
                    int Width = int.Parse(dictionary["Width"]);
                    int Height = int.Parse(dictionary["Height"]);
                    return (Width, Height);
                }
            }
            return (0, 0);
        }

        public async void StartBackgroundTasks()
        {
            await Task.Delay(100);
            commonProgress.Show();
            commonProgress.UpdateStepLabel(title, "Initializing...");
            if (OSType.Equals("Android"))
            {
                await ExecuteAndroid();
            }
            else
            {
                await ExecuteiOSBackgroundMethod();
            }
            if (isScreenServerStarted)
            {
                commonProgress.UpdateStepLabel(title, "Screen server started...");
                commonProgress.Close();
                await ExecuteBackgroundMethod2();
            }
            else
            {
                commonProgress.Close();
               // MessageBox.Show("Please restart device and try again.", "Failed starting screen server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static Dictionary<string, Dictionary<object, object>> deviceDetails = new Dictionary<string, Dictionary<object, object>>();
        Dictionary<object, object> keyValuePairs;


        private Task ExecuteiOSBackgroundMethod()
        {
            try
            {
                bool wdaCheck = iOSMethods.GetInstance().iSWDAInstalled(udid);
                bool profileCheck = iOSMethods.GetInstance().isProfileAvailableToSign(udid).Item1;
                if (wdaCheck | profileCheck)
                {
                    keyValuePairs = new Dictionary<object, object>();
                    if (!deviceDetails.ContainsKey(udid))
                    {
                        commonProgress.UpdateStepLabel(title, "Mounting developer disk image. Please wait, this may take some time...");
                        iOSMethods.GetInstance().MountImage(udid);
                        commonProgress.UpdateStepLabel(title, "Starting iOS Proxy Server...");
                        proxyPort = LoadingScreen.WDAproxyPort;
                        screenServerPort = Common.GetFreePort();
                        iOSAsyncMethods.GetInstance().StartiOSProxyServer(udid, proxyPort, 8100);
                        iOSAsyncMethods.GetInstance().StartiOSProxyServer(udid, screenServerPort, 9100);

                        commonProgress.UpdateStepLabel(title, "Checking if WebDriverAgent installed...");
                        bool isWDAInstalled = iOSMethods.GetInstance().iSWDAInstalled(udid);
                        if (isWDAInstalled)
                        {
                            commonProgress.UpdateStepLabel(title, "WebDriverAgent Installed. Starting WebDriverAgent...");
                        }
                        else
                        {
                            commonProgress.UpdateStepLabel(title, "Installing WebDriverAgent. Please wait, this may take some time...");
                            iOSMethods.GetInstance().InstallWDA(udid);
                        }
                        commonProgress.UpdateStepLabel(title, "Starting WebDriverAgent... Please enter passcode in your iPhone if it asks...");
                        WDAsessionId = iOSAsyncMethods.GetInstance().RunWebDriverAgent(commonProgress, udid, proxyPort);
                        if (WDAsessionId.Equals("Enable Developer Mode"))
                        {
                            commonProgress.Close();
                            isScreenServerStarted = false;
                            MessageBox.Show("Please enable Developer Mode in your " + deviceName + " and try again.\nGo to Settings->Privacy & Security->Developer Mode->Turn ON.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else if (WDAsessionId.Equals("Password Protected"))
                        {
                            commonProgress.Close();
                            isScreenServerStarted = false;
                            MessageBox.Show("Please Unlock your " + deviceName + " and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else if (WDAsessionId.Equals("WDA Not Installed"))
                        {
                            commonProgress.Close();
                            isScreenServerStarted = false;
                            MessageBox.Show("WebDriverAgent Not Installed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else if (WDAsessionId.Equals("Timed out") | WDAsessionId.Equals("nosession passcode required"))
                        {
                            commonProgress.Close();
                            isScreenServerStarted = false;
                            MessageBox.Show("Unable to launch WebDriverAgent on your iPhone. Please enter passcode on your " + deviceName + " when it asks.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else if (!WDAsessionId.Equals("nosession"))
                        {
                            //iOSAPIMethods.GoToHome("http://localhost:" + proxyPort);
                            commonProgress.UpdateStepLabel(title, "Getting device screen size...");
                            var screenSize = iOSAPIMethods.GetScreenSize(WDAsessionId, proxyPort);
                            width = screenSize.Item1;
                            height = screenSize.Item2;
                            commonProgress.UpdateStepLabel(title, "Starting device screen streaming...");
                            isScreenServerStarted = true;
                            keyValuePairs.Add("proxyPort", proxyPort);
                            keyValuePairs.Add("screenPort", screenServerPort);
                            keyValuePairs.Add("sessionId", WDAsessionId);
                            keyValuePairs.Add("width", width);
                            keyValuePairs.Add("height", height);
                            deviceDetails.Add(udid, keyValuePairs);
                        }
                        else
                        {
                            commonProgress.Close();
                            isScreenServerStarted = false;
                            MessageBox.Show("Unhandled Exception", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        proxyPort = (int)deviceDetails[udid]["proxyPort"];
                        screenServerPort = (int)deviceDetails[udid]["screenPort"];
                        width = (int)deviceDetails[udid]["width"];
                        height = (int)deviceDetails[udid]["height"];
                        WDAsessionId = deviceDetails[udid]["sessionId"].ToString();
                        isScreenServerStarted = true;
                        WDAsessionId = iOSAPIMethods.GetWDASessionID("http://localhost:" + proxyPort);
                        bool isLoaded = Common.IsLocalhostLoaded("http://localhost:" + screenServerPort);
                        if (!isLoaded)
                        {
                            commonProgress.UpdateStepLabel(title, "Starting screen server...");
                            Common.KillProcessByPortNumber(screenServerPort);
                            iOSAsyncMethods.GetInstance().StartiOSProxyServer(udid, screenServerPort, 9100);
                        }
                        if (WDAsessionId.Equals("nosession"))
                        {
                            commonProgress.UpdateStepLabel(title, "Checking if WebDriverAgent installed...");
                            bool isWDAInstalled = iOSMethods.GetInstance().iSWDAInstalled(udid);
                            if (isWDAInstalled)
                            {
                                commonProgress.UpdateStepLabel(title, "WebDriverAgent Installed. Starting WebDriverAgent...");
                            }
                            else
                            {
                                commonProgress.UpdateStepLabel(title, "Installing WebDriverAgent. Please wait, this may take some time...");
                                iOSMethods.GetInstance().InstallWDA(udid);
                            }
                            commonProgress.UpdateStepLabel(title, "Starting WebDriverAgent... Please enter passcode in your iPhone if it asks...");
                            WDAsessionId = iOSAsyncMethods.GetInstance().RunWebDriverAgent(commonProgress, udid, proxyPort);
                            if (WDAsessionId.Equals("Enable Developer Mode"))
                            {
                                commonProgress.Close();
                                isScreenServerStarted = false;
                                MessageBox.Show("Please enable Developer Mode in your " + deviceName + " and try again.\nGo to Settings->Privacy & Security->Developer Mode->Turn ON.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else if (WDAsessionId.Equals("Password Protected"))
                            {
                                commonProgress.Close();
                                isScreenServerStarted = false;
                                MessageBox.Show("Please Unlock your " + deviceName + " and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else if (WDAsessionId.Equals("WDA Not Installed"))
                            {
                                commonProgress.Close();
                                isScreenServerStarted = false;
                                MessageBox.Show("WebDriverAgent Not Installed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else if (WDAsessionId.Equals("nosession passcode required"))
                            {
                                commonProgress.Close();
                                isScreenServerStarted = false;
                                MessageBox.Show("Unable to launch WebDriverAgent on your iPhone. Please enter passcode on your " + deviceName + " when it asks.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else if (!WDAsessionId.Equals("nosession"))
                            {
                                isScreenServerStarted = true;
                                deviceDetails[udid]["sessionId"] = WDAsessionId;
                            }
                            else
                            {
                                isScreenServerStarted = false;
                                deviceDetails.Remove(udid);
                            }
                        }
                        else
                        {
                            proxyPort = (int)deviceDetails[udid]["proxyPort"];
                            screenServerPort = (int)deviceDetails[udid]["screenPort"];
                            width = (int)deviceDetails[udid]["width"];
                            height = (int)deviceDetails[udid]["height"];
                            WDAsessionId = deviceDetails[udid]["sessionId"].ToString();
                            isScreenServerStarted = true;
                        }
                    }
                }
                else
                {
                    commonProgress.Close();
                    MessageBox.Show("No profile found for device " + deviceName + "(" + udid + ").\nAdd a profile in Tools->iOS Profile Management.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception e)
            {
                commonProgress.Hide();
                MessageBox.Show("Exception : " + e, "Failed to Start Screen Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                commonProgress.Close();
            }
            return Task.Delay(500);
        }

        public static Dictionary<string,string> deviceSessionId = new Dictionary<string,string>();
        private Task ExecuteAndroid()
        {
            string sessionIdCreatedForScreenServer = string.Empty;
            string sessionIdAvailableForAutomation = string.Empty;
            try
            {
                commonProgress.UpdateStepLabel(title, "Checking UIAutomator installation...");
                bool isUIAutomatorInstalled = AndroidMethods.GetInstance().isUIAutomatorInstalled(udid);
                if (!isUIAutomatorInstalled)
                {
                    commonProgress.UpdateStepLabel(title, "Installing UIAutomator...");
                    AndroidMethods.GetInstance().InstallUIAutomator(udid);
                }
            }
            catch (Exception e)
            {
                 MessageBox.Show(e.Message,"Error installing UIAutomator",MessageBoxButtons.OK,MessageBoxIcon.Error);
                 isScreenServerStarted = false;
                 return Task.Delay(100);
            }
            try
            {
                int forwardedProxyPort = AndroidMethods.GetInstance().GetForwardedPort(udid,6790);
                if (forwardedProxyPort != -1)
                {
                    proxyPort = forwardedProxyPort;
                }
                else
                {
                    commonProgress.UpdateStepLabel(title, "Setting up Proxy Server...");
                    proxyPort = Common.GetFreePort(8221, 8299);
                    AndroidMethods.GetInstance().StartAndroidProxyServer(proxyPort, 6790, udid);
                }
                int forwardedScreenPort = AndroidMethods.GetInstance().GetForwardedPort(udid, 7810);
                if (forwardedScreenPort != -1)
                {
                    screenServerPort = forwardedScreenPort;
                }
                else
                {
                    commonProgress.UpdateStepLabel(title, "Setting up Proxy Server...");
                    screenServerPort = Common.GetFreePort();
                    AndroidMethods.GetInstance().StartAndroidProxyServer(screenServerPort, 7810, udid);
                }
                commonProgress.UpdateStepLabel(title, "Checking UIAutomator running status...");
                bool IsUIAutomatorRunning = AndroidMethods.GetInstance().IsUIAutomatorRunning(udid);
                if (!IsUIAutomatorRunning)
                {
                    commonProgress.UpdateStepLabel(title, "Starting UIAutomator...");
                    AndroidAsyncMethods.GetInstance().StartUIAutomatorServer(udid);
                    sessionIdCreatedForScreenServer = AndroidAPIMethods.CreateSession(proxyPort, screenServerPort);
                }
                else
                {
                    commonProgress.UpdateStepLabel(title, "Checking for existing session...");
                    bool isSessionCreated =false, isItValidSession = false;
                    if (deviceSessionId.ContainsKey(udid))
                    {
                        AndroidAPIMethods.DeleteSession(proxyPort);
                    }
                    else
                    {
                        sessionIdAvailableForAutomation = AndroidAPIMethods.GetSessionID(proxyPort);
                        isSessionCreated = !sessionIdAvailableForAutomation.Equals("nosession");
                    }
                    if (isSessionCreated)
                    {
                        string androidId =AppiumServerSetup.GetAndroidId(proxyPort, sessionIdAvailableForAutomation);
                        isItValidSession = AppiumServerSetup.isExpectedDataAvailableInSessionDetails(androidId);
                        if (isItValidSession == false)
                        {
                            isSessionCreated = false;
                        }
                    }
                    if (!isSessionCreated & !isItValidSession)
                    {
                        commonProgress.UpdateStepLabel(title, "Restarting UIAutomator...");
                        AndroidMethods.GetInstance().StopUIAutomator(udid);
                        AndroidAsyncMethods.GetInstance().StartUIAutomatorServer(udid);
                        sessionIdCreatedForScreenServer = AndroidAPIMethods.CreateSession(proxyPort, screenServerPort);
                    }
                }
                if (sessionIdCreatedForScreenServer != string.Empty)
                {
                    deviceSessionId.Add(udid, sessionIdCreatedForScreenServer);
                    UIAutomatorSessionId = sessionIdCreatedForScreenServer;
                    isScreenServerStarted = true;
                }
                else if (sessionIdAvailableForAutomation != string.Empty)
                {
                    UIAutomatorSessionId = sessionIdAvailableForAutomation;
                    isScreenServerStarted = true;
                }
                else
                {
                    isScreenServerStarted = false;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Failed starting screen server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return Task.Delay(100);
        }

        private Task ExecuteBackgroundMethod2()
        {
            ScreenControl screenForm;
            if (OSType.Equals("Android"))
            {
                screenForm = new ScreenControl(OSType, udid, width, height, UIAutomatorSessionId, deviceName, proxyPort, screenServerPort);
            }
            else
            {
                screenForm = new ScreenControl(OSType, udid, width, height, WDAsessionId, deviceName, proxyPort, screenServerPort);
            }
            screenForm.Name = udid;
            screenForm.Show();
            return Task.Delay(500);
        }
    }
}
