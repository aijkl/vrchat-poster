﻿using Aijkl.VRChat.Posters.Shared.Models;
using Aijkl.VRChat.Posters.Shared.Twitter.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Aijkl.VRChat.Posters.Shared.Twitter
{
    public class LocalSettings
    {        
        [JsonProperty("cloudSettingsId")]
        public string CloudSettingsId { set; get; }

        [JsonProperty("cacheDirectory")]
        public string CacheDirectory { set; get; }

        [JsonProperty("tempDirectory")]
        public string TempDirectory { set; get; }

        [JsonProperty("tempFileName")]
        public string TempFileName { set; get; }

        [JsonProperty("saveDirectory")]
        public string SaveDirectory { set; get; }

        [JsonProperty("twitterParameters")]
        public TwitterParameters TwitterParameters { set; get; }

        [JsonProperty("cloudFlareParameters")]
        public CloudFlareParameters CloudFlareParameters { set; get; }        

        public static LocalSettings Load(string filePath, bool debug)
        {
            JObject jObject = JObject.Parse(File.ReadAllText(filePath));            
            return jObject[(debug ? "debug" : "release")].ToObject<LocalSettings>();
        }
    }
}
