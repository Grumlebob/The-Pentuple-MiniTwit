using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MiniTwit.Client;
using MiniTwit.Client.Authentication;
using MiniTwit.Client.MiniTwitTypedClient;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),
});

var apiBaseUrl = builder.Configuration["ApiBaseUrl"];
if (string.IsNullOrEmpty(apiBaseUrl))
{
    throw new Exception(
        "API base URL is not configured. Please set ApiBaseUrl in appsettings.json."
    );
}

builder.Services.AddScoped<MiniTwitClient>(sp =>
{
    var httpClient = new HttpClient { BaseAddress = new Uri(apiBaseUrl) };
    return new MiniTwitClient(httpClient);
});

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddSingleton<UserSession>();

await builder.Build().RunAsync();
