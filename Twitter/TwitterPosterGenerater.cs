using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Google.Apis.Drive.v3;
using Google.Apis.Translate.v2;
using Google.Apis.Translate.v2.Data;
using Aijkl.VRChat.Posters.Shared;
using Aijkl.VRChat.Posters.Shared.Twitter;
using Aijkl.VRChat.Posters.Shared.Expansion;
using Aijkl.VRChat.Posters.Shared.Twitter.Models;
using CoreTweet;
using static Google.Apis.Translate.v2.TranslationsResource;
using Aijkl.VRChat.Posters.Twitter.Paint.Components;
using System.Net;
using Aijkl.VRChat.SharedPoster.Expansion;
using System.Text.RegularExpressions;
using Aijkl.CloudFlare.API;
using SkiaSharp;
using Aijkl.LinkPreview.API;

namespace Aijkl.VRChat.Posters.Twitter
{
    public class TwitterPosterGenerater : IDisposable
    {               
        private readonly HttpClient httpClient;
        private readonly Tokens tokens;
        private readonly TranslateService translateService;
        private readonly DriveService driveService;
        private readonly DiscordClient discordClient;
        private readonly LocalSettings localSettings;
        private readonly LocalCache localCache;
        private readonly CloudFlareAPIClient cloudFlareClient;
        private readonly PosterMetaDatas cacheMetaDatas;
        private readonly LinkPreviewClient linkPreviewClient;
        private CloudSettings cloudSettings;

        public TwitterPosterGenerater(LocalSettings localSettings, DriveService driveService, TranslateService translateService)
        {
            this.driveService = driveService;            
            this.localSettings = localSettings;
            this.translateService = translateService;

            ServicePointManager.DefaultConnectionLimit = 5;            
            httpClient = new HttpClient(new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip,
                AllowAutoRedirect = true,                
            })
            {
                Timeout = new TimeSpan(TimeSpan.TicksPerMinute)
            };
            httpClient.DefaultRequestHeaders.Add("Connection", "close");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.100");

            cloudSettings = CloudSettings.Fetch(this.driveService, localSettings.CloudSettingsId);
            discordClient = new DiscordClient(httpClient);
            tokens = Tokens.Create(localSettings.TwitterParameters.APIKey, localSettings.TwitterParameters.APISecretKey, localSettings.TwitterParameters.AccessToken, localSettings.TwitterParameters.AccessTokenSecret);
            localCache = new LocalCache(localSettings.CacheDirectory);
            cloudFlareClient = new CloudFlareAPIClient(localSettings.CloudFlareParameters.EmailAdress, localSettings.CloudFlareParameters.AuthToken, new HttpClient());
            linkPreviewClient = new LinkPreviewClient(localSettings.LinkPreviewAPIKey, httpClient);

            string tempDirectoryPath = $"{localSettings.TempDirectory}{Path.DirectorySeparatorChar}{localSettings.TempFileName}";
            if (!Directory.Exists(localSettings.TempDirectory)) Directory.CreateDirectory(localSettings.TempDirectory);
            if (!File.Exists(tempDirectoryPath)) File.WriteAllText(tempDirectoryPath, string.Empty);
            cacheMetaDatas = PosterMetaDatas.FromFile(tempDirectoryPath);            
        }
        public void BeginLoop(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested) cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    cloudSettings = CloudSettings.Fetch(driveService, localSettings.CloudSettingsId);                    
                    List<string> cloudFlareUnnecessaryCaches = new List<string>();
                    List<string> createdFiles = new List<string>();

