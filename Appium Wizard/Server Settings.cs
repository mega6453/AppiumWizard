using Newtonsoft.Json;
using System.Diagnostics;
using System.Text.Json.Nodes;

namespace Appium_Wizard
{
    public partial class Server_Settings : Form
    {
        int portNumber;
        public static string serverCLICommand = string.Empty;
        public static string finalCommand = string.Empty;
        public Server_Settings(int portNumber)
        {
            this.portNumber = portNumber;
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
            this.Text = "Server Settings - " + portNumber;
            string updatedCapabilities = $@"-dc ""{{""appium:webDriverAgentUrl"":""http://localhost:webDriverAgentProxyPort""}}""";
            serverCLICommand = $@"appium --allow-cors --port {portNumber} {updatedCapabilities} ";
            FinalCommandRichTextBox.Text = serverCLICommand;
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
            string updatedCapabilities = $@"-dc ""{{""appium:webDriverAgentUrl"":""http://localhost:webDriverAgentProxyPort""}}""";
            if (string.IsNullOrEmpty(defaultCapabilitiesText))
            {
                updatedCapabilities = $@"-dc ""{{""appium:webDriverAgentUrl"":""http://localhost:webDriverAgentProxyPort""}}""";
            }
            else
            {
                if (defaultCapabilitiesText.StartsWith("{"))
                {
                    defaultCapabilitiesText = defaultCapabilitiesText.Substring(1);
                }
                if (defaultCapabilitiesText.EndsWith("}"))
                {
                    defaultCapabilitiesText = defaultCapabilitiesText.Substring(0, defaultCapabilitiesText.Length - 1);
                }
                updatedCapabilities = $@"-dc ""{{""appium:webDriverAgentUrl"":""http://localhost:webDriverAgentProxyPort"",{defaultCapabilitiesText}}}""";
            }
            serverCLICommand = $@"appium --allow-cors --port {portNumber} {serverArgsText} {updatedCapabilities}";
            FinalCommandRichTextBox.Text = serverCLICommand;
        }


        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            string currentText = FinalCommandRichTextBox.Text;
            finalCommand = $@"{currentText.Replace("\"", "\"\"")}";
            finalCommand = $@"/C {finalCommand.Replace("\"\"{", "\"{").Replace("}\"\"", "}\"")}";
            this.Close();
        }

        private void defaultCapLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                ProcessStartInfo psInfo = new ProcessStartInfo
                {
                    FileName = "https://appium.io/docs/en/latest/guides/caps/",
                    UseShellExecute = true
                };
                Process.Start(psInfo);
            }
            catch (Exception)
            {
            }
        }
    }
}
