using mpNuget;
using System;
using System.Net.Http;
using System.Net.Http.Json;

namespace Common.Sms
{
    public static class MeliPayamak
    {
        public static Uri apiBaseAddress = new Uri("https://console.melipayamak.com");

        public static string Simple(string To, string Text, string From = "50004001423214")
        {
            using (HttpClient client = new HttpClient() { BaseAddress = apiBaseAddress })
            {
                var result = client.PostAsJsonAsync("api/send/simple/d4ef67b192dc47ecabeaa36d5f127e4d",
                    new { from = From, to = To, text = Text }).Result;

                var response = result.Content.ReadAsStringAsync().Result;
                return response;
            }
        }

        public static string Multiple(string[] To, string[] Text, string From = "50004001423214", string Udh = "")
        {
            using (HttpClient client = new HttpClient() { BaseAddress = apiBaseAddress })
            {
                var result = client.PostAsJsonAsync("api/send/multiple/d4ef67b192dc47ecabeaa36d5f127e4d",
                    new
                    {
                        from = From,
                        to = To,
                        text = Text,
                        udh = Udh
                    }).Result;
                var response = result.Content.ReadAsStringAsync().Result;

                return response;
            }
        }

        public static string Advanced(string[] To, string Text, string From = "50004001423214", string Udh = "")
        {
            using (HttpClient client = new HttpClient() { BaseAddress = apiBaseAddress })
            {
                var result = client.PostAsJsonAsync("api/send/advanced/d4ef67b192dc47ecabeaa36d5f127e4d",
                    new
                    {
                        from = From,
                        to = To,
                        text = Text,
                        udh = Udh
                    }).Result;
                var response = result.Content.ReadAsStringAsync().Result;
                return response;
            }
        }

        public static string Simple_Rest(string To, string Text)
        {
            const string username = "9352001451";
            const string password = "#4A1E";
            const string from = "50004001001451";

            const bool isFlash = false;
            RestClient restClient = new RestClient(username, password);
            restClient.Send(To, from, Text, isFlash);
            return "ok";
        }

        public static string PatternSend(string To, string Code)
        {
            Uri apiBaseAddress = new Uri("https://console.melipayamak.com");
            using (HttpClient client = new HttpClient() { BaseAddress = apiBaseAddress })
            {
                var result = client.PostAsJsonAsync("api/send/shared/d4ef67b192dc47ecabeaa36d5f127e4d",
                    new { bodyId = 134071, to = To, args = new[] { Code } }).Result;
                var response = result.Content.ReadAsStringAsync().Result;
                return response.ToString();
            }
        }
    }
}
