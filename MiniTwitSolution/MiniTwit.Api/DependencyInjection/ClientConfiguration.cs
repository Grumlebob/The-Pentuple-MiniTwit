namespace MiniTwit.Api.DependencyInjection;

public static class ClientConfiguration
{
    public static IServiceCollection AddClientCors(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var clientBaseUrl = configuration.GetSection("ClientBaseUrl").Get<string[]>();
        if (clientBaseUrl is null || clientBaseUrl.Length == 0)
        {
            throw new Exception(
                "Client base URL is not configured. Please set ClientBaseUrl in appsettings.json."
            );
        }
        services.AddCors(options =>
        {
            options.AddPolicy(
                "AllowBlazorClient",
                policy =>
                {
                    policy.WithOrigins(clientBaseUrl).AllowAnyHeader().AllowAnyMethod();
                }
            );
        });

        return services;
    }
}
