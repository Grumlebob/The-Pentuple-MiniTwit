using MiniTwit.Shared.DTO.Users.Authentication.LoginUser;

namespace MiniTwit.Api.Features.Users.Authentication.LoginUser;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapLoginUserEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapPost(
            "/login",
            async (LoginUserRequest request, MiniTwitDbContext db) =>
            {
                // Find the user by username.
                var user = await db.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
                if (user is null)
                {
                    return Results.NotFound("User not found.");
                }

                // Verify the password. (For demonstration, we check equality.)
                if (user.PwHash != request.Password)
                {
                    return Results.Unauthorized();
                }

                // Generate a token (for demonstration, a fake token is returned).
                var token = "fake-token";

                var responseDto = new LoginUserResponse(
                    user.UserId,
                    user.Username,
                    user.Email,
                    token
                );
                return Results.Ok(responseDto);
            }
        );

        return routes;
    }
}
