//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Text.RegularExpressions;
//using Aijkl.LinkPreview.API;
//using Aijkl.VRChat.Posters.Shared;
//using Aijkl.VRChat.Posters.Shared.Expansion;
//using Aijkl.VRChat.Posters.Shared.Twitter;
//using Aijkl.VRChat.Posters.Shared.Twitter.Models;
//using Aijkl.VRChat.Posters.Twitter.Paint.Components;
//using Aijkl.VRChat.Posters.Twitter.Shared.Models;
//using Aijkl.VRChat.SharedPoster.Expansion;
//using CoreTweet;
//using Google.Apis.Drive.v3;
//using Google.Apis.Translate.v2;
//using Google.Apis.Translate.v2.Data;
//using SkiaSharp;
//using TranslationsResource = Google.Apis.Translate.v2.TranslationsResource;

//namespace Aijkl.VRChat.Posters.Twitter
//{
//    public class SDK2PosterGenerator : IDisposable
//    {
//        private readonly Tokens _tokens;
//        private readonly LocalCache _localCache;
//        private LinkPreviewClient _linkPreviewClient;
//        private TranslateService _translateService;
//        private DriveService _driveService;
//        private HttpClient _httpClient;
//        public SDK2PosterGenerator(Tokens tokens, LocalCache localCache, TranslateService translateService, DriveService driveService)
//        {
//            _tokens = tokens;
//            _
            

//            ServicePointManager.DefaultConnectionLimit = 5;
//            HttpClientHandler httpClientHandler = new HttpClientHandler();
//            _httpClient = new HttpClient(httpClientHandler)
//            {
//                Timeout = new TimeSpan(TimeSpan.TicksPerMinute)
//            };
//        }

//        public event EventHandler<Exception> CanContinueError;
//        public IEnumerable<SDK2ImagePoster> GeneratePosters(CloudSettings cloudSettings, SDK2PosterParameters posterParameters)
//        {
//            List<SDK2ImagePoster> resultPosters = new List<SDK2ImagePoster>();
//            List<string> languages = new List<string>(posterParameters.TranslationLanguages)
//            {
//                posterParameters.Lang
//            };
//            List<Status> statuses = new List<Status>();

//            foreach (var language in languages)
//            {
//                List<long> drawnTweetIds = new List<long>();
//                for (int i = 0, page = 1; i < posterParameters.Page; page++, i++)
//                {
//                    try
//                    {
//                        if (statuses.All(x => drawnTweetIds.Contains(x.Id)))
//                        {
//                            foreach (var status in _tokens.Search.Tweets(q: posterParameters.Query, count: 50, lang: posterParameters.Lang, locale: posterParameters.Locale, result_type: posterParameters.Sort, include_entities: true, tweet_mode: TweetMode.Extended))
//                            {
//                                statuses.Add(status);
//                            }
//                        }

//                        foreach (PromotionTweet promotionTweet in cloudSettings.Promotion.Tweets.Where(x => x.TargetPosterIds.Any(y => y == posterParameters.Id)))
//                        {
//                            if (promotionTweet.DrawIndex < statuses.Count())
//                            {
//                                if (statuses.Any(x => x.Id == promotionTweet.TweetId))
//                                {
//                                    statuses.Remove(statuses.First(x => x.Id == promotionTweet.TweetId));
//                                }
//                                statuses.Insert(promotionTweet.DrawIndex, _tokens.Statuses.Show(id: promotionTweet.TweetId, trim_user: false, include_my_retweet: false, include_entities: true, include_ext_alt_text: true, TweetMode.Extended));
//                            }
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        OnCanContinueError(ex);

//                        if (resultPosters.Count >= 1)
//                        {
//                            return resultPosters;
//                        }
//                        throw;
//                    }
//                    SDK2PosterBox posterBox = new SDK2PosterBox(posterParameters.Title, posterParameters.Fonts.First(x => x.Key == language).Value, 50, posterParameters.RGBGroup.Background.ToColor());
//                    foreach (var status in statuses.Where(x => !drawnTweetIds.Contains(x.Id) && !cloudSettings.Muted.IsMuted(x)))
//                    {
//                        try
//                        {
//                            using TweetBox tweetBox = GenerateTweetBox(status, posterParameters.Fonts.First(x => x.Key == language).Value, posterParameters.RGBGroup.Tweet.ToColor(), language);
//                            using SKImage tempImage = SKImage.FromBitmap(tweetBox.Result);
//                            if (!posterBox.TryDrawTweet(tempImage))
//                            {
//                                break;
//                            }
//                            drawnTweetIds.Add(status.Id);
//                        }
//                        catch (Exception ex)
//                        {
//                            OnCanContinueError(ex);
//                        }
//                    }
//                    try
//                    {
//                        if (posterBox.Result != null)
//                        {
//                            resultPosters.Add(new SDK2ImagePoster()
//                            {
//                                Id = posterParameters.Id,
//                                PageIndex = page,
//                                Bitmap = posterBox.Result,
//                                FileName = $"{posterParameters.Id}{(page != 1 ? page.ToString() : string.Empty)}.{posterParameters.Format}",
//                                Language = language
//                            });
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        OnCanContinueError(ex);
//                    }
//                }
//            }
//            return resultPosters;
//        }
//        private TweetBox GenerateTweetBox(Status status, string fontName, SKColor color, string lang)
//        {
//            SKBitmap userIcon = null;
//            SKBitmap image = null;

