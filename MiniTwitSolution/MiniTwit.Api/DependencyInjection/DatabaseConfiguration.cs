namespace MiniTwit.Api.DependencyInjection;

public static class DatabaseConfiguration
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration) {
        services.AddDbContext<MiniTwitDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
        );

        services.AddScoped<IMiniTwitDbContext>(provider =>
            provider.GetRequiredService<MiniTwitDbContext>()
        );

        return services;
    }
}