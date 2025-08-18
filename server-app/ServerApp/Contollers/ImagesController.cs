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
        _logger.LogInformation("Get user profile image @{Id} in size @{Size}", id, size);
        try
        {
            var image = await _images.GetProfileImage(id, (AvatarSize)size);
            var type = image.Type.ToString().ToLower();
            return File(image.Bytes, $"image/{type}");
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get user profile image: @{Ex}", ex);
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("chat-avatar/{id}")]
    [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK, "image/webp")]
    public async Task<IActionResult> GetChatImage(string id, int size)
    {
        try
        {
            var image = await _images.GetChatImage(id, (AvatarSize)size);
            var type = image.Type.ToString().ToLower();
            return File(image.Bytes, $"image/{type}");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}