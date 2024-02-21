using System.Diagnostics;
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

        private void button3_Click(object sender, EventArgs e)
        {
            string password = maskedTextBox1.Text;
            string command = $"pkcs12 -info -in \"{p12InputFilePath}\" -noout -passin pass:\"{password}\"";
            string output = ExecuteCommand(opensslFilePath, command, true);
            if (output.Contains("invalid password"))
            {
                MessageBox.Show("Incorrect password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (output.Contains("MAC verified OK"))
            {
                string certificate = GetCertificateFromP12File(p12InputFilePath, password).Replace("\n","");
                var provisionDetails = GetDetailsFromProvisionFile(mobileProvisionFilePath);
                string certificates = provisionDetails["DeveloperCertificates"].ToString();
                if (!certificates.Contains(certificate))
                {
                    MessageBox.Show("P12 file does not match with mobileprovision file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    ExecuteCommand(opensslFilePath, command, true);

                    string profileName = provisionDetails["Name"].ToString();
                    int expirationDays = ExpirationDays(provisionDetails["ExpirationDate"].ToString());
                    string appId = provisionDetails["application-identifier"].ToString();
                    string teamId = provisionDetails["com.apple.developer.team-identifier"].ToString();
                    string updatedExpirationDays = expirationDays.ToString() + " days";
                    try
                    {
                        if (expirationDays <= 0)
                        {
                            updatedExpirationDays = "Expired";
                        }
                        string[] item1 = { profileName, updatedExpirationDays, appId, teamId, folderPath };
                        listView1.Items.Add(new ListViewItem(item1));
                        Close();
                        MessageBox.Show(profileName + " Profile added successfully.", "Import Profile", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        listView1.Refresh();
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            else
            {
                MessageBox.Show("Unhandled exception : \n" + output, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            output = Common.GetTextBetween(output);
            return output;
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
            catch (Exception)
            {
                MessageBox.Show("Error while getting pem from p12", "Error in openssl", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            if (DateTime.TryParseExact(expirationDateString, "yyyy-MM-dd'T'HH:mm:ss'Z'",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.AssumeUniversal, out expirationDate))
            {
                DateTime currentDate = DateTime.Now;
                TimeSpan timeRemaining = expirationDate.Date - currentDate.Date;
                daysRemaining = (int)timeRemaining.TotalDays;

                Console.WriteLine("Days remaining: " + daysRemaining);
            }
            else
            {
                Console.WriteLine("Failed to parse expiration date.");
            }
            return daysRemaining;
        }
    }
}
