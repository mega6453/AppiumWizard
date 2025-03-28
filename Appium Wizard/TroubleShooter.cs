using System;
using System.Reflection;

namespace Appium_Wizard
{
    public partial class TroubleShooter : Form
    {
        bool isDownloaded = false;
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
            bool IsCompatibleWDAAvailable = false;
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
                commonProgress.UpdateStepLabel("Find Issues", "Please wait while checking for WebDriverAgent compatibility...", 90);
                if (IsXCUITestDriverInstalled)
                {
                    string requiredVersion = Common.GetRequiredWebDriverAgentVersion();
                    if (requiredVersion != "versionNotFound" & requiredVersion != "fileNotFound")
                    {
                        string IPAVersion = iOSMethods.GetInstance().GetWDAIPAVersion();
                        Version expectedVersion = new Version(requiredVersion);
                        Version actualVersion = new Version(IPAVersion);
                        //bool areEqual = (expectedVersion.Major == actualVersion.Major) &&
                        //(expectedVersion.Minor == actualVersion.Minor);
                        //if (areEqual)
                        //{
                        //    IsCompatibleWDAAvailable = true;
                        //}
                        if (expectedVersion.Equals(actualVersion))
                        {
                            IsCompatibleWDAAvailable = true;
                        }
                        else if (isDownloaded)
                        {
                            IsCompatibleWDAAvailable = true;
                        }
                        else
                        {
                            IsCompatibleWDAAvailable = false;
                        }
                    }

                }
            });
            if (!IsNodeInstalled)
            {
                NodeJSStatusLabel.Text = "Not OK";
                AppiumStatusLabel.Text = "NodeJS Required";
                XCUITestStatusLabel.Text = "Appium Required";
                UIAutomatorStatusLabel.Text = "Appium Required";
                WDAStatusLabel.Text = "XCUITest Driver Required";
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
                    WDAStatusLabel.Text = "XCUITest Driver Required";
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
                        WDAStatusLabel.Text = "XCUITest Driver Required";
                        FixXCUITestButton.Enabled = true;
                        GoogleAnalytics.SendEvent("Trouble_XCUITest_Not_Installed");
                    }
                    else
                    {
                        XCUITestStatusLabel.Text = "OK";
                        FixXCUITestButton.Enabled = false;
                        FixWDAButton.Enabled = true;
                        if (IsCompatibleWDAAvailable)
                        {
                            WDAStatusLabel.Text = "OK";
                            FixWDAButton.Enabled = false;
                        }
                        else
                        {
                            WDAStatusLabel.Text = "Not OK";
                            FixWDAButton.Enabled = true;
                        }
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
            commonProgress.Owner = this;
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Install NodeJS", "Installing NodeJS, This may take sometime, Please wait...");
            await Task.Run(() =>
            {
                Common.InstallNodeJs(showProgresscheckBox.Checked);
            });
            commonProgress.Close();
            await FindIssues();
            GoogleAnalytics.SendEvent("FixNodeJSButton_Click");
        }

        private async void FixAppiumButton_Click(object sender, EventArgs e)
        {
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Owner = this;
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Install Appium", "Installing Appium Server, This may take sometime, Please wait...");
            await Task.Run(() =>
            {
                Common.InstallAppiumGlobally(showProgresscheckBox.Checked);
            });
            commonProgress.Close();
            await FindIssues();
            GoogleAnalytics.SendEvent("FixAppiumButton_Click");
        }

        private async void FixXCUITestButton_Click(object sender, EventArgs e)
        {
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Owner = this;
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Install XCUITest", "Installing XCUITest(iOS) driver, This may take sometime, Please wait...");
            await Task.Run(() =>
            {
                Common.InstallXCUITestDriver(showProgresscheckBox.Checked);
            });
            commonProgress.Close();
            await FindIssues();
            MessageBox.Show("Restart the Appium server to apply the changes. Server -> Configuration -> Stop and Start", "Restart Appium Server", MessageBoxButtons.OK, MessageBoxIcon.Information);
            GoogleAnalytics.SendEvent("FixXCUITestButton_Click");
        }

        private async void FixUIAutomatorButton_Click(object sender, EventArgs e)
        {
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Owner = this;
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Install UIAutomator2", "Installing UIAutomator2(android) driver, This may take sometime, Please wait...");
            await Task.Run(() =>
            {
                Common.InstallUIAutomatorDriver(showProgresscheckBox.Checked);
            });
            commonProgress.Close();
            await FindIssues();
            MessageBox.Show("Restart the Appium server to apply the changes. Server -> Configuration -> Stop and Start", "Restart Appium Server", MessageBoxButtons.OK, MessageBoxIcon.Information);
            GoogleAnalytics.SendEvent("FixUIAutomatorButton_Click");
        }

        private async void checkForIssues_Click(object sender, EventArgs e)
        {
            await FindIssues();
            GoogleAnalytics.SendEvent("checkForIssues_Click");
        }

        private void TroubleShooter_Shown(object sender, EventArgs e)
        {
            GoogleAnalytics.SendEvent("TroubleShooter_Shown");
        }

        private async void FixWDAButton_Click(object sender, EventArgs e)
        {
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Owner = this;
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Get WebDriverAgent", "Getting compatible WebDriverAgent based on installed XCUITest driver version, This may take sometime, Please wait...");
            await Task.Run(() =>
            {
                Common.GetWebDriverAgentIPAFile();
            });
            commonProgress.Close();
            isDownloaded = true;
            MessageBox.Show("Downloaded compatible version of WDA.\nDelete the already installed WDA from your iPhone and Open the device again to fix any issues.", "Re-Install WDA", MessageBoxButtons.OK, MessageBoxIcon.Information);
            GoogleAnalytics.SendEvent("FixWDAButton_Click");
            await FindIssues();
        }

        private async void reInstallAllButton_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("This action will uninstall appium server, drivers and then will install them again(requires internet connection). Are you sure you want to continue?", "Re-Install everything", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                CommonProgress commonProgress = new CommonProgress();
                commonProgress.Owner = this;
                commonProgress.Show();
                commonProgress.UpdateStepLabel("Uninstall XCUITest driver", "Uninstalling XCUITest driver, This may take sometime, Please wait...", 10);
                await Task.Run(() =>
                {
                    Common.UninstallXCUITestDriver(showProgresscheckBox.Checked);
                });
                commonProgress.UpdateStepLabel("Uninstall UIAutomator2 driver", "Uninstalling UIAutomator2 driver, This may take sometime, Please wait...", 20);
                await Task.Run(() =>
                {
                    Common.UninstallUIAutomatorDriver(showProgresscheckBox.Checked);
                });
                commonProgress.UpdateStepLabel("Uninstall Appium server", "Uninstalling Appium Server, This may take sometime, Please wait...", 20);
                await Task.Run(() =>
                {
                    Common.UninstallAppium(showProgresscheckBox.Checked);
                });
                commonProgress.UpdateStepLabel("Install NodeJS", "Installing NodeJS, This may take sometime, Please wait...", 10);
                await Task.Run(() =>
                {
                    Common.InstallNodeJs(showProgresscheckBox.Checked);
                });
                commonProgress.UpdateStepLabel("Install Appium", "Installing Appium Server, This may take sometime, Please wait...", 30);
                await Task.Run(() =>
                {
                    Common.InstallAppiumGlobally(showProgresscheckBox.Checked);
                });
                commonProgress.UpdateStepLabel("Install XCUITest", "Installing XCUITest(iOS) driver, This may take sometime, Please wait...", 50);
                await Task.Run(() =>
                {
                    Common.InstallXCUITestDriver(showProgresscheckBox.Checked);
                });
                commonProgress.UpdateStepLabel("Install UIAutomator2", "Installing UIAutomator2(android) driver, This may take sometime, Please wait...", 75);
                await Task.Run(() =>
                {
                    Common.InstallUIAutomatorDriver(showProgresscheckBox.Checked);
                });
                commonProgress.UpdateStepLabel("Get WebDriverAgent", "Getting compatible WebDriverAgent based on installed XCUITest driver version, This may take sometime, Please wait...", 90);
                await Task.Run(() =>
                {
                    Common.GetWebDriverAgentIPAFile();
                    isDownloaded = true;
                });
                commonProgress.Close();
                GoogleAnalytics.SendEvent("reInstallAllButton_Click");
                await FindIssues();
                MessageBox.Show("Restart the Appium server to apply the changes. Server -> Configuration -> Stop and Start", "Restart Appium Server", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void reInstallAllButton_MouseHover(object sender, EventArgs e)
        {
            ToolTip toolTip = new ToolTip();
            toolTip.ToolTipIcon = ToolTipIcon.Info;
            toolTip.ToolTipTitle = "Re-Install Everything";
            toolTip.SetToolTip(reInstallAllButton, "Re-install all components if you are facing any issues.");
        }

        private void showProgresscheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (showProgresscheckBox.Checked)
            {
                MessageBox.Show("Checking this box will display the CMD window where the Appium server or driver uninstallation/installation execution occurs. You must manually close the CMD window once the execution is completed to continue accessing the Appium wizard.\"", "Show execution status", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
