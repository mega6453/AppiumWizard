using Newtonsoft.Json;

namespace Appium_Wizard
{
    public class GoogleAnalytics
    {
        static string APISecret = "{GOOGLEANALYTICSAPISECRET}";
        static string MeasurementID = "{GOOGLEANALYTICSMEASUREMENTID}";
        public static string clientId = string.Empty;


        public static void SendEvent(object eventName, string info = "", bool addUserCount = false)
        {
            try
            {
                Task.Run(() =>
                {
                    if (!APISecret.Contains("ANALYTICSAPISECRET"))
                    {
                        var client = new HttpClient();
                        var request = new HttpRequestMessage(HttpMethod.Post, "https://www.google-analytics.com/mp/collect?api_secret=" + APISecret + "&measurement_id=" + MeasurementID);
                        StringContent? content = null;
                        int userCount = addUserCount ? 1 : 0;
                        content = new StringContent($"{{\"client_id\":\"{clientId}\",\"non_personalized_ads\":true,\"events\":[{{\"name\":\"{eventName}\",\"params\":{{\"engagement_time_msec\":{userCount},\"Info\":\"{info}\"}}}}]}}", null, "application/json");
                        request.Content = content;
                        client.Send(request);
                    }
                });
            }
            catch (Exception e)
            {
            }
        }


        public static void SendEvent(object eventName, Dictionary<string, string> parameters, bool addUserCount = false)
        {
            Task.Run(() =>
            {
                if (!APISecret.Contains("ANALYTICSAPISECRET"))
                {
                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Post, "https://www.google-analytics.com/mp/collect?api_secret=" + APISecret + "&measurement_id=" + MeasurementID);
                    StringContent? content = null;
                    int userCount = addUserCount ? 1 : 0;

                    var paramsObject = new Dictionary<string, object> { { "engagement_time_msec", userCount } };

                    if (parameters != null)
                    {
                        foreach (var kvp in parameters)
                        {
                            paramsObject[kvp.Key] = kvp.Value;
                        }
                    }

                    string paramsJson = JsonConvert.SerializeObject(paramsObject);

                    content = new StringContent($"{{\"client_id\":\"{clientId}\",\"non_personalized_ads\":true,\"events\":[{{\"name\":\"{eventName}\",\"params\":{paramsJson}}}]}}", null, "application/json");
                    request.Content = content;
                    client.Send(request);
                }
            });

        }


        public static void SendExceptionEvent(object eventName, string message="", bool addUserCount = false)
        {
            if (!APISecret.Contains("ANALYTICSAPISECRET"))
            {
                Task.Run(async () =>
                {
                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Post, "https://www.google-analytics.com/mp/collect?api_secret=" + APISecret + "&measurement_id=" + MeasurementID);
                    StringContent content = null;
                    int userCount = addUserCount ? 1 : 0;
                    content = new StringContent($"{{\"client_id\":\"{clientId}\",\"non_personalized_ads\":true,\"events\":[{{\"name\":\"Exception\",\"params\":{{\"engagement_time_msec\":{userCount},\"{eventName}\":\"{message}\"}}}}]}}", null, "application/json");
                    string contentAsString = await content.ReadAsStringAsync();
                    request.Content = content;
                    await client.SendAsync(request);
                });
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
