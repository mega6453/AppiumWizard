using Newtonsoft.Json;
using RestSharp;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Appium_Wizard
{
    internal class AppiumServerSetup
    {
        private string AppiumServerFilePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\Resources\\Executables\\appiumserver.exe";
        public bool serverStarted = false;
        public string statusText = "";
        public static string deviceList = "", deviceInfo = "", tempFolder="", logFilePath="";
        public static Dictionary<int, Tuple<Process,string>> listOfProcess = new Dictionary<int,Tuple<Process, string>>();
        //public static Dictionary<int,bool> appiumServerRunningList = new Dictionary<int,bool>();
        public static Dictionary<int, Tuple<int, string>> portServerNumberAndFilePath = new Dictionary<int, Tuple<int, string>>();
        public static List<string> listOfSessionIDs = new List<string>();
        public void StartAppiumServer(int appiumPort, int webDriverAgentProxyPort, int serverNumber)
        {
            string appiumInstallationPath = @"C:\Users\" + Environment.UserName + @"\AppData\Roaming\npm";
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
                    WorkingDirectory = appiumInstallationPath,
                    //Arguments = @"/C appium --allow-cors --default-capabilities ""{\""appium:webDriverAgentUrl\"":\""http://localhost:7777\""}""",                    
                    //Arguments = $@"/C appium --port {appiumPort} --allow-cors --default-capabilities ""{{\""appium:webDriverAgentUrl\"":\""http://localhost:{webDriverAgentProxyPort}\"", \""appium:systemPort\"":{UiAutomatorPort}}}""",
                    Arguments = $@"/C appium --port {appiumPort} --allow-cors --default-capabilities ""{{\""appium:webDriverAgentUrl\"":\""http://localhost:{webDriverAgentProxyPort}\""}}",


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
                appiumServerProcess.OutputDataReceived += (sender, e) => AppiumServer_OutputDataReceived(sender, e, serverNumber);
                appiumServerProcess.ErrorDataReceived += (sender, e) => AppiumServer_OutputDataReceived(sender, e, serverNumber);
                
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
                    portServerNumberAndFilePath[serverNumber] = Tuple.Create(appiumPort,logFilePath);
                }
                else
                {
                    portServerNumberAndFilePath.Add(serverNumber, Tuple.Create(appiumPort, logFilePath));
                }                
            }
            catch (Exception ex)
            {
                string error = "An error occurred while starting Appium Server: " + ex.Message;
                MessageBox.Show(error, "Appium Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine(error);
            }
        }

        Dictionary<string,string> keyValuePairs = new Dictionary<string,string>();
        string tempsessionId;
        private void AppiumServer_OutputDataReceived(object sender, DataReceivedEventArgs e, int serverNumber)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                using (var fileStream = new FileStream(portServerNumberAndFilePath[serverNumber].Item2, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.WriteLine(Regex.Replace(e.Data, @"\x1b\[[0-9;]*[mGKH]", ""));
                    if (e.Data.Contains("No plugins have been installed."))
                    {
                        streamWriter.WriteLine("\n\n\t\t\t\t------------------------------Appium Server Ready to Use------------------------------\n\n");
                    }
                }
                if (e.Data.Contains("Session created with session id:"))
                {
                    string input = e.Data;
                    string pattern = @"session id: (\w+-\w+-\w+-\w+-\w+)";
                    Regex regex = new Regex(pattern);
                    Match match = regex.Match(input);

                    if (match.Success)
                    {
                        string sessionId = match.Groups[1].Value;
                        listOfSessionIDs.Add(sessionId);
                        tempsessionId = sessionId;
                    }                   
                    statusText = "Session Created";
                    if (ScreenControl.screenControl != null)
                    {
                        ScreenControl.screenControl.UpdateStatusLabel(statusText);
                    }
                }
                if (e.Data.Contains("Using device:"))
                {
                    string input = e.Data;
                    int startIndex = input.IndexOf(":") + 2;
                    string deviceCode = input.Substring(startIndex);
                    keyValuePairs.Add(tempsessionId, deviceCode);
                    statusText = "Set Device "+ deviceCode;
                    if (ScreenControl.screenControl != null)
                    {
                        ScreenControl.screenControl.UpdateStatusLabel(statusText);
                    }
                }
                //if (e.Data.Contains("DELETE /session/"))
                //{
                //    statusText = "Session Deleted";
                //    ScreenControl.screenControl.UpdateStatusLabel(statusText);
                //    string input = e.Data;
                //    string pattern = @"/session/(\w+-\w+-\w+-\w+-\w+)";
                //    Regex regex = new Regex(pattern);
                //    Match match = regex.Match(input);
                //    string sessionId="";
                //    if (match.Success)
                //    {
                //        sessionId =  match.Groups[1].Value;
                //    }
                //    try
                //    {
                //        string deviceUDID = keyValuePairs[sessionId];
                //        int proxyPort = (int)OpenDevice.deviceDetails[deviceUDID]["proxyPort"];
                //        int screenServerPort = (int)OpenDevice.deviceDetails[deviceUDID]["screenPort"];
                //        AndroidAsyncMethods.GetInstance().StartUIAutomatorServer(deviceUDID);
                //        AndroidAPIMethods.CreateSession(proxyPort,screenServerPort);
                //    }
                //    catch (Exception)
                //    {
                //    }
                //}
                if (e.Data.Contains("xcuitest"))
                {
                    statusText = "Attempting to load xcuitest(iOS) driver...";
                }
                else if (e.Data.Contains("uiautomator2"))
                {
                    statusText = "Attempting to load uiautomator2(Android) driver...";
                }
                else if (e.Data.Contains("Appium REST http interface listener started"))
                {
                    statusText = "Appium Server Started";
                    serverStarted = true;
                    //int port = int.Parse(GetPortFromInput(e.Data));
                    //appiumServerRunningList.Add(port,true);
                }
                else if (e.Data.Contains("address already in use"))
                {
                    statusText = "address already in use 0.0.0.0:4723";
                }
                else
                {
                    if (ScreenControl.screenControl != null)
                    {
                        if (e.Data.Contains("[POST /element]") | e.Data.Contains("[POST /elements]"))
                        {
                            string json = GetOnlyJson(e.Data);
                            try
                            {
                                if (IsValidJson(json))
                                {
                                    var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                                    statusText = "Find Element " + dictionary["value"];
                                    ScreenControl.screenControl.UpdateStatusLabel(statusText);
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                        else if (e.Data.Contains("Got response with status"))
                        {
                            try
                            {
                                ScreenControl.screenControl.UpdateStatusLabel("");
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }
                }
            }
        }
        static string GetPortFromInput(string input)
        {
            string pattern = @":(\d+)$";
            Match match = Regex.Match(input, pattern);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return null;
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
                    Console.WriteLine(e) ;
                }             
            }
        }



        public static async Task<bool> IsAppiumServerRunningAsync(string appiumUrl, int maxRetries = 10, int retryIntervalMilliseconds = 3000)
        {
            var statusUrl = appiumUrl.TrimEnd('/') + "/status";
            int retryCount = 0;

            while (retryCount < maxRetries)
            {
                try
                {
                    using (var httpClient = new HttpClient())
                    {
                        var response = await httpClient.GetAsync(statusUrl);
                        if (response.IsSuccessStatusCode)
                        {
                            var responseBody = await response.Content.ReadAsStringAsync();
                            if (responseBody.Contains("value"))
                            {
                                return true;
                            }
                        }
                    }
                }
                catch (HttpRequestException)
                {
                    // Ignore and retry
                }

                retryCount++;
                await Task.Delay(retryIntervalMilliseconds);
            }

            return false;
        }
    

        public string GetOnlyJson(string text)
        {
            Match match = Regex.Match(text, @"\{.*\}");
            string output;
            if (match.Success)
            {
                output = match.Value;
            }
            else
            {
                output = "No JSON found in the string";
            }
            return output;
        }

        public bool IsValidJson(string data)
        {
            try
            {
                JsonDocument.Parse(data);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool IsAppiumServerRunning(int port)
        {
            var options = new RestClientOptions("http://localhost:"+port)
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

    }
}
