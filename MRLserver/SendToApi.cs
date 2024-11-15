namespace MRLserver
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    class SendToApi
    {
        // Replace these values with your actual API Key and Device ID
        private static string ApiKey = "5795ERy805wUWYhswsmO5kuxcF8O8r";
        private static string deviceId = "2806523";
        private static readonly string ApiUrl = "https://api.hologram.io/v1/messages";

        // Hologram API endpoint (change as needed)
        //private static string apiUrl = $"https://dashboard.hologram.io/api/1/devices/{deviceId}/data";
        //private static string apiUrl = $"https://api.hologram.io/v1/devices/{deviceId}/data";
        //private static string apiUrl = $"https://dashboard.hologram.io/api/1/devices/messages/2806523/42d8cf16382a48bcac674b1e900b1ae0";

        // https://dashboard.hologram.io/api/1/devices/2806523/data
        public static async Task SendMessageAsync(string message)
        {
            using (HttpClient client = new HttpClient())
            {
                // API kulcs hozzáadása a fejlécbe
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");

                // Üzenet adatainak előkészítése
                var postData = new
                {
 //                   to = phoneNumber,
                    message = message
                };

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(postData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // HTTP POST kérés küldése
                HttpResponseMessage response = await client.PostAsync(ApiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Üzenet sikeresen elküldve!");
                }
                else
                {
                    Console.WriteLine($"Hiba történt az üzenet küldésekor: {response.ReasonPhrase}");
                }
            }
        }
    }

}
