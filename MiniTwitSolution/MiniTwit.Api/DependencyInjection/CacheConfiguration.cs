using Microsoft.Extensions.Caching.Hybrid;

namespace MiniTwit.Api.DependencyInjection;

public static class CacheConfiguration
{
    public static IServiceCollection AddCaching(this IServiceCollection services)
    {
        services.AddMemoryCache();
        // Register HybridCache (using the new .NET 9 API or a preview package)
        #pragma warning disable EXTEXP0018
        services.AddHybridCache(options =>
        {
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                LocalCacheExpiration = TimeSpan.FromMinutes(5),
                Expiration = TimeSpan.FromMinutes(5),
            };
        });
        #pragma warning restore EXTEXP0018

        return services;
    }
}