                    Parallel.ForEach(cloudSettings.Posters, poster => 
                    {
                        try
                        {                            
                            List<Poster> results = GeneratePosters(poster);                            
                            foreach (var result in results)
                            {                                
                                string saveDirectory = $"{localSettings.SaveDirectory}{Path.DirectorySeparatorChar}{result.Language}";
                                if (!Directory.Exists(saveDirectory)) Directory.CreateDirectory(saveDirectory);
                                result.Bitmap.Save($"{saveDirectory}{Path.DirectorySeparatorChar}{result.FileName}",poster.Quality);
                                result.Bitmap.Dispose();
                                createdFiles.Add($"{saveDirectory}{Path.DirectorySeparatorChar}{result.FileName}");

                                string savePath = $"{saveDirectory}{Path.DirectorySeparatorChar}{result.FileName}";
                                if (!cacheMetaDatas.Exsists(savePath)) cacheMetaDatas.Add(new PosterMetaData(savePath));
                                if (!cacheMetaDatas[savePath].Equals(new PosterMetaData(savePath)))
                                {
                                    cloudFlareUnnecessaryCaches.Add($"{localSettings.CloudFlareParameters.BaseUrl}/{result.Language.ToLower()}/{Path.GetFileName(savePath)}");
                                    cacheMetaDatas.HashEvaluation(savePath);
                                }                                
                                Console.WriteLine($"[SaveImage] Keword:{poster.Title} Lang:{poster.Lang} {(poster.TranslationLanguages.Count > 0 ? $"TranslationLanguages:{string.Join(" ", poster.TranslationLanguages)}" : string.Empty)}");
                            }                                                        
                        }
                        catch (Exception ex)
                        {
                            CatchError(ex, cloudSettings.Common.DiscordWebHookURL);
                        }
                    });
                    
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
                    CatchError(ex, cloudSettings.Common.DiscordWebHookURL);
                }
                GC.Collect();                
                Thread.Sleep(cloudSettings.Common.UpdateInterval);
            }
        }
        public void Dispose()
        {
            httpClient?.Dispose();
            translateService?.Dispose();
            driveService?.Dispose();
            discordClient?.Dispose();
            cloudFlareClient?.Dispose();
        }
        private void CatchError(Exception ex, string url = "")
        {
            try
            {
                Console.WriteLine($"[Error] {ex.Message}");
                if (!string.IsNullOrEmpty(url)) discordClient.PostMessage(url, $"{ex.Message}{ex.StackTrace}{ex.InnerException}");
            }
            catch
            {                
            }            
        }
        private List<Poster> GeneratePosters(PosterParameters posterParameters)
        {            
            List<Poster> resultPosters = new List<Poster>();
            List<string> languages = new List<string>(posterParameters.TranslationLanguages)
            {
                posterParameters.Lang
            };
            List<Status> statuses = new List<Status>();            

            foreach (var language in languages)
            {
                List<long> drawnTweetIds = new List<long>();
                for (int i = 0, page = 1; i < posterParameters.Page; page++, i++)
                {
                    try
                    {
                        if (!statuses.Where(x => !drawnTweetIds.Contains(x.Id)).Any())
                        {
                            foreach (var status in tokens.Search.Tweets(q: posterParameters.Query, count: 50, lang: posterParameters.Lang, locale: posterParameters.Locale, result_type: posterParameters.Sort, include_entities: true, tweet_mode: TweetMode.Extended))
                            {
                                statuses.Add(status);
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        CatchError(ex);

                        if(resultPosters.Count >= 1)
                        {
                            return resultPosters;
                        }
                        throw;
                    }                    
                    PosterBox posterBox = new PosterBox(posterParameters.Title, posterParameters.Fonts.Where(x => x.Key == language).First().Value, 50, posterParameters.RGBGroup.Background.ToColor());
                    foreach (var status in statuses.Where(x => !drawnTweetIds.Contains(x.Id) && !cloudSettings.Mute.IsMuted(x)))
                    {
                        try
                        {
                            using TweetBox tweetBox = GenerateTweetBox(status, posterParameters.Fonts.Where(x => x.Key == language).First().Value, posterParameters.RGBGroup.Tweet.ToColor(), language);
                            using SKImage tempImage = SKImage.FromBitmap(tweetBox.Result);
                            if (!posterBox.TryDrawTweet(tempImage))
                            {
                                break;
                            }
                            drawnTweetIds.Add(status.Id);                                                        
                        }
                        catch (Exception ex)
                        {
                            CatchError(ex);
                        }
                    }
                    try
                    {
                        if (posterBox.Result != null)
                        {
                            resultPosters.Add(new Poster()
                            {
                                Bitmap = posterBox.Result,
                                FileName = $"{posterParameters.FileName}{(page != 1 ? page.ToString() : string.Empty)}.{posterParameters.Format}",
                                Language = language
                            });                            
                        }
                    }
                    catch (Exception ex)
                    {
                        CatchError(ex);
                    }
                }
            }
            return resultPosters;            
        }
        private TweetBox GenerateTweetBox(Status status, string fontName, SKColor color, string lang)
        {
            string userName = string.Empty;
            string text = string.Empty;
            SKBitmap userIcon = null;
            SKBitmap image = null;

            if (status.User.ProfileImageUrlHttps != null)
            {
                userIcon = DownloadImage(status.User.ProfileImageUrlHttps);
            }
            if (status.Entities.Media != null && !string.IsNullOrEmpty(status.Entities.Media[0].MediaUrlHttps))
            {
                image = DownloadImage(status.Entities.Media[0].MediaUrlHttps);
            }
            else if (status.Entities.Urls.Length != 0)
            {
                image = DownloadPreviewImage($"https://twitter.com/statuses/{status.Id}", out string responseImageURL);                
                //TODO
                //ここを綺麗にする LinkPreviewAPIでは無料プランだとアイコンなのかコンテンツなのかわからない                
                if(responseImageURL.Contains("icon") || responseImageURL.Contains("favicon"))
                {
                    image = null;
                }
            }
            if (image != null && (image.Width < 50 || image.Height < 50))
            {
                image = null;
            }

            string unTranslatedText = status.FullText.TrimText("utf-8");            
            status.Entities.Urls.ToList().ForEach(x => unTranslatedText = unTranslatedText.Replace(x.Url, x.ExpandedUrl));            
            unTranslatedText = Regex.Replace(unTranslatedText, @"https://t\.co/(.)+", string.Empty);

            string translatedText = string.Empty;
            if (status.Language != lang && !localCache.GetTranslation(status.Id, lang, out translatedText))
            {
                List<string> q = new List<string>
                {
                    unTranslatedText
                };
                TranslateRequest translateRequest = translateService.Translations.Translate(new TranslateTextRequest()
                {
                    Source = status.Language,
                    Target = lang,
                    Format = "text",
                    Q = q
                });
                TranslationsListResponse translationsListResponse = translateRequest.Execute();
                translatedText = translationsListResponse.Translations.First().TranslatedText.TrimText("utf-8");
                localCache.AddTranslation(status.Id, lang, translatedText);
            }
            text = string.IsNullOrEmpty(translatedText) ? unTranslatedText : translatedText;
            userName = status.User.Name.TrimText("utf-8");

            TweetBox tweetBox = new TweetBox(userName, text, fontName, color, new SKPoint(), userIcon, image);
            tweetBox.Draw();
            return tweetBox;
        }
        private SKBitmap DownloadImage(string url)
        {
            string fileName = Path.GetFileName(url);            
            if (!localCache.GetImage(fileName, out SKBitmap bitmap))
            {
                using HttpResponseMessage response = httpClient.GetAsync(url).Result;
                using Stream stream = response.Content.ReadAsStreamAsync().Result;
                bitmap = SKBitmap.Decode(stream);
                if(bitmap != null)
                {
                    localCache.AddImage(fileName, bitmap);
                }                
            }
            return bitmap;
        }
        private SKBitmap DownloadPreviewImage(string url, out string responseImageURL)
        {
            responseImageURL = "";

            try
            {
                Response linkPreview = null;
                if (localCache.GetLinkPreview(url,out Response response))
                {
                    linkPreview = response;
                }
                else
                {
                    try
                    {
                        linkPreview = linkPreviewClient.Main.Preview(url).Result;
                    }
                    catch (Exception ex) when(ex.InnerException is LinkPreviewAPIException exception && (exception.Result.HttpStatusCode != HttpStatusCode.TooManyRequests))
                    {                        
                    }
                    localCache.AddLinkPreview(url, linkPreview);
                }
                
                if (linkPreview != null && !string.IsNullOrEmpty(linkPreview.Image))
                {
                    responseImageURL = linkPreview.Image;                    
                    return DownloadImage(linkPreview.Image);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
