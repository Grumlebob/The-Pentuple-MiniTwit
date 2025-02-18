[Collection("MiniTwitCollection")]
public class MiniTwitTests : IAsyncLifetime
{
    private readonly MiniTwitApiWebAppFactory _factory;

    // Create the typed client from the factory's HttpClient.
    private readonly MiniTwitClient _typedClient;
    private readonly MiniTwitDbContext _dbContext;

    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
    };

    public MiniTwitTests(MiniTwitApiWebAppFactory factory)
    {
        _factory = factory;
        _typedClient = new MiniTwitClient(_factory.HttpClient);
        _dbContext = _factory.Services.GetRequiredService<MiniTwitDbContext>();
    }

    public async Task InitializeAsync()
    {
        _dbContext.ChangeTracker.Clear();
        await Task.CompletedTask;
    }

    public Task DisposeAsync() => _factory.ResetDatabaseAsync();

    [Fact]
    public async Task GetPublicTimelineWorksAsExpected()
    {
        // Arrange: Create two users.
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
        _dbContext.Users.AddRange(user1, user2);
        await _dbContext.SaveChangesAsync();

        // Arrange: Insert messages for both users.
        var currentTime = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var message1 = new Message
        {
            MessageId = 1,
            AuthorId = 1,
            Text = "Hello from user1",
            PubDate = currentTime,
            Flagged = 0,
            // Set navigation property so that m.Author is not null.
            Author = user1,
        };
        var message2 = new Message
        {
            MessageId = 2,
            AuthorId = 2,
            Text = "Hello from user2",
            PubDate = currentTime,
            Flagged = 0,
            Author = user2,
        };
        _dbContext.Messages.AddRange(message1, message2);
        await _dbContext.SaveChangesAsync();

        // Act: Get the public timeline via the typed client.
        IList<GetMessageResponse> messages = await _typedClient.GetMessagesAsync(100);

        // Assert: We expect 2 messages.
        Assert.NotNull(messages);
        Assert.Equal(2, messages.Count);
    }

    [Fact]
    public async Task GetUserTimelineWorksAsExpected()
    {
        // Arrange: Create two users.
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
        _dbContext.Users.AddRange(user1, user2);
        await _dbContext.SaveChangesAsync();

        // Arrange: Insert messages for both users.
        var currentTime = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var message1 = new Message
        {
            MessageId = 1,
            AuthorId = 1,
            Text = "Hello from user1",
            PubDate = currentTime,
            Flagged = 0,
            Author = user1,
        };
        var message2 = new Message
        {
            MessageId = 2,
            AuthorId = 2,
            Text = "Hello from user2",
            PubDate = currentTime,
            Flagged = 0,
            Author = user2,
        };
        _dbContext.Messages.AddRange(message1, message2);
        await _dbContext.SaveChangesAsync();

        // Act: Get the timeline for user "user1" via the typed client.
        IList<GetMessageResponse> userMessages = await _typedClient.GetMessagesForUserAsync(
            "user1",
            100
        );

        // Assert: Only one message should belong to user1.
        Assert.NotNull(userMessages);
        Assert.Single(userMessages);
        Assert.Equal("user1", userMessages[0].AuthorUsername);
    }

    [Fact]
    public async Task FollowUserEndpoint_WorksAsExpected()
    {
        // Arrange: Create two users.
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
        _dbContext.Users.AddRange(user1, user2);
        await _dbContext.SaveChangesAsync();

        // Act: user1 follows user2 using the typed client.
        HttpResponseMessage response = await _typedClient.FollowUserAsync(
            "user1",
            new FollowRequest("user2")
        );

        // Assert: Expect HTTP 204 NoContent.
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify the follow relationship in the database.
        bool exists = await _dbContext.Followers.AnyAsync(f => f.WhoId == 1 && f.WhomId == 2);
        Assert.True(exists);
    }

    [Fact]
    public async Task UnfollowUserEndpoint_WorksAsExpected()
    {
        // Arrange: Create two users and add a follower relationship.
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
        _dbContext.Users.AddRange(user1, user2);
        await _dbContext.SaveChangesAsync();

        _dbContext.Followers.Add(new Follower { WhoId = 1, WhomId = 2 });
        await _dbContext.SaveChangesAsync();

        bool existsBefore = await _dbContext.Followers.AnyAsync(f => f.WhoId == 1 && f.WhomId == 2);
        Assert.True(existsBefore);

        // Act: user1 unfollows user2 using the typed client.
        // The typed client now uses custom serializer options (PascalCase) so that the JSON payload is:
        // { "Unfollow": "user2" }
        HttpResponseMessage response = await _typedClient.UnfollowUserAsync(
            "user1",
            new UnfollowRequest("user2")
        );

        // Assert: Expect HTTP 204 NoContent.
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        bool existsAfter = await _dbContext.Followers.AnyAsync(f => f.WhoId == 1 && f.WhomId == 2);
        Assert.False(existsAfter);
    }

    [Fact]
    public async Task RegisterUserEndpoint_WorksAsExpected()
    {
        // Act: Register a new user via the typed client.
        HttpResponseMessage response = await _typedClient.RegisterUserAsync(
            new RegisterUserRequest("newuser", "newuser@example.com", "secret")
        );

        // Assert: Expect HTTP 204 NoContent.
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify the new user exists in the database.
        var userInDb = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == "newuser");
        Assert.NotNull(userInDb);
    }

    [Fact]
    public async Task LoginUserEndpoint_WorksAsExpected()
    {
        // Arrange: Create a user for login.
        var user = new User
        {
            UserId = 1,
            Username = "loginuser",
            Email = "loginuser@example.com",
            PwHash = "mypassword",
        };
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        // Act: Successful login using the typed client.
        HttpResponseMessage response = await _typedClient.LoginUserAsync(
            new LoginUserRequest("loginuser", "mypassword")
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginUserResponse>(
            _jsonOptions
        );
        Assert.NotNull(loginResponse);
        Assert.Equal(user.UserId, loginResponse!.UserId);
        Assert.Equal("loginuser", loginResponse.Username);
        Assert.Equal("loginuser@example.com", loginResponse.Email);
        Assert.Equal("fake-token", loginResponse.Token);

        // Act: Failed login (wrong password).
        HttpResponseMessage responseWrong = await _typedClient.LoginUserAsync(
            new LoginUserRequest("loginuser", "wrongpassword")
        );
        Assert.Equal(HttpStatusCode.Unauthorized, responseWrong.StatusCode);
    }

    [Fact]
    public async Task LogoutUserEndpoint_WorksAsExpected()
    {
        // Act: Call logout via the typed client.
        HttpResponseMessage response = await _typedClient.LogoutUserAsync();
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var logoutResponse = await response.Content.ReadFromJsonAsync<LogoutUserResponse>(
            _jsonOptions
        );
        Assert.NotNull(logoutResponse);
        Assert.True(logoutResponse!.Success);
        Assert.Equal("Logged out successfully.", logoutResponse.Message);
    }
}
