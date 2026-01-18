using System;

namespace FiletOFiles.Api.DTOs.AuthManagement;

public sealed record RegistrationRequestDto
{
    public string Id { get; set; }
    public bool IsApproved { get; set; }
    public string TelegramId { get; set; }
    public string TelegramUserName { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}
