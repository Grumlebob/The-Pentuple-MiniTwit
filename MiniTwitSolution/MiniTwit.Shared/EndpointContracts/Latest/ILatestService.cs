using MiniTwit.Shared.DTO.Latest;

namespace MiniTwit.Shared.EndpointContracts.Latest
{
    public interface ILatestService
    {
        /// <returns>The latest event ID.</returns>
        Task<GetLatestResponse> GetLatestAsync();

        //Task<HttpResponseMessage> UpdateLatestAsync(int latest);
    }
}
