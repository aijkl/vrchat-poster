namespace Aijkl.VRChat.Posters.Shared
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using LinkPreview.API;
    using Expansion;
    using Aijkl.VRChat.SharedPoster.Expansion;
    using Newtonsoft.Json;
    using SkiaSharp;

    public class LocalCache
    {
        private readonly DirectoryInfo imageDirectoryInfo;
        private readonly DirectoryInfo translationDirectoryInfo;
        private readonly DirectoryInfo linkPreviewDirectoryInfo;
        private readonly List<string> useImages;
        private readonly List<string> useTranslations;
        private readonly List<string> useLinkPreviews;        

        public LocalCache(string directory)
        {
            imageDirectoryInfo = Directory.CreateDirectory($"{directory}{Path.DirectorySeparatorChar}image");
            translationDirectoryInfo = Directory.CreateDirectory($"{directory}{Path.DirectorySeparatorChar}translation");
            linkPreviewDirectoryInfo = Directory.CreateDirectory($"{directory}{Path.DirectorySeparatorChar}linkPreview");
            useImages = new List<string>();
            useTranslations = new List<string>();
            useLinkPreviews = new List<string>();
        }

        public bool ImageExists(string fileName)
        {
            return File.Exists($"{imageDirectoryInfo.FullName}{Path.DirectorySeparatorChar}{fileName.ToHash()}");
        }

        public bool TranslationExists(string fileName)
        {
            return File.Exists($"{translationDirectoryInfo.FullName}{Path.DirectorySeparatorChar}{fileName}");
        }

        public bool LinkPreviewExists(string url)
        {
            return File.Exists($"{linkPreviewDirectoryInfo.FullName}{Path.DirectorySeparatorChar}{ToHash(url)}");
        }

        public bool GetImage(string fileName, out SKBitmap bitmap)
        {
            string id = fileName.ToHash();
            bitmap = null;
            string path = $"{imageDirectoryInfo.FullName}{Path.DirectorySeparatorChar}{id}";
            if (!File.Exists(path)) return false;
            bitmap = SKBitmap.Decode(path);
            if (!useImages.Contains(id)) useImages.Add(id);
            return true;
        }

        public void AddImage(string fileName, SKBitmap bitmap)
        {
            string id = fileName.ToHash();
            bitmap.Save($"{imageDirectoryInfo.FullName}{Path.DirectorySeparatorChar}{id}");
            if (!useImages.Contains(id)) useImages.Add(id);
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

        public bool GetLinkPreview(string url, out Response response)
        {
            string hashedUrl = ToHash(url);

            response = null;
            string path = $"{linkPreviewDirectoryInfo.FullName}{Path.DirectorySeparatorChar}{hashedUrl}";
            if (!File.Exists(path)) return false;
            response = JsonConvert.DeserializeObject<Response>(File.ReadAllText(path));
            if (!useLinkPreviews.Contains(hashedUrl)) useLinkPreviews.Add(hashedUrl);
            return true;
        }

        public void AddLinkPreview(string url, Response response)
        {
            string hashedUrl = ToHash(url);

            string path = $"{linkPreviewDirectoryInfo.FullName}{Path.DirectorySeparatorChar}{hashedUrl}";

            File.WriteAllText(path, JsonConvert.SerializeObject(response));
            if (!useLinkPreviews.Contains(hashedUrl)) useLinkPreviews.Add(hashedUrl);
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
            Directory.GetFiles(linkPreviewDirectoryInfo.FullName).ToList().ForEach(x =>
            {
                if (!useLinkPreviews.Contains(Path.GetFileName(x))) File.Delete(x);
            });
            useImages.Clear();
            useTranslations.Clear();
            useLinkPreviews.Clear();
        }

        private static string ToHash(string url)
        {
            return BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(url))).ToLower().Replace("-", "");            
        }
    }
}
