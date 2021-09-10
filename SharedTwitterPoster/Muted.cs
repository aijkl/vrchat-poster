using CoreTweet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Aijkl.VRChat.Posters.Twitter.Shared.Models;

namespace Aijkl.VRChat.Posters.Shared.Twitter.Models
{
    public class Muted
    {
        [JsonProperty("tweets")]
        public IList<MutedTweet> Tweets { get; set; }

        [JsonProperty("accounts")]
        public IList<MutedAccount> Accounts { get; set; }
        
        public bool IsMuted(Status status)
        {
            return Tweets.Any(x => x.TweetId.Equals(status.Id) && (x.Deadline == null || x.Deadline >= DateTime.Now)) || Accounts.Any(x => x.AccountId.Equals(status.User.Id) && (x.Deadline == null || x.Deadline >= DateTime.Now));
        }
    }
}
