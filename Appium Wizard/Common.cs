using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using RestSharp;
using System.Diagnostics;
using System.IO.Compression;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml;
using Windows.Devices.AllJoyn;

namespace Appium_Wizard
{
    public class Common
    {
        private static string executablesFolderPath = FilesPath.executablesFolderPath;
        private static string serverFolderPath = FilesPath.serverInstalledPath;
        private static string nodeFilePath = FilesPath.nodeZipPath;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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
            using (ICSharpCode.SharpZipLib.Zip.ZipFile zipFile = new ICSharpCode.SharpZipLib.Zip.ZipFile(ipaFilePath))
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

        public static string ExtractInfoPlistFromWDAIPA(string infoPlistPath, string outputFolderPath)
        {
            string wdaPath = FilesPath.WDAFilePath;
            string destinationPath = "";
            using (ZipArchive archive = System.IO.Compression.ZipFile.OpenRead(wdaPath))
            {
                ZipArchiveEntry entry = archive.GetEntry(infoPlistPath);

                if (entry != null)
                {
                    destinationPath = Path.Combine(outputFolderPath, Path.GetFileName(infoPlistPath));
                    entry.ExtractToFile(destinationPath, overwrite: true);
                }
            }
            return destinationPath;
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

        public static void InstallAppiumGlobally(bool showExecution = false)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                string pathVariable = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
                pathVariable += ";" + serverFolderPath;
                process.StartInfo.EnvironmentVariables["PATH"] = pathVariable;
                process.StartInfo.WorkingDirectory = serverFolderPath;
                if (showExecution)
                {
                    process.StartInfo.Arguments = $"/K npm i -g appium";
                    process.StartInfo.CreateNoWindow = false;
                }
                else
                {
                    process.StartInfo.Arguments = $"/C npm i -g appium";
                    process.StartInfo.CreateNoWindow = true;
                }
                process.StartInfo.UseShellExecute = false;
                process.Start();
                process.WaitForExit();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while installing appium. \nOringal exception:" + e.Message, "Install Appium", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void UninstallAppium(bool showExecution = false)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                string pathVariable = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
                pathVariable += ";" + serverFolderPath;
                process.StartInfo.EnvironmentVariables["PATH"] = pathVariable;
                process.StartInfo.WorkingDirectory = serverFolderPath;
                process.StartInfo.UseShellExecute = false;
                if (showExecution)
                {
                    // Uninstall Appium
                    process.StartInfo.Arguments = "/K npm uninstall -g appium";
                    process.StartInfo.CreateNoWindow = false;
                }
                else
                {
                    // Uninstall Appium
                    process.StartInfo.Arguments = "/C npm uninstall -g appium";
                    process.StartInfo.CreateNoWindow = true;
                }
                process.Start();
                process.WaitForExit();

                // Clear npm cache
                process.StartInfo.Arguments = "/C npm cache clean --force";
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                process.WaitForExit();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while uninstalling appium. \nOringal exception:" + e.Message, "Uninstall Appium", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void InstallXCUITestDriver(bool showExecution = false)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "cmd";
                string pathVariable = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
                pathVariable += ";" + serverFolderPath;
                process.StartInfo.EnvironmentVariables["PATH"] = pathVariable;

                if (showExecution)
                {
                    process.StartInfo.Arguments = "/K appium driver install xcuitest";
                    process.StartInfo.CreateNoWindow = false;
                }
                else
                {
                    process.StartInfo.Arguments = "/C appium driver install xcuitest";
                    process.StartInfo.CreateNoWindow = true;
                }
                process.StartInfo.UseShellExecute = false;
                process.Start();
                process.WaitForExit();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while installing xcuitest driver. \nOriginal exception: " + e.Message, "Install xcuitest driver", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void UninstallXCUITestDriver(bool showExecution = false)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "cmd";
                string pathVariable = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
                pathVariable += ";" + serverFolderPath;
                process.StartInfo.EnvironmentVariables["PATH"] = pathVariable;

                if (showExecution)
                {
                    process.StartInfo.Arguments = "/K appium driver uninstall xcuitest";
                    process.StartInfo.CreateNoWindow = false;
                }
                else
                {
                    process.StartInfo.Arguments = "/C appium driver uninstall xcuitest";
                    process.StartInfo.CreateNoWindow = true;
                }
                process.StartInfo.UseShellExecute = false;
                process.Start();
                process.WaitForExit();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while uninstalling xcuitest driver. \nOriginal exception: " + e.Message, "Uninstall xcuitest driver", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        public static void UpdateXCUITestDriver(bool showExecution = false, bool unsafeUpdate = false)
        {
            try
            {
                Process process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd",
                        UseShellExecute = false,
                        CreateNoWindow = !showExecution
                    }
                };

                string pathVariable = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
                pathVariable += ";" + serverFolderPath;
                process.StartInfo.EnvironmentVariables["PATH"] = pathVariable;

                string command = "appium driver update xcuitest";
                if (unsafeUpdate)
                {
                    command += " --unsafe";
                }

                process.StartInfo.Arguments = (showExecution ? "/K " : "/C ") + command;

                process.Start();
                process.WaitForExit();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while updating xcuitest driver. \nOriginal exception: " + e.Message, "Update xcuitest driver", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        public static void InstallUIAutomatorDriver(bool showExecution = false)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "cmd";
                string pathVariable = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
                pathVariable += ";" + serverFolderPath;
                process.StartInfo.EnvironmentVariables["PATH"] = pathVariable;
                if (showExecution)
                {
                    process.StartInfo.Arguments = "/K appium driver install uiautomator2";
                    process.StartInfo.CreateNoWindow = false;
                }
                else
                {
                    process.StartInfo.Arguments = "/C appium driver install uiautomator2";
                    process.StartInfo.CreateNoWindow = true;
                }
                process.StartInfo.UseShellExecute = false;
                process.Start();
                process.WaitForExit();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while installing uiautomator2 driver. \nOriginal exception: " + e.Message, "Install UiAutomator2 driver", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void UninstallUIAutomatorDriver(bool showExecution = false)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "cmd";
                string pathVariable = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
                pathVariable += ";" + serverFolderPath;
                process.StartInfo.EnvironmentVariables["PATH"] = pathVariable;
                if (showExecution)
                {
                    process.StartInfo.Arguments = "/K appium driver uninstall uiautomator2";
                    process.StartInfo.CreateNoWindow = false;
                }
                else
                {
                    process.StartInfo.Arguments = "/C appium driver uninstall uiautomator2";
                    process.StartInfo.CreateNoWindow = true;
                }
                process.StartInfo.UseShellExecute = false;
                process.Start();
                process.WaitForExit();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while uninstalling uiautomator2 driver. \nOriginal exception: " + e.Message, "Uninstall UiAutomator2 driver", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void UpdateUIAutomatorDriver(bool showExecution = false, bool unsafeUpdate = false)
        {
            try
            {
                Process process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd",
                        UseShellExecute = false,
                        CreateNoWindow = !showExecution
                    }
                };

                string pathVariable = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
                pathVariable += ";" + serverFolderPath;
                process.StartInfo.EnvironmentVariables["PATH"] = pathVariable;

                string command = "appium driver update uiautomator2";
                if (unsafeUpdate)
                {
                    command += " --unsafe";
                }

                process.StartInfo.Arguments = (showExecution ? "/K " : "/C ") + command;
                process.Start();
                process.WaitForExit();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while updating UIAutomator2 driver. \nOriginal exception: " + e.Message, "Update UIAutomator2 driver", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        public static Dictionary<string, string> InstalledDriverVersion()
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
                keyValuePairs.Add(driverName, version);
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
                return "NA";
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
                return "NA";
            }
        }

