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
        }

        private void FixAppiumButton_Click(object sender, EventArgs e)
        {
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Install Appium", "Installing Appium Server, Please wait...");
            Common.InstallAppiumGlobally();
            commonProgress.Close();
            FindIssues();
        }

        private void FixXCUITestButton_Click(object sender, EventArgs e)
        {
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Install XCUITest", "Installing XCUITest(iOS) driver, Please wait...");
            Common.InstallXCUITestDriver();
            commonProgress.Close();
            FindIssues();
        }

        private void FixUIAutomatorButton_Click(object sender, EventArgs e)
        {
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Install UIAutomator2", "Installing UIAutomator2(android) driver, Please wait...");
            Common.InstallUIAutomatorDriver();
            commonProgress.Close();
            FindIssues();
        }

        private void checkForIssues_Click(object sender, EventArgs e)
        {
            FindIssues();
        }
    }
}
