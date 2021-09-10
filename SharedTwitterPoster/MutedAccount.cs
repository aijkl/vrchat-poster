using System;
using Newtonsoft.Json;

namespace Aijkl.VRChat.Posters.Twitter.Shared.Models
{
    public class MutedAccount
    {
        public MutedAccount(long accountId, DateTime? deadline)
        {
            AccountId = accountId;
            Deadline = deadline;
        }

        [JsonProperty("accountId")]
        public long AccountId { set; get; }

        [JsonProperty("deadline")]
        public DateTime? Deadline { set; get; }
    }
}
