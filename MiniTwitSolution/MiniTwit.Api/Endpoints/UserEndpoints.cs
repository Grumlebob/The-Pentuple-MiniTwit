using Microsoft.AspNetCore.Mvc;
using MiniTwit.Api.Services.Interfaces;
using MiniTwit.Shared.DTO.Users.Authentication.LoginUser;
using MiniTwit.Shared.DTO.Users.Authentication.RegisterUser;

namespace MiniTwit.Api.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(
            "/register",
            async (
                RegisterUserRequest request,
                IUserService userService,
                CancellationToken cancellationToken,
                [FromQuery(Name = "latest")] int latest = -1
            ) => await userService.RegisterUserAsync(request, latest, cancellationToken)
        );

        endpoints.MapPost(
            "/login",
            async (
                LoginUserRequest request,
                IUserService userService,
                CancellationToken cancellationToken,
                [FromQuery(Name = "latest")] int latest = -1
            ) => await userService.LoginAsync(request, latest, cancellationToken)
        );

        endpoints.MapPost(
            "/logout",
            async (
                IUserService userService,
                CancellationToken cancellationToken,
                [FromQuery(Name = "latest")] int latest = -1
            ) => await userService.LogoutUserAsync(latest, cancellationToken)
        );

        return endpoints;
    }
}
