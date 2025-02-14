// Co-authored by: ChatGPT (https://chat.openai.com/)

using MiniTwit.Shared.DTO.Followers.FollowUser;
using MiniTwit.Shared.DTO.Timeline;

namespace MiniTwit.Shared.EndpointContracts.Followers
{
    public interface IFollowerService
    {
        Task<HttpResponseMessage> FollowUserAsync(FollowRequest followRequest);
        
        Task<HttpResponseMessage> UnfollowUserAsync(UnfollowRequest unfollowRequest);

        /// <summary>
        /// Get the list of users that the specified user follows.
        /// </summary>
        /// <param name="username">
        /// The username whose following list is being requested.
        /// </param>
        /// <param name="limit">
        /// The maximum number of users to return (defaults to 100).
        /// </param>
        /// <returns>
        /// A GetFollowersResponse object containing a list of usernames under the "follows" property.
        /// </returns>
        Task<IList<GetUserResponse>> GetFollowersAsync(string username, int limit = 100);
    }
}
