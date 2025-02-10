using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MiniTwit.Api.Domain;
using MiniTwit.Api.Features.Timeline;
using MiniTwit.Api.Infrastructure;
using MiniTwit.Shared.DTO.Timeline;

namespace MiniTwit.Api.Features.Timeline.GetPublicTimeline
{
    public static class Endpoint
    {
        public static IEndpointRouteBuilder MapGetPublicTimelineEndpoints(
            this IEndpointRouteBuilder routes
        )
        {
            routes.MapGet(
                "/public",
                async (HttpContext context, int? offset, MiniTwitDbContext db) =>
                {
                    const int perPage = 30;
                    int skip = offset ?? 0;

                    // For the public timeline, return all messages that are not flagged.
                    var messages = await db
                        .Messages.Where(m => (m.Flagged ?? 0) == 0)
                        .OrderByDescending(m => m.PubDate)
                        .Skip(skip)
                        .Take(perPage)
                        .Include(m => m.Author)
                        .ToListAsync();

                    var dtos = messages
                        .Select(m => new GetMessageDto
                        {
                            MessageId = m.MessageId,
                            Text = m.Text,
                            PubDate = m.PubDate,
                            Author = m.Author is not null
                                ? new GetUserDto
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
