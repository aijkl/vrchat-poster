using CoreTweet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aijkl.VRChat.Posters.Shared.Twitter.Models
{
    public class Mute
    {
        [JsonProperty("tweets")]
        public List<Tweet> Tweets { get; set; }

        [JsonProperty("accounts")]
        public List<Account> Accounts { get; set; }
        
        public bool IsMuted(Status status)
        {                        
            return Tweets.Any(x => x.Id.Equals(status.Id) && (string.IsNullOrEmpty(x.DueDate) || DateTime.Parse(x.DueDate) >= DateTime.Now)) || Accounts.Any(x => x.Id.Equals(status.User.Id) && (string.IsNullOrEmpty(x.DueDate) || DateTime.Parse(x.DueDate) >= DateTime.Now));
        }
    }
}
