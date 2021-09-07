using Aijkl.VRChat.Posters.Twitter.Shared.Models;
using Newtonsoft.Json;

namespace Aijkl.VRChat.Posters.Shared.Twitter.Models
{
    public class Common
    {
        [JsonProperty("ffmpegParameters")]
        public FFMpegParameters FFMpegParameters { set; get; }

        [JsonProperty("updateInterval")]
        public int UpdateInterval { set; get; }
    }    
}
