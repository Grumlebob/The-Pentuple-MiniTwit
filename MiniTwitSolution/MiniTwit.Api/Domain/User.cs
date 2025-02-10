using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniTwit.Api.Domain;

[Table("user")]
public partial class User
{
    [Key]
    [Column("user_id")]
    public int UserId { get; set; }

    [Column("username", TypeName = "TEXT")]
    public string Username { get; set; } = null!;

    [Column("email", TypeName = "TEXT")]
    public string Email { get; set; } = null!;

    [Column("pw_hash", TypeName = "TEXT")]
    public string PwHash { get; set; } = null!;

    // Navigation property: All follower records where this user is being followed.
    [InverseProperty(nameof(Follower.FollowedUser))]
    public virtual ICollection<Follower> FollowerLinks { get; set; } = new List<Follower>();

    // Navigation property: All follower records where this user is the follower.
    [InverseProperty(nameof(Follower.FollowerUser))]
    public virtual ICollection<Follower> FollowingLinks { get; set; } = new List<Follower>();
}