using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SF.PJ_03.SocialNetwork.Configs;
using SF.PJ_03.SocialNetwork.Models.Users;

namespace SF.PJ_03.SocialNetwork.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration<Friend>(new FriendConfiguration());
            builder.ApplyConfiguration<Message>(new MessageConfiguration());
        }
    }
}
