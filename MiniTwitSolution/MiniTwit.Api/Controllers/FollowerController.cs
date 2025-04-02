using Microsoft.AspNetCore.Mvc;
using MiniTwit.Api.Services.Interfaces;
using MiniTwit.Shared.DTO.Followers.FollowUser;

namespace MiniTwit.Api.Controllers;

[ApiController]
[Route("fllws")]
public class FollowerController(IFollowerService followerService) : ControllerBase
{
    [HttpGet("{username}")]
    public async Task<IActionResult> GetFollowers(
        string username,
        [FromQuery] int no = 100,
        [FromQuery] int latest = -1,
        CancellationToken cancellationToken = default
    )
    {
        return await followerService.GetFollowersAsync(username, no, latest, cancellationToken);
    }

    [HttpPost("{username}")]
    public async Task<IActionResult> FollowOrUnfollow(
        string username,
        [FromBody] FollowOrUnfollowRequest request,
        [FromQuery] int latest = -1,
        CancellationToken cancellationToken = default
    )
    {
        return await followerService.FollowOrUnfollowAsync(
            username,
            request,
            latest,
            cancellationToken
        );
    }
}
