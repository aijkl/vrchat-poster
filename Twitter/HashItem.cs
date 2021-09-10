using CoreTweet;

namespace Aijkl.VRChat.Posters.Twitter
{
    public class HashItem
    {
        private HashItem()
        {

        }
        public string UserName { private set; get; }
        public string ProfileImageUrl { private set; get; }
        public string Text { private set; get; }
        public static HashItem CreateFromStatus(Status status)
        {
            return new HashItem()
            {
                UserName = status.User.Name,
                ProfileImageUrl = status.User.ProfileImageUrl,
                Text = status.FullText
            };
        }
    }
}
