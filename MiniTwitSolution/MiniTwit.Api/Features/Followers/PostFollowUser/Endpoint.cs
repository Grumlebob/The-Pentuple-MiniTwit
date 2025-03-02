using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Hybrid;
using MiniTwit.Api.Utility;

namespace MiniTwit.Api.Features.Followers.PostFollowUser
{
    public static class FollowUserEndpoints
    {
        /// <summary>
        /// Handles both follow and unfollow depending on the request body.
        /// </summary>
        public static IEndpointRouteBuilder MapFollowUserEndpoints(
            this IEndpointRouteBuilder routes
        )
        {
            routes.MapPost(
                "/fllws/{username}",
                async (
                    string username,
                    HttpRequest request,
                    MiniTwitDbContext db,
                    HybridCache hybridCache,
                    CancellationToken cancellationToken,
                    [FromQuery] int latest = -1
                ) =>
                {
                    string requestBody;
                    using (var reader = new StreamReader(request.Body))
                    {
                        requestBody = await reader.ReadToEndAsync(cancellationToken);
                    }
                    using var document = JsonDocument.Parse(requestBody);
                    var targetUsername = "";
                    var followAction = FollowAction.Follow;
                    if (document.RootElement.TryGetProperty("follow", out var followElement))
                    {
                        targetUsername = followElement.GetString();
                    }
                    else if (
                        document.RootElement.TryGetProperty("unfollow", out var unfollowElement)
                    )
                    {
                        targetUsername = unfollowElement.GetString();
                        followAction = FollowAction.Unfollow;
                    }
                    else
                    {
                        return Results.BadRequest("Invalid request body.");
                    }

                    var currentUser = await db.Users.FirstOrDefaultAsync(
                        u => u.Username == username,
                        cancellationToken
                    );
                    var targetUser = await db.Users.FirstOrDefaultAsync(
                        u => u.Username == targetUsername,
                        cancellationToken
                    );

                    if (currentUser == null || targetUser == null)
                    {
                        return Results.BadRequest("Invalid usernames.");
                    }

                    // Build the follow relationship.
                    var followRelation = new Follower
                    {
                        WhoId = currentUser.UserId,
                        WhomId = targetUser.UserId,
                    };
                    // Check if the follow relationship already exists.
                    var alreadyFollowing = await db.Followers.AnyAsync(
                        f => f.WhoId == currentUser.UserId && f.WhomId == targetUser.UserId,
                        cancellationToken
                    );

                    if (followAction == FollowAction.Unfollow && alreadyFollowing)
                    {
                        db.Followers.Remove(followRelation);
                        await db.SaveChangesAsync(cancellationToken);
                        // Invalidate all cache entries for the current user's follows.
                        await hybridCache.RemoveByTagAsync(
                            $"followers:{username}",
                            cancellationToken
                        );

                        await UpdateLatest.UpdateLatestStateAsync(latest, db, hybridCache, cancellationToken);
                        return Results.NoContent();
                    }

                    if (followAction == FollowAction.Follow && alreadyFollowing)
                    {
                        return Results.Conflict("Already following this user.");
                    }

                    db.Followers.Add(followRelation);
                    await db.SaveChangesAsync(cancellationToken);
                    // Invalidate all cache entries for the current user's follows.
                    await hybridCache.RemoveByTagAsync($"followers:{username}", cancellationToken);

                    await UpdateLatest.UpdateLatestStateAsync(latest, db, hybridCache, cancellationToken);
                    return Results.NoContent();
                }
            );

            return routes;
        }
    }

    internal enum FollowAction
    {
        Follow,
        Unfollow,
    }
}
