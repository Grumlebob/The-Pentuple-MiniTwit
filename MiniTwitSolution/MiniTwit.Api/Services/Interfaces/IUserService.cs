using MiniTwit.Shared.DTO.Users.Authentication.LoginUser;
using MiniTwit.Shared.DTO.Users.Authentication.RegisterUser;

namespace MiniTwit.Api.Services.Interfaces;

public interface IUserService
{
    Task<HttpResponseMessage> RegisterUserAsync(RegisterUserRequest registerRequest);
    Task<HttpResponseMessage> LoginUserAsync(LoginUserRequest loginRequest);

    Task<HttpResponseMessage> LogoutUserAsync();
}