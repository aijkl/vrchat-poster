using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Aijkl.CloudFlare.API.Models
{    
    public class Response
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("errors")]
        public List<object> Errors { get; set; }

        [JsonProperty("messages")]
        public List<object> Messages { get; set; }

        [JsonProperty("result")]
        public Dictionary<string,string> Result { get; set; }
    }
}
