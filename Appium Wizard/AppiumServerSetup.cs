using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Appium_Wizard
{
    public class AppiumServerSetup
    {
        public string statusText = "";
        public bool serverStarted = false;
        public static string deviceList = "", deviceInfo = "", tempFolder = "", logFilePath = "";
        public static Dictionary<int, Tuple<Process, string>> listOfProcess = new Dictionary<int, Tuple<Process, string>>();
        //public static Dictionary<int,bool> appiumServerRunningList = new Dictionary<int,bool>();
        public static Dictionary<int, Tuple<int, string>> portServerNumberAndFilePath = new Dictionary<int, Tuple<int, string>>();
        public static bool UpdateStatusInScreenFlag = true;
        public void StartAppiumServer(int appiumPort, int serverNumber)
        {
            int webDriverAgentProxyPort = Common.GetFreePort();
            tempFolder = Path.GetTempPath();
            logFilePath = Path.Combine(tempFolder, "AppiumWizard_Log_" + appiumPort + "_" + DateTime.Now.ToString("d-MMM-yyyy h-mm-ss tt") + ".txt");
            File.WriteAllText(logFilePath, "\t\t\t\t------------------------------Starting Appium Server------------------------------\n\n");
            if (!Common.IsNodeInstalled())
            {
                File.WriteAllText(logFilePath, "\t\t----------------NodeJS not installed. Please install and restart Appium Wizard to start the appium server----------------\n\n");
            }
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    //FileName = AppiumServerFilePath,
                    //Arguments = @"--allow-cors --default-capabilities ""{\""appium:webDriverAgentUrl\"":\""http://localhost:7777\""}""",
                    FileName = "cmd.exe",
                    WorkingDirectory = FilesPath.serverInstalledPath,
                    //Arguments = @"/C appium --allow-cors --default-capabilities ""{\""appium:webDriverAgentUrl\"":\""http://localhost:7777\""}""",                    
                    //Arguments = $@"/C appium --port {appiumPort} --allow-cors --default-capabilities ""{{\""appium:webDriverAgentUrl\"":\""http://localhost:{webDriverAgentProxyPort}\"", \""appium:systemPort\"":{UiAutomatorPort}}}""",
                    //Arguments = $@"/C appium --port {appiumPort} --allow-cors --default-capabilities ""{{\""appium:webDriverAgentUrl\"":\""http://localhost:{webDriverAgentProxyPort}\"",\""appium:mjpegServerPort\"":\""{screenport}\""}}",
                    //Arguments = $@"/C appium --port {appiumPort} --allow-cors  --log-level info --default-capabilities ""{{\""appium:webDriverAgentUrl\"":\""http://localhost:{webDriverAgentProxyPort}\""}}",
                    //Arguments = $@"/C appium --port {appiumPort} --allow-cors --default-capabilities ""{{\""appium:webDriverAgentUrl\"":\""http://localhost:{webDriverAgentProxyPort}\""}}",
                    Arguments = $@"/C appium --port {appiumPort} --allow-cors --allow-insecure=adb_shell --default-capabilities ""{{\""appium:webDriverAgentUrl\"":\""http://localhost:{webDriverAgentProxyPort}\"",\""appium:newCommandTimeout\"":0}}""",
                    //Arguments = $@"/C appium --port {appiumPort} --allow-cors",

                    //working
                    //Arguments = $@"/C appium --port {appiumPort} --allow-cors --default-capabilities ""{{\""appium:webDriverAgentUrl\"":\""http://localhost:{webDriverAgentProxyPort}\"",\""appium:skipUnlock\"":\""true\"",\""appium:skipDeviceInitialization\"": \""true\"",\""appium:dontStopAppOnReset\"":\""true\""}}""",
                    //Arguments = $@"/C appium --port {appiumPort} --allow-cors --default-capabilities ""{{\""appium:webDriverAgentUrl\"":\""http://localhost:{webDriverAgentProxyPort}\"", \""appium:skipServerInstallation\"": \""true\"",\""appium:skipUnlock\"":\""true\"",\""appium:skipDeviceInitialization\"": \""true\"",\""appium:dontStopAppOnReset\"":\""true\""}}""",
                    //Arguments = $@"/C appium --port {appiumPort} --allow-cors --session-override --default-capabilities ""{{\""appium:webDriverAgentUrl\"":\""http://localhost:{webDriverAgentProxyPort}\"", \""appium:skipServerInstallation\"": \""true\"",\""appium:skipUnlock\"":\""true\"",\""appium:skipDeviceInitialization\"": \""true\"",\""appium:noReset\"":true,\""appium:dontStopAppOnReset\"":\""true\""}}""",
                    //Arguments = $@"/C appium --port {appiumPort} --allow-cors --session-override --default-capabilities ""{{\""appium:webDriverAgentUrl\"":\""http://localhost:{webDriverAgentProxyPort}\"", \""appium:skipServerInstallation\"": \""true\"",\""appium:skipUnlock\"":\""true\"",\""appium:skipDeviceInitialization\"": \""true\""}}""",
                    //Arguments = $@"/C appium --port {appiumPort} --allow-cors --default-capabilities ""{{\""appium:webDriverAgentUrl\"":\""http://localhost:{webDriverAgentProxyPort}\""}}",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                Process appiumServerProcess = new Process();
                appiumServerProcess.StartInfo = startInfo;
                //appiumServerProcess.OutputDataReceived += AppiumServer_OutputDataReceived;
                //appiumServerProcess.ErrorDataReceived += AppiumServer_OutputDataReceived;
                appiumServerProcess.OutputDataReceived += (sender, e) => AppiumServer_OutputDataReceived(sender, e, serverNumber, webDriverAgentProxyPort);
                appiumServerProcess.ErrorDataReceived += (sender, e) => AppiumServer_OutputDataReceived(sender, e, serverNumber, webDriverAgentProxyPort);

                appiumServerProcess.EnableRaisingEvents = true;

                appiumServerProcess.Start();
                appiumServerProcess.BeginOutputReadLine();

                if (listOfProcess.ContainsKey(appiumPort))
                {
                    listOfProcess[appiumPort] = Tuple.Create(appiumServerProcess, logFilePath);
                }
                else
                {
                    listOfProcess.Add(appiumPort, Tuple.Create(appiumServerProcess, logFilePath));
                }

                if (portServerNumberAndFilePath.ContainsKey(serverNumber))
                {
                    portServerNumberAndFilePath[serverNumber] = Tuple.Create(appiumPort, logFilePath);
                }
                else
                {
                    portServerNumberAndFilePath.Add(serverNumber, Tuple.Create(appiumPort, logFilePath));
                }
                int processId = appiumServerProcess.Id;
                MainScreen.runningProcesses.Add(processId);
            }
            catch (Exception ex)
            {
                string error = "An error occurred while starting Appium Server: " + ex.Message;
                MessageBox.Show(error, "Appium Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine(error);
            }
        }

        string deviceUDID = "none"; string currentSessionId = "none"; string currentUDID = "none";
        string currentPlatformName = "none";
        //int proxyPort = 0;
        int screenDensity = 0;
        Dictionary<string, string> sessionIdUDID = new Dictionary<string, string>();
        ExecutionStatus executionStatus = new ExecutionStatus();
        string proxiedUDID = "";
        private void AppiumServer_OutputDataReceived(object sender, DataReceivedEventArgs e, int serverNumber, int webDriverAgentProxyPort)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                string data = Regex.Replace(e.Data, @"\x1b\[[0-9;]*[mGKH]", "");
                using (var fileStream = new FileStream(portServerNumberAndFilePath[serverNumber].Item2, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.WriteLine(data);
                    if (data.Contains("No plugins have been installed."))
                    {
                        streamWriter.WriteLine("\n\n\t\t\t\t------------------------------Appium Server Ready to Use------------------------------\n\n");
                    }
                }
                if (data.Contains("Appium REST http interface listener started"))
                {
                    statusText = "Appium Server Started";
                    serverStarted = true;
                }
                else if (data.Contains("address already in use"))
                {
                    statusText = "address already in use";
                    serverStarted = false;
                }

                try
                {
                    //if (currentPlatformName.ToLower().Contains("ios") && !currentUDID.Equals("none") && !isPortReachable(webDriverAgentProxyPort))
                    //{
                    //    iOSAsyncMethods.GetInstance().StartiProxyServer(currentUDID, webDriverAgentProxyPort, 8100);
                    //}
                    if (data.Contains("POST /session {\"desiredCapabilities\":") | data.Contains("POST /session {\"capabilities\""))
                    {
                        string platformName = "";
                        string platformPattern = @"""platformName""\s*:\s*""([^""]+)""";
                        Match platformMatch = Regex.Match(data, platformPattern);
                        if (platformMatch.Success)
                        {
                            platformName = platformMatch.Groups[1].Value;
                            currentPlatformName = platformName;
                        }
                        //------------------------
                        string udidPattern = "\"appium:udid\":\"([^\"]+)\"";
                        Match udidMatch = Regex.Match(data, udidPattern);
                        if (udidMatch.Success)
                        {
                            currentUDID = udidMatch.Groups[1].Value;
                        }
                        //------------------------
                        if (platformName.ToLower().Contains("ios"))
                        {
                            if (iOSAsyncMethods.PortProcessId != null && iOSAsyncMethods.PortProcessId.ContainsKey(webDriverAgentProxyPort))
                            {
                                Common.KillProcessById(iOSAsyncMethods.PortProcessId[webDriverAgentProxyPort]);
                            }
                            iOSAsyncMethods.GetInstance().StartiProxyServer(currentUDID, webDriverAgentProxyPort, 8100);
                            if (MainScreen.DeviceInfo.ContainsKey(currentUDID))
                            {
                                string name = MainScreen.DeviceInfo[currentUDID].Item1;
                                UpdateScreenControl(currentUDID, "Set Device - " + name);
                            }
                            proxiedUDID = currentUDID;
                        }
                    }
                    //------------------------
                    if (data.Contains("added to master session list"))
                    {
                        string pattern = @"session (\w+-\w+-\w+-\w+-\w+)";
                        Regex regex = new Regex(pattern);
                        Match match1 = regex.Match(data);

                        if (match1.Success)
                        {
                            currentSessionId = match1.Groups[1].Value;
                            sessionIdUDID.Add(currentSessionId, currentUDID);
                            if (MainScreen.udidProxyPort.ContainsKey(deviceUDID))
                            {
                                MainScreen.udidProxyPort[currentUDID] = webDriverAgentProxyPort;
                            }
                            else
                            {
                                MainScreen.udidProxyPort.Add(deviceUDID, webDriverAgentProxyPort);
                            }                            
                        }
                    }
                    //------------------------

                    if (data.Contains("POST /session/"))
                    {
                        string pattern = @"/session/([a-f0-9\-]+)/(?:execute|appium|element|actions|timeouts|url|window|context|log|network)";
                        Match match = Regex.Match(data, pattern);

                        if (match.Success)
                        {
                            currentSessionId = match.Groups[1].Value;
                        }
                        if (currentSessionId != "none" && sessionIdUDID.ContainsKey(currentSessionId))
                        {
                            currentUDID = sessionIdUDID[currentSessionId];
                        }
                        if (!proxiedUDID.Equals(currentUDID) && !currentUDID.Equals("none"))
                        {
                            if (iOSAsyncMethods.PortProcessId != null && iOSAsyncMethods.PortProcessId.ContainsKey(webDriverAgentProxyPort))
                            {
                                Common.KillProcessById(iOSAsyncMethods.PortProcessId[webDriverAgentProxyPort]);
                            }
                            iOSAsyncMethods.GetInstance().StartiProxyServer(currentUDID, webDriverAgentProxyPort, 8100);
                            if (MainScreen.DeviceInfo.ContainsKey(currentUDID))
                            {
                                string name = MainScreen.DeviceInfo[currentUDID].Item1;
                                UpdateScreenControl(currentUDID, "Set Device - " + name);
                            }                           
                            proxiedUDID = currentUDID;
                        }
                    }
                    //------------------------

                    if (data.Contains("Calling AppiumDriver"))
                    {
                        string pattern = @"args: \[\""(?<sessionId>[a-f0-9\-]+)\""\]";
                        Match match = Regex.Match(data, pattern);
                        if (match.Success)
                        {
                            currentSessionId = match.Groups[1].Value.Trim('"');
                            if (currentSessionId != "none" && sessionIdUDID.ContainsKey(currentSessionId))
                            {
                                currentUDID = sessionIdUDID[currentSessionId];
                                if (!proxiedUDID.Equals(currentUDID) && !currentUDID.Equals("none"))
                                {
                                    if (MainScreen.DeviceInfo.ContainsKey(currentUDID))
                                    {
                                        string name = MainScreen.DeviceInfo[currentUDID].Item1;
                                        UpdateScreenControl(currentUDID, "Set Device - " + name);
                                    }
                                    if (iOSAsyncMethods.PortProcessId != null && iOSAsyncMethods.PortProcessId.ContainsKey(webDriverAgentProxyPort))
                                    {
                                        Common.KillProcessById(iOSAsyncMethods.PortProcessId[webDriverAgentProxyPort]);
                                    }
                                    iOSAsyncMethods.GetInstance().StartiProxyServer(currentUDID, webDriverAgentProxyPort, 8100);
                                    proxiedUDID = currentUDID;
                                }
                            }
                           
                        }                        
                    }

                    //------------------------
                    if (data.Contains("[HTTP] --> DELETE /session/"))
                    {
                        string pattern = @"/session/(\w+-\w+-\w+-\w+-\w+)";
                        Regex regex = new Regex(pattern);
                        Match match = regex.Match(data);
                        if (match.Success)
                        {
                            string udid = "";
                            string sessionId = match.Groups[1].Value;
                            if (sessionIdUDID.ContainsKey(sessionId))
                            {
                                udid = sessionIdUDID[sessionId];
                                sessionIdUDID.Remove(sessionId);
                                if (MainScreen.DeviceInfo.ContainsKey(udid))
                                {
                                    string name = MainScreen.DeviceInfo[udid].Item1;
                                    string text = "Session Deleted for " + name;
                                    UpdateScreenControl(udid, text);
                                }
                                else
                                {
                                    string text = "Session Deleted for " + udid;
                                    UpdateScreenControl(udid, text);
                                }
                                if (MainScreen.DeviceInfo[udid].Item2.Equals("Android")) // if android
                                {
                                    try
                                    {
                                        int androidProxyPort = (int)OpenDevice.deviceDetails[deviceUDID]["proxyPort"];
                                        int screenServerPort = (int)OpenDevice.deviceDetails[deviceUDID]["screenPort"];
                                        AndroidAsyncMethods.GetInstance().StartUIAutomatorServer(deviceUDID);
                                        AndroidAPIMethods.CreateSession(androidProxyPort, screenServerPort);
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                            }
                        }

                    }
                    //------------------------
                    if (data.Contains("POST /session/") && (data.Contains("/element 200") | data.Contains("/elements 200") | data.Contains("/click 200") | data.Contains("/value 200")))
                    {
                        UpdateScreenControl(currentUDID, "");
                    }
                    //------------------------
                    if (UpdateStatusInScreenFlag)
                    {
                        if (ScreenControl.udidScreenControl.ContainsKey(currentUDID))
                        {
                            executionStatus.UpdateScreenControl(ScreenControl.udidScreenControl[currentUDID], data, screenDensity);
                        }
                    }
                    //------------------------
                    if (data.Contains("Using device:"))
                    {
                        string input = data;
                        int startIndex = input.IndexOf(":") + 2;
                        deviceUDID = input.Substring(startIndex);
                        screenDensity = (int)AndroidMethods.GetInstance().GetScreenDensity(deviceUDID);
                        if (MainScreen.DeviceInfo.ContainsKey(deviceUDID))
                        {
                            string name = MainScreen.DeviceInfo[deviceUDID].Item1;
                            string text = "Set Device - " + name;
                            UpdateScreenControl(deviceUDID, text);
                        }
                        else
                        {
                            string text = "Set Device - " + deviceUDID;
                            UpdateScreenControl(deviceUDID, text);
                        }                                          
                    }
                    //------------------------
                }
                catch (Exception)
                {

                }

            }
        }

        private void UpdateScreenControl(string udid, string text)
        {
            if (UpdateStatusInScreenFlag && ScreenControl.udidScreenControl.ContainsKey(udid))
            {
                var control = ScreenControl.udidScreenControl[udid];
                control.UpdateStatusLabel(control, text);
            }
        }

        public void StopAppiumServer(int port)
        {
            if (listOfProcess.ContainsKey(port))
            {
                try
                {
                    Process process = listOfProcess[port].Item1;
                    Common.KillProcessByPortNumber(port);
                    process.CloseMainWindow();
                    process.Close();
                    process.Dispose();
                    string logfilePath = listOfProcess[port].Item2;
                    using (StreamWriter writer = File.AppendText(logfilePath))
                    {
                        writer.WriteLine("\t\t\t\t------------------------------Appium Server Stopped------------------------------\n\n");
                    }
                    listOfProcess.Remove(port);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }


        public bool IsAppiumServerRunning(int port)
        {
            var options = new RestClientOptions("http://localhost:" + port)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/status", Method.Get);
            RestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.Content.Contains("server is ready to accept");
            }
            else
            {
                return false;
            }
        }

        public static bool isExpectedDataAvailableInSessionDetails(string data)
        {
            Dictionary<string, string> readPortData = Database.QueryDataFromPortNumberTable();
            int port = 0;
            for (int i = 1; i <= 5; i++)
            {
                var PortNumber = readPortData["PortNumber" + i];
                if (PortNumber != "")
                {
                    port = int.Parse(PortNumber);
                }
                else
                {
                    continue;
                }

                var options = new RestClientOptions("http://localhost:" + port)
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest("/sessions", Method.Get);
                RestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (response.Content.Equals("{\"value\":[]}"))
                    {
                        continue;
                    }
                    else
                    {
                        JObject responseObj = JObject.Parse(response.Content);
                        string sessionId = responseObj["value"][0]["id"].ToString();
                        string androidId = GetAndroidIdFromAppiumServer(4723, sessionId);
                        if (androidId.Equals(data))
                        {
                            return true;
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                else
                {
                    return false;
                }

            }

            return false;
        }
        public static string GetAndroidId(int port, string sessionId)
        {
            var options = new RestClientOptions("http://127.0.0.1:"+port)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/session/" + sessionId + "/appium/device/info", Method.Get);
            RestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            JObject responseObj = JObject.Parse(response.Content);
            string androidId = responseObj["value"]["androidId"].ToString();
            return androidId;
        }

        public static string GetAndroidIdFromAppiumServer(int appiumPort, string appiumSessionId)
        {
            try
            {
                var options = new RestClientOptions("http://localhost:" + appiumPort)
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest("/session/" + appiumSessionId + "/execute", Method.Post);
                request.AddHeader("Content-Type", "application/json");
                var body = @"{""script"":""mobile:deviceInfo"",""args"":[]}";
                request.AddStringBody(body, DataFormat.Json);
                RestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);
                JObject responseObj = JObject.Parse(response.Content);
                string androidId = "noId";
                if (responseObj.ContainsKey("androidId"))
                {
                    androidId = responseObj["value"]["androidId"].ToString();
                }                
                return androidId;

            }
            catch (Exception)
            {
                return "error";
            }
        }

        public static Dictionary<string,int> ElementInfo(string url, string elementId)
        {
            try
            {
                var options = new RestClientOptions(url)
                {
                    MaxTimeout = 3000,
                };
                var client = new RestClient(options);
                var request = new RestRequest(elementId + "/rect", Method.Get);
                request.AddHeader("Content-Type", "application/json");
                RestResponse response = client.Execute(request);
                var jsonObject = JsonConvert.DeserializeObject<dynamic>(response.Content);
                int x = jsonObject.value.x;
                int y = jsonObject.value.y;
                int width = jsonObject.value.width;
                int height = jsonObject.value.height;

                Dictionary<string, int> rectangleDict = new Dictionary<string, int>
                    {
                        { "x", x },
                        { "y", y },
                        { "width", width },
                        { "height", height }
                    };

                return rectangleDict;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool isPortReachable(int port)
        {
            var options = new RestClientOptions("http://localhost:"+port)
            {
                Timeout = TimeSpan.FromSeconds(3),
            };
            var client = new RestClient(options);
            var request = new RestRequest("/", Method.Get);
            RestResponse response = client.Execute(request);
            if (response.StatusCode == 0)
            {
                return false;
            }
            return true;
        }
    }
}
