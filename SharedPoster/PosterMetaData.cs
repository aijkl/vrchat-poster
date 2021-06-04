using Newtonsoft.Json;
using System.IO;
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
            MD5HashEvaluation();
        }

        [JsonProperty("filePath")]
        public string FilePath { set; get; }

        [JsonProperty("MD5Hash")]
        public byte[] MD5Hash { set; get; }
        public void MD5HashEvaluation()
        {
            MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
            MD5Hash = mD5CryptoServiceProvider.ComputeHash(File.ReadAllBytes(FilePath));
            mD5CryptoServiceProvider.Clear();
        }
        public override bool Equals(object obj)
        {
            if (!(obj is PosterMetaData)) return false;
            PosterMetaData posterInfo = (PosterMetaData)obj;
            bool equal = false;
            if (MD5Hash.Length == posterInfo.MD5Hash.Length)
            {
                int i = 0;
                while ((i < MD5Hash.Length) && (MD5Hash[i] == posterInfo.MD5Hash[i]))
                {
                    i += 1;
                }
                if (i == MD5Hash.Length)
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
