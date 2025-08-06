using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ServerApp.Controllers;

[ApiController]
[Route("chats")]
public class ChatsController : ControllerBase
{
    private const string imagesPath = "/home/admin-user/test-imgs-can-be-removed";

    [HttpGet("{id}")]
    public IActionResult Get(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }
        return Ok("Here is chat");
    }

    [HttpGet("")]
    public IActionResult GetList()
    {
        return Ok("current user chats");
    }

    [HttpPost("create")]
    public IActionResult Create([FromBody] NewChat chat)
    {
        return Ok("New chat created");
    }

    [HttpPost("{id}/update")]
    public IActionResult Update([FromBody] NewChat chat, string id)
    {
        if (string.IsNullOrEmpty(id))
            return BadRequest("No id provided");
        return Ok("Update chat");
    }

    [HttpPost("{id}/add-member")]
    public IActionResult AddMember([FromQuery] string member, string id)
    {
        if (string.IsNullOrEmpty(id))
            return BadRequest("No id provided");
        if (string.IsNullOrEmpty(member))
            return BadRequest("No member username provided");
        return Ok("");
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

public class NewChat {
    public string Title { get; set; } = string.Empty;
    public string Desctiption { get; set; } = string.Empty;
}