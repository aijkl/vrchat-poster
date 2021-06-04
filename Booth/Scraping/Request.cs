using Aijkl.VRChat.Posters.Booth.Expansion;
using System.Collections.Generic;
using System.Net.Http;
using System;

namespace Aijkl.VRChat.Posters.Booth.Scraping
{
    public static class Request
    {
        public const string BASE_URL = "https://booth.pm";
        public const string USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36";        
        public enum FetchSite
        {
            CrossSite,
            SameOrigin,
            SameSite,
            None
        }
        public enum FetchMode
        {
            Cross,
            Navigate,
            NestedNavigate,
            NoCors,
            SameOrigin,
            Websocket
        }
        public enum FetchDest
        {
            Document,
            Image
        }
        public enum FetchUser
        {
            userActive,
            nonUserActive
        }
        public static HttpRequestMessage CreateGetRequest(string url, FetchSite fetchSite, FetchMode fetchMode, FetchDest fetchDest, FetchUser? fetchUser = null, int? upgradeInsecure = null, int? dnt = null)
        {
            Dictionary<string, string> acceptDictionary = new Dictionary<string, string>
            {
                { "image", "image/avif,image/webp,image/apng,image/*,*/*;q=0.8" },
                { "document", "text/html,application/xhtml+xml,application/xml;q=0.9,image/png,image/jpeg,application/signed-exchange;v=b3;q=0.9" }
            };
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.RequestUri = new Uri(url);
            httpRequestMessage.Headers.Add("Connection", "close");
            if(dnt != null) httpRequestMessage.Headers.Add("DNT", upgradeInsecure.ToString());
            if (upgradeInsecure != null) httpRequestMessage.Headers.Add("Upgrade-Insecure-Requests", upgradeInsecure.ToString());
            httpRequestMessage.Headers.Add("User-Agent", USER_AGENT);
            httpRequestMessage.Headers.Add("DNT", "1");
            httpRequestMessage.Headers.Add("Accept", acceptDictionary.GetValueOrDefault(fetchDest.ToString().ToLower()));
            httpRequestMessage.Headers.Add("Sec-Fetch-Site", fetchSite.ToHeaderValue());
            httpRequestMessage.Headers.Add("Sec-Fetch-Mode", fetchMode.ToHeaderValue());
            httpRequestMessage.Headers.Add("Sec-Fetch-Dest", fetchDest.ToHeaderValue());
            if(fetchUser != null) httpRequestMessage.Headers.Add("Sec-Fetch-User", ((FetchUser)fetchUser).ToHeaderValue());

            httpRequestMessage.Headers.Add("Accept-Language", "ja,en;q=0.9,zh-CN;q=0.8,zh;q=0.7");
            return httpRequestMessage;
        }
    }
}
