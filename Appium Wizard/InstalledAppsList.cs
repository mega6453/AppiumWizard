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

        private void InstalledAppsList_Load(object sender, EventArgs e)
        {
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Installed Apps List", "Please wait while fetching list of installed apps...");
            GetListOfAppsAndUpdateListView();
        }

        public void GetListOfAppsAndUpdateListView(bool update = false)
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
            }
            else
            {
                UninstallButton.Enabled = false;
                LaunchButton.Enabled = false;
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

        private void LaunchButton_Click(object sender, EventArgs e)
        {
            if (os.Equals("Android"))
            {
                string activityName = AndroidMethods.GetInstance().GetAppActivity(udid, selectedPackageName);
                AndroidMethods.GetInstance().LaunchApp(udid, activityName);
            }
            else
            {
                iOSMethods.GetInstance().LaunchApp(udid, selectedPackageName);
            }
        }

        private void UninstallButton_Click(object sender, EventArgs e)
        {
            if (os.Equals("Android"))
            {
                var result = MessageBox.Show("Are you sure you want to uninstall " + selectedPackageName + " from " + deviceName + "?", "Uninstall App", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    AndroidMethods.GetInstance().UnInstallApp(udid, selectedPackageName);
                }
            }
            else
            {
                var result = MessageBox.Show("Are you sure you want to uninstall " + selectedPackageName + " from " + deviceName + "?", "Uninstall App", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    iOSMethods.GetInstance().UninstallApp(udid, selectedPackageName);

                }
            }
            GetListOfAppsAndUpdateListView();
            textBoxSearch.Clear();
            UninstallButton.Enabled = false;
            LaunchButton.Enabled = false;
        }
    }
}
