using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using MiniTwit.Shared.DTO.Timeline;

namespace MiniTwit.Api.Features.Timeline.GetUserTimeline
{
    public static class Endpoint
    {
        public static IEndpointRouteBuilder MapGetUserTimelineEndpoints(
            this IEndpointRouteBuilder routes
        )
        {
            routes.MapGet(
                "/user/{id:int}",
                async (
                    int id,
                    int? offset,
                    MiniTwitDbContext db,
                    HybridCache hybridCache,
                    CancellationToken cancellationToken
                ) =>
                {
                    const int perPage = 30;
                    int skip = offset ?? 0;
                    string cacheKey = $"userTimeline:{id}:{skip}";

                    var dtos = await hybridCache.GetOrCreateAsync<List<GetMessageResponse>>(
                        cacheKey,
                        async ct =>
                        {
                            var messages = await db
                                .Messages.Where(m => m.AuthorId == id && (m.Flagged ?? 0) == 0)
                                .OrderByDescending(m => m.PubDate)
                                .Skip(skip)
                                .Take(perPage)
                                .Include(m => m.Author)
                                .ToListAsync(ct);

                            return messages
                                .Select(m => new GetMessageResponse
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
                                })
                                .ToList();
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
