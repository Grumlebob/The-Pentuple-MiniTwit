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

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),
});

builder.Services.AddScoped<MiniTwitClient>(sp =>
{
    var httpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
    return new MiniTwitClient(httpClient);
});

builder.Services.AddScoped<IFollowerService>(sp => sp.GetRequiredService<MiniTwitClient>());
builder.Services.AddScoped<IUserServices>(sp => sp.GetRequiredService<MiniTwitClient>());
builder.Services.AddScoped<IMessageService>(sp => sp.GetRequiredService<MiniTwitClient>());



await builder.Build().RunAsync();
