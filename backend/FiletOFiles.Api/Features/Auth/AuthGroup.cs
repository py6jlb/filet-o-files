using System;
using FiletOFiles.Api.Features.Auth.LoginUser;
using FiletOFiles.Api.Features.Auth.RegisterUser;
using FiletOFiles.Api.Features.Auth.TokenRefresh;

namespace FiletOFiles.Api.Features.Auth;

public static class AuthGroup
{
    public static IEndpointRouteBuilder MapAuthGroup(
        this IEndpointRouteBuilder endpointRouteBuilder
    )
    {
        endpointRouteBuilder
            .MapGroup("/auth")
            .WithOpenApi()
            .WithTags("Auth")
            .RequireAuthorization()
            .MapLogin()
            .MapRegister()
            .MapRefresh();

        return endpointRouteBuilder;
    }
}
