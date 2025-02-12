using MiniTwit.Shared.DTO.Users.Authentication.LoginUser;

namespace MiniTwit.Api.Features.Users.Authentication.LoginUser;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapLoginUserEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapPost(
            "/loginerere",
            async (LoginUserRequest request, MiniTwitDbContext db) =>
            {
                // Find the user by email.
                var user = await db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (user is null)
                {
                    return Results.NotFound("User not found.");
                }

                // Verify the password. (For demonstration, we check equality.)
                if (user.PwHash != request.Password)
                {
                    return Results.Unauthorized();
                }
                
                return Results.Redirect($"/{user.Username}");
            }
        );

        return routes;
    }
}
