using Newtonsoft.Json;
using System.Diagnostics;
using System.Text.Json.Nodes;

namespace Appium_Wizard
{
    public partial class Server_Settings : Form
    {
        int portNumber;
        public static string logLevelFromDB = string.Empty;
        public static string finalCommandFromDB = string.Empty;
        public static string argsCommandFromDB = string.Empty;
        public static string capsCommandFromDB = string.Empty;
        public static string serverCLICommand = string.Empty;
        public static string finalCommand = string.Empty;
        string serverNumber = string.Empty;
        private bool isFormInitialized = false;
        public Server_Settings(string serverNumber, int portNumber)
        {
            this.serverNumber = serverNumber;
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
            if (Database.QueryDataFromServerFinalCommandTable().ContainsKey(serverNumber))
            {
                finalCommandFromDB = Database.QueryDataFromServerFinalCommandTable()[serverNumber];
            }
            if (Database.QueryDataFromServerArgsTable().ContainsKey(serverNumber))
            {
                argsCommandFromDB = Database.QueryDataFromServerArgsTable()[serverNumber];
            }
            if (Database.QueryDataFromServerCapsTable().ContainsKey(serverNumber))
            {
                capsCommandFromDB = Database.QueryDataFromServerCapsTable()[serverNumber];
            }
            if (Database.QueryDataFromlogLevelTable().ContainsKey(serverNumber))
            {
                logLevelFromDB = Database.QueryDataFromlogLevelTable()[serverNumber];
                SelectRadioButtonBasedOnValue(logLevelFromDB);
            }
            ServerArgsRichTextBox.Text = argsCommandFromDB;
            DefaultCapabilitiesRichTextBox.Text = capsCommandFromDB;

            finalCommandFromDB = finalCommandFromDB.Replace("}\"", "}\"\"").Replace("\"{", "\"\"{");
            finalCommandFromDB = finalCommandFromDB.Replace("\"\"", "\"");
            if (!finalCommandFromDB.Contains("--port"))
            {
                finalCommandFromDB = finalCommandFromDB + " --port " + portNumber;
            }
            if (!finalCommandFromDB.Contains("-dc"))
            {
                finalCommandFromDB = finalCommandFromDB + $@" -dc ""{{""appium:webDriverAgentUrl"":""http://localhost:webDriverAgentProxyPort""}}""";
            }
            FinalCommandRichTextBox.Text = finalCommandFromDB.Replace("\n", string.Empty);
            isFormInitialized = true;
        }

        private void SelectRadioButtonBasedOnValue(string value)
        {
            switch (value)
            {
                case "info":
                    infoRadioButton.Checked = true;
                    break;
                case "error":
                    errorRadioButton.Checked = true;
                    break;
                case "debug":
                    debugRadioButton.Checked = true;
                    break;
                default:
                    infoRadioButton.Checked = true;
                    errorRadioButton.Checked = false;
                    debugRadioButton.Checked = false;
                    break;
            }
        }

        private void ServerArgs_TextChanged(object sender, EventArgs e)
        {
            UpdateFinalCommand();
        }

        private void DefaultCapabilities_TextChanged(object sender, EventArgs e)
        {
            UpdateFinalCommand();
        }

