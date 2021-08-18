using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Aijkl.VRChat.Posters.Twitter.Shared.Models
{
    public class Promotion
    {
        [JsonIgnore] private IList<PromotionTweet> _tweets;

        [JsonProperty("tweets")]
        public IList<PromotionTweet> Tweets
        {
            set
            {
                _tweets = value?.Where(x => x.Deadline == null || x.Deadline >= DateTime.Now).ToList();
            }
            get => _tweets;
        }
    }
}
