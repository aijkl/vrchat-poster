using Newtonsoft.Json;

namespace Aijkl.VRChat.Posters.Twitter.Shared.Models
{
    public class RGBGroup
    {
        [JsonProperty("tweet")]
        public byte[] Tweet { set; get; }

        [JsonProperty("background")]
        public byte[] Background { set; get; }
    }
}
