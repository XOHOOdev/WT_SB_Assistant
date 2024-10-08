using Microsoft.AspNetCore.Identity;

namespace WtSbAssistant.BlazorUI.Data.UserManagementData;

public class UserSteamId
{
    public IdentityUser User { get; set; } = null!;
    public long SteamId { get; set; }
}