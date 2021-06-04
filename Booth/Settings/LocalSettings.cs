using Aijkl.VRChat.Posters.Shared.Models;
using Newtonsoft.Json;
using System.IO;

namespace Aijkl.VRChat.Posters.Booth
{
    public class LocalSettings
    {
        [JsonProperty("cloudSettingsId")]
        public string CloudSettingsId { set; get; }

        [JsonProperty("saveDirectory")]
        public string SaveDirectory { set; get; }

        [JsonProperty("tempDirectory")]
        public string TempDirectory { set; get; }

        [JsonProperty("tempFileName")]
        public string TempFileName { set; get; }

        [JsonProperty("cacheDirectory")]
        public string CacheDirectory { set; get; }

        [JsonProperty("cloudFlareParameters")]
        public CloudFlareParameters CloudFlareParameters { set; get; }
        public static LocalSettings Load(string path)
        {
            return JsonConvert.DeserializeObject<LocalSettings>(File.ReadAllText(path));
        }
    }
}
