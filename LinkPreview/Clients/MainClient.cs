using Aijkl.LinkPreview.API;
using Aijkl.LinkPreview.Internal;
using System.Net.Http;

namespace Aijkl.LinkPreview.API.Clients
{
    public class MainClient
    {
        private readonly APIClient apiClient;
        private readonly string apikey;
        internal MainClient(APIClient apiClient, string apikey)
        {
            this.apiClient = apiClient;
            this.apikey = apikey;
        }        
        public APIResult<Response> Preview(string url)
        {
            HttpRequestMessage request = Request.CreateGetRequest($"q={url}", apikey);
            return apiClient.SendRequestAsync<APIResult<Response>>(request).Result;
        }
    }
}
