namespace MiniTwit.Shared.DTO.Users.Authentication.LoginUser;

public record LoginUserResponse(int UserId, string Username, string Email, string Token);
