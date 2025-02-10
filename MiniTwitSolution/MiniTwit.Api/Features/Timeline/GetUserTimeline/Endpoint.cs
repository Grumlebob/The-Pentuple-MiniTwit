namespace MiniTwit.Api.Features.Timeline.GetUserTimeline;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapGetUserTimelineEndpoints(this IEndpointRouteBuilder routes)
    {
        // This endpoint expects a user ID in the URL, e.g. /user/1
        routes.MapGet("/user/{id:int}", async (int id, int? offset, MiniTwitDbContext db) =>
        {
            const int perPage = 30;
            int skip = offset ?? 0;

            // Return messages authored by the specified user that are not flagged.
            var messages = await db.Messages
                .Where(m => m.AuthorId == id && (m.Flagged ?? 0) == 0)
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