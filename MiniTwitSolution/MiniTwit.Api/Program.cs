using Microsoft.Extensions.Caching.Hybrid;
using MiniTwit.Api.Features.Followers.GetFollowers;
using MiniTwit.Api.Features.Followers.PostFollowUser;
using MiniTwit.Api.Features.Messages.GetMessages;
using MiniTwit.Api.Features.Messages.GetUserMessages;
using MiniTwit.Api.Features.Messages.PostMessage;
using MiniTwit.Api.Features.Users.Authentication.LoginUser;
using MiniTwit.Api.Features.Users.Authentication.LogoutUser;
using MiniTwit.Api.Features.Users.Authentication.RegisterUser;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//TESTING MODE
//If outcommented, we use SQLite in-memory database
//Otherwise it uses Postgres
//builder.Environment.EnvironmentName = "Testing";

// Only configure database if we're not in test mode
if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<MiniTwitDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
    );

    builder.Services.AddScoped<IMiniTwitDbContext>(provider =>
        provider.GetRequiredService<MiniTwitDbContext>()
    );
}

var clientBaseUrl = builder.Configuration["ClientBaseUrl"];
if (string.IsNullOrEmpty(clientBaseUrl))
{
    throw new Exception(
        "API base URL is not configured. Please set ApiBaseUrl in appsettings.json."
    );
}

//allow client to use api
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowBlazorClient",
        policy =>
        {
            policy
                .WithOrigins(clientBaseUrl) // Replace with your client URL.
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    );
});

// Register basic caching services
builder.Services.AddMemoryCache();

// Register HybridCache (using the new .NET 9 API or a preview package)
#pragma warning disable EXTEXP0018
builder.Services.AddHybridCache(options =>
{
    options.DefaultEntryOptions = new HybridCacheEntryOptions()
    {
        LocalCacheExpiration = TimeSpan.FromMinutes(5),
        Expiration = TimeSpan.FromMinutes(5),
    };
});
#pragma warning restore EXTEXP0018


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowBlazorClient");

//Message endpoints
app.MapPostMessageEndpoints(); // registers POST "/msgs/{username}" endpoint.
app.MapGetMessagesEndpoints(); // registers GET "/msgs" endpoint.
app.MapGetUserMessagesEndpoints(); // registers GET "/msgs/{username}" endpoint.

// Map follow/unfollow endpoints.
app.MapFollowUserEndpoints(); // registers POST "/fllws/{username}"
app.MapGetFollowersEndpoints(); // registers GET "/fllws/{username}"

// Map user endpoints.
app.MapRegisterUserEndpoints(); // registers POST "/register"
app.MapLoginUserEndpoints(); // registers POST "/login"
app.MapLogoutUserEndpoints(); // registers POST "/logout"

app.Run();

public partial class Program { }
