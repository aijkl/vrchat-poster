using Aijkl.CloudFlare.API.Models;
using Aijkl.CloudFlare.Internal;
using Newtonsoft.Json;
using System;
using System.Net.Http;

namespace Aijkl.CloudFlare.API
{
    public class CloudFlareAPIException : Exception
    {
        public CloudFlareAPIException(HttpResponseMessage httpResponse) : base(httpResponse.StatusCode.ToString())
        {
            Result = new APIResult<Response>()
            {
                Etag = httpResponse.Headers.ETag,
                ResponseBody = httpResponse.Content.ReadAsStringAsync().Result,
                HttpResponseHeaders = httpResponse.Headers,
                Result = JsonConvert.DeserializeObject<Response>(httpResponse.Content.ReadAsStringAsync().Result)
            };
        }
        public APIResult<Response> Result;        
    }
}
