namespace SF.PJ_03.SocialNetwork.ViewModels.Account
{
    public class SearchViewModel
    {
        public List<UserWithFriendExt>? UserList { get; set; }

        public SearchViewModel()
        {
            UserList = new List<UserWithFriendExt>();
        }
    }
}
