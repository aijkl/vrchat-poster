using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aijkl.VRChat.Posters.Shared.Paint
{
    public interface IPaintComponent
    {
        void Dispose();
        SKBitmap Result { get; }
        SKPoint Point { set; get; }
    }
}
