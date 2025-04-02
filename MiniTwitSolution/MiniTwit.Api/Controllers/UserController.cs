using Microsoft.AspNetCore.Mvc;
using MiniTwit.Api.Services.Interfaces;
using MiniTwit.Api.Utility;
using MiniTwit.Shared.DTO.Users.Authentication.LoginUser;

namespace MiniTwit.Api.Controllers;

[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginUserRequest request,
        [FromQuery] int latest = -1,
        CancellationToken cancellationToken = default
        )
    {
        var response = await _userService.LoginUserAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        return StatusCode((int)response.StatusCode, content);
    }
}