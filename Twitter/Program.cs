using System.IO;
using System.Text;
using System.Threading;
using Aijkl.VRChat.Posters.Shared;
using Aijkl.VRChat.Posters.Shared.Twitter;
using Google.Apis.Drive.v3;
using Google.Apis.Translate.v2;

namespace Aijkl.VRChat.Posters.Twitter
{
    class Program
    {
        static void Main()
        {
            string settingsDirectory = $"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}Assets";            
            string authPath = $"{settingsDirectory}{Path.DirectorySeparatorChar}authToken.json";
            string localSettingsPath = $"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}Assets{Path.DirectorySeparatorChar}localSettings.json";

            string authJson = File.ReadAllText(authPath);
            DriveService driveService = PosterHelper.CreateDriveService(authJson);
            TranslateService translateService = PosterHelper.CreateTranslateService(authJson);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#if DEBUG
            LocalSettings localSettings = LocalSettings.Load(localSettingsPath, debug: true);
#endif
#if !DEBUG
            LocalSettings localSettings = LocalSettings.Load(localSettingsPath, debug: false);
#endif 

            CancellationTokenSource cancellationToken = new CancellationTokenSource();
            using TwitterPosterGenerator twitterPoster = new TwitterPosterGenerator(localSettings, driveService, translateService);
            twitterPoster.BeginLoop(cancellationToken.Token);

            new AutoResetEvent(false).WaitOne();
        }
    }
}
