using Newtonsoft.Json;

namespace Aijkl.VRChat.Posters.Models
{
    public class Common
    {
        [JsonProperty("updateInterval")]
        public int UpdateInterval { set; get; }

        [JsonProperty("discordWebHookURL")]
        public string DiscordWebHookURL { set; get; }
    }
}
