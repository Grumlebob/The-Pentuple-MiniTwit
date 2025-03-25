using MiniTwit.Api.DependencyInjection;
using MiniTwit.Api.Features.Followers.GetFollowers;
using MiniTwit.Api.Features.Followers.PostFollowUser;
using MiniTwit.Api.Features.Latest.GetLatest;
using MiniTwit.Api.Features.Messages.GetMessages;
using MiniTwit.Api.Features.Messages.GetUserMessages;
using MiniTwit.Api.Features.Messages.PostMessage;
using MiniTwit.Api.Features.Users.Authentication.LoginUser;
using MiniTwit.Api.Features.Users.Authentication.LogoutUser;
using MiniTwit.Api.Features.Users.Authentication.RegisterUser;
using MiniTwit.Api.Utility;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// We are using serilog
builder.ConfigureLogging();

// Add services to the container.
builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddDatabase(builder.Configuration)
    .AllowClient(builder.Configuration)
    .AddMemoryCache();
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
        "Client base URL is not configured. Please set ClientBaseUrl in appsettings.json."
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

// Enable buffering for request bodies
app.Use(
    async (context, next) =>
    {
        context.Request.EnableBuffering();
        await next();
    }
);

// Enable buffering for response bodies
app.Use(
    async (context, next) =>
    {
        // Swap out the response stream with a memory stream to capture output
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await next();

        // Reset the response body position to read from the beginning
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
        context.Response.Body.Seek(0, SeekOrigin.Begin);

        // Log the response text here
        // e.g., diagnosticContext.Set("ResponseBody", responseText);

        // Copy the content of the memory stream to the original stream
        await responseBody.CopyToAsync(originalBodyStream);
    }
);

app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate =
        "Handled {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    options.EnrichDiagnosticContext = async (diagnosticContext, httpContext) =>
    {
        // Info from user request
        diagnosticContext.Set("RequestMethod", httpContext.Request.Method);
        diagnosticContext.Set("RequestPath", httpContext.Request.Path);
        diagnosticContext.Set("RequestQuery", httpContext.Request.QueryString.ToString());
        // request body if any
        // Log request content (if available)

        var requestBody = await HttpHelper.GetHttpBodyAsStringAsync(httpContext.Request.Body);
        diagnosticContext.Set("RequestBody", requestBody);

        // Info from response
        diagnosticContext.Set("StatusCode", httpContext.Response.StatusCode);
        // Response body if any

        var reponseBody = await HttpHelper.GetHttpBodyAsStringAsync(httpContext.Response.Body);
        diagnosticContext.Set("ResponseBody", reponseBody);
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

public partial class Program
{
}