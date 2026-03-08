using System;
using System.Windows.Forms;

namespace Appium_Wizard
{
    public partial class DownloadWDA : Form
    {
        private string latestVersion = "";

        public DownloadWDA()
        {
            InitializeComponent();
        }

        private async void DownloadWDA_Load(object sender, EventArgs e)
        {
            downloadLatestButton.Enabled = false;
            downloadLatestButton.Text = "Fetching latest version...";

            latestVersion = await Common.GetLatestWebDriverAgentVersionFromGitHub();

            if (latestVersion != "versionNotFound")
            {
                downloadLatestButton.Text = $"Download latest version of WDA ({latestVersion})";
            }
            else
            {
                downloadLatestButton.Text = "Download latest version of WDA (Unable to fetch)";
            }

            downloadLatestButton.Enabled = true;
        }

        private void DownloadLatestButton_Click(object sender, EventArgs e)
        {
            if (latestVersion == "versionNotFound" || string.IsNullOrEmpty(latestVersion))
            {
                MessageBox.Show("Could not determine the latest WDA version from GitHub. Please check your internet connection or try downloading a specific version instead.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DownloadWDAFile(latestVersion);
        }

        private void DownloadSpecificButton_Click(object sender, EventArgs e)
        {
            string version = versionTextBox.Text.Trim();
            if (string.IsNullOrEmpty(version))
            {
                MessageBox.Show("Please enter a version number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DownloadWDAFile(version);
        }

        private async void DownloadWDAFile(string version)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "IPA files (*.ipa)|*.ipa",
                FileName = $"WebDriverAgent-v{version}.ipa",
                Title = "Save WebDriverAgent IPA File"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string destinationPath = saveFileDialog.FileName;

                CommonProgress commonProgress = new CommonProgress();
                commonProgress.Owner = this;
                commonProgress.Show();
                commonProgress.UpdateStepLabel("Download WDA", "Starting download...", 0);

                bool success = await Common.DownloadWebDriverAgentIPAFile(version, destinationPath, commonProgress);

                commonProgress.Close();

                if (success)
                {
                    MessageBox.Show($"WebDriverAgent v{version} downloaded successfully to:\n{destinationPath}", "Download Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show($"Failed to download WebDriverAgent v{version}. Please check the version number and try again.", "Download Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
