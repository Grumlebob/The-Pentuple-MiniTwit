using MiniTwit.Api.DependencyInjection;
using MiniTwit.Api.Endpoints;
using MiniTwit.Api.Services;
using MiniTwit.Api.Services.Interfaces;
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
    .AddClientCors(builder.Configuration)
    .AddCaching();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IFollowerService, FollowerService>();
builder.Services.AddScoped<ILatestService, LatestService>();


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

app.MapUserEndpoints();
app.MapFollowerEndpoints();
app.MapLatestEndpoints();
app.MapMessageEndpoints();

app.Run();

public abstract partial class Program { }
