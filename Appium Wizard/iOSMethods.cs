using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Net;
using System.Text;
using System.Text.Json;
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
                string deviceListString = ExecuteCommand("list", "any", true, 10000);
                if (deviceListString.Contains("Process did not complete within the allotted time"))
                {
                    return GetListOfDevicesUDID(iOSExecutable.py);
                }
                else
                {
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
            }
            else
            {
                string deviceListString = ExecuteCommandPy("usbmux list");
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
                string output = ExecuteCommand("info", udid, true, 10000);
                if (output.Contains("Process did not complete within the allotted time")
                    | output.Contains("failed getting info"))
                {
                    return GetDeviceInformation(udid, iOSExecutable.py);
                }
                else
                {
                    Dictionary<string, object> deviceValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(output);
                    return deviceValues;
                }
            }
            else
            {
                string output = ExecuteCommandPy("usbmux list");
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
                var output = ExecuteCommandPy("apps list -t User", udid);
                JObject data = JObject.Parse(output);
                var keys = data.Properties().Select(p => p.Name);

                foreach (var key in keys)
                {
                    packageList.Add(key);
                }
                return packageList;
            }
        }

        public string GetInstalledAppVersion(string udid, string bundleId)
        {
            try
            {
                string output = ExecuteCommand("apps --list", udid, true, 10000);
                string pattern = $@"{Regex.Escape(bundleId)}\s+[^\s]+\s+([^\s]+)";
                var match = Regex.Match(output, pattern);

                if (match.Success)
                {
                    return match.Groups[1].Value;
                }
                return "failedToGetVersion";
            }
            catch (Exception)
            {
                return "failedToGetVersion";
            }
        }

        public string GetInstalledWDAVersion(string udid)
        {
            return GetInstalledAppVersion(udid, "com.facebook.WebDriverAgentRunner.xctrunner");
        }

        public bool isLatestVersionWDAInstalled(string udid)
        {
            try
            {
                string installedWDAVersion = GetInstalledWDAVersion(udid);
                if (installedWDAVersion.Equals("failedToGetVersion"))
                {
                    return false;
                }
                else
                {
                    string ipaWDAVersion = GetWDAIPAVersion();
                    Version installed = new Version(installedWDAVersion);
                    Version ipa = new Version(ipaWDAVersion);
                    return installed >= ipa;
                }

            }
            catch (Exception)
            {
                return false;
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

        public bool isDeveloperModeDisabled(string udid, iOSExecutable executable = iOSExecutable.go)
        {
            bool isDevModeDisabled = false;
            if (executable == iOSExecutable.go)
            {
                string output = ExecuteCommand("devmode get", udid);
                if (output.Contains("Developer mode enabled: true"))
                {
                    return true;
                }
                return isDevModeDisabled;
            }
            else
            {
                return ExecuteCommandPy("amfi developer-mode-status", udid).Contains("true");
            }
        }

        public bool RebootDevice(string udid, iOSExecutable executable = iOSExecutable.go)
        {
            if (executable.Equals(iOSExecutable.go))
            {
                var output = ExecuteCommand("reboot", udid);
                if (output.Contains("\"msg\":\"ok\""))
                {
                    return true;
                }
                return false;
            }
            else
            {
                ExecuteCommandPy("diagnostics restart", udid);
                return true; //Need to verify and fix
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
                ExecuteCommandPy("developer core-device uninstall " + bundleId, udid);
            }
        }

        public void TakeScreenshot(string udid, string path)
        {
            if (MainScreen.udidProxyPort.ContainsKey(udid))
            {
                int port = MainScreen.udidProxyPort[udid];
                string sessionId = iOSMethods.GetInstance().IsWDARunning(port);
                if (!sessionId.Equals("nosession"))
                {
                    string url = "http://localhost:" + port;
                    iOSAPIMethods.TakeScreenshot(url, path.Replace("\"", ""));
                    return;
                }
                else
                {
                    TakeScreenshotUsingExecutable(udid, path);
                }
            }
            else
            {
                TakeScreenshotUsingExecutable(udid, path);
            }
        }

        public void TakeScreenshotUsingExecutable(string udid, string path)
        {
            if (isGo)
            {
                iOSAsyncMethods.GetInstance().CreateTunnelGo();
                ExecuteCommand("screenshot --output=" + path, udid);
            }
            else
            {
                ExecuteCommandPy("developer dvt screenshot " + path, udid);
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
                List<string> deviceList;
                bool flag = false;
                string[] profileFolders = Directory.GetDirectories(ProfilesFilePath);
                foreach (string profileFolder in profileFolders)
                {
                    string[] provisioningFiles = Directory.EnumerateFiles(profileFolder, "*.mobileprovision").ToArray();
                    if (provisioningFiles.Length > 0)
                    {
                        foreach (string provisioningFile in provisioningFiles)
                        {
                            string directoryPath = Path.GetDirectoryName(provisioningFile);
                            string expiryDateFromPem = ImportProfile.GetExpirationDateFromPemFile(directoryPath + "\\certificate.pem");
                            int expirationDaysFromPem = ImportProfile.ExpirationDays(expiryDateFromPem);

                            var provisionDetails = ImportProfile.GetDetailsFromProvisionFile(provisioningFile);
                            deviceList = (List<string>)provisionDetails["DevicesList"];
                            int expirationDaysFromProvision = ImportProfile.ExpirationDays(provisionDetails["ExpirationDate"].ToString());
                            if (deviceList.Contains(udid) && expirationDaysFromProvision > 0 && expirationDaysFromPem > 0)
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


        public static string signIPAStackTrace = "";
        public string SignIPA(string udid, string IPAFilePath, CommonProgress commonProgress = null, string message = null)
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
                return SignIPA(certificatPath, IPAFilePath, signedIPAFilePath, commonProgress, message);
            }
            else
            {
                return "ProfileNotAvailable";
            }
        }


        public string SignIPA(string profilePath, string IPAFilePath, string outputPath, string udid, CommonProgress commonProgress = null, string message = null)
        {
            if (udid != "")
            {
                bool isProfileAvailable = isProfileHasUDID(profilePath, udid);
                if (isProfileAvailable)
                {
                    return SignIPA(profilePath, IPAFilePath, outputPath, commonProgress, message);
                }
                else
                {
                    return "ProfileNotAvailable";
                }
            }
            else
            {
                return SignIPA(profilePath, IPAFilePath, outputPath, commonProgress, message);
            }
        }

        private string SignIPA(string profilePath, string IPAFilePath, string outputPath, CommonProgress commonProgress = null, string message = null)
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

            process.OutputDataReceived += (sender, e) => Process_OutputDataReceived(sender, e, commonProgress, message);
            process.ErrorDataReceived += (sender, e) => Process_ErrorDataReceived(sender, e, commonProgress, message);

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

            if (signIPAStackTrace == "Archive Failed")
            {
                return "Sign_IPA_Failed";
            }
            else if (signIPAStackTrace == "Archive OK")
            {
                return outputPath;
            }
            else
            {
                return "Sign_IPA_Failed";
            }
        }

        private static void Process_OutputDataReceived(object sender, DataReceivedEventArgs e, CommonProgress commonProgress = null, string message = null)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                if (e.Data.Contains("Archive Failed"))
                {
                    signIPAStackTrace = "Sign IPA Archive Failed";
                }
                if (e.Data.Contains("Archive OK"))
                {
                    signIPAStackTrace = "Archive OK";
                }
                int percent = 0;
                if (e.Data.Contains("Unzip OK!"))
                {
                    percent = 40;
                }
                else if (e.Data.Contains("Signed OK!"))
                {
                    percent = 70;
                }
                else if (e.Data.Contains("Archive OK!"))
                {
                    percent = 100;
                }
                if (percent != 0)
                {
                    if (commonProgress != null)
                    {
                        commonProgress.UpdateStepLabel("Sign App", message, percent);
                    }
                }
            }
        }

        private static void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e, CommonProgress commonProgress = null, string message = null)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                if (e.Data.Contains("Archive Failed"))
                {
                    signIPAStackTrace = "Sign IPA Archive Failed";
                }
                if (e.Data.Contains("Archive OK"))
                {
                    signIPAStackTrace = "Archive OK";
                }
                int percent = 0;
                if (e.Data.Contains("Unzip OK!"))
                {
                    percent = 40;
                }
                else if (e.Data.Contains("Signed OK!"))
                {
                    percent = 70;
                }
                else if (e.Data.Contains("Archive OK!"))
                {
                    percent = 100;
                }
                if (percent != 0)
                {
                    commonProgress.UpdateStepLabel("Sign App", message, percent);
                }
            }
        }

        public string RunWebDriverAgentQuick(string udid)
        {
            if (isGo)
            {
                //iOSProcess.StartInfo.Arguments = "launch com.facebook.WebDriverAgentRunner.xctrunner" + " --udid=" + udid;
                //iOSProcess.Start();
                //return "";
                iOSProcess.StartInfo.Arguments = "launch com.facebook.WebDriverAgentRunner.xctrunner" + " --udid=" + udid;
                iOSProcess.StartInfo.RedirectStandardOutput = true;
                iOSProcess.StartInfo.RedirectStandardError = true;
                iOSProcess.StartInfo.UseShellExecute = false;
                iOSProcess.StartInfo.CreateNoWindow = true;

                iOSProcess.Start();

                // Read the standard output and standard error
                string output = iOSProcess.StandardOutput.ReadToEnd();
                string error = iOSProcess.StandardError.ReadToEnd();

                iOSProcess.WaitForExit();

                // Combine the output and error messages
                return output + error;
            }
            else
            {
                return ExecuteCommandPy("developer dvt launch com.facebook.WebDriverAgentRunner.xctrunner", udid, false);
            }
        }

        public string IsWDARunning(int port)
        {
            try
            {
                var options = new RestClientOptions("http://localhost:" + port)
                {
                    Timeout = TimeSpan.FromSeconds(5)
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
                output = ExecuteCommandPy("developer core-device list-processes");
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
                return ExecuteCommandPy("mounter auto-mount", udid, false);
            }
        }

        public bool isImageMounted(string udid)
        {
            var output = ExecuteCommand("image list", udid);
            if (output.Contains("\"msg\":\"none\""))
            {
                return false;
            }
            return true;
        }

        public Dictionary<string, string> GetIPAInformation(string ipaFilePath)
        {
            Dictionary<string, string> output = new Dictionary<string, string>();
            string tempFolder = Path.GetTempPath();
            tempFolder = Path.Combine(tempFolder, "Appium_Wizard");
            Directory.CreateDirectory(tempFolder);
            var plistFilePath = Common.ExtractInfoPlistFromIPA(ipaFilePath, tempFolder);
            if (!string.IsNullOrEmpty(plistFilePath))
            {
                string xmlString = ExecutePlistUtil(plistFilePath);
                output = Common.GetValueFromXml(xmlString);
            }
            return output;
        }

        public string GetWDAIPAVersion()
        {
            string infoPlistPathFromRoot = @"Payload/WebDriverAgentRunner-Runner.app/Info.plist"; // Version is always 1.0 in this plistfile - But while downloading WDA updating the correct version in iPA.
            string infoPlistPathFromXCTest = @"Payload/WebDriverAgentRunner-Runner.app/PlugIns/WebDriverAgentRunner.xctest/Frameworks/WebDriverAgentLib.framework/Info.plist"; // Correct version given in this file.

            string rootInfoVersion = "1.0";
            rootInfoVersion = GetWDAIPAVersion(infoPlistPathFromRoot);
            if (rootInfoVersion.Equals("1.0"))
            {
                string xcTestInfoVersion = GetWDAIPAVersion(infoPlistPathFromXCTest);
                return xcTestInfoVersion;
            }
            return rootInfoVersion; 
        }

        private string GetWDAIPAVersion(string infoPlistPath)
        {
            Dictionary<string, string> output = new Dictionary<string, string>();
            string tempFolder = Path.GetTempPath();
            tempFolder = Path.Combine(tempFolder, "Appium_Wizard");
            Directory.CreateDirectory(tempFolder);
            
            var plistFilePath = Common.ExtractInfoPlistFromWDAIPA(infoPlistPath, tempFolder);
            if (!string.IsNullOrEmpty(plistFilePath))
            {
                string xmlString = ExecutePlistUtil(plistFilePath);
                output = Common.GetValueFromXml(xmlString);
            }
            if (output.ContainsKey("CFBundleShortVersionString"))
            {
                return output["CFBundleShortVersionString"];
            }
            return "1.0"; // Return Default version if no key found.
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
            if (MainScreen.udidProxyPort.ContainsKey(udid))
            {
                int port = MainScreen.udidProxyPort[udid];
                string sessionId = iOSMethods.GetInstance().IsWDARunning(port);
                if (!sessionId.Equals("nosession"))
                {
                    string url = "http://localhost:" + port;
                    return iOSAPIMethods.LaunchApp(url, sessionId, bundleId);
                }
                else
                {
                    return LaunchAppUsingExecutable(bundleId, udid);
                }
            }
            else
            {
                return LaunchAppUsingExecutable(bundleId, udid);
            }
        }

        public string LaunchAppUsingExecutable(string bundleId, string udid)
        {
            if (isGo)
            {
                iOSAsyncMethods.GetInstance().CreateTunnelGo();
                return ExecuteCommand("launch " + bundleId, udid);
            }
            else
            {

                return ExecuteCommandPy("developer dvt launch " + bundleId, udid);
            }
        }

        public void KillApp(string udid, string bundleId)
        {
            if (MainScreen.udidProxyPort.ContainsKey(udid))
            {
                int port = MainScreen.udidProxyPort[udid];
                string sessionId = iOSMethods.GetInstance().IsWDARunning(port);
                if (!sessionId.Equals("nosession"))
                {
                    string url = "http://localhost:" + port;
                    iOSAPIMethods.KillApp(url, sessionId, bundleId);
                }
                else
                {
                    KillAppUsingExecutable(bundleId, udid);
                }
            }
            else
            {
                KillAppUsingExecutable(bundleId, udid);
            }
        }

        public void UnlockScreen(string udid, string password)
        {
            if (MainScreen.udidProxyPort.ContainsKey(udid))
            {

                int port = MainScreen.udidProxyPort[udid];
                iOSAPIMethods.GoToHome(port);
                string sessionId = iOSMethods.GetInstance().IsWDARunning(port);
                if (!sessionId.Equals("nosession"))
                {
                    string url = "http://localhost:" + port;
                    var screenSize = iOSAPIMethods.GetScreenSize(port);
                    int width = screenSize.Item1;
                    int height = screenSize.Item2;
                    int pressX = width / 2;
                    int pressY = height - 1;
                    int moveToX = width / 2;
                    int moveToY = 0;
                    iOSAPIMethods.Swipe(url, sessionId, pressX, pressY, moveToX, moveToY, 500);
                    Thread.Sleep(3000);
                    iOSAPIMethods.SendText(url, sessionId, password);
                }
                else
                {
                    MessageBox.Show("WebDriverAgentRunner is not running iPhone. So, screen unlock failed.");
                }
            }
        }

        public void KillAppUsingExecutable(string bundleId, string udid)
        {
            if (isGo)
            {
                iOSAsyncMethods.GetInstance().CreateTunnelGo();
                ExecuteCommand("kill " + bundleId, udid);
            }
            else
            {
                ExecuteCommandPy("developer dvt pkill --bundle " + bundleId, udid);
            }
        }

        public void GoToHomeScreen(string udid)
        {
            LaunchApp(udid, "com.apple.springboard");
        }


        public string ExecuteCommand(string arguments, string udid = "any", bool waitForExit = true, int timeout = 0)
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
                bool processExited = false;
                if (waitForExit)
                {
                    if (timeout == 0)
                    {
                        iOSProcess.WaitForExit();
                    }
                    else
                    {
                        processExited = iOSProcess.WaitForExit(timeout);
                    }
                }
                if (timeout != 0 && !processExited)
                {
                    iOSProcess.Kill(); // Kill the process if it did not exit within the timeout
                    return "Process did not complete within the allotted time.";
                }
                string output = iOSProcess.StandardOutput.ReadToEnd();
                string error = iOSProcess.StandardError.ReadToEnd();
                if (!string.IsNullOrEmpty(output))
                {
                    return output;
                }
                else
                {
                    if (error.Contains("PasswordProtected") | error.Contains("runtime error: invalid memory address or nil pointer dereference"))
                    {
                        MessageBox.Show("Failed to execute the operation. Please Unlock the device/Trust the computer and execute again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return "Error:" + error;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while executing argument " + arguments + ": " + ex.Message);
                return "Exception";
            }
        }

        public string ExecuteCommandPy(string command, string udid = "", bool closeTunnel = false, int timeout = 30000)
        {
            bool isTunnelRunning = false;
            if (command.Contains("developer"))
            {
                isTunnelRunning = iOSAsyncMethods.GetInstance().CreateTunnel();
                if (!isTunnelRunning)
                {
                    return "tunnel not created";
                }
            }
            Process process = new Process();
            process.StartInfo.FileName = FilesPath.pymd3FilePath;
            if (udid == "")
            {
                process.StartInfo.Arguments = command;
            }
            else
            {
                process.StartInfo.Arguments = command + " --udid " + udid;
            }
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            bool processExited = false;
            string error = process.StandardError.ReadToEnd();
            string output = process.StandardOutput.ReadToEnd();
            string result = output + error;
            if (timeout == 0)
            {
                process.WaitForExit();
            }
            else
            {
                processExited = iOSProcess.WaitForExit(timeout);
            }
            if (timeout != 0 && !processExited)
            {
                process.Kill(); // Kill the process if it did not exit within the timeout
                MessageBox.Show("Failed to perform action within the given time. Please try again after opening the device.\n\nIf the issue persists, try restarting Appium Wizard/System.", "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "Process did not complete within the allotted time.";
            }
            result = Regex.Replace(result, @"\x1B\[[0-9;]*[mK]", string.Empty);
            process.Close();
            if (closeTunnel)
            {
                try
                {
                    iOSAsyncMethods.GetInstance().CloseTunnel();
                }
                catch (Exception)
                {
                }
            }
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
        public static bool is17Plus;

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

        public void StartiOSProxyServer(string udid, int localPort, int iOSPort, iOSExecutable executable = iOSExecutable.go)
        {
            if (executable.Equals(iOSExecutable.go))
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
                    MainScreen.runningProcesses.Add(processId);
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
                    pyAsyncProcess.StartInfo.FileName = FilesPath.pymd3FilePath;
                    pyAsyncProcess.StartInfo.Arguments = $"usbmux forward " + localPort + " " + iOSPort + " --serial " + udid;
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
                    MainScreen.runningProcesses.Add(processId);
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
                //Arguments = localPort + " " + iOSPort,
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
            MainScreen.runningProcesses.Add(processId);
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
            MainScreen.runningProcesses.Add(processId);
            MainScreen.runningProcessesPortNumbers.Add(localPort1);
            MainScreen.runningProcessesPortNumbers.Add(localPort2);
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
            MainScreen.runningProcesses.Add(processId);
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
                    pyAsyncProcess.StartInfo.FileName = FilesPath.pymd3FilePath;
                    pyAsyncProcess.StartInfo.Arguments = $"apps install \"{path}\" --udid " + udid;
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
        public async Task<string> RunWebDriverAgent(CommonProgress commonProgress, string udid, int port)
        {
            try
            {
                if (MainScreen.udidProxyPort.ContainsKey(udid))
                {
                    int port1 = MainScreen.udidProxyPort[udid];
                    string sessionId = iOSMethods.GetInstance().IsWDARunning(port);
                    if (!sessionId.Equals("nosession"))
                    {
                        return sessionId;
                    }
                }

                if (isGo && !is17Plus)
                {
                    // Create a new process
                    Process process = new Process();

                    // Configure the process
                    process.StartInfo.FileName = iOSServerFilePath;
                    process.StartInfo.Arguments = "runwda --udid=" + udid;
                    process.StartInfo.UseShellExecute = false;
                    //process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.EnableRaisingEvents = true;

                    // Register event handlers for error and output data received
                    //process.ErrorDataReceived += Process_ErrorDataReceived;
                    //process.OutputDataReceived += Process_OutputDataReceived;
                    process.ErrorDataReceived += (sender, e) => Process_ErrorDataReceived(sender, e, port);
                    process.OutputDataReceived += (sender, e) => Process_OutputDataReceived(sender, e, port);

                    // Start the process
                    process.Start();
                    var processId = process.Id;
                    MainScreen.runningProcesses.Add(processId);
                    //Thread.Sleep(2000);
                    await Task.Delay(2000);
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
                    bool isWDARanAtleaseOnce = false;
                    while (!process.HasExited)
                    {
                        if (!string.IsNullOrEmpty(runwdaError))
                        {
                            if (runwdaError.Contains("Process started successfully"))
                            {
                                sessionId = iOSMethods.GetInstance().IsWDARunning(port);
                                while (sessionId.Equals("nosession") && count <= 6)
                                {
                                    await Task.Run(async () =>
                                    {
                                        //Thread.Sleep(5000);
                                        await Task.Delay(5000);
                                        sessionId = iOSAPIMethods.CreateWDASession(port);
                                        bool IsWDARunningInAppsList = iOSMethods.GetInstance().IsWDARunningInAppsList(udid);
                                        if (sessionId.Equals("nosession") & IsWDARunningInAppsList)
                                        {
                                            commonProgress.UpdateStepLabel("Please enter Passcode on your iPhone to continue...Retrying in 5 seconds...\nRetry " + count + "/6.");
                                            isPasscodeRequired = true;
                                            isWDARanAtleaseOnce = true;
                                        }
                                        else if (!IsWDARunningInAppsList)
                                        {
                                            iOSMethods.GetInstance().RunWebDriverAgentQuick(udid);
                                            if (isWDARanAtleaseOnce)
                                            {
                                                commonProgress.UpdateStepLabel("Please DON'T CANCEL the XCTest passcode request. Enter passcode on your iPhone to continue...Retrying in 5 seconds...\nRetry " + count + "/6.");
                                            }
                                            isPasscodeRequired = true;
                                        }
                                        count++;
                                    });
                                }
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
                    //if (runwdaError.Contains("WDA process ended unexpectedly"))
                    //{
                    //    return RunWebDriverAgentQuick(commonProgress,udid,port);
                    //}
                    return "unhandled";
                }
                else
                {
                    return RunWebDriverAgentQuick(commonProgress, udid, port);
                }

            }
            catch (Exception)
            {
                return RunWebDriverAgentQuick(commonProgress, udid, port);
            }
        }
        public void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e, int port)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Console.WriteLine("Error: " + e.Data);
                runwdaError += e.Data;
                if (e.Data.Contains("\"authorized\":true,\"level\":\"info\",\"msg\":\"authorized\""))
                {
                    iOSAPIMethods.GoToHome(port);
                }
            }
        }

        // Event handler for output data received
        public void Process_OutputDataReceived(object sender, DataReceivedEventArgs e, int port)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Console.WriteLine("Output: " + e.Data);
                runwdaOutput += e.Data;
                if (e.Data.Contains("\"authorized\":true,\"level\":\"info\",\"msg\":\"authorized\""))
                {
                    iOSAPIMethods.GoToHome(port);
                }
            }
        }


        public string RunWebDriverAgentQuick(CommonProgress commonProgress, string udid, int port)
        {
            int count = 1;
            string sessionId = string.Empty;
            string output = iOSMethods.GetInstance().RunWebDriverAgentQuick(udid);
            if (output.Contains("Process launched"))
            {
                Thread.Sleep(2000);
                sessionId = iOSAPIMethods.CreateWDASession(port);
                while (sessionId.Equals("nosession") && count <= 5)
                {
                    Thread.Sleep(7000);
                    iOSMethods.GetInstance().GoToHomeScreen(udid);
                    Thread.Sleep(2000);
                    sessionId = iOSAPIMethods.CreateWDASession(port);
                    if (!sessionId.Equals("nosession"))
                    {
                        break;
                    }
                    commonProgress.UpdateStepLabel("Restarting WebDriverAgentRunner...\nRetry " + count + "/5.");
                    iOSMethods.GetInstance().RunWebDriverAgentQuick(udid);
                    commonProgress.UpdateStepLabel("Please enter Passcode on your iPhone if it asks...\nOnce you see Automation Running, Go to home screen to reduce the retry.\nRetry " + count + "/5.");
                    count++;
                }
            }
            else if (output.Contains("'BSErrorCodeDescription': 'Locked'") | output.Contains("'PasswordProtected'"))
            {
                return "Password Protected";
            }
            //else if (!iOSMethods.GetInstance().isDeveloperModeDisabled(udid))
            //{
            //    return "Enable Developer Mode";
            //}
            iOSMethods.GetInstance().GoToHomeScreen(udid);
            return sessionId;

        }



        Process tunnelProcess;
        public bool CreateTunnel()
        {
            string errorMessage = "As admin permission has not been given, unable to continue with the request. Please try again by providing admin permission or try setting Method 1 in Tools->iOS Executor.";
            CommonProgress commonProgress = new CommonProgress();
            int counter = 1;
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Creating Tunnel", "Please wait while checking for tunnel running status, This may take few seconds...", 10);
            bool isTunnelRunning = iOSAPIMethods.isTunnelRunning();
            commonProgress.UpdateStepLabel("Creating Tunnel", "Please wait while checking for tunnel running status, This may take few seconds...", 30);
            if (isTunnelRunning)
            {
                commonProgress.Close();
                return true;
            }
            else
            {
                var result = MessageBox.Show("----->THIS WORKS ONLY WITH iOS VERSION >=17.4<-----\n\nStarting at iOS 17.0, Apple introduced a new CoreDevice framework to work with iOS devices.\n\nIn order to communicate with the developer services you'll be required to first create trusted tunnel using a command.\n\nThis command must be run with high privileges since it creates a new TUN/TAP device which is a high privilege operation.\n\nSo click OK to grant permission to create the tunnel as an admin[It may not prompt if you logged in as admin or it will ask admin credentials on clicking OK]\n\nNOTE : You might get Unknown publisher security warning from windows, Just Run.", "Admin Privilege Required", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                if (result == DialogResult.OK)
                {
                    commonProgress.UpdateStepLabel("Creating Tunnel", "Please wait while creating tunnel, This may take few seconds...\nNote : Tunnel will be created only once for a application lifecycle.\nIt won't ask permission again until you re-launch the Appium Wizard.");
                    try
                    {
                        tunnelProcess = new Process();
                        tunnelProcess.StartInfo.FileName = FilesPath.pymd3FilePath;
                        tunnelProcess.StartInfo.Arguments = "remote tunneld";
                        tunnelProcess.StartInfo.UseShellExecute = true;
                        tunnelProcess.StartInfo.CreateNoWindow = true;
                        tunnelProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        tunnelProcess.StartInfo.Verb = "runas";
                        tunnelProcess.Start();

                        try
                        {
                            var id = tunnelProcess.Id; // just to check if there's exception.
                        }
                        catch (InvalidOperationException)
                        {
                            commonProgress.Close();
                            MessageBox.Show(errorMessage, "Admin Permission denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return false;
                        }
                        Thread.Sleep(10000);
                        while (!iOSAPIMethods.isTunnelRunning() && counter <= 10)
                        {
                            if (tunnelProcess.HasExited)
                            {
                                commonProgress.Close();
                                MessageBox.Show(errorMessage, "Admin Permission denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return false;
                            }
                            Thread.Sleep(5000);
                            counter++;
                        }
                        var processId = tunnelProcess.Id;
                        MainScreen.runningProcesses.Add(processId);
                        commonProgress.Close();
                    }
                    catch (Exception)
                    {
                        commonProgress.Close();
                        MessageBox.Show(errorMessage, "Admin Permission denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                }
                else
                {
                    commonProgress.Close();
                    MessageBox.Show(errorMessage, "Admin Permission denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    GoogleAnalytics.SendEvent("iOS17_Admin_Cancel");
                    //try
                    //{
                    //    ProcessStartInfo psInfo = new ProcessStartInfo
                    //    {
                    //        FileName = "https://developer.apple.com/forums/thread/730947?answerId=756665022#756665022",
                    //        UseShellExecute = true
                    //    };
                    //    Process.Start(psInfo);
                    //    GoogleAnalytics.SendEvent("iOS17_Admin_Cancel");
                    //}
                    //catch (Exception exception)
                    //{
                    //    GoogleAnalytics.SendExceptionEvent("iOS17_Admin_Cancel", exception.Message);
                    //}
                }
            }
            commonProgress.Close();
            return iOSAPIMethods.isTunnelRunning();
        }

        public bool CreateTunnelGo(bool showProgress = true)
        {
            CommonProgress commonProgress = new CommonProgress();
            int count = 0;
            if (showProgress)
            {
                commonProgress.Show();
            }
            commonProgress.UpdateStepLabel("Creating Tunnel", "Please wait while checking for tunnel running status, This may take few seconds...", 10);
            bool isTunnelRunning = iOSAPIMethods.isTunnelRunningGo();
            commonProgress.UpdateStepLabel("Creating Tunnel", "Please wait while checking for tunnel running status, This may take few seconds...", 30);
            if (!isTunnelRunning)
            {
                commonProgress.UpdateStepLabel("Creating Tunnel", "Please wait while creating tunnel, This may take few seconds...\nNote : Tunnel will be created only once for a application lifecycle.");
                try
                {
                    tunnelProcess = new Process();
                    tunnelProcess.StartInfo.FileName = iOSServerFilePath;
                    tunnelProcess.StartInfo.Arguments = "tunnel start --userspace";
                    tunnelProcess.StartInfo.UseShellExecute = false;
                    tunnelProcess.StartInfo.CreateNoWindow = true;
                    tunnelProcess.Start();
                    do
                    {
                        Thread.Sleep(1000);
                        isTunnelRunning = iOSAPIMethods.isTunnelRunningGo();
                        count++;
                    }
                    while (count <= 5 && !isTunnelRunning);

                    if (isTunnelRunning)
                    {
                        var processId = tunnelProcess.Id;
                        MainScreen.runningProcesses.Add(processId);
                        commonProgress.Close();
                        return true;
                    }
                    commonProgress.Close();
                    return false;
                }
                catch (Exception)
                {
                    commonProgress.Close();
                    return false;
                }
            }
            commonProgress.Close();
            return isTunnelRunning;
        }

        public void CloseTunnel()
        {
            try
            {
                if (tunnelProcess != null)
                {
                    if (!tunnelProcess.HasExited)
                    {
                        tunnelProcess.Kill();
                    }
                    tunnelProcess.Close();
                }
            }
            catch (Exception)
            {
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

        public static (int, int) GetScreenSize(int proxyPort)
        {
            int width, height;
            var options = new RestClientOptions("http://localhost:" + proxyPort)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/window/size", Method.Get);
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
            //var request = new RestRequest("/session/" + sessionId + "/wda/touch/perform", Method.Post);
            var request = new RestRequest("/session/" + sessionId + "/wda/tap", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            //var body = $@"{{""actions"": [{{ ""action"": ""press"", ""options"": {{ ""x"": {pressX}, ""y"": {pressY} }} }},{{ ""action"": ""release"" }}]}}";
            var body = $@"{{ ""x"": {pressX}, ""y"": {pressY} }}";
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
            //var request = new RestRequest("/session/" + sessionId + "/actions", Method.Post);
            var request = new RestRequest("/session/" + sessionId + "/wda/keys", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            //var body = $@"{{""actions"": [{{""type"": ""key"",""id"": ""keyboard"",""actions"":[{{ ""type"": ""keyDown"", ""value"": ""{text}"" }},{{ ""type"": ""keyUp"", ""value"": ""{text}"" }}]}}]}}";
            var body = $@"{{""value"":[""{text}""]}}";
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
            //var request = new RestRequest("/session/" + sessionId + "/wda/touch/perform", Method.Post);
            var request = new RestRequest("/session/" + sessionId + "/actions", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            //var body = $@"{{""actions"":[{{""action"":""press"",""options"":{{""x"":{pressX},""y"":{pressY}}}}},{{""action"":""wait"",""options"":{{""ms"":{waitDuration}}}}},{{""action"":""moveTo"",""options"":{{""x"":{moveToX},""y"":{moveToY}}}}},{{""action"":""release"",""options"":{{}}}}]}}";
            var body = $@"{{""actions"":[{{""type"":""pointer"",""id"":""finger1"",""parameters"":{{""pointerType"":""touch""}},""actions"":[{{""type"":""pointerMove"",""duration"":0,""x"":{pressX},""y"":{pressY}}},{{""type"":""pointerMove"",""duration"":{waitDuration},""origin"":""viewport"",""x"":{moveToX},""y"":{moveToY}}}]}}]}}";
            request.AddStringBody(body, DataFormat.Json);
            RestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
        }

        public static void GoToHome(int proxyPort)
        {
            var options = new RestClientOptions("http://localhost:" + proxyPort)
            {
                Timeout = TimeSpan.FromSeconds(2)
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
            int waitDuration = 500;
            int fromX = screenWidth - 50;
            int fromY = 50;
            int endX = fromX;
            int endY = screenHeight;
            Swipe(URL, sessionId, fromX, fromY, endX, endY, waitDuration);
        }

        public static void CloseControlCenter(string URL, string sessionId, int screenWidth)
        {
            int x = screenWidth / 2;
            int y = 50;
            Tap(URL, sessionId, x, y);
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

        public static bool isTunnelRunning()
        {
            var options = new RestClientOptions("http://127.0.0.1:49151")
            {
                Timeout = TimeSpan.FromSeconds(5)
            };
            var client = new RestClient(options);
            var request = new RestRequest("/hello", Method.Get);
            RestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            if (response.Content != null)
            {
                return response.Content.Contains("Hello, I'm alive");
            }
            else { return false; }
        }

        public static bool isTunnelRunningGo()
        {
            var options = new RestClientOptions("http://127.0.0.1:60105")
            {
                Timeout = TimeSpan.FromSeconds(5)
            };
            var client = new RestClient(options);
            var request = new RestRequest("/", Method.Get);
            RestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound || response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            return false;
        }

        public static string LaunchApp(string URL, string sessionId, string bundleId)
        {
            var options = new RestClientOptions(URL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/session/" + sessionId + "/wda/apps/launch", Method.Post);
            var body = $@"{{""bundleId"": ""{bundleId}""}}";
            request.AddParameter("text/plain", body, ParameterType.RequestBody);
            RestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            return response.Content;
        }

        public static string KillApp(string URL, string sessionId, string bundleId)
        {
            var options = new RestClientOptions(URL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/session/" + sessionId + "/wda/apps/terminate", Method.Post);
            var body = $@"{{""bundleId"": ""{bundleId}""}}";
            request.AddParameter("text/plain", body, ParameterType.RequestBody);
            RestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            return response.Content;
        }

        public static void TakeScreenshot(string URL, string filePath)
        {
            var options = new RestClientOptions(URL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/screenshot", Method.Get);
            RestResponse response = client.Execute(request);
            string jsonString = response.Content;

            JsonDocument doc = JsonDocument.Parse(jsonString);
            string base64String = doc.RootElement.GetProperty("value").GetString();

            byte[] imageBytes = Convert.FromBase64String(base64String);
            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                Image image = Image.FromStream(ms);
                image.Save(filePath, ImageFormat.Png);
            }
        }

        public static bool IsWDARunning(int port)
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
            catch (Exception)
            {
                return false;
            }
        }

        public static string isDeviceLocked(int port)
        {
            try
            {
                var options = new RestClientOptions("http://localhost:" + port)
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest("/wda/locked", Method.Get);
                RestResponse response = client.Execute(request);
                JObject jsonObject = JObject.Parse(response.Content);
                bool isLocked = jsonObject["value"].Value<bool>();
                if (isLocked)
                {
                    return "Yes";
                }
                return "No";
            }
            catch (Exception)
            {
                return "Error";   
            }
        }
    }
}
