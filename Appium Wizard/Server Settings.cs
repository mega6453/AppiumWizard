using Newtonsoft.Json;
using System.Diagnostics;
using System.Text.Json.Nodes;

namespace Appium_Wizard
{
    public partial class Server_Settings : Form
    {
        int portNumber;
       
        public static string defaultCommand = string.Empty;
        public static string serverCLICommand = string.Empty;
        public static string defaultCapabilities = string.Empty;
        public static string finalCommand = string.Empty;
        public Server_Settings(int portNumber)
        {
            this.portNumber = portNumber;
            InitializeComponent();
            defaultCommand = "appium --allow-cors --port " + portNumber;
            defaultCapabilities = $@" -dc ""{{\""appium:webDriverAgentUrl\"":\""http://localhost:webDriverAgentProxyPort\""}}""";
            finalCommand = defaultCommand;
            serverCLICommand = defaultCommand;
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
            this.Text = "Server Settings - " + portNumber;
            FinalCommandRichTextBox.Text = "appium --allow-cors --port " + portNumber;
            finalCommand = FinalCommandRichTextBox.Text;
        }

        private void ServerArgs_TextChanged(object sender, EventArgs e)
        {
            UpdateFinalCommand();
        }

        private void DefaultCapabilities_TextChanged(object sender, EventArgs e)
        {
            UpdateFinalCommand();           
        }

        private void UpdateFinalCommand()
        {
            string serverArgsText = ServerArgs.Text;
            string defaultCapabilitiesText = DefaultCapabilities.Text;
            serverCLICommand = defaultCommand + " " + serverArgsText;
            if (!string.IsNullOrEmpty(defaultCapabilitiesText))
            {
                serverCLICommand += $@" -dc {defaultCapabilitiesText}";
            }
            FinalCommandRichTextBox.Text = serverCLICommand;
        }


        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            string currentText = FinalCommandRichTextBox.Text;
            finalCommand = $@"/C {currentText.Replace("\"", "\\\"")}";
            this.Close();
        }


    }
}
