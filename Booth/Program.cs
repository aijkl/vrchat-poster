using Aijkl.VRChat.Posters.Booth;
using Aijkl.VRChat.Posters.Shared;
using System;
using System.IO;
using System.Text;
using System.Threading;

namespace Aijkl.VRChat.Booth
{
    class Program
    {        
        static void Main()
        {            
            string settingsFilePath = $"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}Assets{Path.DirectorySeparatorChar}localSettings.json";
            string authFilePath = $"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}Assets{Path.DirectorySeparatorChar}authToken.json";
            string logoPath = $"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}Assets{Path.DirectorySeparatorChar}logo.png";
            LocalSettings localSettings = LocalSettings.Load(settingsFilePath);

            BoothPosterGenerater boothPoster = new BoothPosterGenerater(PosterHelper.CreateDriveService(File.ReadAllText(authFilePath)), localSettings, logoPath);
            boothPoster.BeginLoop(new CancellationTokenSource().Token);
            new AutoResetEvent(false).WaitOne();
        }        
    }
}
