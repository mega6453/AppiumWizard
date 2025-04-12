using System.Data;
using System.Windows.Forms;

namespace Appium_Wizard
{
    public partial class InstalledAppsList : Form
    {
        string os, udid, deviceName;
        List<string> packageNames = new List<string>();
        string selectedPackageName;
        Form ownerForm;
        public InstalledAppsList(string os, string udid, string deviceName)
        {
            this.os = os;
            this.udid = udid;
            this.deviceName = deviceName;
            InitializeComponent();
            this.Text = "Installed Apps List - " + deviceName;
            if (os.Equals("Android"))
            {
                DropDownButton.Visible = true;
            }
            else
            {
                DropDownButton.Visible = false;
            }
        }
        public async Task GetInstalledAppsList(object obj)
        {
            CommonProgress commonProgress = new CommonProgress();
            if (obj is Form form)
            {
                ownerForm = form;
                commonProgress.Owner = form;
            }
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Installed Apps List - " + deviceName, "Please wait while fetching list of installed apps...");
            await Task.Run(() =>
            {
                GetListOfAppsAndUpdateListView();
            });
            commonProgress.Close();
        }

        public void GetListOfAppsAndUpdateListView()
        {
            if (os.Equals("Android"))
            {
                packageNames = AndroidMethods.GetInstance().GetListOfInstalledApps(udid);
            }
            else
            {
                packageNames = iOSMethods.GetInstance().GetListOfInstalledApps(udid);
            }
            listView1.Items.Clear();
            foreach (string packageName in packageNames)
            {
                listView1.Items.Add(packageName);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = listView1.SelectedItems[0];
                selectedPackageName = selectedItem.SubItems[0].Text;
                UninstallButton.Enabled = true;
                LaunchButton.Enabled = true;
                KillAppButton.Enabled = true;
                DropDownButton.Enabled = true;
            }
            else
            {
                UninstallButton.Enabled = false;
                LaunchButton.Enabled = false;
                KillAppButton.Enabled = false;
                DropDownButton.Enabled = false;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string searchText = textBoxSearch.Text.ToLower();

            List<string> filteredPackageNames = packageNames
                .Where(packageName => packageName.ToLower().Contains(searchText))
                .ToList();

            listView1.Items.Clear();

            foreach (string packageName in filteredPackageNames)
            {
                listView1.Items.Add(packageName);
            }
        }

        private async void LaunchButton_Click(object sender, EventArgs e)
        {
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Owner = this;
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Launch App", "Please wait while launching app : " + selectedPackageName);
            await Task.Run(() =>
            {
                if (os.Equals("Android"))
                {
                    string activityName = AndroidMethods.GetInstance().GetAppActivity(udid, selectedPackageName);
                    AndroidMethods.GetInstance().LaunchApp(udid, activityName);
                    GoogleAnalytics.SendEvent("Android_App_Launched");
                }
                else
                {
                    var output = iOSMethods.GetInstance().LaunchApp(udid, selectedPackageName);
                    if (output.Contains("profile has not been explicitly trusted by the user"))
                    {
                        MessageBox.Show("Unable to launch " + selectedPackageName + " because it has an invalid code signature, inadequate entitlements or its profile has not been explicitly trusted by the user.\n\nTo trust a profile on the device, go to Settings-> General-> VPN & Device Management-> Select Profile-> Trust.", "Launch Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        GoogleAnalytics.SendEvent("Launch_iOS_App_Profile_Not_Trusted");
                    }
                    GoogleAnalytics.SendEvent("iOS_App_Launched");
                }
            });
            commonProgress.Close();
        }

        private async void UninstallButton_Click(object sender, EventArgs e)
        {
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Owner = this;
            if (os.Equals("Android"))
            {
                var result = MessageBox.Show("Are you sure you want to uninstall " + selectedPackageName + " from " + deviceName + "?", "Uninstall App", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    commonProgress.Show();
                    commonProgress.UpdateStepLabel("Uninstall App", "Please wait while uninstalling app : " + selectedPackageName);
                    await Task.Run(() =>
                    {
                        AndroidMethods.GetInstance().UnInstallApp(udid, selectedPackageName);
                    });
                    GoogleAnalytics.SendEvent("Android_App_Uninstalled");
                }
            }
            else
            {
                var result = MessageBox.Show("Are you sure you want to uninstall " + selectedPackageName + " from " + deviceName + "?", "Uninstall App", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    commonProgress.Show();
                    commonProgress.UpdateStepLabel("Uninstall App", "Please wait while uninstalling app : " + selectedPackageName);
                    await Task.Run(() =>
                    {
                        iOSMethods.GetInstance().UninstallApp(udid, selectedPackageName);
                    });
                    GoogleAnalytics.SendEvent("iOS_App_Uninstalled");
                }
            }
            GetListOfAppsAndUpdateListView();
            textBoxSearch.Clear();
            UninstallButton.Enabled = false;
            LaunchButton.Enabled = false;
            KillAppButton.Enabled = false;
            commonProgress.Close();
        }

        private async void KillAppButton_Click(object sender, EventArgs e)
        {
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Owner = this;
            if (os.Equals("Android"))
            {
                var result = MessageBox.Show("Are you sure you want to kill " + selectedPackageName + " in " + deviceName + "?", "Kill App", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    commonProgress.Show();
                    commonProgress.UpdateStepLabel("Kill App", "Please wait while killing app : " + selectedPackageName);
                    await Task.Run(() =>
                    {
                        AndroidMethods.GetInstance().KillApp(udid, selectedPackageName);
                    });
                    GoogleAnalytics.SendEvent("Android_App_Killed");
                }
            }
            else
            {
                var result = MessageBox.Show("Are you sure you want to kill " + selectedPackageName + " in " + deviceName + "?", "Kill App", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    commonProgress.Show();
                    commonProgress.UpdateStepLabel("Kill App", "Please wait while killing app : " + selectedPackageName);
                    await Task.Run(() =>
                    {
                        iOSMethods.GetInstance().KillApp(udid, selectedPackageName);
                    });
                    GoogleAnalytics.SendEvent("iOS_App_Killed");
                }
            }
            commonProgress.Close();
        }

        private void InstalledAppsList_Shown(object sender, EventArgs e)
        {
            GoogleAnalytics.SendEvent("InstalledAppsList_Shown");
        }

        private void InstalledAppsList_Load(object sender, EventArgs e)
        {
            this.Owner = ownerForm;
        }

        private void DropDownButton_Click(object sender, EventArgs e)
        {
            Point screenPoint = DropDownButton.PointToScreen(new Point(0, DropDownButton.Height));
            contextMenuStrip1.Show(screenPoint);
        }

        private void clearAppDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to clear data for " + selectedPackageName + " from " + deviceName + "?", "Clear App Data", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                AndroidMethods.GetInstance().ClearAppData(udid, selectedPackageName);
                GoogleAnalytics.SendEvent("Android_Clear_App_Data");
            }
        }

        private void listView1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listView1.FocusedItem.Bounds.Contains(e.Location))
                {
                    copyContextMenuStrip.Show(Cursor.Position);
                }
            }
        }

        private void copyPackageNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                string packageName = listView1.SelectedItems[0].SubItems[0].Text;
                Clipboard.SetText(packageName);
                GoogleAnalytics.SendEvent("copyPackageNameToolStripMenuItem_Click");
            }
        }
    }
}
