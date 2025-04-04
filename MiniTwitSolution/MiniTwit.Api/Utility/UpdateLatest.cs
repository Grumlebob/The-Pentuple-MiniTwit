using Microsoft.Extensions.Caching.Hybrid;

namespace MiniTwit.Api.Utility;

public static class UpdateLatest
{
    public static async Task UpdateLatestStateAsync(
        int latest,
        MiniTwitDbContext db,
        HybridCache hybridCache,
        CancellationToken cancellationToken
    )
    {
        await db
            .Latests.Where(l => l.Id == 1)
            .ExecuteUpdateAsync(set => set.SetProperty(l => l.LatestEventId, latest));

        // Remove the latestEvent tag from the cache
        await hybridCache.SetAsync("latestEvent", latest, cancellationToken: cancellationToken);
    }
}
