using Microsoft.AspNetCore.Mvc;
using ServerApp.BusinessLogic.Services.Interfaces;

namespace ServerApp.Controllers;

[ApiController]
[Route("api/images")]
public class ImagesController : ControllerBase
{
    private readonly IImagesService _images;
    private readonly ILogger<ImagesController> _logger;

    public ImagesController(IImagesService images, ILogger<ImagesController> logger)
    {
        _images = images;
        _logger = logger;
    }

    [HttpGet("profile-avatar/{id}")]
    [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK, "image/webp")]
    public async Task<IActionResult> GetProfileImage(string id, int size)
    {
        if (string.IsNullOrEmpty(id))
            return BadRequest("Profile image ID was not provided");
        
        var result = await _images.GetProfileImage(id, (AvatarSize)size);
        if (result.IsFailed)
        {
            _logger.LogError("Failed to get user profile image: @{Error}", result.Error);
            return BadRequest(result.Error);
        }

        var image = result.Value; 
        var type = image.Type.ToString().ToLower();

        _logger.LogInformation("Get user profile image @{Id} in size @{Size}", id, size);
        return File(image.Bytes, $"image/{type}");
    }

    [HttpGet("chat-avatar/{id}")]
    [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK, "image/webp")]
    public async Task<IActionResult> GetChatImage(string id, int size)
    {
        if (string.IsNullOrEmpty(id))
            return BadRequest("Chat image ID was not provided");

        var result = await _images.GetChatImage(id, (AvatarSize)size);
        if (result.IsFailed)
        {
            _logger.LogError("Failed to get chat image: @{Error}", result.Error);
            return BadRequest(result.Error);
        }

        var image = result.Value; 
        var type = image.Type.ToString().ToLower();

        _logger.LogInformation("Get chat image @{Id} in size @{Size}", id, size);
        return File(image.Bytes, $"image/{type}");
    }
}