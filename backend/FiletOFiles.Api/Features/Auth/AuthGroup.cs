using System;
using FiletOFiles.Api.Features.Auth.Approve;
using FiletOFiles.Api.Features.Auth.GetRequests;
using FiletOFiles.Api.Features.Auth.Login;
using FiletOFiles.Api.Features.Auth.Refresh;
using FiletOFiles.Api.Features.Auth.Reject;
using FiletOFiles.Api.Features.Auth.TelegramCallback;

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
            .MapRefresh()
            .MapTelegramCallback()
            .MapGetRequests()
            .MapApprove()
            .MapReject();

        return endpointRouteBuilder;
    }
}
