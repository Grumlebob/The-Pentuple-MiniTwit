using Microsoft.Extensions.Caching.Hybrid;
using MiniTwit.Shared.DTO.Followers.FollowUser;

namespace MiniTwit.Api.Features.Followers.FollowUser
{
    public static class FollowUserEndpoints
    {
        public static IEndpointRouteBuilder MapFollowUserEndpoints(
            this IEndpointRouteBuilder routes
        )
        {
            // POST /follow : Follow a user.
            routes.MapPost(
                "/followerere",
                async (
                    FollowRequest request,
                    MiniTwitDbContext db,
                    HybridCache hybridCache,
                    CancellationToken cancellationToken
                ) =>
                {
                    // Validate that both users exist.
                    var follower = await db.Users.FindAsync(
                        new object[] { request.FollowerId },
                        cancellationToken
                    );
                    var followed = await db.Users.FindAsync(
                        new object[] { request.FollowedId },
                        cancellationToken
                    );
                    if (follower == null || followed == null)
                    {
                        return Results.BadRequest("Invalid user IDs.");
                    }

                    // Check if the follow relationship already exists.
                    bool alreadyFollowing = await db.Followers.AnyAsync(
                        f => f.WhoId == request.FollowerId && f.WhomId == request.FollowedId,
                        cancellationToken
                    );
                    if (alreadyFollowing)
                    {
                        return Results.Conflict("Already following this user.");
                    }

                    // Create the follow relationship.
                    var newFollower = new Follower
                    {
                        WhoId = request.FollowerId,
                        WhomId = request.FollowedId,
                    };

                    db.Followers.Add(newFollower);
                    await db.SaveChangesAsync(cancellationToken);

                    // Invalidate the follower’s private timeline (first page).
                    await hybridCache.RemoveAsync(
                        $"privateTimeline:{request.FollowerId}:0",
                        cancellationToken
                    );

                    // Return a simple DTO.
                    var dto = new FollowResponse(newFollower.WhoId, newFollower.WhomId);
                    return Results.Created(
                        $"/follow?followerId={request.FollowerId}&followedId={request.FollowedId}",
                        dto
                    );
                }
            );

            return routes;
        }
    }
}
