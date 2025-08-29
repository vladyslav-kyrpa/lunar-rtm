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

    public async Task<Result> LoginUserAsync(string username, string password)
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

    public async Task<Result> RegisterUserAsync(RegisterProfileRequest request)
    {
        var validationResult = await ValidateRegisterRequest(request);
        if (validationResult.IsFailed)
            return validationResult;

        var result = await _signInManager.UserManager
            .CreateAsync(RequestToUser(request), request.Password);

        if (result.Succeeded)
            return Result.Ok();

        var errors = string.Join("; ", result.Errors.Select(e => e.Description).ToList());
        return Result.Fail(errors);
    }

    private static UserProfileEntity RequestToUser(RegisterProfileRequest request) => new()
    {
        UserName = request.UserName,
        Email = request.Email,
        DisplayName = request.DisplayName ?? request.UserName.ToUpper(),
        CreationDate = DateTime.UtcNow
    };

    private async Task<Result> ValidateRegisterRequest(RegisterProfileRequest request) {
        if (!UserProfileInputValidator.IsUsernameValid(request.UserName))
            return Result.Fail("Username is invalid");
        if (!UserProfileInputValidator.IsEmailValid(request.Email))
            return Result.Fail("Email is invalid");
        if (await IsUsernameTaken(request.UserName))
            return Result.Fail("Username is already taken");
        return Result.Ok();
    }
    

    private async Task<bool> IsUsernameTaken(string username)
    {
        return await _signInManager.UserManager.FindByNameAsync(username) != null;
    }

    public async Task LogoutUserAsync()
    {
        await _signInManager.SignOutAsync();
    }
}