using System.Collections.Generic;
using Newtonsoft.Json;

namespace Aijkl.VRChat.Posters.Twitter.Shared.Models
{
    public class SDK3PosterParameters
    {
        [JsonProperty("id")]
        public string Id { set; get; }

        [JsonProperty("language")]
        public string Language { set; get; }

        [JsonProperty("tempImageFormat")]
        public string TempImageFormat { set; get; }

        [JsonProperty("targetImagePosters")]
        public List<SDK2ImagePosterBase> TargetImagePosters { set; get; }
    }
}
