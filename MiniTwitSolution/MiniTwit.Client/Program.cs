using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MiniTwit.Client;
using MiniTwit.Client.MiniTwitTypedClient;
using MiniTwit.Shared.EndpointContracts.Followers;
using MiniTwit.Shared.EndpointContracts.Messages;
using MiniTwit.Shared.EndpointContracts.Users;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Basic HttpClient that is used by our typed Client
builder.Services.AddScoped(_ => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

builder.Services.AddTypedClient(builder.Configuration)
    .AddBlazoredLocalStorage()
    .AddSingleton<UserSession>();

await builder.Build().RunAsync();
