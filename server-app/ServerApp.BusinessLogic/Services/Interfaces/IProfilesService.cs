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
    Task<Result<UserProfile>> GetByUsername(string username);

    /// <summary>
    /// Update user's public information.
    /// </summary>
    /// <param name="username">Old username</param>
    /// <param name="newUsername">Updated username</param>
    /// <param name="newDisplayName">Updated display name</param>
    /// <param name="newBio">Updated bio</param>
    /// <returns>Operation result</returns>
    Task<Result> Update(string username, string newUsername, string? newDisplayName, string? newBio);

    /// <summary>
    /// Delete user profile
    /// </summary>
    /// <param name="username">Username</param>
    /// <returns>Operation result</returns>
    Task<Result> Delete(string username);

    /// <summary>
    /// Update user profile image.
    /// </summary>
    /// <param name="username">Username</param>
    /// <param name="bytes">Image bytes</param>
    /// <returns>Operation result</returns>
    Task<Result> UpdateImage(string username, byte[] bytes);
}
