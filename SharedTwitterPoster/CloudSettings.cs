using System.Collections.Generic;
using System.IO;
using System.Text;
using Aijkl.VRChat.Posters.Shared.Expansion;
using Aijkl.VRChat.Posters.Shared.Twitter.Models;
using Aijkl.VRChat.Posters.Twitter.Shared.Models;
using Google.Apis.Drive.v3;
using Newtonsoft.Json;

namespace Aijkl.VRChat.Posters.Shared.Twitter
{
    public class CloudSettings
    {
        [JsonIgnore]
        public string FileId { private set; get; }

        [JsonProperty("sdk2Posters")]
        public List<SDK2PosterParameters> SDK2Posters { get; set; }

        [JsonProperty("sdk3Posters")]
        public List<SDK3PosterParameters> SDK3Posters { set; get; }

        [JsonProperty("common")]
        public Common Common { set; get; }

        [JsonProperty("muted")]
        public Muted Muted { get; set; }        

        [JsonProperty("promotion")]
        public Promotion Promotion { set; get; }

        public static CloudSettings Fetch(DriveService driveService, string fileId)
        {
            using Stream stream = driveService.DownLoadAsSteamAsync(fileId).Result;
            stream.Position = 0;
            using StreamReader streamReader = new StreamReader(stream);
            using JsonReader jsonReader = new JsonTextReader(streamReader);
            JsonSerializer jsonSerializer = new JsonSerializer();
            CloudSettings cloudSettings = jsonSerializer.Deserialize<CloudSettings>(jsonReader);
            cloudSettings.FileId = fileId;
            return cloudSettings;            
        }
        public void Push(DriveService driveService)
        {
            MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this, Formatting.Indented)));
            driveService.UpdateStreamAsync(memoryStream, FileId).Wait();
        }
    }
}
