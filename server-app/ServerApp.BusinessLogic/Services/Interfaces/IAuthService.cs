using ServerApp.BusinessLogic.Common;

namespace ServerApp.BusinessLogic.Services.Interfaces;

public interface IAuthService
{
    /// <summary>
    /// Register new user
    /// </summary>
    /// <param name="request">new user data</param>
    /// <returns>Operation result</returns>
    Task<Result> RegisterUserAsync(RegisterProfileRequest request);

    /// <summary>
    /// Log-in user with given credentials
    /// </summary>
    /// <param name="username">Username</param>
    /// <param name="password">Password</param>
    Task<Result> LoginUserAsync(string username, string password);

    /// <summary>
    /// Log-out current user
    /// </summary>
    Task LogoutUserAsync();
}

public class RegisterProfileRequest
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set;} = string.Empty;
    public string? DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}