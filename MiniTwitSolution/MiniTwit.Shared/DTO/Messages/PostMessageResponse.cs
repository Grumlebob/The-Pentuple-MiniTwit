namespace MiniTwit.Shared.DTO.Messages;

public record PostMessageResponse(int MessageId, int AuthorId, string Content, int? PubDate);
