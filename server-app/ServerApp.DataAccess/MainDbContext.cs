using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ServerApp.DataAccess.Entities;

namespace ServerApp.DataAccess;

public class MainDbContext : IdentityDbContext<UserProfileEntity>
{
    public DbSet<ProfileImageEntity> ProfileImages { get; set; }
    public DbSet<ChatImageEntity> ChatImages { get; set; }
    public DbSet<ChatEntity> Chats { get; set; }
    public DbSet<MessageEntity> Messages { get; set; }


    public MainDbContext(DbContextOptions<MainDbContext> options) :
        base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<ProfileImageEntity>()
            .HasKey(x => x.Id);
        builder.Entity<ProfileImageEntity>()
            .HasOne(x => x.Profile)
            .WithOne()
            .HasForeignKey<ProfileImageEntity>(x => x.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UserProfileEntity>()
            .HasOne(x => x.Image)
            .WithOne()
            .HasForeignKey<UserProfileEntity>(x => x.ImageId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<ChatImageEntity>()
            .HasKey(x => x.Id);

        builder.Entity<ChatEntity>()
            .HasKey(x => x.Id);
        builder.Entity<ChatEntity>()
            .HasOne(x => x.Image) // Chat has one img
            .WithOne(x=>x.Chat) // Image belongs to one chat
            .HasForeignKey<ChatImageEntity>(x => x.ChatId)
            .OnDelete(DeleteBehavior.Cascade); // delete image when chat it deleted

        builder.Entity<MessageEntity>()
            .HasKey(x => x.Id);
        builder.Entity<MessageEntity>()
            .HasOne(x => x.Chat)
            .WithMany()
            .HasForeignKey(x => x.ChatId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Entity<MessageEntity>()
            .HasOne(x => x.Sender)
            .WithMany()
            .HasForeignKey(x=>x.SenderId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        base.OnModelCreating(builder);
    }    
}
