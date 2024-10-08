﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace WtSbAssistant.BlazorUI.Areas.Identity.Pages.Account;

public class LoginWithRecoveryCodeModel(
    SignInManager<IdentityUser> signInManager,
    UserManager<IdentityUser> userManager,
    ILogger<LoginWithRecoveryCodeModel> logger)
    : PageModel
{
    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    [BindProperty]
    public InputModel Input { get; set; }

    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public string ReturnUrl { get; set; }

    public async Task<IActionResult> OnGetAsync(string returnUrl = null)
    {
        // Ensure the user has gone through the username & password screen first
        var user = await signInManager.GetTwoFactorAuthenticationUserAsync() ??
                   throw new InvalidOperationException("Unable to load two-factor authentication user.");
        ReturnUrl = returnUrl;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        if (!ModelState.IsValid) return Page();

        var user = await signInManager.GetTwoFactorAuthenticationUserAsync() ??
                   throw new InvalidOperationException("Unable to load two-factor authentication user.");
        var recoveryCode = Input.RecoveryCode.Replace(" ", string.Empty);

        var result = await signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

        var userId = await userManager.GetUserIdAsync(user);

        if (result.Succeeded)
        {
            logger.LogInformation("User with ID '{UserId}' logged in with a recovery code.", user.Id);
            return LocalRedirect(returnUrl ?? Url.Content("~/"));
        }

        if (result.IsLockedOut)
        {
            logger.LogWarning("User account locked out.");
            return RedirectToPage("./Lockout");
        }

        logger.LogWarning("Invalid recovery code entered for user with ID '{UserId}' ", user.Id);
        ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
        return Page();
    }

    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class InputModel
    {
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Recovery Code")]
        public string RecoveryCode { get; set; }
    }
}