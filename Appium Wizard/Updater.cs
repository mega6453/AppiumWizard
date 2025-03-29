using System;

namespace Appium_Wizard
{
    public partial class Updater : Form
    {
        Dictionary<string, string> driverVersion = new Dictionary<string, string>();
        bool isUpdated = false;
        string InstalledAppiumServerVersion = string.Empty;
        string InstalledXCUITestDriverVersion = string.Empty;
        string InstalledUIAutomatorDriverVersion = string.Empty;
        string AvailableAppiumVersion = string.Empty;
        string AvailableXCUITestVersion = string.Empty;
        string AvailableUIAutomatorVersion = string.Empty;
        public Updater()
        {
            InitializeComponent();
            this.Owner = MainScreen.main;
        }

        public async Task GetVersionInformation()
        {
            CommonProgress commonProgress = new CommonProgress();
            if (MainScreen.main == null)
            {
                commonProgress.Owner = this;
            }
            else
            {
                commonProgress.Owner = MainScreen.main;
            }
            commonProgress.Show();
            await Task.Run(() =>
            {
                try
                {
                    commonProgress.UpdateStepLabel("Check for Server updates", "Please wait while getting installed Appium server version information. This may take sometime...", 10);
                    InstalledAppiumServerVersion = Common.InstalledAppiumServerVersion();
                    commonProgress.UpdateStepLabel("Check for Server updates", "Please wait while getting installed driver version information. This may take sometime...", 30);
                    driverVersion = Common.InstalledDriverVersion();
                    if (driverVersion.ContainsKey("xcuitest"))
                    {
                        InstalledXCUITestDriverVersion = driverVersion["xcuitest"];
                    }
                    else
                    {
                        InstalledXCUITestDriverVersion = "NA";
                    }
                    if (driverVersion.ContainsKey("uiautomator2"))
                    {
                        InstalledUIAutomatorDriverVersion = driverVersion["uiautomator2"];
                    }
                    else
                    {
                        InstalledUIAutomatorDriverVersion = "NA";
                    }
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
            try
            {
                appiumCurrentVersionLabel.Text = InstalledAppiumServerVersion;
                XCUITestCurrentVersionLabel.Text = InstalledXCUITestDriverVersion;
                UIAutomatorCurrentVersionLabel.Text = InstalledUIAutomatorDriverVersion;
                AppiumAvailableVersionLabel.Text = AvailableAppiumVersion;
                XCUITestAvailableVersionLabel.Text = AvailableXCUITestVersion;
                UIAutomatorAvailableVersionLabel.Text = AvailableUIAutomatorVersion;

                Version currentVersionAppium = new Version(InstalledAppiumServerVersion);
                Version latestVersionAppium = new Version(AvailableAppiumVersion);

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
                Version currentVersionXCUITest = new Version(InstalledXCUITestDriverVersion);
                Version latestVersionXCUITest = new Version(AvailableXCUITestVersion);

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
                Version currentVersionUI = new Version(InstalledUIAutomatorDriverVersion);
                Version latestVersionUI = new Version(AvailableUIAutomatorVersion);

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
            catch (Exception)
            {
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
                Common.InstallAppiumGlobally(showExecutionCheckbox.Checked);
                GoogleAnalytics.SendEvent("Update_Appium");
                isUpdated = true;
            });
            commonProgress.Close();
            await GetVersionInformation();
        }

        private async void UIAutomatorButton_Click(object sender, EventArgs e)
        {
            try
            {
                Version v1 = new Version(InstalledUIAutomatorDriverVersion);
                Version v2 = new Version(AvailableUIAutomatorVersion);
                if (v1.Major != v2.Major)
                {
                    var result = MessageBox.Show("The driver 'uiautomator2' has a major revision update (" + v1.Major + ".x.x => " + v2.Major + ".x.x), which could include breaking changes. " +
                        "\n\nPresss Yes to apply this update.\nPress No to apply next minor version update(if there's any).\nPress Cancel to cancel the update.", "UIAutomator2 Update", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        CommonProgress commonProgress = new CommonProgress();
                        commonProgress.Owner = this;
                        commonProgress.Show();
                        commonProgress.UpdateStepLabel("Update UIAutomator driver", "Please wait while updating UIAutomator driver version to " + AvailableUIAutomatorVersion);
                        await Task.Run(() =>
                        {
                            Common.UpdateUIAutomatorDriver(showExecutionCheckbox.Checked, true);
                            isUpdated = true;
                        });
                        commonProgress.Close();
                        await GetVersionInformation();
                    }
                    if (result == DialogResult.No)
                    {
                        CommonProgress commonProgress = new CommonProgress();
                        commonProgress.Owner = this;
                        commonProgress.Show();
                        commonProgress.UpdateStepLabel("Update UIAutomator driver", "Please wait while updating UIAutomator driver version to next available minor version(if there's any)...");
                        await Task.Run(() =>
                        {
                            Common.UpdateUIAutomatorDriver(showExecutionCheckbox.Checked);
                            isUpdated = true;
                        });
                        commonProgress.Close();
                        await GetVersionInformation();
                    }
                }
                else
                {
                    CommonProgress commonProgress = new CommonProgress();
                    commonProgress.Owner = this;
                    commonProgress.Show();
                    commonProgress.UpdateStepLabel("Update UIAutomator driver", "Please wait while updating UIAutomator driver version to " + AvailableUIAutomatorVersion);
                    await Task.Run(() =>
                    {
                        Common.UpdateUIAutomatorDriver(showExecutionCheckbox.Checked, true);
                        isUpdated = true;
                    });
                    commonProgress.Close();
                    await GetVersionInformation();
                }

            }
            catch (Exception)
            {
                CommonProgress commonProgress = new CommonProgress();
                commonProgress.Owner = this;
                commonProgress.Show();
                commonProgress.UpdateStepLabel("Update UIAutomator driver", "Please wait while updating UIAutomator driver version to " + AvailableUIAutomatorVersion);
                await Task.Run(() =>
                {
                    Common.UpdateUIAutomatorDriver(showExecutionCheckbox.Checked, true);
                    isUpdated = true;
                });
                commonProgress.Close();
                await GetVersionInformation();
            }
            GoogleAnalytics.SendEvent("Update_UIAutomator");
        }

        private async void XCUITestButton_Click(object sender, EventArgs e)
        {
            try
            {
                Version v1 = new Version(InstalledXCUITestDriverVersion);
                Version v2 = new Version(AvailableXCUITestVersion);
                if (v1.Major != v2.Major)
                {
                    var result = MessageBox.Show("The driver 'xcuitest' has a major revision update (" + v1.Major + ".x.x => " + v2.Major + ".x.x), which could include breaking changes. " +
                        "\n\nPresss Yes to apply this update.\nPress No to apply next minor version update(if there's any).\nPress Cancel to cancel the update.", "XCUITest Update", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        CommonProgress commonProgress = new CommonProgress();
                        commonProgress.Owner = this;
                        commonProgress.Show();
                        commonProgress.UpdateStepLabel("Update XCUITest driver", "Please wait while updating XCUITest driver version to " + AvailableXCUITestVersion);
                        await Task.Run(() =>
                        {
                            Common.UpdateXCUITestDriver(showExecutionCheckbox.Checked, true);
                            isUpdated = true;
                        });
                        commonProgress.Close();
                        await GetVersionInformation();
                    }
                    if (result == DialogResult.No)
                    {
                        CommonProgress commonProgress = new CommonProgress();
                        commonProgress.Owner = this;
                        commonProgress.Show();
                        commonProgress.UpdateStepLabel("Update XCUITest driver", "Please wait while updating XCUITest driver version to next available minor version(if there's any)...");
                        await Task.Run(() =>
                        {
                            Common.UpdateXCUITestDriver(showExecutionCheckbox.Checked);
                            isUpdated = true;
                        });
                        commonProgress.Close();
                        await GetVersionInformation();
                    }
                }
                else
                {
                    CommonProgress commonProgress = new CommonProgress();
                    commonProgress.Owner = this;
                    commonProgress.Show();
                    commonProgress.UpdateStepLabel("Update XCUITest driver", "Please wait while updating XCUITest driver version to " + AvailableXCUITestVersion);
                    await Task.Run(() =>
                    {
                        Common.UpdateXCUITestDriver(showExecutionCheckbox.Checked, true);
                        isUpdated = true;
                    });
                    commonProgress.Close();
                    await GetVersionInformation();
                }
            }
            catch (Exception)
            {
                CommonProgress commonProgress = new CommonProgress();
                commonProgress.Owner = this;
                commonProgress.Show();
                commonProgress.UpdateStepLabel("Update XCUITest driver", "Please wait while updating XCUITest driver version to " + AvailableXCUITestVersion);
                await Task.Run(() =>
                {
                    Common.UpdateXCUITestDriver(showExecutionCheckbox.Checked, true);
                    isUpdated = true;
                });
                commonProgress.Close();
                await GetVersionInformation();
            }
            GoogleAnalytics.SendEvent("Update_XCUITest");
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

        private void showExecutionCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (showExecutionCheckbox.Checked)
            {
                MessageBox.Show("Checking this box will display the CMD window where the Appium server or driver update execution occurs. You must manually close the CMD window once the execution is completed to continue accessing the Appium wizard.\"", "Show execution status",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        }
    }
}
