using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Net.Http;
using System.Threading;
using System.Drawing.Imaging;
using System.Collections.Generic;
using Google.Apis.Drive.v3;
using Aijkl.VRChat.Posters.Shared;
using Aijkl.VRChat.Posters.Booth.Scraping;
using Aijkl.VRChat.Posters.Booth.Expansion;
using Aijkl.VRChat.Posters.Shared.Expansion;
using Encoder = System.Drawing.Imaging.Encoder;
using Aijkl.VRChat.SharedPoster.Expansion;
using Aijkl.CloudFlare.API;

namespace Aijkl.VRChat.Posters.Booth
{
    internal class BoothPosterGenerater
    {        
        private readonly string logoPath;
        private readonly BoothClient boothClient;                        
        private readonly DiscordClient discord;
        private readonly CloudFlareAPIClient cloudFlareClient;
        private readonly LocalCache localCache;
        private readonly DriveService driveService;
        private readonly PosterMetaDataCollection cacheMetaDatas;
        private readonly LocalSettings localSettings;        
        private CloudSettings cloudSettings;        

        public BoothPosterGenerater(DriveService driveService, LocalSettings localSettings, string logoPath)
        {
            this.localSettings = localSettings;
            this.logoPath = logoPath;
            this.driveService = driveService;

            HttpClient httpClient = new HttpClient(new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.All }) { Timeout = TimeSpan.FromSeconds(10) };
            boothClient = new BoothClient(httpClient);
            cloudSettings = CloudSettings.Fetch(driveService, localSettings.CloudSettingsId);
            discord = new DiscordClient(httpClient);
            localCache = new LocalCache(localSettings.CacheDirectory);
            cloudFlareClient = new CloudFlareClient(localSettings.CloudFlareParameters.EmailAdress, localSettings.CloudFlareParameters.AuthToken);            

