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
        if (username == string.Empty || username == "me")
            return Ok("Current profile");
        return Ok($"User {username} profile");
    }

    [HttpPost("create")]
    public IActionResult Create([FromBody] ProfileInfo profile)
    {
        return Ok("Profile Created");
    }

    [HttpPost("update")]
    public IActionResult Update([FromBody] ProfileInfo profile)
    {
        return Ok("Profile updated");
    }

    [HttpPost("{id}/update-image")]
    public async Task<IActionResult> UpdateImage([FromBody] IFormFile image, string id)
    {
        // read file
        try
        {
            using var stream = new MemoryStream();
            await image.CopyToAsync(stream);
            var bytes = stream.ToArray();

            // save file
            System.IO.File.WriteAllBytes(Path.Combine(imagesPath, id + ".jpeg"), bytes);
            return Ok("");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/image")]
    public async Task<IActionResult> GetImage(string id)
    {
        try
        {
            var path = Path.Combine(imagesPath, id + ".jpeg");
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

public class ProfileInfo {
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}