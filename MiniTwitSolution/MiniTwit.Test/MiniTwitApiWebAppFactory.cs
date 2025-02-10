using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MiniTwit.Api.Infrastructure;
using Respawn;
using Npgsql;
using Testcontainers.PostgreSql;
using Xunit;

namespace MiniTwit.Test;

public class MiniTwitApiWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private const int MaxWaitTimeMinutes = 5;

    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .Build();

    private DbConnection _dbConnection = default!;
    private Respawner _respawner = default!;
    public HttpClient HttpClient { get; private set; } = default!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Set environment to Testing to prevent default DB configuration
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Add PostgreSQL DbContext for tests
            services.AddDbContext<MiniTwitDbContext>(options =>
            {
                options.UseNpgsql(_dbContainer.GetConnectionString());
            });

            services.AddScoped<IMiniTwitDbContext>(provider =>
                provider.GetRequiredService<MiniTwitDbContext>());
        });
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        
        _dbConnection = new NpgsqlConnection(_dbContainer.GetConnectionString());
        
        HttpClient = CreateClient();
        HttpClient.Timeout = TimeSpan.FromMinutes(MaxWaitTimeMinutes);

        // Initialize database
        using (var scope = Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<MiniTwitDbContext>();
            await context.Database.EnsureCreatedAsync();
        }
        
        await InitializeRespawner();
    }

    private async Task InitializeRespawner()
    {
        await _dbConnection.OpenAsync();
        _respawner = await Respawner.CreateAsync(
            _dbConnection,
            new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = new[] { "public" }
            }
        );
    }

    public new async Task DisposeAsync()
    {
        if (_dbConnection.State == System.Data.ConnectionState.Open)
        {
            await _dbConnection.CloseAsync();
        }
        await _dbContainer.DisposeAsync();
    }
}