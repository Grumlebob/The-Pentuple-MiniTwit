using MiniTwit.Shared.DTO.Users.Authentication.RegisterUser;

namespace MiniTwit.Api.Features.Users.Authentication.RegisterUser;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapRegisterUserEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/register", async (RegisterUserRequest request, MiniTwitDbContext db) =>
        {
            // Check if a user with the same email or username already exists.
            var existingUser = await db.Users.FirstOrDefaultAsync(u => u.Email == request.Email || u.Username == request.Username);
            if (existingUser is not null)
            {
                return Results.Conflict("A user with the same email or username already exists.");
            }

            // Create a new user. (For demonstration, we use the password directly as the hash.)
            var newUser = new User
            {
                Username = request.Username,
                Email = request.Email,
                PwHash = request.Password
            };
            db.Users.Add(newUser);
            await db.SaveChangesAsync();

            var responseDto = new RegisterUserResponse(newUser.UserId, newUser.Username, newUser.Email);
            return Results.Created($"/register/{newUser.UserId}", responseDto);
        });

        return routes;
    }
}