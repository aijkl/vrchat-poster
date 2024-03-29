﻿using Aijkl.VRChat.Posters.Shared.Paint;
using SkiaSharp;
using System;
using Topten.RichTextKit;

namespace Aijkl.VRChat.Posters.Twitter.Paint.Components
{
    public class SDK2PosterBox : IDisposable
    {
        private readonly Margin margin;
        private readonly SKCanvas skCanvas;
        private SKPoint drawnPosition;
        public SDK2PosterBox(string title, string fontName, int titleFontSize, SKColor color)
        {
            margin = new Margin(8, 30, 8, 30);
            drawnPosition = new SKPoint(margin.Left, 126);

            Result = new SKBitmap(1100,1600);
            skCanvas = new SKCanvas(Result);
            skCanvas.DrawRect(skCanvas.LocalClipBounds, new SKPaint() { Color = color });

            RichString richString = new RichString();
            richString.FontFamily(fontName);
            richString.FontSize(titleFontSize);
            richString.Bold();
            richString.Add(title);
            richString.Paint(skCanvas, new SKPoint(31, 31));            
        }
        public SKBitmap Result { private set; get; }
        public bool TryDrawTweet(SKImage tweet)
        {
            if (drawnPosition.Y + tweet.Height + margin.Top + margin.Bottom > Result.Height)
            {
                return false;
            }

            skCanvas.DrawImage(tweet, margin.Left, drawnPosition.Y + margin.Top);
            drawnPosition.Y += tweet.Height + margin.Top + margin.Bottom;
            return true;
        }
        public void Dispose()
        {
            Result?.Dispose();
            Result = null;
        }
    }
}
