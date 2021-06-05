using Aijkl.LinkPreview.Internal;
using Newtonsoft.Json;
using System;
using System.Net.Http;

namespace Aijkl.LinkPreview.API
{
    public class LinkPreviewAPIException : Exception
    {
        public LinkPreviewAPIException(HttpResponseMessage httpResponse) : base(httpResponse.StatusCode.ToString())
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
