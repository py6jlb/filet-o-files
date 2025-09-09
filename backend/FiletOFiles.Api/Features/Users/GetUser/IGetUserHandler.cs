using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.DTOs.Users;

namespace FiletOFiles.Api.Features.Users.GetUser;

public interface IGetUserHandler
{
    Task<Result<UserDto>> GetUser(string userId, CancellationToken cancellationToken = default);
}
