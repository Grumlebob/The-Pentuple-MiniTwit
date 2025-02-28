using MiniTwit.Shared.DTO.Users.Authentication.LogoutUser;
using MiniTwit.Api.Utility;
using Microsoft.AspNetCore.Mvc;

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
                [FromQuery] int latest = -1
            ) =>
            {
                // In a real application, you might clear authentication cookies or invalidate a token.
                var responseDto = new LogoutUserResponse(true, "Logged out successfully.");
                await UpdateLatest.UpdateLatestStateAsync(latest, db);
                return Results.Ok(responseDto);
            }
        );

        return routes;
    }
}
