using Microsoft.EntityFrameworkCore;
using SF.PJ_03.SocialNetwork.Models.Users;

namespace SF.PJ_03.SocialNetwork.Data.Repository
{
    public class MessageRepository : Repository<Message>
    {
        public MessageRepository(ApplicationDbContext db) : base(db)
        {
            
        }

        public List<Message> GetMessages(User sender, User recipient)
        {
            Set.Include(x => x.Recipient);
            Set.Include(x => x.Sender);

            var from = Set.AsEnumerable().Where(x => x.SenderId == sender.Id && x.RecipientId == recipient.Id).ToList();
            var to = Set.AsEnumerable().Where(x => x.SenderId == recipient.Id && x.RecipientId == sender.Id).ToList();

            var itog = new List<Message>();
            itog.AddRange(from);
            itog.AddRange(to);
            itog.OrderBy(x => x.Id);
            return itog;
        }

        public async Task ClearMessagesAsync(User user)
        {
            var messages = Set.AsEnumerable().Where(x => x.SenderId == user.Id || x.RecipientId == user.Id).ToList();
            foreach (var message in messages)
            {
                await Delete(message);
            }
        }
    }
}
