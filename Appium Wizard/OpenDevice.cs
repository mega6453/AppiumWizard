using NLog;

namespace Appium_Wizard
{
    public class OpenDevice
    {
        CommonProgress commonProgress = new CommonProgress();

        public string WDAsessionId = "", UIAutomatorSessionId = "", URL;
        int width, height, proxyPort, screenServerPort;
        public string IPAddress = "localhost";
        string deviceName, udid, OSType, OSVersion, connectionType;
        bool isScreenServerStarted = false;
        string title;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public OpenDevice(string udid, string selectedOS, string selectedVersion, string selectedDeviceName, string connectionType, string deviceIPAddress)
        {
            this.udid = udid;
            this.deviceName = selectedDeviceName;
            this.OSType = selectedOS;
            this.OSVersion = selectedVersion;
            this.connectionType = connectionType;
            var screeSize = getDeviceScreenSize(udid);
            this.width = screeSize.Item1;
            this.height = screeSize.Item2;
            title = "Opening " + deviceName;
            if (OSType.Equals("Android") && connectionType.Equals("Wi-Fi"))
            {
                this.udid = deviceIPAddress;
            }
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

        public async Task<bool> StartBackgroundTasks()
        {
            commonProgress.Owner = MainScreen.main;
            commonProgress.Show();
            commonProgress.UpdateStepLabel(title, "Initializing...", 5);
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
                commonProgress.UpdateStepLabel(title, "Screen server started...", 100);
                commonProgress.Close();
                await ExecuteBackgroundMethod2();
            }
            else
            {
                commonProgress.Close();
                // MessageBox.Show("Please restart device and try again.", "Failed starting screen server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return isScreenServerStarted;
        }

        public static Dictionary<string, Dictionary<object, object>> deviceDetails = new Dictionary<string, Dictionary<object, object>>();
        Dictionary<object, object> keyValuePairs;


        private async Task ExecuteiOSBackgroundMethod()
        {
            bool usePreInstalledWDA = MainScreen.UDIDPreInstalledWDA.ContainsKey(udid);
            Logger.Info("usePreInstalledWDA : " + usePreInstalledWDA);
            await Task.Run(() =>
            {
                var deviceList = iOSMethods.GetInstance().GetListOfDevicesUDID();
                if (!deviceList.Contains(udid))
                {
                    MessageBox.Show("Device not found. Please re-connect the device and try again.", "Device Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    isScreenServerStarted = false;
                    return;
                }
                try
                {
                    if (deviceDetails.ContainsKey(udid))
                    {
                        Logger.Info("deviceDetails.ContainsKey : "+udid);
                        proxyPort = (int)deviceDetails[udid]["proxyPort"];
                        screenServerPort = (int)deviceDetails[udid]["screenPort"];
                        width = (int)deviceDetails[udid]["width"];
                        height = (int)deviceDetails[udid]["height"];
                    }
                    else
                    {
                        Logger.Info("deviceDetails does not contain : " + udid);
                        proxyPort = Common.GetFreePort();
                        screenServerPort = Common.GetFreePort();
                    }
                    commonProgress.UpdateStepLabel(title, "Starting iOS Proxy Server...", 50);
                    if (connectionType.Equals("Wi-Fi"))
                    {
                        Logger.Info("connectionType.Equals(\"Wi-Fi\"), Start Go-iOS Proxy server");
                        iOSAsyncMethods.GetInstance().StartiOSProxyServer(udid, proxyPort, 8100);
                        iOSAsyncMethods.GetInstance().StartiOSProxyServer(udid, screenServerPort, 9100);
                    }
                    else
                    {
                        Logger.Info("connectionType NOT Equals(\"Wi-Fi\"), Start iProxy server");
                        iOSAsyncMethods.GetInstance().StartiProxyServer(udid, proxyPort, 8100, screenServerPort, 9100);
                    }
                    WDAsessionId = iOSAPIMethods.GetWDASessionID("http://localhost:" + proxyPort);
                    if (!WDAsessionId.Equals("nosession"))
                    {
                        if (width == 0 | height == 0)
                        {
                            commonProgress.UpdateStepLabel(title, "Getting device screen size...", 80);
                            var screenSize = iOSAPIMethods.GetScreenSize(proxyPort);
                            width = screenSize.Item1;
                            height = screenSize.Item2;
                        }
                        commonProgress.UpdateStepLabel(title, "Starting device screen streaming...", 95);
                        isScreenServerStarted = true;
                    }
                    else
                    {
                        bool wdaCheck = false; bool profileCheck = false;
                        commonProgress.UpdateStepLabel(title, "Checking if WebDriverAgent installed...", 10);
                        bool isInstalled = iOSMethods.GetInstance().iSWDAInstalled(udid);
                        if (usePreInstalledWDA)
                        {
                            if (isInstalled)
                            {
                                wdaCheck = true;
                                Logger.Info("wda installed");
                            }
                            else
                            {
                                wdaCheck = false;
                                Logger.Info("wda not installed");
                                profileCheck = iOSMethods.GetInstance().isProfileAvailableToSign(udid).Item1;
                                Logger.Info("isProfileAvailableToSign: "+profileCheck);
                            }
                        }
                        else
                        {
                            //commonProgress.UpdateStepLabel(title, "Checking if latest version of WebDriverAgent installed...", 10);
                            wdaCheck = iOSMethods.GetInstance().isLatestVersionWDAInstalled(udid);
                            Logger.Info("isLatestVersionWDAInstalled:"+wdaCheck);
                            commonProgress.UpdateStepLabel(title, "Checking if provisioning profile available to sign WDA...", 15);
                            profileCheck = iOSMethods.GetInstance().isProfileAvailableToSign(udid).Item1;
                            if (isInstalled && !wdaCheck && !profileCheck) // WDA installed but not latest version and profile not available to sign.
                            {
                                Logger.Info("WDA installed but not latest version and profile not available to sign, so considering WDA available");
                                wdaCheck = true;  // considering WDA available, so that it won't try to install again and not throw error saying no profile found.
                            }
                        }
                        if (wdaCheck | profileCheck)
                        {
                            keyValuePairs = new Dictionary<object, object>();
                            bool installedNow = false;
                            bool isRunning = false;
                            if (!wdaCheck)
                            {
                                if (usePreInstalledWDA)
                                {
                                    commonProgress.UpdateStepLabel(title, "No Pre-Installed WDA(" + MainScreen.UDIDPreInstalledWDA[udid] + ")found. Please wait, while installing WDA...", 20);
                                }
                                else
                                {
                                    commonProgress.UpdateStepLabel(title, "Installing WebDriverAgent. Please wait, this may take some time...", 20);
                                }
                                installedNow = iOSMethods.GetInstance().InstallWDA(udid);
                            }
                            if (installedNow)
                            {
                                isRunning = false;
                            }
                            else
                            {
                                isRunning = iOSAPIMethods.IsWDARunning(proxyPort);
                            }
                            if (isRunning)
                            {
                                commonProgress.UpdateStepLabel(title, "Getting device screen size...", 80);
                                var screenSize = iOSAPIMethods.GetScreenSize(proxyPort);
                                width = screenSize.Item1;
                                height = screenSize.Item2;
                                commonProgress.UpdateStepLabel(title, "Starting device screen streaming...", 95);
                                isScreenServerStarted = true;
                                keyValuePairs.Add("proxyPort", proxyPort);
                                keyValuePairs.Add("screenPort", screenServerPort);
                                keyValuePairs.Add("sessionId", WDAsessionId);
                                keyValuePairs.Add("width", width);
                                keyValuePairs.Add("height", height);
                                deviceDetails.Remove(udid);
                                deviceDetails.Add(udid, keyValuePairs);
                                if (MainScreen.udidProxyPort.ContainsKey(udid))
                                {
                                    MainScreen.udidProxyPort[udid] = proxyPort;
                                }
                                else
                                {
                                    MainScreen.udidProxyPort.Add(udid, proxyPort);
                                }
                                if (MainScreen.udidScreenPort.ContainsKey(udid))
                                {
                                    MainScreen.udidScreenPort[udid] = screenServerPort;
                                }
                                else
                                {
                                    MainScreen.udidScreenPort.Add(udid, screenServerPort);
                                }
                            }
                            else
                            {
                                Version deviceVersion = new Version(OSVersion);
                                Logger.Info(deviceVersion);
                                Version version17Plus = new Version("17.0.0");
                                if (deviceVersion >= version17Plus)
                                {
                                    Logger.Info("device version >= 17.0.0 , so creating tunnel");
                                    //commonProgress.UpdateStepLabel(title, "Please wait while creating tunnel, This may take few seconds...", 35);
                                    bool isTunnelStarted = iOSAsyncMethods.GetInstance().CreateTunnelGo();
                                    if (isTunnelStarted)
                                    {
                                        Logger.Info("tunnel started");
                                        iOSMethods.isGo = true;
                                        iOSAsyncMethods.isGo = true;
                                    }
                                    else
                                    {
                                        Logger.Info("tunnel not started");
                                        var result = MessageBox.Show("Tunnel creation failed. Running with admin rights may work. Do you want to try with admin privilege?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                                        if (result == DialogResult.Yes)
                                        {
                                            Logger.Info("Clicked yes to create tunnel with admin rights.");
                                            isTunnelStarted = iOSAsyncMethods.GetInstance().CreateTunnel();
                                            Logger.Info("isTunnelStarted - " + isTunnelStarted);
                                            if (isTunnelStarted)
                                            {
                                                iOSMethods.isGo = false;
                                                iOSAsyncMethods.isGo = false;
                                            }
                                            else
                                            {
                                                commonProgress.Close();
                                                isScreenServerStarted = false;
                                                MessageBox.Show("Tunnel creation failed.\n\nIf Admin permission not given, Please provide admin permission when system prompts.\n\nIf Admin permission given, Please try again after restarting Appium Wizard/System.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            isScreenServerStarted = false;
                                            return;
                                        }
                                    }
                                }
                                commonProgress.UpdateStepLabel(title, "Mounting developer disk image. Please wait, this may take some time...", 40);
                                var output = iOSMethods.GetInstance().MountImage(udid);
                                if (deviceVersion >= version17Plus)
                                {
                                    if (output.Contains("tunnel not created"))
                                    {
                                        commonProgress.Close();
                                        isScreenServerStarted = false;
                                        MessageBox.Show("Tunnel creation failed, Unable to continue. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        return;
                                    }
                                }
                                if (output.Contains("Password Protected"))
                                {
                                    commonProgress.Close();
                                    isScreenServerStarted = false;
                                    MessageBox.Show("Please Unlock your " + deviceName + " and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                                if (output.Contains("Developer Mode is disabled") | output.Contains("Could not start service:com.apple.testmanagerd.lockdown.secure"))
                                {
                                    commonProgress.Close();
                                    isScreenServerStarted = false;
                                    MessageBox.Show("Please enable Developer Mode in your " + deviceName + " and try again.\nGo to Settings->Privacy & Security->Developer Mode->Turn ON.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                                commonProgress.UpdateStepLabel(title, "Starting WebDriverAgent... Please enter passcode in your iPhone, if it asks...\nOnce you see Automation Running, Go to home screen to reduce the retry.", 70);
                                WDAsessionId = iOSAsyncMethods.GetInstance().RunWebDriverAgent(commonProgress, udid, proxyPort).GetAwaiter().GetResult();
                                Logger.Info("WDAsessionId:"+ WDAsessionId);
                                if (WDAsessionId.Equals("Enable Developer Mode"))
                                {
                                    commonProgress.Close();
                                    isScreenServerStarted = false;
                                    MessageBox.Show("Please enable Developer Mode in your " + deviceName + " and try again.\nGo to Settings->Privacy & Security->Developer Mode->Turn ON.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                                else if (WDAsessionId.Equals("Password Protected"))
                                {
                                    commonProgress.Close();
                                    isScreenServerStarted = false;
                                    MessageBox.Show("Please Unlock your " + deviceName + " and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                                else if (WDAsessionId.Equals("WDA Not Installed"))
                                {
                                    commonProgress.Close();
                                    isScreenServerStarted = false;
                                    MessageBox.Show("WebDriverAgent Not Installed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                                else if (WDAsessionId.Equals("nosession") | WDAsessionId.Equals("Timed out") | WDAsessionId.Equals("nosession passcode required"))
                                {
                                    commonProgress.Close();
                                    isScreenServerStarted = false;
                                    MessageBox.Show("Unable to launch WebDriverAgent on your iPhone. Please enter passcode on your " + deviceName + " when it asks.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                                else if (string.IsNullOrEmpty(WDAsessionId) | WDAsessionId.Equals("unhandled"))
                                {
                                    commonProgress.Close();
                                    isScreenServerStarted = false;
                                    MessageBox.Show("Unhandled Exception - Try launching Webdriveragent manually and try opening the device.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                                else if (!WDAsessionId.Equals("nosession"))
                                {
                                    iOSAPIMethods.GoToHome(proxyPort);
                                    commonProgress.UpdateStepLabel(title, "Getting device screen size...", 80);
                                    var screenSize = iOSAPIMethods.GetScreenSize(proxyPort);
                                    width = screenSize.Item1;
                                    height = screenSize.Item2;
                                    commonProgress.UpdateStepLabel(title, "Starting device screen streaming...", 95);
                                    isScreenServerStarted = true;
                                    keyValuePairs.Add("proxyPort", proxyPort);
                                    keyValuePairs.Add("screenPort", screenServerPort);
                                    keyValuePairs.Add("sessionId", WDAsessionId);
                                    keyValuePairs.Add("width", width);
                                    keyValuePairs.Add("height", height);
                                    deviceDetails.Remove(udid);
                                    deviceDetails.Add(udid, keyValuePairs);
                                }
                                else
                                {
                                    iOSAsyncMethods.GetInstance().CloseTunnel();
                                    commonProgress.Close();
                                    isScreenServerStarted = false;
                                    MessageBox.Show("Unhandled Exception", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }
                        }
                        else
                        {
                            commonProgress.Close();
                            isScreenServerStarted = false;
                            MessageBox.Show("No pre-installed WebDriverAgent found, and no provisioning profile is available for the device " + deviceName + "(" + udid + ").\n\nOption 1 : Install WDA manually and configure to use pre-installed WDA by right clicking on the device in the list.\n\nOption 2 : Add a provisioning profile in Tools → iOS Profile Management so that AppiumWizard can install the WDA automatically.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
                catch (Exception e)
                {
                    commonProgress.Close();
                    isScreenServerStarted = false;
                    MessageBox.Show("Exception : " + e, "Failed to Start Screen Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (MainScreen.udidProxyPort.ContainsKey(udid))
                {
                    MainScreen.udidProxyPort[udid] = proxyPort;
                }
                else
                {
                    MainScreen.udidProxyPort.Add(udid, proxyPort);
                }
                if (MainScreen.udidScreenPort.ContainsKey(udid))
                {
                    MainScreen.udidScreenPort[udid] = screenServerPort;
                }
                else
                {
                    MainScreen.udidScreenPort.Add(udid, screenServerPort);
                }
            });
        }

        public static Dictionary<string, string> deviceSessionId = new Dictionary<string, string>();
        private async Task ExecuteAndroid()
        {
            await Task.Run(() =>
            {
                keyValuePairs = new Dictionary<object, object>();
                string sessionIdCreatedForScreenServer = string.Empty;
                string sessionIdAvailableForAutomation = string.Empty;
                try
                {
                    commonProgress.UpdateStepLabel(title, "Checking UIAutomator installation...", 10);
                    AndroidMethods.GetInstance().UninstallOtherInstrumentationApps(udid);
                    commonProgress.UpdateStepLabel(title, "Checking UIAutomator installation...", 15);
                    bool isUIAutomatorInstalled = AndroidMethods.GetInstance().isUIAutomatorInstalled(udid, true, 10000);
                    Logger.Info("isUIAutomatorInstalled : "+ isUIAutomatorInstalled);
                    if (!isUIAutomatorInstalled)
                    {
                        commonProgress.UpdateStepLabel(title, "Installing UIAutomator...", 30);
                        AndroidMethods.GetInstance().InstallUIAutomator(udid);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Error installing uiautomator");
                    MessageBox.Show(e.Message, "Error installing UIAutomator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    isScreenServerStarted = false;
                    return;
                }
                try
                {
                    int forwardedProxyPort = AndroidMethods.GetInstance().GetForwardedPort(udid, 6790);
                    if (forwardedProxyPort != -1)
                    {
                        proxyPort = forwardedProxyPort;
                    }
                    else
                    {
                        commonProgress.UpdateStepLabel(title, "Setting up Proxy Server...", 50);
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
                        commonProgress.UpdateStepLabel(title, "Setting up Proxy Server...", 60);
                        screenServerPort = Common.GetFreePort();
                        AndroidMethods.GetInstance().StartAndroidProxyServer(screenServerPort, 7810, udid);
                    }
                    commonProgress.UpdateStepLabel(title, "Checking UIAutomator running status...", 70);
                    bool IsUIAutomatorRunning = AndroidMethods.GetInstance().IsUIAutomatorRunning(udid);
                    if (!IsUIAutomatorRunning)
                    {
                        commonProgress.UpdateStepLabel(title, "Starting UIAutomator...", 75);
                        AndroidAsyncMethods.GetInstance().StartUIAutomatorServer(udid);
                        sessionIdCreatedForScreenServer = AndroidAPIMethods.CreateSession(proxyPort);
                    }
                    else
                    {
                        commonProgress.UpdateStepLabel(title, "Checking for existing session...", 75);
                        bool isSessionCreated = false, isItValidSession = false;
                        if (deviceSessionId.ContainsKey(udid))
                        {
                            Logger.Info("deviceSessionId.ContainsKey(udid) : "+udid);
                            AndroidAPIMethods.DeleteSession(proxyPort);
                        }
                        else
                        {
                            Logger.Info("deviceSessionId does not ContainsKey(udid) : " + udid);
                            sessionIdAvailableForAutomation = AndroidAPIMethods.GetSessionID(proxyPort);
                            isSessionCreated = !sessionIdAvailableForAutomation.Equals("nosession");
                        }
                        if (isSessionCreated)
                        {
                            string androidId = AppiumServerSetup.GetAndroidId(proxyPort, sessionIdAvailableForAutomation);
                            isItValidSession = AppiumServerSetup.isExpectedDataAvailableInSessionDetails(androidId);
                            if (isItValidSession == false)
                            {
                                isSessionCreated = false;
                            }
                        }
                        if (!isSessionCreated & !isItValidSession)
                        {
                            commonProgress.UpdateStepLabel(title, "Restarting UIAutomator...", 80);
                            AndroidMethods.GetInstance().StopUIAutomator(udid);
                            AndroidAsyncMethods.GetInstance().StartUIAutomatorServer(udid);
                            sessionIdCreatedForScreenServer = AndroidAPIMethods.CreateSession(proxyPort);
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
                    if (isScreenServerStarted)
                    {
                        commonProgress.UpdateStepLabel(title, "Starting device screen streaming...", 95);
                        if (!deviceDetails.ContainsKey(udid))
                        {
                            keyValuePairs.Add("proxyPort", proxyPort);
                            keyValuePairs.Add("screenPort", screenServerPort);
                            deviceDetails.Add(udid, keyValuePairs);
                        }
                    }
                }
                catch (Exception e)
                {
                    isScreenServerStarted = false;
                    MessageBox.Show(e.Message, "Failed starting screen server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
        }


        private async Task ExecuteBackgroundMethod2()
        {
            ScreenControl screenForm;
            if (OSType.Equals("Android"))
            {
                screenForm = new ScreenControl(OSType, OSVersion, udid, width, height, UIAutomatorSessionId, deviceName, proxyPort, screenServerPort);
            }
            else
            {
                screenForm = new ScreenControl(OSType, OSVersion, udid, width, height, WDAsessionId, deviceName, proxyPort, screenServerPort);
            }
            screenForm.Name = udid;
            screenForm.Show();
        }
    }
}
