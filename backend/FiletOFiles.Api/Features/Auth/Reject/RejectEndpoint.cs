using System;
using FiletOFiles.Api.Domain.Entities;
using FiletOFiles.Api.Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace FiletOFiles.Api.Features.Auth.Reject;

public static class RejectEndpoint
{
    public static IEndpointRouteBuilder MapReject(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder
            .MapPost("/reject/{id}", HandleAsync)
            .WithName(nameof(RejectEndpoint))
            .WithDescription("Отклонение заявки на регистрацию")
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
        var request = await identityDb
            .RegistrationRequests.Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        if (request == null)
        {
            return TypedResults.NotFound("Заявка с указанным id не найдена");
        }

        var identityUser = await userManager.FindByNameAsync(request.TelegramUserName);
        if (identityUser == null)
        {
            return TypedResults.NotFound("Пользователь не найден");
        }

        using var transaction = await identityDb.Database.BeginTransactionAsync(cancellationToken);
        identityUser.IsApproved = true;
        await userManager.UpdateAsync(identityUser);
        identityDb.RegistrationRequests.Remove(request);
        await identityDb.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return TypedResults.NoContent();
    }
}
