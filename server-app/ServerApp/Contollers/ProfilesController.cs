using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ServerApp.Controllers;

[ApiController]
[Authorize]
[Route("api/profiles")]
public class ProfilesController : ControllerBase
{
    private const string imagesPath = "/home/admin-user/test-imgs-can-be-removed";
    private readonly UserManager<UserProfileEntity> _userManager;

    public ProfilesController(UserManager<UserProfileEntity> userManager)
    {
        _userManager = userManager;
    }

    public record UpdatedProfile (string Username, string DisplayName, string Bio);

    [HttpGet("{username}")]
    public async Task<IActionResult> GetProfile(string username)
    {
        Console.WriteLine($"Get profile: @{username}");
        
        if (username == string.Empty || username == "me")
        {
            // TODO: move logic to the ProfilesService
            // Get the user id from the claims
            var userId = User?.FindFirst("sub")?.Value ?? User?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            if (userId == null)
                return Unauthorized("User not found in claims.");

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound("User not found.");

            return Ok( new 
            {
                Username = user.UserName,
                DisplayName = user.DisplayName,
                Bio = user.Bio,
                ImageUrl = user.ProfileImageId.ToString() ?? "null"
            });
        }

        // TODO: get user from ProfileService
        return Ok(new
        {
            Username = username,
            DisplayName = username.ToUpper(),
            Bio = "Just user",
            ImageUrl = "http://localhost:5219/profiles/none/image"
        });
    }

    [HttpPost("{username}/update")]
    public IActionResult Update([FromBody] UpdatedProfile profile, string username)
    {
        Console.WriteLine($"Update {username} profile with username:{profile.Username}, bio:{profile.Bio}, displayname:{profile.DisplayName}");
        return Ok("Profile updated");
    }

    [HttpPost("{username}/update-image")]
    public async Task<IActionResult> UpdateImage(IFormFile image, string username)
    {
        Console.WriteLine($"Update {username} profile picture");

        if (image == null || image.Length == 0)
            return BadRequest("No file uploaded.");

        const long maxSizeInBytes = 2 * 1024 * 1024; // 2mb
        if (image.Length > maxSizeInBytes)
            return BadRequest("File size exceeds 2MB.");

        var allowedMimeTypes = new[] { "image/jpeg", "image/png" };
        if (!allowedMimeTypes.Contains(image.ContentType))
            return BadRequest("Invalid file type. Only JPEG, PNG are allowed.");

        try
        {
            // read file
            using var stream = new MemoryStream();
            await image.CopyToAsync(stream);
            var bytes = stream.ToArray();

            // save file
            System.IO.File.WriteAllBytes(Path.Combine(imagesPath, username + ".jpeg"), bytes);
            return Ok("");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{username}/image")]
    [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK, "image/jpeg")]
    public async Task<IActionResult> GetImage(string username)
    {
        Console.WriteLine($"Get profile picture {username}");

        try
        {
            var path = Path.Combine(imagesPath, username + ".jpeg");
            if (!System.IO.File.Exists(path))
                return NotFound();
            var bytes = await System.IO.File.ReadAllBytesAsync(path);
            return File(bytes, "image/jpeg");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}