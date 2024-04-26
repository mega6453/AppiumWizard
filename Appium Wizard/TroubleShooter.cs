using System.Reflection;

namespace Appium_Wizard
{
    public partial class TroubleShooter : Form
    {
        public TroubleShooter()
        {
            InitializeComponent();
        }

        private void TroubleShooter_Load(object sender, EventArgs e)
        {
            FindIssues();
        }

        private void FindIssues()
        {
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Find Issues", "Please wait while looking for the issues...");
            bool IsNodeInstalled = Common.IsNodeInstalled();
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
                bool IsAppiumInstalled = Common.IsAppiumInstalled();
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
                    string InstalledDriverList = Common.AppiumInstalledDriverList();
                    bool IsXCUITestDriverInstalled = InstalledDriverList.Contains("xcuitest@");
                    bool IsUIAutomatorDriverInstalled = InstalledDriverList.Contains("uiautomator2@");
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


        private void FixNodeJSButton_Click(object sender, EventArgs e)
        {
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Install NodeJS", "Installing NodeJS, Please provide required permission when system prompts...");
            Common.InstallNodeJs();
            commonProgress.Close();
            FindIssues();
            GoogleAnalytics.SendEvent(MethodBase.GetCurrentMethod().Name);
        }

        private void FixAppiumButton_Click(object sender, EventArgs e)
        {
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Install Appium", "Installing Appium Server, Please wait...");
            Common.InstallAppiumGlobally();
            commonProgress.Close();
            FindIssues();
            GoogleAnalytics.SendEvent(MethodBase.GetCurrentMethod().Name);
        }

        private void FixXCUITestButton_Click(object sender, EventArgs e)
        {
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Install XCUITest", "Installing XCUITest(iOS) driver, Please wait...");
            Common.InstallXCUITestDriver();
            commonProgress.Close();
            FindIssues();
            MessageBox.Show("Restart the Appium server to apply the changes. Server -> Configuration -> Stop and Start", "Restart Appium Server", MessageBoxButtons.OK, MessageBoxIcon.Information);
            GoogleAnalytics.SendEvent(MethodBase.GetCurrentMethod().Name);
        }

        private void FixUIAutomatorButton_Click(object sender, EventArgs e)
        {
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Install UIAutomator2", "Installing UIAutomator2(android) driver, Please wait...");
            Common.InstallUIAutomatorDriver();
            commonProgress.Close();
            FindIssues();
            MessageBox.Show("Restart the Appium server to apply the changes. Server -> Configuration -> Stop and Start", "Restart Appium Server",MessageBoxButtons.OK,MessageBoxIcon.Information);
            GoogleAnalytics.SendEvent(MethodBase.GetCurrentMethod().Name);
        }

        private void checkForIssues_Click(object sender, EventArgs e)
        {
            FindIssues();
            GoogleAnalytics.SendEvent(MethodBase.GetCurrentMethod().Name);
        }

        private void TroubleShooter_Shown(object sender, EventArgs e)
        {
            GoogleAnalytics.SendEvent(MethodBase.GetCurrentMethod().Name);
        }
    }
}
