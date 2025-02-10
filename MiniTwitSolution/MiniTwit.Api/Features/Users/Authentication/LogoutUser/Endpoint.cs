using MiniTwit.Shared.DTO.Users.Authentication.LogoutUser;

namespace MiniTwit.Api.Features.Users.Authentication.LogoutUser;

public static class LogoutUserEndpoints
{
    public static IEndpointRouteBuilder MapLogoutUserEndpoints(this IEndpointRouteBuilder routes)
    {
        // For demonstration, we use POST /logout.
        routes.MapPost("/logout", (HttpContext context) =>
        {
            // In a real application, you might clear authentication cookies or invalidate a token.
            var responseDto = new LogoutUserResponse(true, "Logged out successfully.");
            return Results.Ok(responseDto);
        });

        return routes;
    }
}