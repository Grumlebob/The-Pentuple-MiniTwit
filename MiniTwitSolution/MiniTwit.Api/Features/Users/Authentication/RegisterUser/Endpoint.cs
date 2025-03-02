using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Hybrid;
using MiniTwit.Api.Utility;
using MiniTwit.Shared.DTO.Users.Authentication.RegisterUser;

namespace MiniTwit.Api.Features.Users.Authentication.RegisterUser;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapRegisterUserEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapPost(
            "/register",
            async (
                RegisterUserRequest request,
                MiniTwitDbContext db,
                HybridCache hybridCache,
                CancellationToken cancellationToken,
                [FromQuery] int latest = -1
            ) =>
            {
                // Check if a user with the same email or username already exists.
                var existingUser = await db.Users.FirstOrDefaultAsync(u =>
                    u.Email == request.Email || u.Username == request.Username
                );
                if (existingUser is not null)
                {
                    return Results.Conflict(
                        "A user with the same email or username already exists."
                    );
                }

                // Create a new user. (For demonstration, we use the password directly as the hash.)
                var newUser = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    PwHash = request.Password,
                };
                db.Users.Add(newUser);
                await db.SaveChangesAsync();

                await UpdateLatest.UpdateLatestStateAsync(
                    latest,
                    db,
                    hybridCache,
                    cancellationToken
                );
                return Results.NoContent();
            }
        );

        return routes;
    }
}
