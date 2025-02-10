// Program.cs

using MiniTwit.Api.Features.Followers.FollowUser;
using MiniTwit.Api.Features.Followers.UnfollowUser;
using MiniTwit.Api.Features.Timeline.GetPrivateTimeline;
using MiniTwit.Api.Features.Timeline.GetPublicTimeline;
using MiniTwit.Api.Features.Timeline.GetUserTimeline;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//TESTING MODE
builder.Environment.EnvironmentName = "Testing";

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Timeline endpoints
app.MapGetPrivateTimelineEndpoints(); // Registers GET "/" with timeline logic.
app.MapGetPublicTimelineEndpoints(); // registers GET "/public"
app.MapGetUserTimelineEndpoints(); // registers GET "/user/{id:int}"

// Map follow/unfollow endpoints.
app.MapFollowUserEndpoints(); // registers POST "/follow"
app.MapUnfollowUserEndpoints(); // registers DELETE "/follow"

app.Run();

public partial class Program { }
