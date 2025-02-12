using Microsoft.Extensions.Caching.Hybrid;
using MiniTwit.Shared.DTO.Messages;

namespace MiniTwit.Api.Features.Messages.PostMessage;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapPostMessageEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapPost(
            "/add_message",
            async (PostMessageRequest request, MiniTwitDbContext db, HybridCache hybridCache,
                CancellationToken cancellationToken) =>
            {
                // Validate that the author exists.
                var author = await db.Users.FindAsync(new object[] { request.AuthorId }, cancellationToken);
                if (author == null)
                {
                    return Results.BadRequest("Author not found.");
                }

                // Determine publication date.
                int pubDate = request.PubDate ?? (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                // Create and save the new message.
                var message = new Message
                {
                    AuthorId = request.AuthorId,
                    Text = request.Text,
                    PubDate = pubDate,
                    Flagged = 0,
                };

                db.Messages.Add(message);
                await db.SaveChangesAsync(cancellationToken);

                // Invalidate caches impacted by a new message:
                // - Public timeline (first page)
                await hybridCache.RemoveAsync("publicTimeline:0", cancellationToken);
                // - Author's own timeline (first page)
                await hybridCache.RemoveAsync($"userTimeline:{request.AuthorId}:0", cancellationToken);
                // - Private timelines of all followers of the author (first page)
                var followerIds = await db.Followers
                    .Where(f => f.WhomId == request.AuthorId)
                    .Select(f => f.WhoId)
                    .ToListAsync(cancellationToken);
                foreach (var followerId in followerIds)
                {
                    await hybridCache.RemoveAsync($"privateTimeline:{followerId}:0", cancellationToken);
                }

                // Build the response DTO.
                var responseDto = new PostMessageResponse(
                    message.MessageId,
                    message.AuthorId,
                    message.Text,
                    message.PubDate
                );
                return Results.Created($"/message/{message.MessageId}", responseDto);
            }
        );

        return routes;
    }
}