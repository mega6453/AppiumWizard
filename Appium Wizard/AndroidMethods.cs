using Newtonsoft.Json.Linq;
using RestSharp;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Appium_Wizard
{
    public class AndroidMethods
    {
        private static AndroidMethods instance;
        private static readonly object lockObject = new object();
        private string adbFilePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\Resources\\Executables\\adb.exe";
        private string aaptFilePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\Resources\\Executables\\aapt.exe";
        private string serverAPKFilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)+"\\.appium\\node_modules\\appium-uiautomator2-driver\\node_modules\\appium-uiautomator2-server\\apks\\";
        private string settingsAPKFilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\.appium\\node_modules\\appium-uiautomator2-driver\\node_modules\\io.appium.settings\\apks\\";
        private Process adbProcess;
        public static Dictionary<int, int> PortProcessId = new Dictionary<int, int>();

        public void StartAdbServer(int AdbPort)
        {
            ProcessStartInfo adbStartInfo = new ProcessStartInfo
            {
                FileName = adbFilePath, // Replace with the path to your adb executable if not in PATH
                Arguments = "-p " + AdbPort + " start-server",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process adbProcess = new Process { StartInfo = adbStartInfo };
            adbProcess.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);
            adbProcess.ErrorDataReceived += (sender, e) => Console.WriteLine(e.Data);

            adbProcess.Start();
            adbProcess.BeginOutputReadLine();
            adbProcess.BeginErrorReadLine();

            // Wait for the ADB server to start (you can adjust the sleep duration based on your needs)
            Thread.Sleep(2000);
        }

        public void StopAdbServer(int AdbPort)
        {
            ProcessStartInfo adbStopInfo = new ProcessStartInfo
            {
                FileName = adbFilePath, // Replace with the path to your adb executable if not in PATH
                Arguments = "-p" + AdbPort + " kill-server",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process adbProcess = new Process { StartInfo = adbStopInfo };
            adbProcess.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);
            adbProcess.ErrorDataReceived += (sender, e) => Console.WriteLine(e.Data);

            adbProcess.Start();
            adbProcess.BeginOutputReadLine();
            adbProcess.BeginErrorReadLine();

            adbProcess.WaitForExit();
        }

        public void StopUIAutomator(string udid)
        {
            ExecuteCommand("-s " + udid + " shell am force-stop io.appium.uiautomator2.server.test", false);
            ExecuteCommand("-s " + udid + " shell am force-stop io.appium.uiautomator2.server", false);
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
            string pattern = @"Override size: (\d+)x(\d+)";
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
            float dpi = GetScreenDensity(udid);
            x = DpToPixels(x, dpi);
            y = DpToPixels(y, dpi);
            ExecuteCommand("-s " + udid + " shell input tap " + x + " " + y + "");
        }

        public void SendText(string udid, string text)
        {
            ExecuteCommand("-s " + udid + " shell input text " + text);
        }

        public void Swipe(string udid, int startX, int startY, int endX, int endY, int duration)
        {
            float dpi = GetScreenDensity(udid);
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

        public bool InstallApp(string udid, string path)
        {
           var output = ExecuteCommand($"-s \"{udid}\" install \"{path}\"");
           return output.Contains("Success");
        }
        
        public void LaunchApp(string udid,string packageName,string activityName)
        {
            ExecuteCommand("-s " + udid + " shell am start -n " +packageName+"/"+activityName);
        }

        public bool isUIAutomatorInstalled(string udid)
        {
            string output = ExecuteCommand("-s " + udid + " shell pm list instrumentation");
            return output.Contains("io.appium.uiautomator2.server.test/androidx.test.runner.AndroidJUnitRunner") ? true : false;
        }

        public void ReInstallUIAutomator(string udid)
        {
            UnInstallApp(udid,"io.appium.uiautomator2.server");
            UnInstallApp(udid,"io.appium.uiautomator2.server.test");
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


        public void AddToWhiteList(string udid)
        {
            ExecuteCommand("-s " + udid + " shell dumpsys deviceidle whitelist +io.appium.settings ; dumpsys deviceidle whitelist +io.appium.uiautomator2.server ; dumpsys deviceidle whitelist +io.appium.uiautomator2.server.test ;");
        }

        public void UnInstallApp(string udid, string packageName)
        {
            ExecuteCommand("-s " + udid + " uninstall " + packageName);
        }

        public string ExecuteCommand(string arguments, bool waitForExit = true)
        {
            try
            {
                adbProcess.StartInfo.Arguments = arguments;
                adbProcess.Start();
                if (waitForExit)
                {
                    adbProcess.WaitForExit();
                }
                string output = adbProcess.StandardOutput.ReadToEnd();
                string error = adbProcess.StandardError.ReadToEnd();
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
                    string line = process.StandardOutput.ReadLine();

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
        private static AndroidAsyncMethods instance;
        private static readonly object lockObject = new object();
        private string adbFilePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\Resources\\Executables\\adb.exe";
        private Process adbAsyncProcess;
        public void StartUIAutomatorServer(string udid,bool stop)
        {
            // if fails, then only need to uninstall.. need to add a condition here..
            //UninstallUIAutomator(udid);
            if (!AndroidMethods.GetInstance().isUIAutomatorInstalled(udid))
            {
                InstallUIAutomator(udid);
            }
            else
            {
                StopUIAutomator(udid);
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
            ExecuteCommand("-s " + udid + " shell am instrument -w -e disableAnalytics true io.appium.uiautomator2.server.test/androidx.test.runner.AndroidJUnitRunner", false);
            Thread.Sleep(5000);
        }

        public void StopUIAutomator(string udid)
        {
            ExecuteCommand("-s " + udid + " shell am force-stop io.appium.uiautomator2.server.test", false);
            ExecuteCommand("-s " + udid + " shell am force-stop io.appium.uiautomator2.server", false);
        }

        public void InstallUIAutomator(string udid)
        {
            string username = Environment.UserName;
            string settingsPath = "C:\\Users\\" + username + "\\node_modules\\appium-uiautomator2-driver\\node_modules\\io.appium.settings\\apks\\settings_apk-debug.apk";
            AndroidMethods.GetInstance().ExecuteCommand("-s " + udid + " install " + settingsPath);
            string testPath = "C:\\Users\\mc\\node_modules\\appium-uiautomator2-driver\\node_modules\\appium-uiautomator2-server\\apks\\appium-uiautomator2-server-debug-androidTest.apk";
            AndroidMethods.GetInstance().ExecuteCommand("-s " + udid + " install " + testPath);
            string serverPath = "C:\\Users\\mc\\node_modules\\appium-uiautomator2-driver\\node_modules\\appium-uiautomator2-server\\apks\\appium-uiautomator2-server-v5.12.2.apk";
            AndroidMethods.GetInstance().ExecuteCommand("-s " + udid + " install " + serverPath);
        }

        public void UninstallUIAutomator(string udid)
        {
            AndroidMethods.GetInstance().UnInstallApp(udid, "io.appium.uiautomator2.server");
            AndroidMethods.GetInstance().UnInstallApp(udid, "io.appium.uiautomator2.server.test");
            AndroidMethods.GetInstance().UnInstallApp(udid, "com.experitest.uiautomator.test");
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
            deviceInfo.Add("Width",screenSize.Item1.ToString());
            deviceInfo.Add("Height",screenSize.Item2.ToString());
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

        public static string CreateSession(int proxyPort,int screenPort)
        {
            var options = new RestClientOptions("http://localhost:" + proxyPort)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/session", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            var body = $@"{{""capabilities"":{{""platformName"":""Android"",""automationName"":""UiAutomator2"",""newCommandTimeout"":0}}}}";
            //var body = $@"{{""capabilities"":{{""platformName"":""Android"",""automationName"":""UiAutomator2"",""newCommandTimeout"":0,""mjpegServerPort"":{screenPort}}}}}";
            //var body = $@"{{""capabilities"":{{""platformName"":""Android"",""automationName"":""UiAutomator2"",""appium:newCommandTimeout"":0,""mjpegServerPort"":{screenPort},""mjpegScreenshotUrl"":""http://localhost:{screenPort}""}}}}";
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
                    MaxTimeout = -1,
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
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest("/sessions", Method.Get);
                RestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);
                JObject obj = JObject.Parse(response.Content);
                string sessionId = (string)obj["value"][0]["id"];
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


        public static void DeleteSession(int proxyPort)
        {
            string sessionId = GetSessionID(proxyPort);
            var options = new RestClientOptions("http://localhost:" + proxyPort)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/session/" + sessionId, Method.Delete);
            RestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
        }

    }
}
