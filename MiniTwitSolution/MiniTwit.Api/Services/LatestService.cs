using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Hybrid;
using MiniTwit.Api.Services.Interfaces;
using MiniTwit.Shared.DTO.Latest;

namespace MiniTwit.Api.Services;

public class LatestService : ILatestService
{
    private readonly MiniTwitDbContext _db;
    private readonly HybridCache _cache;

    public LatestService(MiniTwitDbContext db, HybridCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<IActionResult> GetLatestAsync(CancellationToken cancellationToken)
    {
        const string cacheKey = "latestEvent";

        var response = await _cache.GetOrCreateAsync<GetLatestResponse>(
            cacheKey,
            async ct =>
            {
                var latest = await _db.Latests.AsNoTracking().Where(l => l.Id == 1).FirstAsync(ct);

                return new GetLatestResponse(latest.LatestEventId);
            },
            new HybridCacheEntryOptions
            {
                LocalCacheExpiration = TimeSpan.FromMinutes(5),
                Expiration = TimeSpan.FromMinutes(5),
            },
            cancellationToken: cancellationToken,
            tags: new[] { cacheKey }
        );

        return new OkObjectResult(response);
    }
}
