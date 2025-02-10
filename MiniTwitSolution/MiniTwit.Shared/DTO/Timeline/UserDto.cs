namespace MiniTwit.Shared.DTO.Timeline;

public record UserDto
{
    public int UserId { get; init; }
    public string Username { get; init; } = string.Empty;
}
