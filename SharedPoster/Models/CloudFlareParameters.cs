using Newtonsoft.Json;

namespace Aijkl.VRChat.Posters.Shared.Models
{
    public class CloudFlareParameters
    {
        [JsonProperty("emailAdress")]
        public string EmailAdress { set; get; }

        [JsonProperty("authToken")]
        public string AuthToken { set; get; }

        [JsonProperty("zoneId")]
        public string ZoneId { set; get; }

        [JsonProperty("BaseUrl")]
        public string BaseUrl { set; get; }
    }
}
