using System.Drawing;
using Aijkl.VRChat.Posters.Shared;
using Aijkl.VRChat.Posters.Shared.Expansion;

namespace Aijkl.VRChat.Posters.Booth.Expansion
{
    public static class StringExpansion
    {
        public static string ChangeLengthText(this string text, int width, Font font)
        {
            string temp = text;
            using (Bitmap bitmap = new Bitmap(100, 100))
            using (Graphics graphics = PosterHelper.CreateGraphicsObject(bitmap))
            {
                if ((graphics.MeasureString(text,font).Width < width))
                {
                    return text;
                }

                for (int i = 0; text.Length > i; i++)
                {
                    temp = temp.Remove(temp.Length - 1, 1);
                    if (graphics.MeasureString(temp, font).Width < width)
                    {
                        break;
                    }
                }
            }
            return temp;
        }
    }
}
