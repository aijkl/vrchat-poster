using System;
using Newtonsoft.Json;

namespace Aijkl.VRChat.Posters.Shared.Twitter.Models
{
    public class MutedTweet
    {
        public MutedTweet(long tweetId, DateTime? deadline)
        {
            TweetId = tweetId;
            Deadline = deadline;
        }

        [JsonProperty("tweetId")]
        public long TweetId { set; get; }

        [JsonProperty("deadline")]
        public DateTime? Deadline { set; get; }
    }
}