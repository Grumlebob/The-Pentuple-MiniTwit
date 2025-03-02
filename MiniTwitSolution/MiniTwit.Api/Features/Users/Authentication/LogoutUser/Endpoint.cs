using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Hybrid;
using MiniTwit.Api.Utility;
using MiniTwit.Shared.DTO.Users.Authentication.LogoutUser;

namespace MiniTwit.Api.Features.Users.Authentication.LogoutUser;

public static class LogoutUserEndpoints
{
    public static IEndpointRouteBuilder MapLogoutUserEndpoints(this IEndpointRouteBuilder routes)
    {
        // For demonstration, we use POST /logout.
        routes.MapPost(
            "/logout",
            async (
                HttpContext context,
                MiniTwitDbContext db,
                HybridCache hybridCache,
                CancellationToken cancellationToken,
                [FromQuery] int latest = -1
            ) =>
            {
                // In a real application, you might clear authentication cookies or invalidate a token.
                var responseDto = new LogoutUserResponse(true, "Logged out successfully.");
                await UpdateLatest.UpdateLatestStateAsync(
                    latest,
                    db,
                    hybridCache,
                    cancellationToken
                );
                return Results.Ok(responseDto);
            }
        );

        return routes;
    }
}
