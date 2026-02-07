using System;
using FiletOFiles.Api.Features.Auth.Login;
using FiletOFiles.Api.Features.Auth.Refresh;
using FiletOFiles.Api.Features.Auth.Register;

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
            .MapLogin()
            .MapRefresh()
            .MapRegister();

        return endpointRouteBuilder;
    }
}
