﻿using Microsoft.AspNetCore.Identity;

namespace WtSbAssistant.BlazorUI.Data.UserManagementData;

public class UserRoleModel
{
    public IdentityUser User { get; set; } = null!;

    public IEnumerable<UserRoleEntryModel> Roles { get; set; } = null!;
}