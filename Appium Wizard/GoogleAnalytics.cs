using Newtonsoft.Json;
using OpenQA.Selenium.Appium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Appium_Wizard
{
    public class GoogleAnalytics
    {
        //static string APISecret = "buMhNs8dTx-oAmn7qcvBxg";
        //static string MeasurementID = "G-ZD94XZM110";
        //static string APISecret = "9hfGBu6GTEmQepGUwGKK7g";
        //static string MeasurementID = "G-ZEK1LV1JRJ";
        static string APISecret = "{GOOGLEANALYTICSAPISECRET}";
        static string MeasurementID = "{GOOGLEANALYTICSMEASUREMENTID}";
        public static string clientId = string.Empty;


        public static void SendEvent(object eventName, string info = "", bool addUserCount = false)
        {
            if (!APISecret.Equals("{GOOGLEANALYTICSAPISECRET}"))
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://www.google-analytics.com/mp/collect?api_secret=" + APISecret + "&measurement_id=" + MeasurementID);
                StringContent? content = null;
                int userCount = -1;
                if (addUserCount)
                {
                    userCount = 1;
                }
                else
                {
                    userCount = 0;
                }
                content = new StringContent($"{{\"client_id\":\"{clientId}\",\"non_personalized_ads\":true,\"events\":[{{\"name\":\"{eventName}\",\"params\":{{\"engagement_time_msec\":{userCount},\"Info\":\"{info}\"}}}}]}}", null, "application/json");
                request.Content = content;
                client.Send(request);
            }
        }


        public static void SendEvent(object eventName, Dictionary<string, string> parameters, bool addUserCount = false)
        {
            if (!APISecret.Equals("{GOOGLEANALYTICSAPISECRET}"))
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://www.google-analytics.com/mp/collect?api_secret=" + APISecret + "&measurement_id=" + MeasurementID);
                StringContent? content = null;
                int userCount = addUserCount ? 1 : 0;

                // Construct the params JSON object
                var paramsObject = new Dictionary<string, object> { { "engagement_time_msec", userCount } };

                // Add custom parameters if provided
                if (parameters != null)
                {
                    foreach (var kvp in parameters)
                    {
                        paramsObject[kvp.Key] = kvp.Value;
                    }
                }

                // Serialize the params object to JSON
                string paramsJson = JsonConvert.SerializeObject(paramsObject);

                content = new StringContent($"{{\"client_id\":\"{clientId}\",\"non_personalized_ads\":true,\"events\":[{{\"name\":\"{eventName}\",\"params\":{paramsJson}}}]}}", null, "application/json");
                request.Content = content;
                client.Send(request);

            }
        }


        public static async void SendExceptionEvent(object eventName, string message="", bool addUserCount = false)
        {
            if (!APISecret.Equals("{GOOGLEANALYTICSAPISECRET}"))
            {
                //eventName = eventName.ToString().Replace(" ", "_");
                //eventName = TrimLength(eventName,39);
                //message = Regex.Replace(message, @"[^a-zA-Z0-9]+", "");
                //message = TrimLength(message,99);
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://www.google-analytics.com/mp/collect?api_secret=" + APISecret + "&measurement_id=" + MeasurementID);
                StringContent? content = null;
                int userCount = -1;
                if (addUserCount)
                {
                    userCount = 1;
                }
                else
                {
                    userCount = 0;
                }
                content = new StringContent($"{{\"client_id\":\"{clientId}\",\"non_personalized_ads\":true,\"events\":[{{\"name\":\"Exception\",\"params\":{{\"engagement_time_msec\":{userCount},\"{eventName}\":\"{message}\"}}}}]}}", null, "application/json");
                string contentAsString = await content.ReadAsStringAsync();
                request.Content = content;
                client.Send(request);
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
