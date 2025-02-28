namespace MiniTwit.Api.Domain;

[Table("latest")]
public class Latest
{
    [Key]
    [Column("id")]
    public int LatestId { get; set; } = 1; // It is always 1, since there is only a latest event.
}
