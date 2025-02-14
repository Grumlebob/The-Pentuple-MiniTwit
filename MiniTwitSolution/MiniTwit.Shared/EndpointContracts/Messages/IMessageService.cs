using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MiniTwit.Shared.DTO.Messages;
using MiniTwit.Shared.DTO.Timeline;

namespace MiniTwit.Shared.EndpointContracts.Messages
{
    /// <summary>
    /// Provides methods for interacting with the messages endpoints.
    /// </summary>
    public interface IMessageService
    {
        Task<IList<GetMessageResponse>> GetMessagesForUserAsync(string username, int limit = 100);

        /// <summary>
        /// Retrieves messages for all users.
        /// </summary>
        Task<IList<GetMessageResponse>> GetMessagesAsync(int limit = 100);
        
        Task<HttpResponseMessage> PostMessageAsync(PostMessageRequest messageRequest);
    }
}