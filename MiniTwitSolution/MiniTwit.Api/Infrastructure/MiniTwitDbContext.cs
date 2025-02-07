using Microsoft.EntityFrameworkCore;
using MiniTwit.Api.Domain;

namespace MiniTwit.Api.Infrastructure
{
    public partial class MiniTwitDbContext : DbContext
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
            modelBuilder.Entity<Follower>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("follower");

                entity.Property(e => e.WhoId).HasColumnName("who_id");
                entity.Property(e => e.WhomId).HasColumnName("whom_id");
            });

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
                    .HasColumnType("TEXT")  // SQLite uses TEXT for strings.
                    .HasColumnName("text");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id");
                entity.Property(e => e.Email)
                    .HasColumnType("TEXT")
                    .HasColumnName("email");
                entity.Property(e => e.PwHash)
                    .HasColumnType("TEXT")
                    .HasColumnName("pw_hash");
                entity.Property(e => e.Username)
                    .HasColumnType("TEXT")
                    .HasColumnName("username");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
