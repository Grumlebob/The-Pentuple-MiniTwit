using System.Linq.Expressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MiniTwit.Api.Domain;
using MiniTwit.Api.Features.Timeline;
using MiniTwit.Api.Infrastructure;
using MiniTwit.Shared.DTO.Timeline;

namespace MiniTwit.Api.Features.Timeline.GetPrivateTimeline
{
    public static class Endpoint
    {
        public static IEndpointRouteBuilder MapGetPrivateTimelineEndpoints(
            this IEndpointRouteBuilder routes
        )
        {
            routes.MapGet(
                "/",
                async (HttpContext context, int? offset, MiniTwitDbContext db) =>
                {
                    // Check for "userId" query parameter to simulate logged-in user.
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

                    // Get the list of followed user IDs
                    var followedUserIds = await db
                        .Followers.Where(f => f.WhoId == userId)
                        .Select(f => f.WhomId)
                        .ToListAsync();

                    // Include the current user's own ID.
                    followedUserIds.Add(userId);

                    // Query messages with the filter.
                    var messages = await db
                        .Messages.Where(m =>
                            (m.Flagged ?? 0) == 0 && followedUserIds.Contains(m.AuthorId)
                        )
                        .OrderByDescending(m => m.PubDate)
                        .Skip(skip)
                        .Take(perPage)
                        .Include(m => m.Author)
                        .ToListAsync();

                    // Map Message entities to DTOs.
                    var dtos = messages
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

                    return Results.Ok(dtos);
                }
            );

            return routes;
        }
    }
}
