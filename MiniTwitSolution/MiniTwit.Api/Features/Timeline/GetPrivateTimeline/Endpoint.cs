using MiniTwit.Shared.DTO.Timeline;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace MiniTwit.Api.Features.Timeline.GetPrivateTimeline
{
    public static class Endpoint
    {
        public static IEndpointRouteBuilder MapGetPrivateTimelineEndpoints(this IEndpointRouteBuilder routes)
        {
            routes.MapGet(
                "/",
                async (HttpContext context, int? offset, MiniTwitDbContext db, HybridCache hybridCache, CancellationToken cancellationToken) =>
                {
                    // Require the "userId" query parameter.
                    if (!context.Request.Query.ContainsKey("userId"))
                    {
                        return Results.Redirect("/public");
                    }
                    if (!int.TryParse(context.Request.Query["userId"], out int userId))
                    {
                        return Results.BadRequest("Invalid userId");
                    }

                    const int perPage = 30;
                    int skip = offset ?? 0;
                    string cacheKey = $"privateTimeline:{userId}:{skip}";

                    var dtos = await hybridCache.GetOrCreateAsync<List<GetMessageResponse>>(
                        cacheKey,
                        async ct =>
                        {
                            // Get the list of users the current user follows.
                            var followedUserIds = await db.Followers
                                .Where(f => f.WhoId == userId)
                                .Select(f => f.WhomId)
                                .ToListAsync(ct);
                            followedUserIds.Add(userId); // Include self

                            var messages = await db.Messages
                                .Where(m => (m.Flagged ?? 0) == 0 && followedUserIds.Contains(m.AuthorId))
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
