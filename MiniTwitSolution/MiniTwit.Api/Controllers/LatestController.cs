using Microsoft.AspNetCore.Mvc;
using MiniTwit.Api.Services.Interfaces;

namespace MiniTwit.Api.Controllers;

[ApiController]
[Route("latest")]
public class LatestController(ILatestService latestService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetLatest(CancellationToken cancellationToken = default)
    {
        return await latestService.GetLatestAsync(cancellationToken);
    }
}