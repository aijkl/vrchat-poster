using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Aijkl.VRChat.Posters.Twitter.Shared.Models
{
    public class PromotionTweet
    {
        [JsonProperty("tweetId")]
        public long TweetId { set; get; }

        [JsonProperty("drawIndex")]
        public int DrawIndex { set; get; }

        [JsonProperty("targetPosterIds")]
        public IList<string> TargetPosterIds { set; get; }

        [JsonProperty("deadline")]
        public DateTime? Deadline { set; get; }
    }
}
