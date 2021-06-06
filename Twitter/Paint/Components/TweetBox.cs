using Aijkl.VRChat.Posters.Shared.Paint;
using Aijkl.VRChat.Posters.Shared.Paint.Components;
using SkiaSharp;
using System;

namespace Aijkl.VRChat.Posters.Twitter.Paint.Components
{
    public class TweetBox : IPaintComponent , IDisposable
    {
        private readonly string userName;
        private readonly string text;
        private readonly string fontFamily;
        private readonly SKBitmap userIcon;
        private readonly SKBitmap image;
        private readonly SKColor backgroundColor;

        public TweetBox(string userName, string text, string fontFamily, SKColor backgroundColor, SKPoint point, SKBitmap userIcon, SKBitmap image = null)
        {
            this.userName = string.IsNullOrEmpty(userName) ? "UserName" : userName;
            this.text = string.IsNullOrEmpty(text) ? "Text" : text;
            this.fontFamily = fontFamily;
            this.userIcon = userIcon;
            this.image = image;
            this.backgroundColor = backgroundColor;
            Point = point;           
        }                
        public SKBitmap Result { get; private set; }
        public SKPoint Point { set; get; }
        public void Dispose()
        {
            Result?.Dispose();
            Result = null;
        }
        public void Draw()
        {
            SKSize size = new SKSize(1040, 0);

            PictureBox imageBox = null;
            PictureBox userIconBox;
            TextBox textBox;
            TextBox userNameBox;
            if (image != null)
            {
                Margin margin = new Margin(8, 8, 8, 8);
                size.Height = image.Height;
                imageBox = new PictureBox(image, new SKSize(400, 230), new SKPoint());
                userIconBox = new PictureBox(userIcon, new SKSize(48, 48), new SKPoint(imageBox.Result.Width + margin.Left, 0));
                textBox = new TextBox(text, 7, 15, fontFamily, (int)size.Width - (imageBox.Result.Width + margin.Left + margin.Right), imageBox.Result.Height - (userIconBox.Result.Height + margin.Top + margin.Bottom), new SKPoint(imageBox.Result.Width + margin.Left, userIconBox.Result.Height + margin.Top), backgroundColor);
                userNameBox = new TextBox(userName, 3, 18, fontFamily, (int)size.Width - (imageBox.Result.Width + userIcon.Width + margin.Left + margin.Left + margin.Right), 40, new SKPoint(imageBox.Result.Width + userIconBox.Result.Width + margin.Left + margin.Left, 11), backgroundColor);                
                size.Height = imageBox.Result.Height;
            }
            else
            {
                Margin textBoxMargin = new Margin(7, 8, 8, 4);
                Margin margin = new Margin(8, 8, 8, 8);

                userIconBox = new PictureBox(userIcon, new SKSize(48, 48), new SKPoint());
                textBox = new TextBox(text, 7, 15, fontFamily, (int)size.Width - (textBoxMargin.Left + textBoxMargin.Right), 300, new SKPoint(textBoxMargin.Left, userIconBox.Result.Height + textBoxMargin.Top), backgroundColor);
                userNameBox = new TextBox(userName, 3, 18, fontFamily, (int)size.Width - (userIcon.Width + margin.Left + margin.Right), 40, new SKPoint(userIconBox.Result.Width + margin.Left, 0), backgroundColor);
                userNameBox.Point = new SKPoint(userNameBox.Point.X, 11);
                size.Height = userIcon.Height + textBox.Result.Height + margin.Top + margin.Bottom;
            }

            PosterCanvas posterCanvas = new PosterCanvas(size, backgroundColor);
            if(imageBox != null) posterCanvas.Components.Add(imageBox);
            posterCanvas.Components.Add(userIconBox);
            posterCanvas.Components.Add(textBox);
            posterCanvas.Components.Add(userNameBox);
            posterCanvas.Draw();
            Result = posterCanvas.Result;
        }
    }
}
