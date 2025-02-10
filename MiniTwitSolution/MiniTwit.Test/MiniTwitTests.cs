using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using MiniTwit.Api.Domain;
using MiniTwit.Api.Infrastructure;
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
    public async Task HelloEndpoint_ReturnsHelloMessage()
    {
        // Act: Call the /hello endpoint.
        var response = await _client.GetAsync("/hello");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        // Assert: Verify that the response is the expected string.
        Assert.Equal("Hello, MiniTwit API is running!", content);
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
        var user1 = new User { UserId = 1, Username = "user1", Email = "user1@example.com", PwHash = "hash1" };
        var user2 = new User { UserId = 2, Username = "user2", Email = "user2@example.com", PwHash = "hash2" };
        _miniTwitContext.Users.AddRange(user1, user2);
        await _miniTwitContext.SaveChangesAsync();

        // Create a follower relationship: user1 follows user2.
        _miniTwitContext.Followers.Add(new Follower { WhoId = 1, WhomId = 2 });
        await _miniTwitContext.SaveChangesAsync();

        // Insert messages for both users.
        var currentTime = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        _miniTwitContext.Messages.Add(new Message
        {
            MessageId = 1,
            AuthorId = 1,
            Text = "Hello from user1",
            PubDate = currentTime,
            Flagged = 0
        });
        _miniTwitContext.Messages.Add(new Message
        {
            MessageId = 2,
            AuthorId = 2,
            Text = "Hello from user2",
            PubDate = currentTime,
            Flagged = 0
        });
        await _miniTwitContext.SaveChangesAsync();

        // --- Call the API Endpoint ---
        // The private timeline endpoint is at "/" and expects a query parameter "userId".
        var response = await _client.GetAsync("/?userId=1&offset=0");
        response.EnsureSuccessStatusCode();

        // Read and deserialize the JSON response.
        var json = await response.Content.ReadAsStringAsync();
        var messages = JsonSerializer.Deserialize<List<Message>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(messages);
        // We expect 2 messages: one from user1 and one from user2.
        Assert.Equal(2, messages!.Count);
    }

    [Fact]
    public async Task GetPublicTimelineWorksAsExpected()
    {
        // Clear existing data
        _miniTwitContext.Users.RemoveRange(_miniTwitContext.Users.ToList());
        _miniTwitContext.Followers.RemoveRange(_miniTwitContext.Followers.ToList());
        _miniTwitContext.Messages.RemoveRange(_miniTwitContext.Messages.ToList());
        await _miniTwitContext.SaveChangesAsync();

// Clear any tracked entities to avoid conflicts
        _miniTwitContext.ChangeTracker.Clear();


        // Create two users.
        var user1 = new User { UserId = 1, Username = "user1", Email = "user1@example.com", PwHash = "hash1" };
        var user2 = new User { UserId = 2, Username = "user2", Email = "user2@example.com", PwHash = "hash2" };
        _miniTwitContext.Users.AddRange(user1, user2);
        await _miniTwitContext.SaveChangesAsync();

        // Insert messages for both users (both flagged=0).
        var currentTime = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        _miniTwitContext.Messages.Add(new Message
        {
            MessageId = 1,
            AuthorId = 1,
            Text = "Hello from user1",
            PubDate = currentTime,
            Flagged = 0
        });
        _miniTwitContext.Messages.Add(new Message
        {
            MessageId = 2,
            AuthorId = 2,
            Text = "Hello from user2",
            PubDate = currentTime,
            Flagged = 0
        });
        await _miniTwitContext.SaveChangesAsync();

        // --- Act: Call the public timeline endpoint (GET "/public?offset=0") ---
        var response = await _client.GetAsync("/public?offset=0");
        response.EnsureSuccessStatusCode();

        // Read and deserialize the JSON response.
        var json = await response.Content.ReadAsStringAsync();
        var messages = JsonSerializer.Deserialize<List<Message>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert: Expect 2 messages in the public timeline.
        Assert.NotNull(messages);
        Assert.Equal(2, messages!.Count);
    }

    [Fact]
    public async Task GetUserTimelineWorksAsExpected()
    {
// Remove all existing entities (force enumeration with ToList())
        _miniTwitContext.Users.RemoveRange(_miniTwitContext.Users.ToList());
        _miniTwitContext.Followers.RemoveRange(_miniTwitContext.Followers.ToList());
        _miniTwitContext.Messages.RemoveRange(_miniTwitContext.Messages.ToList());
        await _miniTwitContext.SaveChangesAsync();

// Clear any tracked entities to avoid conflicts
        _miniTwitContext.ChangeTracker.Clear();


        // Create two users.
        var user1 = new User { UserId = 1, Username = "user1", Email = "user1@example.com", PwHash = "hash1" };
        var user2 = new User { UserId = 2, Username = "user2", Email = "user2@example.com", PwHash = "hash2" };
        _miniTwitContext.Users.AddRange(user1, user2);
        await _miniTwitContext.SaveChangesAsync();

        // Insert messages for both users.
        var currentTime = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        _miniTwitContext.Messages.Add(new Message
        {
            MessageId = 1,
            AuthorId = 1,
            Text = "Hello from user1",
            PubDate = currentTime,
            Flagged = 0
        });
        _miniTwitContext.Messages.Add(new Message
        {
            MessageId = 2,
            AuthorId = 2,
            Text = "Hello from user2",
            PubDate = currentTime,
            Flagged = 0
        });
        await _miniTwitContext.SaveChangesAsync();

        // --- Act: Call the user timeline endpoint (GET "/user/1?offset=0") ---
        var response = await _client.GetAsync("/user/1?offset=0");
        response.EnsureSuccessStatusCode();

        // Read and deserialize the JSON response.
        var json = await response.Content.ReadAsStringAsync();
        var messages = JsonSerializer.Deserialize<List<Message>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert: Expect only messages authored by user 1.
        Assert.NotNull(messages);
        Assert.Single(messages);
        Assert.Equal(1, messages![0].AuthorId);
    }


    //We don't care about the InitializeAsync method, but needed to implement the IAsyncLifetime interface
    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();
}