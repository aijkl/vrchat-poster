using Newtonsoft.Json;

namespace Aijkl.VRChat.Posters.Twitter.Shared.Models
{
    public class GoogleDriveSupport
    {
        [JsonProperty("needUpload")]
        public bool NeedUpload { set; get; }

        [JsonProperty("fileId")]
        public string FileId { set; get; }
    }
}
