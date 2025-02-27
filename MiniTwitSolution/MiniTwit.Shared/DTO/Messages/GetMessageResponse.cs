namespace MiniTwit.Shared.DTO.Messages;

public record GetMessageResponse(
    int MessageId,
    int? PubDate,
    string? User,
    string Content = ""
);
