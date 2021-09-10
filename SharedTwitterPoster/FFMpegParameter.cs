using Newtonsoft.Json;

namespace Aijkl.VRChat.Posters.Twitter.Shared.Models
{
    public class FFMpegParameters
    {
        [JsonProperty("ffmpegPath")]
        public string FFMpegPath { set; get; }

        [JsonProperty("timeSeconds")]
        public int TimeSeconds { set; get; }

        [JsonProperty("codec")]
        public string Codec { set; get; }

        [JsonProperty("outputFormat")]
        public string OutputFormat { set; get; }

        [JsonProperty("Framerate")]
        public int Framerate { set; get; }

        [JsonProperty("pixelFormat")]
        public string PixelFormat { set; get; }

        [JsonProperty("crf")]
        public int Crf { set; get; }

        [JsonProperty("loop")]
        public string Loop { set; get; }
    }
}