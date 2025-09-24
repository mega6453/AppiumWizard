using System;
using System.Diagnostics;

namespace Appium_Wizard
{
    public partial class Updater : Form
    {
        Dictionary<string, string> driverVersion = new Dictionary<string, string>();
        bool isUpdated = false;
        string InstalledNodeJSVersion = string.Empty;
        string InstalledAppiumServerVersion = string.Empty;
        string InstalledXCUITestDriverVersion = string.Empty;
        string InstalledUIAutomatorDriverVersion = string.Empty;
        string AvailableNodeJSVersion = string.Empty;
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
            await Task.Run(async () =>
            {
                try
                {
                    commonProgress.UpdateStepLabel("Check for Server updates", "Please wait while getting installed NodeJS version information. This may take sometime...", 10);
                    InstalledNodeJSVersion = Common.GetNodeVersion();
                    commonProgress.UpdateStepLabel("Check for Server updates", "Please wait while getting installed Appium server version information. This may take sometime...", 20);
                    InstalledAppiumServerVersion = Common.InstalledAppiumServerVersionFromPackageJson();
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
                    commonProgress.UpdateStepLabel("Check for Server updates", "Please wait while checking for NodeJS updates. This may take sometime...", 40);
                    AvailableNodeJSVersion = await Common.GetLatestNodeVersion();
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
                NodeCurrentVersionLabel.Text = InstalledNodeJSVersion;
                appiumCurrentVersionLabel.Text = InstalledAppiumServerVersion;
                XCUITestCurrentVersionLabel.Text = InstalledXCUITestDriverVersion;
                UIAutomatorCurrentVersionLabel.Text = InstalledUIAutomatorDriverVersion;
                NodeAvailableVersionLabel.Text = AvailableNodeJSVersion;
                AppiumAvailableVersionLabel.Text = AvailableAppiumVersion;
                XCUITestAvailableVersionLabel.Text = AvailableXCUITestVersion;
                UIAutomatorAvailableVersionLabel.Text = AvailableUIAutomatorVersion;

                //NodeJS
                Version currentVersionNode = new Version(InstalledNodeJSVersion);
                Version latestVersionNode = new Version(AvailableNodeJSVersion);

                bool isUpdateAvailableNode = latestVersionNode > currentVersionNode;
                NodeUpdateButton.Enabled = isUpdateAvailableNode;

                //Appium
                Version currentVersionAppium = new Version(InstalledAppiumServerVersion);
                Version latestVersionAppium = new Version(AvailableAppiumVersion);

                bool isUpdateAvailableAppium = latestVersionAppium > currentVersionAppium;
                AppiumButton.Enabled = isUpdateAvailableAppium;

                //XCUITest
                Version currentVersionXCUITest = new Version(InstalledXCUITestDriverVersion);
                Version latestVersionXCUITest = new Version(AvailableXCUITestVersion);

                bool isUpdateAvailableXCUITest = latestVersionXCUITest > currentVersionXCUITest;
                XCUITestButton.Enabled = isUpdateAvailableXCUITest;

                //UIAutomator2
                Version currentVersionUI = new Version(InstalledUIAutomatorDriverVersion);
                Version latestVersionUI = new Version(AvailableUIAutomatorVersion);

                bool isUpdateAvailableUI = latestVersionUI > currentVersionUI;
                UIAutomatorButton.Enabled = isUpdateAvailableUI;    
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
                CommonProgress commonProgress = new CommonProgress();
                commonProgress.Owner = this;
                Version v1 = new Version(InstalledXCUITestDriverVersion);
                Version v2 = new Version(AvailableXCUITestVersion);
                if (v1.Major != v2.Major)
                {
                    var result = MessageBox.Show("The driver 'xcuitest' has a major revision update (" + v1.Major + ".x.x => " + v2.Major + ".x.x), which could include breaking changes. " +
                        "\n\nPresss Yes to apply this update.\nPress No to apply next minor version update(if there's any).\nPress Cancel to cancel the update.", "XCUITest Update", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        commonProgress.Show();
                        commonProgress.UpdateStepLabel("Update XCUITest driver", "Please wait while updating XCUITest driver version to " + AvailableXCUITestVersion);
                        await Task.Run(() =>
                        {
                            Common.UpdateXCUITestDriver(showExecutionCheckbox.Checked, true);
                            isUpdated = true;
                        });
                    }
                    if (result == DialogResult.No)
                    {
                        commonProgress.Show();
                        commonProgress.UpdateStepLabel("Update XCUITest driver", "Please wait while updating XCUITest driver version to next available minor version(if there's any)...");
                        await Task.Run(() =>
                        {
                            Common.UpdateXCUITestDriver(showExecutionCheckbox.Checked);
                            isUpdated = true;
                        });
                    }
                }
                else
                {
                    commonProgress.Show();
                    commonProgress.UpdateStepLabel("Update XCUITest driver", "Please wait while updating XCUITest driver version to " + AvailableXCUITestVersion);
                    await Task.Run(() =>
                    {
                        Common.UpdateXCUITestDriver(showExecutionCheckbox.Checked, true);
                        isUpdated = true;
                    });
                }
                if (isUpdated)
                {
                    commonProgress.UpdateStepLabel("Get WebDriverAgent", "Getting compatible WebDriverAgent based on the updated XCUITest driver version, This may take sometime, Please wait...", 75);
                    await Task.Run(() =>
                    {
                        Common.GetWebDriverAgentIPAFile();
                    });
                    commonProgress.Close();
                    MessageBox.Show("Downloaded compatible version of WDA.\nDelete the already installed WDA from your iPhone and Open the device again to install the compataible WDA.", "Re-Install WDA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                await GetVersionInformation();
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
                MessageBox.Show("Checking this box will display the CMD window where the Appium server or driver update execution occurs. You must manually close the CMD window once the execution is completed to continue accessing the Appium wizard.", "Show execution status", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async void NodeUpdateButton_Click(object sender, EventArgs e)
        {
            string nodePath = Path.Combine(FilesPath.serverInstalledPath, "node.exe");
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Show();
            commonProgress.Owner = this;
            commonProgress.UpdateStepLabel("Update NodeJS", "Please wait...",5);
            if (File.Exists(nodePath))
            {
                // Check if node.exe is running from the specified path
                var runningNodeProcesses = Process.GetProcessesByName("node")
                    .Where(p =>
                    {
                        try
                        {
                            return string.Equals(p.MainModule.FileName, nodePath, StringComparison.OrdinalIgnoreCase);
                        }
                        catch
                        {
                            // Access denied to process info, ignore this process
                            return false;
                        }
                    }).ToList();

                if (runningNodeProcesses.Any())
                {
                    var result = MessageBox.Show(
                        "Node.js is currently running. Do you want to kill it to continue?",
                        "Node.js Running",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        foreach (var proc in runningNodeProcesses)
                        {
                            try
                            {
                                proc.Kill();
                                proc.WaitForExit();
                            }
                            catch (Exception ex)
                            {
                                commonProgress.Close();
                                MessageBox.Show($"Failed to kill process. Please try ending node.exe process using task manager and then try again.");
                                return; // abort download if we can't kill the process
                            }
                        }
                    }
                    else
                    {
                        // User chose not to kill node.exe, abort operation
                        commonProgress.Close();
                        return;
                    }
                }
            }
            commonProgress.UpdateStepLabel("Update NodeJS", "Please wait while downloading newer version of nodejs...", 20);
            await Common.DownloadNodeJS();
            commonProgress.UpdateStepLabel("Update NodeJS", "Please wait while installing newer version of nodejs...", 60);
            await Task.Run(() =>
            {
                Common.InstallNodeJs();
            });
            commonProgress.UpdateStepLabel("Update NodeJS", "Please wait while installing newer version of nodejs...", 90);
            commonProgress.Close();
            GoogleAnalytics.SendEvent("Update_NodeJS");
            await GetVersionInformation();
        }
    }
}
