namespace MiniTwit.Shared.DTO.Timeline;

public record GetMessageResponse(
    int MessageId,
    int? PubDate,
    string? AuthorUsername,
    string Text = ""
);
