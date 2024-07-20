using System.Data;

namespace Appium_Wizard
{
    public partial class InstalledAppsList : Form
    {
        string os, udid, deviceName;
        List<string> packageNames = new List<string>();
        string selectedPackageName;
        public InstalledAppsList(string os, string udid, string deviceName)
        {
            this.os = os;
            this.udid = udid;
            this.deviceName = deviceName;
            InitializeComponent();
        }

        public async Task GetInstalledAppsList(MainScreen main)
        {
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Owner = main;
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Installed Apps List", "Please wait while fetching list of installed apps...");
            await Task.Run(() => {
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
            }
            else
            {
                UninstallButton.Enabled = false;
                LaunchButton.Enabled = false;
                KillAppButton.Enabled = false;
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
            await Task.Run(() => {
                if (os.Equals("Android"))
                {
                    string activityName = AndroidMethods.GetInstance().GetAppActivity(udid, selectedPackageName);
                    AndroidMethods.GetInstance().LaunchApp(udid, activityName);
                    GoogleAnalytics.SendEvent("Android_App_Launched");
                }
                else
                {
                    iOSMethods.GetInstance().LaunchApp(udid, selectedPackageName);
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
                    await Task.Run(() => {
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
                    await Task.Run(() => {
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
                    await Task.Run(() => {
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
                    await Task.Run(() => {
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
    }
}
