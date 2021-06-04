using Aijkl.CloudFlare.API.Clients;
using Aijkl.CloudFlare.Internal;
using System;
using System.Net.Http;

namespace Aijkl.CloudFlare.API
{
    public class CloudFlareAPIClient : IDisposable
    {        
        private APIClient apiClient;
        public CloudFlareAPIClient(string emailAdress, string authKey, HttpClient httpClient = null)
        {            
            apiClient = new APIClient(httpClient ?? new HttpClient());
            apiClient.AddHeader("X-Auth-Email", emailAdress);
            apiClient.AddHeader("X-Auth-Key", authKey);
        }
        public ZoneClient Zone => new ZoneClient(apiClient);
        public void Dispose()
        {
            apiClient?.Dispose();
            apiClient = null;
        }
    }
}
