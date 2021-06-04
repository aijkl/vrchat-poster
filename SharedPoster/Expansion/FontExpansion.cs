using System.Drawing;

namespace Aijkl.VRChat.SharedPoster.Expansion
{
    public static class FontExpansion
    {
        public static Font ReSize(this Font font, float size)
        {
            return new Font(font.FontFamily, size, font.Style);
        }
    }
}
