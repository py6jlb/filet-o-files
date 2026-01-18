using System;

namespace FiletOFiles.Api.DTOs.AuthManagement;

public sealed record NewRegisterRequestDto
{
    public required string Email { get; set; }
}
