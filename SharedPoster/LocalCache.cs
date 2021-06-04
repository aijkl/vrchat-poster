using System.IO;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using SkiaSharp;
using Aijkl.VRChat.SharedPoster.Expansion;

namespace Aijkl.VRChat.Posters.Shared
{
    public class LocalCache
    {
        private readonly DirectoryInfo imageDirectoryInfo;
        private readonly DirectoryInfo translationDirectoryInfo;
        private readonly List<string> useImages;
        private readonly List<string> useTranslations;
        public enum ContentType
        {
            Image,
            Translation
        }
        public LocalCache(string directory)
        {
            imageDirectoryInfo = Directory.CreateDirectory($"{directory}{Path.DirectorySeparatorChar}image");
            translationDirectoryInfo = Directory.CreateDirectory($"{directory}{Path.DirectorySeparatorChar}translation");
            useImages = new List<string>();
            useTranslations = new List<string>();            
        }        
        public bool ImageExists(string fileName)
        {
            return File.Exists($"{imageDirectoryInfo.FullName}{Path.DirectorySeparatorChar}{fileName}");
        }
        public bool TranslationExists(string fileName)
        {
            return File.Exists($"{translationDirectoryInfo.FullName}{Path.DirectorySeparatorChar}{fileName}");
        }
        public bool GetImage(string fileName, out SKBitmap bitmap)
        {
            bitmap = null;
            string path = $"{imageDirectoryInfo.FullName}{Path.DirectorySeparatorChar}{fileName}";
            if (!File.Exists(path)) return false;
            bitmap = SKBitmap.Decode(path);
            if (!useImages.Contains(fileName)) useImages.Add(fileName);
            return true;
        }
        public void AddImage(string fileName, SKBitmap bitmap)
        {
            bitmap.Save($"{imageDirectoryInfo.FullName}{Path.DirectorySeparatorChar}{fileName}");
            if (!useImages.Contains(fileName)) useImages.Add(fileName);
        }
        public bool GetTranslation(long tweetId, string lang, out string text)
        {
            text = string.Empty;
            string fileName = $"{tweetId}_lang={lang}";
            string path = $"{translationDirectoryInfo.FullName}{Path.DirectorySeparatorChar}{fileName}";

            if (!File.Exists(path)) return false;
            text = File.ReadAllText(path);
            if (!useTranslations.Contains(fileName)) useTranslations.Add(fileName);
            return true;
        }
        public void AddTranslation(long tweetId, string lang, string text)
        {
            string fileName = $"{tweetId}_lang={lang}";
            string path = $"{translationDirectoryInfo.FullName}{Path.DirectorySeparatorChar}{fileName}";

            File.WriteAllText(path, text);
            if (!useTranslations.Contains(fileName)) useTranslations.Add(fileName);
        }
        public void DeleteUnusedCache()
        {            
            Directory.GetFiles(imageDirectoryInfo.FullName).ToList().ForEach(x => 
            {                
                if (!useImages.Contains(Path.GetFileName(x))) File.Delete(x);
            });            
            Directory.GetFiles(translationDirectoryInfo.FullName).ToList().ForEach(x =>
            {
                if (!useTranslations.Contains(Path.GetFileName(x))) File.Delete(x);
            });
            useImages.Clear();
            useTranslations.Clear();
        }
    }
}
