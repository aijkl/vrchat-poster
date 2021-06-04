namespace Aijkl.VRChat.Posters.Booth.Scraping
{
    public class Product
    {
        public Product(int id, string title, string category, string keyWord,int likes, string price, string thumbnailUrl, Shop shop)
        {
            Id = id;
            Title = title;
            Category = category;
            KeyWord = keyWord;
            Like = likes;
            Price = price;
            ThumbnailUrl = thumbnailUrl;
            Shop = shop;
        }        
        public int Id { set; get; }
        public string KeyWord { set; get; }
        public string Title { get; set; }
        public string Category { get; set; }
        public int Like { get; set; }
        public string Price { get; set; }
        public string ThumbnailUrl { get; set; }
        public Shop Shop { get; set; }        
    }
}
