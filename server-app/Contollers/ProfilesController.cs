using Microsoft.AspNetCore.Mvc;

namespace ServerApp.Controllers;

[ApiController]
[Route("profiles")]
public class ProfilesController : ControllerBase
{
    private const string imagesPath = "/home/admin-user/test-imgs-can-be-removed";

    [HttpGet("{username}")]
    public IActionResult GetProfile(string username)
    {
        Console.WriteLine($"Get profile: @{username}");

        if (username == string.Empty || username == "me")
        {
            return Ok(new UserProfile
            {
                Username = "current-user-profile",
                DisplayName = "Current user",
                Bio = "Just current user",
                ImageUrl = "http://localhost:5219/profiles/current-user/image"
            });
        }
        return Ok(new UserProfile
        {
            Username = username,
            DisplayName = username.ToUpper(),
            Bio = "Just user",
            ImageUrl = "http://localhost:5219/profiles/none/image"
        });
    }

    [HttpPost("create")]
    public IActionResult Create([FromBody] NewProfileInfo profile)
    {
        Console.WriteLine($"Create profile username:{profile.Username}, displayname:{profile.DisplayName}, password:{profile.Password}, email:{profile.Email}");
        return Ok("Profile Created");
    }

    [HttpPost("{username}/update")]
    public IActionResult Update([FromBody] ProfileInfo profile, string username)
    {
        Console.WriteLine($"Update {username} profile with username:{profile.Username}, bio:{profile.Bio}, displayname:{profile.DisplayName}");
        return Ok("Profile updated");
    }

    [HttpPost("{username}/update-image")]
    public async Task<IActionResult> UpdateImage([FromForm] IFormFile image, string username)
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

public class UserProfile
{
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;

}

public class NewProfileInfo
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class ProfileInfo
{
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
}