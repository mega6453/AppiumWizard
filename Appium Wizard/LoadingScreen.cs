using Microsoft.VisualBasic;
using System;
using System.Net;
using System.Reflection;
using System.Windows.Forms;

namespace Appium_Wizard
{
    public partial class LoadingScreen : Form
    {
        AppiumServerSetup serverSetup = new AppiumServerSetup();
        public static int WDAproxyPort, UiAutomatorPort;
        public int appiumPort = 4723;
        public LoadingScreen()
        {
            string clientId = Database.QueryDataFromGUIDTable();
            if (clientId.Equals("Empty"))
            {
                Guid guid = Guid.NewGuid();
                GoogleAnalytics.clientId = guid.ToString();
                Database.UpdateDataIntoGUIDTable(guid.ToString());
                GoogleAnalytics.SendEvent(GoogleAnalytics.screenName.App_Launched, "First Launch", true);
            }
            else
            {
                GoogleAnalytics.clientId = clientId;
                GoogleAnalytics.SendEvent(GoogleAnalytics.screenName.App_Launched, "Not First Launch");
            }
            InitializeComponent();
            try
            {
                Dictionary<string, string> readPortData = Database.QueryDataFromPortNumberTable();
                if (readPortData.TryGetValue("PortNumber1", out string portValue))
                {
                    if (!string.IsNullOrEmpty(portValue))
                    {
                        appiumPort = int.Parse(portValue);
                    }
                    else
                    {
                        Database.UpdateDataIntoPortNumberTable("PortNumber1", appiumPort);
                    }
                }
                else
                {
                    Database.UpdateDataIntoPortNumberTable("PortNumber1", appiumPort);
                }
            }
            catch (Exception)
            {
            }
        }
        private void LoadingScreen_Load(object sender, EventArgs e)
        {
            StartBackgroundTasks();
        }

        private void StartBackgroundTasks()
        {
            var startTime = DateTime.Now;
            Show();
            Common.SetAndroidHomeEnvironmentVariable();
            productVersion.Text = "Version " + VersionInfo.VersionNumber;
            productVersion.Refresh();
            bool isFirstTimeRun = Database.QueryDataFromFirstTimeRunTable().Contains("Yes");
            if (isFirstTimeRun)
            {
                firstTimeRunLabel.Text = "First time run verifies the installation, It may take sometime, Please wait...";
                firstTimeRunLabel.Refresh();
                bool isNodeInstalled = Common.IsNodeInstalled();
                if (!isNodeInstalled)
                {
                    UpdateStepLabel("Installing NodeJS, Please provide required permission when system prompts...");
                    Common.InstallNodeJs();
                    GoogleAnalytics.SendEvent(GoogleAnalytics.screenName.Loading_Screen, "NodeJS not installed");
                }
                bool iSAppiumInstalled = Common.IsAppiumInstalled();
                if (!iSAppiumInstalled)
                {
                    UpdateStepLabel("Installing Appium Server, Please wait...");
                    Common.InstallAppiumGlobally();
                    GoogleAnalytics.SendEvent(GoogleAnalytics.screenName.Loading_Screen, "Appium not installed");
                }
                string InstalledDriverList = Common.AppiumInstalledDriverList();
                bool IsXCUITestDriverInstalled = InstalledDriverList.Contains("xcuitest@");
                bool IsUIAutomatorDriverInstalled = InstalledDriverList.Contains("uiautomator2@");
                if (!IsXCUITestDriverInstalled)
                {
                    UpdateStepLabel("Installing XCUITest driver, Please wait...");
                    Common.InstallXCUITestDriver();
                    GoogleAnalytics.SendEvent(GoogleAnalytics.screenName.Loading_Screen, "XCUITest not installed");
                }
                if (!IsUIAutomatorDriverInstalled)
                {
                    UpdateStepLabel("Installing UIAutomator2 driver, Please wait...");
                    Common.InstallUIAutomatorDriver();
                    GoogleAnalytics.SendEvent(GoogleAnalytics.screenName.Loading_Screen, "UIAutomator2 not installed");
                }
                firstTimeRunLabel.Text = "";
            }
            Database.UpdateDataIntoFirstTimeRunTable("No");
            UpdateStepLabel("Starting Appium Server...");
            ExecuteBackgroundMethod();
            UpdateStepLabel("Loading Modules...");
            //int adbPort = Common.GetFreePort();
            int adbPort = 5037;
            AndroidMethods.GetInstance().StartAdbServer(adbPort);
            UpdateStepLabel("Initializing User Interface...");
            var endTime = DateTime.Now;
            string timeTaken = Common.GetDuration(startTime, endTime);
            string info = "Completed - Time Taken : " + timeTaken;
            GoogleAnalytics.SendEvent(GoogleAnalytics.screenName.Loading_Screen, info);
            MainScreen mainForm = new MainScreen();
            Hide();
            mainForm.ShowDialog();
        }

        private Task ExecuteBackgroundMethod()
        {
            WDAproxyPort = Common.GetFreePort();
            UiAutomatorPort = Common.GetFreePort(8200, 8220);
            //AndroidMethods.GetInstance().StartAndroidProxyServer(UiAutomatorPort, 6790);
            int screenport = Common.GetFreePort();
            Task.Run(() =>
            {
                serverSetup.StartAppiumServer(appiumPort, WDAproxyPort, 1, screenport);
                MainScreen.runningProcessesPortNumbers.Add(appiumPort);
                MainScreen.runningProcessesPortNumbers.Add(WDAproxyPort);
                DialogResult result = DialogResult.None;
                if (Common.IsNodeInstalled())
                {
                    while (!serverSetup.serverStarted)
                    {
                        if (!string.IsNullOrEmpty(serverSetup.statusText))
                        {
                            // UpdateStepLabel(AppiumServerSetup.statusText);
                            if (serverSetup.statusText.Contains("address already in use"))
                            {
                                result = MessageBox.Show("Port " + appiumPort + " is being used by " + Common.RunNetstatAndFindProcessByPort(appiumPort).Item2 + ".Please try to configure in different port here, File-> Server config.", "Error on Starting Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                            }
                        }
                    }
                    if (serverSetup.serverStarted)
                    {
                        statusLabel.Text = "Running";
                    }

                }
            });
            return Task.Delay(500);
        }

        public void UpdateStepLabel(string stepText)
        {
            statusLabel.Text = stepText;
            statusLabel.Refresh();
        }

        private void LoadingScreen_Shown(object sender, EventArgs e)
        {
            GoogleAnalytics.SendEvent(MethodBase.GetCurrentMethod().Name);
        }
    }
}
