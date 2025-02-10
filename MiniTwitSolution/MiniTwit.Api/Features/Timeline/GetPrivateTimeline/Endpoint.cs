namespace MiniTwit.Api.Features.Timeline.GetPrivateTimeline;

public static class GetPrivateTimelineEndpoints
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

                // Create an expression filter: messages not flagged (Flagged equals 0) and with an author in the followed list.
                Expression<Func<Message, bool>> filter = m =>
                    (m.Flagged ?? 0) == 0 && followedUserIds.Contains(m.AuthorId);

                // Query messages with the filter.
                var messages = await db
                    .Messages.Where(filter)
                    .OrderByDescending(m => m.PubDate)
                    .Skip(skip)
                    .Take(perPage)
                    .Include(m => m.Author)
                    .ToListAsync();

                return Results.Ok(messages);
            }
        );

        return routes;
    }
}
