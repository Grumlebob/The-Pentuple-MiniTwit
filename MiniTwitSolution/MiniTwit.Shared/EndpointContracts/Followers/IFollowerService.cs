// Co-authored by: ChatGPT (https://chat.openai.com/)

using MiniTwit.Shared.DTO.Followers.FollowUser;
using MiniTwit.Shared.DTO.Users;

namespace MiniTwit.Shared.EndpointContracts.Followers
{
    public interface IFollowerService
    {
        Task<HttpResponseMessage> FollowUserAsync(string currentUsername, FollowRequest followRequest);
        
        Task<HttpResponseMessage> UnfollowUserAsync(string currentUsername, UnfollowRequest unfollowRequest);
        
        /// <returns>List of usernames that follow current user </returns>
        Task<GetFollowersResponse> GetFollowersAsync(string currentUsername, int limit = 100);
    }
}
