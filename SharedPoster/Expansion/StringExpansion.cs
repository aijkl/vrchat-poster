using System.Drawing;
using System.Drawing.Text;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Aijkl.VRChat.Posters.Shared.Expansion
{
    public static class StringExpansion
    {
        public static SizeF MeasureString(this string text, Font font, bool includeGlyphs = false)
        {            
            if (includeGlyphs)
            {
                using Graphics includeGlyphsGraphics = Graphics.FromImage(new Bitmap(1, 1));
                return includeGlyphsGraphics.MeasureString(text, font);
            }

            using Graphics graphics = PosterHelper.CreateGraphicsObject(new Bitmap(1, 1));
            using StringFormat sf = new StringFormat(StringFormat.GenericTypographic);
            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            return graphics.MeasureString(text, font, int.MaxValue, sf);
        }
        public static string TrimText(this string str, string encoderName)
        {                                    
            str = HttpUtility.HtmlDecode(str);
            Encoding encoder = Encoding.GetEncoding(encoderName, new EncoderReplacementFallback(string.Empty), new DecoderReplacementFallback(string.Empty));
            str = encoder.GetString(encoder.GetBytes(str));
            str = str.Trim();
            return str;
        }     
    }
}
