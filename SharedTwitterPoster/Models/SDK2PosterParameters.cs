using Aijkl.VRChat.Posters.Twitter.Shared.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Aijkl.VRChat.Posters.Shared.Twitter.Models
{
    public class SDK2PosterParameters
    {        
        [JsonProperty("query")]
        public string Query { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { set; get; }        

        [JsonProperty("lang")]
        public string Lang { set; get; }

        [JsonProperty("locale")]
        public string Locale { set; get; }

        [JsonProperty("sort")]
        public string Sort { set; get; }

        [JsonProperty("format")]
        public string Format { set; get; }
                
        [JsonProperty("rgbGroup")]
        public RGBGroup RGBGroup { set; get; }

        [JsonProperty("translationLanguages")]
        public List<string> TranslationLanguages { set; get; }

        [JsonProperty("fonts")]
        public Dictionary<string, string> Fonts { set; get; }

        [JsonProperty("quality")]
        public int Quality { set; get; }

        [JsonProperty("page")]
        public int Page { set; get; }
    }
}
