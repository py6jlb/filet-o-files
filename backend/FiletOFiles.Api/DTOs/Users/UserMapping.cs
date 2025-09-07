using System;
using System.Linq.Expressions;
using FiletOFiles.Api.Domain.Entities;

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
}
