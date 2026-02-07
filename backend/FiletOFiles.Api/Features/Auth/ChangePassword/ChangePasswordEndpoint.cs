using System;
using FiletOFiles.Api.DTOs.Auth;
using FiletOFiles.Api.Extensions;
using FiletOFiles.Api.Helpers;
using FiletOFiles.Api.Infrastructure.Database;
using FiletOFiles.Api.Services;
using FiletOFiles.Api.Settings;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FiletOFiles.Api.Features.Auth.ChangePassword;

public static class ChangePasswordEndpoint
{
    public static IEndpointRouteBuilder MapChangePassword(
        this IEndpointRouteBuilder endpointRouteBuilder
    )
    {
        endpointRouteBuilder
            .MapPost("/change-password", Handle)
            .WithName(nameof(ChangePasswordEndpoint))
            .WithDescription("Смена пароля")
            .Produces<AccessTokenDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        return endpointRouteBuilder;
    }

    public static async Task<IResult> Handle(
        HttpContext context,
        [FromServices] AuthService service,
        [FromBody] ChangePasswordDto request,
        CancellationToken cancellationToken = default
    )
    {
        string userId = context.GetUserId();
        var result = await service.ChangePassword(userId, request);
        return result.IsSuccess ? TypedResults.Ok() : ErrorHelper.GetProblem(result.Errors[0]);
    }
}
