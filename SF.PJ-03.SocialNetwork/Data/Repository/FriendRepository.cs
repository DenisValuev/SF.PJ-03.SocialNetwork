using Microsoft.EntityFrameworkCore;
using SF.PJ_03.SocialNetwork.Models.Users;
using System.Threading.Tasks;

namespace SF.PJ_03.SocialNetwork.Data.Repository
{
    public class FriendsRepository : Repository <Friend>
    {
        public FriendsRepository(ApplicationDbContext db) : base(db)
        {

        }


        public async Task AddFriendAsync(User target, User friend)
        {
            var friends = await Set.FirstOrDefaultAsync(x => x.UserId == target.Id && x.CurrentFriendId == friend.Id);

            if (friends == null)
            {
                var item = new Friend()
                {
                    UserId = target.Id,
                    User = target,
                    CurrentFriend = friend,
                    CurrentFriendId = friend.Id,
                };

                await Create(item);
            }
        }
        public void AddFriend(User target, User Friend)
        {
            var friends = Set.AsEnumerable().FirstOrDefault(x => x.UserId == target.Id && x.CurrentFriendId == Friend.Id);

            if (friends == null)
            {
                var item = new Friend()
                {
                    UserId = target.Id,
                    User = target,
                    CurrentFriend = Friend,
                    CurrentFriendId = Friend.Id,
                };

                Create(item);
            }
        }

        public List<User> GetFriendsByUser(User target)
        {
            var userFriends = Set.AsEnumerable().FirstOrDefault(x => x.UserId == target.Id);
            if (userFriends != null)
            {
                var friends = Set.Include(x => x.CurrentFriend).AsEnumerable().Where(x => x.UserId == target.Id).Select(x => x.CurrentFriend);
                return friends.ToList();
            }

            return new List<User>();
        }


        public void DeleteFriend(User target, User friend)
        {
            var friends = Set.AsEnumerable().FirstOrDefault(x => x.UserId == target.Id && x.CurrentFriendId == friend.Id);

            if (friends != null)
            {
                Delete(friends);
            }
        }
        public async Task DeleteFriendAsync(User target, User friend)
        {
            var friends = await Set.FirstOrDefaultAsync(x => x.UserId == target.Id && x.CurrentFriendId == friend.Id);

            if (friends != null)
            {
                await Delete(friends);
            }
        }

        public async Task ClearFriendsAsync(User user)
        {
            var friends = Set.AsEnumerable().Where(x => x.UserId == user.Id || x.CurrentFriendId == user.Id).ToList();
            foreach (var friend in friends)
            {
                await Delete(friend);
            }
        }
    }
}