//            if (status.User.ProfileImageUrlHttps != null)
//            {
//                userIcon = DownloadImage(status.User.ProfileImageUrlHttps);
//            }
//            if (status.Entities.Media != null && !string.IsNullOrEmpty(status.Entities.Media[0].MediaUrlHttps))
//            {
//                image = DownloadImage(status.Entities.Media[0].MediaUrlHttps);
//            }
//            else if (status.Entities.Urls.Length != 0)
//            {
//                image = DownloadPreviewImage($"https://twitter.com/statuses/{status.Id}", out string responseImageUrl);
//                //TODO
//                //ここを綺麗にする LinkPreviewAPIでは無料プランだとアイコンなのかコンテンツなのかわからない                
//                if (responseImageUrl.Contains("icon") || responseImageUrl.Contains("favicon"))
//                {
//                    image = null;
//                }
//            }
//            if (image != null && (image.Width < 50 || image.Height < 50))
//            {
//                image = null;
//            }

//            string unTranslatedText = status.FullText.TrimText("utf-8");
//            status.Entities.Urls.ToList().ForEach(x => unTranslatedText = unTranslatedText.Replace(x.Url, x.ExpandedUrl));
//            unTranslatedText = Regex.Replace(unTranslatedText, @"https://t\.co/(.)+", string.Empty);

//            string translatedText = string.Empty;
//            if (status.Language != lang && !_localCache.GetTranslation(status.Id, lang, out translatedText))
//            {
//                List<string> q = new List<string>
//                {
//                    unTranslatedText
//                };
//                TranslationsResource.TranslateRequest translateRequest = _translateService.Translations.Translate(new TranslateTextRequest()
//                {
//                    Source = status.Language,
//                    Target = lang,
//                    Format = "text",
//                    Q = q
//                });
//                TranslationsListResponse translationsListResponse = translateRequest.Execute();
//                translatedText = translationsListResponse.Translations.First().TranslatedText.TrimText("utf-8");
//                _localCache.AddTranslation(status.Id, lang, translatedText);
//            }
//            string text = string.IsNullOrEmpty(translatedText) ? unTranslatedText : translatedText;
//            string userName = status.User.Name.TrimText("utf-8");

//            TweetBox tweetBox = new TweetBox(userName, text, fontName, color, new SKPoint(), userIcon, image);
//            tweetBox.Draw();
//            return tweetBox;
//        }
//        private SKBitmap DownloadImage(string url)
//        {
//            string fileName = Path.GetFileName(url);
//            if (!_localCache.GetImage(fileName, out SKBitmap bitmap))
//            {
//                using HttpResponseMessage response = _httpClient.GetAsync(url).Result;
//                using Stream stream = response.Content.ReadAsStreamAsync().Result;
//                bitmap = SKBitmap.Decode(stream);
//                if (bitmap != null)
//                {
//                    _localCache.AddImage(fileName, bitmap);
//                }
//            }
//            return bitmap;
//        }
//        private SKBitmap DownloadPreviewImage(string url, out string responseImageUrl)
//        {
//            responseImageUrl = "";

//            try
//            {
//                Response linkPreview = null;
//                if (_localCache.GetLinkPreview(url, out Response response))
//                {
//                    linkPreview = response;
//                }
//                else
//                {
//                    try
//                    {
//                        linkPreview = _linkPreviewClient.Main.Preview(url).Result;
//                    }
//                    catch (Exception ex) when (ex.InnerException is LinkPreviewAPIException exception && (exception.Result.HttpStatusCode != HttpStatusCode.TooManyRequests))
//                    {
//                    }
//                    _localCache.AddLinkPreview(url, linkPreview);
//                }

//                if (linkPreview != null && !string.IsNullOrEmpty(linkPreview.Image))
//                {
//                    responseImageUrl = linkPreview.Image;
//                    return DownloadImage(linkPreview.Image);
//                }
//                else
//                {
//                    return null;
//                }
//            }
//            catch (Exception)
//            {
//                return null;
//            }
//        }
//        protected virtual void OnCanContinueError(Exception e)
//        {
//            CanContinueError?.Invoke(this, e);
//        }
//        public void Dispose()
//        {
//            _httpClient?.Dispose();
//            _httpClient = null;
//        }
//    }
//}
