using Microsoft.Extensions.Caching.Hybrid;
using MiniTwit.Shared.DTO.Messages;
using MiniTwit.Api.Utility;
using Microsoft.AspNetCore.Mvc;

namespace MiniTwit.Api.Features.Messages.PostMessage
{
    public static class Endpoint
    {
        public static IEndpointRouteBuilder MapPostMessageEndpoints(
            this IEndpointRouteBuilder routes
        )
        {
            routes.MapPost(
                "/msgs/{username}",
                async (
                    string username,
                    PostMessageRequest request,
                    MiniTwitDbContext db,
                    HybridCache hybridCache,
                    CancellationToken cancellationToken,
                    [FromQuery] int latest = -1
                ) =>
                {
                    // Validate that the author exists.
                    var author = await db.Users.FirstOrDefaultAsync(
                        u => u.Username == username,
                        cancellationToken
                    );
                    if (author == null)
                    {
                        return Results.BadRequest("Author not found.");
                    }

                    var pubDate = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                    var message = new Message
                    {
                        AuthorId = author.UserId,
                        Text = request.Content,
                        PubDate = pubDate,
                        Flagged = 0,
                    };

                    db.Messages.Add(message);
                    await db.SaveChangesAsync(cancellationToken);

                    // Invalidate caches impacted by a new message:
                    // - Public timeline (all caches tagged with "publicTimeline")
                    await hybridCache.RemoveByTagAsync("publicTimeline", cancellationToken);
                    // - Author's own timeline (all caches tagged with "userTimeline:{author.Username}")
                    await hybridCache.RemoveByTagAsync(
                        $"userTimeline:{author.Username}",
                        cancellationToken
                    );
                    // - Private timelines of all followers of the author (assuming you tag them with "privateTimeline:{followerId}")
                    var followerIds = await db
                        .Followers.Where(f => f.WhomId == author.UserId)
                        .Select(f => f.WhoId)
                        .ToListAsync(cancellationToken);
                    foreach (var followerId in followerIds)
                    {
                        await hybridCache.RemoveByTagAsync(
                            $"privateTimeline:{followerId}",
                            cancellationToken
                        );
                    }
                    await UpdateLatest.UpdateLatestStateAsync(latest, db);
                    return Results.NoContent();
                }
            );

            return routes;
        }
    }
}
