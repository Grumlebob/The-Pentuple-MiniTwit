using Blazored.LocalStorage;

namespace MiniTwit.Client.Authentication;

// We don't currently have authentication. We use local storage to handle the user sessions.
public class UserSession
{
    public bool IsLoggedIn { get; set; }
    public string Username { get; set; } = string.Empty;

    public async Task SaveAsync(ILocalStorageService localStorage)
    {
        await localStorage.SetItemAsync("UserSession", this);
    }

    public static async Task<UserSession> LoadAsync(ILocalStorageService localStorage)
    {
        return await localStorage.GetItemAsync<UserSession>("UserSession") ?? new UserSession();
    }
}
