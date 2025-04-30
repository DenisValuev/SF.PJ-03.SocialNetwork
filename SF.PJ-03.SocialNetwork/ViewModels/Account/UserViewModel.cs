using SF.PJ_03.SocialNetwork.Models.Users;

namespace SF.PJ_03.SocialNetwork.ViewModels.Account
{
    public class UserViewModel
    {
        public User User { get; set; }

        public UserViewModel(User user)
        {
            User = user;
        }
    }
}
