using SkiaSharp;
using System.Drawing;

namespace Aijkl.VRChat.SharedPoster.Expansion
{
    public static class ByteArryExpansion
    {
        public static SKColor ToColor(this byte[] rgb)
        {
            return new SKColor(rgb[0], rgb[1], rgb[2]);
        }
    }
}
