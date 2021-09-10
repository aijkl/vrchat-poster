using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aijkl.VRChat.Posters.Shared;
using Newtonsoft.Json;

namespace Aijkl.VRChat.Posters.Twitter
{
    public class ApiResponseHashes : List<ApiResponseHash>
    {

        [JsonIgnore]
        public string FilePath { set; get; }
        public ApiResponseHash this[string id]
        {
            get
            {
                return this.FirstOrDefault(x => x.Id == id);
            }
        }
        public bool Exists(string id,int pageIndex)
        {
            return this.Any(x => x.Id == id && x.PageIndex == pageIndex);
        }

        public ApiResponseHash? FirstOrDefault(ApiResponseHash apiResponseHash)
        {
            return this.FirstOrDefault(x => x.Id == apiResponseHash.Id && x.Language == apiResponseHash.Language && x.PageIndex == apiResponseHash.PageIndex);
        }
        public void SaveToFile()
        {
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(this,Formatting.Indented));
        }
        public void AddOrUpdate(ApiResponseHash apiResponseHash)
        {
            ApiResponseHash temp = this.FirstOrDefault(x => x.Id == apiResponseHash.Id && x.Language == apiResponseHash.Language && x.PageIndex == apiResponseHash.PageIndex);
            if (temp != null)
            {
                temp.Md5Hash = apiResponseHash.Md5Hash;
            }
            else
            {
                Add(apiResponseHash);
            }
        }
        public static ApiResponseHashes FromFile(string filePath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? throw new InvalidOperationException());
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }

            string file = File.ReadAllText(filePath);
            file = string.IsNullOrEmpty(file) ? JsonConvert.SerializeObject(new ApiResponseHashes()) : file;
            ApiResponseHashes posterMetaData = JsonConvert.DeserializeObject<ApiResponseHashes>(file, new JsonSerializerSettings());
            posterMetaData.FilePath = filePath;
            return posterMetaData;
        }
    }
}
