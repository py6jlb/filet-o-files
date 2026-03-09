using System;
using FiletOFiles.Api.Features.Files.DeleteFile;
using FiletOFiles.Api.Features.Files.GetFile;
using FiletOFiles.Api.Features.Files.UploadFile;

namespace FiletOFiles.Api.Features.Files;

public static class FilesGroup
{
    public static IEndpointRouteBuilder MapFilesGroup(
        this IEndpointRouteBuilder endpointRouteBuilder
    )
    {
        endpointRouteBuilder
            .MapGroup("/files")
            .WithOpenApi()
            .WithTags("Files")
            .RequireAuthorization()
            .MapDeleteFile()
            .MapUploadFile()
            .MapGetFile();

        return endpointRouteBuilder;
    }
}
