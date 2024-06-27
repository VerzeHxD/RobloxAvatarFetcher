using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

class Program
{
    static async Task Main(string[] args)
    {
        Console.Write("Enter Roblox Username: ");
        string username = Console.ReadLine();

        try
        {
            string userId = await GetUserId(username);
            if (userId != null)
            {
                await GetAvatarItems(userId);
            }
            else
            {
                Console.WriteLine("User Not Found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An Error Occurred: {ex.Message}");
        }
    }

    static async Task<string> GetUserId(string username)
    {
        using (HttpClient client = new HttpClient())
        {
            var payload = new
            {
                usernames = new[] { username }
            };
            string jsonPayload = JsonConvert.SerializeObject(payload);
            HttpContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("https://users.roblox.com/v1/usernames/users", content);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            JObject user = JObject.Parse(responseBody);

            return user["data"]?[0]?["id"]?.ToString();
        }
    }

    static async Task GetAvatarItems(string userId)
    {
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync($"https://avatar.roblox.com/v1/users/{userId}/avatar");
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            JObject avatar = JObject.Parse(responseBody);

            Console.WriteLine("Avatar Items:");
            foreach (var asset in avatar["assets"])
            {
                string assetType = asset["assetType"]["name"].ToString();
                string assetId = asset["id"].ToString();
                string assetUrl = $"https://www.roblox.com/catalog/{assetId}";
                Console.WriteLine($"{assetType}: {assetUrl}");
            }
        }
    }
}
