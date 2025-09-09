using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.DTOs.Auth;

namespace FiletOFiles.Api.Features.Auth.RegisterUser;

public interface IRegisterUserHandler
{
    Task<Result<AccessTokenDto>> Register(
        RegisterUserDto request,
        CancellationToken cancellationToken = default
    );
}
