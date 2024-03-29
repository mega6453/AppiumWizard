using ICSharpCode.SharpZipLib.Zip;
using System.Diagnostics;
using System.Net;
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
            string currentPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User);
            if (!currentPath.Contains(path))
            {
                string newPath = currentPath + ";" + path;
                Environment.SetEnvironmentVariable("PATH", newPath, EnvironmentVariableTarget.User);
                string updatedPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User);
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

        public static bool IsLocalhostLoaded(string URL)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Timeout = 5000;
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Console.WriteLine("Web page loaded successfully.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while loading web page: " + ex.Message);
            }
            finally
            {
                response?.Close();
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
            string value = "";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);
            XmlNodeList keyNodes = xmlDoc.SelectNodes("//key");
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
            return keyValuePairs;
        }

        public static bool IsNodeInstalled()
        {
            try
            {
                string pathVariable = Environment.GetEnvironmentVariable("PATH");
                string[] paths = pathVariable.Split(';');
                foreach (string path in paths)
                {
                    string nodePath = System.IO.Path.Combine(path, "node.exe");
                    if (System.IO.File.Exists(nodePath))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsAppiumInstalled()
        {
            try
            {
                string appiumInstallationPath = @"C:\Users\" + Environment.UserName + @"\AppData\Roaming\npm";

                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.WorkingDirectory = appiumInstallationPath;
                process.StartInfo.Arguments = "/c appium --version";
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
            process.StartInfo.FileName = "cmd";
            process.StartInfo.Arguments = "/c npm i --location=global appium";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            process.WaitForExit();
        }

        public static void InstallXCUITestDriver()
        {
            string appiumInstallationPath = @"C:\Users\" + Environment.UserName + @"\AppData\Roaming\npm";

            Process process = new Process();
            process.StartInfo.FileName = "cmd";
            process.StartInfo.WorkingDirectory = appiumInstallationPath;
            process.StartInfo.Arguments = "/c appium driver install xcuitest";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            process.WaitForExit();
        }


        public static void InstallUIAutomatorDriver()
        {
            string appiumInstallationPath = @"C:\Users\" + Environment.UserName + @"\AppData\Roaming\npm";

            Process process = new Process();
            process.StartInfo.FileName = "cmd";
            process.StartInfo.WorkingDirectory = appiumInstallationPath;
            process.StartInfo.Arguments = "/c appium driver install uiautomator2";
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
                string appiumInstallationPath = @"C:\Users\" + Environment.UserName + @"\AppData\Roaming\npm";

                Process process = new Process();
                process.StartInfo.FileName = "cmd";
                process.StartInfo.WorkingDirectory = appiumInstallationPath;
                process.StartInfo.Arguments = "/c appium driver list";
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
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "cmd";
                process.StartInfo.Arguments = "/c winget install OpenJS.NodeJS.LTS --accept-package-agreements --accept-source-agreements";
                process.Start();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occurred while executing the command
                Console.WriteLine("An error occurred: " + ex.Message);
            }
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
    }
}
