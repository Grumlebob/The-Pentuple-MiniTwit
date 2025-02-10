namespace MiniTwit.Shared.DTO.Timeline;

public record GetMessageResponse
{
    public int MessageId { get; init; }
    public string Text { get; init; } = string.Empty;
    public int? PubDate { get; init; }
    public GetUserResponse? Author { get; init; }
}
