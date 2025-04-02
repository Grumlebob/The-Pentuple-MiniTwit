using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Hybrid;
using MiniTwit.Api.Services.Interfaces;
using MiniTwit.Api.Utility;
using MiniTwit.Shared.DTO.Followers.FollowUser;

namespace MiniTwit.Api.Services;

public class FollowerService : IFollowerService
{
    private readonly MiniTwitDbContext _db;
    private readonly HybridCache _cache;

    public FollowerService(MiniTwitDbContext db, HybridCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<IActionResult> GetFollowersAsync(
        string username,
        int no,
        int latest,
        CancellationToken cancellationToken
    )
    {
        var user = await _db.Users.FirstOrDefaultAsync(
            u => u.Username == username,
            cancellationToken
        );
        if (user == null)
        {
            return new NotFoundObjectResult("User not found.");
        }

        var cacheKey = $"followers:{username}:{no}";
        var response = await _cache.GetOrCreateAsync<GetFollowersResponse>(
            cacheKey,
            async ct =>
            {
                var followUsernames = await (
                    from f in _db.Followers
                    join u in _db.Users on f.WhomId equals u.UserId
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

        await UpdateLatest.UpdateLatestStateAsync(latest, _db, _cache, cancellationToken);
        return new OkObjectResult(response);
    }

    public async Task<IActionResult> FollowOrUnfollowAsync(
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
            return new BadRequestObjectResult(
                "You must provide either 'follow' or 'unfollow', but not both."
            );
        }

        var targetUsername = request.Follow ?? request.Unfollow!;
        var followAction = request.Follow is not null;

        var currentUser = await _db.Users.FirstOrDefaultAsync(
            u => u.Username == username,
            cancellationToken
        );
        var targetUser = await _db.Users.FirstOrDefaultAsync(
            u => u.Username == targetUsername,
            cancellationToken
        );

        if (currentUser == null || targetUser == null)
        {
            return new BadRequestObjectResult("Invalid usernames");
        }

        var followRelation = new Follower
        {
            WhoId = currentUser.UserId,
            WhomId = targetUser.UserId,
        };

        var alreadyFollowing = await _db.Followers.AnyAsync(
            f => f.WhoId == currentUser.UserId && f.WhomId == targetUser.UserId,
            cancellationToken
        );

        if (!followAction && alreadyFollowing)
        {
            _db.Followers.Remove(followRelation);
            await _db.SaveChangesAsync(cancellationToken);
            await _cache.RemoveByTagAsync($"followers:{username}", cancellationToken);
            await UpdateLatest.UpdateLatestStateAsync(latest, _db, _cache, cancellationToken);
            return new NoContentResult();
        }

        if (followAction && alreadyFollowing)
        {
            return new ConflictObjectResult("Already following this user.");
        }

        _db.Followers.Add(followRelation);
        await _db.SaveChangesAsync(cancellationToken);
        await _cache.RemoveByTagAsync($"followers:{username}", cancellationToken);
        await UpdateLatest.UpdateLatestStateAsync(latest, _db, _cache, cancellationToken);
        return new NoContentResult();
    }
}
