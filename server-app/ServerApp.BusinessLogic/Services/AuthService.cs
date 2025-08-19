using Microsoft.AspNetCore.Identity;
using ServerApp.BusinessLogic.Common;
using ServerApp.BusinessLogic.Services.Interfaces;

namespace ServerApp.BusinessLogic.Services;

public class AuthService : IAuthService
{
    private readonly SignInManager<UserProfileEntity> _signInManager;

    public AuthService(SignInManager<UserProfileEntity> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task<Result> LoginUser(string username, string password)
    {
        var result = await _signInManager.PasswordSignInAsync(
            username,
            password,
            isPersistent: false,
            lockoutOnFailure: false
        );

        return result.Succeeded ? Result.Ok() 
            : Result.Fail("Invalid username or password");
    }

    public async Task<Result> RegisterUser(string username, string password, string? displayName, string email)
    {
        if (!UserProfileInputValidator.IsUsernameValid(username))
            return Result.Fail("Username is invalid");
        if (!UserProfileInputValidator.IsEmailValid(email))
            return Result.Fail("Email is invalid");
        if (await _signInManager.UserManager.FindByNameAsync(username) != null)
            return Result.Fail("Username is already taken");
        if(string.IsNullOrEmpty(displayName))
            displayName = username.ToUpper();

        var user = new UserProfileEntity
        {
            UserName = username,
            Email = email,
            DisplayName = displayName,
            CreationDate = DateTime.UtcNow
        };
        var result = await _signInManager.UserManager.CreateAsync(user, password);

        if (result.Succeeded)
            return Result.Ok();

        var errors = string.Join(". ", result.Errors.Select(e=>e.Description).ToList());
        return Result.Fail(errors);
    }

    public async Task LogoutUser()
    {
        await _signInManager.SignOutAsync();
    }
}