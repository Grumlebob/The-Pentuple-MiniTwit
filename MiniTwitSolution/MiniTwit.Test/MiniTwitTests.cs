using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MiniTwit.Api.Domain;
using MiniTwit.Api.Infrastructure;
using MiniTwit.Shared.DTO.Followers.FollowUser;
using MiniTwit.Shared.DTO.Timeline;
using MiniTwit.Shared.DTO.Users.Authentication.LoginUser;
using MiniTwit.Shared.DTO.Users.Authentication.LogoutUser;
using MiniTwit.Shared.DTO.Users.Authentication.RegisterUser;
using Xunit;

namespace MiniTwit.Test;

//Taken from this guide https://www.youtube.com/watch?v=tj5ZCtvgXKY
[CollectionDefinition("MiniTwitCollection")]
public class MiniTwitCollection : ICollectionFixture<MiniTwitApiWebAppFactory>;

[Collection("MiniTwitCollection")]
public class MiniTwitTests : IAsyncLifetime
{
    private HttpClient _client;

    //Delegate used to call the ResetDatabaseAsync method from the factory, without having to expose the factory
    private readonly Func<Task> _resetDatabase;

    private readonly MiniTwitDbContext _miniTwitContext;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public MiniTwitTests(MiniTwitApiWebAppFactory factory)
    {
        _client = factory.HttpClient;
        _resetDatabase = factory.ResetDatabaseAsync;
        _miniTwitContext = factory.Services.GetRequiredService<MiniTwitDbContext>();
    }

