using MiniTwit.Shared.DTO.Users.Authentication.LoginUser;
using MiniTwit.Shared.DTO.Users.Authentication.RegisterUser;

namespace MiniTwit.Shared.EndpointContracts.Users;

public interface IUserServices
{
    Task<HttpResponseMessage> RegisterUserAsync(RegisterUserRequest registerRequest);
    Task<HttpResponseMessage> LoginUserAsync(LoginUserRequest loginRequest);

    Task<HttpResponseMessage> LogoutUserAsync();
}
