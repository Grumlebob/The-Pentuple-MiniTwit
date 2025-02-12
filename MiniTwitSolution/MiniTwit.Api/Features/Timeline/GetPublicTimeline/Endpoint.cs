using MiniTwit.Shared.DTO.Timeline;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace MiniTwit.Api.Features.Timeline.GetPublicTimeline
{
    public static class Endpoint
    {
        public static IEndpointRouteBuilder MapGetPublicTimelineEndpoints(this IEndpointRouteBuilder routes)
        {
            routes.MapGet(
                "/public",
                async (HttpContext context, int? offset, MiniTwitDbContext db, HybridCache hybridCache, CancellationToken cancellationToken) =>
                {
                    const int perPage = 30;
                    int skip = offset ?? 0;
                    
                    // Construct a unique cache key.
                    string cacheKey = $"publicTimeline:{skip}";

                    // Retrieve from cache or execute the factory delegate.
                    var dtos = await hybridCache.GetOrCreateAsync<List<GetMessageResponse>>(
                        cacheKey,
                        async ct =>
                        {
                            var messages = await db.Messages
                                .Where(m => (m.Flagged ?? 0) == 0)
                                .OrderByDescending(m => m.PubDate)
                                .Skip(skip)
                                .Take(perPage)
                                .Include(m => m.Author)
                                .ToListAsync(ct);

                            return messages.Select(m => new GetMessageResponse
                            {
                                MessageId = m.MessageId,
                                Text = m.Text,
                                PubDate = m.PubDate,
                                Author = m.Author is not null ? new GetUserResponse
                                {
                                    UserId = m.Author.UserId,
                                    Username = m.Author.Username,
                                } : null,
                            }).ToList();
                        },
                        cancellationToken: cancellationToken
                    );

                    return Results.Ok(dtos);
                }
            );

            return routes;
        }
    }
}
