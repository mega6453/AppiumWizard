using Newtonsoft.Json;
using NLog;

namespace Appium_Wizard
{
    public class GoogleAnalytics
    {
        static string APISecret = "{GOOGLEANALYTICSAPISECRET}";
        static string MeasurementID = "{GOOGLEANALYTICSMEASUREMENTID}";
        public static string clientId = string.Empty;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static Dictionary<string, object> userProperties = new Dictionary<string, object>();

        /// <summary>
        /// Sets a user property that will be sent with all future events.
        /// User properties are useful for segmenting users in GA4 dashboards.
        /// </summary>
        /// <param name="propertyName">Name of the user property (e.g., "app_version", "os_type")</param>
        /// <param name="propertyValue">Value of the user property</param>
        public static void SetUserProperty(string propertyName, string propertyValue)
        {
            if (!string.IsNullOrEmpty(propertyName) && !string.IsNullOrEmpty(propertyValue))
            {
                userProperties[propertyName] = new { value = propertyValue };
                Logger.Info($"User property set: {propertyName} = {propertyValue}");
            }
        }

        /// <summary>
        /// Sets multiple user properties at once.
        /// </summary>
        public static void SetUserProperties(Dictionary<string, string> properties)
        {
            if (properties != null)
            {
                foreach (var kvp in properties)
                {
                    SetUserProperty(kvp.Key, kvp.Value);
                }
            }
        }

        public static void SendEvent(object eventName, string info = "", bool addUserCount = false)
        {
            Logger.Info(eventName+" : "+info);
            try
            {
                Task.Run(() =>
                {
                    if (!APISecret.Contains("ANALYTICSAPISECRET"))
                    {
                        var client = new HttpClient();
                        var request = new HttpRequestMessage(HttpMethod.Post, "https://www.google-analytics.com/mp/collect?api_secret=" + APISecret + "&measurement_id=" + MeasurementID);

                        var payload = new
                        {
                            client_id = clientId,
                            non_personalized_ads = true,
                            user_properties = userProperties.Count > 0 ? userProperties : null,
                            events = new[]
                            {
                                new
                                {
                                    name = eventName.ToString(),
                                    @params = new Dictionary<string, object>
                                    {
                                        { "Info", info }
                                    }
                                }
                            }
                        };

                        string jsonPayload = JsonConvert.SerializeObject(payload, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        });

                        var content = new StringContent(jsonPayload, null, "application/json");
                        request.Content = content;
                        client.Send(request);
                    }
                });
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error sending GA event");
            }
        }


        public static void SendEvent(object eventName, Dictionary<string, string> parameters, bool addUserCount = false)
        {
            try
            {
                Task.Run(() =>
                {
                    if (!APISecret.Contains("ANALYTICSAPISECRET"))
                    {
                        var client = new HttpClient();
                        var request = new HttpRequestMessage(HttpMethod.Post, "https://www.google-analytics.com/mp/collect?api_secret=" + APISecret + "&measurement_id=" + MeasurementID);

                        var paramsObject = new Dictionary<string, object>();

                        if (parameters != null)
                        {
                            foreach (var kvp in parameters)
                            {
                                paramsObject[kvp.Key] = kvp.Value;
                            }
                        }

                        var payload = new
                        {
                            client_id = clientId,
                            non_personalized_ads = true,
                            user_properties = userProperties.Count > 0 ? userProperties : null,
                            events = new[]
                            {
                                new
                                {
                                    name = eventName.ToString(),
                                    @params = paramsObject
                                }
                            }
                        };

                        string jsonPayload = JsonConvert.SerializeObject(payload, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        });

                        var content = new StringContent(jsonPayload, null, "application/json");
                        request.Content = content;
                        client.Send(request);
                    }
                });
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error sending GA event with parameters");
            }
        }


        public static void SendExceptionEvent(object eventName, string message="", bool addUserCount = false)
        {
            string eventNameString = eventName?.ToString() ?? "Unknown Exception";
            Logger.Error(eventNameString, message);
            if (!APISecret.Contains("ANALYTICSAPISECRET"))
            {
                try
                {
                    Task.Run(async () =>
                    {
                        var client = new HttpClient();
                        var request = new HttpRequestMessage(HttpMethod.Post, "https://www.google-analytics.com/mp/collect?api_secret=" + APISecret + "&measurement_id=" + MeasurementID);

                        var payload = new
                        {
                            client_id = clientId,
                            non_personalized_ads = true,
                            user_properties = userProperties.Count > 0 ? userProperties : null,
                            events = new[]
                            {
                                new
                                {
                                    name = "Exception",
                                    @params = new Dictionary<string, object>
                                    {
                                        { "exception_type", eventNameString },
                                        { "exception_message", message }
                                    }
                                }
                            }
                        };

                        string jsonPayload = JsonConvert.SerializeObject(payload, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        });

                        var content = new StringContent(jsonPayload, null, "application/json");
                        request.Content = content;
                        await client.SendAsync(request);
                    });
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Error sending GA exception event");
                }
            }
        }

        public enum screenName
        {
            App_Launched,
            About_Screen,
            DeviceInformation_Screen,
            ImportProfile_Screen,
            iOSProfileManagement_Screen,
            InstallApp,
            Loading_Screen,
            Main_Screen,
            ScreenControl_Screen,
            ServerConfig_Screen,
            TroubleShooter_Screen,
            TroubleShooting_Guide
        }

        private static string TrimLength(object input, int maxCount)
        {
            return input.ToString().Substring(0, Math.Min(input.ToString().Length, maxCount));
        }
    }
}
