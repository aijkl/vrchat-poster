using SkiaSharp;
using System;
using Topten.RichTextKit;

namespace Aijkl.VRChat.Posters.Shared.Paint.Components
{
    public class TextBox : IPaintComponent, IDisposable
    {
        private SKPoint _point;
        private SKBitmap _bitmap;
        public TextBox(string text, int fontSizeMin, int fontSizeMax, string fontFamily, int width, int heigthMax, SKPoint point, SKColor? backgroundColor = null, SKColor? fontColor = null)
        {
            _point = point;
            SKBitmap tempBitmap = new SKBitmap(width, heigthMax);
            SKCanvas tempCanvas = new SKCanvas(tempBitmap);
            int fontSize = fontSizeMax;

            while (true)
            {
                tempCanvas.Clear();

                RichString richString = new RichString();                
                richString.Bold();                
                richString.FontSize(fontSize);
                richString.FontFamily(fontFamily);
                richString.Alignment(TextAlignment.Left);
                richString.TextColor(fontColor ?? SKColors.Black);
                richString.MaxWidth = width;
                richString.MaxHeight = heigthMax;

                richString.Add(text);

                richString.Paint(tempCanvas);
                fontSize--;
                if (fontSize < fontSizeMin)
                {
                    throw new Exception();
                }
                if (fontSize <= fontSizeMin || !richString.Truncated)
                {
                    tempBitmap.Dispose();

                    SKBitmap skBitmap = new SKBitmap(width, (int)Math.Round(richString.MeasuredHeight));
                    SKCanvas skCanvas = new SKCanvas(skBitmap);
                    skCanvas.DrawRect(skCanvas.LocalClipBounds, new SKPaint() { Color = backgroundColor ?? SKColors.White });
                    richString.Paint(skCanvas);
                    //MEMO
                    //なぜか上に1.2の余白が出来る
                    //MeasuredHeightの値がおかしい

                    _bitmap = skBitmap;
                    break;
                }
            }
        }       
        public SKBitmap Result { get { return _bitmap; } }
        public SKPoint Point { set { _point = value; } get { return _point; } }
        public void Dispose()
        {
            _bitmap?.Dispose();
            _bitmap = null;
        }                
    }
}
