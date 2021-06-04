using System;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using Aijkl.VRChat.Posters.Shared.Models;

namespace Aijkl.VRChat.Posters.Shared
{
    public class DiscordClient : IDisposable
    {
        private HttpClient httpClient;        
        public DiscordClient(HttpClient httpClient = null)
        {
            this.httpClient = httpClient ?? new HttpClient();           
        }

        public void Dispose()
        {
            httpClient?.Dispose();
            httpClient = null;
        }

        public void PostMessage(string url, string message)
        {
            Message messageJson = new Message
            {
                Content = message
            };
            string json = JsonConvert.SerializeObject(messageJson);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            httpClient.PostAsync(url, content).Wait();
        }
    }
}
