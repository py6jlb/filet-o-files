using System;
using System.Linq.Expressions;
using FiletOFiles.Api.Domain.Entities;
using FiletOFiles.Api.DTOs.Auth;

namespace FiletOFiles.Api.DTOs.Users;

public static class UserMapping
{
    public static Expression<Func<User, UserDto>> ProjectToDto()
    {
        return u => new UserDto
        {
            Id = u.Id,
            Email = u.Email,
            CreatedAtUtc = u.CreatedAtUtc,
            Name = u.Name,
            UpdatedAtUtc = u.UpdatedAtUtc,
        };
    }

    public static User ToEntity(this RegisterUserDto dto)
    {
        return new User
        {
            Id = $"u_{Ulid.NewUlid()}",
            Email = dto.Email,
            Name = dto.Name,
            CreatedAtUtc = DateTime.UtcNow,
        };
    }
}
