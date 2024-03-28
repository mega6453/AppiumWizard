using System.Reflection;

namespace Appium_Wizard
{
    public partial class iOSProfileManagement : Form
    {
        string ProfilesFilePath = FilesPath.ProfilesFilePath;
        string selectedProfileName, selectedProfilePath;
        ListViewItem selectedItem;
        public iOSProfileManagement()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ImportProfile importProfile = new ImportProfile(listView1);
            importProfile.ShowDialog();
        }


        private void iOSProfileManagement_Load(object sender, EventArgs e)
        {
            RefreshProfileListView();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to delete " + selectedProfileName + " profile?", "Delete Profile", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {
                Directory.Delete(selectedProfilePath, true);
                MessageBox.Show(selectedProfileName + " removed successfully.", "Delete Profile", MessageBoxButtons.OK, MessageBoxIcon.Information);
                listView1.Items.Remove(selectedItem);
            }
            GoogleAnalytics.SendEvent("iOSProfileManage_DeleteProfile_Clicked");
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                selectedItem = listView1.SelectedItems[0];
                selectedProfileName = selectedItem.SubItems[0].Text;
                selectedProfilePath = selectedItem.SubItems[4].Text;
                deleteProfileButton.Enabled = true;
            }
            else
            {
                deleteProfileButton.Enabled = false;
            }
        }

        public void RefreshProfileListView()
        {
            if (!Directory.Exists(ProfilesFilePath))
            {
                Directory.CreateDirectory(ProfilesFilePath);
            }
            //List<string> UDIDFromProfile = new List<string>();
            //List<string> UDIDFromDB = new List<string>();
            //var devicesFromDB = Database.QueryDataFromDevicesTable();
            //foreach (var device in devicesFromDB)
            //{
            //    UDIDFromDB.Add(device["UDID"]);
            //}
            string[] profileFolders = Directory.GetDirectories(ProfilesFilePath);
            foreach (string profileFolder in profileFolders)
            {
                string[] provisioningFiles = Directory.EnumerateFiles(profileFolder, "*.mobileprovision").ToArray();

                if (provisioningFiles.Length > 0)
                {
                    foreach (string provisioningFile in provisioningFiles)
                    {
                        var provisionDetails = ImportProfile.GetDetailsFromProvisionFile(provisioningFile);
                        //UDIDFromProfile = (List<string>)provisionDetails["DevicesList"];
                        string profileName = provisionDetails["Name"].ToString();
                        int expirationDays = ImportProfile.ExpirationDays(provisionDetails["ExpirationDate"].ToString());
                        string appId = provisionDetails["application-identifier"].ToString();
                        string teamId = provisionDetails["com.apple.developer.team-identifier"].ToString();
                        string updatedExpirationDays = expirationDays.ToString() + " days";
                        try
                        {
                            if (expirationDays <= 0)
                            {
                                updatedExpirationDays = "Expired";
                            }
                            string[] item1 = { profileName, updatedExpirationDays, appId, teamId, profileFolder };
                            listView1.Items.Add(new ListViewItem(item1));
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            //GoogleAnalytics.SendEvent("RefreshProfilesListView");
        }

        private void iOSProfileManagement_Shown(object sender, EventArgs e)
        {
            GoogleAnalytics.SendEvent(MethodBase.GetCurrentMethod().Name);
        }
    }
}
