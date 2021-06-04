using System.Collections.Generic;

namespace Aijkl.VRChat.Posters.Booth.Scraping
{
    public class SearchResult<T> : List<T>
    {
        public string SearchKeyWorld { set; get; }
        public SearchResult(List<T> list) : base (list)
        {            
        }        
    }
}
