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

        string sha1Hash = await ComputeSha1HashAsync(request.File, cancellationToken);

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

        // Сохраняем превью, если оно есть
        string? previewFileId = null;
        if (request.Preview != null && request.Preview.Length > 0)
        {
            var previewHash = await ComputeSha1HashAsync(request.Preview, cancellationToken);
            var previewFilePath = Path.Combine(path, previewHash);

            if (!File.Exists(previewFilePath))
            {
                using var previewStream = new FileStream(previewFilePath, FileMode.Create);
                await request.Preview.CopyToAsync(previewStream, cancellationToken);
            }

            var previewFileDto = new FileDto()
            {
                FileName = request.Preview.FileName,
                Source = previewFilePath,
                MimeType = MimeTypes.GetMimeType(request.Preview.FileName),
                IsTitle = false,
                RecipeId = recipe.Id,
                Size = request.Preview.Length,
            };
            var previewEntity = previewFileDto.ToEntity();
            await _db.Files.AddAsync(previewEntity, cancellationToken);
            previewFileId = previewEntity.Id;
        }

        var newFile = new FileDto()
        {
            FileName = request.File.FileName,
            Source = filePath,
            MimeType = MimeTypes.GetMimeType(request.File.FileName),
            IsTitle = request.IsTitle,
            RecipeId = recipe.Id,
            Size = request.File.Length,
            PreviewFileId = previewFileId,
        };
        await _db.Files.AddAsync(newFile.ToEntity(), cancellationToken);

        //сохраняем все добавленное
        await _db.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }

    private static async Task<string> ComputeSha1HashAsync(
        IFormFile file,
        CancellationToken cancellationToken
    )
    {
        using var stream = file.OpenReadStream();
        using var sha1 = SHA1.Create();
        byte[] hashBytes = await sha1.ComputeHashAsync(stream, cancellationToken);
        return Convert.ToHexStringLower(hashBytes);
    }

    public async Task<Result<FileDto>> GetFileDescriptor(
        string fileId,
        CancellationToken cancellationToken
    )
    {
        var fileDescr = await _db
            .Files.Include(f => f.PreviewFile)
            .FirstOrDefaultAsync(x => x.Id == fileId, cancellationToken);
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
        var fileDescr = await _db
            .Files.Include(f => f.PreviewFile)
            .FirstOrDefaultAsync(x => x.Id == fileId, cancellationToken);

        if (fileDescr == null)
        {
            return Result.Fail(new Error("Файл не найден").WithMetadata("code", 404));
        }

        // Удаляем основной файл
        var path = Path.Combine(_cfg.Path, fileDescr.Source);
        try
        {
            var attr = File.GetAttributes(path);
            if (!attr.HasFlag(FileAttributes.Directory))
            {
                File.Delete(path);
            }
        }
        catch (IOException ex)
        {
            // Игнорируем ошибки удаления основного файла
        }
        catch (UnauthorizedAccessException ex)
        {
            // Игнорируем ошибки доступа
        }

        // Удаляем превью, если есть
        if (fileDescr.PreviewFile != null)
        {
            var previewPath = Path.Combine(_cfg.Path, fileDescr.PreviewFile.Source);
            try
            {
                var attr = File.GetAttributes(previewPath);
                if (!attr.HasFlag(FileAttributes.Directory))
                {
                    File.Delete(previewPath);
                }
            }
            catch (IOException ex)
            {
                // Игнорируем ошибки удаления превью
            }
            catch (UnauthorizedAccessException ex)
            {
                // Игнорируем ошибки доступа
            }

            _db.Files.Remove(fileDescr.PreviewFile);
        }

        _db.Files.Remove(fileDescr);
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}
