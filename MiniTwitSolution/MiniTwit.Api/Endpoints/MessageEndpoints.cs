using Microsoft.AspNetCore.Mvc;
using MiniTwit.Api.Services.Interfaces;
using MiniTwit.Shared.DTO.Messages;

namespace MiniTwit.Api.Endpoints;

public static class MessageEndpoints
{
    public static IEndpointRouteBuilder MapMessageEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(
            "/msgs",
            async (
                IMessageService messageService,
                CancellationToken cancellationToken,
                [FromQuery] int no = 100,
                [FromQuery(Name = "latest")] int latest = -1
            ) => await messageService.GetPublicMessagesAsync(no, latest, cancellationToken)
        );

        endpoints.MapGet(
            "/msgs/{username}",
            async (
                string username,
                IMessageService messageService,
                CancellationToken cancellationToken,
                [FromQuery] int no = 100,
                [FromQuery(Name = "latest")] int latest = -1
            ) => await messageService.GetUserMessagesAsync(username, no, latest, cancellationToken)
        );

        endpoints.MapPost(
            "/msgs/{username}",
            async (
                string username,
                PostMessageRequest request,
                IMessageService messageService,
                CancellationToken cancellationToken,
                [FromQuery(Name = "latest")] int latest = -1
            ) => await messageService.PostMessageAsync(username, request, latest, cancellationToken)
        );

        return endpoints;
    }
}
