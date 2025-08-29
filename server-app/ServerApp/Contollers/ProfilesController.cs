using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerApp.BusinessLogic.Common;
using ServerApp.BusinessLogic.Models;
using ServerApp.BusinessLogic.Services.Interfaces;

namespace ServerApp.Controllers;

[ApiController]
[Authorize]
[Route("api/profiles")]
public class ProfilesController : ControllerBase
{
    private readonly IAuthService _auth;
    private readonly IProfilesService _profiles;
    private readonly ILogger<ProfilesController> _logger;

    public ProfilesController(IProfilesService profiles,
        IAuthService auth,
        ILogger<ProfilesController> logger)
    {
        _profiles = profiles;
        _auth = auth;
        _logger = logger;
    }

    [HttpGet("{username}")]
    public async Task<IActionResult> GetProfile(string username)
    {
        var currentUser = User?.Identity?.Name
            ?? throw new Exception("No username for an authorized user"); 

        Result<UserProfile> result;
        if (username == string.Empty || username == "me")
            result = await _profiles.GetAsync(currentUser);
        else
            result = await _profiles.GetAsync(username);

        if (result.IsFailed)
        {
            _logger.LogError("Failed to get user @{username} profile: @{error}",
                username, result.Error);
            return BadRequest(result.Error);
        }

        _logger.LogInformation("Get user @{user} profle", result.Value.Username);
        return Ok(result.Value);
    }

    [HttpPost("{username}/update")]
    public async Task<IActionResult> Update([FromBody] UpdateProfileRequest updatedProfile, string username)
    {
        var currentUser = User?.Identity?.Name
            ?? throw new Exception("No username for an authorized user"); 

        if (username != "me")
        {
            if (currentUser != username)
            {
                _logger.LogWarning("User @{user1} tries to uplade user's @{user2} profile",
                    currentUser, username);
                return Unauthorized("You have no rights to update this profile");
            }
        }
        else username = currentUser;

        var result = await _profiles.UpdateAsync(username, updatedProfile);
        if (result.IsFailed)
        {
            _logger.LogError("Failed to update user @{user} profile: @{error}",
                username, result.Error);
            return BadRequest(result.Error);
        }

        _logger.LogInformation("User @{user} updates user's @{user2} profile",
            currentUser, username);
        return Ok("Profile updated");
    }

    [HttpPost("{username}/delete")]
    public async Task<IActionResult> DeleteProfile(string username)
    {
        var currentUser = User?.Identity?.Name
            ?? throw new Exception("No username for an authorized user"); 
        
        if (username != "me")
        {
            if (currentUser != username)
            {
                _logger.LogWarning("User @{user1} tries to uplade user's @{user2} profile",
                    currentUser, username);
                return Unauthorized("You have no rights to update this profile");
            }
        }
        else username = currentUser;

        var result = await _profiles.DeleteAsync(username);
        if (result.IsFailed)
        {
            _logger.LogError("Failed to delte user's @{user} profile: @{error}",
                username, result.Error);
            return BadRequest(result.Error);
        }

        _logger.LogInformation("User @{user} deletes user's @{user2} profile",
            currentUser, username);
        await _auth.LogoutUserAsync();
        return Ok("User profile was deleted permanently");
    }

    [HttpPost("{username}/update-image")]
    public async Task<IActionResult> UpdateImage(IFormFile image, string username)
    {
        var user = User?.Identity?.Name ??
            throw new Exception("No username for an authorized user");

        if (user != username)
        {
            _logger.LogWarning("User @{user} tries to update user's @{} image without permissions",
                user, username);
            return BadRequest("Have no rights to update this image");
        }
        if (!IsImageValid(image))
        {
            _logger.LogWarning("User @{user} tries to upload an invalid image: @{size}, @{type}",
                user, image.Length, image.ContentType);
            return BadRequest("Image is not valid (max size - 2MB, allowed types: jpeg, png)");
        }

        using var stream = new MemoryStream();
        await image.CopyToAsync(stream);
        var bytes = stream.ToArray();

        var result = await _profiles.UpdateImageAsync(username, bytes);
        if (result.IsFailed)
        {
            _logger.LogError("Failed to update user's @{user} profile image: @{error}",
                user, result.Error);
            return BadRequest(result.Error);
        }

        _logger.LogInformation("User @{user} updates user's @{user2} profile image",
            user, username);
        return Ok("Profile image updated"); 
    }

    private bool IsImageValid(IFormFile image)
    {
        if (image == null || image.Length == 0)
            return false;

        const long maxSizeInBytes = 2 * 1024 * 1024; // 2mb
        if (image.Length > maxSizeInBytes)
            return false;

        var allowedMimeTypes = new[] { "image/jpeg", "image/png" };
        if (!allowedMimeTypes.Contains(image.ContentType))
            return false;

        return true;
    }
}