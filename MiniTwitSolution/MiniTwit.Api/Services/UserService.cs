using Microsoft.Extensions.Caching.Hybrid;
using MiniTwit.Api.Services.Interfaces;
using MiniTwit.Api.Utility;
using MiniTwit.Shared.DTO.Users.Authentication.LoginUser;
using MiniTwit.Shared.DTO.Users.Authentication.LogoutUser;
using MiniTwit.Shared.DTO.Users.Authentication.RegisterUser;

namespace MiniTwit.Api.Services;

public class UserService(MiniTwitDbContext db, HybridCache hybridCache) : IUserService
{
    public async Task<IResult> LoginAsync(
        LoginUserRequest request,
        int latest,
        CancellationToken cancellationToken
    )
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken);
        if (user is null)
        {
            return Results.NotFound("User not found.");
        }

        if (user.PwHash != request.Password)
        {
            return Results.Unauthorized();
        }

        // Fake token for demo purposes
        var token = "fake-token";
        var responseDto = new LoginUserResponse(user.UserId, user.Username, user.Email, token);

        await UpdateLatest.UpdateLatestStateAsync(latest, db, hybridCache, cancellationToken);

        return Results.Ok(responseDto);
    }

    public async Task<IResult> RegisterUserAsync(
        RegisterUserRequest registerRequest,
        int latest,
        CancellationToken cancellationToken
    )
    {
        var existingUser = await db.Users.FirstOrDefaultAsync(
            u => u.Email == registerRequest.Email || u.Username == registerRequest.Username,
            cancellationToken
        );

        if (existingUser is not null)
        {
            return Results.Conflict("A user with the same email or username already exists.");
        }

        var newUser = new User
        {
            Username = registerRequest.Username,
            Email = registerRequest.Email,
            PwHash = registerRequest.Password,
        };

        db.Users.Add(newUser);
        await db.SaveChangesAsync(cancellationToken);

        await UpdateLatest.UpdateLatestStateAsync(latest, db, hybridCache, cancellationToken);

        return Results.NoContent();
    }

    public async Task<IResult> LogoutUserAsync(
        int latest,
        CancellationToken cancellationToken
    )
    {
        var responseDto = new LogoutUserResponse(true, "Logged out successfully.");

        await UpdateLatest.UpdateLatestStateAsync(latest, db, hybridCache, cancellationToken);

        return Results.Ok(responseDto);
    }
}
