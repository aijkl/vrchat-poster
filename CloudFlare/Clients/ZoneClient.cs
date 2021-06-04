using Aijkl.CloudFlare.API.Models;
using Aijkl.CloudFlare.Internal;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Aijkl.CloudFlare.API.Clients
{
    public class ZoneClient
    {
        private readonly APIClient apiClient;
        internal ZoneClient(APIClient apiClient)
        {
            this.apiClient = apiClient;
        }        
        public APIResult<Response> PurgeFilesByUrl(string zone,List<string> urls)
        {
            var request = Request.CreatePostRequest($"zones/{zone}/purge_cache", json: JsonConvert.SerializeObject(new PurgeRequestObject()
            {
                Files = urls
            }));
            return apiClient.SendRequestAsync<APIResult<Response>>(request).Result;
        }
    }
}
