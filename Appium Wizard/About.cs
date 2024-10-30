using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Appium_Wizard
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
        }

        private async void About_Load(object sender, EventArgs e)
        {
            label1.Text = "Version : " + VersionInfo.VersionNumber;
           
            try
            {
                string MITLicense = await AboutText.GetLicenseText("MIT");
                string GPLLicense = await AboutText.GetLicenseText("GPL");
                string linebreak = "\n\n--------------------------------------------------------------------------\n\n";
                string finalLicense = MITLicense + linebreak + GPLLicense;
                LicenseRichTextBox.Text = finalLicense;
                var thanksTo = await AboutText.ExtractSections();
                ThanksToRichTextBox.Text = thanksTo;
            }
            catch (Exception)
            {
                LicenseRichTextBox.Text = "https://github.com/mega6453/AppiumWizard/blob/master/LICENSE-MIT\n" +
                                          "https://github.com/mega6453/AppiumWizard/blob/master/LICENSE-GPL";
                ThanksToRichTextBox.Text = "https://github.com/mega6453/AppiumWizard/blob/master/README.md#thanks-to";
            }
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
            GoogleAnalytics.SendEvent("About_Shown");
        }
    }

    public class AboutText
    {
        private static readonly HttpClient client = new HttpClient();
        public static async Task<string> GetReadmeContent()
        {
            try
            {
                string url = $"https://api.github.com/repos/mega6453/AppiumWizard/readme";
                client.DefaultRequestHeaders.UserAgent.TryParseAdd("request");

                var response = await client.GetStringAsync(url);
                var json = JObject.Parse(response);
                string contentBase64 = json["content"]?.ToString() ?? string.Empty;
                byte[] data = Convert.FromBase64String(contentBase64);
                string decodedString = System.Text.Encoding.UTF8.GetString(data);
                return decodedString;
            }
            catch (Exception)
            {
                return "https://github.com/mega6453/AppiumWizard?tab=readme-ov-file#thanks-to";
            }
        }

        public async static Task<string> ExtractSections()
        {
            string readmeContent = await GetReadmeContent();

            string thanksToPattern = @"## Thanks To\s*(.*?)\s*(##|$)";
            string iconsPattern = @"## Icons\s*(.*?)\s*(##|$)";

            string thanksToSection = ExtractSection(readmeContent, thanksToPattern);
            string iconsSection = ExtractSection(readmeContent, iconsPattern);

            if (thanksToSection.Contains("Section not found") | iconsSection.Contains("Section not found"))
            {
                return "https://github.com/mega6453/AppiumWizard/blob/master/README.md#thanks-to";
            }
            string linebreak = "\n\n-----------------------------------------------------------------------";
            return "Executables:\n"+thanksToSection + linebreak + "\n\nICONS:\n" + iconsSection;
        }

        private static string ExtractSection(string content, string pattern)
        {
            var match = Regex.Match(content, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return match.Groups[1].Value.Trim();
            }
            else
            {
                return "Section not found";
            }
        }

        public static async Task<string> GetLicenseText(string licenseType)
        {
            string url = "https://raw.githubusercontent.com/mega6453/AppiumWizard/refs/heads/master/LICENSE-"+licenseType;
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }
                catch (HttpRequestException)
                {
                    return url;
                }
            }
        }
    }
}
