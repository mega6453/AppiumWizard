﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Appium_Wizard
{
    public class ExecutionStatus
    {
        static Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
        public static List<string> listOfSessionIDs = new List<string>();
        static string tempsessionId = string.Empty;
        static string deviceId = string.Empty;
        static string element = string.Empty;
        public void UpdateStatus(string data)
        {
            string statusText;
            if (data.Contains("Appium REST http interface listener started"))
            {
                statusText = "Appium Server Started";
                //ExecutionStatus.statusText = statusText;
                //serverStarted = true;
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
                //ExecutionStatus.statusText = statusText;
            }
           
        }
        string sessionId = string.Empty; static string url = "";
        int x, y, width, height;
        public void UpdateScreenControl(ScreenControl screenControl, string data, int screenDensity=0)
        {
            string statusText;
            if (screenControl != null)
            {
                //if (data.Contains("added to master session list"))
                //{
                //    // statusText = "Session Created";
                //}
                //if (data.Contains("Session created with session id:"))
                //{
                //    string input = data;
                //    string pattern = @"session id: (\w+-\w+-\w+-\w+-\w+)";
                //    Regex regex = new Regex(pattern);
                //    Match match = regex.Match(input);

                //    if (match.Success)
                //    {
                //        string sessionId = match.Groups[1].Value;
                //        listOfSessionIDs.Add(sessionId);
                //        tempsessionId = sessionId;
                //    }
                //    statusText = "Session Created";
                //    //ScreenControl.screenControl.UpdateStatusLabel(statusText);
                //}
                //else if (data.Contains("Using device:"))
                //{
                //    string input = data;
                //    int startIndex = input.IndexOf(":") + 2;
                //    deviceId = input.Substring(startIndex);
                //    keyValuePairs.Add(tempsessionId, deviceId);
                //    statusText = "Set Device " + deviceId;
                //    screenControl.UpdateStatusLabel(screenControl, statusText);
                //}
                //else if (data.Contains("DELETE /session/"))
                //{
                //    statusText = "Session Deleted";
                //    screenControl.UpdateStatusLabel(screenControl, statusText);
                //    string input = data;
                //    string pattern = @"/session/(\w+-\w+-\w+-\w+-\w+)";
                //    Regex regex = new Regex(pattern);
                //    Match match = regex.Match(input);
                //    string sessionId = "";
                //    if (match.Success)
                //    {
                //        sessionId = match.Groups[1].Value;
                //    }
                //    try
                //    {
                //        string deviceUDID = keyValuePairs[sessionId];
                //        int proxyPort = (int)OpenDevice.deviceDetails[deviceUDID]["proxyPort"];
                //        int screenServerPort = (int)OpenDevice.deviceDetails[deviceUDID]["screenPort"];
                //        AndroidAsyncMethods.GetInstance().StartUIAutomatorServer(deviceUDID);
                //        AndroidAPIMethods.CreateSession(proxyPort, screenServerPort);
                //    }
                //    catch (Exception)
                //    {
                //    }
                //}
                if (data.Contains("POST /session/") && data.Contains("/element"))
                {
                    int startIndex = data.IndexOf("/session/") + "/session/".Length;
                    int endIndex = data.IndexOf("/", startIndex);
                    sessionId = data.Substring(startIndex, endIndex - startIndex);
                }

                if (data.Contains("[POST /element]") | data.Contains("[POST /elements]") | data.Contains("{\"using\":"))
                {
                    string pattern = @"(?<=\[POST\s)(http:\/\/[^\/]+\/session\/[^\/]+\/element)";
                    Regex regex = new Regex(pattern);
                    Match match = regex.Match(data);
                    if (match.Success)
                    {
                        url = match.Value;
                    }
                    string json = GetOnlyJson(data);
                    try
                    {
                        if (IsValidJson(json))
                        {
                            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                            element = dictionary["value"];
                            statusText = "Find Element " + element;
                            screenControl.UpdateStatusLabel(screenControl, statusText);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                else if ((data.Contains("[POST /session/") | data.Contains("[POST /element/")) && data.Contains("/click]"))
                {
                    statusText = "Click " + element;
                    screenControl.UpdateStatusLabel(screenControl, statusText);
                    x = x + (width/2);
                    y = y + (height/2);
                    screenControl.DrawDot(screenControl, x, y);
                }
                else if (data.Contains("{\"text\":") | (data.Contains("POST /session/") && data.Contains("/value")))
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
                            screenControl.UpdateStatusLabel(screenControl, statusText);
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
                        if (data.Contains("value") && data.Contains("ELEMENT") && data.Contains("sessionId"))
                        {
                            string json = GetOnlyJson(data);
                            JObject jsonObject = JObject.Parse(json);
                            string elementId = jsonObject["value"]["ELEMENT"].ToString();
                            sessionId = jsonObject["sessionId"].ToString();
                            var result = AppiumServerSetup.ElementInfo(url, elementId);
                            if (result != null)
                            {
                                if (screenDensity !=0)
                                {
                                    x = (int)(result["x"] / (screenDensity / 160f));
                                    y = (int)(result["y"] / (screenDensity / 160f));
                                    width = (int)(result["width"] / (screenDensity / 160f));
                                    height = (int)(result["height"] / (screenDensity / 160f));
                                }
                                else
                                {
                                    x = result["x"];
                                    y = result["y"];
                                    width = result["width"];
                                    height = result["height"];
                                }                             
                                screenControl.DrawRectangle(screenControl, x, y, width, height);
                            }
                            screenControl.UpdateStatusLabel(screenControl, "");
                        }
                        else
                        {
                            screenControl.UpdateStatusLabel(screenControl, "");
                        }
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
