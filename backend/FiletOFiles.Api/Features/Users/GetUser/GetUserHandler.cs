using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.DTOs.Users;
using FiletOFiles.Api.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace FiletOFiles.Api.Features.Users.GetUser;

public class GetUserHandler : IGetUserHandler
{
    private readonly ILogger<GetUserHandler> _logger;
    private readonly AppDbContext _db;

    public GetUserHandler(ILogger<GetUserHandler> logger, AppDbContext db)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Result<UserDto>> GetUser(
        string userId,
        CancellationToken cancellationToken = default
    )
    {
        var user = await _db
            .Users.Where(u => u.Id == userId)
            .Select(UserMapping.ProjectToDto())
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
        {
            return Result.Failure<UserDto>("Пользователь не найден");
        }

        return Result.Success(user);
    }
}
