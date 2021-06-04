using Aijkl.VRChat.Posters.Settings;
using Newtonsoft.Json;

namespace Aijkl.VRChat.Booth.Models
{    
    public class PosterParameters
    {                
        [JsonProperty("fontName")]
        public string FontName { set; get; }

        [JsonProperty("fileName")]
        public string FileName { set; get; }

        [JsonProperty("keyWord")]
        public string KeyWord { set; get; }

        [JsonProperty("format")]
        public string Format { set; get; }

        [JsonProperty("minLike")]
        public int MinLike { set; get; }

        [JsonProperty("page")]
        public int Page { set; get; }

        [JsonProperty("quality")]
        public int Quality { set; get; }

        [JsonProperty("rgbGroup")]
        public RgbGroup RgbGroup { set; get; }
    }        
}
