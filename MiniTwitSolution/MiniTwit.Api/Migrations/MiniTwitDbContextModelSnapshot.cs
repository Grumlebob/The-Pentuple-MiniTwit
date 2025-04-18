﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MiniTwit.Api.Infrastructure;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MiniTwit.Api.Migrations
{
    [DbContext(typeof(MiniTwitDbContext))]
    partial class MiniTwitDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("MiniTwit.Api.Domain.Follower", b =>
                {
                    b.Property<int>("WhoId")
                        .HasColumnType("integer")
                        .HasColumnName("who_id")
                        .HasColumnOrder(0);

                    b.Property<int>("WhomId")
                        .HasColumnType("integer")
                        .HasColumnName("whom_id")
                        .HasColumnOrder(1);

                    b.HasKey("WhoId", "WhomId");

                    b.HasIndex("WhomId");

                    b.ToTable("follower", (string)null);
                });

            modelBuilder.Entity("MiniTwit.Api.Domain.Latest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("LatestEventId")
                        .HasColumnType("integer")
                        .HasColumnName("latest_event");

                    b.HasKey("Id");

                    b.ToTable("latest", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            LatestEventId = 0
                        });
                });

            modelBuilder.Entity("MiniTwit.Api.Domain.Message", b =>
                {
                    b.Property<int>("MessageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("message_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("MessageId"));

                    b.Property<int>("AuthorId")
                        .HasColumnType("integer")
                        .HasColumnName("author_id");

                    b.Property<int?>("Flagged")
                        .HasColumnType("integer")
                        .HasColumnName("flagged");

                    b.Property<int?>("PubDate")
                        .HasColumnType("integer")
                        .HasColumnName("pub_date");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("text");

                    b.HasKey("MessageId");

                    b.HasIndex("AuthorId");

                    b.ToTable("message", (string)null);
                });

            modelBuilder.Entity("MiniTwit.Api.Domain.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("UserId"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("email");

                    b.Property<string>("PwHash")
                        .HasColumnType("TEXT")
                        .HasColumnName("pw_hash");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("username");

                    b.HasKey("UserId");

                    b.ToTable("user");
                });

            modelBuilder.Entity("MiniTwit.Api.Domain.Follower", b =>
                {
                    b.HasOne("MiniTwit.Api.Domain.User", "FollowerUser")
                        .WithMany("FollowingLinks")
                        .HasForeignKey("WhoId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("MiniTwit.Api.Domain.User", "FollowedUser")
                        .WithMany("FollowerLinks")
                        .HasForeignKey("WhomId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("FollowedUser");

                    b.Navigation("FollowerUser");
                });

            modelBuilder.Entity("MiniTwit.Api.Domain.Message", b =>
                {
                    b.HasOne("MiniTwit.Api.Domain.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("MiniTwit.Api.Domain.User", b =>
                {
                    b.Navigation("FollowerLinks");

                    b.Navigation("FollowingLinks");
                });
#pragma warning restore 612, 618
        }
    }
}
