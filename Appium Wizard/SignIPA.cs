﻿using System.Diagnostics;
using System.Reflection;

namespace Appium_Wizard
{
    public partial class SignIPA : Form
    {
        List<string[]> profilesList;
        Dictionary<int, string> profilesDictionary = new Dictionary<int, string>();
        public SignIPA(List<string[]> profilesList)
        {
            InitializeComponent();
            this.profilesList = profilesList;
        }

        private void SignIPA_Load(object sender, EventArgs e)
        {
            int index = 0;
            foreach (var item in profilesList)
            {
                string profile = item[0] + " - " + item[1] + " - " + item[2] + " - " + item[3];
                profilesListComboBox.Items.Add(profile);
                profilesDictionary.Add(index, item[4]);
                index++;
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
            GoogleAnalytics.SendEvent("Sign_IPA_CancelButton_Clicked");
        }

        private void SignButton_Click(object sender, EventArgs e)
        {
            if (profilesListComboBox.SelectedIndex.Equals(-1) || string.IsNullOrEmpty(IPAFilePathTextBox.Text) || string.IsNullOrEmpty(OutputPathTextBox.Text))
            {
                MessageBox.Show("All the fields are required to Sign IPA. Please check missing fields.", "Missing fields", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string profilePath = profilesDictionary[profilesListComboBox.SelectedIndex];
                CommonProgress commonProgress = new CommonProgress();
                commonProgress.Show();
                if (UDIDTextbox.Text == "")
                {
                    commonProgress.UpdateStepLabel("Sign App", "Attempting to sign the given IPA file. Please wait, this may take some time...\n\nIPA file : " + IPAFilePathTextBox.Name + "\nProfile : " + profilesListComboBox.SelectedItem);
                }
                else
                {
                    commonProgress.UpdateStepLabel("Sign App", "Attempting to sign the given IPA file. Please wait, this may take some time...\n\nIPA file : " + IPAFilePathTextBox.Name + "\nDevice : " + UDIDTextbox.Text + "\nProfile : " + profilesListComboBox.SelectedItem);
                }
                string output = iOSMethods.GetInstance().SignIPA(profilePath, IPAFilePathTextBox.Text, OutputPathTextBox.Text, UDIDTextbox.Text);
                commonProgress.Hide();
                if (output.Equals("ProfileNotAvailable"))
                {
                    var dialogResult = MessageBox.Show("The selected profile does not have the given UDID. Do you want to still Sign this IPA?", "Profile does not have UDID", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        commonProgress.Show();
                        commonProgress.UpdateStepLabel("Sign App", "Attempting to sign the given IPA file. Please wait, this may take some time...\n\nIPA file : " + IPAFilePathTextBox.Name + "\nProfile : " + profilesListComboBox.SelectedItem);
                        output = iOSMethods.GetInstance().SignIPA(profilePath, IPAFilePathTextBox.Text, OutputPathTextBox.Text, "");
                        GoogleAnalytics.SendEvent("ProfileNotAvailable_Yes_Sign");
                    }
                    else
                    {
                        GoogleAnalytics.SendEvent("ProfileNotAvailable_Dont_Sign");
                        return;
                    }
                }
                if (output.Equals("Sign_IPA_Failed"))
                {
                    MessageBox.Show("Signing Failed, Please try with different profile / different output path / different IPA file.", "Signing Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    GoogleAnalytics.SendEvent("Sign_IPA_Failed");
                }
                else
                {
                    var result = MessageBox.Show("Do you want to open the output folder?", "Signing Success", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            Process.Start("explorer.exe", "/select, \"" + output + "\"");
                        }
                        catch (Exception ex)
                        {
                            GoogleAnalytics.SendExceptionEvent("OpenOutputFolder_Yes", ex.Message);
                        }
                        GoogleAnalytics.SendEvent("OpenOutputFolder_Yes");
                    }
                    else
                    {
                        GoogleAnalytics.SendEvent("OpenOutputFolder_No");
                    }
                    GoogleAnalytics.SendEvent("Sign_IPA_Success");
                }
                commonProgress.Close();
            }
        }

        private void IPAFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "IPA files (*.ipa)|*.ipa";
            dialog.Title = "Select an IPA file";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = dialog.FileName;
                string fileName = Path.GetFileName(filePath);
                IPAFilePathTextBox.Text = filePath;
                IPAFilePathTextBox.Name = fileName;
            }
        }

        private void OutputPathButton_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "IPA files (*.ipa)|*.ipa";
                saveFileDialog.Title = "Save Signed IPA File";
                if (IPAFilePathTextBox.Text != "")
                {
                    saveFileDialog.FileName = IPAFilePathTextBox.Text.Replace(".ipa", "") + "_SignedIPA.ipa";
                }
                else
                {
                    saveFileDialog.FileName = "SignedIPA.ipa";
                }
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;
                    OutputPathTextBox.Text = filePath;
                }
            }
        }

        private void SignIPA_Shown(object sender, EventArgs e)
        {
            GoogleAnalytics.SendEvent(MethodBase.GetCurrentMethod().Name);
        }
    }
}