            string tempFilePath = $"{localSettings.TempDirectory}{Path.DirectorySeparatorChar}{localSettings.TempFileName}";
            if (!File.Exists(tempFilePath)) File.WriteAllText(tempFilePath, string.Empty);
            cacheMetaDatas = PosterMetaDataCollection.FromFile(tempFilePath);            
        }
        public void BeginLoop(CancellationToken cancellationToken)            
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested) cancellationToken.ThrowIfCancellationRequested();
                try
                {                                        
                    cloudSettings = CloudSettings.Fetch(driveService, localSettings.CloudSettingsId);
                    List<Product> products = new List<Product>();
                    List<string> cloudFlareUnnecessaryCaches = new List<string>();
                    List<string> createdFiles = new List<string>();

                    foreach (var poster in cloudSettings.Posters)
                    {
                        for (int i = 0, page = 1; i < poster.Page; i++)
                        {
                            for (int requestCount = 0; products.Where(x => x.Like >= poster.MinLike && x.KeyWord == poster.KeyWord).Skip(i * 16).Count() < 16;)
                            {
                                if (requestCount > 15) { break; }

                                SearchResult<Product> searchResult = boothClient.SearchProducts(poster.KeyWord, BoothClient.Sort.New, page);
                                foreach (var product in searchResult)
                                {
                                    if (!products.Any(x => x.Id == product.Id))
                                    {
                                        products.Add(product);
                                    }
                                }

                                requestCount++;
                                page++;
                            }                                              
                        }
                    }                                
                    foreach (var poster in cloudSettings.Posters)
                    {
                        for (int i = 0, page = 1; i < poster.Page; page++, i++)
                        {
                            try
                            {
                                ImageCodecInfo imageCodecInfo = ImageCodecInfo.GetImageEncoders().ToList().First(y => y.MimeType.Contains(poster.Format));
                                EncoderParameters encoderParameters = new EncoderParameters(1);
                                encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, poster.Quality);
                                if (!Directory.Exists(localSettings.SaveDirectory))
                                {
                                    Directory.CreateDirectory(localSettings.SaveDirectory);
                                }

                                using Bitmap result = CreatePoster(products.Where(y => poster.KeyWord == y.KeyWord && y.Like >= poster.MinLike).Skip((i) * 16).Select(x => x).Take(16).ToList(), poster.FontName, poster.RgbGroup.BackGround.ToColor(), poster.RgbGroup.ProductBox.ToColor());
                                string path = $"{localSettings.SaveDirectory}{Path.DirectorySeparatorChar}ja{Path.DirectorySeparatorChar}{poster.FileName}{(page > 1 ? page.ToString() : string.Empty)}.{poster.Format}";
                                if (!Directory.Exists(Path.GetDirectoryName(path))) Directory.CreateDirectory(path);
                                result.Save(path, imageCodecInfo, encoderParameters);
                                createdFiles.Add(path);

                                if (!cacheMetaDatas.Exsists(path)) cacheMetaDatas.Add(new PosterMetaData(path));
                                if (!cacheMetaDatas[path].Equals(new PosterMetaData(path)))
                                {
                                    cloudFlareUnnecessaryCaches.Add($"{localSettings.CloudFlareParameters.BaseUrl}/ja/{Path.GetFileName(path)}");
                                    cacheMetaDatas.HashEvaluation(path);
                                }

                                Console.WriteLine($"[SaveImage] Keyword:{poster.KeyWord}");
                            }
                            catch (Exception ex)
                            {
                                CacheError(ex, cloudSettings.Common.DiscordWebHookURL);
                            }
                        }
                    }

                    cacheMetaDatas.Where(x => !createdFiles.Contains(x.FilePath)).ToList().ForEach(x =>
                    {
                        File.Delete(x.FilePath);
                        cloudFlareUnnecessaryCaches.Add($"{localSettings.CloudFlareParameters.BaseUrl}/{x.FilePath.Replace($"{localSettings.SaveDirectory}{Path.DirectorySeparatorChar}", string.Empty).Replace(Path.DirectorySeparatorChar.ToString(), "/")}");
                    });

                    localCache.DeleteUnusedCache();
                    cacheMetaDatas.DeleteUnUsedMetaData();
                    cacheMetaDatas.SaveToFile();

                    if (cloudFlareUnnecessaryCaches.Count > 0)
                    {
                        cloudFlareClient.Zone.PurgeFilesByUrl(localSettings.CloudFlareParameters.ZoneId, cloudFlareUnnecessaryCaches);
                        Console.WriteLine($"[DeleteCache] {string.Join(" ", cloudFlareUnnecessaryCaches.Select(x => Path.GetFileName(x)))}");
                        cloudFlareUnnecessaryCaches.Clear();
                    }
                }
                catch (Exception ex)
                {
                    CacheError(ex,cloudSettings.Common.DiscordWebHookURL);
                }
                Thread.Sleep(cloudSettings.Common.UpdateInterval);
            }
        }
        public Bitmap CreatePoster(List<Product> products, string fontName, Color backgroundColor, Color productBoxColor)
        {                         
            Bitmap result = new Bitmap(1100, 1600);
            using (Bitmap logo = new Bitmap(logoPath))
            using (Graphics graphics = PosterHelper.CreateGraphicsObject(result))
            {                
                Point position = new Point(36, 155);                              
                Size margin = new Size(36, 10);

                graphics.FillRectangle(new SolidBrush(backgroundColor), graphics.VisibleClipBounds);
                graphics.DrawImage(logo, new Point(36, 36));                
                for (int i = 0; i < products.Count; i++)
                {
                    Bitmap productImage = null;
                    try
                    {
                        productImage = CreateProduct(products[i], fontName, productBoxColor);
                    }
                    catch (Exception ex)
                    {
                        CacheError(ex, cloudSettings.Common.DiscordWebHookURL);
                    }   

                    if(productImage != null)
                    {
                        if (result.Width < (position.X + margin.Width + productImage.Width))
                        {
                            position.X = margin.Width;
                            position.Y += (productImage.Height + margin.Height);
                        }
                        if (result.Height < (position.Y + margin.Height + productImage.Height))
                        {
                            break;
                        }

                        graphics.DrawImage(productImage, position);
                        position.X += (productImage.Width + margin.Width);
                        productImage.Dispose();                                                
                    }                    
                }
            }                                    
            return result;
        }
        public Bitmap CreateProduct(Product product, string fontName, Color color)
        {
            using Font titleFont = new Font(fontName, 12, FontStyle.Bold);
            using Font priceFont = new Font(fontName, 15, FontStyle.Regular);
            using Font shopNameFont = new Font(fontName, 11, FontStyle.Regular);

            DownloadAsCache(product.ThumbnailUrl, new Size(230, 230));
            DownloadAsCache(product.Shop.IconUrl, new Size(36, 38));
            localCache.GetImage(Path.GetFileName(product.ThumbnailUrl),out Bitmap thumbnail);
            localCache.GetImage(Path.GetFileName(product.Shop.IconUrl), out Bitmap userIcon);

            Bitmap result = new Bitmap(thumbnail.Width, thumbnail.Height + 115);
            
            using (Graphics graphics = PosterHelper.CreateGraphicsObject(result))
            {
                Point positon = new Point(0, 0);
                Size margin = new Size(5, 7);

                using (thumbnail)
                using (userIcon)
                {                    
                    graphics.FillRectangle(new SolidBrush(color), graphics.ClipBounds);                

                    graphics.DrawImage(thumbnail, positon);                    
                    positon.Y += thumbnail.Height + margin.Height;
                    
                    string title = product.Title.TrimText("shift-jis").ChangeLengthText(thumbnail.Width, titleFont);
                    graphics.DrawString(title, titleFont, Brushes.Black, positon.X, positon.Y);
                    positon.Y += (int)title.MeasureString(titleFont).Height + margin.Height;

                    graphics.DrawImage(userIcon, positon);
                    string shopName = product.Shop.Name.TrimText("shift-jis").ChangeLengthText(thumbnail.Width - (userIcon.Width + margin.Width), shopNameFont);
                    graphics.DrawString(shopName, shopNameFont, Brushes.Black, new PointF(userIcon.Width + margin.Width, (int)(positon.Y + ( userIcon.Height - shopName.MeasureString(shopNameFont, includeGlyphs: false).Height ) / 2)));//ショップ名                                                                
                    positon.Y += userIcon.Height + margin.Height;
                    
                    graphics.DrawString(product.Price, priceFont, new SolidBrush(Color.FromArgb(255, 92, 103)), positon);
                    positon.X += (int)(product.Price.MeasureString(priceFont).Width);
                    
                    string likeCount = $"{Convert.ToChar(0x2665)} {product.Like}";
                    SolidBrush redBrush = new SolidBrush(Color.FromArgb(255, 92, 103));
                    graphics.DrawString(likeCount, priceFont, redBrush, new Point(result.Width - ((int)likeCount.MeasureString(priceFont, includeGlyphs: true).Width + 2), positon.Y));
                }                
            }
            return result;
        }
        private void DownloadAsCache(string url, Size size)
        {
            if (!localCache.ImageExists(Path.GetFileName(url)))
            {
                using Bitmap bitmap = boothClient.DownloadImage(url);
                using Bitmap resizedBitmap = new Bitmap(bitmap, size);
                localCache.AddImage(Path.GetFileName(url), resizedBitmap);
            }
        }
        private void CacheError(Exception exception,string url)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("```");
            stringBuilder.AppendLine($"[CacheException] {GetType().FullName}");            
            stringBuilder.AppendLine(exception.Message);
            stringBuilder.AppendLine(exception.StackTrace);
            stringBuilder.Append("```");
            discord.PostMessage(stringBuilder.ToString(), url);
        }
    }
}
