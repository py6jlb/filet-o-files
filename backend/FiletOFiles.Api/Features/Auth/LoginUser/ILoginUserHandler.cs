using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.DTOs.Auth;

namespace FiletOFiles.Api.Features.Auth.LoginUser;

public interface ILoginUserHandler
{
    Task<Result<AccessTokenDto>> Login(
        LoginUserDto request,
        CancellationToken cancellationToken = default
    );
}
