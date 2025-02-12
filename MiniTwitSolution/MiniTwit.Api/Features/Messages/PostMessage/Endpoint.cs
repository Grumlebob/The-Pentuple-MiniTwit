using Microsoft.Extensions.Caching.Hybrid;
using MiniTwit.Shared.DTO.Messages;

namespace MiniTwit.Api.Features.Messages.PostMessage
{
    public static class Endpoint
    {
        public static IEndpointRouteBuilder MapPostMessageEndpoints(this IEndpointRouteBuilder routes)
        {
            routes.MapPost(
                "/add_messageerer",
                async (
                    HttpContext context, // added so we can access session
                    PostMessageRequest request,
                    MiniTwitDbContext db,
                    HybridCache hybridCache,
                    CancellationToken cancellationToken
                ) =>
                {
                    // Validate that the author exists.
                    var author = await db.Users.FindAsync(
                        new object[] { request.AuthorId },
                        cancellationToken
                    );
                    if (author == null)
                    {
                        return Results.BadRequest("Author not found.");
                    }

                    // Check that text is provided (optional).
                    if (string.IsNullOrWhiteSpace(request.Text))
                    {
                        // Optionally, you can redirect with an error message.
                        context.Session.SetString("FlashMessage", "Message text cannot be empty.");
                        return Results.Redirect($"/?userId={request.AuthorId}");
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
                    await hybridCache.RemoveAsync("publicTimeline:0", cancellationToken);
                    await hybridCache.RemoveAsync($"userTimeline:{request.AuthorId}:0", cancellationToken);
                    
                    var followerIds = await db.Followers
                        .Where(f => f.WhomId == request.AuthorId)
                        .Select(f => f.WhoId)
                        .ToListAsync(cancellationToken);
                    foreach (var followerId in followerIds)
                    {
                        await hybridCache.RemoveAsync($"privateTimeline:{followerId}:0", cancellationToken);
                    }

                    // Set a flash message in the session.
                    context.Session.SetString("FlashMessage", "Your message was recorded.");

                    // Redirect to the timeline page, passing the author’s id.
                    return Results.Redirect($"/?userId={request.AuthorId}");
                }
            );

            return routes;
        }
    }
}