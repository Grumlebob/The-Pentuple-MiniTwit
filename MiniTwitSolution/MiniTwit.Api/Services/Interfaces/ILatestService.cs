namespace MiniTwit.Api.Services.Interfaces
{
    public interface ILatestService
    {
        Task<IResult> GetLatestAsync(CancellationToken cancellationToken);
    }
}
