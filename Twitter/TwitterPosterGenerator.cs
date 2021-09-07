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
using Aijkl.VRChat.Posters.Twitter.Shared.Models;
using Discord.Webhook;

namespace Aijkl.VRChat.Posters.Twitter
{
    public class TwitterPosterGenerator : IDisposable
    {               
        private HttpClient httpClient;
        private CloudFlareAPIClient cloudFlareClient;
        private LinkPreviewClient linkPreviewClient;
        private DiscordWebhookClient discordWebhookClient;
        private TranslateService translateService;
        private DriveService driveService;
        private CloudSettings cloudSettings;
        private readonly Tokens tokens;
        private readonly LocalSettings localSettings;
        private readonly LocalCache localCache;
        private readonly PosterMetaDataCollection cacheMetaDataCollection;
        private readonly ImageToVideoConverter imageToVideoConverter;
        public TwitterPosterGenerator(LocalSettings localSettings)
        {
            this.localSettings = localSettings;
            string authJson = File.ReadAllText(localSettings.ServiceAccountTokenPath);
            driveService = PosterHelper.CreateDriveService(authJson);
            translateService = PosterHelper.CreateTranslateService(authJson);

            ServicePointManager.DefaultConnectionLimit = 5;
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            httpClient = new HttpClient(httpClientHandler)
            {
                Timeout = new TimeSpan(TimeSpan.TicksPerMinute)
            };

            cloudSettings = CloudSettings.Fetch(this.driveService, localSettings.CloudSettingsId);
            discordWebhookClient = new DiscordWebhookClient(localSettings.DiscordWebHookUrl);
            tokens = Tokens.Create(localSettings.TwitterParameters.APIKey, localSettings.TwitterParameters.APISecretKey, localSettings.TwitterParameters.AccessToken, localSettings.TwitterParameters.AccessTokenSecret);
            localCache = new LocalCache(localSettings.CacheDirectory);
            cloudFlareClient = new CloudFlareAPIClient(localSettings.CloudFlareParameters.EmailAddress, localSettings.CloudFlareParameters.AuthToken, new HttpClient());
            linkPreviewClient = new LinkPreviewClient(localSettings.LinkPreviewAPIKey, httpClient);
            imageToVideoConverter = new ImageToVideoConverter(cloudSettings.Common.FFMpegParameters);

            string tempDirectoryPath = $"{localSettings.TempDirectory}{Path.DirectorySeparatorChar}{localSettings.TempFileName}";
            if (!Directory.Exists(localSettings.TempDirectory)) Directory.CreateDirectory(localSettings.TempDirectory);
            if (!File.Exists(tempDirectoryPath)) File.WriteAllText(tempDirectoryPath, string.Empty);
            cacheMetaDataCollection = PosterMetaDataCollection.FromFile(tempDirectoryPath);            
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
                    List<string> createdFilePathList = new List<string>();
                    List<SDK2ImagePosterBase> sdk2ImagePosterBases = new List<SDK2ImagePosterBase>();

                    Parallel.ForEach(cloudSettings.SDK2Posters, poster => 
                    {
                        try
                        {                            
                            IEnumerable<SDK2ImagePoster> results = GenerateSDK2Posters(poster);                            
                            foreach (var result in results)
                            {                                
                                string saveDirectory = $"{localSettings.SaveDirectory}{Path.DirectorySeparatorChar}{result.Language}";
                                if (!Directory.Exists(saveDirectory)) Directory.CreateDirectory(saveDirectory);
                                result.Bitmap.Save($"{saveDirectory}{Path.DirectorySeparatorChar}{result.FileName}",poster.Quality);
                                result.Bitmap.Dispose();
                                sdk2ImagePosterBases.Add(result);
                                createdFilePathList.Add($"{saveDirectory}{Path.DirectorySeparatorChar}{result.FileName}");

                                string savePath = $"{saveDirectory}{Path.DirectorySeparatorChar}{result.FileName}";
                                if (!cacheMetaDataCollection.Exists(savePath)) cacheMetaDataCollection.Add(new PosterMetaData(savePath));
                                if (!cacheMetaDataCollection[savePath].Equals(new PosterMetaData(savePath)))
                                {
                                    cloudFlareUnnecessaryCaches.Add($"{localSettings.CloudFlareParameters.BaseUrl}/{result.Language.ToLower()}/{Path.GetFileName(savePath)}");
                                    cacheMetaDataCollection.HashEvaluation(savePath);
                                }                                
                                Console.WriteLine($"[SaveImage] Keyword:{poster.Title} Lang:{poster.Lang} {(poster.TranslationLanguages.Count > 0 ? $"TranslationLanguages:{string.Join(" ", poster.TranslationLanguages)}" : string.Empty)}");
                            }                                                        
                        }
                        catch (Exception ex)
                        {
                            CatchError(ex);
                        }
                    });

                    foreach (SDK3PosterParameters poster in cloudSettings.SDK3Posters)
                    {
                        if (poster.TargetImagePosters.All(x => sdk2ImagePosterBases.Any(x.Equals)))
                        {
                            SDK3PosterBox sdk3PosterBox = new SDK3PosterBox();
                            foreach (SDK2ImagePosterBase sdk2Poster in sdk2ImagePosterBases.Where(x => poster.TargetImagePosters.Any(x.Equals)))
                            {
                                using SKImage skImage = SKImage.FromBitmap(SKBitmap.Decode(Path.Combine(localSettings.SaveDirectory, sdk2Poster.Language, sdk2Poster.FileName)));
                                if (!sdk3PosterBox.TryDrawTweet(skImage)) return;
                            }

                            string tempFilePath = Path.Combine(localSettings.TempDirectory, $"{poster.Id}.{poster.TempImageFormat}");
                            try
                            {
                                sdk3PosterBox.Result.Save(tempFilePath);
                                string savePath = imageToVideoConverter.GenerateOutputPath(Path.Combine(localSettings.SaveDirectory, poster.Language), poster.Id);
                                imageToVideoConverter.CreateVideoFromImage(tempFilePath, savePath);
                                createdFilePathList.Add(savePath);

                                if (!cacheMetaDataCollection.Exists(savePath)) cacheMetaDataCollection.Add(new PosterMetaData(savePath));
                                if (!cacheMetaDataCollection[savePath].Equals(new PosterMetaData(savePath)))
                                {
                                    cloudFlareUnnecessaryCaches.Add($"{localSettings.CloudFlareParameters.BaseUrl}/{poster.Language.ToLower()}/{Path.GetFileName(savePath)}");
                                    cacheMetaDataCollection.HashEvaluation(savePath);
                                }
                            }
                            catch (Exception exception)
                            {
                                CatchError(exception);
                            }
                            finally
                            {
                                try
                                {
                                    if (File.Exists(tempFilePath))
                                    {
                                        File.Delete(tempFilePath);
                                    }
                                }
                                catch (Exception exception)
                                {
                                    CatchError(exception);
                                    // ignore
                                }
                            }
                        }
                    }

                    cacheMetaDataCollection.Where(x => !createdFilePathList.Contains(x.FilePath)).ToList().ForEach(x => 
                    {
                        File.Delete(x.FilePath);
                        cloudFlareUnnecessaryCaches.Add($"{localSettings.CloudFlareParameters.BaseUrl}/{x.FilePath.Replace($"{localSettings.SaveDirectory}{Path.DirectorySeparatorChar}", string.Empty).Replace(Path.DirectorySeparatorChar.ToString(), "/")}");                                                                        
                    });

                    localCache.DeleteUnusedCache();
                    cacheMetaDataCollection.DeleteUnUsedMetaData();
                    cacheMetaDataCollection.SaveToFile();                    
                    
                    if (cloudFlareUnnecessaryCaches.Count > 0)
                    {
                        cloudFlareClient.Zone.PurgeFilesByUrl(localSettings.CloudFlareParameters.ZoneId, cloudFlareUnnecessaryCaches);
                        Console.WriteLine($"[DeleteCache] {string.Join(" ", cloudFlareUnnecessaryCaches.Select(Path.GetFileName))}");
                        cloudFlareUnnecessaryCaches.Clear();
                    }
                }
                catch (Exception ex)
                {
                    CatchError(ex);
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
            discordWebhookClient?.Dispose();
            cloudFlareClient?.Dispose();
            linkPreviewClient?.Dispose();

            httpClient = null;
            translateService = null;
            driveService = null;
            discordWebhookClient = null;
            cloudFlareClient = null;
            linkPreviewClient = null;
        }
        private void CatchError(Exception ex)
        {
            try
            {
                Console.WriteLine($"[Error] {ex.Message}");
                discordWebhookClient?.SendMessageAsync($"{ex.Message}{ex.StackTrace}{ex.InnerException}");
            }
            catch
            {
                // ignored
            }
        }
        private IEnumerable<SDK2ImagePoster> GenerateSDK2Posters(SDK2PosterParameters posterParameters)
        {            
            List<SDK2ImagePoster> resultPosters = new List<SDK2ImagePoster>();
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
                        if (statuses.All(x => drawnTweetIds.Contains(x.Id)))
                        {
                            foreach (var status in tokens.Search.Tweets(q: posterParameters.Query, count: 50, lang: posterParameters.Lang, locale: posterParameters.Locale, result_type: posterParameters.Sort, include_entities: true, tweet_mode: TweetMode.Extended))
                            {
                                statuses.Add(status);
                            }
                        }

                        foreach (PromotionTweet promotionTweet in cloudSettings.Promotion.Tweets.Where(x => x.TargetPosterIds.Any(y => y == posterParameters.Id)))
                        {
                            if (promotionTweet.DrawIndex < statuses.Count())
                            {
                                if (statuses.Any(x => x.Id == promotionTweet.TweetId))
                                {
                                    statuses.Remove(statuses.First(x => x.Id == promotionTweet.TweetId));
                                }
                                statuses.Insert(promotionTweet.DrawIndex, tokens.Statuses.Show(id: promotionTweet.TweetId, trim_user: false, include_my_retweet: false, include_entities: true, include_ext_alt_text: true, TweetMode.Extended));
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
                    SDK2PosterBox posterBox = new SDK2PosterBox(posterParameters.Title, posterParameters.Fonts.First(x => x.Key == language).Value, 50, posterParameters.RGBGroup.Background.ToColor());
                    foreach (var status in statuses.Where(x => !drawnTweetIds.Contains(x.Id) && !cloudSettings.Muted.IsMuted(x)))
                    {
                        try
                        {
                            using TweetBox tweetBox = GenerateTweetBox(status, posterParameters.Fonts.First(x => x.Key == language).Value, posterParameters.RGBGroup.Tweet.ToColor(), language);
                            using SKImage tempImage = SKImage.FromBitmap(tweetBox.Result);
                            if (!posterBox.TryDrawTweet(tempImage))
                            {
                                break;
                            }

                            drawnTweetIds.Add(status.Id);
                        }
                        catch (ArgumentException)
                        {
                            // ignore
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
                            resultPosters.Add(new SDK2ImagePoster()
                            {
                                Id = posterParameters.Id,
                                PageIndex = page,
                                Bitmap = posterBox.Result,
                                FileName = $"{posterParameters.Id}{(page != 1 ? page.ToString() : string.Empty)}.{posterParameters.Format}",
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
                image = DownloadPreviewImage($"https://twitter.com/statuses/{status.Id}", out string responseImageUrl);                
                //TODO
                //ここを綺麗にする LinkPreviewAPIでは無料プランだとアイコンなのかコンテンツなのかわからない                
                if(responseImageUrl.Contains("icon") || responseImageUrl.Contains("favicon"))
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
            string text = string.IsNullOrEmpty(translatedText) ? unTranslatedText : translatedText;
            string userName = status.User.Name.TrimText("utf-8");

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
        private SKBitmap DownloadPreviewImage(string url, out string responseImageUrl)
        {
            responseImageUrl = "";

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
                    responseImageUrl = linkPreview.Image;                    
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
