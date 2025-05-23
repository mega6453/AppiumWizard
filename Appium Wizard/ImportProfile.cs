﻿using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using ListView = System.Windows.Forms.ListView;

namespace Appium_Wizard
{
    public partial class ImportProfile : Form
    {
        static string opensslFilePath = FilesPath.opensslFilePath;
        string p12InputFilePath = "";
        string mobileProvisionFilePath = "";
        string ProfilesFilePath = FilesPath.ProfilesFilePath;
        private int folderCounter;
        ListView listView1;

        public ImportProfile(ListView listView)
        {
            InitializeComponent();
            listView1 = listView;
            LoadCounter();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select .p12 File";
            openFileDialog.Filter = "P12 Files (*.p12)|*.p12";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                p12InputFilePath = openFileDialog.FileName;
                textBox1.Text = openFileDialog.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select .mobileprovision File";
            openFileDialog.Filter = "MobileProvision Files (*.mobileprovision)|*.mobileprovision";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                mobileProvisionFilePath = openFileDialog.FileName;
                textBox2.Text = openFileDialog.FileName;
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            //GoogleAnalytics.SendEvent("ImportProfileButton_Clicked");
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Owner = this;
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Import Profile", "Please wait while checking profile...", 10);
            try
            {
                string password = maskedTextBox1.Text;
                string command = $"pkcs12 -info -in \"{p12InputFilePath}\" -noout -passin pass:\"{password}\"";
                string output = string.Empty;
                await Task.Run(() =>
                {
                    output = ExecuteCommand(opensslFilePath, command, true);
                });
                commonProgress.UpdateStepLabel("Import Profile", "Please wait while checking profile...", 30);
                if (output.Contains("invalid password"))
                {
                    commonProgress.Close();
                    MessageBox.Show("Incorrect password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //GoogleAnalytics.SendEvent("iOS_Profile_Password_Wrong");
                }
                else if (output.Contains("MAC verified OK"))
                {
                    string certificate = string.Empty;
                    Dictionary<object, object> provisionDetails = new Dictionary<object, object>();
                    await Task.Run(() =>
                    {
                        certificate = GetCertificateFromP12File(p12InputFilePath, password).Replace("\n", "");
                        commonProgress.UpdateStepLabel("Import Profile", "Please wait while checking profile...", 50);
                        provisionDetails = GetDetailsFromProvisionFile(mobileProvisionFilePath);
                        commonProgress.UpdateStepLabel("Import Profile", "Please wait while checking profile...", 70);
                    });
                    string certificates = provisionDetails["DeveloperCertificates"].ToString();
                    if (!certificates.Contains(certificate))
                    {
                        commonProgress.Close();
                        MessageBox.Show("P12 file does not match with mobileprovision file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //GoogleAnalytics.SendEvent("iOS_P12_Provision_Not_Matching");
                    }
                    else
                    {
                        string folderPath = Path.Combine(ProfilesFilePath, "profile" + folderCounter + "\\");
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                            folderCounter++;
                            SaveCounter();
                        }
                        string p12fileName = Path.GetFileName(p12InputFilePath);
                        string mobileProvisionfileName = Path.GetFileName(mobileProvisionFilePath);
                        string p12DestinationPath = folderPath + p12fileName;
                        File.Copy(p12InputFilePath, p12DestinationPath, true);
                        string mobileProvisionDestinationPath = folderPath + mobileProvisionfileName;
                        File.Copy(mobileProvisionFilePath, mobileProvisionDestinationPath, true);

                        command = $"pkcs12 -in \"{p12InputFilePath}\" -out \"{folderPath + "certificate.pem"}\" -nodes -password pass:\"{password}\"";
                        await Task.Run(() =>
                        {
                            ExecuteCommand(opensslFilePath, command, true);
                            commonProgress.UpdateStepLabel("Import Profile", "Please wait while checking profile...", 80);
                        });
                        string expiryDateFromPem = GetExpirationDateFromPemFile(folderPath + "certificate.pem");
                        int expirationDaysFromPem = ExpirationDays(expiryDateFromPem);
                        string profileName = provisionDetails["Name"].ToString() ?? "emptyProfileName";
                        string expiryDateFromProvision = provisionDetails["ExpirationDate"].ToString() ?? "emptyExpirationDate";
                        DateTimeOffset dateTimeOffset = DateTimeOffset.Parse(expiryDateFromProvision);
                        DateTime dateTimeInUtc = dateTimeOffset.UtcDateTime;
                        string expiryDateFromProvisionInGMT = dateTimeInUtc.ToString("MMM dd HH:mm:ss yyyy", CultureInfo.InvariantCulture);

                        int expirationDaysFromProvisionFile = ExpirationDays(expiryDateFromProvision);
                        string appId = provisionDetails["application-identifier"].ToString() ?? "emptyAppId";
                        string teamId = provisionDetails["com.apple.developer.team-identifier"].ToString() ?? "emptyTeamId";

                        if (expirationDaysFromPem <= 0 & expirationDaysFromProvisionFile <= 0)
                        {
                            MessageBox.Show("Import Failed - P12 and Mobile Provision files both are Expired.\n\nP12 File Expiry Date - " + expiryDateFromPem + "\nMobile Provision File Expiry Date - " + expiryDateFromProvisionInGMT, "Profiles Expired", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else if (expirationDaysFromPem <= 0)
                        {
                            MessageBox.Show("Import Failed - P12 File Expired.\n\nP12 File Expiry Date - " + expiryDateFromPem + "\nMobile Provision File Expiry Date - " + expiryDateFromProvisionInGMT, "Profiles Expired", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else if (expirationDaysFromProvisionFile <= 0)
                        {
                            MessageBox.Show("Import Failed - Mobile Provision file Expired.\n\nP12 File Expiry Date - " + expiryDateFromPem + "\nMobile Provision File Expiry Date - " + expiryDateFromProvisionInGMT, "Profiles Expired", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            commonProgress.UpdateStepLabel("Import Profile", "Please wait while importing profile...", 90);
                            string updatedExpirationDays = string.Empty;
                            if (expirationDaysFromPem < expirationDaysFromProvisionFile)
                            {
                                updatedExpirationDays = expirationDaysFromPem.ToString() + " days";
                            }
                            else
                            {
                                updatedExpirationDays = expirationDaysFromProvisionFile.ToString() + " days";
                            }
                            try
                            {
                                string[] item1 = { profileName, updatedExpirationDays, appId, teamId, folderPath };
                                listView1.Items.Add(new ListViewItem(item1));
                                commonProgress.Close();
                                Close();
                                MessageBox.Show(profileName + " Profile added successfully.", "Import Profile", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                listView1.Refresh();
                                GoogleAnalytics.SendEvent("iOS_Profile_Added_Successfully");
                            }
                            catch (Exception ex)
                            {
                                commonProgress.Close();
                                MessageBox.Show("Unhandled exception : \n" + ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        commonProgress.Close();
                    }
                }
                else
                {
                    commonProgress.Close();
                    MessageBox.Show("Unhandled exception : \n" + output, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    GoogleAnalytics.SendEvent("ImportProfileButton_Clicked_Unhandled", "Unhandled Scenario");
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Unhandled exception : \n" + exception, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                commonProgress.Close();
                GoogleAnalytics.SendExceptionEvent("ImportProfileButton_Clicked", exception.Message);
            }
        }

        public static string GetExpirationDateFromPemFile(string certificatePath)
        {
            string arguments = $"x509 -in \"{certificatePath}\" -noout -enddate";
            string output = ExecuteCommand(opensslFilePath, arguments, false);

            string pattern = @"notAfter=(\w{3} \d{1,2} \d{2}:\d{2}:\d{2} \d{4}) GMT";
            Match match = Regex.Match(output, pattern);

            if (match.Success)
            {
                string dateString = match.Groups[1].Value;
                DateTime date = DateTime.ParseExact(dateString, "MMM dd HH:mm:ss yyyy", CultureInfo.InvariantCulture);
                string formattedDate = date.ToString("MMM dd HH:mm:ss yyyy", CultureInfo.InvariantCulture);
                return formattedDate;
            }
            return "error";
        }

        private void LoadCounter()
        {
            folderCounter = Database.QueryDataFromProfileCounterTable();
        }
        private void SaveCounter()
        {
            Database.UpdateDataIntoProfileCounterTable(folderCounter);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Close();
            GoogleAnalytics.SendEvent("ImportProfile_Cancel_Clicked");
        }

        public static Dictionary<object, object> GetDetailsFromProvisionFile(string mobileProvisionFilePath)
        {
            Dictionary<object, object> keyValuePairs = new Dictionary<object, object>();
            string arguments = $"smime -inform der -verify -noverify -in \"{mobileProvisionFilePath}\"";
            string xmlOutput = ExecuteCommand(opensslFilePath, arguments, false);
            keyValuePairs = ParseXmlContent(xmlOutput);
            var udids = ExtractUDIDsFromXml(xmlOutput);
            keyValuePairs.Add("DevicesList", udids);
            return keyValuePairs;
        }



        public static string GetCertificateFromP12File(string p12File, string password)
        {
            try
            {
                string arguments = $"pkcs12 -info -in \"{p12File}\" -passin pass:\"{password}\" -passout pass:\"{password}\"";

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = opensslFilePath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                Process opensslProcess = new Process();
                opensslProcess.StartInfo = startInfo;
                opensslProcess.Start();
                string output = opensslProcess.StandardOutput.ReadToEnd();
                opensslProcess.WaitForExit();
                output = Common.GetCertificateText(output);
                return output;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while getting pem from p12", "Error in openssl", MessageBoxButtons.OK, MessageBoxIcon.Error);
                GoogleAnalytics.SendExceptionEvent("GetCertificateFromP12File", e.Message);
                return "exception";
            }
        }

        static string ExecuteCommand(string fileName, string arguments, bool returnErrorAlso)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                Process opensslProcess = new Process();
                opensslProcess.StartInfo = startInfo;
                opensslProcess.Start();

                string output = opensslProcess.StandardOutput.ReadToEnd();
                string error = opensslProcess.StandardError.ReadToEnd();
                opensslProcess.WaitForExit();

                Console.WriteLine("Output:");
                Console.WriteLine(output);
                Console.WriteLine("Error:");
                Console.WriteLine(error);
                if (returnErrorAlso)
                {
                    return output + " " + error;
                }
                else
                {
                    return output;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error while getting pem from p12", "Error in openssl", MessageBoxButtons.OK, MessageBoxIcon.Error);
                GoogleAnalytics.SendExceptionEvent("Error_while_getting_pem_from_p12", exception.Message);
                return "exception";
            }
        }

        public static Dictionary<object, object> ParseXmlContent(string xmlContent)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlContent);

            Dictionary<object, object> keyValuePairs = new Dictionary<object, object>();

            XmlNodeList keyNodes = xmlDoc.SelectNodes("//key");
            foreach (XmlNode keyNode in keyNodes)
            {
                XmlNode valueNode = keyNode.NextSibling;
                if (valueNode != null && valueNode.NodeType == XmlNodeType.Element)
                {
                    keyValuePairs.Add(keyNode.InnerText.Trim(), valueNode.InnerText.Trim());
                }
            }

            return keyValuePairs;
        }

        public static List<string> ExtractUDIDsFromXml(string xmlContent)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlContent);

            List<string> udidList = new List<string>();

            XmlNode provisionedDevicesNode = xmlDoc.SelectSingleNode("//key[text()='ProvisionedDevices']");
            if (provisionedDevicesNode != null && provisionedDevicesNode.NextSibling != null && provisionedDevicesNode.NextSibling.Name == "array")
            {
                XmlNodeList udidNodes = provisionedDevicesNode.NextSibling.SelectNodes("string");
                foreach (XmlNode udidNode in udidNodes)
                {
                    udidList.Add(udidNode.InnerText.Trim());
                }
            }
            return udidList;
        }

        public static int ExpirationDays(string expirationDateString)
        {
            DateTime expirationDate;
            int daysRemaining = 0;

            // List of possible date formats
            string[] dateFormats = new[]
            {
            "yyyy-MM-dd'T'HH:mm:ss'Z'", // ISO 8601 format
            "MMM dd HH:mm:ss yyyy",     // Example: "Nov 21 13:10:20 2025"
            "yyyy-MM-dd",               // Date only
            "MM/dd/yyyy",               // US date format
            "dd/MM/yyyy",               // European date format
            "dd-MM-yyyy",               // Common alternative format
            "yyyy/MM/dd",               // Another common format
            "MMM dd, yyyy",             // Example: "Nov 21, 2025"
            "dd MMM yyyy",              // Example: "21 Nov 2025"
            "yyyy.MM.dd",               // Dotted format
            // Add more formats as needed
        };

            if (DateTime.TryParseExact(expirationDateString, dateFormats,
                CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out expirationDate))
            {
                DateTime currentDate = DateTime.UtcNow;
                TimeSpan timeRemaining = expirationDate.Date - currentDate.Date;
                daysRemaining = (int)timeRemaining.TotalDays;
            }
            else
            {
                GoogleAnalytics.SendExceptionEvent("Profile_Failed_to_parse_expiration_date");
            }

            return daysRemaining;
        }

        private void ImportProfile_Shown(object sender, EventArgs e)
        {
           GoogleAnalytics.SendEvent("ImportProfile_Shown");
        }

        private void ImportProfile_HelpButtonClicked(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBox.Show("For iOS automation, you need to add valid profiles here. Please contact your iOS developer to get the profiles for your device UDID.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
           GoogleAnalytics.SendEvent("ImportProfile_HelpButtonClicked");
        }

        private void ImportProfile_Load(object sender, EventArgs e)
        {

        }
    }
}
