using System;
using FiletOFiles.Api.Features.Auth.Authorize;
using FiletOFiles.Api.Features.Auth.RegisterUser;
using FiletOFiles.Api.Features.Auth.Token;

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
            .MapToken()
            .MapAuthorize();

        return endpointRouteBuilder;
    }
}
