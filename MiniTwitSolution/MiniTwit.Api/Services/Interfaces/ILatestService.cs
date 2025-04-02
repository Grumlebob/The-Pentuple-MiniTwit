using Microsoft.AspNetCore.Mvc;
using MiniTwit.Shared.DTO.Latest;

namespace MiniTwit.Api.Services.Interfaces
{
    public interface ILatestService
    {
        Task<IActionResult> GetLatestAsync(CancellationToken cancellationToken);
    }
}
