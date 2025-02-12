using Microsoft.Extensions.Caching.Hybrid;
using MiniTwit.Shared.DTO.Timeline;

namespace MiniTwit.Api.Features.Timeline.GetPublicTimeline;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapGetPublicTimelineEndpoints(
        this IEndpointRouteBuilder routes
    )
    {
        routes.MapGet(
            "/public",
            async (
                HttpContext context, 
                int? offset, 
                MiniTwitDbContext db, 
                HybridCache hybridCache,
                CancellationToken cancellationToken) =>
            {
                const int perPage = 30;
                int skip = offset ?? 0;

                // Build a cache key based on the offset.
                string cacheKey = $"publicTimeline:{skip}";

                // Use GetOrCreateAsync to either get the cached result or execute the factory.
                var dtos = await hybridCache.GetOrCreateAsync<List<GetMessageResponse>>(
                    cacheKey,
                    async ct =>
                    {
                        // Query the database (pass the cancellation token to EF Core)
                        var messages = await db.Messages
                            .Where(m => (m.Flagged ?? 0) == 0)
                            .OrderByDescending(m => m.PubDate)
                            .Skip(skip)
                            .Take(perPage)
                            .Include(m => m.Author)
                            .ToListAsync(ct);

                        // Map entities to DTOs.
                        return messages.Select(m => new GetMessageResponse
                        {
                            MessageId = m.MessageId,
                            Text = m.Text,
                            PubDate = m.PubDate,
                            Author = m.Author is not null
                                ? new GetUserResponse
                                {
                                    UserId = m.Author.UserId,
                                    Username = m.Author.Username,
                                }
                                : null,
                        }).ToList();
                    }, cancellationToken: cancellationToken);

                return Results.Ok(dtos);
            }
        );
        return routes;
    }
}