        string updatedCapabilities = string.Empty;
        string serverArgsText = string.Empty;
        private void UpdateFinalCommand()
        {
            serverArgsText = ServerArgsRichTextBox.Text;
            string defaultCapabilitiesText = DefaultCapabilitiesRichTextBox.Text;
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
            serverCLICommand = $@"appium --port {portNumber} {serverArgsText} {updatedCapabilities}";
            FinalCommandRichTextBox.Text = serverCLICommand.Replace("\n", string.Empty);
        }


        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            string currentText = FinalCommandRichTextBox.Text;
            finalCommand = $@"{currentText.Replace("\"", "\"\"")}";
            finalCommand = $@"{finalCommand.Replace("\"\"{", "\"{").Replace("}\"\"", "}\"")}";
            Database.UpdateDataIntoServerArgsTable(serverNumber, serverArgsText);
            Database.UpdateDataIntoServerCapsTable(serverNumber, DefaultCapabilitiesRichTextBox.Text);
            Database.UpdateDataIntoServerFinalCommandTable(serverNumber, finalCommand);
            UpdateLogLevel(serverNumber);
            MessageBox.Show("Please Stop and Start the Server to use the updated command.", "Restart Server", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            GoogleAnalytics.SendEvent("applyButton_Click");
            this.Close();
        }
        private void UpdateLogLevel(string serverNumber)
        {
            string selectedValue = null;
            if (infoRadioButton.Checked)
            {
                selectedValue = infoRadioButton.Text;
            }
            else if (errorRadioButton.Checked)
            {
                selectedValue = errorRadioButton.Text;
            }
            else if (debugRadioButton.Checked)
            {
                selectedValue = debugRadioButton.Text;
            }
            if (selectedValue != null)
            {
                Database.UpdateDataIntoLogLevelTable(serverNumber, selectedValue);
            }
            else
            {
                MessageBox.Show("Please select a log level.");
            }
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            ServerArgsRichTextBox.Text = "--allow-cors --allow-insecure=adb_shell";
            DefaultCapabilitiesRichTextBox.Text = "";
            FinalCommandRichTextBox.Text = $@"appium --port {portNumber} {ServerArgsRichTextBox.Text}" + $@" -dc ""{{""appium:webDriverAgentUrl"":""http://localhost:webDriverAgentProxyPort""}}""";
            GoogleAnalytics.SendEvent("resetButton_Click");
        }

        private void sessionCapabilityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessStartInfo psInfo = new ProcessStartInfo
                {
                    FileName = "https://appium.io/docs/en/latest/guides/caps/",
                    UseShellExecute = true
                };
                Process.Start(psInfo);
                GoogleAnalytics.SendEvent("sessionCapabilityToolStripMenuItem_Click");
            }
            catch (Exception exception)
            {
                GoogleAnalytics.SendEvent("sessionCapabilityToolStripMenuItem_Click", exception.Message);
            }
        }

        private void xCUITestCapabilityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessStartInfo psInfo = new ProcessStartInfo
                {
                    FileName = "https://appium.github.io/appium-xcuitest-driver/latest/reference/capabilities/",
                    UseShellExecute = true
                };
                Process.Start(psInfo);
                GoogleAnalytics.SendEvent("xCUITestCapabilityToolStripMenuItem_Click");
            }
            catch (Exception exception)
            {
                GoogleAnalytics.SendEvent("xCUITestCapabilityToolStripMenuItem_Click", exception.Message);
            }
        }

        private void uIAutomator2CapabilityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessStartInfo psInfo = new ProcessStartInfo
                {
                    FileName = "https://github.com/appium/appium-uiautomator2-driver?tab=readme-ov-file#capabilities",
                    UseShellExecute = true
                };
                Process.Start(psInfo);
                GoogleAnalytics.SendEvent("uIAutomator2CapabilityToolStripMenuItem_Click");
            }
            catch (Exception exception)
            {
                GoogleAnalytics.SendEvent("uIAutomator2CapabilityToolStripMenuItem_Click", exception.Message);
            }
        }

        private void DefaultCapsLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Point screenPoint = DefaultCapsLinkLabel.PointToScreen(new Point(0, DefaultCapsLinkLabel.Height));
            contextMenuStrip1.Show(screenPoint);
            GoogleAnalytics.SendEvent("DefaultCapsLinkLabel_LinkClicked");
        }

        private void debugRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (isFormInitialized && debugRadioButton.Checked)
            {
                MessageBox.Show("Setting log-level as \"Debug\" may cause the appium wizard to lag due to frequent appium server log updates.\n\n Set to \"Debug\" only if necessary; otherwise, set to \"Error\" or \"Info\".","Log-level = Debug",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
        }

        private void errorRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (isFormInitialized && errorRadioButton.Checked)
            {
                MessageBox.Show("Setting the log level to \"Error\" may restrict detailed information required for debugging issues, as only critical errors will be displayed. You may not even see the Server starting logs.", "Log-level = Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
