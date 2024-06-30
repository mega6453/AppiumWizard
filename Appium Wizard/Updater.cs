using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        private void Updater_Load(object sender, EventArgs e)
        {
            GetVersionInformation();
        }

        private void GetVersionInformation()
        {
            try
            {
                CommonProgress commonProgress = new CommonProgress();
                commonProgress.Show();
                commonProgress.UpdateStepLabel("Check for Server updates", "Please wait while getting installed server version information. This may take sometime...");
                driverVersion = Common.InstalledDriverVersion();
                appiumCurrentVersionLabel.Text = Common.InstalledAppiumServerVersion();
                XCUITestCurrentVersionLabel.Text = driverVersion["xcuitest"];
                UIAutomatorCurrentVersionLabel.Text = driverVersion["uiautomator2"];
                commonProgress.UpdateStepLabel("Check for Server updates", "Please wait while checking for server updates. This may take sometime...");
                AppiumAvailableVersionLabel.Text = Common.AavailableAppiumVersion();
                XCUITestAvailableVersionLabel.Text = Common.AavailableXCUITestVersion();
                UIAutomatorAvailableVersionLabel.Text = Common.AavailableUIAutomatorVersion();

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
            catch (Exception e)
            {
                MessageBox.Show(e.Message,"Unhandled Exception",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        private void AppiumButton_Click(object sender, EventArgs e)
        {
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Update Appium Server", "Please wait while updating appium server version to " + AppiumAvailableVersionLabel.Text);
            Common.InstallAppiumGlobally();
            commonProgress.Close();
            GetVersionInformation();
            GoogleAnalytics.SendEvent("Update_Appium");
            isUpdated = true;
        }

        private void UIAutomatorButton_Click(object sender, EventArgs e)
        {
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Update UIAutomator driver", "Please wait while updating UIAutomator driver version to " + UIAutomatorAvailableVersionLabel.Text);
            Common.UpdateUIAutomatorDriver();
            commonProgress.Close();
            GetVersionInformation();
            GoogleAnalytics.SendEvent("Update_UIAutomator");
            isUpdated = true;
        }

        private void XCUITestButton_Click(object sender, EventArgs e)
        {
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Update XCUITest driver", "Please wait while updating XCUITest driver version to " + XCUITestAvailableVersionLabel.Text);
            Common.UpdateXCUITestDriver();
            commonProgress.Close();
            GetVersionInformation();
            GoogleAnalytics.SendEvent("Update_XCUITest");
            isUpdated = true;
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
