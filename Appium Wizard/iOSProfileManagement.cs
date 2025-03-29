using System.Reflection;

namespace Appium_Wizard
{
    public partial class iOSProfileManagement : Form
    {
        static string ProfilesFilePath = FilesPath.ProfilesFilePath;
        string selectedProfileName, selectedProfilePath;
        ListViewItem selectedItem;
        public iOSProfileManagement()
        {
            InitializeComponent();
            AdjustFormAndListViewSize();
        }

        private void import_Click(object sender, EventArgs e)
        {
            ImportProfile importProfile = new ImportProfile(listView1);
            importProfile.ShowDialog();
        }

        private void delete_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to delete " + selectedProfileName + " profile?", "Delete Profile", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {
                try
                {
                    Directory.Delete(selectedProfilePath, true);
                    MessageBox.Show(selectedProfileName + " removed successfully.", "Delete Profile", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    listView1.Items.Remove(selectedItem);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Exception - " + ex, "Failed to Delete", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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

        public static List<string[]> FetchProfiles()
        {
            List<string[]> profilesList = new List<string[]>();
            try
            {
                if (!Directory.Exists(ProfilesFilePath))
                {
                    Directory.CreateDirectory(ProfilesFilePath);
                }
                string[] profileFolders = Directory.GetDirectories(ProfilesFilePath);
                foreach (string profileFolder in profileFolders)
                {
                    string[] provisioningFiles = Directory.EnumerateFiles(profileFolder, "*.mobileprovision").ToArray();

                    if (provisioningFiles.Length > 0)
                    {
                        foreach (string provisioningFile in provisioningFiles)
                        {
                            string directoryPath = Path.GetDirectoryName(provisioningFile);
                            string expiryDateFromPem = ImportProfile.GetExpirationDateFromPemFile(directoryPath + "\\certificate.pem");
                            int expirationDaysFromPem = ImportProfile.ExpirationDays(expiryDateFromPem);

                            var provisionDetails = ImportProfile.GetDetailsFromProvisionFile(provisioningFile);
                            string profileName = provisionDetails["Name"].ToString();
                            int expirationDaysFromProvisionFile = ImportProfile.ExpirationDays(provisionDetails["ExpirationDate"].ToString());
                            string appId = provisionDetails["application-identifier"].ToString();
                            string teamId = provisionDetails["com.apple.developer.team-identifier"].ToString();
                            string updatedExpirationDays = string.Empty;
                            try
                            {
                                if (expirationDaysFromProvisionFile <= 0 | expirationDaysFromPem <= 0)
                                {
                                    updatedExpirationDays = "Expired";
                                }
                                else
                                {
                                    if (expirationDaysFromPem < expirationDaysFromProvisionFile)
                                    {
                                        updatedExpirationDays = expirationDaysFromPem.ToString() + " days";
                                    }
                                    else
                                    {
                                        updatedExpirationDays = expirationDaysFromProvisionFile.ToString() + " days";
                                    }
                                }
                                string[] item1 = { profileName, updatedExpirationDays, appId, teamId, profileFolder };
                                profilesList.Add(item1);
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return profilesList;
        }

        public async Task UpdateProfilesList(CommonProgress commonProgress)
        {
            commonProgress.UpdateStepLabel("Load Profiles", "Please wait while fetching profiles...");
            await Task.Run(() => {
                List<string[]> profilesList = FetchProfiles();
                foreach (var item in profilesList)
                {
                    if (listView1.InvokeRequired)
                    {
                        listView1.Invoke(new Action(() => listView1.Items.Add(new ListViewItem(item))));
                    }
                    else
                    {
                        listView1.Items.Add(new ListViewItem(item));
                    }
                }
            });
            commonProgress.Close();
        }

        private void iOSProfileManagement_Shown(object sender, EventArgs e)
        {
           GoogleAnalytics.SendEvent("iOSProfileManagement_Shown");
        }

        private void AdjustFormAndListViewSize()
        {
            using (Graphics g = this.CreateGraphics())
            {
                // Get the current DPI scaling factor
                float dpiX = g.DpiX / 96.0f;

                // Calculate the new width based on the scaling factor
                int baseListViewWidth = 815;
                int newListViewWidth = (int)(baseListViewWidth * dpiX);

                // Adjust column widths and calculate total column width
                int totalColumnWidth = (int)((300 + 200 + 150 + 150) * dpiX);

                // Ensure ListView width is enough to fit all columns without scroll bar
                listView1.Width = Math.Max(newListViewWidth, totalColumnWidth);

                // Adjust Form width accordingly
                this.Width = listView1.Width + (this.Width - this.ClientSize.Width);

                // Set column widths
                listView1.Columns[0].Width = (int)(300 * dpiX);
                listView1.Columns[1].Width = (int)(200 * dpiX);
                listView1.Columns[2].Width = (int)(150 * dpiX);
                listView1.Columns[3].Width = (int)(150 * dpiX);

                // Hide the 5th column if it exists
                if (listView1.Columns.Count > 4)
                {
                    listView1.Columns[4].Width = 0;
                }
            }
        }
    }
}
