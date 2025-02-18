namespace MiniTwit.Test;

//WebApplicationFactory is a class that allows us to create a test server for our application in memory,
//but setup with real dependencies.
public class MiniTwitApiWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    //Tests use a single DB connection.
    //Between each test, "Respawn" will handle DB setup.
    //This is an extra check, to ensure ALL tests finish within x minutes
    private const int MaxWaitTimeMinutes = 5;

    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .Build();

    //Default! cause we are not initializing it here, but in the InitializeAsync method
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
                provider.GetRequiredService<MiniTwitDbContext>()
            );
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

        // Initialize database - THIS IS WHERE YOU CAN ADD SEED DATA
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
                SchemasToInclude = new[] { "public" },
            }
        );
    }

    //"New": to tell compiler that this is a new DisposeAsync method
    public new async Task DisposeAsync()
    {
        if (_dbConnection.State == System.Data.ConnectionState.Open)
        {
            await _dbConnection.CloseAsync();
        }
        await _dbContainer.DisposeAsync();
    }
}