    [Fact]
    public async Task GetPrivateTimeLineWorksAsExpected()
    {
        // Create two users.
        var user1 = new User
        {
            UserId = 1,
            Username = "user1",
            Email = "user1@example.com",
            PwHash = "hash1",
        };
        var user2 = new User
        {
            UserId = 2,
            Username = "user2",
            Email = "user2@example.com",
            PwHash = "hash2",
        };
        _miniTwitContext.Users.AddRange(user1, user2);
        await _miniTwitContext.SaveChangesAsync();

        // Create a follower relationship: user1 follows user2.
        _miniTwitContext.Followers.Add(new Follower { WhoId = 1, WhomId = 2 });
        await _miniTwitContext.SaveChangesAsync();

        // Insert messages for both users.
        var currentTime = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        _miniTwitContext.Messages.Add(
            new Message
            {
                MessageId = 1,
                AuthorId = 1,
                Text = "Hello from user1",
                PubDate = currentTime,
                Flagged = 0,
            }
        );
        _miniTwitContext.Messages.Add(
            new Message
            {
                MessageId = 2,
                AuthorId = 2,
                Text = "Hello from user2",
                PubDate = currentTime,
                Flagged = 0,
            }
        );
        await _miniTwitContext.SaveChangesAsync();

        // --- Act: Call the private timeline endpoint ---
        // The private timeline endpoint is at "/" and expects a query parameter "userId".
        var response = await _client.GetAsync("/?userId=1&offset=0");
        response.EnsureSuccessStatusCode();

        // Read and deserialize the JSON response into a list of DTOs.
        var json = await response.Content.ReadAsStringAsync();
        var dtos = JsonSerializer.Deserialize<List<GetMessageResponse>>(
            json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        Assert.NotNull(dtos);
        // We expect 2 messages: one from user1 and one from user2.
        Assert.Equal(2, dtos!.Count);
    }

    [Fact]
    public async Task GetPublicTimelineWorksAsExpected()
    {
        // Create two users.
        var user1 = new User
        {
            UserId = 1,
            Username = "user1",
            Email = "user1@example.com",
            PwHash = "hash1",
        };
        var user2 = new User
        {
            UserId = 2,
            Username = "user2",
            Email = "user2@example.com",
            PwHash = "hash2",
        };
        _miniTwitContext.Users.AddRange(user1, user2);
        await _miniTwitContext.SaveChangesAsync();

        // Insert messages for both users (both flagged=0).
        var currentTime = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        _miniTwitContext.Messages.Add(
            new Message
            {
                MessageId = 1,
                AuthorId = 1,
                Text = "Hello from user1",
                PubDate = currentTime,
                Flagged = 0,
            }
        );
        _miniTwitContext.Messages.Add(
            new Message
            {
                MessageId = 2,
                AuthorId = 2,
                Text = "Hello from user2",
                PubDate = currentTime,
                Flagged = 0,
            }
        );
        await _miniTwitContext.SaveChangesAsync();

        // --- Act: Call the public timeline endpoint (GET "/public?offset=0") ---
        var response = await _client.GetAsync("/public?offset=0");
        response.EnsureSuccessStatusCode();

        // Read and deserialize the JSON response into a list of DTOs.
        var json = await response.Content.ReadAsStringAsync();
        var dtos = JsonSerializer.Deserialize<List<GetMessageResponse>>(json, _jsonOptions);

        // Assert: Expect 2 messages in the public timeline.
        Assert.NotNull(dtos);
        Assert.Equal(2, dtos!.Count);
    }

    [Fact]
    public async Task GetUserTimelineWorksAsExpected()
    {
        // Create two users.
        var user1 = new User
        {
            UserId = 1,
            Username = "user1",
            Email = "user1@example.com",
            PwHash = "hash1",
        };
        var user2 = new User
        {
            UserId = 2,
            Username = "user2",
            Email = "user2@example.com",
            PwHash = "hash2",
        };
        _miniTwitContext.Users.AddRange(user1, user2);
        await _miniTwitContext.SaveChangesAsync();

        // Insert messages for both users.
        var currentTime = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        _miniTwitContext.Messages.Add(
            new Message
            {
                MessageId = 1,
                AuthorId = 1,
                Text = "Hello from user1",
                PubDate = currentTime,
                Flagged = 0,
            }
        );
        _miniTwitContext.Messages.Add(
            new Message
            {
                MessageId = 2,
                AuthorId = 2,
                Text = "Hello from user2",
                PubDate = currentTime,
                Flagged = 0,
            }
        );
        await _miniTwitContext.SaveChangesAsync();

        // --- Act: Call the user timeline endpoint (GET "/user/1?offset=0") ---
        var response = await _client.GetAsync("/user/1?offset=0");
        response.EnsureSuccessStatusCode();

        // Read and deserialize the JSON response into a list of DTOs.
        var json = await response.Content.ReadAsStringAsync();
        var dtos = JsonSerializer.Deserialize<List<GetMessageResponse>>(json, _jsonOptions);

        // Assert: Expect only messages authored by user 1.
        Assert.NotNull(dtos);
        Assert.Single(dtos!);
        Assert.Equal(1, dtos![0].Author?.UserId);
    }

    [Fact]
    public async Task FollowUserEndpoint_ReturnsCorrectDTO()
    {
        var user1 = new User
        {
            UserId = 1,
            Username = "user1",
            Email = "user1@example.com",
            PwHash = "hash1",
        };
        var user2 = new User
        {
            UserId = 2,
            Username = "user2",
            Email = "user2@example.com",
            PwHash = "hash2",
        };
        _miniTwitContext.Users.AddRange(user1, user2);
        await _miniTwitContext.SaveChangesAsync();

        // Act: Call POST /follow with a JSON body.
        var followRequest = new { FollowerId = 1, FollowedId = 2 };
        var response = await _client.PostAsJsonAsync("/follow", followRequest);
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);

        // Deserialize the response into the DTO.
        var dto = await response.Content.ReadFromJsonAsync<FollowResponse>(_jsonOptions);
        Assert.NotNull(dto);
        Assert.Equal(1, dto!.FollowerId);
        Assert.Equal(2, dto.FollowedId);

        // Verify directly in the database.
        bool exists = await _miniTwitContext.Followers.AnyAsync(f => f.WhoId == 1 && f.WhomId == 2);
        Assert.True(exists);
    }

