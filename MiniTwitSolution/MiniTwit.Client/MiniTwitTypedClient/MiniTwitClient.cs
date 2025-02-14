using System.Net.Http.Json;
using MiniTwit.Shared.DTO.Followers.FollowUser;
using MiniTwit.Shared.DTO.Messages;
using MiniTwit.Shared.DTO.Users.Authentication.LoginUser;
using MiniTwit.Shared.DTO.Users.Authentication.RegisterUser;
using MiniTwit.Shared.EndpointContracts.Followers;
using MiniTwit.Shared.EndpointContracts.Messages;
using MiniTwit.Shared.EndpointContracts.Users;

namespace MiniTwit.Client.MiniTwitTypedClient;

public class MiniTwitClient : IFollowerService, IUserServices, IMessageService
{
    private readonly HttpClient _httpClient;

    public MiniTwitClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpResponseMessage> FollowUserAsync(string currentUsername, FollowRequest followRequest)
    {
        return await _httpClient.PostAsJsonAsync($"/fllws/{currentUsername}", followRequest);
    }

    public async Task<HttpResponseMessage> UnfollowUserAsync(string currentUsername, UnfollowRequest unfollowRequest)
    {
        return await _httpClient.PostAsJsonAsync($"/fllws/{currentUsername}", unfollowRequest);
    }

    public async Task<GetFollowersResponse> GetFollowersAsync(string currentUsername, int limit = 100)
    {
        var response = await _httpClient.GetAsync($"/fllws/{currentUsername}?no={limit}");
        if (response.IsSuccessStatusCode)
        {
            return (await response.Content.ReadFromJsonAsync<GetFollowersResponse>())!;
        }
        return new GetFollowersResponse([]);
    }

    public async Task<HttpResponseMessage> RegisterUserAsync(RegisterUserRequest registerRequest)
    {
        return await _httpClient.PostAsJsonAsync("/register", registerRequest);
    }

    public async Task<HttpResponseMessage> LoginUserAsync(LoginUserRequest loginRequest)
    {
        return await _httpClient.PostAsJsonAsync("/login", loginRequest);
    }

    public async Task<HttpResponseMessage> LogoutUserAsync()
    {
        return await _httpClient.GetAsync("/logout");
    }

    public async Task<IList<GetMessageResponse>> GetMessagesAsync(int limit = 100)
    {
        var response = await _httpClient.GetAsync($"/msgs?no={limit}");
        if (response.IsSuccessStatusCode)
        {
            return (await response.Content.ReadFromJsonAsync<IList<GetMessageResponse>>())!;
        }
        return [];
    }

    public async Task<IList<GetMessageResponse>> GetMessagesForUserAsync(string username, int limit = 100)
    {
        var response = await _httpClient.GetAsync($"/msgs/{username}?no={limit}");
        if (response.IsSuccessStatusCode)
        {
            return (await response.Content.ReadFromJsonAsync<IList<GetMessageResponse>>())!;
        }
        return [];
    }

    public async Task<HttpResponseMessage> PostMessageAsync(string currentUsername, PostMessageRequest messageRequest)
    {
        return await _httpClient.PostAsJsonAsync($"/msgs/{currentUsername}", messageRequest);
    }
}