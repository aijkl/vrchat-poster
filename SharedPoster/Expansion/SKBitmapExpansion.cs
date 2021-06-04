using SkiaSharp;
using System;
using System.IO;
using System.Linq;

namespace Aijkl.VRChat.SharedPoster.Expansion
{
    public static class SKBitmapExpansion
    {
        public static void Save(this SKBitmap skBitmap, string path,int quality = 100)
        {
            SKEncodedImageFormat format = SKEncodedImageFormat.Jpeg;
            if (Enum.GetNames(typeof(SKEncodedImageFormat)).ToList().Any(x => x.ToLower() == Path.GetExtension(path).Remove(0, 1)))
            {
                format = (SKEncodedImageFormat)Enum.Parse(typeof(SKEncodedImageFormat), Enum.GetNames(typeof(SKEncodedImageFormat)).ToList().Where(x => x.ToLower() == Path.GetExtension(path).Remove(0, 1).ToLower()).FirstOrDefault());
            }                        
            using (SKData skData = skBitmap.Encode(format, quality))
            using (var stream = File.OpenWrite(path))
            {
                skData.SaveTo(stream);
            }
        }
    }
}
