using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MiniTwit.Api.Domain;
using MiniTwit.Api.Infrastructure;
using MiniTwit.Shared.DTO.Followers.FollowUser;
using MiniTwit.Shared.DTO.Timeline;
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

    public MiniTwitTests(MiniTwitApiWebAppFactory factory)
    {
        _client = factory.HttpClient;
        _resetDatabase = factory.ResetDatabaseAsync;
        _miniTwitContext = factory.Services.GetRequiredService<MiniTwitDbContext>();
    }

    [Fact]
    public async Task GetPrivateTimeLineWorksAsExpected()
    {
        // Remove all existing entities (force enumeration with ToList())
        _miniTwitContext.Users.RemoveRange(_miniTwitContext.Users.ToList());
        _miniTwitContext.Followers.RemoveRange(_miniTwitContext.Followers.ToList());
        _miniTwitContext.Messages.RemoveRange(_miniTwitContext.Messages.ToList());
        await _miniTwitContext.SaveChangesAsync();

        // Clear any tracked entities to avoid conflicts
        _miniTwitContext.ChangeTracker.Clear();

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
        var dtos = JsonSerializer.Deserialize<List<GetMessageDto>>(
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
        // Clear existing data.
        _miniTwitContext.Users.RemoveRange(_miniTwitContext.Users.ToList());
        _miniTwitContext.Followers.RemoveRange(_miniTwitContext.Followers.ToList());
        _miniTwitContext.Messages.RemoveRange(_miniTwitContext.Messages.ToList());
        await _miniTwitContext.SaveChangesAsync();

        // Clear any tracked entities to avoid conflicts.
        _miniTwitContext.ChangeTracker.Clear();

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
        var dtos = JsonSerializer.Deserialize<List<GetMessageDto>>(
            json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        // Assert: Expect 2 messages in the public timeline.
        Assert.NotNull(dtos);
        Assert.Equal(2, dtos!.Count);
    }

    [Fact]
    public async Task GetUserTimelineWorksAsExpected()
    {
        // Remove all existing entities (force enumeration with ToList())
        _miniTwitContext.Users.RemoveRange(_miniTwitContext.Users.ToList());
        _miniTwitContext.Followers.RemoveRange(_miniTwitContext.Followers.ToList());
        _miniTwitContext.Messages.RemoveRange(_miniTwitContext.Messages.ToList());
        await _miniTwitContext.SaveChangesAsync();

        // Clear any tracked entities to avoid conflicts.
        _miniTwitContext.ChangeTracker.Clear();

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
        var dtos = JsonSerializer.Deserialize<List<GetMessageDto>>(
            json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        // Assert: Expect only messages authored by user 1.
        Assert.NotNull(dtos);
        Assert.Single(dtos!);
        Assert.Equal(1, dtos![0].Author?.UserId);
    }

    [Fact]
    public async Task FollowUserEndpoint_ReturnsCorrectDTO()
    {
        // Arrange: Clear database and seed two users.
        _miniTwitContext.Users.RemoveRange(_miniTwitContext.Users.ToList());
        _miniTwitContext.Followers.RemoveRange(_miniTwitContext.Followers.ToList());
        _miniTwitContext.Messages.RemoveRange(_miniTwitContext.Messages.ToList());
        await _miniTwitContext.SaveChangesAsync();
        _miniTwitContext.ChangeTracker.Clear();

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
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var dto = await response.Content.ReadFromJsonAsync<FollowResponse>(options);
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
        // Arrange: Clear data, seed two users, and add a follow relationship.
        _miniTwitContext.Users.RemoveRange(_miniTwitContext.Users.ToList());
        _miniTwitContext.Followers.RemoveRange(_miniTwitContext.Followers.ToList());
        _miniTwitContext.Messages.RemoveRange(_miniTwitContext.Messages.ToList());
        await _miniTwitContext.SaveChangesAsync();
        _miniTwitContext.ChangeTracker.Clear();

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
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var unfollowDto = await response.Content.ReadFromJsonAsync<UnfollowResponse>(options);
        Assert.NotNull(unfollowDto);
        Assert.True(unfollowDto!.Success);
        Assert.Contains("Unfollowed", unfollowDto.Message);

        bool existsAfter = await _miniTwitContext.Followers.AnyAsync(f =>
            f.WhoId == 1 && f.WhomId == 2
        );
        Assert.False(existsAfter);
    }

    //We don't care about the InitializeAsync method, but needed to implement the IAsyncLifetime interface
    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();
}
