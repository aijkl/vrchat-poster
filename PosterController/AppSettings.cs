using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Aijkl.VRChat.Posters.Shared.Twitter.Models;

namespace PosterController
{
    public class AppSettings
    {
        [JsonIgnore] 
        public string FilePath { set; get; }

        [JsonProperty("authTokenPath")]
        public string AuthTokenPath { set; get; }

        [JsonProperty("fileID")]
        public string FileID { set; get; }

        [JsonProperty("languageDataSet")]
        public LanguageDataSet LanguageDataSet { set; get; }

        [JsonProperty("twitterParameters")]
        public TwitterApiParameters TwitterParameters { set; get; }

        public static AppSettings Load(string path)
        {
            AppSettings appSettings = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(Path.GetFullPath(path)));
            appSettings.FilePath = path;
            return appSettings;
        }
        public void Save()
        {
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(this));
        }
    }
    public class LanguageDataSet
    {
        public const string CONFIG_FILE__LOAD_ERROR = "An error occurred while reading the configuration file";
        public string GetValue(string memberName)
        {
            Dictionary<string, string> keyValuePairs = (Dictionary<string, string>)GetType().GetProperty(memberName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).GetValue(this);
            if (keyValuePairs.TryGetValue(CultureInfo.CurrentCulture.TwoLetterISOLanguageName, out string value))
            {
                return value;
            }
            else
            {
                value = keyValuePairs.ToList().FirstOrDefault().Value;
                value = string.IsNullOrEmpty(value) ? string.Empty : value;
                return value;
            }
        }
        
        [JsonProperty("server.Communication")]
        public Dictionary<string, string> ServerCommunication { set; get; }

        [JsonProperty("server.Communication.Error")]
        public Dictionary<string, string> ServerCommunicationError { set; get; }

        [JsonProperty("general.Header.Text")]
        public Dictionary<string, string> GeneralHeaderText { set; get; }

        [JsonProperty("general.Error")]
        public Dictionary<string, string> GeneralError { set; get; }

        [JsonProperty("general.FormatError")]
        public Dictionary<string, string> GeneralFormatError { set; get; }

        [JsonProperty("initialization.Error")]
        public Dictionary<string,string> InitializationError { set; get; }

        [JsonProperty("mute.Poster.Select")]
        public Dictionary<string, string> MutePosterSelect { set; get; }

        [JsonProperty("mute.Info")]
        public Dictionary<string, string> MuteInfo { set; get; }

        [JsonProperty("mute.Success")]
        public Dictionary<string, string> MuteSuccess { set; get; }

        [JsonProperty("mute.Tweet.Select")]
        public Dictionary<string,string> MuteTweetSelect { set; get; }

        [JsonProperty("poster.Info")]
        public Dictionary<string, string> PosterInfo { set; get; }

        [JsonProperty("promotion.Poster.Select")]
        public Dictionary<string, string> PromotionPosterSelect { set; get; }

        [JsonProperty("promotion.TweetId")]
        public Dictionary<string, string> PromotionTweetId { set; get; }

        [JsonProperty("promotion.AlreadyExists")]
        public Dictionary<string, string> PromotionAlreadyExists { set; get; }

        [JsonProperty("promotion.Days")]
        public Dictionary<string, string> PromotionDays { set; get; }

        [JsonProperty("promotion.DrawIndex")]
        public Dictionary<string, string> PromotionDrawIndex { set; get; }

        [JsonProperty("promotion.NotFound")]
        public Dictionary<string, string> PromotionNotFound { set; get; }

        [JsonProperty("promotion.Success")]
        public Dictionary<string, string> PromotionSuccess { set; get; }
    }
}
