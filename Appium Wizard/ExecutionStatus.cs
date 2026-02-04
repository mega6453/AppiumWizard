using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using Newtonsoft.Json;
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
        static string elementId = string.Empty;
        static string url = "";
        int x, y, width, height, screenDensity;
        ScreenControl screenControl;

        public void UpdateScreenControl(ScreenControl screenControl, string data)
        {
            this.screenControl = screenControl;
            screenDensity = screenControl.screenDensity;
            try
            {
                string statusText;
                if (screenControl != null)
                {
                    // Handling for Find element
                    if (data.Contains("[POST /element]") || data.Contains("[POST /elements]"))
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
                                if (dictionary.ContainsKey("selector"))
                                {
                                    element = dictionary["selector"];
                                }
                                else if (dictionary.ContainsKey("value"))
                                {
                                    element = dictionary["value"];
                                }
                                statusText = "Find Element " + element;
                                screenControl.UpdateStatusLabel(screenControl, statusText);
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                    // Handling for Click
                    else if ((data.Contains(" --> POST /session/") && data.Contains("/element/")) && data.Contains("/click {}"))
                    {
                        statusText = "Click " + element;
                        screenControl.UpdateStatusLabel(screenControl, statusText);
                        int updatedX = x + (width / 2);
                        int updatedY = y + (height / 2);
                        screenControl.DrawDot(screenControl, updatedX, updatedY);
                    }
                    // Handling for Send text
                    else if (data.Contains("{\"text\":") | (data.Contains("POST /session/") && data.Contains("/value")))
                    {
                        screenControl.DrawRectangle(screenControl, x, y, width, height);
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
                    // Handling for installApp action
                    else if (data.Contains("POST /session/") && data.Contains("/appium/device/install_app"))
                    {
                        string fileName = string.Empty;
                        string appPath = GetValueFromJson(data, "appPath");
                        if (appPath != null)
                        {
                            fileName = Path.GetFileName(appPath);
                        }
                        statusText = "Install " + fileName;
                        screenControl.UpdateStatusLabel(screenControl, statusText);
                    }
                    // Handling for uninstallApp action
                    else if (data.Contains("POST /session/") && data.Contains("/appium/device/remove_app"))
                    {
                        string appId = GetValueFromJson(data, "appId");
                        statusText = "Uninstall " + appId;
                        screenControl.UpdateStatusLabel(screenControl, statusText);
                    }
                    // Handling for closeApp action
                    else if (data.Contains("POST /session/") && data.Contains("/appium/app/terminate_app"))
                    {
                        string appId = GetValueFromJson(data, "appId");
                        statusText = "Close App " + appId;
                        screenControl.UpdateStatusLabel(screenControl, statusText);
                    }
                    // Handling for install and uninstall success
                    else if (data.Contains("/appium/device/install_app 200") | data.Contains("/appium/device/terminate_app 200") | data.Contains("/appium/device/remove_app 200"))
                    {
                        screenControl.UpdateStatusLabel(screenControl, "");
                    }
                    // Handling for launchApp action
                    else if (data.Contains("POST /session/") && data.Contains("/appium/app/launch"))
                    {
                        statusText = "Launch App";
                        screenControl.UpdateStatusLabel(screenControl, statusText);
                    }

                    // Handling for clear
                    else if (data.Contains("[POST /element/") && data.Contains("/clear]"))
                    {
                        statusText = "Clear " + element;
                        screenControl.UpdateStatusLabel(screenControl, statusText);
                    }
                    // Handling for getText action
                    else if (data.Contains("[HTTP] --> GET /session/") && data.Contains("/text"))
                    {
                        statusText = "Get Text from " + element;
                        screenControl.UpdateStatusLabel(screenControl, statusText);
                    }
                    // Handling for isDisplayed action
                    else if (data.Contains("[HTTP] --> GET /session/") && data.Contains("/displayed"))
                    {
                        statusText = "is Displayed " + element;
                        screenControl.UpdateStatusLabel(screenControl, statusText);
                    }
                    // Handling for isEnabled action
                    else if (data.Contains("[HTTP] --> GET /session/") && data.Contains("/enabled"))
                    {
                        statusText = "is Enabled " + element;
                        screenControl.UpdateStatusLabel(screenControl, statusText);
                    }
                    // Handling for isSelected action
                    else if (data.Contains("[HTTP] --> GET /session/") && data.Contains("/selected"))
                    {
                        statusText = "is Selected " + element;
                        screenControl.UpdateStatusLabel(screenControl, statusText);
                    }
                    // Handling for getAttribute action
                    else if (data.Contains("[HTTP] --> GET /session/") && data.Contains("/attribute/"))
                    {
                        string attributeName = data.Substring(data.LastIndexOf("/") + 1).TrimEnd(']').Replace("{}", "");
                        statusText = "Get Attribute \"" + attributeName.Trim() + "\" from Element " + element;
                        screenControl.UpdateStatusLabel(screenControl, statusText);
                    }
                    // Handling for getCssValue action
                    else if (data.Contains("[HTTP] --> GET /session/") && data.Contains("/css/"))
                    {
                        string cssProperty = data.Substring(data.LastIndexOf("/") + 1).TrimEnd(']');
                        statusText = "Get CSS Value \"" + cssProperty + "\" from Element " + element;
                        screenControl.UpdateStatusLabel(screenControl, statusText);
                    }
                    // Handling for getElementSize action
                    else if (data.Contains("[HTTP] --> GET /session/") && data.Contains("/size"))
                    {
                        statusText = "Get Size of " + element;
                        screenControl.UpdateStatusLabel(screenControl, statusText);
                    }
                    // Handling for getElementLocation action
                    else if (data.Contains("[HTTP] --> GET /session/") && data.Contains("/location"))
                    {
                        statusText = "Get Location of " + element;
                        screenControl.UpdateStatusLabel(screenControl, statusText);
                    }
                    // Handling for getElementTagName action
                    else if (data.Contains("[HTTP] --> GET /session/") && data.Contains("/name"))
                    {
                        statusText = "Get Tag Name of " + element;
                        screenControl.UpdateStatusLabel(screenControl, statusText);
                    }
                    // Handling for getElementRect action
                    else if (data.Contains("[HTTP] --> GET /session/") && data.Contains("/rect"))
                    {
                        statusText = "Get Rect of " + element;
                        screenControl.UpdateStatusLabel(screenControl, statusText);
                    }

                    //Drag Gesture - Android
                    else if (data.Contains(" POST /session/") && data.Contains("dragGesture"))
                    {
                        string jsonString = GetOnlyJson(data);
                        var jsonDocument = JsonDocument.Parse(jsonString);
                        var root = jsonDocument.RootElement;

                        var argsElement = root.GetProperty("args")[0];
                        var argsDictionary = new Dictionary<string, object>();

                        foreach (var property in argsElement.EnumerateObject())
                        {
                            argsDictionary[property.Name] = property.Value.ToString();
                        }
                        int startX = (int)(Convert.ToInt32(argsDictionary["startX"]) / (screenDensity / 160f));
                        int startY = (int)(Convert.ToInt32(argsDictionary["startY"]) / (screenDensity / 160f));
                        int endX = (int)(Convert.ToInt32(argsDictionary["endX"]) / (screenDensity / 160f));
                        int endY = (int)(Convert.ToInt32(argsDictionary["endY"]) / (screenDensity / 160f));
                        screenControl.DrawArrow(screenControl, startX, startY, endX, endY);
                    }

                    //Drag Gesture - iOS
                    else if (data.Contains(" POST /session/") && data.Contains("dragFromToForDuration"))
                    {
                        string jsonString = GetOnlyJson(data);
                        var jsonDocument = JsonDocument.Parse(jsonString);
                        var root = jsonDocument.RootElement;

                        var argsElement = root.GetProperty("args")[0];
                        var argsDictionary = new Dictionary<string, object>();

                        foreach (var property in argsElement.EnumerateObject())
                        {
                            argsDictionary[property.Name] = property.Value.ToString();
                        }
                        int startX = Convert.ToInt32(argsDictionary["fromX"]);
                        int startY = Convert.ToInt32(argsDictionary["fromY"]);
                        int endX = Convert.ToInt32(argsDictionary["toX"]);
                        int endY = Convert.ToInt32(argsDictionary["toY"]);
                        screenControl.DrawArrow(screenControl, startX, startY, endX, endY);
                    }

                    else if (data.Contains("POST /session/") && data.Contains("/execute/sync") && data.Contains("{\"script\":\"appiumwizard"))
                    {
                        var match = Regex.Match(data, @"appiumwizard:(.*?)""");

                        if (match.Success)
                        {
                            statusText = match.Groups[1].Value;
                            screenControl.UpdateStatusLabel(screenControl, statusText);
                        }
                    }

                    // Handling Responses
                    else if (data.Contains("Got response with status 200"))
                    {
                        try
                        {
                            if (data.Contains("value") && data.Contains("ELEMENT") && data.Contains("sessionId"))
                            {
                                string json = GetOnlyJson(data);
                                JObject jsonObject = JObject.Parse(json);
                                string elementId = jsonObject["value"]["ELEMENT"].ToString();
                                GetRectValues(url, elementId);
                            }
                            screenControl.UpdateStatusLabel(screenControl, "");
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }


        private void GetRectValues(string url, string elementId)
        {
            var result = AppiumServerSetup.ElementInfo(url, elementId);
            if (result.Count != 0 && result != null)
            {
                if (screenControl.isAndroid)
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

        public static string GetValueFromJson(string data, string expected)
        {
            string json = GetOnlyJson(data);
            JObject jsonObject = JObject.Parse(json);
            string output = jsonObject[expected].ToString();
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

        private string GetElementId(string input)
        {
            string pattern = @"/element/(?<elementId>[a-fA-F0-9\-]+)/";
            Match match = Regex.Match(input, pattern);
            string elementId = "";
            if (match.Success)
            {
                elementId = match.Groups["elementId"].Value;
            }
            return elementId;
        }
    }
}
