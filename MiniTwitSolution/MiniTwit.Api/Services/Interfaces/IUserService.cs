using MiniTwit.Shared.DTO.Users.Authentication.LoginUser;
using MiniTwit.Shared.DTO.Users.Authentication.RegisterUser;

namespace MiniTwit.Api.Services.Interfaces;

public interface IUserService
{
    Task<IResult> RegisterUserAsync(
        RegisterUserRequest registerRequest,
        int latest,
        CancellationToken cancellationToken
    );

    Task<IResult> LoginAsync(
        LoginUserRequest request,
        int latest,
        CancellationToken cancellationToken
    );

    Task<IResult> LogoutUserAsync(
        int latest,
        CancellationToken cancellationToken
    );
}