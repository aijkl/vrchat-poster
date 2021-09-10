using System.IO;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace Aijkl.VRChat.Posters.Shared
{
    public class PosterMetaData
    {           
        public PosterMetaData()
        {

        }
        public PosterMetaData(string filePath)
        {
            FilePath = filePath;
            Md5HashEvaluation();
        }

        [JsonProperty("filePath")]
        public string FilePath { set; get; }

        [JsonProperty("MD5Hash")]
        public byte[] Md5Hash { set; get; }

        public void Md5HashEvaluation()
        {
            MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
            Md5Hash = mD5CryptoServiceProvider.ComputeHash(File.ReadAllBytes(FilePath));
            mD5CryptoServiceProvider.Clear();
        }
        public override bool Equals(object obj)
        {
            if (!(obj is PosterMetaData)) return false;
            PosterMetaData posterInfo = (PosterMetaData)obj;
            bool equal = false;
            if (Md5Hash.Length == posterInfo.Md5Hash.Length)
            {
                int i = 0;
                while ((i < Md5Hash.Length) && (Md5Hash[i] == posterInfo.Md5Hash[i]))
                {
                    i += 1;
                }
                if (i == Md5Hash.Length)
                {
                    equal = true;
                }
            }
            return equal && (posterInfo.FilePath == FilePath);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }        
    }
}
