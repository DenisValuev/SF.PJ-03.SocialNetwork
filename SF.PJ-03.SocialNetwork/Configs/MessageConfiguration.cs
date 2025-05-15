using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SF.PJ_03.SocialNetwork.Models.Users;

namespace SF.PJ_03.SocialNetwork.Configs
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("Mesages").HasKey(p => p.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
            builder
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
