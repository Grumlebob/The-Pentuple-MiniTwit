using Microsoft.Extensions.Caching.Hybrid;
using MiniTwit.Shared.DTO.Followers.FollowUser;

namespace MiniTwit.Api.Features.Followers.UnfollowUser
{
    public static class Endpoint
    {
        public static IEndpointRouteBuilder MapUnfollowUserEndpoints(
            this IEndpointRouteBuilder routes
        )
        {
            // DELETE /follow : Unfollow a user.
            routes.MapDelete(
                "/follow",
                async (
                    HttpRequest request,
                    MiniTwitDbContext db,
                    HybridCache hybridCache,
                    CancellationToken cancellationToken
                ) =>
                {
                    // Expect query parameters: followerId and followedId.
                    if (
                        !int.TryParse(request.Query["followerId"], out int followerId)
                        || !int.TryParse(request.Query["followedId"], out int followedId)
                    )
                    {
                        return Results.BadRequest("Invalid followerId or followedId.");
                    }

                    // Locate the follow relationship.
                    var followRecord = await db.Followers.FirstOrDefaultAsync(
                        f => f.WhoId == followerId && f.WhomId == followedId,
                        cancellationToken
                    );
                    if (followRecord == null)
                    {
                        return Results.NotFound("Follow relationship not found.");
                    }

                    db.Followers.Remove(followRecord);
                    await db.SaveChangesAsync(cancellationToken);

                    // Invalidate the follower’s private timeline (first page).
                    await hybridCache.RemoveAsync(
                        $"privateTimeline:{followerId}:0",
                        cancellationToken
                    );

                    // Return a DTO indicating success.
                    var dto = new UnfollowResponse(true, "Unfollowed successfully.");
                    return Results.Ok(dto);
                }
            );
            return routes;
        }
    }
}
