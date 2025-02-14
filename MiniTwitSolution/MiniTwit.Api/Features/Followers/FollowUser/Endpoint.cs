using System.Text.Json;
using MiniTwit.Shared.DTO.Followers.FollowUser;

namespace MiniTwit.Api.Features.Followers.FollowUser
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
                    CancellationToken cancellationToken
                ) =>
                {
                    using var doc = await JsonDocument.ParseAsync(request.Body, cancellationToken: cancellationToken);
                    var json = doc.RootElement.GetRawText();
                    var targetUsername = "";
                    var followAction = FollowAction.Follow;
                    try
                    {
                        var followRequest = JsonSerializer.Deserialize<FollowRequest>(json);
                        targetUsername = followRequest!.Follow;
                    }
                    catch (JsonException)
                    {
                        try
                        {
                            var unfollowRequest = JsonSerializer.Deserialize<UnfollowRequest>(json);
                            targetUsername = unfollowRequest!.Unfollow;
                            followAction = FollowAction.Unfollow;
                        }
                        catch (JsonException)
                        {
                            return Results.BadRequest("Invalid request body.");
                        }
                        
                    }

                    var currentUser = await db.Users.Where(u => u.Username == username)
                        .FirstOrDefaultAsync(cancellationToken: cancellationToken);
                    var targetUser = await db.Users.Where(u => u.Username == targetUsername)
                        .FirstOrDefaultAsync(cancellationToken: cancellationToken);

                    if (currentUser == null || targetUser == null)
                    {
                        return Results.BadRequest("Invalid usernames.");
                    }

                    if (followAction == FollowAction.Unfollow)
                    {
                        db.Followers.Remove(new Follower
                        {
                            WhoId = currentUser.UserId,
                            WhomId = targetUser.UserId
                        });
                        await db.SaveChangesAsync(cancellationToken);
                        return Results.NoContent();
                    }

                    // Check if the follow relationship already exists.
                    var alreadyFollowing = await db.Followers.AnyAsync(
                        f => f.WhoId == currentUser.UserId && f.WhomId == targetUser.UserId,
                        cancellationToken
                    );
                    if (alreadyFollowing)
                    {
                        return Results.Conflict("Already following this user.");
                    }

                    // Create the follow relationship.
                    var newFollower = new Follower
                    {
                        WhoId = currentUser.UserId,
                        WhomId = targetUser.UserId
                    };

                    db.Followers.Add(newFollower);
                    await db.SaveChangesAsync(cancellationToken);

                    return Results.NoContent();
                }
            );

            return routes;
        }
    }
    
}

internal enum FollowAction
{
    Follow,
    Unfollow
}