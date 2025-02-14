namespace MiniTwit.Shared.DTO.Messages;

public record PostMessageRequest(string AuthorUsername, string Text, int? PubDate);
