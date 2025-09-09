using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.DTOs.Auth;

namespace FiletOFiles.Api.Features.Auth.TokenRefresh;

public interface IRefreshTokenHandler
{
    Task<Result<AccessTokenDto>> Refresh(
        RefreshTokenDto request,
        CancellationToken cancellationToken = default
    );
}
