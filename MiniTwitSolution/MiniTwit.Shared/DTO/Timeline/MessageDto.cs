namespace MiniTwit.Shared.DTO.Timeline;

public record MessageDto
{
    public int MessageId { get; init; }
    public string Text { get; init; } = string.Empty;
    public int? PubDate { get; init; }
    public UserDto? Author { get; init; }
}
