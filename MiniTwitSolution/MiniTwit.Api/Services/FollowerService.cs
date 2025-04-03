using Microsoft.Extensions.Caching.Hybrid;
using MiniTwit.Api.Services.Interfaces;
using MiniTwit.Api.Utility;
using MiniTwit.Shared.DTO.Followers.FollowUser;

namespace MiniTwit.Api.Services;

public class FollowerService(MiniTwitDbContext db, HybridCache cache) : IFollowerService
{
    public async Task<IResult> GetFollowersAsync(
        string username,
        int no,
        int latest,
        CancellationToken cancellationToken
    )
    {
        var user = await db.Users.FirstOrDefaultAsync(
            u => u.Username == username,
            cancellationToken
        );
        if (user == null)
        {
            return Results.NotFound("User not found.");
        }

        var cacheKey = $"followers:{username}:{no}";
        var response = await cache.GetOrCreateAsync<GetFollowersResponse>(
            cacheKey,
            async ct =>
            {
                var followUsernames = await (
                    from f in db.Followers
                    join u in db.Users on f.WhomId equals u.UserId
                    where f.WhoId == user.UserId
                    select u.Username
                )
                    .Take(no)
                    .ToListAsync(ct);

                return new GetFollowersResponse(followUsernames);
            },
            new HybridCacheEntryOptions
            {
                LocalCacheExpiration = TimeSpan.FromMinutes(5),
                Expiration = TimeSpan.FromMinutes(5),
            },
            cancellationToken: cancellationToken,
            tags: new[] { $"followers:{username}" }
        );

        await UpdateLatest.UpdateLatestStateAsync(latest, db, cache, cancellationToken);
        return Results.Ok(response);
    }

    public async Task<IResult> FollowOrUnfollowAsync(
        string username,
        FollowOrUnfollowRequest request,
        int latest,
        CancellationToken cancellationToken
    )
    {
        if (
            (request.Follow is not null && request.Unfollow is not null)
            || (request.Follow is null && request.Unfollow is null)
        )
        {
            return Results.BadRequest("You must provide either 'follow' or 'unfollow', but not both.");
        }

        var targetUsername = request.Follow ?? request.Unfollow!;
        var followAction = request.Follow is not null;

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
            return Results.BadRequest("Invalid usernames");
        }

        var alreadyFollowing = await db.Followers.AnyAsync(
            f => f.WhoId == currentUser.UserId && f.WhomId == targetUser.UserId,
            cancellationToken
        );

        if (!followAction && alreadyFollowing)
        {
            db.Followers.Remove(new Follower
            {
                WhoId = currentUser.UserId,
                WhomId = targetUser.UserId
            });
            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync($"followers:{username}", cancellationToken);
            await UpdateLatest.UpdateLatestStateAsync(latest, db, cache, cancellationToken);
            return Results.NoContent();
        }

        if (followAction && alreadyFollowing)
        {
            return Results.Conflict("Already following this user.");
        }

        db.Followers.Add(new Follower
        {
            WhoId = currentUser.UserId,
            WhomId = targetUser.UserId
        });
        await db.SaveChangesAsync(cancellationToken);
        await cache.RemoveByTagAsync($"followers:{username}", cancellationToken);
        await UpdateLatest.UpdateLatestStateAsync(latest, db, cache, cancellationToken);

        return Results.NoContent();
    }
}
