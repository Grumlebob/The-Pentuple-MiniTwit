﻿using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MiniTwit.Client.MiniTwitTypedClient;
using MiniTwit.Shared.EndpointContracts.Followers;
using MiniTwit.Shared.EndpointContracts.Messages;
using MiniTwit.Shared.EndpointContracts.Users;

namespace MiniTwit.Client.DependencyInjection;

public static class TypedClientConfiguration
{
    public static IServiceCollection AddTypedClient(this IServiceCollection services, WebAssemblyHostConfiguration configuration)
    {
        var apiBaseUrl = configuration["ApiBaseUrl"];
        if (string.IsNullOrEmpty(apiBaseUrl))
        {
            throw new Exception(
                "API base URL is not configured. Please set ApiBaseUrl in appsettings.json."
            );
        }

        services.AddScoped<MiniTwitClient>(_ =>
        {
            var httpClient = new HttpClient { BaseAddress = new Uri(apiBaseUrl) };
            return new MiniTwitClient(httpClient);
        });
        services.AddScoped<IFollowerService>(sp => sp.GetRequiredService<MiniTwitClient>());
        services.AddScoped<IUserServices>(sp => sp.GetRequiredService<MiniTwitClient>());
        services.AddScoped<IMessageService>(sp => sp.GetRequiredService<MiniTwitClient>());
        return services;
    }
}