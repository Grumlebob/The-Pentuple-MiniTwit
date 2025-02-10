namespace MiniTwit.Shared.DTO.Timeline;

public record GetUserResponse
{
    public int UserId { get; init; }
    public string Username { get; init; } = string.Empty;
}
