using System.Collections.Generic;
using System.IO;
using System.Text;
using Aijkl.VRChat.Posters.Shared.Expansion;
using Aijkl.VRChat.Posters.Shared.Twitter.Models;
using Google.Apis.Drive.v3;
using Newtonsoft.Json;

namespace Aijkl.VRChat.Posters.Shared.Twitter
{
    public class CloudSettings
    {
        [JsonIgnore]
        public string FileId { private set; get; }

        [JsonProperty("posters")]
        public List<PosterParameters> Posters { get; set; }

        [JsonProperty("common")]
        public Common Common { set; get; }

        [JsonProperty("mute")]
        public Mute Mute { get; set; }        

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
