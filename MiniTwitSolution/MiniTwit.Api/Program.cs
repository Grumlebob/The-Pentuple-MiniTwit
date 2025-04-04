using MiniTwit.Api.DependencyInjection;
using MiniTwit.Api.Endpoints;
using MiniTwit.Api.Services;
using MiniTwit.Api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// We are using serilog
builder.AddSerilog();

// Add services to the container.
builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddDatabase(builder.Configuration)
    .AddClientCors(builder.Configuration)
    .AddCaching()
    .AddScoped<IUserService, UserService>()
    .AddScoped<IMessageService, MessageService>()
    .AddScoped<IFollowerService, FollowerService>()
    .AddScoped<ILatestService, LatestService>();

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

//app.UseHttpsRedirection();
app.ConfigureSerilog();
app.UseCors("AllowBlazorClient");
    
app.MapUserEndpoints()
    .MapFollowerEndpoints()
    .MapLatestEndpoints()
    .MapMessageEndpoints();

app.Run();

public abstract partial class Program;