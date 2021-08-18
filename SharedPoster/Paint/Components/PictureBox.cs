using SkiaSharp;
using System;

namespace Aijkl.VRChat.Posters.Shared.Paint.Components
{
    public class PictureBox : IPaintComponent , IDisposable
    {
        public PictureBox(SKBitmap bitmap, SKSize size, SKPoint point)
        {
            Point = point;
            if(bitmap.Width > size.Width || bitmap.Height > size.Height)
            {
                for (double i = 1.0; i < 10.0; i += 0.05)
                {
                    SKSize resize = new SKSize((int)(bitmap.Width / i), (int)(bitmap.Height / i));
                    if (resize.Height <= size.Height)
                    {
                        Result = bitmap.Resize(new SKImageInfo((int)resize.Width, (int)resize.Height), SKFilterQuality.High);
                        break;
                    }
                }
            }
            else
            {
                Result = bitmap;
            }
        }
        public SKPoint Point { set; get; }
        public SKBitmap Result { get; private set; }
        public void Dispose()
        {
            Result?.Dispose();
            Result = null;
        }        
    }
}
