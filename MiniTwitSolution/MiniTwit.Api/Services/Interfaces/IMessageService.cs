using MiniTwit.Shared.DTO.Messages;

namespace MiniTwit.Api.Services.Interfaces
{
    public interface IMessageService
    {
        Task<IResult> GetPublicMessagesAsync(
            int no,
            int latest,
            CancellationToken cancellationToken
        );
        Task<IResult> GetUserMessagesAsync(
            string username,
            int no,
            int latest,
            CancellationToken cancellationToken
        );
        Task<IResult> PostMessageAsync(
            string username,
            PostMessageRequest request,
            int latest,
            CancellationToken cancellationToken
        );
    }
}
