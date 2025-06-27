using Microsoft.EntityFrameworkCore;
using net_chat.Model;

namespace net_chat.Services
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Account> Accounts { get; set; }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Friend> Friends { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostReaction> PostReactions { get; set; }
        public DbSet<Comment> Comments { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.UserProfile)
                .WithMany()
                .HasForeignKey(c => c.UserId);
        }

    }
}
