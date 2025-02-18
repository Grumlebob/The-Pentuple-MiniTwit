using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Hybrid;
using MiniTwit.Shared.DTO.Messages;

namespace MiniTwit.Api.Features.Messages.GetUserMessages
{
    public static class Endpoint
    {
        public static IEndpointRouteBuilder MapGetUserMessagesEndpoints(this IEndpointRouteBuilder routes)
        {
            routes.MapGet(
                "/msgs/{username}",
                async (
                    string username,
                    MiniTwitDbContext db,
                    HybridCache hybridCache,
                    CancellationToken cancellationToken,
                    [FromQuery] int no = 100) =>
                {
                    // First, check if the user exists.
                    var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
                    if (user == null)
                    {
                        return Results.NotFound("User not found.");
                    }
                    
                    // Define a cache key that includes the username and limit.
                    var cacheKey = $"userTimeline:{username}:{no}";

                    // Retrieve or create the cached response.
                    var response = await hybridCache.GetOrCreateAsync<List<GetMessageResponse>>(
                        cacheKey,
                        async ct =>
                        {
                            // Retrieve the user's messages where flagged == 0.
                            var messages = await db.Messages
                                .Where(m => m.AuthorId == user.UserId && m.Flagged == 0)
                                .OrderByDescending(m => m.PubDate)
                                .Take(no)
                                .ToListAsync(ct);

                            // Map each message to the DTO.
                            var dto = messages
                                .Select(m => new GetMessageResponse(m.MessageId, m.PubDate, user.Username, m.Text))
                                .ToList();

                            return dto;
                        },
                        cancellationToken: cancellationToken,
                        tags: new[] { $"userTimeline:{username}" }
                    );

                    return Results.Json(response);
                }
            );

            return routes;
        }
    }
}