    [Fact]
    public async Task UnfollowUserEndpoint_ReturnsCorrectDTO()
    {
        var user1 = new User
        {
            UserId = 1,
            Username = "user1",
            Email = "user1@example.com",
            PwHash = "hash1",
        };
        var user2 = new User
        {
            UserId = 2,
            Username = "user2",
            Email = "user2@example.com",
            PwHash = "hash2",
        };
        _miniTwitContext.Users.AddRange(user1, user2);
        await _miniTwitContext.SaveChangesAsync();

        var follow = new Follower { WhoId = 1, WhomId = 2 };
        _miniTwitContext.Followers.Add(follow);
        await _miniTwitContext.SaveChangesAsync();

        bool existsBefore = await _miniTwitContext.Followers.AnyAsync(f =>
            f.WhoId == 1 && f.WhomId == 2
        );
        Assert.True(existsBefore);

        // Act: Call DELETE /follow?followerId=1&followedId=2.
        var response = await _client.DeleteAsync("/follow?followerId=1&followedId=2");
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        // Deserialize the response into the unfollow DTO.
        var unfollowDto = await response.Content.ReadFromJsonAsync<UnfollowResponse>(_jsonOptions);
        Assert.NotNull(unfollowDto);
        Assert.True(unfollowDto!.Success);
        Assert.Contains("Unfollowed", unfollowDto.Message);

        bool existsAfter = await _miniTwitContext.Followers.AnyAsync(f =>
            f.WhoId == 1 && f.WhomId == 2
        );
        Assert.False(existsAfter);
    }

    [Fact]
    public async Task RegisterUserEndpoint_WorksAsExpected()
    {
        // Prepare the registration request.
        var registerRequest = new
        {
            Username = "newuser",
            Email = "newuser@example.com",
            Password = "secret",
        };

        // Act: Call POST /register.
        var response = await _client.PostAsJsonAsync("/register", registerRequest);
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);

        // Deserialize the response into the DTO.
        var registerResponse = await response.Content.ReadFromJsonAsync<RegisterUserResponse>(
            _jsonOptions
        );
        Assert.NotNull(registerResponse);
        Assert.Equal("newuser", registerResponse!.Username);
        Assert.Equal("newuser@example.com", registerResponse.Email);
        Assert.True(registerResponse.UserId > 0);

        // Verify that the user exists in the database.
        var userInDb = await _miniTwitContext.Users.FirstOrDefaultAsync(u =>
            u.UserId == registerResponse.UserId
        );
        Assert.NotNull(userInDb);
        Assert.Equal("newuser", userInDb!.Username);
    }

    [Fact]
    public async Task LoginUserEndpoint_WorksAsExpected()
    {
        var user = new User
        {
            Username = "loginuser",
            Email = "loginuser@example.com",
            PwHash = "mypassword",
        };
        _miniTwitContext.Users.Add(user);
        await _miniTwitContext.SaveChangesAsync();

        // Act: Successful login.
        var loginRequest = new { Email = "loginuser@example.com", Password = "mypassword" };
        var response = await _client.PostAsJsonAsync("/login", loginRequest);
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginUserResponse>(
            _jsonOptions
        );
        Assert.NotNull(loginResponse);
        Assert.Equal(user.UserId, loginResponse!.UserId);
        Assert.Equal("loginuser", loginResponse.Username);
        Assert.Equal("loginuser@example.com", loginResponse.Email);
        Assert.Equal("fake-token", loginResponse.Token);

        // Act: Failed login (wrong password).
        var wrongLoginRequest = new { Email = "loginuser@example.com", Password = "wrongpassword" };
        var responseWrong = await _client.PostAsJsonAsync("/login", wrongLoginRequest);
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, responseWrong.StatusCode);
    }

    [Fact]
    public async Task LogoutUserEndpoint_WorksAsExpected()
    {
        // Act: Call POST /logout.
        var response = await _client.PostAsync("/logout", null);
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        var logoutResponse = await response.Content.ReadFromJsonAsync<LogoutUserResponse>(
            _jsonOptions
        );
        Assert.NotNull(logoutResponse);
        Assert.True(logoutResponse!.Success);
        Assert.Equal("Logged out successfully.", logoutResponse.Message);
    }

    //We don't care about the InitializeAsync method, but needed to implement the IAsyncLifetime interface
    public Task InitializeAsync()
    {
        _miniTwitContext.ChangeTracker.Clear();
        return Task.CompletedTask;
    }

    public Task DisposeAsync() => _resetDatabase();
}
