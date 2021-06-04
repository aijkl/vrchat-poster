namespace Aijkl.VRChat.Posters.Shared.Paint
{
    public class Margin
    {
        public Margin(int top, int right, int bottom, int left)
        {
            Top = top;
            Right = right;
            Bottom = bottom;
            Left = left;
        }

        public int Top { private set; get; }        
        public int Right { private set; get; }        
        public int Bottom { private set; get; }        
        public int Left { private set; get; }        
    }
}
