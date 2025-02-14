//Co-authored by: ChatGPT (https://chat.openai.com/)
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Hybrid;
using MiniTwit.Shared.DTO.Followers.FollowUser;


namespace MiniTwit.Api.Features.Followers.GetFollowers
{
    public static class Endpoint
    {
        public static IEndpointRouteBuilder MapGetFollowersEndpoints(this IEndpointRouteBuilder routes)
        {
            routes.MapGet(
                "/fllws/{username}",
                async (
                    string username,
                    MiniTwitDbContext db,
                    HybridCache hybridCache,
                    CancellationToken cancellationToken,
                    [FromQuery] int no = 100) =>
                {
                    // Retrieve the user by username.
                    var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
                    if (user == null)
                    {
                        return Results.NotFound("User not found.");
                    }

                    // Define a cache key that uniquely identifies this query.
                    var cacheKey = $"followers:{username}:{no}";

                    // Retrieve or create the cached response.
                    var response = await hybridCache.GetOrCreateAsync<GetFollowersResponse>(
                        cacheKey,
                        async ct =>
                        {
                            // Query the database to get the list of usernames that this user follows.
                            var followUsernames = await (
                                from f in db.Followers
                                join u in db.Users on f.WhomId equals u.UserId
                                where f.WhoId == user.UserId
                                select u.Username
                            ).Take(no).ToListAsync(ct);

                            return new GetFollowersResponse(followUsernames);
                        },
                        cancellationToken: cancellationToken
                    );

                    return Results.Json(response);
                }
            );

            return routes;
        }
    }
}
