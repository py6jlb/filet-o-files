using System;
using FiletOFiles.Api.Helpers;
using FiletOFiles.Api.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;

namespace FiletOFiles.Api.Features.Files.GetFile;

public static class GetFileEndpoint
{
    public static IEndpointRouteBuilder MapGetFile(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder
            .MapGet("/{fileId}", HandleAsync)
            .WithName(nameof(GetFileEndpoint))
            .WithDescription("Получить файл")
            .AllowAnonymous()
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        return endpointRouteBuilder;
    }

    public static async Task<IResult> HandleAsync(
        [FromServices] FilesService service,
        string fileId,
        CancellationToken cancellationToken
    )
    {
        var descr = await service.GetFileDescriptor(fileId, cancellationToken);
        if (descr.IsFailed)
        {
            return ErrorHelper.GetProblem(descr.Errors[0]);
        }
        var result = service.GetFileStream(descr.Value.Source);
        return result.IsSuccess
            ? TypedResults.File(
                result.Value,
                contentType: descr.Value.MimeType,
                fileDownloadName: descr.Value.FileName
            )
            : ErrorHelper.GetProblem(result.Errors[0]);
    }
}
