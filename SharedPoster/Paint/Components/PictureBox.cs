using SkiaSharp;
using System;

namespace Aijkl.VRChat.Posters.Shared.Paint.Components
{
    public class PictureBox : IPaintComponent , IDisposable
    {
        private SKPoint _point;
        private SKBitmap _bitmap;
        public PictureBox(SKBitmap bitmap, SKSize size, SKPoint point)
        {
            _point = point;
            if(bitmap.Width > size.Width || bitmap.Height > size.Height)
            {
                for (double i = 1.0; i < 10.0; i += 0.05)
                {
                    SKSize resize = new SKSize((int)(bitmap.Width / i), (int)(bitmap.Height / i));
                    if (resize.Height <= size.Height)
                    {
                        _bitmap = bitmap.Resize(new SKImageInfo((int)resize.Width, (int)resize.Height), SKFilterQuality.High);
                        break;
                    }
                }
            }
            else
            {
                _bitmap = bitmap;
            }
        }
        public SKPoint Point { set { _point = value; } get {return _point; } }
        public SKBitmap Result { get { return _bitmap; } }
        public void Dispose()
        {
            _bitmap?.Dispose();
            _bitmap = null;
        }        
    }
}
