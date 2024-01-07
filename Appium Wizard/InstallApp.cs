using System.ComponentModel;

namespace Appium_Wizard
{
    public partial class InstallApp : Form
    {
        string filePath,fileName;
        string selectedUDID;
        string selectedDeviceName;
        InstallApp installApp;
        public InstallApp(string selectedDeviceName, string selectedUDID, string filePath, string fileName)
        {
            InitializeComponent();
            this.filePath = filePath;
            this.selectedUDID = selectedUDID;
            this.selectedDeviceName = selectedDeviceName;
            this.fileName = fileName;
            installApp = this;
        }

        private void InstallApp_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            MessageBox.Show("Sign & Install : If you sign and install then you don't need to manually Trust this app in settings.\n\nJust Install : App Installation may fail. If Installed successfully, then You may need to Trust this app in settings, if it's not signed already.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool profileCheck = iOSMethods.GetInstance().isProfileAvailableToSign(selectedUDID).Item1;
            if (profileCheck)
            {
                Close();
                CommonProgress commonProgress = new CommonProgress();
                commonProgress.Show();
                commonProgress.UpdateStepLabel("Sign App", "Attempting to Sign " + fileName + ". Please wait, this may take some time...");
                string signediPAPath = iOSMethods.GetInstance().SignIPA(selectedUDID, filePath);
                commonProgress.Close();
                if (signediPAPath.Equals("notsigned"))
                {
                    MessageBox.Show("No profile found for device "+selectedDeviceName+"(" + selectedUDID + ").\nAdd a profile in Tools->iOS Profile Management.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MainScreen.InstalliOSApp(selectedDeviceName, selectedUDID, signediPAPath, installApp);
                }
            }
            else
            {
                MessageBox.Show("No profile found for device " + selectedUDID + ".\nAdd a profile in Tools->iOS Profile Management.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
            MainScreen.InstalliOSApp(selectedDeviceName, selectedUDID, filePath, installApp);
        }
    }
}
