namespace MiniTwit.Api.Infrastructure;

public interface IMiniTwitDbContext
{
    DbSet<Follower> Followers { get; set; }
    DbSet<Message> Messages { get; set; }
    DbSet<User> Users { get; set; }
}

public partial class MiniTwitDbContext : DbContext, IMiniTwitDbContext
{
    public MiniTwitDbContext(DbContextOptions<MiniTwitDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Follower> Followers { get; set; }
    public virtual DbSet<Message> Messages { get; set; }
    public virtual DbSet<User> Users { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure Follower as an explicit join entity.
        modelBuilder.Entity<Follower>(entity =>
        {
            entity.ToTable("follower");

            // Define composite primary key
            entity.HasKey(e => new { e.WhoId, e.WhomId });

            entity.Property(e => e.WhoId)
                .HasColumnName("who_id")
                .IsRequired();

            entity.Property(e => e.WhomId)
                .HasColumnName("whom_id")
                .IsRequired();

            // Configure relationship:
            // A Follower has one FollowerUser (the user doing the following)
            // and many Follower records can reference the same user in that role.
            entity.HasOne(f => f.FollowerUser)
                .WithMany(u => u.FollowingLinks)
                .HasForeignKey(f => f.WhoId)
                .OnDelete(DeleteBehavior.Restrict);

            // A Follower has one FollowedUser (the user being followed)
            // and many Follower records can reference the same user in that role.
            entity.HasOne(f => f.FollowedUser)
                .WithMany(u => u.FollowerLinks)
                .HasForeignKey(f => f.WhomId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Message entity.
        modelBuilder.Entity<Message>(entity =>
        {
            entity.ToTable("message");

            entity.Property(e => e.MessageId)
                .HasColumnName("message_id");

            entity.Property(e => e.AuthorId)
                .HasColumnName("author_id");

            entity.Property(e => e.Flagged)
                .HasColumnName("flagged");

            entity.Property(e => e.PubDate)
                .HasColumnName("pub_date");

            entity.Property(e => e.Text)
                .HasColumnName("text")
                .HasColumnType("TEXT");

            entity.HasOne(m => m.Author)
                .WithMany() // or .WithMany(u => u.Messages) if you add such a navigation property to User.
                .HasForeignKey(m => m.AuthorId);
        });

        // The User entity is already configured via data annotations.
        // (You can add additional configuration here if needed.)

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}