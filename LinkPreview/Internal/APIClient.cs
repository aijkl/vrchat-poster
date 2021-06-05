using Aijkl.LinkPreview.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Aijkl.LinkPreview.Internal
{
    internal class APIClient : IDisposable
    {
        private HttpClient httpClient;
        public APIClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;            
        }        
        internal void AddHeader(string name, string value)
        {
            httpClient.DefaultRequestHeaders.Add(name, value);
        }
        internal void RemoveHeader(string name)
        {
            httpClient.DefaultRequestHeaders.Remove(name);
        }
        internal void SetCookie(Cookie cookie)
        {
            string value = string.Empty;
            value = httpClient.DefaultRequestHeaders.TryGetValues("Cookie", out IEnumerable<string> values) ? value + string.Join(" ", values) : value;
            value += $" {cookie.Name}={cookie.Value};";

            RemoveCookie();
            httpClient.DefaultRequestHeaders.Add("Cookie", value);
        }
        internal void RemoveCookie()
        {
            httpClient.DefaultRequestHeaders.Remove("Cookie");
        }
        internal async Task<APIResult<Response>> SendRequestAsync<T>(HttpRequestMessage httpRequest)
        {
            HttpResponseMessage response = await httpClient.SendAsync(httpRequest);

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NotModified)
            {
                throw new LinkPreviewAPIException(response);
            }

            if (response.StatusCode != HttpStatusCode.NotModified)
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                APIResult<Response> result = new APIResult<Response>
                {
                    Result = JsonConvert.DeserializeObject<Response>(responseBody),
                    ResponseBody = responseBody,
                    HttpStatusCode = response.StatusCode,
                    HttpResponseHeaders = response.Headers,
                    Etag = response.Headers.ETag
                };
                return result;
            }
            else
            {
                return new APIResult<Response>() { HttpStatusCode = response.StatusCode };
            }
        }
        internal async Task<bool> SendRequestAsync(HttpRequestMessage httpRequest)
        {
            HttpResponseMessage response = await httpClient.SendAsync(httpRequest);
            return response.IsSuccessStatusCode;
        }
        internal async Task<HttpResponseMessage> SendRequestAysnc(HttpRequestMessage httpResponse)
        {
            return await httpClient.SendAsync(httpResponse);
        }
        public void Dispose()
        {
            httpClient?.Dispose();
            httpClient = null;
        }
    }
}
