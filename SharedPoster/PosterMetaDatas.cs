using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Aijkl.VRChat.Posters.Shared
{    
    public class PosterMetaDataCollection : List<PosterMetaData>
    {
        [JsonIgnore]
        public string FilePath { set; get; }
        public PosterMetaData this[string path]
        {
            get 
            {
                return this.FirstOrDefault(x => path == x.FilePath);
            }            
        }                
        public bool Exists(string path)
        {
            return this.Any(x => x.FilePath == path);
        }
        public void HashEvaluation(string path)
        {
            this.Where(x => x.FilePath == path).ToList().ForEach(x =>
            {
                x.Md5HashEvaluation();
            });            
        }
        public void SaveToFile()
        {
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(this));
        }        
        public void DeleteUnUsedMetaData()
        {            
            List<PosterMetaData> deleteMetaData = new List<PosterMetaData>();            
            for (int i = 0; i < Count; i++)
            {
                if (!File.Exists(this[i].FilePath))
                {
                    deleteMetaData.Add(this[i]);
                }
            }
            deleteMetaData.ForEach(x =>
            {
                Remove(x);
            });
        }
        public static PosterMetaDataCollection FromFile(string filePath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? throw new InvalidOperationException());
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }

            string file = File.ReadAllText(filePath);
            file = string.IsNullOrEmpty(file) ? JsonConvert.SerializeObject(new PosterMetaDataCollection()) : file;
            PosterMetaDataCollection posterMetaData = JsonConvert.DeserializeObject<PosterMetaDataCollection>(file, new JsonSerializerSettings());            
            posterMetaData.FilePath = filePath;            
            return posterMetaData;
        }
    }   
}
