namespace MiniTwit.Shared.DTO.Timeline;

public record GetUserDto
{
    public int UserId { get; init; }
    public string Username { get; init; } = string.Empty;
}
