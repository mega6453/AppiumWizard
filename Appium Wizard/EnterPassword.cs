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
    public partial class EnterPassword : Form
    {
        string udid, os, deviceName;
        public EnterPassword(string os, string udid, string deviceName)
        {
            this.udid = udid;
            this.os = os;
            this.deviceName = deviceName;
            InitializeComponent();
            if (os.Equals("Android"))
            {
                NoteForiOSLabel.Visible = false;
            }
            else
            {
                NoteForiOSLabel.Visible = true;
            }
            this.Text = "Unlock Screen - " + deviceName;
        }

        private async void Unlock_Click(object sender, EventArgs e)
        {
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Owner = this;
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Unlock Screen - " + deviceName, "Please wait while unlocking screen...");
            await Task.Run(() =>
            {
                if (os.Equals("Android"))
                {
                    AndroidMethods.GetInstance().UnlockScreen(udid, PasswordTextbox.Text);
                }
                else
                {
                    iOSMethods.GetInstance().UnlockScreen(udid, PasswordTextbox.Text, deviceName);
                }
            });
            commonProgress.Close();
            this.Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
