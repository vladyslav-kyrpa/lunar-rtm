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

    public record UpdatedProfile(string Username, string DisplayName, string Bio);

    [HttpGet("{username}")]
    public async Task<IActionResult> GetProfile(string username)
    {
        Result<UserProfile> result;

        if (username == string.Empty || username == "me")
        {
            var currentUser = User?.Identity?.Name;
            if (currentUser == null)
                return Unauthorized("User is not authenticated");
            result = await _profiles.GetByUsername(currentUser);
        }
        else
        {
            result = await _profiles.GetByUsername(username);
        }

        return result.Success ? Ok(result.Value)
            : BadRequest(result.Error);
    }

    [HttpPost("{username}/update")]
    public async Task<IActionResult> Update([FromBody] UpdatedProfile profile, string username)
    {
        var currentUser = User?.Identity?.Name;
        if (currentUser == null)
            return Unauthorized("Cannot get current user");
        if (currentUser != username)
            return BadRequest("You have no rights to delete this profile");

        var result = await _profiles.Update(currentUser, profile.Username, profile.DisplayName, profile.Bio);

        return result.Success ? Ok("Profile updated")
            : BadRequest(result.Error);
    }

    [HttpGet("{username}/delete")]
    public async Task<IActionResult> DeleteProfile(string username)
    {
        var currentUser = User?.Identity?.Name; 
        if (currentUser == null)
            return Unauthorized("Cannot get current user");
        if (currentUser != username)
            return BadRequest("You have no rights to delete this profile");

        var result = await _profiles.Delete(username);
        await _auth.LogoutUser();

        return result.Success
            ? Ok("User profile was deleted permanently")
            : BadRequest(result.Error);
    }

    [HttpPost("{username}/update-image")]
    public async Task<IActionResult> UpdateImage(IFormFile image, string username)
    {
        _logger.LogInformation("User @{Username} updates profile image", username);
        if (username != User?.Identity?.Name)
            return BadRequest("You have no rights to change this image");

        if (image == null || image.Length == 0)
            return BadRequest("No file uploaded.");

        const long maxSizeInBytes = 2 * 1024 * 1024; // 2mb
        if (image.Length > maxSizeInBytes)
            return BadRequest("File size exceeds 2MB.");

        var allowedMimeTypes = new[] { "image/jpeg", "image/png" };

        if (!allowedMimeTypes.Contains(image.ContentType))
            return BadRequest("Invalid file type. Only JPEG, PNG are allowed.");

        using var stream = new MemoryStream();
        await image.CopyToAsync(stream);
        var bytes = stream.ToArray();

        var result = await _profiles.UpdateImage(username, bytes);

        return result.Success ? Ok("Profile image updated") 
            : BadRequest(result.Error);
    }
}