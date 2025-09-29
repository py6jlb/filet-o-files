using System;
using FiletOFiles.Api.DTOs.Auth;
using FiletOFiles.Api.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace FiletOFiles.Api.Features.Auth.TelegramCallback;

public static class TelegramCallbackEndpoint
{
    public static IEndpointRouteBuilder MapTelegramCallback(
        this IEndpointRouteBuilder endpointRouteBuilder
    )
    {
        endpointRouteBuilder
            .MapPost("/tg/callback", HandleASync)
            .WithName(nameof(TelegramCallbackEndpoint))
            .AllowAnonymous()
            .WithDescription("telegram auth callback")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status202Accepted)
            .Produces(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        ;
        return endpointRouteBuilder;
    }

    public static async Task<IResult> HandleASync(
        [FromServices] AuthService service,
        [FromBody] TelegramPayload payload,
        CancellationToken cancellationToken = default
    )
    {
        var result = await service.HandleTelegramCallback(payload, cancellationToken);
        if (result.IsSuccess)
        {
            var code = (int?)
                result
                    .Reasons?[0].Metadata.Where(x => x.Key == "code")
                    .Select(x => x.Value)
                    .FirstOrDefault();
            if (code.HasValue && code.Value == 202)
            {
                var text = result.Reasons?[0].Message;
                return TypedResults.Accepted(text);
            }
            else
            {
                return TypedResults.Ok(result.Value);
            }
        }
        else
        {
            return ErrorHelper.GetProblem(result.Errors[0]);
        }
    }
}
