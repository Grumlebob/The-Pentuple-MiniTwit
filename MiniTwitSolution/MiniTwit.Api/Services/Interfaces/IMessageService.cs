using Microsoft.AspNetCore.Mvc;
using MiniTwit.Shared.DTO.Messages;

namespace MiniTwit.Api.Services.Interfaces
{
    public interface IMessageService
    {
        Task<IActionResult> GetPublicMessagesAsync(int no, int latest, CancellationToken cancellationToken);
        Task<IActionResult> GetUserMessagesAsync(string username, int no, int latest, CancellationToken cancellationToken);
        Task<IActionResult> PostMessageAsync(string username, PostMessageRequest request, int latest, CancellationToken cancellationToken);
    }
}
