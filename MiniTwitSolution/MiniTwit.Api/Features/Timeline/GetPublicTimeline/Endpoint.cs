namespace MiniTwit.Api.Features.Timeline.GetPublicTimeline;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapGetPublicTimelineEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/public", async (HttpContext context, int? offset, MiniTwitDbContext db) =>
        {
            const int perPage = 30;
            int skip = offset ?? 0;

            // For the public timeline, return all messages that are not flagged.
            var messages = await db.Messages
                .Where(m => (m.Flagged ?? 0) == 0)
                .OrderByDescending(m => m.PubDate)
                .Skip(skip)
                .Take(perPage)
                .Include(m => m.Author)
                .ToListAsync();

            return Results.Ok(messages);
        });

        return routes;
    }
}