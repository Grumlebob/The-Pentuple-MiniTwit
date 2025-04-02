using Microsoft.AspNetCore.Mvc;
using MiniTwit.Api.Services.Interfaces;
using MiniTwit.Api.Utility;
using MiniTwit.Shared.DTO.Users.Authentication.LoginUser;
using MiniTwit.Shared.DTO.Users.Authentication.RegisterUser;

namespace MiniTwit.Api.Controllers;

[ApiController]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginUserRequest request,
        [FromQuery] int latest = -1,
        CancellationToken cancellationToken = default)
    {
        return await userService.LoginAsync(request, latest, cancellationToken);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUserRequest request,
        [FromQuery] int latest = -1,
        CancellationToken cancellationToken = default)
    {
        return await userService.RegisterUserAsync(request, latest, cancellationToken);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
        [FromQuery] int latest = -1,
        CancellationToken cancellationToken = default)
    {
        return await userService.LogoutUserAsync(latest, cancellationToken);
    }
}