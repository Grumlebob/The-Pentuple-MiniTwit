using MiniTwit.Shared.DTO.Followers.FollowUser;

namespace MiniTwit.Api.Services.Interfaces
{
    public interface IFollowerService
    {
        Task<IResult> GetFollowersAsync(
            string username,
            int no,
            int latest,
            CancellationToken cancellationToken
        );

        Task<IResult> FollowOrUnfollowAsync(
            string username,
            FollowOrUnfollowRequest request,
            int latest,
            CancellationToken cancellationToken
        );
    }
}