using System;
using FiletOFiles.Api.Domain.Entities;

namespace FiletOFiles.Api.DTOs.Auth;

public static class AuthMapping
{
    public static RegistrationRequestDto ToRegistrationRequestDto(this RegistrationRequest request)
    {
        return new RegistrationRequestDto
        {
            Id = request.Id,
            CreatedAtUtc = request.CreatedAtUtc,
            TelegramId = request.TelegramId,
            TelegramUserName = request.TelegramUserName,
        };
    }
}
