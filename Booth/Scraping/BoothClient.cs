using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;

namespace Aijkl.VRChat.Posters.Booth.Scraping
{
    public class BoothClient
    {
        private readonly HttpClient httpClient;
        private readonly HtmlParser parser;                
        public BoothClient(HttpClient httpClient = null)
        {
            this.httpClient = httpClient ?? new HttpClient(new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.All });
            parser = new HtmlParser();                        
        }
        public enum Sort
        {            
            New,
            Popular,
            PriceDesc,
            PriceAsc
        }
        public SearchResult<Product> SearchProducts(string keyWord, Sort sort, int page = 0)
        {
            var uriBuilder = new UriBuilder($"{Request.BASE_URL}/ja/search/{keyWord}");
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["page"] = page.ToString();
            parameters["sort"] = sort.ToString().ToLower();
            uriBuilder.Query = parameters.ToString();

            List<Product> products = new List<Product>();
            var doc = parser.ParseDocument(DownLoadPage(uriBuilder.Uri.AbsoluteUri));
            foreach (var product in doc.GetElementsByClassName("item-card l-card"))
            {
                string titlte = product.GetElementsByClassName("item-card__title-anchor--multiline nav").FirstOrDefault().TextContent;
                string shopName = product.GetElementsByClassName("item-card__shop-name").FirstOrDefault().TextContent;
                string price = product.GetElementsByClassName("price u-text-primary u-text-left u-tpg-caption2").FirstOrDefault().TextContent;
                string category = product.GetElementsByClassName("item-card__category-anchor nav-reverse").FirstOrDefault().TextContent;
                string imageUrl = product.GetElementsByClassName("js-thumbnail-image item-card__thumbnail-image").FirstOrDefault().GetAttribute("data-original");
                string iconURL = product.GetElementsByClassName("user-avatar at-item-footer").FirstOrDefault().GetAttribute("src");
                int id = int.Parse(product.GetAttribute("data-product-id"));
                int likeCount = int.Parse(product.GetElementsByClassName("count").FirstOrDefault().TextContent);                

                Shop shop = new Shop(iconURL, shopName);
                products.Add(new Product(id, titlte, category, keyWord, likeCount, price, imageUrl, shop));
            }                                                           
            return new SearchResult<Product>(products);
        }
        public Bitmap DownloadImage(string url)
        {
            HttpRequestMessage httpRequestMessage = Request.CreateGetRequest(url, Request.FetchSite.CrossSite, Request.FetchMode.NoCors, Request.FetchDest.Image, upgradeInsecure: 1);
            using HttpResponseMessage httpResponseMessage = httpClient.SendAsync(httpRequestMessage).Result;
            return new Bitmap(httpResponseMessage.Content.ReadAsStreamAsync().Result);
        }
        private string DownLoadPage(string url)
        {
            HttpRequestMessage httpRequestMessage = Request.CreateGetRequest(url, Request.FetchSite.None, Request.FetchMode.Navigate, Request.FetchDest.Document, Request.FetchUser.nonUserActive, upgradeInsecure: 1);            
            return httpClient.SendAsync(httpRequestMessage).Result.Content.ReadAsStringAsync().Result;
        }                
    }
}
