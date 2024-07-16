namespace Appium_Wizard
{
    public partial class Updater : Form
    {
        Dictionary<string, string> driverVersion = new Dictionary<string, string>();
        bool isUpdated = false;
        public Updater()
        {
            InitializeComponent();
        }

        public async Task GetVersionInformation()
        {
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Check for Server updates", "Please wait while getting installed server version information. This may take sometime...", 0);
            await Task.Run(() =>
            {
                try
                {
                    driverVersion = Common.InstalledDriverVersion();
                    commonProgress.UpdateStepLabel("Check for Server updates", "Please wait while getting installed server version information. This may take sometime...", 10);
                    appiumCurrentVersionLabel.Text = Common.InstalledAppiumServerVersion();
                    commonProgress.UpdateStepLabel("Check for Server updates", "Please wait while getting installed server version information. This may take sometime...", 30);
                    XCUITestCurrentVersionLabel.Text = driverVersion["xcuitest"];
                    UIAutomatorCurrentVersionLabel.Text = driverVersion["uiautomator2"];
                    commonProgress.UpdateStepLabel("Check for Server updates", "Please wait while checking for server updates. This may take sometime...", 50);
                    AppiumAvailableVersionLabel.Text = Common.AavailableAppiumVersion();
                    commonProgress.UpdateStepLabel("Check for Server updates", "Please wait while checking for server updates. This may take sometime...", 70);
                    XCUITestAvailableVersionLabel.Text = Common.AavailableXCUITestVersion();
                    commonProgress.UpdateStepLabel("Check for Server updates", "Please wait while checking for server updates. This may take sometime...", 90);
                    UIAutomatorAvailableVersionLabel.Text = Common.AavailableUIAutomatorVersion();
                    commonProgress.UpdateStepLabel("Check for Server updates", "Please wait while checking for server updates. This may take sometime...", 95);

                    Version currentVersionAppium = new Version(appiumCurrentVersionLabel.Text);
                    Version latestVersionAppium = new Version(AppiumAvailableVersionLabel.Text);

                    bool isUpdateAvailableAppium = latestVersionAppium > currentVersionAppium;
                    if (isUpdateAvailableAppium)
                    {
                        AppiumButton.Enabled = true;
                    }
                    else
                    {
                        AppiumButton.Enabled = false;
                    }

                    //XCUITest
                    Version currentVersionXCUITest = new Version(XCUITestCurrentVersionLabel.Text);
                    Version latestVersionXCUITest = new Version(XCUITestAvailableVersionLabel.Text);

                    bool isUpdateAvailableXCUITest = latestVersionXCUITest > currentVersionXCUITest;
                    if (isUpdateAvailableXCUITest)
                    {
                        XCUITestButton.Enabled = true;
                    }
                    else
                    {
                        XCUITestButton.Enabled = false;
                    }

                    //UIAutomator2
                    Version currentVersionUI = new Version(UIAutomatorCurrentVersionLabel.Text);
                    Version latestVersionUI = new Version(UIAutomatorAvailableVersionLabel.Text);

                    bool isUpdateAvailableUI = latestVersionUI > currentVersionUI;
                    if (isUpdateAvailableUI)
                    {
                        UIAutomatorButton.Enabled = true;
                    }
                    else
                    {
                        UIAutomatorButton.Enabled = false;
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
            commonProgress.Close();
        }

        private async void AppiumButton_Click(object sender, EventArgs e)
        {
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Update Appium Server", "Please wait while updating appium server version to " + AppiumAvailableVersionLabel.Text);
            await Task.Run(() =>
            {
                Common.InstallAppiumGlobally();
                GoogleAnalytics.SendEvent("Update_Appium");
                isUpdated = true;
            });
            commonProgress.Close();
            GetVersionInformation();
        }

        private async void UIAutomatorButton_Click(object sender, EventArgs e)
        {
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Update UIAutomator driver", "Please wait while updating UIAutomator driver version to " + UIAutomatorAvailableVersionLabel.Text);
            await Task.Run(() =>
            {
                Common.UpdateUIAutomatorDriver();
                GoogleAnalytics.SendEvent("Update_UIAutomator");
                isUpdated = true;
            });
            commonProgress.Close();
            GetVersionInformation();
        }

        private async void XCUITestButton_Click(object sender, EventArgs e)
        {
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Update XCUITest driver", "Please wait while updating XCUITest driver version to " + XCUITestAvailableVersionLabel.Text);
            await Task.Run(() =>
            {
                Common.UpdateXCUITestDriver();
                GoogleAnalytics.SendEvent("Update_XCUITest");
                isUpdated = true;
            });
            commonProgress.Close();
            GetVersionInformation();
        }

        private void Updater_Shown(object sender, EventArgs e)
        {
            GoogleAnalytics.SendEvent("Updater_Shown");
        }

        private void Updater_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (isUpdated)
            {
                MessageBox.Show("Please restart the Appium Server to use the updated version. Go to Server > Config > Stop > Start.", "Server Updater", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
