using System;
using FiletOFiles.Api.DTOs.AuthManagement;
using FiletOFiles.Api.Features.AuthManagement.Approve;
using FiletOFiles.Api.Features.AuthManagement.GetRequests;

namespace FiletOFiles.Api.Features.AuthManagement;

public static class AuthManagementGroup
{
    public static IEndpointRouteBuilder MapAuthManagementGroup(
        this IEndpointRouteBuilder endpointRouteBuilder
    )
    {
        endpointRouteBuilder
            .MapGroup("/auth-management")
            .WithOpenApi()
            .WithTags("AuthManagement")
            .RequireAuthorization(p => p.RequireRole(Roles.Admin))
            .MapGetRequests()
            .MapApprove();

        return endpointRouteBuilder;
    }
}
