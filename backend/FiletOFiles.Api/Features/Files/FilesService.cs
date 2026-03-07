using System;
using System.Security.Cryptography;
using FiletOFiles.Api.DTOs.Files;
using FiletOFiles.Api.Infrastructure.Database;
using FiletOFiles.Api.Settings;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FiletOFiles.Api.Features.Files;

public sealed class FilesService
{
    private readonly AppDbContext _db;
    private readonly Filestorage _cfg;

    public FilesService(AppDbContext db, IOptions<Filestorage> cfg)
    {
        _cfg = cfg.Value;
        _db = db;
    }

    public async Task<Result> Upload(
        DTOs.Files.UploadFile request,
        CancellationToken cancellationToken = default
    )
    {
        var recipe = await _db.Recipes.FirstOrDefaultAsync(
            x => x.Id == request.RecipeId,
            cancellationToken
        );

        if (recipe is null)
        {
            return Result.Fail(new Error($"Не найден рецепт").WithMetadata("code", 404));
        }

        string sha1Hash;
        using (var stream = request.File.OpenReadStream())
        {
            using var sha1 = SHA1.Create();
            byte[] hashBytes = await sha1.ComputeHashAsync(stream, cancellationToken);
            sha1Hash = Convert.ToHexStringLower(hashBytes);
        }

        var path = Path.Combine(_cfg.Path, recipe.Id);
        var dirName = Path.GetDirectoryName(path);
        var dirInfo = Directory.CreateDirectory(path);
        var filePath = Path.Combine(path, sha1Hash);
        if (File.Exists(filePath))
        {
            return Result.Fail(
                new Error(
                    $"A file with the name '{request.File.FileName}' already exists."
                ).WithMetadata("code", 409)
            );
        }
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await request.File.CopyToAsync(stream, cancellationToken);
        }

        var newFile = new FileDto()
        {
            FileName = request.File.FileName,
            Source = filePath,
            MimeType = MimeTypes.GetMimeType(request.File.FileName),
            IsTitle = request.IsTitle,
            RecipeId = recipe.Id,
            Size = request.File.Length,
        };
        await _db.Files.AddAsync(newFile.ToEntity(), cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }

    public async Task<Result<FileDto>> GetFileDescriptor(
        string fileId,
        CancellationToken cancellationToken
    )
    {
        var fileDescr = await _db.Files.FirstOrDefaultAsync(x => x.Id == fileId, cancellationToken);
        if (fileDescr == null)
        {
            return Result.Fail<FileDto>(new Error("Файл не найден").WithMetadata("code", 404));
        }

        var result = fileDescr.ToDto();
        return Result.Ok(result);
    }

    public Result<FileStream> GetFileStream(string path)
    {
        if (!File.Exists(path))
        {
            return Result.Fail<FileStream>(new Error("Файл не найден").WithMetadata("code", 404));
        }
        var stream = File.OpenRead(path);
        return Result.Ok(stream);
    }

    public async Task<Result> DeleteFile(string fileId, CancellationToken cancellationToken)
    {
        var fileDescr = await _db.Files.FirstOrDefaultAsync(x => x.Id == fileId, cancellationToken);
        if (fileDescr == null)
        {
            return Result.Fail(new Error("Файл не найден").WithMetadata("code", 404));
        }

        var path = Path.Combine(_cfg.Path, fileDescr.Source);
        try
        {
            File.Delete(path);
        }
        catch (IOException ex)
        {
            return Result.Fail(
                new Error($"Error deleting file: {ex.Message}").WithMetadata("code", 500)
            );
        }
        catch (UnauthorizedAccessException ex)
        {
            return Result.Fail(new Error($"Access denied: {ex.Message}").WithMetadata("code", 500));
        }
        catch (Exception ex)
        {
            return Result.Fail(
                new Error($"An unexpected error occurred: {ex.Message}").WithMetadata("code", 500)
            );
        }
        _db.Files.Remove(fileDescr);
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}
