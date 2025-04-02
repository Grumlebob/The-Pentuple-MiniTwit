using Microsoft.AspNetCore.Mvc;
using MiniTwit.Shared.DTO.Followers.FollowUser;
using System.Threading;
using System.Threading.Tasks;

namespace MiniTwit.Api.Services.Interfaces
{
    public interface IFollowerService
    {
        Task<IActionResult> GetFollowersAsync(string username, int no, int latest, CancellationToken cancellationToken);
        Task<IActionResult> FollowOrUnfollowAsync(string username, FollowOrUnfollowRequest request, int latest, CancellationToken cancellationToken);
    }
}