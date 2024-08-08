using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Appium_Wizard
{
    public partial class Server_Settings : Form
    {
        public Server_Settings()
        {
            InitializeComponent();
        }

        private void ServerArgsLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                ProcessStartInfo psInfo = new ProcessStartInfo
                {
                    FileName = "https://appium.io/docs/en/latest/cli/args/",
                    UseShellExecute = true
                };
                Process.Start(psInfo);
                GoogleAnalytics.SendEvent("ServerArgsLink_LinkClicked");
            }
            catch (Exception exception)
            {
                GoogleAnalytics.SendExceptionEvent("ServerArgsLink_LinkClicked", exception.Message);
            }
        }

        private void Server_Settings_Load(object sender, EventArgs e)
        {

        }
    }
}
