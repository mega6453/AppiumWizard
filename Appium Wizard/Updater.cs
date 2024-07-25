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

        public async Task GetVersionInformation(MainScreen mainScreen = null)
        {
            string InstalledAppiumServerVersion = string.Empty;
            string AvailableAppiumVersion = string.Empty;
            string AvailableXCUITestVersion = string.Empty;
            string AvailableUIAutomatorVersion = string.Empty;
            CommonProgress commonProgress = new CommonProgress();
            if (mainScreen == null)
            {
                commonProgress.Owner = this;
            }
            else
            {
                commonProgress.Owner = mainScreen;
            }
            commonProgress.Show();
            await Task.Run(() =>
            {
                try
                {
                    commonProgress.UpdateStepLabel("Check for Server updates", "Please wait while getting installed Appium server version information. This may take sometime...", 10);
                    InstalledAppiumServerVersion = Common.InstalledAppiumServerVersion();
                    commonProgress.UpdateStepLabel("Check for Server updates", "Please wait while getting installed Driver version information. This may take sometime...", 30);
                    driverVersion = Common.InstalledDriverVersion();
                    commonProgress.UpdateStepLabel("Check for Server updates", "Please wait while checking for Appium Server updates. This may take sometime...", 50);
                    AvailableAppiumVersion = Common.AvailableAppiumVersion();
                    commonProgress.UpdateStepLabel("Check for Server updates", "Please wait while checking for XCUITest driver updates. This may take sometime...", 70);
                    AvailableXCUITestVersion = Common.AvailableXCUITestVersion();
                    commonProgress.UpdateStepLabel("Check for Server updates", "Please wait while checking for UIAutomator driver updates. This may take sometime...", 90);
                    AvailableUIAutomatorVersion = Common.AvailableUIAutomatorVersion();
                    commonProgress.UpdateStepLabel("Check for Server updates", "Please wait while updating the information. This may take sometime...", 95);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });

            appiumCurrentVersionLabel.Text = InstalledAppiumServerVersion;
            XCUITestCurrentVersionLabel.Text = driverVersion["xcuitest"];
            UIAutomatorCurrentVersionLabel.Text = driverVersion["uiautomator2"];
            AppiumAvailableVersionLabel.Text = AvailableAppiumVersion;
            XCUITestAvailableVersionLabel.Text = AvailableXCUITestVersion;
            UIAutomatorAvailableVersionLabel.Text = AvailableUIAutomatorVersion;

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
            commonProgress.Close();
        }

        private async void AppiumButton_Click(object sender, EventArgs e)
        {
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Owner = this;
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Update Appium Server", "Please wait while updating appium server version to " + AppiumAvailableVersionLabel.Text);
            await Task.Run(() =>
            {
                Common.InstallAppiumGlobally();
                GoogleAnalytics.SendEvent("Update_Appium");
                isUpdated = true;
            });
            commonProgress.Close();
            await GetVersionInformation();
        }

        private async void UIAutomatorButton_Click(object sender, EventArgs e)
        {
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Owner = this;
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Update UIAutomator driver", "Please wait while updating UIAutomator driver version to " + UIAutomatorAvailableVersionLabel.Text);
            await Task.Run(() =>
            {
                Common.UpdateUIAutomatorDriver();
                GoogleAnalytics.SendEvent("Update_UIAutomator");
                isUpdated = true;
            });
            commonProgress.Close();
            await GetVersionInformation();
        }

        private async void XCUITestButton_Click(object sender, EventArgs e)
        {
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Owner = this;
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Update XCUITest driver", "Please wait while updating XCUITest driver version to " + XCUITestAvailableVersionLabel.Text);
            await Task.Run(() =>
            {
                Common.UpdateXCUITestDriver();
                GoogleAnalytics.SendEvent("Update_XCUITest");
                isUpdated = true;
            });
            commonProgress.Close();
            await GetVersionInformation();
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
