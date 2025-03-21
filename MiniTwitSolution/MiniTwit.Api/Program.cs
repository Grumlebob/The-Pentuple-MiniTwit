using Microsoft.Extensions.Caching.Hybrid;
using MiniTwit.Api.Features.Followers.GetFollowers;
using MiniTwit.Api.Features.Followers.PostFollowUser;
using MiniTwit.Api.Features.Latest.GetLatest;
using MiniTwit.Api.Features.Messages.GetMessages;
using MiniTwit.Api.Features.Messages.GetUserMessages;
using MiniTwit.Api.Features.Messages.PostMessage;
using MiniTwit.Api.Features.Users.Authentication.LoginUser;
using MiniTwit.Api.Features.Users.Authentication.LogoutUser;
using MiniTwit.Api.Features.Users.Authentication.RegisterUser;
using Serilog;
using Serilog.Extensions;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithRequestBody()
    .Enrich.WithRequestQuery()
    .CreateLogger();

// Replace the default logging provider with Serilog
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);

//TESTING MODE

// Only configure database if we're not in test mode

builder.Services.AddDbContext<MiniTwitDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddScoped<IMiniTwitDbContext>(provider =>
    provider.GetRequiredService<MiniTwitDbContext>()
);

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
            policy.WithOrigins(clientBaseUrl).AllowAnyHeader().AllowAnyMethod();
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

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MiniTwitDbContext>();
    db.Database.Migrate();
}

// Enable buffering for request bodies early in the pipeline.
app.Use(
    async (context, next) =>
    {
        context.Request.EnableBuffering();
        await next();
    }
);

app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate =
        "Handled {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        // Top-level properties for easy querying in Seq
        diagnosticContext.Set("RequestMethod", httpContext.Request.Method);
        diagnosticContext.Set("RequestPath", httpContext.Request.Path);

        // Composite property with additional HTTP request details
        diagnosticContext.Set(
            "HttpRequest",
            new
            {
                Method = httpContext.Request.Method,
                Path = httpContext.Request.Path,
                QueryString = httpContext.Request.QueryString.ToString(),
                Headers = httpContext.Request.Headers.ToDictionary(
                    h => h.Key,
                    h => h.Value.ToString()
                ),
            }
        );

        // Log request content (if available)
        if (httpContext.Request.ContentLength > 0 && httpContext.Request.Body.CanSeek)
        {
            httpContext.Request.Body.Position = 0; // Reset stream position
            using var reader = new StreamReader(httpContext.Request.Body, leaveOpen: true);
            var content = reader.ReadToEnd();
            diagnosticContext.Set("Content", content);
            httpContext.Request.Body.Position = 0; // Reset again for further processing
        }
        else
        {
            diagnosticContext.Set("Content", "Not Available");
        }
    };
});

//app.UseHttpsRedirection();

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

// Map latest endpoints.
app.MapGetLatestEndpoint();

app.Run();

public partial class Program { }
