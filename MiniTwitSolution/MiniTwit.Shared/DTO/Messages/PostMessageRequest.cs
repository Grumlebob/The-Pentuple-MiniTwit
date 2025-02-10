namespace MiniTwit.Shared.DTO.Messages;

public record PostMessageRequest(int AuthorId, string Text, int? PubDate);
