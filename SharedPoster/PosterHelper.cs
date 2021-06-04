using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Translate.v2;
using System.Drawing;
using System.Drawing.Text;

namespace Aijkl.VRChat.Posters.Shared
{
    public static class PosterHelper
    {
        public static DriveService CreateDriveService(string json)
        {
            var credential = GoogleCredential.FromJson(json).CreateScoped(DriveService.Scope.Drive).UnderlyingCredential as ServiceAccountCredential;
            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential
            });
        }
        public static TranslateService CreateTranslateService(string json)
        {
            var credential = GoogleCredential.FromJson(json).CreateScoped(TranslateService.Scope.CloudTranslation).UnderlyingCredential as ServiceAccountCredential;
            return new TranslateService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential
            });
        }        
        public static Graphics CreateGraphicsObject(Bitmap bitmap)
        {
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            return graphics;
        }
    }
}
