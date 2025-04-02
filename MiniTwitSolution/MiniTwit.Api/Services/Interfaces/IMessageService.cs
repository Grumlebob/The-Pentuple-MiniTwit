using MiniTwit.Shared.DTO.Messages;

namespace MiniTwit.Api.Services.Interfaces
{
    public interface IMessageService
    {
        Task<IList<GetMessageResponse>> GetMessagesForUserAsync(
            string currentUsername,
            int limit = 100
        );
        Task<IList<GetMessageResponse>> GetMessagesAsync(int limit = 100);

        Task<HttpResponseMessage> PostMessageAsync(
            string currentUsername,
            PostMessageRequest messageRequest
        );
    }
}
