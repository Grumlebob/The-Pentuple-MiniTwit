using Microsoft.AspNetCore.Mvc;
using MiniTwit.Api.Services.Interfaces;
using MiniTwit.Shared.DTO.Messages;

namespace MiniTwit.Api.Controllers;

[ApiController]
[Route("msgs")]
public class MessageController(IMessageService messageService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetPublicMessages(
        [FromQuery] int no = 100,
        [FromQuery] int latest = -1,
        CancellationToken cancellationToken = default
    )
    {
        return await messageService.GetPublicMessagesAsync(no, latest, cancellationToken);
    }

    [HttpGet("{username}")]
    public async Task<IActionResult> GetUserMessages(
        string username,
        [FromQuery] int no = 100,
        [FromQuery] int latest = -1,
        CancellationToken cancellationToken = default
    )
    {
        return await messageService.GetUserMessagesAsync(username, no, latest, cancellationToken);
    }

    [HttpPost("{username}")]
    public async Task<IActionResult> PostMessage(
        string username,
        [FromBody] PostMessageRequest request,
        [FromQuery] int latest = -1,
        CancellationToken cancellationToken = default
    )
    {
        return await messageService.PostMessageAsync(username, request, latest, cancellationToken);
    }
}
