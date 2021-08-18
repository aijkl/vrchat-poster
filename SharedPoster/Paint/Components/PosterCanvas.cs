using SkiaSharp;
using System;
using System.Collections.Generic;

namespace Aijkl.VRChat.Posters.Shared.Paint.Components
{
    public class PosterCanvas : IPaintComponent , IDisposable
    {
        public PosterCanvas(SKSize size, SKColor? color = null, SKPoint? point = null)
        {            
            Result = new SKBitmap((int)size.Width, (int)size.Height);
            Point = point ?? new SKPoint();
            Point = new SKPoint();
            Size = size;
            Components = new List<IPaintComponent>();
            Color = color ?? SKColors.White;
        }
        public List<IPaintComponent> Components { private set; get; }
        public SKColor Color { set; get; }
        public SKSize Size { set; get; }
        public SKBitmap Result { get; private set; }
        public SKPoint Point { set; get; }
        public void Dispose()
        {
            foreach (var component in Components)
            {
                component?.Dispose();
            }
            Components = null;
            Result?.Dispose();
            Result = null;
        }        
        public void Draw()
        {
            SKCanvas skCanvas = new SKCanvas(Result);
            skCanvas.DrawRect(skCanvas.LocalClipBounds, new SKPaint() { Color = Color });
            foreach (var component in Components)
            {
                skCanvas.DrawImage(SKImage.FromBitmap(component.Result), component.Point);
            }
        }
    }
}
