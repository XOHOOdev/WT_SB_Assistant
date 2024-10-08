using Microsoft.AspNetCore.Identity;

namespace Sparta.BlazorUI.Data.UserManagementData;

public class UserSteamId
{
    public IdentityUser User { get; set; } = null!;
    public long SteamId { get; set; }
}