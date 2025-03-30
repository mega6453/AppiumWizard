using Appium_Wizard.Properties;
using NLog;
using System.Linq;

namespace Appium_Wizard
{
    public partial class UsePreInstalledWDA : Form
    {
        string udid, deviceName;
        string defaultBundleId = "com.facebook.WebDriverAgentRunner.xctrunner";
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public UsePreInstalledWDA(string udid, string deviceName)
        {
            this.udid = udid;
            this.deviceName = deviceName;
            InitializeComponent();
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (dontUseRadioButton.Checked)
                {
                    Logger.Info("Don't use Pre installed WDA");
                    GoogleAnalytics.SendEvent("preinstalledwda_dontuse");
                    Database.DeleteUDIDFromUsePreInstalledWDAList(udid);
                    MainScreen.UDIDPreInstalledWDA.Remove(udid);
                    Close();
                }
                if (customRadioButton.Checked)
                {
                    if (string.IsNullOrEmpty(bundleIdTextbox.Text.Trim()) | string.IsNullOrWhiteSpace(bundleIdTextbox.Text.Trim()))
                    {
                        MessageBox.Show("Please enter the bundle id of WebDriverAgentRunner which you have installed in your device and then apply.", "Use Pre-Installed WDA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        Logger.Info("Use Pre installed custom WDA. Bundle Id : "+ bundleIdTextbox.Text.Trim());
                        GoogleAnalytics.SendEvent("preinstalledwda_custom");
                        Database.DeleteUDIDFromUsePreInstalledWDAList(udid);
                        Database.InsertUDIDAndBundleIdIntoUsePreInstalledWDAList(udid, bundleIdTextbox.Text.Trim());
                        MainScreen.UDIDPreInstalledWDA.Remove(udid);
                        MainScreen.UDIDPreInstalledWDA.Add(udid, bundleIdTextbox.Text.Trim());
                        Close();
                    }
                }
                if (defaultRadioButton.Checked)
                {
                    Logger.Info("Use Pre installed default WDA. Bundle Id : "+defaultBundleId);
                    GoogleAnalytics.SendEvent("preinstalledwda_default");
                    Database.DeleteUDIDFromUsePreInstalledWDAList(udid);
                    Database.InsertUDIDAndBundleIdIntoUsePreInstalledWDAList(udid, defaultBundleId);
                    MainScreen.UDIDPreInstalledWDA.Remove(udid);
                    MainScreen.UDIDPreInstalledWDA.Add(udid, defaultBundleId);
                    Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error while applying user preferences for pre-installed wda.");
                MessageBox.Show("Error while applying user preferences for pre-installed wda. \nOriginal exception: " + ex.Message, "Use Pre-Installed WDA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UsePreInstalledWDA_Load(object sender, EventArgs e)
        {
            deviceNameLabel.Text = deviceName;            
            var udids = Database.QueryUDIDsFromUsePreInstalledWDAList();
            var udidsAndBundleIds = Database.QueryUDIDsAndBundleIdsFromUsePreInstalledWDAList();

            if (udids.Contains(udid))
            {
                foreach (var item in udidsAndBundleIds)
                {
                    if (item.UDID.Equals(udid))
                    {
                        if (item.BundleId.Equals(defaultBundleId))
                        {
                            defaultRadioButton.Checked = true;
                            label1.Enabled = false;
                            bundleIdTextbox.Enabled = false;
                        }
                        else
                        {
                            customRadioButton.Checked = true;
                            label1.Enabled = true;
                            bundleIdTextbox.Enabled = true;
                            bundleIdTextbox.Text = item.BundleId.ToString();
                        }
                        break;
                    }
                }
            }
            else
            {
                dontUseRadioButton.Select();
                label1.Enabled = false;
                bundleIdTextbox.Enabled = false;
            }           
        }

        private void customRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (customRadioButton.Checked)
            {
                label1.Enabled = true;
                bundleIdTextbox.Enabled = true;
            }
            else
            {
                label1.Enabled = false;
                bundleIdTextbox.Enabled = false;
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
