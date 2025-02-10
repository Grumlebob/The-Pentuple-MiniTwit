namespace MiniTwit.Api.Domain;

[Table("message")]
public partial class Message
{
    [Key]
    [Column("message_id")]
    public int MessageId { get; set; }

    [Column("author_id")]
    public int AuthorId { get; set; }

    [Column("text", TypeName = "TEXT")]
    public string Text { get; set; } = null!;

    [Column("pub_date")]
    public int? PubDate { get; set; }

    [Column("flagged")]
    public int? Flagged { get; set; }

    // Navigation property to the author.
    public virtual User? Author { get; set; }
}