        public static Dictionary<string, string> GetListOfInstalledPlugins()
        {
            Dictionary<string, string> plugins = new Dictionary<string, string>();
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.WorkingDirectory = serverFolderPath;
                process.StartInfo.Arguments = "/C appium plugin list";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();
                output += error;
                string cleanedOutput = Regex.Replace(output, @"\x1B\[[0-9;]*[a-zA-Z]", "");
                string[] lines = cleanedOutput.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in lines)
                {
                    var match = Regex.Match(line, @"- ([a-zA-Z0-9\-]+)(?:@([\d.]+))? \[.*\]");
                    if (match.Success)
                    {
                        string pluginName = match.Groups[1].Value;
                        string version = match.Groups[2].Success ? match.Groups[2].Value : "NotInstalled";
                        plugins[pluginName] = version;
                    }
                }
                return plugins;
            }
            catch (Exception)
            {
                return plugins;
            }
        }

        public static void InstallPlugin(string plugin, string installOrUpdate, bool ShowWindow)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "cmd";
                string pathVariable = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
                pathVariable += ";" + serverFolderPath;
                process.StartInfo.EnvironmentVariables["PATH"] = pathVariable;
                if (ShowWindow)
                {
                    process.StartInfo.Arguments = "/K appium plugin " + installOrUpdate + " " + plugin;
                    process.StartInfo.UseShellExecute = false; // Use the shell to execute the command
                    process.StartInfo.CreateNoWindow = false; // Show the window
                }
                else
                {
                    process.StartInfo.Arguments = "/C appium plugin " + installOrUpdate + " " + plugin;
                    process.StartInfo.CreateNoWindow = true; // Hide the window
                }
                process.Start();
                process.WaitForExit();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while installing " + plugin + " plugin. \nOriginal exception: " + e.Message, "Install Plugin", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void UninstallPlugin(string plugin, bool ShowWindow)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "cmd";
                string pathVariable = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
                pathVariable += ";" + serverFolderPath;
                process.StartInfo.EnvironmentVariables["PATH"] = pathVariable;
                process.StartInfo.UseShellExecute = false; // Use the shell to execute the command
                if (ShowWindow)
                {
                    process.StartInfo.Arguments = "/K appium plugin uninstall " + plugin;
                    process.StartInfo.CreateNoWindow = false; // Show the window
                }
                else
                {
                    process.StartInfo.Arguments = "/C appium plugin uninstall " + plugin;
                    process.StartInfo.CreateNoWindow = true; // Hide the window
                }
                process.Start();
                process.WaitForExit();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while uninstalling " + plugin + " plugin. \nOriginal exception: " + e.Message, "Uninstall Plugin", MessageBoxButtons.OK, MessageBoxIcon.Error);
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


        public static void InstallNodeJs(bool showExecution = false)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = FilesPath.zipExtractorFilePath;
                process.StartInfo.Arguments = $"x \"{nodeFilePath}\" -o\"{serverFolderPath}\" -y";
                process.StartInfo.UseShellExecute = false;
                if (showExecution)
                {
                    process.StartInfo.CreateNoWindow = false;
                }
                else
                {
                    process.StartInfo.CreateNoWindow = true;
                }
                process.Start();
                process.WaitForExit();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while installing NodeJs. \nOriginal exception: " + e.Message, "Install NodeJs", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        public static string GetIPAddress()
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

        public static string GetRequiredWebDriverAgentVersion()
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @".appium\node_modules\appium-xcuitest-driver\node_modules\appium-webdriveragent\package.json");
            if (!File.Exists(filePath))
            {
                return "fileNotFound";
            }

            string jsonContent = File.ReadAllText(filePath);

            using (JsonDocument doc = JsonDocument.Parse(jsonContent))
            {
                JsonElement root = doc.RootElement;

                if (root.TryGetProperty("version", out JsonElement versionElement))
                {
                    return versionElement.GetString();
                }
                else
                {
                    return "versionNotFound";
                }
            }
        }

        public static void GetWebDriverAgentIPAFile()
        {
            string version = GetRequiredWebDriverAgentVersion();
            if (version != "versionNotFound" & version != "fileNotFound")
            {
                try
                {
                    string url = "https://github.com/appium/WebDriverAgent/releases/download/v" + version + "/WebDriverAgentRunner-Runner.zip";
                    string tempFolder = Path.GetTempPath();
                    tempFolder = Path.Combine(tempFolder, "Appium_Wizard");
                    try
                    {
                        Directory.Delete(tempFolder, true);
                    }
                    catch (Exception)
                    {
                    }
                    Directory.CreateDirectory(tempFolder);
                    string downloadedZip = tempFolder + "\\wdadownloaded.zip";
                    using (var client = new HttpClient())
                    {
                        using (var s = client.GetStreamAsync(url))
                        {
                            using (var fs = new FileStream(downloadedZip, FileMode.OpenOrCreate))
                            {
                                s.Result.CopyTo(fs);
                            }
                        }
                    }


                    //--------------------
                    string extractFolder = tempFolder + "\\Extracted";
                    string PayloadFolder = tempFolder + "\\Extracted\\Payload";
                    string finalIPAFile = tempFolder + "\\wda.ipa";
                    System.IO.Compression.ZipFile.ExtractToDirectory(downloadedZip, PayloadFolder);
                    UpdateVersionInPlistFile(PayloadFolder + "\\WebDriverAgentRunner-Runner.app\\Info.plist", version);
                    System.IO.Compression.ZipFile.CreateFromDirectory(extractFolder, finalIPAFile);
                    //--------------------
                    string destinationFilePath = FilesPath.iOSFilesPath + "wda.ipa";
                    File.Delete(destinationFilePath);
                    File.Move(finalIPAFile, destinationFilePath, true);
                    //--------------------
                    Task.Run(() =>
                    {
                        try
                        {
                            Directory.Delete(tempFolder, true);
                        }
                        catch (Exception)
                        {
                        }
                    });
                }
                catch (Exception)
                {
                }
            }
        }

        public static void UpdateVersionInPlistFile(string filePath, string version)
        {
            string plistContent = File.ReadAllText(filePath);
            string updatedPlistContent = plistContent.Replace("<string>1.0</string>", "<string>" + version + "</string>");
            File.WriteAllText(filePath, updatedPlistContent);
        }



        public static Dictionary<string, Process> screenRecordingUDIDProcess = new Dictionary<string, Process>();
        public static Dictionary<string, int> screenRecordingUDIDProcessId = new Dictionary<string, int>();
        public async Task StartScreenRecording(string udid, string deviceName)
        {
            string downloadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            string timestamp = DateTime.Now.ToString("dd-MMM-yyyy_hh.mmtt");
            string filePath = Path.Combine(downloadPath, $"Screen_Recording_{deviceName.Replace("'", "").Replace(" ", "_")}_{timestamp}.mp4");
            filePath = "\"" + filePath + "\"";
            string videoUrl = "http://localhost:" + ScreenControl.devicePorts[udid].Item1;
            string FfmpegCommand = $"ffmpeg -use_wallclock_as_timestamps 1 -f mjpeg -i {videoUrl} -c copy -y " + filePath;
            string[] commands = { $"set PATH=\"{FilesPath.FFMpegFilePath}\";%PATH%", FfmpegCommand };

            Process screenRecordingProcess = new Process();
            screenRecordingProcess.StartInfo.FileName = "cmd.exe";
            screenRecordingProcess.StartInfo.Arguments = "/K";
            screenRecordingProcess.StartInfo.UseShellExecute = false;
            screenRecordingProcess.StartInfo.CreateNoWindow = true;
            screenRecordingProcess.StartInfo.RedirectStandardOutput = true;
            screenRecordingProcess.StartInfo.RedirectStandardError = true;
            screenRecordingProcess.StartInfo.RedirectStandardInput = true;

            await Task.Run(async () =>
            {
                screenRecordingProcess.Start();
                foreach (string command in commands)
                {
                    screenRecordingProcess.StandardInput.WriteLine(command);
                    screenRecordingProcess.StandardInput.WriteLine("echo Command completed");
                }
                await Task.Delay(3000);
                int processId = GetFFMpegProcess(udid);
                if (screenRecordingUDIDProcessId.ContainsKey(udid))
                {
                    screenRecordingUDIDProcessId[udid] = processId;
                }
                else
                {
                    screenRecordingUDIDProcessId.Add(udid, processId);
                }
                MainScreen.runningProcesses.Add(processId);
            });
            if (screenRecordingUDIDProcess.ContainsKey(udid))
            {
                screenRecordingUDIDProcess[udid] = screenRecordingProcess;
            }
            else
            {
                screenRecordingUDIDProcess.Add(udid, screenRecordingProcess);
            }
            MainScreen.runningProcesses.Add(screenRecordingProcess.Id);
        }

        public async Task StopScreenRecording(string udid)
        {
            if (screenRecordingUDIDProcess.ContainsKey(udid))
            {
                var screenRecordingProcess = screenRecordingUDIDProcess[udid];
                try
                {
                    if (!screenRecordingProcess.HasExited)
                    {
                        // Send "q" to gracefully stop ffmpeg
                        screenRecordingProcess.StandardInput.WriteLine("q");
                        screenRecordingProcess.StandardInput.Flush();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error sending 'q': " + ex.Message);
                }
                finally
                {
                    if (!screenRecordingProcess.HasExited)
                    {
                        screenRecordingProcess.Kill();
                    }
                    screenRecordingProcess.Close();

                    try
                    {
                        await Task.Delay(3000);
                        int processId = screenRecordingUDIDProcessId[udid];
                        Process.GetProcessById(processId).Kill();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error killing process: " + ex.Message);
                    }
                    screenRecordingUDIDProcess.Remove(udid);
                    screenRecordingUDIDProcessId.Remove(udid);
                }
            }
        }

        private int GetFFMpegProcess(string udid)
        {
            try
            {
                var searcher = new ManagementObjectSearcher("SELECT CommandLine, ProcessId FROM Win32_Process WHERE Name = 'ffmpeg.exe'");
                foreach (ManagementObject obj in searcher.Get())
                {
                    string? commandLine = obj["CommandLine"]?.ToString();
                    if (commandLine != null && commandLine.Contains(ScreenControl.devicePorts[udid].Item1.ToString()))
                    {
                        int processId = Convert.ToInt32(obj["ProcessId"]);
                        return processId;
                    }
                }
            }
            catch (Exception)
            {
                return 0;
            }
            return 0;
        }

        private static bool isActivationHandlerSet = false;
        public static void ShowNotification(string title, string message)
        {
            try
            {
                string iconPath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\Resources\\appiumwizardlogo.ico";
                if (message.Contains("Downloads folder"))
                {
                    new ToastContentBuilder()
                       .AddText(title)
                       .AddText(message)
                       .SetToastDuration(ToastDuration.Short)
                       .AddAppLogoOverride(new Uri(iconPath), ToastGenericAppLogoCrop.Circle)
                       .AddButton(new ToastButton()
                       .SetContent("Open Downloads")
                       .AddArgument("action", "openDownloads"))
                       .Show();
                    if (!isActivationHandlerSet)
                    {
                        ToastNotificationManagerCompat.OnActivated += toastArgs =>
                        {
                            ToastArguments args = ToastArguments.Parse(toastArgs.Argument);
                            if (args["action"] == "openDownloads")
                            {
                                Process.Start("explorer.exe", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads");
                            }
                        };
                        isActivationHandlerSet = true;
                    }
                }
                else
                {
                    new ToastContentBuilder()
                     .AddText(title)
                     .AddText(message)
                     .SetToastDuration(ToastDuration.Short)
                     .AddAppLogoOverride(new Uri(iconPath), ToastGenericAppLogoCrop.Circle)
                     .Show();
                }
            }
            catch (Exception)
            {
            }
        }

        public static void ScanForDetachedProcesses()
        {
            try
            {
                StringBuilder runningProcesses = new StringBuilder();
                List<int> processIds = new List<int>();

                Process[] iproxyProcesses = Process.GetProcessesByName("iproxy");
                if (iproxyProcesses.Length > 0)
                {
                    runningProcesses.AppendLine("iproxy");
                    foreach (var process in iproxyProcesses)
                    {
                        processIds.Add(process.Id);
                    }
                }

                Process[] iOSServerProcesses = Process.GetProcessesByName("iOSServer");
                if (iOSServerProcesses.Length > 0)
                {
                    runningProcesses.AppendLine("iOSServer");
                    foreach (var process in iOSServerProcesses)
                    {
                        processIds.Add(process.Id);
                    }
                }

                Process[] iOSServerPyProcesses = Process.GetProcessesByName("iOSServerPy");
                if (iOSServerPyProcesses.Length > 0)
                {
                    runningProcesses.AppendLine("iOSServerPy");
                    foreach (var process in iOSServerPyProcesses)
                    {
                        processIds.Add(process.Id);
                    }
                }

                if (runningProcesses.Length > 0)
                {
                    var dialogResult = MessageBox.Show(
                        $"The following Appium Wizard detached processes are running. If you did not start these processes manually, press Yes to kill the processes to run Appium Wizard smoothly or press No(You might face issues with your iOS execution):\n\n{runningProcesses}",
                        "Detached Process",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (dialogResult == DialogResult.Yes)
                    {
                        foreach (var item in processIds)
                        {
                            KillProcessById(item);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public static void DeleteLogFiles()
        {
            try
            {
                DirectoryInfo directory = new DirectoryInfo(FilesPath.logsFilesPath);
                if (directory.Exists)
                {
                    // Get all files in the directory
                    FileInfo[] files = directory.GetFiles();

                    foreach (FileInfo file in files)
                    {
                        // Check if the file is older than 3 days
                        if (file.LastWriteTime.Date < DateTime.Today.AddDays(-2).Date)
                        {
                            // Delete the file
                            file.Delete();
                            Logger.Info("Deleted log file - " + file + ", last modified - " + file.LastWriteTime.Date);
                        }
                    }
                }
                else
                {
                    Logger.Info("Delete log file - Directory does not exist");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Delete log file - exception");
            }
        }

        public static async Task<string> ReadJsonFromRemote(string fileUrl)
        {
            // URL of the file you want to download
            string jsonContent = string.Empty;

            await Task.Run(() =>
            {
                // Create a new instance of HttpClient
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        // Send a GET request to the specified URL synchronously
                        HttpResponseMessage response = client.GetAsync(fileUrl).Result;

                        // Ensure the request was successful
                        response.EnsureSuccessStatusCode();

                        // Read the response content as a string synchronously
                        jsonContent = response.Content.ReadAsStringAsync().Result;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Message}");
                        // Handle the exception as needed
                        jsonContent = string.Empty; // or null, depending on your preference
                    }
                }
            });

            return jsonContent;
        }

        //public static async Task StartHttpServer()
        //{

        //// Create an instance of HttpListener
        //HttpListener listener = new HttpListener();

        //// Add a prefix for the listener to respond to
        //// Use "http://localhost:8080/" to avoid requiring admin rights
        //listener.Prefixes.Add("http://localhost:8080/");

        //// Start the listener
        //listener.Start();
        //Console.WriteLine("Listening for connections on http://localhost:8080/");

        //// Handle incoming requests asynchronously
        //while (true)
        //{
        //    // Wait for an incoming request
        //    HttpListenerContext context = await listener.GetContextAsync();

        //    // Get the request and response objects
        //    HttpListenerRequest request = context.Request;
        //    HttpListenerResponse response = context.Response;

        //    // Create a response string
        //    string responseString = "<html><body>Hello, world!</body></html>";
        //    byte[] buffer = Encoding.UTF8.GetBytes(responseString);

        //    // Set the content length and write the response
        //    response.ContentLength64 = buffer.Length;
        //    await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);

        //    // Close the output stream
        //    response.OutputStream.Close();
        //}
        //}
        public static async Task StartHttpServer()
        {
            // Use a specific IP address or "0.0.0.0" to listen on all interfaces
            using (var server = CreateWebServer("http://0.0.0.0:9696/"))
            {
                // Start the server and wait for it to complete
                //MessageBox.Show("Server starting...");
                await server.RunAsync();
            }
        }

        private static WebServer CreateWebServer(string url)
        {
            return new WebServer(o => o
                    .WithUrlPrefix(url)
                    .WithMode(HttpListenerMode.EmbedIO))
                .WithWebApi("/api", m => m
                    .WithController<HelloWorldController>());
        }
    }

    public class HelloWorldController : WebApiController
    {
        // Define a GET endpoint
        [Route(HttpVerbs.Get, "/hello")]
        public string GetHello()
        {
            try
            {
                return "Hello from EmbedIO!";
            }
            catch (Exception ex)
            {
                return "An error occurred.";
            }
        }
    }
}
