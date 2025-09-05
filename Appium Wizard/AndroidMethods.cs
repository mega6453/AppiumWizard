using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using RestSharp;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Management;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Windows.Media.Devices.Core;
using static System.Windows.Forms.AxHost;

namespace Appium_Wizard
{
    public class AndroidMethods
    {
        private static AndroidMethods? instance;
        private static readonly object lockObject = new object();
        private string adbFilePath = FilesPath.adbFilePath;
        private string aaptFilePath = FilesPath.aaptFilePath;
        private string serverAPKFilePath = FilesPath.serverAPKFilePath;
        private string settingsAPKFilePath = FilesPath.settingsAPKFilePath;
        private Process adbProcess;
        public static Dictionary<int, int> PortProcessId = new Dictionary<int, int>();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public bool StartAdbServer()
        {
            if (string.IsNullOrEmpty(adbFilePath) || !File.Exists(adbFilePath))
            {
                throw new FileNotFoundException("ADB executable not found");
            }

            try
            {
                ProcessStartInfo adbStartInfo = new ProcessStartInfo
                {
                    FileName = adbFilePath,
                    Arguments = "start-server",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = new Process { StartInfo = adbStartInfo })
                {
                    process.Start();

                    if (process.WaitForExit(10000))
                    {
                        return IsAdbServerRunning();
                    }
                    else
                    {
                        process.Kill();
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting ADB server: {ex.Message}");
                return false;
            }
        }

        public bool StopAdbServer()
        {
            try
            {
                ProcessStartInfo adbStopInfo = new ProcessStartInfo
                {
                    FileName = adbFilePath,
                    Arguments = "kill-server",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = new Process { StartInfo = adbStopInfo })
                {
                    process.Start();

                    if (process.WaitForExit(5000))
                    {
                        return process.ExitCode == 0;
                    }
                    else
                    {
                        process.Kill();
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error stopping ADB server: {ex.Message}");
                return false;
            }
        }

        public bool IsAdbServerRunning()
        {
            try
            {
                ProcessStartInfo checkInfo = new ProcessStartInfo
                {
                    FileName = adbFilePath,
                    Arguments = $"devices",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = new Process { StartInfo = checkInfo })
                {
                    process.Start();
                    process.WaitForExit(3000);
                    return process.ExitCode == 0;
                }
            }
            catch
            {
                return false;
            }
        }

        public void StopUIAutomator(string udid)
        {
            ExecuteCommand("-s " + udid + " shell am force-stop io.appium.uiautomator2.server", false);
            ExecuteCommand("-s " + udid + " shell am force-stop io.appium.uiautomator2.server.test", false);
        }

        public void UninstallOtherInstrumentationApps(string udid)
        {
            var apps = ListOfInstrumentationPackages(udid);
            apps.Remove("io.appium.uiautomator2.server.test");
            apps.Remove("io.appium.uiautomator2.server");
            foreach (var item in apps)
            {
                Logger.Info("UninstallOtherInstrumentationApps :"+item);
                UnInstallApp(udid,item);
            }
        }

        public List<string> ListOfInstrumentationPackages(string udid)
        {
            List<string> packageNames = new List<string>();
            try
            {
                string instrumentations = ExecuteCommand("-s " + udid + " shell pm list instrumentation", false);
                string pattern = @"instrumentation:([\w\.]+)/";

                MatchCollection matches = Regex.Matches(instrumentations, pattern);

                foreach (Match match in matches)
                {
                    if (match.Groups.Count > 1)
                    {
                        packageNames.Add(match.Groups[1].Value);
                    }
                }
            }
            catch (Exception)
            {
            }
            return packageNames;
        }



        public List<string> GetListOfDevicesUDID()
        {
            List<string> list = new List<string>();
            string deviceListString = ExecuteCommand("devices");
            string[] lines = deviceListString.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 1; i < lines.Length; i++)
            {
                string[] parts = lines[i].Split('\t');
                if (parts.Length == 2 && parts[1].Equals("device"))
                {
                    string udid = parts[0];
                    list.Add(udid);
                }
            }
            return list;
        }

        public void StartAndroidProxyServer(int localPort, int AndroidPort, string udid = "firstDevice")
        {
            if (udid.Equals("firstDevice"))
            {
                ExecuteCommand(" forward tcp:" + localPort + " tcp:" + AndroidPort);
            }
            else
            {
                ExecuteCommand("-s " + udid + " forward tcp:" + localPort + " tcp:" + AndroidPort);
            }
            //var processId = adbProcess.Id;
            //PortProcessId.Add(localPort, processId);   // item already exception occurs if we delete and add device again.. need to check.
        }

        public void StopAndroidProxyServer(string udid, int localPort)
        {
            ExecuteCommand("-s " + udid + " forward --remove tcp:" + localPort);
        }

        public bool IsUIAutomatorRunning(string udid)
        {
            string output = ExecuteCommand("-s " + udid + " shell ps | grep io.appium.uiautomator2.server");
            return output.Contains("io.appium.uiautomator2.server");
        }

        public (int, int) GetScreenSize(string udid)
        {
            var screenDensity = GetScreenDensity(udid);
            var sizeInPixel = GetScreenSizeInPixel(udid);
            int width = (int)(sizeInPixel.Item1 / (screenDensity / 160f)); // converting to dp from pixel, because pixel creates bigger screen
            int height = (int)(sizeInPixel.Item2 / (screenDensity / 160f)); // converting to dp from pixel, because pixel creates bigger screen
            return (width, height);
        }

        public (int, int) GetScreenSizeInPixel(string udid)
        {
            string output = ExecuteCommand("-s " + udid + " shell wm size");
            string pattern = string.Empty;
            if (output.Contains("Override"))
            {
                pattern = @"Override size: (\d+)x(\d+)";
            }
            else
            {
                pattern = @"(\d+)x(\d+)";
            }

            Match match = Regex.Match(output, pattern);

            if (match.Success)
            {
                int widthInPixel = int.Parse(match.Groups[1].Value); // in pixel
                int heightInPixel = int.Parse(match.Groups[2].Value); // in pixel
                return (widthInPixel, heightInPixel);
            }
            else
            {
                return (0, 0);
            }
        }

        public float GetScreenDensity(string udid)
        {
            string output = ExecuteCommand("-s " + udid + " shell wm density");
            string pattern = @"\d+";
            Match match = Regex.Match(output, pattern);
            if (match.Success)
            {
                float densityValue = float.Parse(match.Value);
                return densityValue;
            }
            else
            {
                return 0;
            }
        }

        public static int DpToPixels(int dp, float dpi)
        {
            const float dpFactor = 160f;
            return (int)(dp * (dpi / dpFactor));
        }

        public void Tap(string udid, int x, int y)
        {
            float dpi = 0;
            if (MainScreen.udidScreenDensity.ContainsKey(udid))
            {
                dpi = MainScreen.udidScreenDensity[udid];
            }
            else
            {
                dpi = GetScreenDensity(udid);
                MainScreen.udidScreenDensity[udid] = dpi;
            }
            x = DpToPixels(x, dpi);
            y = DpToPixels(y, dpi);
            ExecuteCommand("-s " + udid + " shell input tap " + x + " " + y + "");
        }

        public void SendText(string udid, string text)
        {
            ExecuteCommand("-s " + udid + " shell input text " + text);
        }

        public void SwipeForScreenControl(string udid, int startX, int startY, int endX, int endY, int duration)
        {
            float dpi = 0;
            if (MainScreen.udidScreenDensity.ContainsKey(udid))
            {
                dpi = MainScreen.udidScreenDensity[udid];
            }
            else
            {
                dpi = GetScreenDensity(udid);
                MainScreen.udidScreenDensity[udid] = dpi;
            }
            startX = DpToPixels(startX, dpi);
            startY = DpToPixels(startY, dpi);
            endX = DpToPixels(endX, dpi);
            endY = DpToPixels(endY, dpi);
            ExecuteCommand("-s " + udid + " shell input swipe " + startX + " " + startY + " " + endX + " " + endY + " " + duration);
        }

        public void GoToHome(string udid)
        {
            ExecuteCommand("-s " + udid + " shell input keyevent KEYCODE_HOME");
        }

        public void Back(string udid)
        {
            ExecuteCommand("-s " + udid + " shell input keyevent KEYCODE_BACK");
        }

        public void LaunchSettings(string udid)
        {
            ExecuteCommand("-s " + udid + " shell am start -a android.settings.SETTINGS");
        }

        public bool InstallApp(string udid, string path)
        {
            path = path.Replace("\"", "");
            path = "\"" + path + "\"";
            var output = ExecuteCommand($"-s {udid} install {path}");
            Logger.Info("install app - " + path);
            Logger.Info("install app output :"+output);
            return output.Contains("Success");
        }

        public string GetAppActivity(string udid, string packageName)
        {
            return ExecuteCommand("-s " + udid + " shell \"cmd package resolve-activity --brief " + packageName + " | tail -n 1\"").Replace("\r", "").Replace("\n", "");
        }

        public void LaunchApp(string udid, string packageName, string activityName)
        {
            ExecuteCommand("-s " + udid + " shell am start -n " + packageName + "/" + activityName);
        }

        public void LaunchApp(string udid, string activityName)
        {
            ExecuteCommand("-s " + udid + " shell am start -n " + activityName);
        }

        public bool isUIAutomatorInstalled(string udid, bool waitForExit = true, int timeout = 0)
        {
            string output = ExecuteCommand("-s " + udid + " shell pm list instrumentation", waitForExit, timeout);
            return output.Contains("io.appium.uiautomator2.server.test/androidx.test.runner.AndroidJUnitRunner") ? true : false;
        }

        public void ReInstallUIAutomator(string udid)
        {
            UnInstallApp(udid, "io.appium.uiautomator2.server");
            UnInstallApp(udid, "io.appium.uiautomator2.server.test");
            InstallUIAutomator(udid);
        }

        public void InstallUIAutomator(string udid)
        {
            string[] serverAPKPath = Directory.GetFiles(serverAPKFilePath, "*.apk");
            foreach (string apkFilePath in serverAPKPath)
            {
                InstallApp(udid, apkFilePath);
            }
            string[] settingsAPKPath = Directory.GetFiles(settingsAPKFilePath, "*.apk");
            foreach (string apkFilePath in settingsAPKPath)
            {
                InstallApp(udid, apkFilePath);
            }
        }

        public void RebootDevice(string udid)
        {
            ExecuteCommand("-s " + udid + " reboot");
        }

        public string GetAndroidIPAddress(string udid)
        {
            string output = ExecuteCommand("-s " + udid + " shell ifconfig");
            return Common.GetTextBetween(output, "inet addr:", "  Bcast").Trim();
        }

        public void EnableWirelssADB(string udid)
        {
            ExecuteCommand("-s " + udid + " tcpip 5555");
        }
        public string ConnectToAndroidWirelessly(string connectAddress)
        {
            return ExecuteCommand(" connect " + connectAddress);
        }

        public void ConnectToAndroidWirelessly(string udid, string IPAddress, string Port)
        {
            ExecuteCommand("-s " + udid + " connect " + IPAddress + ":" + Port);
        }

        public Dictionary<string, Tuple<string, string>> FindPairingReadyDevicesOverWiFi()
        {
            string output = ExecuteCommand("mdns services");
            Dictionary<string, Tuple<string, string>> keyValuePairs = new Dictionary<string, Tuple<string, string>>();
            string pattern = @"(\S+)\s+(_adb-tls-connect|_adb-tls-pairing)\._tcp\.\s+(\d+\.\d+\.\d+\.\d+):(\d+)";
            MatchCollection matches = Regex.Matches(output, pattern);

            foreach (Match match in matches)
            {
                string deviceName = match.Groups[1].Value;
                string[] parts = deviceName.Split('-');
                if (parts.Length >= 3)
                {
                    deviceName = parts[1];
                }
                string service = match.Groups[2].Value;
                string ipAddress = match.Groups[3].Value;
                string portNumber = match.Groups[4].Value;
                string address = ipAddress + ":" + portNumber;
                if (service.Contains("pairing"))
                {
                    if (keyValuePairs.ContainsKey(deviceName))
                    {
                        var tuple = keyValuePairs[deviceName];
                        keyValuePairs[deviceName] = Tuple.Create(address, tuple.Item2);
                    }
                    else
                    {
                        keyValuePairs.Add(deviceName, Tuple.Create(address, ""));
                    }
                }
                else
                {
                    if (keyValuePairs.ContainsKey(deviceName))
                    {
                        var tuple = keyValuePairs[deviceName];
                        keyValuePairs[deviceName] = Tuple.Create(tuple.Item1, address);
                    }
                    else
                    {
                        keyValuePairs.Add(deviceName, Tuple.Create("", address));
                    }
                }
            }
            return keyValuePairs;
        }

        public List<string> FindConnectReadyDevicesOverWiFi()
        {
            List<string> address = new List<string>();
            string output = ExecuteCommand("mdns services");
            string pattern = @"_adb-tls-connect._tcp\.\s+(\d+\.\d+\.\d+\.\d+):(\d+)";

            MatchCollection matches = Regex.Matches(output, pattern);

            foreach (Match match in matches)
            {
                string ipAddress = match.Groups[1].Value;
                string portNumber = match.Groups[2].Value;
                string result = ipAddress + ":" + portNumber;
                address.Add(result);
            }
            return address;
        }

        public string PairAndroidWirelessly(string address, string PairingCode)
        {
            return ExecuteCommand(" pair " + address + " " + PairingCode);
        }

        public void ConnectToAndroidWirelessly(string IPAddress, string Port)
        {
            ExecuteCommand(" connect " + IPAddress + ":" + Port);
        }

        public void DisconnectAndroidWireless(string IPAddress)
        {
            ExecuteCommand(" disconnect " + IPAddress);
        }

        public void StartSettingsApp(string udid)
        {
            ExecuteCommand("-s " + udid + " shell am start -n io.appium.settings/.Settings -a android.intent.action.MAIN -c android.intent.category.LAUNCHER");
        }

        public void AddToWhiteList(string udid)
        {
            ExecuteCommand("-s " + udid + " shell dumpsys deviceidle whitelist +io.appium.settings ; dumpsys deviceidle whitelist +io.appium.uiautomator2.server ; dumpsys deviceidle whitelist +io.appium.uiautomator2.server.test ;");
        }

        public void RelaxHiddenAPIPolicy(string udid)
        {
            ExecuteCommand("-s " + udid + " shell 'settings put global hidden_api_policy_pre_p_apps 1;settings put global hidden_api_policy_p_apps 1;settings put global hidden_api_policy 1'");
        }

        public void UnInstallApp(string udid, string packageName)
        {
            ExecuteCommand("-s " + udid + " uninstall " + packageName);
        }

        public void KillApp(string udid, string packageName)
        {
            ExecuteCommand("-s " + udid + " shell am force-stop " + packageName);
        }

        public void TakeScreenshot(string udid, string path)
        {
            ExecuteCommandWithCmd("-s " + udid + " exec-out screencap -p > " + path);
        }

        public void OpenNotification(string udid)
        {
            ExecuteCommandWithCmd("-s " + udid + " shell cmd statusbar expand-notifications");
        }
        public void CloseNotification(string udid)
        {
            ExecuteCommandWithCmd("-s " + udid + " shell cmd statusbar collapse");
        }

        public void ShowRecentApps(string udid)
        {
            ExecuteCommandWithCmd("-s " + udid + " shell input keyevent KEYCODE_APP_SWITCH");
        }

        public void UnlockScreen(string udid, string password)
        {
            ExecuteCommandWithCmd("-s " + udid + " shell input keyevent 82"); // Wake screen
            Thread.Sleep(500);
            SendText(udid, password);
            Thread.Sleep(300);
            ExecuteCommandWithCmd("-s " + udid + " shell input keyevent 66"); // Enter key
        }

        public bool ClearAppData(string udid, string packageName)
        {
            var output = ExecuteCommandWithCmd("-s " + udid + " shell pm clear " + packageName);
            if (output.Contains("Success"))
            {
                return true;
            }
            return false;
        }

        public void BackSpace(string udid)
        {
            ExecuteCommandWithCmd("-s " + udid + " shell input keyevent 67");
        }

        public void Delete(string udid)
        {
            ExecuteCommandWithCmd("-s " + udid + " shell input keyevent 112");
        }

        public void UpArrow(string udid)
        {
            ExecuteCommandWithCmd("-s " + udid + " shell input keyevent 19");
        }

        public void DownArrow(string udid)
        {
            ExecuteCommandWithCmd("-s " + udid + " shell input keyevent 20");
        }

        public void LeftArrow(string udid)
        {
            ExecuteCommandWithCmd("-s " + udid + " shell input keyevent 21");
        }

        public void RightArrow(string udid)
        {
            ExecuteCommandWithCmd("-s " + udid + " shell input keyevent 22");
        }

        public void LongPress(string udid,  int x, int y)
        {
            float dpi = 0;
            if (MainScreen.udidScreenDensity.ContainsKey(udid))
            {
                dpi = MainScreen.udidScreenDensity[udid];
            }
            else
            {
                dpi = GetScreenDensity(udid);
                MainScreen.udidScreenDensity[udid] = dpi;
            }
            x = DpToPixels(x, dpi);
            y = DpToPixels(y, dpi);
            ExecuteCommandWithCmd("-s " + udid + " shell input touchscreen swipe "+x+" "+y+ " "+x+" "+y+" 1000");
        }

        public List<string> GetListOfInstalledApps(string udid)
        {
            var output = ExecuteCommand("-s " + udid + " shell pm list packages -3 --user 0");
            List<string> packageNames = new List<string>();
            string[] lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                if (line.StartsWith("package:"))
                {
                    string packageName = line.Substring(8);
                    packageNames.Add(packageName);
                }
            }
            return packageNames;
        }

        public void UninstallUIAutomator(string udid)
        {
            AndroidMethods.GetInstance().UnInstallApp(udid, "io.appium.uiautomator2.server");
            AndroidMethods.GetInstance().UnInstallApp(udid, "io.appium.uiautomator2.server.test");
            AndroidMethods.GetInstance().UnInstallApp(udid, "com.experitest.uiautomator.test");
        }
        public string ExecuteCommand(string arguments, bool waitForExit = true, int timeout = 0)
        {
            try
            {
                adbProcess.StartInfo.Arguments = arguments;
                adbProcess.Start();
                bool processExited = false;
                if (waitForExit)
                {
                    if (timeout == 0)
                    {
                        adbProcess.WaitForExit();
                    }
                    else
                    {
                        processExited = adbProcess.WaitForExit(timeout);
                    }
                }
                if (timeout != 0 && !processExited)
                {
                    adbProcess.Kill(); // Kill the process if it did not exit within the timeout
                    return "Process did not complete within the allotted time.";
                }
                string output = adbProcess.StandardOutput.ReadToEnd();
                string error = adbProcess.StandardError.ReadToEnd();
                int processId = adbProcess.Id;
                MainScreen.runningProcesses.Add(processId);
                return output + "\n" + error;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while executing argument " + arguments + ": " + ex.Message);
                return "Exception";
            }
        }

        public string ExecuteCommandWithCmd(string arguments, bool waitForExit = true)
        {
            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo();
                processStartInfo.FileName = "cmd.exe";
                processStartInfo.Arguments = "/C adb.exe " + arguments;
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.WorkingDirectory = FilesPath.executablesFolderPath;
                processStartInfo.RedirectStandardError = true;
                processStartInfo.UseShellExecute = false;
                processStartInfo.CreateNoWindow = true;
                Process process = new Process();
                process.StartInfo = processStartInfo;
                process.Start();
                if (waitForExit)
                {
                    process.WaitForExit();
                }
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                return output + "\n" + error;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while executing argument " + arguments + ": " + ex.Message);
                return "Exception";
            }
        }

        public Dictionary<string, string> GetApkInformation(string apkPath)
        {
            Dictionary<string, string> apkInfo = new Dictionary<string, string>();

            ProcessStartInfo processInfo = new ProcessStartInfo
            {
                FileName = aaptFilePath,
                Arguments = $"dump badging \"{apkPath}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process())
            {
                process.StartInfo = processInfo;
                process.Start();

                while (!process.StandardOutput.EndOfStream)
                {
                    string line = process.StandardOutput.ReadLine() ?? string.Empty;

                    if (line.StartsWith("application-label:", StringComparison.OrdinalIgnoreCase))
                    {
                        string appName = line.Substring(line.IndexOf(":") + 1).Trim('\'');
                        apkInfo.Add("AppName", appName);
                    }

                    if (line.StartsWith("package:", StringComparison.OrdinalIgnoreCase))
                    {

                        string[] parts = line.Split(' ');
                        foreach (string part in parts)
                        {
                            string[] keyValue = part.Split('=');
                            if (keyValue.Length == 2)
                            {
                                string key = keyValue[0].Trim();
                                string value = keyValue[1].Trim('\'');
                                if (key.Equals("name", StringComparison.OrdinalIgnoreCase))
                                {
                                    apkInfo.Add("PackageName", value);
                                }
                                else if (key.Equals("versionName", StringComparison.OrdinalIgnoreCase))
                                {
                                    apkInfo.Add("Version", value);
                                }
                            }
                        }
                    }
                    if (line.StartsWith("launchable-activity:", StringComparison.OrdinalIgnoreCase))
                    {
                        string[] parts = line.Split(' ');
                        foreach (string part in parts)
                        {
                            if (part.StartsWith("name=", StringComparison.OrdinalIgnoreCase))
                            {
                                string activityName = part.Split('=')[1].Trim('\'');
                                apkInfo.Add("ActivityName", activityName);
                                break;
                            }
                        }
                    }
                }
            }
            return apkInfo;
        }

        public int GetForwardedPort(string udid, int port)
        {
            try
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = adbFilePath,
                    Arguments = "-s " + udid + " forward --list",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(processStartInfo))
                {
                    string output = process?.StandardOutput.ReadToEnd() ?? string.Empty;
                    string[] lines = output.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string line in lines)
                    {
                        string[] parts = line.Split(' ');
                        string device = parts[0];
                        int sourcePort = int.Parse(parts[2].Split(':')[1]);
                        int destinationPort = int.Parse(parts[1].Split(':')[1]);

                        if (device == udid)
                        {
                            if (port == sourcePort)
                            {
                                return destinationPort;
                            }
                        }
                    }
                    process?.Close();
                    return -1;
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }


        public static AndroidMethods GetInstance()
        {
            if (instance == null)
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new AndroidMethods();
                    }
                }
            }
            return instance;
        }
        private AndroidMethods()
        {
            InitializeProcess();
        }

        private void InitializeProcess()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = adbFilePath,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true

            };

            adbProcess = new Process();
            adbProcess.StartInfo = startInfo;
        }
    }

    public class AndroidAsyncMethods
    {
        private static AndroidAsyncMethods? instance;
        private static readonly object lockObject = new object();
        private string adbFilePath = FilesPath.adbFilePath;
        private Process adbAsyncProcess = new Process();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void StartUIAutomatorServer(string udid, bool stop)
        {
            // if fails, then only need to uninstall.. need to add a condition here..
            //UninstallUIAutomator(udid);
            if (!AndroidMethods.GetInstance().isUIAutomatorInstalled(udid))
            {
                AndroidMethods.GetInstance().InstallUIAutomator(udid);
            }
            else
            {
                AndroidMethods.GetInstance().StopUIAutomator(udid);
            }
            AndroidMethods.GetInstance().AddToWhiteList(udid);
            Thread.Sleep(1000);
            //ExecuteCommand("-s " + udid + " shell am instrument -w io.appium.uiautomator2.server.test/androidx.test.runner.AndroidJUnitRunner", false);
            ExecuteCommand("-s " + udid + " shell am instrument -w -e disableAnalytics true io.appium.uiautomator2.server.test/androidx.test.runner.AndroidJUnitRunner", false);

        }

        public void StartUIAutomatorServer(string udid)
        {
            // if fails, then only need to uninstall.. need to add a condition here..
            //UninstallUIAutomator(udid);
            //if (!AndroidMethods.GetInstance().isUIAutomatorInstalled(udid))
            //{
            //    InstallUIAutomator(udid);
            //}
            //else
            //{
            //    StopUIAutomator(udid);
            //}
            AndroidMethods.GetInstance().AddToWhiteList(udid);
            Thread.Sleep(1000);
            //ExecuteCommand("-s " + udid + " shell am start -n io.appium.settings/.Settings");
            //ExecuteCommand("-s " + udid + " shell am instrument -w io.appium.uiautomator2.server.test/androidx.test.runner.AndroidJUnitRunner", false);
            //AndroidMethods.GetInstance().StartSettingsApp(udid);
            ExecuteCommand("-s " + udid + " shell am instrument -w -e disableAnalytics true io.appium.uiautomator2.server.test/androidx.test.runner.AndroidJUnitRunner", false);
            //Thread.Sleep(5000);
        }



        public void ExecuteCommand(string arguments, bool waitForExit = true)
        {
            try
            {
                adbAsyncProcess.StartInfo.Arguments = arguments;
                adbAsyncProcess.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while executing argument " + arguments + ": " + ex.Message);
            }
        }

        public Dictionary<string, string> GetDeviceInformation(string udid)
        {
            string deviceInfoString = GetInstance().ExecuteCommand2("-s " + udid + " shell getprop");
            Dictionary<string, string> deviceInfo = new Dictionary<string, string>();
            string[] lines = deviceInfoString.Split("\r\n");

            foreach (string line in lines)
            {
                int separatorIndex = line.IndexOf(':');
                if (separatorIndex != -1)
                {
                    string key = line.Substring(1, separatorIndex - 2).Trim();
                    string value = line.Substring(separatorIndex + 2, line.Length - separatorIndex - 3).Trim();
                    value = value.TrimStart('[');
                    if (!deviceInfo.Keys.Contains(key))
                    {
                        deviceInfo.Add(key, value);
                    }
                }
            }
            string deviceName = AndroidMethods.GetInstance().ExecuteCommand("-s " + udid + " shell settings get global device_name").Replace("\r\n", "");
            deviceInfo.Add("deviceName", deviceName);
            var screenSize = AndroidMethods.GetInstance().GetScreenSize(udid);
            deviceInfo.Add("Width", screenSize.Item1.ToString());
            deviceInfo.Add("Height", screenSize.Item2.ToString());
            return deviceInfo;
        }
        public string ExecuteCommand2(string arguments, bool waitForExit = true)
        {
            try
            {
                adbAsyncProcess.StartInfo.Arguments = arguments;
                StringBuilder outputBuilder = new StringBuilder();
                StringBuilder errorBuilder = new StringBuilder();

                adbAsyncProcess.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        outputBuilder.AppendLine(e.Data);
                    }
                };

                adbAsyncProcess.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        errorBuilder.AppendLine(e.Data);
                    }
                };

                adbAsyncProcess.Start();
                adbAsyncProcess.BeginOutputReadLine();
                adbAsyncProcess.BeginErrorReadLine();

                if (waitForExit)
                {
                    adbAsyncProcess.WaitForExit();
                }

                string output = outputBuilder.ToString();
                string error = errorBuilder.ToString();
                adbAsyncProcess.CancelErrorRead();
                adbAsyncProcess.CancelOutputRead();
                return output + "\n" + error;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while executing argument " + arguments + ": " + ex.Message);
                return "Exception";
            }
        }

        public static AndroidAsyncMethods GetInstance()
        {
            if (instance == null)
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new AndroidAsyncMethods();
                    }
                }
            }
            return instance;
        }
        private AndroidAsyncMethods()
        {
            InitializeProcess();
        }

        private void InitializeProcess()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = adbFilePath,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true

            };

            adbAsyncProcess = new Process();
            adbAsyncProcess.StartInfo = startInfo;
        }
    }

    public class AndroidAPIMethods
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public static string CreateSession(int proxyPort)
        {
            var options = new RestClientOptions("http://localhost:" + proxyPort)
            {
                Timeout = TimeSpan.FromSeconds(5),
            };
            var client = new RestClient(options);
            var request = new RestRequest("/session", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            //var body = $@"{{""capabilities"":{{""platformName"":""Android"",""automationName"":""UiAutomator2"",""newCommandTimeout"":0}}}}";
            var body = $@"{{""capabilities"":{{""platformName"":""Android"",""automationName"":""UiAutomator2""}}}}";
            //var body = $@"{{""capabilities"":{{""platformName"":""Android"",""automationName"":""UiAutomator2"",""newCommandTimeout"":0,""ensureWebviewsHavePages"":true,""takesScreenshot"":true,""javascriptEnabled"":true,""mjpegServerPort"":{screenPort}}}}}";
            //var body = "{\"capabilities\":{\"firstMatch\":[{\"platformName\":\"Android\",\"automationName\":\"UiAutomator2\",\"newCommandTimeout\":0,\"mjpegServerPort\":5555,\"ensureWebviewsHavePages\":true,\"nativeWebScreenshot\":true,\"connectHardwareKeyboard\":true,\"webDriverAgentUrl\":\"http://localhost:51436\",\"platform\":\"LINUX\",\"webStorageEnabled\":false,\"takesScreenshot\":true,\"javascriptEnabled\":true,\"databaseEnabled\":false,\"networkConnectionEnabled\":true,\"locationContextEnabled\":false,\"warnings\":{},\"desired\":{\"platformName\":\"Android\",\"automationName\":\"UiAutomator2\",\"newCommandTimeout\":0,\"mjpegServerPort\":5555,\"ensureWebviewsHavePages\":true,\"nativeWebScreenshot\":true,\"connectHardwareKeyboard\":true,\"webDriverAgentUrl\":\"http://localhost:51436\"}},\"deviceName\":\"R5CN3172FHT\",\"deviceUDID\":\"R5CN3172FHT\"}],\"alwaysMatch\":{}}";

            //var body = $@"{{""capabilities"":{{""platformName"":""Android"",""automationName"":""UiAutomator2"",""newCommandTimeout"":0,""mjpegServerPort"":{screenPort}}}}}";
            //var body = $@"{{""capabilities"":{{""platformName"":""Android"",""automationName"":""UiAutomator2"",""appium:newCommandTimeout"":0,""mjpegServerPort"":{screenPort},""mjpegScreenshotUrl"":""http://localhost:{screenPort}/mjpeg""}}}}";
            request.AddStringBody(body, DataFormat.Json);
            RestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            if (response.Content == null)
            {
                return "nosession";
            }
            else
            {
                JObject jsonObject = JObject.Parse(response.Content);
                string sessionId = jsonObject?["sessionId"]?.ToString() ?? "nosession";
                return sessionId;
            }
        }

        public static bool isUIAutomatorSessionStarted(int proxyPort)
        {
            try
            {
                var options = new RestClientOptions("http://localhost:" + proxyPort)
                {
                    Timeout = TimeSpan.FromSeconds(5),
                };
                var client = new RestClient(options);
                var request = new RestRequest("/status", Method.Get);
                RestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);
                if (response.Content == null)
                {
                    return false;
                }
                else
                {
                    return response.Content.Contains("UiAutomator2 Server is ready to accept commands") ? true : false;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
        }

        public static string GetSessionID(int proxyPort)
        {
            try
            {
                var options = new RestClientOptions("http://localhost:" + proxyPort)
                {
                    Timeout = TimeSpan.FromSeconds(5),
                };
                var client = new RestClient(options);
                var request = new RestRequest("/sessions", Method.Get);
                RestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);
                string sessionId = "nosession";
                if (response.Content != null)
                {
                    JObject obj = JObject.Parse(response.Content);
                    sessionId = (string?)obj["value"]?[0]?["id"] ?? string.Empty;
                }
                if (sessionId == null)
                {
                    sessionId = "nosession";
                }
                return sessionId;
            }
            catch (Exception)
            {
                return "nosession";
            }
        }


        public static void DeleteSession(int proxyPort, string sessionId)
        {
            //string sessionId = GetSessionID(proxyPort);
            var options = new RestClientOptions("http://localhost:" + proxyPort)
            {
                Timeout = TimeSpan.FromSeconds(5),
            };
            var client = new RestClient(options);
            var request = new RestRequest("/session/" + sessionId, Method.Delete);
            RestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
        }

        public static void DeleteSession(int proxyPort)
        {
            string sessionId = GetSessionID(proxyPort);
            if (!sessionId.Equals("nosession"))
            {
                var options = new RestClientOptions("http://localhost:" + proxyPort)
                {
                    Timeout = TimeSpan.FromSeconds(5),
                };
                var client = new RestClient(options);
                var request = new RestRequest("/session/" + sessionId, Method.Delete);
                RestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);
            }
        }

        public static string GetPageSource(int port, string sessionId="")
        {
            string value = "empty";
            try
            {
                string URL = "http://localhost:" + port;
                if (string.IsNullOrEmpty(sessionId))
                {
                    sessionId = GetSessionID(port);
                }

                // Attempt to get the page source with the retrieved session ID
                value = AttemptGetPageSource(URL, sessionId);

                // If the value indicates an invalid session, create a new session and retry
                if (value == "Invalid session")
                {
                    CreateSession(port);
                    sessionId = GetSessionID(port);

                    if (!sessionId.Equals("nosession"))
                    {
                        value = AttemptGetPageSource(URL, sessionId);
                    }
                    else
                    {
                        value = "Failed to create a new session.";
                    }
                }

                return value;
            }
            catch (Exception ex)
            {
                return "Exception while getting page source : " + ex.Message;
            }
        }

        // Helper method to attempt to get the page source
        private static string AttemptGetPageSource(string URL, string sessionId)
        {
            try
            {
                var options = new RestClientOptions(URL)
                {
                    Timeout = TimeSpan.FromSeconds(30)
                };
                var client = new RestClient(options);
                var request = new RestRequest("/session/" + sessionId + "/source", Method.Get);
                RestResponse response = client.Execute(request);

                if (!response.IsSuccessful || response.Content.Contains("invalid session id"))
                {
                    // Check if the response indicates an invalid session
                    return "Invalid session";
                }

                if (response.Content != null)
                {
                    using (JsonDocument doc = JsonDocument.Parse(response.Content))
                    {
                        JsonElement root = doc.RootElement;
                        return root.GetProperty("value").GetString() ?? "empty";
                    }
                }

                return "empty";
            }
            catch
            {
                return "Invalid session";
            }
        }

        public static Image TakeScreenshotWithSessionId(int port, string sessionId = "")
        {
            Image image = null;
            try
            {
                string URL = "http://localhost:" + port;

                // If sessionId is not provided or invalid, get a new session ID
                if (string.IsNullOrEmpty(sessionId))
                {
                    sessionId = GetSessionID(port);
                }

                // Attempt to take a screenshot with the provided or new sessionId
                image = AttemptTakeScreenshot(URL, sessionId);

                // If the image is null, it indicates an invalid session, create a new session and retry
                if (image == null)
                {
                    Console.WriteLine("Session is invalid. Creating a new session...");
                    CreateSession(port);
                    sessionId = GetSessionID(port);

                    if (!sessionId.Equals("nosession"))
                    {
                        image = AttemptTakeScreenshot(URL, sessionId);
                    }
                    else
                    {
                        Console.WriteLine("Failed to create a new session.");
                    }
                }

                return image;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return image;
            }
        }

        private static Image AttemptTakeScreenshot(string URL, string sessionId)
        {
            try
            {
                var options = new RestClientOptions(URL)
                {
                    Timeout = TimeSpan.FromSeconds(15)
                };
                var client = new RestClient(options);
                var request = new RestRequest("/session/" + sessionId + "/screenshot", Method.Get);
                RestResponse response = client.Execute(request);

                string jsonString = response.Content;

                if (!response.IsSuccessful || jsonString.Contains("invalid session id"))
                {
                    // Check if the response indicates an invalid session
                    return null;
                }

                JsonDocument doc = JsonDocument.Parse(jsonString);
                string base64String = doc.RootElement.GetProperty("value").GetString();

                byte[] imageBytes = Convert.FromBase64String(base64String);
                using (MemoryStream ms = new MemoryStream(imageBytes))
                {
                    return Image.FromStream(ms);
                }
            }
            catch
            {
                return null;
            }
        }

        public static string TakeScreenshot(int port, string filePath)
        {
            Image image = null;
            try
            {
                string sessionId = GetSessionID(port);
                if (sessionId.Equals("nosession"))
                {
                    CreateSession(port);
                    sessionId = GetSessionID(port);
                }
                var options = new RestClientOptions("http://localhost:" + port)
                {
                    Timeout = TimeSpan.FromSeconds(10)
                };
                var client = new RestClient(options);
                var request = new RestRequest("/session/" + sessionId + "/screenshot", Method.Get);
                RestResponse response = client.Execute(request);
                string jsonString = response.Content;

                JsonDocument doc = JsonDocument.Parse(jsonString);
                string base64String = doc.RootElement.GetProperty("value").GetString();

                byte[] imageBytes = Convert.FromBase64String(base64String);
                using (MemoryStream ms = new MemoryStream(imageBytes))
                {
                    image = Image.FromStream(ms);
                    image.Save(filePath, ImageFormat.Png);
                }
                return response.StatusDescription;
            }
            catch (Exception)
            {
                return "Exception";
            }
        }

        public async static void DragDrop(string udid, string sessionId, int port, int startX, int startY, int endX, int endY, int speed=1000)
        {
            try
            {

                float dpi = 0;
                if (MainScreen.udidScreenDensity.ContainsKey(udid))
                {
                    dpi = MainScreen.udidScreenDensity[udid];
                }
                else
                {
                    dpi = AndroidMethods.GetInstance().GetScreenDensity(udid);
                    MainScreen.udidScreenDensity[udid] = dpi;
                }
                startX = AndroidMethods.DpToPixels(startX, dpi);
                startY = AndroidMethods.DpToPixels(startY, dpi);
                endX = AndroidMethods.DpToPixels(endX, dpi);
                endY = AndroidMethods.DpToPixels(endY, dpi);

                var options = new RestClientOptions("http://localhost:" + port)
                {
                    Timeout = TimeSpan.FromSeconds(10)
                };
                var client = new RestClient(options);
                var request = new RestRequest("/session/"+sessionId+"/appium/gestures/drag", Method.Post);
                request.AddHeader("Content-Type", "application/json");
                string body = $@"{{""start"":{{""x"":{startX},""y"":{startY}}},""end"":{{""x"":{endX},""y"":{endY}}},""speed"":{speed}}}";
                request.AddStringBody(body, DataFormat.Json);
                RestResponse response = await client.ExecuteAsync(request);
            }
            catch (Exception)
            {
            }            
        }

        public static string FindElement(string URL, string sessionId, string XPath)
        {
            string elementId = string.Empty;
            try
            {
                var options = new RestClientOptions(URL)
                {
                    Timeout = TimeSpan.FromSeconds(10)
                };
                var client = new RestClient(options);
                var request = new RestRequest("/session/" + sessionId + "/element", Method.Post);
                request.AddHeader("Content-Type", "application/json");
                //string body = $@"{{""value"": ""{XPath}"",""using"": ""xpath""}}";
                string body = $@"{{""strategy"":""xpath"",""selector"":""{XPath}""}}";
                request.AddStringBody(body, DataFormat.Json);
                RestResponse response = client.Execute(request);
                if (response.Content != null)
                {
                    JObject jsonObject = JObject.Parse(response.Content);
                    elementId = jsonObject["value"]?["ELEMENT"]?.ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return elementId;
        }

        public static string FindElement(string URL, string json)
        {
            string elementId = string.Empty;
            try
            {
                var options = new RestClientOptions(URL)
                {
                    Timeout = TimeSpan.FromSeconds(10)
                };
                var client = new RestClient(options);
                var request = new RestRequest(URL + "/element", Method.Post);
                request.AddHeader("Content-Type", "application/json");
                string body = json.Replace("\"using\"", "\"strategy\"").Replace("\"value\"", "\"selector\"");
                request.AddStringBody(body, DataFormat.Json);
                RestResponse response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK && response.Content != null)
                {
                    JObject jsonObject = JObject.Parse(response.Content);
                    elementId = jsonObject["value"]?["ELEMENT"]?.ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return elementId;
        }

        public static string ClickElement(string udid, string URL, string sessionId, string XPath)
        {
            string elementId = FindElement(URL, sessionId, XPath);
            GetElementRectAndDraw(udid, URL, sessionId, elementId, "dot");
            var options = new RestClientOptions(URL)
            {
                Timeout = TimeSpan.FromSeconds(10)
            };
            var client = new RestClient(options);
            var request = new RestRequest("/session/" + sessionId + "/element/" + elementId + "/click", Method.Post);
            RestResponse response = client.Execute(request);
            return response.StatusDescription;
        }

        public static string SendText(string udid, string URL, string sessionId, string XPath, string text)
        {
            string elementId = FindElement(URL, sessionId, XPath);
            GetElementRectAndDraw(udid,URL,sessionId,elementId,"rect");
            var options = new RestClientOptions(URL)
            {
                Timeout = TimeSpan.FromSeconds(10)
            };
            var client = new RestClient(options);
            var request = new RestRequest("/session/" + sessionId + "/element/" + elementId + "/value", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            string[] valueArray = text.ToCharArray().Select(c => c.ToString()).ToArray();
            string body = $@"{{
                                ""text"":""{text}"",
                                ""replace"":false,
                                ""value"":{JsonConvert.SerializeObject(valueArray)}
                            }}";
            request.AddStringBody(body, DataFormat.Json);
            RestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            return response.StatusDescription;
        }

        public static bool isElementDisplayed(string URL, string sessionId, string XPath)
        {
            try
            {
                var options = new RestClientOptions(URL)
                {
                    Timeout = TimeSpan.FromSeconds(10)
                };
                var client = new RestClient(options);
                var request = new RestRequest("/session/"+sessionId+"/element", Method.Post);
                request.AddHeader("Content-Type", "application/json");
                var body = $@"{{""strategy"":""xpath"",""selector"":""{XPath}""}}";
                request.AddStringBody(body, DataFormat.Json);
                RestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);
                return response.StatusCode.Equals(HttpStatusCode.OK); ;
            }
            catch (Exception)
            {
                return false;
            }
        }

        static Dictionary<string,int> udidScreenDensity = new Dictionary<string,int>();
        public static void GetElementRectAndDraw(string udid, string URL, string sessionId, string elementId, string draw)
        {
            try
            {
                var options = new RestClientOptions(URL)
                {
                    // Timeout = TimeSpan.FromSeconds(1)
                };
                var client = new RestClient(options);
                var request = new RestRequest("/session/" + sessionId + "/element/" + elementId + "/rect", Method.Get);
                RestResponse response = client.Execute(request);
                dynamic data = JsonConvert.DeserializeObject(response.Content);
                int x = Convert.ToInt32(data.value.x);
                int y = Convert.ToInt32(data.value.y);
                int width = Convert.ToInt32(data.value.width);
                int height = Convert.ToInt32(data.value.height);
                ScreenControl screenControl = ScreenControl.udidScreenControl[udid];
                int screenDensity = 0;
                if (udidScreenDensity.ContainsKey(udid))
                {
                    screenDensity = udidScreenDensity[udid];
                }
                else
                {
                    screenDensity = (int)AndroidMethods.GetInstance().GetScreenDensity(udid);
                    udidScreenDensity.Add(udid, screenDensity);
                }
                if (screenDensity != 0)
                {
                    x = (int)(x / (screenDensity / 160f));
                    y = (int)(y / (screenDensity / 160f));
                    width = (int)(width / (screenDensity / 160f));
                    height = (int)(height / (screenDensity / 160f));
                }
                if (draw == "dot")
                {
                    int updatedX = x + (width / 2);
                    int updatedY = y + (height / 2);
                    screenControl.DrawDot(screenControl, updatedX, updatedY);
                }
                else
                {
                    screenControl.DrawRectangle(screenControl, x, y, width, height);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
