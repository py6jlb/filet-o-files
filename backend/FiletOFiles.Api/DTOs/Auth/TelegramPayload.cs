using System;

namespace FiletOFiles.Api.DTOs.Auth;

public sealed record TelegramPayload
{
#pragma warning disable IDE1006 // Naming Styles
    public string id { get; set; }
#pragma warning restore IDE1006 // Naming Styles
#pragma warning disable IDE1006 // Naming Styles
    public string username { get; set; }
#pragma warning restore IDE1006 // Naming Styles
#pragma warning disable IDE1006 // Naming Styles
    public string first_name { get; set; }
#pragma warning restore IDE1006 // Naming Styles
#pragma warning disable IDE1006 // Naming Styles
    public string last_name { get; set; }
#pragma warning restore IDE1006 // Naming Styles
#pragma warning disable IDE1006 // Naming Styles
    public string auth_date { get; set; }
#pragma warning restore IDE1006 // Naming Styles
#pragma warning disable IDE1006 // Naming Styles
    public string hash { get; set; }
#pragma warning restore IDE1006 // Naming Styles
}
