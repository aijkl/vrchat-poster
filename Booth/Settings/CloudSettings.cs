using Aijkl.VRChat.Booth.Models;
using Aijkl.VRChat.Posters.Models;
using Aijkl.VRChat.Posters.Shared.Expansion;
using Google.Apis.Drive.v3;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Aijkl.VRChat.Posters.Booth
{ 
    public class CloudSettings
    {
        [JsonProperty("common")]
        public Common Common { set; get; }

        [JsonProperty("posters")]
        public List<PosterParameters> Posters { set; get; }

        public static CloudSettings Fetch(DriveService driveService, string fileId)
        {
            return JsonConvert.DeserializeObject<CloudSettings>(driveService.DownLoadAsStringAsync(fileId).Result);
        }
    }
}
