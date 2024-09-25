using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Diagnostics;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Appium_Wizard
{
    public class Common
    {
        private static string executablesFolderPath = FilesPath.executablesFolderPath;
        private static string serverFolderPath = FilesPath.serverInstalledPath;
        private static string nodeFilePath = FilesPath.nodeZipPath;
        public static void TerminateProcess(string command)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c " + command,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                };
                Process process = new Process();
                process.StartInfo = startInfo;
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                Console.WriteLine(output);

                process.WaitForExit();
                process.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while executing the command: " + ex.Message);
            }
        }


        public static void SetEnvironmentVariable(string path)
        {
            string currentPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User) ?? string.Empty;
            if (!currentPath.Contains(path))
            {
                string newPath = currentPath + ";" + path;
                Environment.SetEnvironmentVariable("PATH", newPath, EnvironmentVariableTarget.User);
                string updatedPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User) ?? string.Empty;
                Console.WriteLine("Updated PATH: " + updatedPath);
            }
        }

        public static void SetAndroidHomeEnvironmentVariable()
        {
            Environment.SetEnvironmentVariable("ANDROID_HOME", executablesFolderPath, EnvironmentVariableTarget.User);
        }

        public static int GetFreePort()
        {
            // Create a temporary TCP listener
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();

            // Retrieve the assigned port
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;

            // Stop and dispose of the listener
            listener.Stop();
            listener.Server.Dispose();

            return port;
        }

        public static int GetFreePort(int minPort, int maxPort)
        {
            // Iterate through the range of ports and try to create a temporary TCP listener on each one
            for (int port = minPort; port <= maxPort; port++)
            {
                try
                {
                    var listener = new TcpListener(IPAddress.Loopback, port);
                    listener.Start();
                    listener.Stop();
                    return port;
                }
                catch (SocketException)
                {
                    // Port is already in use, continue to next port
                    continue;
                }
            }

            // No free port was found in the specified range
            throw new Exception($"No free port was found in the range {minPort}-{maxPort}");
        }

        public static bool IsPortBeingUsed(int port)
        {
            try
            {
                using (TcpClient tcpClient = new TcpClient())
                {
                    tcpClient.Connect("localhost", port);
                    return true;
                }
            }
            catch (SocketException)
            {

            }
            return false;
        }

        public static void KillProcessByPortNumber(int portNumber)
        {
            var output = RunNetstatAndFindProcessByPort(portNumber);
            KillProcessById(output.Item1);
        }

        public static (int, string) RunNetstatAndFindProcessByPort(int portNumber)
        {
            Process netstatProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/C netstat -ano | findstr \"LISTENING\" | findstr \":" + portNumber,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            netstatProcess.Start();
            string netstatOutput = netstatProcess.StandardOutput.ReadToEnd();
            netstatProcess.WaitForExit();

            Regex regex = new Regex(@"\sLISTENING\s+(\d+)");
            Match match = regex.Match(netstatOutput);

            if (match.Success)
            {
                string processIdStr = match.Groups[1].Value;
                int processId = int.Parse(processIdStr);
                Process process = Process.GetProcessById(processId);
                Console.WriteLine($"Process Name: {process.ProcessName}, Process ID: {process.Id}");
                return (process.Id, process.ProcessName);
            }
            else
            {
                Console.WriteLine("No process ID found in the netstat output.");
                return (0, "");
            }

        }

        public static void KillProcessById(int processId)
        {
            try
            {
                Process process = Process.GetProcessById(processId);

                if (!process.HasExited)
                {
                    process.Kill();
                }
                else
                {
                    Console.WriteLine("Process with ID " + processId + " is already terminated.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Process with ID " + processId + " not found: " + ex.Message);
            }
        }

        public static async Task<bool> IsLocalhostLoaded(string URL)
        {
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMilliseconds(5000);
                try
                {
                    HttpResponseMessage response = await client.GetAsync(URL);
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Web page loaded successfully.");
                        return true;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }

        public static string ExtractInfoPlistFromIPA(string ipaFilePath, string outputFolderPath)
        {
            string appName = ""; int count = 1; string outputPath = "";
            using (ZipFile zipFile = new ZipFile(ipaFilePath))
            {
                foreach (ZipEntry entry in zipFile)
                {
                    if (entry.Name.Contains(".app") && count == 1)
                    {
                        appName = entry.Name;
                        count++;
                    }
                    if (entry.Name == appName + "Info.plist")
                    {
                        outputPath = Path.Combine(outputFolderPath, "Info.plist");
                        using (Stream inputStream = zipFile.GetInputStream(entry))
                        using (FileStream outputStream = new FileStream(outputPath, FileMode.Create))
                        {
                            inputStream.CopyTo(outputStream);
                        }
                        break;
                    }
                }
            }
            return outputPath;
        }


        public static Dictionary<string, string> GetValueFromXml(string xmlString)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);
            XmlNodeList keyNodes = xmlDoc.SelectNodes("//key");
            if (keyNodes != null)
            {
                foreach (XmlNode keyNode in keyNodes)
                {
                    XmlNode valueNode = keyNode.NextSibling;
                    if (valueNode != null && valueNode.Name == "string")
                    {
                        if (!keyValuePairs.ContainsKey(keyNode.InnerText))
                        {
                            keyValuePairs.Add(keyNode.InnerText, valueNode.InnerText);
                        }
                    }
                }
            }
          
            return keyValuePairs;
        }

        public static bool IsNodeInstalled()
        {
            string nodePath = Path.Combine(serverFolderPath, "node.exe");
            if (File.Exists(nodePath))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsAppiumInstalled()
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.WorkingDirectory = serverFolderPath;
                process.StartInfo.Arguments = "/C appium --version";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();
                if (error.Contains("'appium' is not recognized as an internal or external command") |
                    error.Contains("throw err"))
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void InstallAppiumGlobally()
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            string pathVariable = Environment.GetEnvironmentVariable("PATH");
            pathVariable += ";" + serverFolderPath;
            process.StartInfo.EnvironmentVariables["PATH"] = pathVariable;
            process.StartInfo.WorkingDirectory = serverFolderPath;
            process.StartInfo.Arguments = $"/C npm i -g appium";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            process.WaitForExit();
        }

        public static void InstallXCUITestDriver()
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd";
            string pathVariable = Environment.GetEnvironmentVariable("PATH");
            pathVariable += ";" + serverFolderPath;
            process.StartInfo.EnvironmentVariables["PATH"] = pathVariable;

            process.StartInfo.Arguments = "/C appium driver install xcuitest";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            process.WaitForExit();
        }

        public static void UpdateXCUITestDriver()
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd";
            string pathVariable = Environment.GetEnvironmentVariable("PATH");
            pathVariable += ";" + serverFolderPath;
            process.StartInfo.EnvironmentVariables["PATH"] = pathVariable;

            process.StartInfo.Arguments = "/C appium driver update xcuitest";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            process.WaitForExit();
        }


        public static void InstallUIAutomatorDriver()
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd";
            string pathVariable = Environment.GetEnvironmentVariable("PATH");
            pathVariable += ";" + serverFolderPath;
            process.StartInfo.EnvironmentVariables["PATH"] = pathVariable;

            process.StartInfo.Arguments = "/C appium driver install uiautomator2";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            process.WaitForExit();
        }

        public static void UpdateUIAutomatorDriver()
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd";
            string pathVariable = Environment.GetEnvironmentVariable("PATH");
            pathVariable += ";" + serverFolderPath;
            process.StartInfo.EnvironmentVariables["PATH"] = pathVariable;

            process.StartInfo.Arguments = "/C appium driver update uiautomator2";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            process.WaitForExit();
        }


        public static string AppiumInstalledDriverList()
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "cmd";
                process.StartInfo.WorkingDirectory = serverFolderPath;
                process.StartInfo.Arguments = "/C appium driver list";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();
                process.WaitForExit();
                output = Regex.Replace(output, @"\x1B\[[^@-~]*[@-~]", string.Empty);
                error = Regex.Replace(error, @"\x1B\[[^@-~]*[@-~]", string.Empty);

                return output + error;

            }
            catch (Exception)
            {
                return "exception";
            }
        }

        public static Dictionary<string,string> InstalledDriverVersion()
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();    
            var output = AppiumInstalledDriverList();
            string pattern = @"- (\w+)@(\d+(\.\d+){0,2})";
            Regex regex = new Regex(pattern);
            MatchCollection matches = regex.Matches(output);
            foreach (Match match in matches)
            {
                string driverName = match.Groups[1].Value;
                string version = match.Groups[2].Value;
                keyValuePairs.Add(driverName,version);
            }
            return keyValuePairs;
        }

        public static string InstalledAppiumServerVersion()
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.WorkingDirectory = serverFolderPath;
                process.StartInfo.Arguments = "/C appium --version";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();
                return output + error;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string AvailableAppiumVersion()
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.WorkingDirectory = serverFolderPath;
                process.StartInfo.Arguments = "/C npm show appium version";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();
                return output + error;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string AvailableXCUITestVersion()
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.WorkingDirectory = serverFolderPath;
                process.StartInfo.Arguments = "/C npm show appium-xcuitest-driver version";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();
                return output + error;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string AvailableUIAutomatorVersion()
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.WorkingDirectory = serverFolderPath;
                process.StartInfo.Arguments = "/C npm show appium-uiautomator2-driver version";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();
                return output + error;
            }
            catch (Exception)
            {
                return "";
            }
        }

        const int SM_REBOOTREQUIRED = 0x42;
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetSystemMetrics(int nIndex);

        public static bool isRebootRequired()
        {
            if (GetSystemMetrics(SM_REBOOTREQUIRED) != 0)
            {
                Console.WriteLine("System requires a reboot.");
                return true;
            }
            else
            {
                Console.WriteLine("System does not require a reboot.");
                return false;
            }
        }


        public static void InstallNodeJs()
        {
            Process process = new Process();
            process.StartInfo.FileName = FilesPath.zipExtractorFilePath;
            process.StartInfo.Arguments = $"x \"{nodeFilePath}\" -o\"{serverFolderPath}\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
        }

        public static string WSLHelp()
        {
            string output;
            using (var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "wsl.exe",
                    Arguments = "--help",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.Unicode,
                }
            })
            {
                proc.Start();
                output = proc.StandardOutput.ReadToEnd();
            }
            return output;
            // WSL is finishing an upgrade...
            //Update failed(exit code: 1603).
            //Error code: Wsl / CallMsi / E_ABORT
        }

        public static string WSLList()
        {
            string output;
            using (var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "wsl.exe",
                    Arguments = "--list",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.Unicode,
                }
            })
            {
                proc.Start();
                output = proc.StandardOutput.ReadToEnd();
            }
            return output;
        }

        public static bool IsWSLImportInPlaceSupported()
        {
            return WSLHelp().Contains("--import-in-place");
        }

        public static bool InstallWSL()
        {
            string output = string.Empty;
            using (var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "wsl.exe",
                    Arguments = "--install --no-distribution",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.Unicode,
                }
            })
            {
                proc.Start();
                output = proc.StandardOutput.ReadToEnd();
            }
            return output.Contains("The operation completed successfully");
        }

        public static string RegisterWSLDistro()
        {
            string distroPath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\Resources\\iOS\\AppiumWizardDistro.vhdx";

            string output = string.Empty;
            using (var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "wsl.exe",
                    Arguments = $"--import-in-place AppiumWizardDistro \"{distroPath}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.Unicode,
                }
            })
            {
                proc.Start();
                output = proc.StandardOutput.ReadToEnd();
            }
            return output;
            //return output.Contains("The operation completed successfully") | output.Contains("A distribution with the supplied name already exists");
        }

        public static string GetCertificateText(string input)
        {
            string pattern = @"-----BEGIN CERTIFICATE-----\r?\n?(.*?)\r?\n?-----END CERTIFICATE-----";

            Match match = Regex.Match(input, pattern, RegexOptions.Singleline);
            string certificateText = "No certificate found";
            if (match.Success)
            {
                certificateText = match.Groups[1].Value;
                Console.WriteLine(certificateText);
            }
            return certificateText;
        }

        public static string GetTextBetween(string STR, string FirstString, string LastString)
        {
            string FinalString;
            int Pos1 = STR.IndexOf(FirstString) + FirstString.Length;
            int Pos2 = STR.IndexOf(LastString);
            FinalString = STR.Substring(Pos1, Pos2 - Pos1);
            return FinalString.Trim();
        }

        public static string GetDuration(DateTime fromTime, DateTime endTime)
        {
            var duration = endTime - fromTime;
            if (duration.TotalSeconds < 60)
            {
                return Math.Round(duration.TotalSeconds).ToString() + " sec";
            }
            else
            {
                return Math.Round(duration.TotalMinutes).ToString() + " min";
            }
        }

        private static string GetIPAddress()
        {
            try
            {
                foreach (NetworkInterface netInterface in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 &&
                        netInterface.OperationalStatus == OperationalStatus.Up)
                    {
                        IPInterfaceProperties ipProps = netInterface.GetIPProperties();
                        foreach (UnicastIPAddressInformation addr in ipProps.UnicastAddresses)
                        {
                            if (addr.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                return addr.Address.ToString();
                            }
                        }
                    }
                }
                return "NoValidAddress";
            }
            catch (Exception)
            {
                return "NoValidAddress";
            }
        }

        public static string GetOnlyNetworkPortion()
        {
            try
            {
                string ip = GetIPAddress();
                string[] octets = ip.Split('.');
                string network = string.Join(".", octets.Take(3));
                return network;
            }
            catch (Exception)
            {
                return "NotValid";  
            }
        }

        public static Dictionary<string, string> GetLatestReleaseInfo()
        {
            Dictionary<string, string> responseDictionary = new Dictionary<string, string>();
            try
            {
                var options = new RestClientOptions("https://api.github.com")
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest("/repos/mega6453/AppiumWizard/releases/latest", Method.Get);
                RestResponse response = client.Execute(request);
                var responseObject = JObject.Parse(response.Content);
                foreach (var property in responseObject.Properties())
                {
                    responseDictionary.Add(property.Name, property.Value.ToString());
                }
                return responseDictionary;
            }
            catch (Exception)
            {
                return responseDictionary;
            }
        }

        public static bool isInternetAvailable()
        {
            try
            {
                using (var ping = new Ping())
                {
                    var reply = ping.Send("www.google.com");
                    return reply.Status == IPStatus.Success;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool isValidIPAddress(string ipAddressString)
        {
            bool isValidIpAddress = Regex.IsMatch(ipAddressString, @"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$") && IPAddress.TryParse(ipAddressString, out IPAddress ipAddress);
            return isValidIpAddress;
        }

        public static bool isProcessIdExist(int processId)
        {
            Process[] processes = Process.GetProcesses();
            bool isRunning = false;

            foreach (Process process in processes)
            {
                if (process.Id == processId)
                {
                    isRunning = true;
                    break;
                }
            }
            if (isRunning)
            {
                Process process = Process.GetProcessById(processId);
                if (process.ProcessName.Equals("pymobiledevice3", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static int GetChildProcessId(int parentId, string processName)
        {
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher($"SELECT * FROM Win32_Process WHERE ParentProcessID={parentId}"))
            {
                foreach (ManagementObject process in searcher.Get())
                {
                    if (process["Name"].ToString().ToLower() == processName.ToLower())
                    {
                        return Convert.ToInt32(process["ProcessId"]);
                    }
                }
            }
            return -1; // Not found
        }
    }
}
