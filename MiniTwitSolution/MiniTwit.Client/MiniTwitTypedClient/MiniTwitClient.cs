using System.Net.Http.Json;
using MiniTwit.Shared.DTO.Followers.FollowUser;
using MiniTwit.Shared.DTO.Messages;
using MiniTwit.Shared.DTO.Timeline;
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

    public async Task<HttpResponseMessage> FollowUserAsync(FollowRequest followRequest)
    {
        return await _httpClient.PostAsJsonAsync($"/fllws/{followRequest.Follow}", followRequest);
    }

    public async Task<HttpResponseMessage> UnfollowUserAsync(UnfollowRequest unfollowRequest)
    {
        return await _httpClient.PostAsJsonAsync($"/fllws/{unfollowRequest.Unfollow}", unfollowRequest);
    }

    public async Task<IList<GetUserResponse>> GetFollowersAsync(string username, int limit = 100)
    {
        var response = await _httpClient.GetAsync($"/fllws/{username}");
        if (response.IsSuccessStatusCode)
        {
            return (await response.Content.ReadFromJsonAsync<IList<GetUserResponse>>())!;
        }
        return [];
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
        var response = await _httpClient.GetAsync("/msgs");
        if (response.IsSuccessStatusCode)
        {
            return (await response.Content.ReadFromJsonAsync<IList<GetMessageResponse>>())!;
        }
        return [];
    }
    
    public async Task<IList<GetMessageResponse>> GetMessagesForUserAsync(string username, int limit = 100)
    {
        var response = await _httpClient.GetAsync($"/msgs/{username}");
        if (response.IsSuccessStatusCode)
        {
            return (await response.Content.ReadFromJsonAsync<IList<GetMessageResponse>>())!;
        }
        return [];
    }

    public async Task<HttpResponseMessage> PostMessageAsync(PostMessageRequest messageRequest)
    {
        return await _httpClient.PostAsJsonAsync($"/msgs/{messageRequest.AuthorUsername}", messageRequest);
    }
}