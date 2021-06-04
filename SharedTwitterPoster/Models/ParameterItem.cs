using Newtonsoft.Json;

namespace Aijkl.VRChat.Posters.Shared.Twitter.Models
{
    public class ParameterItem
    {        
        public ParameterItem()
        {

        }
        public ParameterItem(long id, string dueDate)
        {
            Id = id;
            DueDate = dueDate;
        }

        [JsonProperty("id")]
        public long Id { get; set; } 
        
        [JsonProperty("dueDate")]
        public string DueDate { get; set; }
    }
    public class Tweet : ParameterItem
    {        
        public Tweet()
        {

        }
        public Tweet(long id, string dueDate = "") : base(id, dueDate)
        {

        }
    }
    public class Account : ParameterItem
    {       
        public Account()
        {

        }
    }
}
