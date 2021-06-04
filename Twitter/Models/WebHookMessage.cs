using Newtonsoft.Json;

namespace Aijkl.VRChat.Posters.Twitter.Models
{
    class WebHookMessage
    {
        [JsonProperty("content")]
        public string Content { set; get; }
    }
}
