using SkiaSharp;

namespace Aijkl.VRChat.Posters.Twitter.Paint.Components
{
    public class SDK3PosterBox
    {
        private readonly SKCanvas skCanvas;
        private SKPoint drawnPosition;
        public SDK3PosterBox()
        {
            Result = new SKBitmap(4400, 1600);
            skCanvas = new SKCanvas(Result);
        }
        public SKBitmap Result { private set; get; }
        public bool TryDrawTweet(SKImage poster)
        {
            if (drawnPosition.Y + poster.Height > Result.Height) return false;

            if (drawnPosition.X + poster.Width > Result.Width)
            {
                drawnPosition.Y += poster.Height;
                drawnPosition.X = 0;
            }
            
            skCanvas.DrawImage(poster, drawnPosition);
            drawnPosition.X += poster.Width;

            return true;
        }
        public void Dispose()
        {
            Result?.Dispose();
            Result = null;
        }
    }
}
