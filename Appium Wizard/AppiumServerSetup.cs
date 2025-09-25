using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using RestSharp;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
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
        public static Dictionary<int, int> serverNumberWDAPortNumber = new Dictionary<int, int>();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        string appiumLogLevel, updatedCommand;
        public void StartAppiumServer(int appiumPort, int serverNumber, string command = "appium --allow-cors --allow-insecure=adb_shell")
        {
            Logger.Info("Appium server initial command : " + command);
            string versionString = Common.InstalledAppiumServerVersionFromPackageJson();
            if (Version.TryParse(versionString, out Version installedVersion))
            {
                Version minimumVersion = new Version(2, 12, 3);
                if (installedVersion > minimumVersion)
                {
                    // installedVersion is greater than 3.0.0
                    if (command.Contains("adb_shell") && !command.Contains(":adb_shell"))
                    {
                        command = command.Replace("adb_shell", "*:adb_shell");
                    }
                    if (command.Contains("get_server_logs") && !command.Contains(":get_server_logs"))
                    {
                        command = command.Replace("get_server_logs", "*:get_server_logs");
                    }
                    if (command.Contains("record_audio") && !command.Contains(":record_audio"))
                    {
                        command = command.Replace("record_audio", "*:record_audio");
                    }
                }
            }
            tempFolder = Path.GetTempPath();
            logFilePath = Path.Combine(tempFolder, "AppiumWizard_Log_" + appiumPort + "_" + DateTime.Now.ToString("d-MMM-yyyy_h-mm-ss_tt") + ".txt");
            if (!command.Contains("webDriverAgentProxyPort"))
            {
                command = command + $@" -dc ""{{""""appium:webDriverAgentUrl"""":""""http://localhost:webDriverAgentProxyPort""""}}""";
            }
            if (!command.Contains("--port"))
            {
                command = command + " --port " + appiumPort;
            }
            appiumLogLevel = Database.QueryDataFromlogLevelTable()["Server" + serverNumber];
            if (!command.Contains(" --log "))
            {
                command = command + " --log " + logFilePath;
            }
            if (!command.Contains("--log-level"))
            {
                if (appiumLogLevel.Equals("info"))
                {
                    appiumLogLevel = "debug:info"; //use debug logs to get element information to update screen control, show info logs in Main screen.
                }
                else if (appiumLogLevel.Equals("error"))
                {
                    appiumLogLevel = "debug:error";
                }
                else
                {
                    appiumLogLevel = "debug:debug";
                }
                command = command + " --log-level " + appiumLogLevel;
            }
            if (!command.Contains(" --local-timezone"))
            {
                command = command + " --local-timezone";
            }
            int webDriverAgentProxyPort = Common.GetFreePort();
            if (serverNumberWDAPortNumber.ContainsKey(serverNumber))
            {
                serverNumberWDAPortNumber[serverNumber] = webDriverAgentProxyPort;
            }
            else
            {
                serverNumberWDAPortNumber.Add(serverNumber, webDriverAgentProxyPort);
            }
            updatedCommand = "/C " + command.Replace("webDriverAgentProxyPort", webDriverAgentProxyPort.ToString());
            Logger.Info("Appium server final updated command : "+updatedCommand);
            string startingText = "\n\t\t------------------------------Starting Appium Server------------------------------\n\n";
            InitializeLogWriter(serverNumber,logFilePath);
            WriteLog(serverNumber, "Running following command : "+updatedCommand.Replace("/C ",string.Empty));
            WriteLog(serverNumber, startingText);
            CloseLogWriter(serverNumber);
            if (!Common.IsNodeInstalled())
            {
                startingText = "\t\t----------------NodeJS not installed. Go to Server -> Troubleshooter to fix the issue and start the appium server----------------\n\n";
                InitializeLogWriter(serverNumber, logFilePath);
                WriteLog(serverNumber, startingText);
                CloseLogWriter(serverNumber);
                statusText = "NodeJS not installed";
                return;
            }           
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    WorkingDirectory = FilesPath.serverInstalledPath,
                    Arguments = updatedCommand,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                startInfo.EnvironmentVariables["ANDROID_HOME"] = FilesPath.executablesFolderPath;
                Process appiumServerProcess = new Process();
                appiumServerProcess.StartInfo = startInfo;
                appiumServerProcess.OutputDataReceived += (sender, e) => AppiumServer_OutputDataReceived(sender, e, serverNumber, webDriverAgentProxyPort);
                appiumServerProcess.ErrorDataReceived += (sender, e) => AppiumServer_OutputDataReceived(sender, e, serverNumber, webDriverAgentProxyPort);
                appiumServerProcess.Exited += (sender, e) => AppiumServer_ProcessExited(sender, e, serverNumber);
                appiumServerProcess.EnableRaisingEvents = true;

                appiumServerProcess.Start();
                appiumServerProcess.BeginOutputReadLine();
                appiumServerProcess.BeginErrorReadLine();
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
                MainScreen.runningProcessesPortNumbers.Add(appiumPort);                
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
        private DateTime lastExecutionTime = DateTime.MinValue;
        
        public void AppiumServer_OutputDataReceived(object sender, DataReceivedEventArgs e, int serverNumber, int webDriverAgentProxyPort)
        {
            try
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    if (MainScreen.main != null)
                    {
                        MainScreen.main.StartLogsServer(serverNumber);
                    }
                    string data = Regex.Replace(e.Data, @"\x1b\[[0-9;]*[mGKH]", "");
                    data = Regex.Replace(data, @"<sup>\(1\)</sup>", "");
                    if (portServerNumberAndFilePath.ContainsKey(serverNumber))
                    {
                        if (data.Contains("No plugins have been installed.") || data.Contains("No plugins activated."))
                        {
                            serverStarted = true;
                            InitializeLogWriter(serverNumber, portServerNumberAndFilePath[serverNumber].Item2);
                            if (appiumLogLevel.Contains("error"))
                            {
                                WriteLog(serverNumber, "Appium Server Running in Port : " + portServerNumberAndFilePath[serverNumber].Item1 + "\n");
                                WriteLog(serverNumber, "Appium command : " + updatedCommand + "\n");
                                WriteLog(serverNumber, "Log level set to error - So only error logs will be displayed.");
                            }
                            WriteLog(serverNumber, "\n\n\t\t------------------------------Appium Server Ready to Use------------------------------\n\n");
                            CloseLogWriter(serverNumber);
                            if (MainScreen.main != null)
                            {
                                MainScreen.main.UpdateTabText(serverNumber, portServerNumberAndFilePath[serverNumber].Item1, true);
                                MainScreen.main.UpdateOpenLogsButtonText(serverNumber, true);
                            }
                        }
                    }
                    if (!serverStarted && (data.Contains("WARN Appium") || data.Contains("[ERROR]") || data.Contains("Fatal Error:") || data.Contains("Error: listen EADDRINUSE") || data.Contains("Could not find 'node' executable") || data.Contains("Error: adb not found") || data.Contains("Error: Instruments crashed") || data.Contains("Error: Unable to find a matching device") || data.Contains("Error: bootstrap failed")))
                    {
                        InitializeLogWriter(serverNumber, portServerNumberAndFilePath[serverNumber].Item2);
                        WriteLog(serverNumber, "\n"+data);
                        CloseLogWriter(serverNumber);
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
                        if (data.Contains("XCUITestDriver") && data.Contains("Could not proxy command to the remote server"))
                        {
                            DateTime now = DateTime.Now;
                            if ((now - lastExecutionTime).TotalSeconds >= 30)
                            {
                                RestartiOSProxyServer(currentUDID,webDriverAgentProxyPort);
                                lastExecutionTime = now;
                            }
                        }
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
                                RestartiOSProxyServer(currentUDID, webDriverAgentProxyPort);
                                if (MainScreen.DeviceInfo.ContainsKey(currentUDID))
                                {
                                    string name = MainScreen.DeviceInfo[currentUDID].Item1;
                                    UpdateScreenControl(currentUDID, "Set Device - " + name);
                                }
                                proxiedUDID = currentUDID;

                                var proxyManager = new ProxyServerManager();
                                proxyManager.WaitForProxyServer("localhost", webDriverAgentProxyPort);
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
                                RestartiOSProxyServer(currentUDID, webDriverAgentProxyPort);
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
                                        RestartiOSProxyServer(currentUDID,webDriverAgentProxyPort);
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
                                            AndroidAPIMethods.CreateSession(androidProxyPort);
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                }
                            }

                        }
                        //------------------------
                        if (data.Contains("POST /session/") && (data.Contains("/element 200") || data.Contains("/elements 200") || data.Contains("/click 200") || data.Contains("/value 200")))
                        {
                            UpdateScreenControl(currentUDID, "");
                        }
                        //------------------------
                        if (UpdateStatusInScreenFlag)
                        {
                            if (ScreenControl.udidScreenControl.ContainsKey(currentUDID))
                            {
                                Task.Run(() =>
                                {
                                    executionStatus.UpdateScreenControl(ScreenControl.udidScreenControl[currentUDID], data);
                                });
                            }
                        }
                        //------------------------
                        if (data.Contains("Using device:"))
                        {
                            string input = data;
                            int startIndex = input.IndexOf(":") + 2;
                            deviceUDID = input.Substring(startIndex);
                            currentUDID = deviceUDID;                            
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
                        if (data.Contains("Error executing adbExec.") | data.Contains("cannot be proxied to UiAutomator2 server because the instrumentation process is not running (probably crashed)"))
                        {
                            AndroidMethods.GetInstance().StopAdbServer();
                            AndroidMethods.GetInstance().StartAdbServer();    
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public bool processExited;
        private void AppiumServer_ProcessExited(object sender, EventArgs e, int serverNumber)
        {
            string exitedText = "\n\t\t<<<-------------------------Failed to Start Appium Server------------------------->>>\n\n";
            processExited = true;
            InitializeLogWriter(serverNumber, portServerNumberAndFilePath[serverNumber].Item2);
            WriteLog(serverNumber, exitedText);
            CloseLogWriter(serverNumber);
        }
        private readonly Dictionary<int, StreamWriter> serverLogWriters = new Dictionary<int, StreamWriter>();
        private void InitializeLogWriter(int serverNumber, string filePath)
        {
            if (!serverLogWriters.ContainsKey(serverNumber))
            {
                var fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                var streamWriter = new StreamWriter(fileStream);
                serverLogWriters[serverNumber] = streamWriter;
            }
        }
        private void WriteLog(int serverNumber, string logLine)
        {
            if (serverLogWriters.ContainsKey(serverNumber))
            {
                serverLogWriters[serverNumber].WriteLine(logLine);
                serverLogWriters[serverNumber].Flush();
            }
        }

        private void CloseLogWriter(int serverNumber)
        {
            if (serverLogWriters.ContainsKey(serverNumber))
            {
                serverLogWriters[serverNumber].Close();
                serverLogWriters[serverNumber].Dispose();
                serverLogWriters.Remove(serverNumber);
            }
        }
        private void RestartiOSProxyServer(string udid, int proxyPort)
        {
            if (iOSAsyncMethods.PortProcessId != null && iOSAsyncMethods.PortProcessId.ContainsKey(proxyPort))
            {
                Common.KillProcessById(iOSAsyncMethods.PortProcessId[proxyPort]);
            }
            iOSAsyncMethods.GetInstance().StartiOSProxyServer(udid, proxyPort, 8100, iOS_Proxy.selectediOSProxyMethod);
        }

        private void UpdateScreenControl(string udid, string text)
        {
            if (UpdateStatusInScreenFlag && ScreenControl.udidScreenControl.ContainsKey(udid))
            {
                var control = ScreenControl.udidScreenControl[udid];
                control.UpdateStatusLabel(control, text);
            }
        }

        public void StopAppiumServer(int serverNumber, int port)
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

                    WriteLog(serverNumber, "\t\t------------------------------Appium Server Stopped------------------------------\n\n");
                    CloseLogWriter(serverNumber);
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
                Timeout = TimeSpan.FromSeconds(5),
            };
            var client = new RestClient(options);
            var request = new RestRequest("/status", Method.Get);
            RestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            if (response.StatusCode == HttpStatusCode.OK && response.Content != null)
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
                if (port == 0)
                {
                    continue;
                }
                var options = new RestClientOptions("http://localhost:" + port)
                {
                    Timeout = TimeSpan.FromSeconds(5),
                };
                var client = new RestClient(options);
                var request = new RestRequest("/sessions", Method.Get);
                RestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);
                if (response.StatusCode == HttpStatusCode.OK && response.Content != null)
                {
                    if (response.Content.Equals("{\"value\":[]}"))
                    {
                        continue;
                    }
                    else
                    {
                        JObject responseObj = JObject.Parse(response.Content);
                        string sessionId = responseObj["value"]?[0]?["id"]?.ToString() ?? string.Empty;
                        string androidId = GetAndroidIdFromAppiumServer(port, sessionId);
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
            var options = new RestClientOptions("http://127.0.0.1:" + port)
            {
                Timeout = TimeSpan.FromSeconds(5),
            };
            var client = new RestClient(options);
            var request = new RestRequest("/session/" + sessionId + "/appium/device/info", Method.Get);
            RestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            string androidId = "noId";
            if (response.Content != null)
            {
                JObject responseObj = JObject.Parse(response.Content);
                androidId = responseObj?["value"]?["androidId"]?.ToString() ?? "noId";
            }
            return androidId;
        }

        public static string GetAndroidIdFromAppiumServer(int appiumPort, string appiumSessionId)
        {
            try
            {
                var options = new RestClientOptions("http://localhost:" + appiumPort)
                {
                    Timeout = TimeSpan.FromSeconds(5),
                };
                var client = new RestClient(options);
                var request = new RestRequest("/session/" + appiumSessionId + "/execute", Method.Post);
                request.AddHeader("Content-Type", "application/json");
                var body = @"{""script"":""mobile:deviceInfo"",""args"":[]}";
                request.AddStringBody(body, DataFormat.Json);
                RestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);
                string androidId = "noId";
                if (response.Content != null)
                {
                    JObject responseObj = JObject.Parse(response.Content);
                    if (responseObj["value"]?["androidId"] != null)
                    {
                        androidId = responseObj?["value"]?["androidId"]?.ToString() ?? string.Empty;
                    }
                }
                return androidId;

            }
            catch (Exception)
            {
                return "error";
            }
        }

        public static Dictionary<string, int> ElementInfo(string url, string elementId)
        {
            Dictionary<string, int> rectangleDict = new Dictionary<string, int>();
            try
            {
                var options = new RestClientOptions(url)
                {
                    //Timeout = TimeSpan.FromSeconds(5),
                };
                var client = new RestClient(options);
                var request = new RestRequest(elementId + "/rect", Method.Get);
                RestResponse response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK && response.Content != null)
                {
                    var jsonObject = JsonConvert.DeserializeObject<dynamic>(response.Content);
                    int x = jsonObject?.value.x;
                    int y = jsonObject?.value.y;
                    int width = jsonObject?.value.width;
                    int height = jsonObject?.value.height;

                    rectangleDict = new Dictionary<string, int>
                    {
                        { "x", x },
                        { "y", y },
                        { "width", width },
                        { "height", height }
                    };
                }
                return rectangleDict;
            }
            catch (Exception)
            {
                return rectangleDict;
            }
        }

        public static bool isPortReachable(int port)
        {
            var options = new RestClientOptions("http://localhost:" + port)
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

//-----------------------------------------------------------------------------------------------------------------------------------------

public class ProxyServerManager
{
    private const int CheckInterval = 1000;
    private const int Timeout = 10000;

    private bool IsProxyServerReady(string host, int port)
    {
        try
        {
            using (var client = new TcpClient(host, port))
            {
                return true;
            }
        }
        catch (SocketException)
        {
            return false;
        }
    }

    public void WaitForProxyServer(string host, int port)
    {
        int elapsed = 0;
        while (!IsProxyServerReady(host, port))
        {
            if (elapsed >= Timeout)
            {
                MessageBox.Show("iOS Proxy server did not start in time. Try selecting different method or start proxy manually from Tools->iOS Proxy", "Failed to start iOS Proxy", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Thread.Sleep(CheckInterval);
            elapsed += CheckInterval;
        }
    }
}
