using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using MiniTwit.Api.Infrastructure;
using MiniTwit.Api.Services.Interfaces;
using MiniTwit.Api.Utility;
using MiniTwit.Shared.DTO.Users.Authentication.LoginUser;
using MiniTwit.Shared.DTO.Users.Authentication.RegisterUser;

namespace MiniTwit.Api.Services;

public class UserService : IUserService
{
    private readonly MiniTwitDbContext _db;
    private readonly HybridCache _hybridCache;

    public UserService(MiniTwitDbContext db, HybridCache hybridCache)
    {
        _db = db;
        _hybridCache = hybridCache;
    }

    public async Task<HttpResponseMessage> LoginUserAsync(
        LoginUserRequest request,
        int latest,
        CancellationToken cancellationToken
    )
    {
        var response = await LoginUserAsync(request);
        await UpdateLatest.UpdateLatestStateAsync(
            latest,
            _db,       
            _hybridCache,    
            cancellationToken
        );
        return response;
    }

    public async Task<HttpResponseMessage> LoginUserAsync(
        LoginUserRequest request
        )
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
        if (user is null)
        {
            return new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("User not found.")
            };
        }

        if (user.PwHash != request.Password)
        {
            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }

        var token = "fake-token";

        var responseDto = new LoginUserResponse(
            user.UserId,
            user.Username,
            user.Email,
            token
        );

        await UpdateLatest.UpdateLatestStateAsync(
            latest: -1,
            _db,
            _hybridCache,
            cancellationToken: CancellationToken.None
        );

        var json = JsonSerializer.Serialize(responseDto);
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
    }
    
    public Task<HttpResponseMessage> RegisterUserAsync(RegisterUserRequest registerRequest) =>
        Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotImplemented));

    public Task<HttpResponseMessage> LogoutUserAsync() =>
        Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotImplemented));
}
