using MiniTwit.Shared.DTO.Messages;

namespace MiniTwit.Shared.EndpointContracts.Messages
{
    /// <summary>
    /// Provides methods for interacting with the messages endpoints.
    /// </summary>
    public interface IMessageService
    {
        Task<IList<GetMessageResponse>> GetMessagesForUserAsync(string currentUsername, int limit = 100);

        /// <summary>
        /// Retrieves messages for all users.
        /// </summary>
        Task<IList<GetMessageResponse>> GetMessagesAsync(int limit = 100);
        
        Task<HttpResponseMessage> PostMessageAsync(string currentUsername, PostMessageRequest messageRequest);
    }
}