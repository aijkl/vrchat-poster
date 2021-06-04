using Newtonsoft.Json;

namespace Aijkl.VRChat.Posters.Shared.Twitter.Models
{
    public class TwitterParameters
    {
        [JsonProperty("apiKey")]
        public string APIKey { get; set; }

        [JsonProperty("apiSecretkey")]
        public string APISecretKey { get; set; }

        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }

        [JsonProperty("accessTokenSecret")]
        public string AccessTokenSecret { get; set; }
    }
}
