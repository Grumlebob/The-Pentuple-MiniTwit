using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using MiniTwit.Api.Services.Interfaces;
using MiniTwit.Api.Utility;
using MiniTwit.Shared.DTO.Users.Authentication.LoginUser;
using MiniTwit.Shared.DTO.Users.Authentication.LogoutUser;
using MiniTwit.Shared.DTO.Users.Authentication.RegisterUser;
using System.Threading;
using System.Threading.Tasks;

public class UserService : IUserService
{
    private readonly MiniTwitDbContext _db;
    private readonly HybridCache _hybridCache;

    public UserService(MiniTwitDbContext db, HybridCache hybridCache)
    {
        _db = db;
        _hybridCache = hybridCache;
    }

    public async Task<IActionResult> LoginAsync(LoginUserRequest request, int latest, CancellationToken cancellationToken)
    {
        // Find the user by username.
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
        if (user is null)
        {
            return new NotFoundObjectResult("User not found.");
        }

        // Verify the password. (For demonstration, we check equality.)
        if (user.PwHash != request.Password)
        {
            return new UnauthorizedResult();
        }

        // Generate a token (for demonstration, a fake token is returned).
        var token = "fake-token";

        var responseDto = new LoginUserResponse(
            user.UserId,
            user.Username,
            user.Email,
            token
        );

        await UpdateLatest.UpdateLatestStateAsync(latest, _db, _hybridCache, cancellationToken);

        return new OkObjectResult(responseDto);
    }

    public async Task<IActionResult> RegisterUserAsync(RegisterUserRequest registerRequest, int latest, CancellationToken cancellationToken)
    {
        var existingUser = await _db.Users.FirstOrDefaultAsync(u =>
            u.Email == registerRequest.Email || u.Username == registerRequest.Username, cancellationToken);

        if (existingUser is not null)
        {
            return new ConflictObjectResult("A user with the same email or username already exists.");
        }

        // Create a new user. (For demonstration, we use the password directly as the hash.)
        var newUser = new User
        {
            Username = registerRequest.Username,
            Email = registerRequest.Email,
            PwHash = registerRequest.Password,
        };

        _db.Users.Add(newUser);
        await _db.SaveChangesAsync(cancellationToken);

        await UpdateLatest.UpdateLatestStateAsync(latest, _db, _hybridCache, cancellationToken);

        return new NoContentResult();
    }

    public async Task<IActionResult> LogoutUserAsync(int latest, CancellationToken cancellationToken)
    {
        // In a real application, you might clear authentication cookies or invalidate a token.
        var responseDto = new LogoutUserResponse(true, "Logged out successfully.");

        await UpdateLatest.UpdateLatestStateAsync(latest, _db, _hybridCache, cancellationToken);

        return new OkObjectResult(responseDto);
    }
}
