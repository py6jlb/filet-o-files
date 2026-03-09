using System;
using FiletOFiles.Api.Domain.Entities;
using FiletOFiles.Api.Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace FiletOFiles.Api.Features.AuthManagement.Approve;

public static class ApproveEndpoint
{
    public static IEndpointRouteBuilder MapApprove(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder
            .MapPost("/approve/{id}", HandleAsync)
            .WithName(nameof(ApproveEndpoint))
            .WithDescription("Подтверждение заявки на регистрацию")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        return endpointRouteBuilder;
    }

    public static async Task<IResult> HandleAsync(
        [FromRoute] string id,
        [FromServices] AppDbIdentityContext identityDb,
        [FromServices] UserManager<AppIdentityUser> userManager,
        CancellationToken cancellationToken = default
    )
    {
        var identityUser = await userManager.FindByIdAsync(id);
        if (identityUser == null)
        {
            return TypedResults.NotFound("Пользователь не найден");
        }

        using var transaction = await identityDb.Database.BeginTransactionAsync(cancellationToken);
        identityUser.IsApproved = true;
        await userManager.UpdateAsync(identityUser);
        await identityDb.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return TypedResults.NoContent();
    }
}
