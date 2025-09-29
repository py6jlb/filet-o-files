using System;

namespace FiletOFiles.Api.Domain.Entities;

public class RegistrationRequest
{
    public string Id { get; set; }
    public string TelegramId { get; set; }
    public string TelegramUserName { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}
