using System.Diagnostics;
using System.Reflection;

namespace Appium_Wizard
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
        }

        private void About_Load(object sender, EventArgs e)
        {
            label1.Text = "Version : " + VersionInfo.VersionNumber;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string eventName = "GithubRepo_Link_Clicked";
            try
            {
                ProcessStartInfo psInfo = new ProcessStartInfo
                {
                    FileName = "https://github.com/mega6453/AppiumWizard",
                    UseShellExecute = true
                };
                Process.Start(psInfo);
            }
            catch (Exception exception)
            {
                GoogleAnalytics.SendExceptionEvent(eventName, exception.Message);
            }
            GoogleAnalytics.SendEvent(eventName);
        }

        private void About_Shown(object sender, EventArgs e)
        {
            GoogleAnalytics.SendEvent(MethodBase.GetCurrentMethod().Name);
        }
    }
}
