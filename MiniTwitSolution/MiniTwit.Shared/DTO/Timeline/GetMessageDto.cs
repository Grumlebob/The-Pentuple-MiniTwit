namespace MiniTwit.Shared.DTO.Timeline;

public record GetMessageDto
{
    public int MessageId { get; init; }
    public string Text { get; init; } = string.Empty;
    public int? PubDate { get; init; }
    public GetUserDto? Author { get; init; }
}
