using SkiaSharp;

namespace Aijkl.VRChat.Posters.Shared.Paint
{
    public interface IPaintComponent
    {
        void Dispose();
        SKBitmap Result { get; }
        SKPoint Point { set; get; }
    }
}
