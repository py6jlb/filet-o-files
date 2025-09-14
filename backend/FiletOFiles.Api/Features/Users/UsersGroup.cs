using System;
using FiletOFiles.Api.Features.Users.GetUser;

namespace FiletOFiles.Api.Features.Users;

public static class UserGroup
{
    public static IEndpointRouteBuilder MapUsersGroup(
        this IEndpointRouteBuilder endpointRouteBuilder
    )
    {
        endpointRouteBuilder
            .MapGroup("/users")
            .WithOpenApi()
            .WithTags("Users")
            .RequireAuthorization()
            .MapGetUser();

        return endpointRouteBuilder;
    }
}
