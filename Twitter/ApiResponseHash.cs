using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using CoreTweet;
using Newtonsoft.Json;

namespace Aijkl.VRChat.Posters.Twitter
{
    public class ApiResponseHash
    {
        private readonly string _id;
        private readonly byte[] _md5hash;
        private readonly int _pageIndex;
        private readonly string _language;

        private ApiResponseHash()
        {

        }

        public ApiResponseHash(string id, string language, int pageIndex, Status status)
        {
            _id = id;
            Id = _id;
            PageIndex = pageIndex;
            _pageIndex = pageIndex;
            _language = language;
            Language = language;
            HashEvaluation(HashItem.CreateFromStatus(status));
            _md5hash = Md5Hash;
        }

        public ApiResponseHash(string id, string language, int pageIndex, IEnumerable<Status> statuses)
        {
            _id = id;
            Id = _id;
            PageIndex = pageIndex;
            _language = language;
            _pageIndex = pageIndex;
            Language = language;
            HashEvaluation(statuses.Select(HashItem.CreateFromStatus));
            _md5hash = Md5Hash;
        }

        [JsonProperty("id")]
        public string Id { private set; get; }

        [JsonProperty("pageIndex")]
        public int PageIndex { private set; get; }

        [JsonProperty("language")]
        public string Language { private set; get; }

        [JsonProperty("fileMD5Hash")]
        public byte[] Md5Hash { set;get; }

        private void HashEvaluation(object obj)
        {
            MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
            Md5Hash = mD5CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj)));
            mD5CryptoServiceProvider.Clear();
        }
        public override bool Equals(object obj)
        {
            if (obj is ApiResponseHash apiResponseHash)
            {
                return Id == apiResponseHash.Id && PageIndex == apiResponseHash.PageIndex && Language == apiResponseHash.Language && Md5Hash.SequenceEqual(apiResponseHash.Md5Hash);
            }
            return false;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(_id, _pageIndex, _language,_md5hash);
        }
    }
}
