using Microsoft.AspNetCore.Mvc;
using MiniTwit.Shared.DTO.Users.Authentication.LoginUser;
using MiniTwit.Shared.DTO.Users.Authentication.LogoutUser;
using MiniTwit.Shared.DTO.Users.Authentication.RegisterUser;

namespace MiniTwit.Api.Services.Interfaces;

public interface IUserService
{
    Task<IActionResult> RegisterUserAsync(
        RegisterUserRequest registerRequest,
        int latest,
        CancellationToken cancellationToken
    );
    Task<IActionResult> LoginAsync(
        LoginUserRequest request,
        int latest,
        CancellationToken cancellationToken
    );

    Task<IActionResult> LogoutUserAsync(int latest, CancellationToken cancellationToken);
}
