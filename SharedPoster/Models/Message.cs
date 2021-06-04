using Newtonsoft.Json;

namespace Aijkl.VRChat.Posters.Shared.Models
{
    class Message
    {
        [JsonProperty("content")]
        public string Content { set; get; }
    }
}