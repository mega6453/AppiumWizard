using NLog;
using NLog.Fluent;
using System.Diagnostics;
namespace Appium_Wizard
{
    public partial class LoadingScreen : Form
    {
        AppiumServerSetup serverSetup = new AppiumServerSetup();
        public static int UiAutomatorPort;
        public static int appiumPort = 4723;
        public static bool isServerStarted;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public LoadingScreen()
        {
            InitializeComponent();
            statusLabel.Text = "Initializing...";
            Logger.Debug("Initializing...");
            try
            {
                Dictionary<string, string> readPortData = Database.QueryDataFromPortNumberTable();
                if (readPortData.TryGetValue("PortNumber1", out string portValue))
                {
                    if (!string.IsNullOrEmpty(portValue))
                    {
                        Logger.Debug("Using port from db to start server 1, port number : "+portValue);
                        appiumPort = int.Parse(portValue);
                    }
                    else
                    {
                        Logger.Debug("Server number 1 port is empty, using default port 4723");
                        Database.UpdateDataIntoPortNumberTable("PortNumber1", appiumPort);
                    }
                }
                else
                {
                    Logger.Debug("Server number 1 port is empty, using default port 4723");
                    Database.UpdateDataIntoPortNumberTable("PortNumber1", appiumPort);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Exception in getting port number");
            }
            string clientId = Database.QueryDataFromGUIDTable();
            if (clientId.Equals("Empty"))
            {
                Guid guid = Guid.NewGuid();
                GoogleAnalytics.clientId = guid.ToString();
                Database.UpdateDataIntoGUIDTable(guid.ToString());
                GoogleAnalytics.SendEvent(GoogleAnalytics.screenName.App_Launched, "First Launch", true);
                Logger.Debug("First Launch");
            }
            else
            {
                GoogleAnalytics.clientId = clientId;
                GoogleAnalytics.SendEvent(GoogleAnalytics.screenName.App_Launched, "Not First Launch");
                Logger.Debug("Not First Launch");
            }
        }
        private async void LoadingScreen_Load(object sender, EventArgs e)
        {
            await StartBackgroundTasks();
        }

        private async Task StartBackgroundTasks()
        {
            bool isFirstTimeRun = false;
            var startTime = DateTime.Now;
            Show();
            await Task.Run(() =>
            {
                Common.SetAndroidHomeEnvironmentVariable();
                Logger.Debug("SetAndroidHomeEnvironmentVariable");
            });
            productVersion.Text = "Version " + VersionInfo.VersionNumber;
            Logger.Info(productVersion.Text);
            productVersion.Refresh();
#if DEBUG
            Console.WriteLine("Running in Debug mode.");
            isFirstTimeRun = false;
#else
            Console.WriteLine("Running in Release mode.");
            isFirstTimeRun = Database.QueryDataFromFirstTimeRunTable().Contains("Yes");

#endif
            if (isFirstTimeRun)
            {
                firstTimeRunLabel.Text = "First time run verifies the installation, This may take sometime, Please wait...";
                firstTimeRunLabel.Refresh();
                await Task.Run(() =>
                {
                    UpdateStepLabel("Checking for Node Installation, Please wait...");
                    bool isNodeInstalled = Common.IsNodeInstalled();
                    if (!isNodeInstalled)
                    {
                        UpdateStepLabel("Installing NodeJS, This may take sometime, Please wait...");
                        Common.InstallNodeJs();
                        GoogleAnalytics.SendEvent(GoogleAnalytics.screenName.Loading_Screen, "NodeJS not installed");
                    }
                    UpdateStepLabel("Checking for Appium Installation, Please wait...");
                    bool iSAppiumInstalled = Common.IsAppiumInstalled();
                    if (!iSAppiumInstalled)
                    {
                        UpdateStepLabel("Installing Appium Server, This may take sometime, Please wait...");
                        Common.InstallAppiumGlobally();
                        GoogleAnalytics.SendEvent(GoogleAnalytics.screenName.Loading_Screen, "Appium not installed");
                    }
                    UpdateStepLabel("Checking for XCUITest, UIAutomator2 driver Installation, Please wait...");
                    string InstalledDriverList = Common.AppiumInstalledDriverList();
                    bool IsXCUITestDriverInstalled = InstalledDriverList.Contains("xcuitest@");
                    bool IsUIAutomatorDriverInstalled = InstalledDriverList.Contains("uiautomator2@");
                    if (!IsXCUITestDriverInstalled)
                    {
                        UpdateStepLabel("Installing XCUITest driver, This may take sometime, Please wait...");
                        Common.InstallXCUITestDriver();
                        GoogleAnalytics.SendEvent(GoogleAnalytics.screenName.Loading_Screen, "XCUITest not installed");
                    }
                    if (!IsUIAutomatorDriverInstalled)
                    {
                        UpdateStepLabel("Installing UIAutomator2 driver, This may take sometime, Please wait...");
                        Common.InstallUIAutomatorDriver();
                        GoogleAnalytics.SendEvent(GoogleAnalytics.screenName.Loading_Screen, "UIAutomator2 not installed");
                    }
                    UpdateStepLabel("Getting compatible WebDriverAgentRunner, This may take sometime, Please wait...");
                    Common.GetWebDriverAgentIPAFile();
                });
                firstTimeRunLabel.Text = "";
            }
            UpdateStepLabel("Starting Appium Server...");
            Database.UpdateDataIntoFirstTimeRunTable("No");
            await ExecuteBackgroundMethod();
            UpdateStepLabel("Scanning for detached Appium Wizard processes...");
            Common.ScanForDetachedProcesses();
            UpdateStepLabel("Loading Modules...");
            int adbPort = 5037;
            AndroidMethods.GetInstance().StartAdbServer(adbPort);
            Common.DeleteLogFiles();
            UpdateStepLabel("Initializing User Interface...");
            var endTime = DateTime.Now;
            string timeTaken = Common.GetDuration(startTime, endTime);
            string info = "Completed - Time Taken : " + timeTaken;
            GoogleAnalytics.SendEvent(GoogleAnalytics.screenName.Loading_Screen, info);
            MainScreen mainForm = new MainScreen();
            Hide();
            try
            {
                mainForm.ShowDialog();
            }
            catch (Exception ex)
            {
                Logger.Error(ex,"App crashed");
                GoogleAnalytics.SendEvent("App_Crashed", ex.Message);
                Close();
            }
        }

        private async Task ExecuteBackgroundMethod()
        {
            bool isAppiumPortFree = !Common.IsPortBeingUsed(appiumPort);
            if (isAppiumPortFree)
            {
                _ = StartServer();
                isServerStarted = true;
            }
            else
            {
                isServerStarted = false;
            }
        }

        private async Task StartServer()
        {
            await Task.Run(() =>
            {
                string serverCommand = Database.QueryDataFromServerFinalCommandTable()["Server1"];
                if (!serverCommand.Contains("--port"))
                {
                    serverCommand = "appium --port " + appiumPort + " --allow-cors --allow-insecure=adb_shell";
                    Database.UpdateDataIntoServerFinalCommandTable("Server1",serverCommand);
                }
                serverSetup.StartAppiumServer(appiumPort, 1, serverCommand);
                if (Common.IsNodeInstalled())
                {
                    while (!serverSetup.serverStarted)
                    {
                        if (!string.IsNullOrEmpty(serverSetup.statusText))
                        {
                            if (serverSetup.statusText.Contains("address already in use"))
                            {
                                Logger.Debug("address already in use");
                                MessageBox.Show("Go to Server -> Config -> Set a port number -> Start the Appium Server.", "Error on Starting Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                            }
                        }
                    }
                    if (serverSetup.serverStarted)
                    {
                        isServerStarted = true;
                    }
                }
            });
        }

        public void UpdateStepLabel(string stepText)
        {
            Logger.Debug(stepText);
            if (statusLabel.InvokeRequired)
            {
                statusLabel.Invoke(new Action<string>(UpdateStepLabel), stepText);
            }
            else
            {
                statusLabel.Text = stepText;
                statusLabel.Refresh();
            }
        }

        private void LoadingScreen_Shown(object sender, EventArgs e)
        {
            Logger.Debug("LoadingScreen_Shown");
            GoogleAnalytics.SendEvent("LoadingScreen_Shown");
        }
    }
}
