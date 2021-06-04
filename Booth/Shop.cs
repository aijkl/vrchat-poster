using System.Drawing;

namespace Aijkl.VRChat.Posters.Booth
{
    public class Shop
    {        
        public Shop(string iconUrl,string Name)
        {
            IconUrl = iconUrl;
            this.Name = Name;
        }
        public string IconUrl { set; get; }
        public string Name { set; get; }        
    }
}
