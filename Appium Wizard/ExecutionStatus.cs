using Newtonsoft.Json;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Appium_Wizard
{
    public static class ExecutionStatus
    {
        static string statusText = "";
        static Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
        public static List<string> listOfSessionIDs = new List<string>();
        static string tempsessionId = string.Empty;
        static string deviceId = string.Empty;
        static string element = string.Empty;
        public static void UpdateStatus(string data)
        {

            if (data.Contains("Session created with session id:"))
            {
                string input = data;
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
                ScreenControl.screenControl.UpdateStatusLabel(statusText);
            }
            if (data.Contains("Using device:"))
            {
                string input = data;
                int startIndex = input.IndexOf(":") + 2;
                deviceId = input.Substring(startIndex);
                keyValuePairs.Add(tempsessionId, deviceId);
                statusText = "Set Device " + deviceId;
                ScreenControl.screenControl.UpdateStatusLabel(statusText);
            }
            if (data.Contains("DELETE /session/"))
            {
                statusText = "Session Deleted";
                ScreenControl.screenControl.UpdateStatusLabel(statusText);
                string input = data;
                string pattern = @"/session/(\w+-\w+-\w+-\w+-\w+)";
                Regex regex = new Regex(pattern);
                Match match = regex.Match(input);
                string sessionId = "";
                if (match.Success)
                {
                    sessionId = match.Groups[1].Value;
                }
                try
                {
                    string deviceUDID = keyValuePairs[sessionId];
                    int proxyPort = (int)OpenDevice.deviceDetails[deviceUDID]["proxyPort"];
                    int screenServerPort = (int)OpenDevice.deviceDetails[deviceUDID]["screenPort"];
                    AndroidAsyncMethods.GetInstance().StartUIAutomatorServer(deviceUDID);
                    AndroidAPIMethods.CreateSession(proxyPort, screenServerPort);
                }
                catch (Exception)
                {
                }
            }

            //if (data.Contains("Could not proxy command to the remote server"))
            //{
            //    AndroidMethods.GetInstance().StopUIAutomator(deviceId);
            //    MessageBox.Show("Session creation failed. Please retry again.\nIf issue persists, restart your device and try again.", "Failed to create session", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            //if (data.Contains("xcuitest"))
            //{
            //    statusText = "Attempting to load xcuitest(iOS) driver...";
            //}
            //else if (data.Contains("uiautomator2"))
            //{
            //    statusText = "Attempting to load uiautomator2(Android) driver...";
            //}
            //else if (data.Contains("Appium REST http interface listener started"))
            //{
            //    statusText = "Appium Server Started";
            //    serverStarted = true;
            //    //int port = int.Parse(GetPortFromInput(data));
            //    //appiumServerRunningList.Add(port,true);
            //}
            else if (data.Contains("address already in use"))
            {
                statusText = "address already in use 0.0.0.0:4723";
            }
            else
            {
                //if (data.Contains("[POST /element]") | data.Contains("[POST /elements]"))
                if (data.Contains("{\"using\":"))
                {
                    string json = GetOnlyJson(data);
                    try
                    {
                        if (IsValidJson(json))
                        {
                            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                            element = dictionary["value"];
                            statusText = "Find Element " + element;
                            ScreenControl.screenControl.UpdateStatusLabel(statusText);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                else if (data.Contains("POST /session/") && data.Contains("/click"))
                {
                    statusText = "Click " + element;
                    ScreenControl.screenControl.UpdateStatusLabel(statusText);
                }
                //else if (data.Contains("POST /session/") && data.Contains("/value"))
                else if (data.Contains("{\"text\":"))
                {
                    string text;
                    string json = GetOnlyJson(data);
                    try
                    {
                        if (IsValidJson(json))
                        {
                            var jsonObject = JsonConvert.DeserializeObject<dynamic>(json);
                            text = jsonObject.text;
                            statusText = "Send text \"" + text + "\" to " + element;
                            ScreenControl.screenControl.UpdateStatusLabel(statusText);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                else if (data.Contains("Got response with status"))
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

        public static string GetOnlyJson(string text)
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

        public static bool IsValidJson(string data)
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

        static string GetPortFromInput(string input)
        {
            string pattern = @":(\d+)$";
            Match match = Regex.Match(input, pattern);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            return "";
        }
    }
}
