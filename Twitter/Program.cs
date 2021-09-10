using System.IO;
using System.Text;
using System.Threading;
using Aijkl.VRChat.Posters.Shared.Twitter;

namespace Aijkl.VRChat.Posters.Twitter
{
    class Program
    {
        static void Main()
        {
            string localSettingsPath = $"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}Resources{Path.DirectorySeparatorChar}localSettings.json";

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#if DEBUG
            LocalSettings localSettings = LocalSettings.Load(localSettingsPath, debug: true);
#endif
#if !DEBUG
            LocalSettings localSettings = LocalSettings.Load(localSettingsPath, debug: false);
#endif 

            CancellationTokenSource cancellationToken = new CancellationTokenSource();
            using TwitterPosterGenerator twitterPoster = new TwitterPosterGenerator(localSettings);
            twitterPoster.BeginLoop(cancellationToken.Token);

            new AutoResetEvent(false).WaitOne();
        }
    }
}
