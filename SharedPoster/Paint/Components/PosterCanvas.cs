using SkiaSharp;
using System;
using System.Collections.Generic;

namespace Aijkl.VRChat.Posters.Shared.Paint.Components
{
    public class PosterCanvas : IPaintComponent , IDisposable
    {
        private SKBitmap _bitmap;        
        private SKPoint _point;
        public PosterCanvas(SKSize size, SKColor? color = null, SKPoint? point = null)
        {            
            _bitmap = new SKBitmap((int)size.Width, (int)size.Height);
            _point = point ?? new SKPoint();
            _point = new SKPoint();
            Size = size;
            Components = new List<IPaintComponent>();
            Color = color ?? SKColors.White;
        }
        public List<IPaintComponent> Components { private set; get; }
        public SKColor Color { set; get; }
        public SKSize Size { set; get; }
        public SKBitmap Result { get { return _bitmap; } }
        public SKPoint Point { set { _point = value; } get { return _point; } }
        public void Dispose()
        {
            foreach (var component in Components)
            {
                component?.Dispose();
            }
            Components = null;
            _bitmap?.Dispose();
            _bitmap = null;
        }        
        public void Draw()
        {
            SKCanvas skCanvas = new SKCanvas(_bitmap);
            skCanvas.DrawRect(skCanvas.LocalClipBounds, new SKPaint() { Color = Color });
            foreach (var component in Components)
            {
                skCanvas.DrawImage(SKImage.FromBitmap(component.Result), component.Point);
            }
        }
    }
}
