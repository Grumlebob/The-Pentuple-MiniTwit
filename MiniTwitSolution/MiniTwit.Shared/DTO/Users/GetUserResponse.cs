namespace MiniTwit.Shared.DTO.Users;

public record GetUserResponse
{
    public int UserId { get; init; }
    public string Username { get; init; } = string.Empty;
}
