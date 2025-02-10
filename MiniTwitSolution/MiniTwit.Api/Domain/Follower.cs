namespace MiniTwit.Api.Domain;

[Table("follower")]
public partial class Follower
{
    [Key]
    [Column("who_id", Order = 0)]
    public int WhoId { get; set; } // The user who follows

    [Key]
    [Column("whom_id", Order = 1)]
    public int WhomId { get; set; } // The user being followed

    // Navigation properties
    [ForeignKey(nameof(WhoId))]
    public virtual User FollowerUser { get; set; } = null!;

    [ForeignKey(nameof(WhomId))]
    public virtual User FollowedUser { get; set; } = null!;
}
