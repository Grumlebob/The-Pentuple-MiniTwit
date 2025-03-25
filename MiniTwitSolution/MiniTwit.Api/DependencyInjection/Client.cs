namespace MiniTwit.Api.DependencyInjection;

public static class Client
{
    public static IServiceCollection AllowClient(this IServiceCollection services, IConfiguration configuration) {
        var clientBaseUrl = configuration["ClientBaseUrl"];
        if (string.IsNullOrEmpty(clientBaseUrl))
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