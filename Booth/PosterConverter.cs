using System.Drawing;
using System.IO;
using Aijkl.VRChat.Posters.Shared;
using Aijkl.VRChat.Posters.Shared.Expansion;
using Aijkl.VRChat.Posters.Booth.Scraping;
using Aijkl.VRChat.Posters.Booth.Expansion;

namespace Aijkl.VRChat.Posters
{
    public class PosterConverter
    {        
        private readonly LocalCache localCache;
        private readonly BoothClient boothClient;
        public PosterConverter(LocalCache localCache, BoothClient boothClient)
        {
            this.localCache = localCache;
            this.boothClient = boothClient;
        }
        public DrawParameters Convert(Product product, string fontName)
        {
            DrawParameters drawParameters = new DrawParameters();
            
            Size thumbnailSize = new Size(230, 230);
            Size shopIconSize = new Size(36, 38);

            DownloadAsCache(product.ThumbnailUrl, thumbnailSize);
            DownloadAsCache(product.Shop.IconUrl, shopIconSize);
            drawParameters.ThumbnailFileName = Path.GetFileName(product.ThumbnailUrl);
            drawParameters.ShopIconFileName = Path.GetFileName(product.Shop.IconUrl);            

            drawParameters.Like = $"{System.Convert.ToChar(0x2665)} {product.Like}";
            drawParameters.Price = product.Price;

            //drawParameters.Title = product.Title.DeleteUnSupportChar().ChangeLengthText(thumbnailSize.Width, new Font(fontName,FontStyle.Regular));
            //drawParameters.ShopName = product.Shop.Name.DeleteUnSupportChar().ChangeLengthText(thumbnailSize.Width - (shopIconSize.Width + 5), font);

            return drawParameters;
        }        
        private void DownloadAsCache(string url, Size size)
        {
            if(!localCache.ImageExists(Path.GetFileName(url)))
            {
                using Bitmap bitmap = boothClient.DownloadImage(url);
                using Bitmap resizedBitmap = new Bitmap(bitmap, size);
                localCache.AddImage(Path.GetFileName(url), resizedBitmap);
            }
        }
    }
}
