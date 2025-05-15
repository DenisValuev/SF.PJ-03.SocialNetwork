using SF.PJ_03.SocialNetwork.Models.Users;

namespace SF.PJ_03.SocialNetwork.ViewModels.Account
{
    public class UserWithFriendExt : User
    {
        public bool IsFriendWithCurrent { get; set; }
        public bool IsCurrentUser { get; set; }
    }
}
