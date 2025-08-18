using ServerApp.BusinessLogic.Common;

namespace ServerApp.BusinessLogic.Services.Interfaces;

public interface IAuthService
{
    /// <summary>
    /// Create new regular user profile.  
    /// </summary>
    Task<Result> RegisterUser(string username, string password, string? displayName, string email);

    /// <summary>
    /// Log-in user with given credentials.
    /// </summary>
    /// <param name="username">Username</param>
    /// <param name="password">Password</param>
    Task<Result> LoginUser(string username, string password);

    /// <summary>
    /// Log-out current user
    /// </summary>
    Task LogoutUser();
}