using MiniTwit.Api.Services.Interfaces;

namespace MiniTwit.Api.Endpoints;

public static class LatestEndpoints
{
    public static IEndpointRouteBuilder MapLatestEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/latest", async (
            ILatestService latestService,
            CancellationToken cancellationToken
        ) => await latestService.GetLatestAsync(cancellationToken));

        return endpoints;
    }
}