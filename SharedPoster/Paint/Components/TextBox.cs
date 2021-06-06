using SkiaSharp;
using System;
using Topten.RichTextKit;

namespace Aijkl.VRChat.Posters.Shared.Paint.Components
{
    public class TextBox : IPaintComponent, IDisposable
    {
        public TextBox(string text, int fontSizeMin, int fontSizeMax, string fontFamily, int width, int heightMax, SKPoint point, SKColor? backgroundColor = null, SKColor? fontColor = null)
        {
            Point = point;
            SKBitmap tempBitmap = new SKBitmap(width, heightMax);
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
                richString.MaxHeight = heightMax;

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

                    // MEMO
                    // なぜか上に1.2の余白が出来る
                    // MeasuredHeightの値がおかしい
                    SKBitmap skBitmap = new SKBitmap(width, (int)Math.Round(richString.MeasuredHeight));
                    SKCanvas skCanvas = new SKCanvas(skBitmap);
                    skCanvas.DrawRect(skCanvas.LocalClipBounds, new SKPaint() { Color = backgroundColor ?? SKColors.White });
                    richString.Paint(skCanvas);

                    Result = skBitmap;
                    break;
                }
            }
        }

        public SKBitmap Result { get; private set; }

        public SKPoint Point { get; set; }

        public void Dispose()
        {
            Result?.Dispose();
            Result = null;
        }
    }
}
