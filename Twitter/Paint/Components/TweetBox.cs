using Aijkl.VRChat.Posters.Shared.Paint;
using Aijkl.VRChat.Posters.Shared.Paint.Components;
using SkiaSharp;
using System;

namespace Aijkl.VRChat.Posters.Twitter.Paint.Components
{
    public class TweetBox : IPaintComponent , IDisposable
    {
        private readonly string _userName;
        private readonly string _text;
        private readonly string _fontFamily;
        private readonly SKBitmap _userIcon;
        private readonly SKBitmap _image;
        private readonly SKColor _backgroundColor;

        private SKPoint _point;
        private SKBitmap _bitmap;

        public TweetBox(string userName, string text, string fontFamily, SKColor backgroundColor, SKPoint point, SKBitmap userIcon, SKBitmap image = null)
        {
            _userName = string.IsNullOrEmpty(userName) ? "UserName" : userName;
            _text = string.IsNullOrEmpty(text) ? "Text" : text;
            _fontFamily = fontFamily;
            _userIcon = userIcon;
            _image = image;
            _backgroundColor = backgroundColor;
            _point = point;           
        }                
        public SKBitmap Result { get { return _bitmap; } }
        public SKPoint Point { set { _point = value; } get { return _point; } }
        public void Dispose()
        {
            _bitmap?.Dispose();
            _bitmap = null;
        }
        public void Draw()
        {
            SKSize size = new SKSize(1040, 0);

            PictureBox imageBox = null;
            PictureBox userIconBox;
            TextBox textBox;
            TextBox userNameBox;
            if (_image != null)
            {
                Margin margin = new Margin(8, 8, 8, 8);
                size.Height = _image.Height;
                imageBox = new PictureBox(_image, new SKSize(400, 230), new SKPoint());
                userIconBox = new PictureBox(_userIcon, new SKSize(48, 48), new SKPoint(imageBox.Result.Width + margin.Left, 0));
                textBox = new TextBox(_text, 7, 15, _fontFamily, (int)size.Width - (imageBox.Result.Width + margin.Left + margin.Right), imageBox.Result.Height - (userIconBox.Result.Height + margin.Top + margin.Bottom), new SKPoint(imageBox.Result.Width + margin.Left, userIconBox.Result.Height + margin.Top), _backgroundColor);
                userNameBox = new TextBox(_userName, 3, 18, _fontFamily, (int)size.Width - (imageBox.Result.Width + _userIcon.Width + margin.Left + margin.Left + margin.Right), 40, new SKPoint(imageBox.Result.Width + userIconBox.Result.Width + margin.Left + margin.Left, 11), _backgroundColor);                
                size.Height = imageBox.Result.Height;
            }
            else
            {
                Margin textBoxMargin = new Margin(7, 8, 8, 4);
                Margin margin = new Margin(8, 8, 8, 8);

                userIconBox = new PictureBox(_userIcon, new SKSize(48, 48), new SKPoint());
                textBox = new TextBox(_text, 7, 15, _fontFamily, (int)size.Width - (textBoxMargin.Left + textBoxMargin.Right), 300, new SKPoint(textBoxMargin.Left, userIconBox.Result.Height + textBoxMargin.Top), _backgroundColor);
                userNameBox = new TextBox(_userName, 3, 18, _fontFamily, (int)size.Width - (_userIcon.Width + margin.Left + margin.Right), 40, new SKPoint(userIconBox.Result.Width + margin.Left, 0), _backgroundColor);
                userNameBox.Point = new SKPoint(userNameBox.Point.X, 11);
                size.Height = _userIcon.Height + textBox.Result.Height + margin.Top + margin.Bottom;
            }

            PosterCanvas posterCanvas = new PosterCanvas(size, _backgroundColor);
            if(imageBox != null) posterCanvas.Components.Add(imageBox);
            posterCanvas.Components.Add(userIconBox);
            posterCanvas.Components.Add(textBox);
            posterCanvas.Components.Add(userNameBox);
            posterCanvas.Draw();
            _bitmap = posterCanvas.Result;
        }
    }
}
