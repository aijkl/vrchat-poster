using Newtonsoft.Json;

namespace Aijkl.LinkPreview.API
{
    public class Response
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
