using System.IO;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Upload;
using Google.Apis.Drive.v3;
using File = Google.Apis.Drive.v3.Data.File;

namespace Aijkl.VRChat.Posters.Shared.Expansion
{
    public static class DriveServiceExpansion
    {
        public static async Task<IUploadProgress> UpdateStreamAsync(this DriveService driveService, Stream stream, string fileId, string contentType = "")
        {
            using (stream)
            {
                File file = new File();
                var request = driveService.Files.Update(file, fileId, stream, contentType);
                IUploadProgress uploadProgress = await request.UploadAsync().ConfigureAwait(false);

                if (uploadProgress.Status != UploadStatus.Completed)
                {
                    throw uploadProgress.Exception;
                }
                return uploadProgress;
            }
        }
        public async static Task<MemoryStream> DownLoadAsSteamAsync(this DriveService driveService, string fileId)
        {
            var request = driveService.Files.Get(fileId);
            MemoryStream memoryStream = new MemoryStream();
            await request.DownloadAsync(memoryStream).ConfigureAwait(false);
            return memoryStream;
        }
        public async static Task<string> DownLoadAsStringAsync(this DriveService driveService, string fileId)
        {
            var request = driveService.Files.Get(fileId);
            MemoryStream memoryStream = new MemoryStream();
            await request.DownloadAsync(memoryStream).ConfigureAwait(false);
            return Encoding.UTF8.GetString(memoryStream.ToArray());
        }       
    }
}
