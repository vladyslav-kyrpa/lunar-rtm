using ServerApp.BusinessLogic.Common;
using ServerApp.BusinessLogic.Models;

namespace ServerApp.BusinessLogic.Services.Interfaces;

public interface IProfilesService
{
    /// <summary>
    /// Get user profile by its username. 
    /// </summary>
    /// <param name="username">Username</param>
    /// <returns>User profile</returns>
    Task<Result<UserProfile>> GetAsync(string username);

    /// <summary>
    /// Update user's public information.
    /// </summary>
    /// <param name="username">Old username</param>
    /// <param name="request">Updated profile values</param>
    /// <returns>Operation result</returns>
    Task<Result> UpdateAsync(string username, UpdateProfileRequest request);

    /// <summary>
    /// Delete user profile
    /// </summary>
    /// <param name="username">Username</param>
    /// <returns>Operation result</returns>
    Task<Result> DeleteAsync(string username);

    /// <summary>
    /// Update user profile image.
    /// </summary>
    /// <param name="username">Username</param>
    /// <param name="bytes">Image bytes</param>
    /// <returns>Operation result</returns>
    Task<Result> UpdateImageAsync(string username, byte[] bytes);
}

public class UpdateProfileRequest
{
    public string Username { get; set;} = string.Empty;
    public string? DisplayName { get; set;}
    public string? Bio { get; set;}
}