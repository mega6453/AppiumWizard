using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Toolkit.Uwp.Notifications;
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
            try
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
                    return (process.Id, process.ProcessName);
                }
                else
                {
                    return (0, "");
                }
            }
            catch (Exception)
            {
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
            catch (Exception)
            {
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

        public static string InstalledAppiumServerVersionFromPackageJson()
        {
            try
            {
                string appiumPath = Path.Combine(serverFolderPath, "node_modules", "appium");
                string packageJsonPath = Path.Combine(appiumPath, "package.json");
                if (!File.Exists(packageJsonPath))
                    return "";

                string jsonContent = File.ReadAllText(packageJsonPath);

                using (JsonDocument doc = JsonDocument.Parse(jsonContent))
                {
                    if (doc.RootElement.TryGetProperty("version", out JsonElement versionElement))
                    {
                        return versionElement.GetString() ?? "";
                    }
                }

                return "";
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


        public static void InstallNodeJsOld(bool showExecution = false)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = FilesPath.zipExtractorFilePath;
                process.StartInfo.Arguments = $"x \"{nodeFilePath}\" -o\"{serverFolderPath}\" -y";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = !showExecution;
                process.Start();
                process.WaitForExit();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while installing NodeJs. \nOriginal exception: " + e.Message, "Install NodeJs", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void InstallNodeJs(bool showExecution = false)
        {
            string tempPath = null;
            try
            {
                // Delete existing node files
                DeleteNodeFiles();

                // Create temporary extraction directory
                tempPath = Path.Combine(Path.GetTempPath(), "NodeJsInstall_" + Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                // Extract to temporary directory
                Process process = new Process();
                process.StartInfo.FileName = FilesPath.zipExtractorFilePath;
                process.StartInfo.Arguments = $"x \"{nodeFilePath}\" -o\"{tempPath}\" -y";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = !showExecution;

                process.Start();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    throw new Exception($"7-Zip extraction failed with exit code: {process.ExitCode}");
                }

                // Ensure target directory exists
                Directory.CreateDirectory(serverFolderPath);

                // Handle the extracted content - THIS IS WHERE WE CALL THE METHOD
                ExtractContentToTarget(tempPath, serverFolderPath);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while installing NodeJs. \nOriginal exception: " + e.Message, "Install NodeJs", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Clean up temporary directory
                if (tempPath != null && Directory.Exists(tempPath))
                {
                    try
                    {
                        Directory.Delete(tempPath, true);
                    }
                    catch
                    {
                        // Ignore cleanup errors
                    }
                }
            }
        }

        // This method determines what to extract and calls CopyDirectoryContents
        private static void ExtractContentToTarget(string tempPath, string targetPath)
        {
            var files = Directory.GetFiles(tempPath);
            var directories = Directory.GetDirectories(tempPath);

            if (files.Length == 0 && directories.Length == 1)
            {
                // Only one directory with no files at root level - copy its contents
                // THIS IS WHERE CopyDirectoryContents IS CALLED
                CopyDirectoryContents(directories[0], targetPath);
            }
            else
            {
                // Copy all items directly
                // THIS IS WHERE CopyDirectoryContents IS CALLED
                CopyDirectoryContents(tempPath, targetPath);
            }
        }

        // Solution 3: The actual copy method with overwrite capability
        private static void CopyDirectoryContents(string sourceDir, string targetDir)
        {
            // Ensure target directory exists
            Directory.CreateDirectory(targetDir);

            // Copy all files with overwrite
            foreach (string sourceFile in Directory.GetFiles(sourceDir))
            {
                string fileName = Path.GetFileName(sourceFile);
                string targetFile = Path.Combine(targetDir, fileName);
                File.Copy(sourceFile, targetFile, true); // true = overwrite existing
            }

            // Copy all directories recursively
            foreach (string sourceSubDir in Directory.GetDirectories(sourceDir))
            {
                string dirName = Path.GetFileName(sourceSubDir);
                string targetSubDir = Path.Combine(targetDir, dirName);
                CopyDirectoryContents(sourceSubDir, targetSubDir); // Recursive call
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
                    Timeout = TimeSpan.FromMilliseconds(-1)
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

        public static Dictionary<int, int> serverNumberPortNumber = new Dictionary<int, int>();
        private static Dictionary<int, HttpListener> serverListeners = new Dictionary<int, HttpListener>();
        private static Dictionary<int, CancellationTokenSource> serverTokens = new Dictionary<int, CancellationTokenSource>();
        private static Dictionary<int, string> serverHtmlFiles = new Dictionary<int, string>(); // Track temp files

        // Add these constants for memory management
        private const int MAX_LOG_LINES = 1000;
        private const long MAX_FILE_SIZE_BYTES = 5 * 1024 * 1024; // 5MB
        private static readonly object fileLock = new object();

        public static void StartServer(int serverNumber, int port, string htmlFilePath, string serverLogsFilePath)
        {
            string prefix = "http://localhost:" + port + "/";
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(prefix);

            CancellationTokenSource cts = new CancellationTokenSource();
            serverTokens[serverNumber] = cts;
            CancellationToken token = cts.Token;

            serverListeners[serverNumber] = listener;
            serverNumberPortNumber[serverNumber] = port;
            serverHtmlFiles[serverNumber] = htmlFilePath; // Track HTML file for cleanup

            listener.Start();
            Console.WriteLine($"Server {serverNumber} started at {prefix}");

            _ = Task.Run(async () =>
            {
                try
                {
                    while (!token.IsCancellationRequested)
                    {
                        try
                        {
                            HttpListenerContext context = await GetContextAsync(listener, token);
                            if (context == null) break;

                            _ = Task.Run(() => ProcessRequest(context, htmlFilePath, serverLogsFilePath), token);
                        }
                        catch (ObjectDisposedException)
                        {
                            break; // Listener was disposed
                        }
                        catch (HttpListenerException)
                        {
                            break; // Listener was stopped
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Server error: " + ex.Message);
                }
            }, token);
        }

        private static async Task<HttpListenerContext> GetContextAsync(HttpListener listener, CancellationToken token)
        {
            try
            {
                return await Task.Run(() => listener.GetContext(), token);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static void ProcessRequest(HttpListenerContext context, string htmlFilePath, string serverLogsFilePath)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;
            Console.WriteLine($"Requested log file path: {serverLogsFilePath}");
            Console.WriteLine($"File.Exists: {File.Exists(serverLogsFilePath)}");
            try
            {
                // Set CORS headers
                response.Headers.Add("Access-Control-Allow-Origin", "*");
                response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
                response.Headers.Add("Pragma", "no-cache");
                response.Headers.Add("Expires", "0");

                if (request.Url.AbsolutePath == "/index.html" && File.Exists(htmlFilePath))
                {
                    // Serve the HTML file
                    ServeHtmlFile(response, htmlFilePath);
                }
                else if (request.Url.AbsolutePath == "/file" && File.Exists(serverLogsFilePath))
                {
                    // Check if full log download requested via query parameter
                    bool fullLogRequested = false;
                    var query = request.Url.Query; // e.g., "?full=true"
                    if (!string.IsNullOrEmpty(query))
                    {
                        var queryParams = System.Web.HttpUtility.ParseQueryString(query);
                        fullLogRequested = queryParams["full"]?.Equals("true", StringComparison.OrdinalIgnoreCase) ?? false;
                    }

                    if (fullLogRequested)
                    {
                        // Serve full log file without truncation
                        ServeFullLogFile(response, serverLogsFilePath);
                    }
                    else
                    {
                        // Serve truncated log file as before
                        ServeLogFile(response, serverLogsFilePath);
                    }
                }
                else
                {
                    // Return 404 if the file is not found
                    Send404Response(response);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Request processing error: " + ex.Message);
                try
                {
                    response.StatusCode = 500;
                    response.Close();
                }
                catch { }
            }
        }

        private static void ServeFullLogFile(HttpListenerResponse response, string serverLogsFilePath)
        {
            try
            {
                lock (fileLock)
                {
                    using (var fs = new FileStream(
                        serverLogsFilePath,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.ReadWrite))
                    {
                        response.ContentType = "text/plain";
                        response.ContentLength64 = fs.Length;

                        // Set Content-Disposition header with original filename
                        string fileName = Path.GetFileName(serverLogsFilePath);
                        response.AddHeader("Content-Disposition", $"attachment; filename=\"{fileName}\"");

                        fs.CopyTo(response.OutputStream);
                    }
                }
                response.OutputStream.Flush();
                response.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error serving full log file: " + ex.Message);
                Send404Response(response);
            }
        }

        private static void ServeHtmlFile(HttpListenerResponse response, string htmlFilePath)
        {
            try
            {
                byte[] buffer = File.ReadAllBytes(htmlFilePath);
                response.ContentType = "text/html";
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error serving HTML: " + ex.Message);
                Send404Response(response);
            }
        }

        private static void ServeLogFile(HttpListenerResponse response, string serverLogsFilePath)
        {
            try
            {
                lock (fileLock)
                {
                    FileInfo fileInfo = new FileInfo(serverLogsFilePath);

                    // Check file size limit
                    if (fileInfo.Length > MAX_FILE_SIZE_BYTES)
                    {
                        // Read only the last portion of large files
                        string limitedContent = ReadLastLinesFromLargeFile(serverLogsFilePath, MAX_LOG_LINES);
                        byte[] buffer = Encoding.UTF8.GetBytes(limitedContent);
                        response.ContentType = "text/plain";
                        response.ContentLength64 = buffer.Length;
                        response.OutputStream.Write(buffer, 0, buffer.Length);
                    }
                    else
                    {
                        // For smaller files, read last N lines efficiently
                        string fileContent = ReadLastLines(serverLogsFilePath, MAX_LOG_LINES);
                        byte[] buffer = Encoding.UTF8.GetBytes(fileContent);
                        response.ContentType = "text/plain";
                        response.ContentLength64 = buffer.Length;
                        response.OutputStream.Write(buffer, 0, buffer.Length);
                    }
                }
                response.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error serving log file: " + ex.Message);
                Send404Response(response);
            }
        }

        private static string ReadLastLines(string filePath, int maxLines)
        {
            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var streamReader = new StreamReader(fileStream))
                {
                    var lines = new List<string>();
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        lines.Add(line);
                        if (lines.Count > maxLines)
                        {
                            lines.RemoveAt(0); // Remove oldest line
                        }
                    }
                    return string.Join("\n", lines);
                }
            }
            catch
            {
                return "Error reading log file";
            }
        }

        private static string ReadLastLinesFromLargeFile(string filePath, int maxLines)
        {
            try
            {
                const int bufferSize = 8192;
                var lines = new List<string>();

                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    // Start from the end of file and read backwards
                    long position = fileStream.Length;
                    byte[] buffer = new byte[bufferSize];
                    StringBuilder currentLine = new StringBuilder();

                    while (position > 0 && lines.Count < maxLines)
                    {
                        int bytesToRead = (int)Math.Min(bufferSize, position);
                        position -= bytesToRead;
                        fileStream.Seek(position, SeekOrigin.Begin);
                        fileStream.Read(buffer, 0, bytesToRead);

                        // Process buffer in reverse
                        for (int i = bytesToRead - 1; i >= 0; i--)
                        {
                            if (buffer[i] == '\n')
                            {
                                if (currentLine.Length > 0)
                                {
                                    // Reverse the characters in currentLine
                                    char[] chars = currentLine.ToString().ToCharArray();
                                    Array.Reverse(chars);
                                    lines.Add(new string(chars));
                                    currentLine.Clear();

                                    if (lines.Count >= maxLines)
                                        break;
                                }
                            }
                            else if (buffer[i] != '\r')
                            {
                                currentLine.Append((char)buffer[i]);
                            }
                        }
                    }

                    // Add any remaining content
                    if (currentLine.Length > 0 && lines.Count < maxLines)
                    {
                        char[] chars = currentLine.ToString().ToCharArray();
                        Array.Reverse(chars);
                        lines.Add(new string(chars));
                    }
                }

                // Reverse the lines list to get correct order
                lines.Reverse();
                return string.Join("\n", lines);
            }
            catch
            {
                return "Error reading large log file";
            }
        }

        private static void Send404Response(HttpListenerResponse response)
        {
            try
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                byte[] buffer = Encoding.UTF8.GetBytes("File not found");
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.Close();
            }
            catch { }
        }


        public static void StopLogsServer(int serverNumber)
        {
            if (serverListeners.ContainsKey(serverNumber) && serverTokens.ContainsKey(serverNumber))
            {
                try
                {
                    // Cancel the token first
                    serverTokens[serverNumber].Cancel();

                    // Stop and close the listener
                    if (serverListeners[serverNumber].IsListening)
                    {
                        serverListeners[serverNumber].Stop();
                    }
                    serverListeners[serverNumber].Close();

                    // Clean up temporary HTML file
                    if (serverHtmlFiles.ContainsKey(serverNumber))
                    {
                        try
                        {
                            if (File.Exists(serverHtmlFiles[serverNumber]))
                            {
                                File.Delete(serverHtmlFiles[serverNumber]);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error deleting temp HTML file: {ex.Message}");
                        }
                        serverHtmlFiles.Remove(serverNumber);
                    }

                    // Remove from dictionaries
                    serverListeners.Remove(serverNumber);
                    serverTokens.Remove(serverNumber);
                    serverNumberPortNumber.Remove(serverNumber);

                    Console.WriteLine($"Server {serverNumber} stopped.");

                    // Force garbage collection
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error stopping server {serverNumber}: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"Server {serverNumber} is not running.");
            }
        }

        public static string GenerateHtmlWithFilePath(string filePath, int appiumPort, int interval)
        {
            string htmlTemplate = @"
                         <!DOCTYPE html>
                        <html lang=""en"">
                        <head>
                            <meta charset=""UTF-8"">
                            <title>Appium Wizard Server Logs - {PORT}</title>
                            <style>
                                body { 
                                    font-family: Arial, sans-serif; 
                                    margin: 0; 
                                    padding: 0; 
                                    height: 100vh;
                                    overflow: hidden;
                                }
                                #container { 
                                    display: flex;
                                    flex-direction: column;
                                    height: 100vh; 
                                }
                                #header {
                                    flex-shrink: 0;
                                    display: flex;
                                    justify-content: space-between;
                                    align-items: center;
                                    background: rgba(255, 255, 255, 0.8);
                                    padding: 5px 10px;
                                    border-radius: 4px;
                                    min-height: 40px;
                                }
                                #controls {
                                    display: flex;
                                    align-items: center;
                                    gap: 10px;
                                }
                                #content {
                                    flex: 1;
                                    padding: 20px;
                                    overflow-y: auto;
                                    overflow-x: auto;
                                    white-space: pre-wrap;
                                    background-color: #f4f4f4;
                                    word-wrap: break-word;
                                    box-sizing: border-box;
                                    font-family: Monaco, 'Lucida Console', monospace;
                                    font-size: 12px;
                                    line-height: 1.4;
                                }
                                #title {
                                    text-align: center;
                                    font-size: 18px;
                                    font-weight: bold;
                                    background-color: #0078d7;
                                    color: white;
                                    flex-grow: 1;
                                    margin-right: 10px;
                                    padding: 5px 10px;
                                    border-radius: 4px;
                                }
                                button {
                                    cursor: pointer;
                                    padding: 5px 10px;
                                    border-radius: 3px;
                                    border: 1px solid #0078d7;
                                    background-color: white;
                                    color: #0078d7;
                                    font-weight: bold;
                                }
                                button:hover {
                                    background-color: #0078d7;
                                    color: white;
                                }
                            </style>
                        </head>
                        <body>
                            <div id=""container"">
                                <div id=""header"">
                                    <div id=""title"">Appium Wizard Server Logs</div>
                                    <div id=""controls"">
                                        <button id=""downloadBtn"" style=""position: relative;"">
                                          Download full logs
                                          <span 
                                            style=""margin-left: 6px; cursor: pointer; font-weight: bold; color: #555;"" 
                                            title=""Only the last 1000 lines are shown in the viewer. Click to download the complete log file."">
                                            &#9432;
                                          </span>
                                        </button>
                                        <label><input type=""checkbox"" id=""pauseFetch""> Pause Logs</label>
                                        <label><input type=""checkbox"" id=""autoScroll"" checked> Auto-scroll</label>
                                    </div>
                                </div>
                                <pre id=""content"">Loading logs...</pre>
                            </div>
                            <script>
                                const contentElement = document.getElementById('content');
                                const autoScrollCheckbox = document.getElementById('autoScroll');
                                const pauseFetchCheckbox = document.getElementById('pauseFetch');
                                const downloadBtn = document.getElementById('downloadBtn');

                                function fetchTextFile() {
                                    if (pauseFetchCheckbox.checked) {
                                        return;
                                    }
                                    fetch('/file')
                                        .then(response => {
                                            if (!response.ok) {
                                                throw new Error(`HTTP error! status: ${response.status}`);
                                            }
                                            return response.text();
                                        })
                                        .then(text => {
                                            contentElement.textContent = text;
                                            if (autoScrollCheckbox.checked) {
                                                // Use requestAnimationFrame to ensure DOM is updated before scrolling
                                                requestAnimationFrame(() => {
                                                    contentElement.scrollTop = contentElement.scrollHeight;
                                                });
                                            }
                                        })
                                        .catch(error => {
                                            contentElement.textContent = 'Error loading file: ' + error;
                                        });
                                }

                                async function downloadFullLogs() {
                                        try {
                                            const response = await fetch('/file?full=true');
                                            if (!response.ok) {
                                                alert('Error downloading logs: ' + response.statusText);
                                                return;
                                            }
                                            const blob = await response.blob();

                                            // Get filename from Content-Disposition header
                                            const contentDisposition = response.headers.get('Content-Disposition');
                                            let fileName = 'logs.txt'; // fallback filename
                                            if (contentDisposition) {
                                                const fileNameMatch = contentDisposition.match(/filename=""(.+)""/);
                                                if (fileNameMatch.length === 2) {
                                                    fileName = fileNameMatch[1];
                                                }
                                            }

                                            const url = window.URL.createObjectURL(blob);
                                            const a = document.createElement('a');
                                            a.href = url;
                                            a.download = fileName;  // use filename from server
                                            document.body.appendChild(a);
                                            a.click();
                                            a.remove();
                                            window.URL.revokeObjectURL(url);
                                        } catch (err) {
                                            alert('Failed to download logs: ' + err);
                                        }
                                    }

                                downloadBtn.addEventListener('click', downloadFullLogs);

                                // Initial fetch and interval
                                fetchTextFile();
                                setInterval(fetchTextFile, {INTERVAL});
                            </script>
                        </body>
                        </html>";

            // Replace placeholders
            htmlTemplate = htmlTemplate.Replace("Appium Wizard Server Logs", $"Appium Wizard Server Logs - {appiumPort}");
            htmlTemplate = htmlTemplate.Replace("{PORT}", appiumPort.ToString());
            htmlTemplate = htmlTemplate.Replace("setInterval(fetchTextFile, {INTERVAL})", $"setInterval(fetchTextFile, {interval})");

            // Save temp file
            string tempHtmlFilePath = Path.Combine(Path.GetTempPath(), $"GeneratedHtml_{Guid.NewGuid()}.html");
            File.WriteAllText(tempHtmlFilePath, htmlTemplate);

            return tempHtmlFilePath;
        }

        public static void CleanupTempFiles()
        {
            try
            {
                string tempDir = Path.Combine(Path.GetTempPath(), "AppiumWizard_Logs");
                if (Directory.Exists(tempDir))
                {
                    var files = Directory.GetFiles(tempDir, "ServerLogs_*.html");
                    var cutoffTime = DateTime.Now.AddHours(-1); // Delete files older than 1 hour

                    foreach (var file in files)
                    {
                        try
                        {
                            if (File.GetCreationTime(file) < cutoffTime)
                            {
                                File.Delete(file);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error deleting temp file {file}: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during temp file cleanup: {ex.Message}");
            }
        }

        public static void StopAllServers()
        {
            var serverNumbers = serverListeners.Keys.ToList();
            foreach (var serverNumber in serverNumbers)
            {
                StopLogsServer(serverNumber);
            }

            // Final cleanup
            CleanupTempFiles();
            GC.Collect();
        }

        public static async Task<string> GetLatestNodeVersion()
        {
            string url = "https://nodejs.org/dist/index.json";

            using HttpClient client = new HttpClient();

            try
            {
                var json = await client.GetStringAsync(url);
                var releases = JsonSerializer.Deserialize<NodeRelease[]>(json);

                if (releases != null && releases.Length > 0)
                {
                    return releases[0].version.TrimStart('v').Trim();
                }
                else
                {
                    return "NA";
                }
            }
            catch
            {
                return "NA";
            }
        }

        public static async Task DownloadNodeJS()
        {
            string indexJsonUrl = "https://nodejs.org/dist/index.json";

            try
            {
                using HttpClient client = new HttpClient();

                // Fetch the index.json to get latest release info
                string json = await client.GetStringAsync(indexJsonUrl);
                var releases = JsonSerializer.Deserialize<NodeRelease[]>(json);

                if (releases == null || releases.Length == 0)
                {
                    return;
                }

                var latestRelease = releases[0];
                string versionNoV = latestRelease.version.StartsWith("v") ? latestRelease.version.Substring(1) : latestRelease.version;
                string zipFileName = $"node-v{versionNoV}-win-x64.zip";

                // Construct download URL
                string downloadUrl = $"https://nodejs.org/dist/{latestRelease.version}/{zipFileName}";

                // Define paths
                string downloadFolder = Path.Combine(FilesPath.serverInstalledPath, "Backup");
                Directory.CreateDirectory(downloadFolder); // Ensure folder exists

                string newFilePath = Path.Combine(downloadFolder, zipFileName);
                string finalFilePath = Path.Combine(downloadFolder, "node.zip");

                // Download the zip file to newFilePath
                using (var response = await client.GetAsync(downloadUrl))
                {
                    response.EnsureSuccessStatusCode();
                    using (var fs = new FileStream(newFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await response.Content.CopyToAsync(fs);
                    }
                }

                // Delete old node.zip if exists
                if (File.Exists(finalFilePath))
                {
                    File.Delete(finalFilePath);
                }

                // Rename the downloaded file to node.zip
                File.Move(newFilePath, finalFilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private class NodeRelease
        {
            public string version { get; set; }
            public string[] files { get; set; }
        }

        public static string GetNodeVersion()
        {
            string nodeExePath = FilesPath.nodePath;
            if (!File.Exists(nodeExePath))
            {
                return "NA";
            }

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = nodeExePath,
                Arguments = "--version",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(psi))
            {
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                return output.TrimStart('v').Trim();
            }
        }

        public static void DeleteNodeFiles()
        {
            // Keep rules
            string[] keepFiles = Directory.GetFiles(serverFolderPath)
                .Select(Path.GetFileName)
                .Where(f => f.StartsWith("appium", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            // Delete files
            foreach (var file in Directory.GetFiles(serverFolderPath))
            {
                string fileName = Path.GetFileName(file);

                if (!keepFiles.Contains(fileName, StringComparer.OrdinalIgnoreCase))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }

            // Delete folders except Backup and node_modules\appium
            foreach (var dir in Directory.GetDirectories(serverFolderPath))
            {
                string dirName = Path.GetFileName(dir);

                if (dirName.Equals("Backup", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (dirName.Equals("node_modules", StringComparison.OrdinalIgnoreCase))
                {
                    // Inside node_modules, delete everything except "appium"
                    foreach (var subDir in Directory.GetDirectories(dir))
                    {
                        if (!Path.GetFileName(subDir).Equals("appium", StringComparison.OrdinalIgnoreCase))
                        {
                            try
                            {
                                Directory.Delete(subDir, true);
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }

                    // Delete stray files in node_modules (not folders)
                    foreach (var subFile in Directory.GetFiles(dir))
                    {
                        try
                        {
                            File.Delete(subFile);
                        }
                        catch (Exception ex)
                        {
                        }
                    }

                    continue;
                }

                // Delete other folders
                try
                {
                    Directory.Delete(dir, true);
                }
                catch (Exception ex)
                {
                }
            }
        }

        public static void KillExeRunningFromAppiumWizardFolder(string ExePath, string processName)
        {
            Process[] ProcessesList = Process.GetProcessesByName(processName); //example: adb (without .exe)

            foreach (var process in ProcessesList)
            {
                try
                {
                    // Get the main module's file path
                    string processPath = process.MainModule.FileName;

                    // Compare paths (case-insensitive)
                    if (string.Equals(Path.GetFullPath(processPath), Path.GetFullPath(ExePath), StringComparison.OrdinalIgnoreCase))
                    {
                        process.Kill();
                        break;
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        public static void KillExeRunningFromAppiumWizardFolder()
        {
            //KillExeRunningFromAppiumWizardFolder(FilesPath.adbFilePath, "adb"); 
            KillExeRunningFromAppiumWizardFolder(FilesPath.iProxyFilePath, "iproxy");
            KillExeRunningFromAppiumWizardFolder(FilesPath.iOSServerFilePath, "iOSServer");
            KillExeRunningFromAppiumWizardFolder(FilesPath.nodePath, "node");
            //KillExeRunningFromAppiumWizardFolder(FilesPath.pymd3FilePath, "iOSServerPy");
        }


        public static string GetAppiumPeerDependencyVersionForInstalledUIAutomator(string version)
        {
            string packageName = "appium-uiautomator2-driver";
            string url = $"https://registry.npmjs.org/{packageName}/{version}";

            using HttpClient client = new HttpClient();

            try
            {
                // Synchronously wait for the response
                var response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();

                // Synchronously read the content
                string json = response.Content.ReadAsStringAsync().Result;

                using JsonDocument doc = JsonDocument.Parse(json);
                JsonElement root = doc.RootElement;

                if (root.TryGetProperty("peerDependencies", out JsonElement peerDeps) &&
                    peerDeps.TryGetProperty("appium", out JsonElement appiumVersion))
                {
                    return new string(appiumVersion.GetString().SkipWhile(c => !char.IsDigit(c)).ToArray());
                }
            }
            catch
            {
                // Handle exceptions or log if necessary
            }

            return null; // Return null if not found or error
        }
    }
}
