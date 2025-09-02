using System;
using FiletOFiles.Api.Domain.Entities;
using FiletOFiles.Api.Models;

namespace FiletOFiles.Api.Mappings;

internal static class FileMapping
{
    public static Domain.Entities.File ToFile(this AddFileRequest request)
    {
        return new()
        {
            FileName = request.FileName,
            IsTitle = request.IsTitle,
            MimeType = request.MimeType,
            Size = request.Size,
            Source = request.Source,
            RecipeId = request.RecipeId,
        };
    }

    public static GetFileResponse ToResponse(this Domain.Entities.File file)
    {
        return new(
            file.Id,
            file.FileName,
            file.MimeType ?? "application/octet-stream",
            file.Source,
            file.Size,
            file.IsTitle
        );
    }
}
