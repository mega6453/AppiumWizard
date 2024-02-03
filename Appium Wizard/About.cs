using System.Diagnostics;

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
                Console.WriteLine("Exception" + exception);
            }
        }
    }
}
