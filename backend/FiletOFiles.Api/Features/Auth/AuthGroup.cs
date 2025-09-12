using System;
using FiletOFiles.Api.Features.Auth.LoginUser;
using FiletOFiles.Api.Features.Auth.RegisterUser;

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
            .MapRegister();

        return endpointRouteBuilder;
    }
}
