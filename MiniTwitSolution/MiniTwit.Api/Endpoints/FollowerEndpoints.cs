using Microsoft.AspNetCore.Mvc;
using MiniTwit.Api.Services.Interfaces;
using MiniTwit.Shared.DTO.Followers.FollowUser;

namespace MiniTwit.Api.Endpoints;

public static class FollowerEndpoints
{
    public static IEndpointRouteBuilder MapFollowerEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/fllws/{username}", async (
            string username,
            IFollowerService followerService,
            CancellationToken cancellationToken,
            [FromQuery] int no = 100,
            [FromQuery(Name = "latest")] int latest = -1
        ) => await followerService.GetFollowersAsync(username, no, latest, cancellationToken));

        endpoints.MapPost("/fllws/{username}", async (
            string username,
            FollowOrUnfollowRequest request,
            IFollowerService followerService,
            CancellationToken cancellationToken,
            [FromQuery(Name = "latest")] int latest = -1
        ) => await followerService.FollowOrUnfollowAsync(username, request, latest, cancellationToken));

        return endpoints;
    }
}