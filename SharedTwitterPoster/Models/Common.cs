using Newtonsoft.Json;

namespace Aijkl.VRChat.Posters.Shared.Twitter.Models
{
    public class Common
    {        
        [JsonProperty("discordWebHookURL")]
        public string DiscordWebHookURL { set; get; }        

        [JsonProperty("updateInterval")]
        public int UpdateInterval { set; get; }

        [JsonProperty("timeout")]
        public int TimeOut { set; get; }        
    }    
}
