using Newtonsoft.Json;

namespace Appium_Wizard
{
    public partial class LoadingScreen : Form
    {
        AppiumServerSetup serverSetup = new AppiumServerSetup();
        public static int WDAproxyPort, UiAutomatorPort;
        public int appiumPort = 4723;
        public LoadingScreen()
        {
            bool isRestartRequired = Common.WSLHelp().Contains("1603");
            if (isRestartRequired)
            {
                MessageBox.Show("System Restart is required to complete the installation. Please Restart the system and Launch Appium Wizard again.", "Restart Required", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
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
            Show();
            productVersion.Text = "Version " + ProductVersion;
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
                }
                bool iSAppiumInstalled = Common.IsAppiumInstalled();
                if (!iSAppiumInstalled)
                {
                    UpdateStepLabel("Installing Appium Server, Please wait...");
                    Common.InstallAppiumGlobally();
                }
                string InstalledDriverList = Common.AppiumInstalledDriverList();
                bool IsXCUITestDriverInstalled = InstalledDriverList.Contains("xcuitest@");
                bool IsUIAutomatorDriverInstalled = InstalledDriverList.Contains("uiautomator2@");
                if (!IsXCUITestDriverInstalled)
                {
                    UpdateStepLabel("Installing XCUITest driver, Please wait...");
                    Common.InstallXCUITestDriver();
                }
                if (!IsUIAutomatorDriverInstalled)
                {
                    UpdateStepLabel("Installing UIAutomator2 driver, Please wait...");
                    Common.InstallUIAutomatorDriver();
                }
                UpdateStepLabel("Checking WSL status, Please wait...");
                bool isWSLEnabled = Common.IsWSLImportInPlaceSupported();
                if (!isWSLEnabled)
                {
                    UpdateStepLabel("Installing WSL(for iOS app signing), Please provide required permission when system prompts...");
                    Common.InstallWSL();
                    bool isRestartRequired = Common.WSLHelp().Contains("1603");
                    if (isRestartRequired)
                    {
                        MessageBox.Show("System Restart is required to complete the installation. Please Restart the system and  Launch Appium Wizard again.", "Restart Required", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(0);
                    }
                }
                else
                {
                    string wslList = Common.WSLList();
                    if (!wslList.Contains("AppiumWizard"))
                    {
                        Common.RegisterWSLDistro();
                    }
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
            MainScreen mainForm = new MainScreen();
            Hide();
            mainForm.ShowDialog();
            Close();
        }

        private Task ExecuteBackgroundMethod()
        {
            WDAproxyPort = Common.GetFreePort();
            UiAutomatorPort = Common.GetFreePort(8200, 8220);
            //AndroidMethods.GetInstance().StartAndroidProxyServer(UiAutomatorPort, 6790);

            Task.Run(() =>
            {
                serverSetup.StartAppiumServer(appiumPort, WDAproxyPort, 1);
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

    }
}
