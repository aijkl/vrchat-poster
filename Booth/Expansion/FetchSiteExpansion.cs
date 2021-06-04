using Aijkl.VRChat.Posters.Booth.Scraping;

namespace Aijkl.VRChat.Posters.Booth.Expansion
{
    public static class FetchSiteExpansion
    {
        public static string ToHeaderValue(this Request.FetchSite fetchSite)
        {
            string[] parameters = { "cross-site", "same-origin", "same-site", "none" };
            return parameters[(int)fetchSite];
        }
        public static string ToHeaderValue(this Request.FetchMode fetchMode)
        {
            string[] parameters = { "cross", "navigate", "nested-navigate", "no-cors" , "same-origin", "websocket" };
            return parameters[(int)fetchMode];
        }
        public static string ToHeaderValue(this Request.FetchDest fetchDest)
        {
            string[] parameters = { "document", "image"};
            return parameters[(int)fetchDest];
        }
        public static string ToHeaderValue(this Request.FetchUser fetchDest)
        {
            string[] parameters = { "?0", "?1" };
            return parameters[(int)fetchDest];
        }
    }
}
