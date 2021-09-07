using Newtonsoft.Json;

namespace Aijkl.VRChat.Posters.Twitter.Shared.Models
{
    public class SDK2ImagePosterBase
    {
        [JsonProperty("id")]
        public string Id { set; get; }

        [JsonProperty("fileName")]
        public string FileName { set; get; }

        [JsonProperty("language")]
        public string Language { set; get; }

        [JsonProperty("pageIndex")]
        public int PageIndex { set; get; }

        public override bool Equals(object? obj)
        {
            if (obj is SDK2ImagePosterBase sdk2ImagePosterBase)
            {
                return Id == sdk2ImagePosterBase.Id && Language == sdk2ImagePosterBase.Language && PageIndex == sdk2ImagePosterBase.PageIndex;
            }
            return false;
        }
        protected bool Equals(SDK2ImagePosterBase other)
        {
            return Id == other.Id && Language == other.Language && PageIndex == other.PageIndex;
        }
    }
}
