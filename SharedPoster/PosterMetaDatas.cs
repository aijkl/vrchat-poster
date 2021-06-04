using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Aijkl.VRChat.Posters.Shared
{    
    public class PosterMetaDatas : List<PosterMetaData>
    {
        [JsonIgnore]
        public string FilePath { set; get; }
        public PosterMetaData this[string path]
        {
            get 
            {
                return this.Where(x => path == x.FilePath).FirstOrDefault();
            }            
        }                
        public bool Exsists(string path)
        {
            return this.Any(x => x.FilePath == path);
        }        
        public void HashEvaluation(string path)
        {
            this.Where(x => x.FilePath == path).ToList().ForEach(x =>
            {
                x.MD5HashEvaluation();
            });            
        }
        public void SaveToFile()
        {
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(this));
        }        
        public void DeleteUnUsedMetaData()
        {            
            List<PosterMetaData> deleteMetaDatas = new List<PosterMetaData>();            
            for (int i = 0; i < Count; i++)
            {
                if (!File.Exists(this[i].FilePath))
                {
                    deleteMetaDatas.Add(this[i]);
                }
            }
            deleteMetaDatas.ForEach(x =>
            {
                Remove(x);
            });
        }
        public static PosterMetaDatas FromFile(string directoryPath)
        {
            string file = File.ReadAllText(directoryPath);
            file = string.IsNullOrEmpty(file) ? JsonConvert.SerializeObject(new PosterMetaDatas()) : file;
            PosterMetaDatas posterMetaDatas = JsonConvert.DeserializeObject<PosterMetaDatas>(file, new JsonSerializerSettings());            
            posterMetaDatas.FilePath = directoryPath;            
            return posterMetaDatas;
        }
    }   
}
