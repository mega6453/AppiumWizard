using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium.Remote;
using RestSharp;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Appium_Wizard
{
    public class iOSMethods
    {
        private static iOSMethods? instance;
        private static readonly object lockObject = new object();

        private string iOSServerFilePath = FilesPath.iOSServerFilePath;
        private string iOSFilesPath = FilesPath.iOSFilesPath;
        private string pListUtilFilePath = FilesPath.pListUtilFilePath;
        string ProfilesFilePath = FilesPath.ProfilesFilePath;
        private Process iOSProcess;
        public bool serverStarted = false;
        public string statusText = "";
        public static string deviceList = "";
        public string deviceInfo = "";
        public enum iOSExecutable { go, py }
        public static bool isGo;
        public List<string> GetListOfDevicesUDID(iOSExecutable executable = iOSExecutable.go)
        {
            List<string> list = new List<string>();
            if (executable == iOSExecutable.go)
            {
                string deviceListString = ExecuteCommand("list");
                if (deviceListString.Contains("dial tcp 127.0.0.1:27015: connectex: No connection could be made because the target machine actively refused it"))
                {
                    list.Add("ITunes not installed");
                }
                else
                {
                    JObject json = JObject.Parse(deviceListString);
                    JArray deviceList = (JArray)json["deviceList"];
                    foreach (JToken device in deviceList)
                    {
                        string udid = device.ToString();
                        list.Add(udid);
                    }
                }
                return list;
            }
            else
            {
                string deviceListString = ExecuteCommandPy("pymobiledevice3 usbmux list");
                List<dynamic> devices = JsonConvert.DeserializeObject<List<dynamic>>(deviceListString);
                foreach (dynamic device in devices)
                {
                    list.Add(device.UniqueDeviceID.ToString());
                }
                return list;
            }
        }

        public Dictionary<string, object> GetDeviceInformation(string udid, iOSExecutable executable = iOSExecutable.go)
        {
            if (executable == iOSExecutable.go)
            {
                string output = ExecuteCommand("info", udid);
                Dictionary<string, object> deviceValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(output);
                return deviceValues;
            }
            else
            {
                string output = ExecuteCommandPy("pymobiledevice3 usbmux list");
                List<dynamic> devices = JsonConvert.DeserializeObject<List<dynamic>>(output);

                Dictionary<string, object> deviceValues = new Dictionary<string, object>();
                foreach (dynamic device in devices)
                {
                    string uniqueDeviceID = device.UniqueDeviceID.ToString();
                    if (uniqueDeviceID == udid)
                    {
                        foreach (var property in device)
                        {
                            string key = property.Name.ToString();
                            string value = property.Value;
                            deviceValues[key] = value;
                        }
                        break;
                    }
                }
                return deviceValues;
            }
        }

        public static string iOSConnectedVia(bool HostAttached)
        {
            if (HostAttached.Equals(true))
            {
                return "USB";
            }
            else
            {
                return "Wi-Fi";
            }
        }

        public List<string> GetListOfInstalledApps(string udid, iOSExecutable executable = iOSExecutable.go)
        {
            List<string> packageList = new List<string>();
            if (executable == iOSExecutable.go)
            {
                var output = ExecuteCommand("apps --list", udid);
                string pattern = @"(\w+(\.\w+)*)\s";
                MatchCollection matches = Regex.Matches(output, pattern);

                foreach (Match match in matches)
                {
                    bool containsAlphabets = Regex.IsMatch(match.Groups[1].Value, @"[a-zA-Z]");
                    if (containsAlphabets && match.Groups[1].Value.Contains("."))
                    {
                        packageList.Add(match.Groups[1].Value);
                    }
                }
                return packageList;
            }
            else
            {
                var output = ExecuteCommandPy("pymobiledevice3 apps list -t User", udid);
                JObject data = JObject.Parse(output);
                var keys = data.Properties().Select(p => p.Name);

                foreach (var key in keys)
                {
                    packageList.Add(key);
                }
                return packageList;
            }
        }

        public bool iSWDAInstalled(string udid)
        {
            bool isInstalled = GetListOfInstalledApps(udid).Contains("com.facebook.WebDriverAgentRunner.xctrunner");
            return isInstalled;
        }

        public bool isPasswordProtected(string udid)
        {
            bool isProtected = ExecuteCommand("devicestate list", udid).Contains("PasswordProtected");
            return isProtected;
        }

        public bool isDeveloperModeDisabled(string udid)
        {
            if (isGo)
            {
                bool isDevModeDisabled = ExecuteCommand("devicestate list", udid).Contains("Could not start service:com.apple.instruments.remoteserver.DVTSecureSocketProxy with reason:'InvalidService'");
                return isDevModeDisabled;
            }
            else
            {
                return ExecuteCommandPy("pymobiledevice3 amfi developer-mode-status", udid).Contains("true");
            }
        }

        public void RebootDevice(string udid, iOSExecutable executable = iOSExecutable.go)
        {
            if (executable.Equals(iOSExecutable.go))
            {
                ExecuteCommand("reboot", udid);
            }
            else
            {
                ExecuteCommandPy("pymobiledevice3 diagnostics restart", udid);
            }
        }

        public void UninstallApp(string udid, string bundleId, iOSExecutable executable = iOSExecutable.go)
        {
            if (executable.Equals(iOSExecutable.go))
            {
                ExecuteCommand("uninstall " + bundleId, udid);
            }
            else
            {
                ExecuteCommandPy("pymobiledevice3 developer core-device uninstall " + bundleId, udid);
            }
        }

        public void TakeScreenshot(string udid, string path)
        {
            if (isGo)
            {
                ExecuteCommand("screenshot --output=" + path, udid);
            }
            else
            {
                ExecuteCommandPy("pymobiledevice3 developer dvt screenshot " + path, udid);
            }
        }

        public string RunWebDriverAgent(CommonProgress commonProgress, string udid, int port)
        {
            string sessionId = "nosession";
            int count = 6;
            try
            {
                bool isInstalled = iSWDAInstalled(udid);
                if (isInstalled)
                {
                    //iOSAsyncProcess.StartInfo.Arguments = "runwda";
                    sessionId = IsWDARunning(port);
                    if (sessionId.Equals("nosession"))
                    {
                        //iOSProcess.StartInfo.Arguments = "runwda" + " --udid=" + udid;
                        iOSProcess.StartInfo.Arguments = "launch com.facebook.WebDriverAgentRunner.xctrunner" + " --udid=" + udid;
                        iOSProcess.Start();
                        Thread.Sleep(2000);
                        iOSProcess.WaitForExit();
                        string output = iOSProcess.StandardOutput.ReadToEnd();
                        string error = iOSProcess.StandardError.ReadToEnd();
                        if (error.Contains("Could not start service"))
                        {
                            sessionId = "Enable Developer Mode";
                        }
                        else
                        {
                            bool isPasscodeRequired = false;
                            if (error.Contains("Process started successfully") | output.Contains("Process started successfully"))
                            {
                                sessionId = IsWDARunning(port);
                                while (sessionId.Equals("nosession") && count <= 6 && count > 0)
                                {
                                    sessionId = iOSAPIMethods.CreateWDASession(port);
                                    if (sessionId.Equals("nosession") & IsWDARunningInAppsList(udid))
                                    {
                                        int seconds = 5 * count;
                                        commonProgress.UpdateStepLabel("Please enter Passcode on your iPhone to continue...Will timeout in " + seconds.ToString() + " seconds.");
                                        isPasscodeRequired = true;
                                    }
                                    Thread.Sleep(5000);
                                    count--;
                                }
                            }
                            if (sessionId.Equals("nosession") & isPasscodeRequired)
                            {
                                return "nosession passcode required";
                            }
                            else
                            {
                                return sessionId;
                            }

                            if (!sessionId.Equals("nosession"))
                            {
                                return sessionId;
                            }
                            else
                            {
                                return sessionId;
                            }

                            count = 6;
                            //var progressLabelOriginalColor = progressLabel.ForeColor;
                            while (sessionId.Equals("nosession") && IsWDARunningInAppsList(udid) && count <= 6 && count > 0)
                            {
                                int seconds = 5 * count;
                                commonProgress.UpdateStepLabel("Open Device", "Please enter Passcode on your iPhone...This popup will close in " + seconds.ToString() + " seconds...");
                                //progressLabel.Text = "Please enter Passcode on your iPhone...This popup will close in " + seconds.ToString() + " seconds...";
                                //progressLabel.ForeColor = Color.Red;
                                //progressLabel.Refresh();
                                Thread.Sleep(5000);
                                count--;
                                //progressLabel.ForeColor = progressLabelOriginalColor;
                                //progressLabel.Refresh();
                                sessionId = IsWDARunning(port);
                                if (sessionId.Equals("nosession"))
                                {
                                    sessionId = iOSAPIMethods.CreateWDASession(port);
                                }
                            }
                        }
                    }
                }
                else
                {
                    InstallWDA(udid);
                    RunWebDriverAgentQuick(udid);
                    sessionId = IsWDARunning(port);
                    if (sessionId.Equals("nosession"))
                    {
                        sessionId = iOSAPIMethods.CreateWDASession(port);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while running WebDriverAgent: " + ex.Message);
            }
            return sessionId;
        }

        public void InstallWDA(string udid)
        {
            string WDAPath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\Resources\\iOS\\wda.ipa";
            string signedIPA = SignIPA(udid, WDAPath);
            if (!signedIPA.Equals("notsigned"))
            {
                iOSAsyncMethods.GetInstance().InstallApp(udid, signedIPA);
                Thread.Sleep(3000);
            }
        }

        public (bool, string) isProfileAvailableToSign(string udid)
        {
            try
            {
                string certificatPath = "";
                List<string> deviceList; int expirationDays;
                bool flag = false;
                string[] profileFolders = Directory.GetDirectories(ProfilesFilePath);
                foreach (string profileFolder in profileFolders)
                {
                    string[] provisioningFiles = Directory.EnumerateFiles(profileFolder, "*.mobileprovision").ToArray();
                    if (provisioningFiles.Length > 0)
                    {
                        foreach (string provisioningFile in provisioningFiles)
                        {
                            var provisionDetails = ImportProfile.GetDetailsFromProvisionFile(provisioningFile);
                            deviceList = (List<string>)provisionDetails["DevicesList"];
                            expirationDays = ImportProfile.ExpirationDays(provisionDetails["ExpirationDate"].ToString());
                            if (deviceList.Contains(udid) && expirationDays > 0)
                            {
                                flag = true;
                                certificatPath = profileFolder;
                                break;
                            }
                        }
                    }
                    if (flag)
                    {
                        break;
                    }
                }
                return (flag, certificatPath);
            }
            catch (Exception)
            {
                return (false, "");
            }
        }

        public bool isProfileHasUDID(string profileFolder, string udid)
        {
            try
            {
                string certificatPath = "";
                List<string> deviceList; int expirationDays;
                bool flag = false;
                string[] provisioningFiles = Directory.EnumerateFiles(profileFolder, "*.mobileprovision").ToArray();
                if (provisioningFiles.Length > 0)
                {
                    foreach (string provisioningFile in provisioningFiles)
                    {
                        var provisionDetails = ImportProfile.GetDetailsFromProvisionFile(provisioningFile);
                        deviceList = (List<string>)provisionDetails["DevicesList"];
                        expirationDays = ImportProfile.ExpirationDays(provisionDetails["ExpirationDate"].ToString());
                        if (deviceList.Contains(udid) && expirationDays > 0)
                        {
                            flag = true;
                            certificatPath = profileFolder;
                            break;
                        }
                    }
                }
                return flag;
            }
            catch (Exception)
            {
                return false;
            }
        }


        static string stackTrace = "";
        public string SignIPA(string udid, string IPAFilePath)
        {
            var output = isProfileAvailableToSign(udid);
            bool isProfileAvailable = output.Item1;
            string certificatPath = output.Item2;
            if (isProfileAvailable)
            {
                string tempFolder = Path.GetTempPath();
                tempFolder = Path.Combine(tempFolder, "Appium_Wizard");
                Directory.CreateDirectory(tempFolder);
                string signedIPAFilePath = tempFolder + "\\signedIPA.ipa";
                return SignIPA(certificatPath, IPAFilePath, signedIPAFilePath);
            }
            else
            {
                return "ProfileNotAvailable";
            }
        }


        public string SignIPA(string profilePath, string IPAFilePath, string outputPath, string udid)
        {
            if (udid != "")
            {
                bool isProfileAvailable = isProfileHasUDID(profilePath, udid);
                if (isProfileAvailable)
                {
                    return SignIPA(profilePath, IPAFilePath, outputPath);
                }
                else
                {
                    return "ProfileNotAvailable";
                }
            }
            else
            {
                return SignIPA(profilePath, IPAFilePath, outputPath);
            }
        }

        private string SignIPA(string profilePath, string IPAFilePath, string outputPath)
        {
            string[] pemFiles = Directory.GetFiles(profilePath, "*.pem");
            string[] mobileprovisionFiles = Directory.GetFiles(profilePath, "*.mobileprovision");
            string pemFileName = pemFiles.Length > 0 ? Path.GetFileName(pemFiles[0]) : null;
            string mobileprovisionFileName = mobileprovisionFiles.Length > 0 ? Path.GetFileName(mobileprovisionFiles[0]) : null;
            string[] commands = {
                $"set PATH=\"{iOSFilesPath}\";%PATH%",
                $"cd \"{profilePath}\"",
                $"zsign -k \"{pemFileName}\" -m \"{mobileprovisionFileName}\" -z 9 -o \"{outputPath}\" \"{IPAFilePath}\""
                 };

            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/K";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardInput = true;

            process.OutputDataReceived += Process_OutputDataReceived;
            process.ErrorDataReceived += Process_ErrorDataReceived;
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            foreach (string command in commands)
            {
                process.StandardInput.WriteLine(command);
                process.StandardInput.WriteLine("echo Command completed");
            }
            process.StandardInput.Close();
            process.WaitForExit();
            int exitCode = process.ExitCode;
            process.Close();

            if (stackTrace == "Archive Failed")
            {
                return "Sign_IPA_Failed";
            }
            else if (stackTrace == "Archive OK")
            {
                return outputPath;
            }
            else
            {
                return "Sign_IPA_Failed";
            }
        }

        private static void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                if (e.Data.Contains("Archive Failed"))
                {
                    stackTrace = "Sign IPA Archive Failed";
                }
                if (e.Data.Contains("Archive OK"))
                {
                    stackTrace = "Archive OK";
                }
            }
        }

        private static void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                if (e.Data.Contains("Archive Failed"))
                {
                    stackTrace = "Sign IPA Archive Failed";
                }
                if (e.Data.Contains("Archive OK"))
                {
                    stackTrace = "Archive OK";
                }
            }
        }

        public static string GetLinuxPathFromInput(string WindowsPath)
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/C wsl wslpath -u \"{WindowsPath}\"";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();
            string output = process.StandardOutput.ReadToEnd().Trim();
            process.WaitForExit();
            return "\"" + output + "\"";

        }


        public string RunWebDriverAgentQuick(string udid)
        {
            if (isGo)
            {
                iOSProcess.StartInfo.Arguments = "launch com.facebook.WebDriverAgentRunner.xctrunner" + " --udid=" + udid;
                iOSProcess.Start();
                return "";
            }
            else
            {
                return ExecuteCommandPy("pymobiledevice3 developer dvt launch com.facebook.WebDriverAgentRunner.xctrunner", udid);
            }
        }

        public string IsWDARunning(int port)
        {
            try
            {
                var options = new RestClientOptions("http://localhost:" + port)
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest("/status", Method.Get);
                RestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);
                if (response.Content != null)
                {
                    if (response.Content.Contains("WebDriverAgent is ready to accept commands"))
                    {
                        JObject jObject = JObject.Parse(response.Content);
                        string sessionId = (string)jObject["sessionId"];
                        if (sessionId == null)
                        {
                            sessionId = iOSAPIMethods.CreateWDASession(port);
                        }
                        return sessionId;
                    }
                    else
                    {
                        return "nosession";
                    }
                }
                else
                {
                    return "nosession";
                }
            }
            catch (Exception)
            {
                return "nosession";
            }
        }

        public bool IsWDARunningInAppsList(string udid)
        {
            bool isRunning = false; string output = "";
            if (isGo)
            {
                output = ExecuteCommand("ps --apps", udid, false);
            }
            else
            {
                output = ExecuteCommandPy("pymobiledevice3 developer core-device list-processes");
            }
            if (output.Contains("WebDriverAgentRunner-Runner"))
            {
                isRunning = true;
            }
            return isRunning;
        }

        public string MountImage(string udid)
        {
            if (isGo)
            {
                return ExecuteCommand("image auto", udid);
            }
            else
            {
                return ExecuteCommandPy("pymobiledevice3 mounter auto-mount", udid);
            }
        }

        public Dictionary<string, string> GetIPAInformation(string ipaFilePath)
        {
            Dictionary<string, string> output = new Dictionary<string, string>();
            var tempFolder = Path.GetTempPath();
            var plistFilePath = Common.ExtractInfoPlistFromIPA(ipaFilePath, tempFolder);
            if (plistFilePath != null)
            {
                string xmlString = ExecutePlistUtil(plistFilePath);
                output = Common.GetValueFromXml(xmlString);
            }
            return output;
        }

        public string ExecutePlistUtil(string plistFilePath)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = pListUtilFilePath,
                Arguments = "-i " + plistFilePath + " -f xml",
                //Arguments = $"-i \"{plistFilePath}"+ " -f xml",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process())
            {
                process.StartInfo = startInfo;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                return output;
            }
        }

        public string LaunchApp(string udid, string bundleId)
        {
            if (isGo)
            {
                return ExecuteCommand("launch " + bundleId, udid);
            }
            else
            {
                return ExecuteCommandPy("pymobiledevice3 developer dvt launch " + bundleId, udid);
            }
        }
        public string KillApp(string udid, string bundleId)
        {
            if (isGo)
            {
                return ExecuteCommand("kill " + bundleId, udid);
            }
            else
            {
                return ExecuteCommandPy("pymobiledevice3 developer dvt pkill --bundle " + bundleId, udid);
            }
        }

        public string ExecuteCommand(string arguments, string udid = "any", bool waitForExit = true)
        {
            try
            {
                if (udid.Equals("any"))
                {
                    iOSProcess.StartInfo.Arguments = arguments;
                }
                else
                {
                    iOSProcess.StartInfo.Arguments = arguments + " --udid=" + udid;
                }
                iOSProcess.Start();
                if (waitForExit)
                {
                    iOSProcess.WaitForExit();
                }
                string output = iOSProcess.StandardOutput.ReadToEnd();
                string error = iOSProcess.StandardError.ReadToEnd();
                if (output != "")
                {
                    return output;
                }
                else
                {
                    return error;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while executing argument " + arguments + ": " + ex.Message);
                return "Exception";
            }
        }

        public string ExecuteCommandPy(string command, string udid = "")
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            if (udid == "")
            {
                process.StartInfo.Arguments = "/C " + command;
            }
            else
            {
                process.StartInfo.Arguments = "/C " + command + " --udid " + udid;
            }
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            string error = process.StandardError.ReadToEnd();
            string output = process.StandardOutput.ReadToEnd();
            string result = output + error;
            process.WaitForExit();
            result = Regex.Replace(result, @"\x1B\[[0-9;]*[mK]", string.Empty);
            return result;
        }

        public string GetDeviceModel(string input)
        {
            var filePath = FilesPath.AppleDeviceTypesFilePath;
            var AppleDeviceTypesRawData = File.ReadAllText(filePath);
            if (!AppleDeviceTypesRawData.Contains(input))
            {
                var url = "https://gist.githubusercontent.com/adamawolf/3048717/raw/1ee7e1a93dff9416f6ff34dd36b0ffbad9b956e9/Apple_mobile_device_types.txt";
                AppleDeviceTypesRawData = new WebClient().DownloadString(url);
            }
            Dictionary<string, string> AppleDeviceTypes = new Dictionary<string, string>();
            string model;
            StringReader reader = new StringReader(AppleDeviceTypesRawData);
            while ((model = reader.ReadLine()) != null)
            {
                try
                {
                    string[] value = model.Split(':');
                    AppleDeviceTypes.Add(value[0].Trim(), value[1].Trim());
                }
                catch (Exception)
                {
                    continue;
                }
            };
            return AppleDeviceTypes[input].Trim();
        }


        private iOSMethods()
        {
            InitializeProcess();
        }

        public static iOSMethods GetInstance()
        {
            if (instance == null)
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new iOSMethods();
                    }
                }
            }
            return instance;
        }

        private void InitializeProcess()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = iOSServerFilePath,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            iOSProcess = new Process();
            iOSProcess.StartInfo = startInfo;
        }
    }

    //-------------------------------------------------------------iOSAsyncMethods--------------------------------------------------------------------

    public class iOSAsyncMethods
    {
        private string iOSServerFilePath = FilesPath.iOSServerFilePath;
        private string iProxyFilePath = FilesPath.iProxyFilePath;

        private static iOSAsyncMethods? instance;
        private static readonly object lockObject = new object();
        private Process iOSAsyncProcess;
        private Process pyAsyncProcess;
        private StringBuilder outputBuffer;
        public static Dictionary<int, int>? PortProcessId;
        public enum iOSExecutable { go, py }
        public static bool isGo;
        public iOSAsyncMethods()
        {
            InitializeProcess();
            PortProcessId = new Dictionary<int, int>();
        }

        public static iOSAsyncMethods GetInstance()
        {
            if (instance == null)
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new iOSAsyncMethods();
                    }
                }
            }

            return instance;
        }

        private void InitializeProcess()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = iOSServerFilePath,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            iOSAsyncProcess = new Process();
            iOSAsyncProcess.StartInfo = startInfo;
            outputBuffer = new StringBuilder();

            iOSAsyncProcess.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Console.WriteLine(e.Data);
                    outputBuffer.AppendLine(e.Data);
                }
            };
        }

        public void StartiOSProxyServer(string udid, int localPort, int iOSPort)
        {
            if (isGo)
            {
                try
                {
                    iOSAsyncProcess.StartInfo.Arguments = "forward --udid=" + udid + " " + localPort + " " + iOSPort + "";
                    iOSAsyncProcess.Start();
                    var processId = iOSAsyncProcess.Id;
                    if (!PortProcessId.ContainsKey(localPort))
                    {
                        PortProcessId.Add(localPort, processId);
                    }
                    MainScreen.runningProcessesPortNumbers.Add(localPort);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred while forwarding port: " + ex.Message);
                }
            }
            else
            {
                try
                {
                    pyAsyncProcess = new Process();
                    pyAsyncProcess.StartInfo.FileName = "cmd.exe";
                    pyAsyncProcess.StartInfo.Arguments = $"/C pymobiledevice3 usbmux forward " + udid + " " + localPort + " " + iOSPort + "";
                    pyAsyncProcess.StartInfo.RedirectStandardError = true;
                    pyAsyncProcess.StartInfo.RedirectStandardOutput = true;
                    pyAsyncProcess.StartInfo.UseShellExecute = false;
                    pyAsyncProcess.StartInfo.CreateNoWindow = true;
                    pyAsyncProcess.Start();
                    var processId = pyAsyncProcess.Id;
                    if (!PortProcessId.ContainsKey(localPort))
                    {
                        PortProcessId.Add(localPort, processId);
                    }
                    MainScreen.runningProcessesPortNumbers.Add(localPort);
                }
                catch (Exception)
                {
                }
            }
        }

        public void StartiProxyServer(string udid, int localPort, int iOSPort)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = iProxyFilePath,
                Arguments = localPort + ":" + iOSPort + " -u " + udid,
                //Arguments = localPort + " " + iOSPort + " " + udid,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true
            };
            Process iProxyProcess = new Process
            {
                StartInfo = startInfo
            };
            iProxyProcess.Start();
            var processId = iProxyProcess.Id;
            if (!PortProcessId.ContainsKey(localPort))
            {
                PortProcessId.Add(localPort, processId);
            }
            MainScreen.runningProcessesPortNumbers.Add(localPort);
        }

        public void StartiProxyServer(string udid, int localPort1, int iOSPort1, int localPort2, int iOSPort2)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = iProxyFilePath,
                Arguments = localPort1 + ":" + iOSPort1 + " " + localPort2 + ":" + iOSPort2 + " -u " + udid,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true
            };
            Process iProxyProcess = new Process
            {
                StartInfo = startInfo
            };
            iProxyProcess.Start();
            var processId = iProxyProcess.Id;
            if (!PortProcessId.ContainsKey(localPort1))
            {
                PortProcessId.Add(localPort1, processId);
            }
            MainScreen.runningProcessesPortNumbers.Add(localPort1);
        }

        public void StartiProxyServer(int localPort, int iOSPort)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = iProxyFilePath,
                Arguments = localPort + " " + iOSPort,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true
            };
            Process iProxyProcess = new Process
            {
                StartInfo = startInfo
            };
            iProxyProcess.Start();
            var processId = iProxyProcess.Id;
            if (!PortProcessId.ContainsKey(localPort))
            {
                PortProcessId.Add(localPort, processId);
            }
            MainScreen.runningProcessesPortNumbers.Add(localPort);
        }

        public void InstallApp(string udid, string path, iOSExecutable executable = iOSExecutable.go)
        {
            if (executable == iOSExecutable.go)
            {
                try
                {
                    iOSAsyncProcess.StartInfo.Arguments = $"install --path=\"{path}\" --udid=" + udid;
                    iOSAsyncProcess.OutputDataReceived += InstallOutput;
                    iOSAsyncProcess.ErrorDataReceived += InstallOutput;
                    iOSAsyncProcess.Start();
                    iOSAsyncProcess.BeginErrorReadLine();
                    iOSAsyncProcess.BeginOutputReadLine();
                    iOSAsyncProcess.WaitForExit();
                }
                catch (Exception ex)
                {
                }
            }
            else
            {
                try
                {
                    pyAsyncProcess = new Process();
                    pyAsyncProcess.StartInfo.FileName = "cmd.exe";
                    pyAsyncProcess.StartInfo.Arguments = $"/C pymobiledevice3 apps install \"{path}\" --udid " + udid;
                    pyAsyncProcess.OutputDataReceived += InstallOutputPy;
                    pyAsyncProcess.ErrorDataReceived += InstallOutputPy;
                    pyAsyncProcess.StartInfo.RedirectStandardError = true;
                    pyAsyncProcess.StartInfo.RedirectStandardOutput = true;
                    pyAsyncProcess.StartInfo.UseShellExecute = false;
                    pyAsyncProcess.StartInfo.CreateNoWindow = true;
                    pyAsyncProcess.Start();
                    pyAsyncProcess.BeginErrorReadLine();
                    pyAsyncProcess.BeginOutputReadLine();
                    pyAsyncProcess.WaitForExit();
                }
                catch (Exception ex)
                {
                }
            }
        }

        public static string installationProgress = "0";
        private void InstallOutput(object sender, DataReceivedEventArgs e)
        {
            int percentComplete = 0;
            if (e.Data != null)
            {
                if (e.Data.Contains("percentComplete"))
                {
                    JObject jsonObject = JObject.Parse(e.Data);
                    percentComplete = jsonObject["percentComplete"].Value<int>();
                    installationProgress = percentComplete.ToString();
                }
                else if (e.Data.Contains("error"))
                {
                    JObject jsonObject = JObject.Parse(e.Data);
                    installationProgress = jsonObject["err"].ToString();
                }
            }
            else
            {
                iOSAsyncProcess.CancelErrorRead();
                iOSAsyncProcess.CancelOutputRead();
            }
        }

        private void InstallOutputPy(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                if (e.Data.Contains("Complete"))
                {
                    string pattern = @"\b(\d+)%";
                    Regex regex = new Regex(pattern);
                    Match match = regex.Match(e.Data);
                    if (match.Success)
                    {
                        installationProgress = match.Groups[1].Value;
                    }
                }
                else if (e.Data.Contains("error"))
                {
                    installationProgress = e.Data;
                }
            }
            else
            {
                pyAsyncProcess.CancelErrorRead();
                pyAsyncProcess.CancelOutputRead();
            }
        }

        string runwdaOutput = "", runwdaError = "";
        public string RunWebDriverAgent(CommonProgress commonProgress, string udid, int port)
        {
            if (isGo)
            {
                // Create a new process
                Process process = new Process();

                // Configure the process
                process.StartInfo.FileName = iOSServerFilePath;
                process.StartInfo.Arguments = "runwda";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.EnableRaisingEvents = true;

                // Register event handlers for error and output data received
                process.ErrorDataReceived += Process_ErrorDataReceived;
                process.OutputDataReceived += Process_OutputDataReceived;

                // Start the process
                process.Start();
                var processId = process.Id;
                MainScreen.runningProcesses.Add(processId);
                Thread.Sleep(2000);
                // Begin asynchronous reading of the output/error streams
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();

                // Wait for the process to exit
                //process.WaitForExit();

                // Clean up resources
                //process.Close();
                bool isPasscodeRequired = false;
                int count = 1;
                string sessionId = string.Empty;
                while (!process.HasExited)
                {
                    if (!string.IsNullOrEmpty(runwdaError))
                    {
                        if (runwdaError.Contains("Process started successfully"))
                        {
                            sessionId = iOSMethods.GetInstance().IsWDARunning(port);
                            while (sessionId.Equals("nosession") && count <= 6)
                            {
                                sessionId = iOSAPIMethods.CreateWDASession(port);
                                Thread.Sleep(5000);
                                sessionId = iOSAPIMethods.CreateWDASession(port);
                                if (sessionId.Equals("nosession") & iOSMethods.GetInstance().IsWDARunningInAppsList(udid))
                                {
                                    commonProgress.UpdateStepLabel("Please enter Passcode on your iPhone to continue...Retrying in 5 seconds...\nRetry " + count + "/6.");
                                    isPasscodeRequired = true;
                                    Thread.Sleep(5000);
                                }
                                count++;
                            }
                            //if (runwdaError.Contains("Timed out while enabling automation mode"))
                            //{
                            //    return "Timed out";
                            //}
                            if (sessionId.Equals("nosession") & isPasscodeRequired)
                            {
                                process.Close();
                                return "nosession passcode required";
                            }
                            else
                            {
                                process.Close();
                                sessionId = iOSMethods.GetInstance().IsWDARunning(port);
                                return sessionId;
                            }
                            //if (runwdaError.Contains("Could not start service:com.apple.testmanagerd.lockdown.secure with reason:'InvalidService'"))
                            //{
                            //    return "Enable Developer Mode";
                            //}
                            //if (runwdaError.Contains("Could not start service:com.apple.testmanagerd.lockdown.secure with reason:'PasswordProtected'"))
                            //{
                            //    return "Password Protected";
                            //}

                        }
                    }
                }
                if (runwdaError.Contains("Could not start service:com.apple.testmanagerd.lockdown.secure"))
                {
                    return "Enable Developer Mode";
                }
                if (runwdaError.Contains("Could not start service:com.apple.testmanagerd.lockdown.secure with reason:'PasswordProtected'"))
                {
                    return "Password Protected";
                }
                if (runwdaError.Contains("Timed out while enabling automation mode"))
                {
                    return "Timed out";
                }
                return "unhandled";
            }
            else
            {
                return iOSMethods.GetInstance().RunWebDriverAgentQuick(udid);
            }
        }
        public void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Console.WriteLine("Error: " + e.Data);
                runwdaError += e.Data;
            }
        }

        // Event handler for output data received
        public void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Console.WriteLine("Output: " + e.Data);
                runwdaOutput += e.Data;
            }
        }

        public void CreateTunnel()
        {
            bool isProcessRunning = false;
            int processId = Database.QueryDataFromTunnelTable();
            if (processId != 0)
            {
                isProcessRunning = Common.isProcessIdExist(processId);
            }            
            if (processId == 0 | !isProcessRunning)
            {
                var result = MessageBox.Show("Starting at iOS 17.0, Apple introduced a new CoreDevice framework to work with iOS devices.\n\nIn order to communicate with the developer services you'll be required to first create trusted tunnel using a command.\n\nThis command must be run with high privileges since it creates a new TUN/TAP device which is a high privilege operation.\n\nSo click OK to grant permission to create the tunnel in the next windows prompt which will allow to run iOS 17+ automation OR Click cancel to read the official apple comment about the change.", "Admin Privilege Required", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                if (result == DialogResult.OK)
                {
                    try
                    {
                        Process tunnelProcess = new Process();
                        tunnelProcess.StartInfo.FileName = "cmd.exe";
                        tunnelProcess.StartInfo.Arguments = $"/C pymobiledevice3 remote tunneld";
                        tunnelProcess.StartInfo.UseShellExecute = true;
                        tunnelProcess.StartInfo.CreateNoWindow = false;
                        tunnelProcess.StartInfo.Verb = "runas";
                        tunnelProcess.Start();
                        processId = tunnelProcess.Id;
                        Database.UpdateDataIntoTunnelTable(processId);
                        //if (!PortProcessId.ContainsKey(localPort))
                        //{
                        //    PortProcessId.Add(localPort, processId);
                        //}
                        //MainScreen.runningProcessesPortNumbers.Add(localPort);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("As admin permission has not been given, unable to continue with the request. Please try again by providing admin permission.", "Admin Permission denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                else
                {
                    try
                    {
                        ProcessStartInfo psInfo = new ProcessStartInfo
                        {
                            FileName = "https://developer.apple.com/forums/thread/730947?answerId=756665022#756665022",
                            UseShellExecute = true
                        };
                        Process.Start(psInfo);
                        GoogleAnalytics.SendEvent("iOS17_Admin_Cancel");
                    }
                    catch (Exception exception)
                    {
                        GoogleAnalytics.SendExceptionEvent("iOS17_Admin_Cancel", exception.Message);
                    }
                }
            }
        }
    }

    //-------------------------------------------------------------iOSAPIMethods--------------------------------------------------------------------

    public class iOSAPIMethods
    {
        public static string CreateWDASession(int proxyPort)
        {
            var options = new RestClientOptions("http://localhost:" + proxyPort)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/session", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            var body = @"{""capabilities"":{}}";
            //var body = $@"{{""capabilities"":{{""mjpegServerPort"":{screenPort},""mjpegScreenshotUrl"": ""http://localhost:{screenPort}""}}}}";
            request.AddStringBody(body, DataFormat.Json);
            RestResponse response = client.Execute(request);
            string sessionId = "nosession";
            if (!string.IsNullOrEmpty(response.Content))
            {
                try
                {
                    JObject jsonObject = JObject.Parse(response.Content);
                    sessionId = (string)jsonObject["value"]["sessionId"];
                }
                catch (Exception)
                {
                }
            }
            return sessionId;
        }

        public static (int, int) GetScreenSize(string sessionId, int proxyPort)
        {
            int width, height;
            var options = new RestClientOptions("http://localhost:" + proxyPort)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/session/" + sessionId + "/window/size", Method.Get);
            RestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                dynamic jsonObj = JsonConvert.DeserializeObject(response.Content);
                width = jsonObj.value.width;
                height = jsonObj.value.height;
            }
            else
            {
                throw new Exception(response.Content);
            }
            return (width, height);
        }

        public static void Tap(string URL, string sessionId, int pressX, int pressY)
        {
            var options = new RestClientOptions(URL);
            var client = new RestClient(options);
            var request = new RestRequest("/session/" + sessionId + "/wda/touch/perform", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            var body = $@"{{""actions"": [{{ ""action"": ""press"", ""options"": {{ ""x"": {pressX}, ""y"": {pressY} }} }},{{ ""action"": ""release"" }}]}}";
            request.AddStringBody(body, DataFormat.Json);
            RestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
        }

        public static void SendText(string URL, string sessionId, string text)
        {
            var options = new RestClientOptions(URL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/session/" + sessionId + "/actions", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            var body = $@"{{""actions"": [{{""type"": ""key"",""id"": ""keyboard"",""actions"":[{{ ""type"": ""keyDown"", ""value"": ""{text}"" }},{{ ""type"": ""keyUp"", ""value"": ""{text}"" }}]}}]}}";
            request.AddStringBody(body, DataFormat.Json);
            RestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
        }

        public static void Swipe(string URL, string sessionId, int pressX, int pressY, int moveToX, int moveToY, int waitDuration)
        {
            var options = new RestClientOptions(URL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/session/" + sessionId + "/wda/touch/perform", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            var body = $@"{{""actions"":[{{""action"":""press"",""options"":{{""x"":{pressX},""y"":{pressY}}}}},{{""action"":""wait"",""options"":{{""ms"":{waitDuration}}}}},{{""action"":""moveTo"",""options"":{{""x"":{moveToX},""y"":{moveToY}}}}},{{""action"":""release"",""options"":{{}}}}]}}";
            request.AddStringBody(body, DataFormat.Json);
            RestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
        }

        public static void GoToHome(string URL)
        {
            var options = new RestClientOptions(URL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/wda/homescreen", Method.Post);
            var body = @"";
            request.AddParameter("text/plain", body, ParameterType.RequestBody);
            RestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
        }

        public static void OpenControlCenter(string URL, string sessionId, int screenWidth, int screenHeight)
        {
            var options = new RestClientOptions(URL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/session/" + sessionId + "/wda/touch/perform", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            int waitDuration = 500;
            int fromX = screenWidth - 50;
            int fromY = 50;
            int endX = fromX;
            int endY = screenHeight;
            var body = $@"{{""actions"":[{{""action"":""press"",""options"":{{""x"":{fromX},""y"":{fromY}}}}},{{""action"":""wait"",""options"":{{""ms"":{waitDuration}}}}},{{""action"":""moveTo"",""options"":{{""x"":{endX},""y"":{endY}}}}},{{""action"":""release"",""options"":{{}}}}]}}";
            request.AddStringBody(body, DataFormat.Json);
            RestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
        }

        public static void CloseControlCenter(string URL, string sessionId, int screenWidth)
        {
            var options = new RestClientOptions(URL);
            var client = new RestClient(options);
            var request = new RestRequest("/session/" + sessionId + "/wda/touch/perform", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            int x = screenWidth / 2;
            int y = 50;
            var body = $@"{{""actions"": [{{ ""action"": ""press"", ""options"": {{ ""x"": {x}, ""y"": {y} }} }},{{ ""action"": ""release"" }}]}}";
            request.AddStringBody(body, DataFormat.Json);
            RestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
        }

        public static string GetWDASessionID(string URL)
        {
            try
            {
                var options = new RestClientOptions(URL)
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest("/sessions", Method.Get);
                RestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);
                dynamic jsonObject = JsonConvert.DeserializeObject(response.Content);
                string sessionId = jsonObject.sessionId != null ? jsonObject.sessionId : "nosession";
                return sessionId;
            }
            catch (Exception)
            {
                return "nosession";
            }
        }

        public bool IsSessionValid(string URL, string sessionId)
        {
            var options = new RestClientOptions(URL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/session/" + sessionId + "", Method.Get);
            RestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine("Session is valid");
                return true;
            }
            else
            {
                Console.WriteLine("Session is not valid");
                return false;
            }
        }


    }
}
