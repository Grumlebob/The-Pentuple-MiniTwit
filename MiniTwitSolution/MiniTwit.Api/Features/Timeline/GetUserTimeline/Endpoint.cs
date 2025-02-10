using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using MiniTwit.Api.Domain;
using MiniTwit.Api.Features.Timeline;
using MiniTwit.Api.Infrastructure;
using MiniTwit.Shared.DTO.Timeline;

namespace MiniTwit.Api.Features.Timeline.GetUserTimeline
{
    public static class Endpoint
    {
        public static IEndpointRouteBuilder MapGetUserTimelineEndpoints(
            this IEndpointRouteBuilder routes
        )
        {
            // This endpoint expects a user ID in the URL, e.g. /user/1
            routes.MapGet(
                "/user/{id:int}",
                async (int id, int? offset, MiniTwitDbContext db) =>
                {
                    const int perPage = 30;
                    int skip = offset ?? 0;

                    // Return messages authored by the specified user that are not flagged.
                    var messages = await db
                        .Messages.Where(m => m.AuthorId == id && (m.Flagged ?? 0) == 0)
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
