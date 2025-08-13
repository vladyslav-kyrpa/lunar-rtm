using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using ServerApp.Services.Interfaces;

namespace ServerApp.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<UserProfileEntity> _userManager;
    private readonly SignInManager<UserProfileEntity> _signInManager;

    public AuthService(
        UserManager<UserProfileEntity> userManager,
        SignInManager<UserProfileEntity> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<bool> LoginUser(string username, string password)
    {
        var result = await _signInManager.PasswordSignInAsync(
            username,
            password,
            isPersistent: false,
            lockoutOnFailure: false
        );

        return result.Succeeded;
    }

    public async Task RegisterUser(string username, string password, string displayName, string email)
    {
        if (!InputValidator.IsUsernameValid(username))
            throw new ArgumentException(nameof(username), "username is invalid");
        if (!InputValidator.IsEmailValid(email))
            throw new ArgumentException(nameof(email), "email is invalid");
        if (await _userManager.FindByNameAsync(username) != null)
            throw new ArgumentException(nameof(username), "username is already exists");

        var user = new UserProfileEntity
        {
            UserName = username,
            Email = email,
            DisplayName = displayName
        };
        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
            throw new Exception("Failed to create new user profile");
    }

    public async Task LogoutUser()
    {
        await _signInManager.SignOutAsync();
    }
}

public class InputValidator
{
    public static bool IsUsernameValid(string value)
    {
        if (value.Length < 2 || value.Length > 64)
            return false;
        var allowedChars = "abcdefghijklmnopqrstuvwxyz1234567890.-_";
        foreach (var c in value)
            if(!allowedChars.Contains(c)) return false;
        return true;
    }

    public static bool IsEmailValid(string value)
        => Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
}