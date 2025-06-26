using HobbyHub.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace HobbyHub.Database.Infrastructure;


public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Username=postgres;Password=password;Database=HobbyHub");
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Follow>(entity =>
        {
            entity.HasKey(f => f.FollowId);

            entity.HasOne(f => f.Follower)
                .WithMany(u => u.Following)
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(f => f.Followed)
                .WithMany(u => u.Followers)
                .HasForeignKey(f => f.FollowedId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(f => new { f.FollowerId, f.FollowedId }).IsUnique();
        });
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Note> Notes { get; set; }
    public DbSet<Hobby> Hobbies { get; set; }
    public DbSet<UserHobby> UserHobbies { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Activity> Activities { get; set; }
    public DbSet<ActivityPicture> ActivityPictures { get; set; }
    public DbSet<UserIdentity> UserIdentities { get; set; }
    public DbSet<FavoriteHobby> FavoriteHobbies { get; set; }
    public DbSet<GroupPost> GroupPosts { get; set; }
    public DbSet<GroupPostPicture> GroupPostPictures { get; set; }
    public DbSet<Follow> Follows { get; set; }
    public DbSet<MarketplaceItem> MarketplaceItems { get; set; }
    public DbSet<MarketplaceItemPicture> MarketplaceItemPictures { get; set; }
    public DbSet<Tutorial> Tutorials { get; set; }
    public DbSet<TutorialStep> TutorialSteps { get; set; }
    public DbSet<TutorialStepPicture> TutorialStepPictures { get; set; }
    public DbSet<SavedMarketplaceItem> SavedMarketplaceItems { get; set; }
}