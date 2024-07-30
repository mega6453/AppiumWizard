using System.Reflection;

namespace Appium_Wizard
{
    public partial class TroubleShooter : Form
    {
        public TroubleShooter()
        {
            InitializeComponent();
        }
        public async Task FindIssues(MainScreen mainScreen = null)
        {
            bool IsNodeInstalled = false;
            bool IsAppiumInstalled = false;
            bool IsXCUITestDriverInstalled = false;
            bool IsUIAutomatorDriverInstalled = false;
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
                commonProgress.UpdateStepLabel("Find Issues", "Please wait while checking for NodeJS installation...", 10);
                IsNodeInstalled = Common.IsNodeInstalled();
                commonProgress.UpdateStepLabel("Find Issues", "Please wait while checking for Appium installation...", 50);
                IsAppiumInstalled = Common.IsAppiumInstalled();
                commonProgress.UpdateStepLabel("Find Issues", "Please wait while checking for drivers installation...", 75);
                string InstalledDriverList = Common.AppiumInstalledDriverList();
                IsXCUITestDriverInstalled = InstalledDriverList.Contains("xcuitest@");
                IsUIAutomatorDriverInstalled = InstalledDriverList.Contains("uiautomator2@");
            });
            if (!IsNodeInstalled)
            {
                NodeJSStatusLabel.Text = "Not OK";
                AppiumStatusLabel.Text = "NodeJS Required";
                XCUITestStatusLabel.Text = "Appium Required";
                UIAutomatorStatusLabel.Text = "Appium Required";
                FixNodeJSButton.Enabled = true;
                GoogleAnalytics.SendEvent("Trouble_NodeJS_Not_Installed");
            }
            else
            {
                NodeJSStatusLabel.Text = "OK";
                FixNodeJSButton.Enabled = false;
                if (!IsAppiumInstalled)
                {
                    AppiumStatusLabel.Text = "Not OK";
                    XCUITestStatusLabel.Text = "Appium Required";
                    UIAutomatorStatusLabel.Text = "Appium Required";
                    FixAppiumButton.Enabled = true;
                    GoogleAnalytics.SendEvent("Trouble_Appium_Not_Installed");
                }
                else
                {
                    AppiumStatusLabel.Text = "OK";
                    FixAppiumButton.Enabled = false;
                    if (!IsXCUITestDriverInstalled)
                    {
                        XCUITestStatusLabel.Text = "Not OK";
                        FixXCUITestButton.Enabled = true;
                        GoogleAnalytics.SendEvent("Trouble_XCUITest_Not_Installed");
                    }
                    else
                    {
                        XCUITestStatusLabel.Text = "OK";
                        FixXCUITestButton.Enabled = false;
                    }
                    if (!IsUIAutomatorDriverInstalled)
                    {
                        UIAutomatorStatusLabel.Text = "Not OK";
                        FixUIAutomatorButton.Enabled = true;
                        GoogleAnalytics.SendEvent("Trouble_UIAutomator_Not_Installed");
                    }
                    else
                    {
                        UIAutomatorStatusLabel.Text = "OK";
                        FixUIAutomatorButton.Enabled = false;
                    }
                }
            }
            commonProgress.Close();        
        }

        private async void FixNodeJSButton_Click(object sender, EventArgs e)
        {
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Install NodeJS","Installing NodeJS, This may take sometime, Please wait...");
            await Task.Run(() =>
            {
                Common.InstallNodeJs();
            });
            commonProgress.Close();
            await FindIssues();
            GoogleAnalytics.SendEvent(MethodBase.GetCurrentMethod().Name);
        }

        private async void FixAppiumButton_Click(object sender, EventArgs e)
        {
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Install Appium", "Installing Appium Server, This may take sometime, Please wait...");
            await Task.Run(() =>
            {
                Common.InstallAppiumGlobally();
            });
            commonProgress.Close();
            await FindIssues();
            GoogleAnalytics.SendEvent(MethodBase.GetCurrentMethod().Name);
        }

        private async void FixXCUITestButton_Click(object sender, EventArgs e)
        {
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Install XCUITest", "Installing XCUITest(iOS) driver, This may take sometime, Please wait...");
            await Task.Run(() =>
            {
                Common.InstallXCUITestDriver();
            });
            commonProgress.Close();
            await FindIssues();
            MessageBox.Show("Restart the Appium server to apply the changes. Server -> Configuration -> Stop and Start", "Restart Appium Server", MessageBoxButtons.OK, MessageBoxIcon.Information);
            GoogleAnalytics.SendEvent(MethodBase.GetCurrentMethod().Name);
        }

        private async void FixUIAutomatorButton_Click(object sender, EventArgs e)
        {
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Install UIAutomator2", "Installing UIAutomator2(android) driver, This may take sometime, Please wait...");
            await Task.Run(() =>
            {
                Common.InstallUIAutomatorDriver();
            });
            commonProgress.Close();
            await FindIssues();
            MessageBox.Show("Restart the Appium server to apply the changes. Server -> Configuration -> Stop and Start", "Restart Appium Server", MessageBoxButtons.OK, MessageBoxIcon.Information);
            GoogleAnalytics.SendEvent(MethodBase.GetCurrentMethod().Name);
        }

        private async void checkForIssues_Click(object sender, EventArgs e)
        {
            await FindIssues();
            GoogleAnalytics.SendEvent(MethodBase.GetCurrentMethod().Name);
        }

        private void TroubleShooter_Shown(object sender, EventArgs e)
        {
            GoogleAnalytics.SendEvent(MethodBase.GetCurrentMethod().Name);
        }
    }
}
