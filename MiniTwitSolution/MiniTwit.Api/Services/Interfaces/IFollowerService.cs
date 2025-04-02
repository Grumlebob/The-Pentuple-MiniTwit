using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MiniTwit.Shared.DTO.Followers.FollowUser;

namespace MiniTwit.Api.Services.Interfaces
{
    public interface IFollowerService
    {
        Task<IActionResult> GetFollowersAsync(
            string username,
            int no,
            int latest,
            CancellationToken cancellationToken
        );
        Task<IActionResult> FollowOrUnfollowAsync(
            string username,
            FollowOrUnfollowRequest request,
            int latest,
            CancellationToken cancellationToken
        );
    }
}
