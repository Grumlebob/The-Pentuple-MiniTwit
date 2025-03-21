using Microsoft.Extensions.Caching.Hybrid;
using MiniTwit.Shared.DTO.Latest;

namespace MiniTwit.Api.Features.Latest.GetLatest;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapGetLatestEndpoint(this IEndpointRouteBuilder routes)
    {
        routes.MapGet(
            "/latest",
            async (
                MiniTwitDbContext db,
                HybridCache hybridCache,
                CancellationToken cancellationToken
            ) =>
            {
                const string cacheKey = "latestEvent";

                // Define cache entry options
                var cacheEntryOptions = new HybridCacheEntryOptions
                {
                    LocalCacheExpiration = TimeSpan.FromMinutes(5),
                    Expiration = TimeSpan.FromMinutes(5),
                };

                var response = await hybridCache.GetOrCreateAsync<GetLatestResponse>(
                    cacheKey,
                    async ct =>
                    {
                        var latest = await db
                            .Latests.AsNoTracking()
                            .Where(l => l.Id == 1)
                            .FirstAsync(ct);
                        return new GetLatestResponse(latest.LatestEventId);
                    },
                    cacheEntryOptions,
                    cancellationToken: cancellationToken,
                    tags: new[] { cacheKey }
                );

                return Results.Ok(response);
            }
        );

        return routes;
    }
}
