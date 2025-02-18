namespace MiniTwit.Shared.DTO.Messages;

public record GetMessageResponse(
    int MessageId,
    int? PubDate,
    string? AuthorUsername,
    string Text = ""
);
