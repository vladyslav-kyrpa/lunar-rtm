using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerApp.BusinessLogic.Models;
using ServerApp.BusinessLogic.Services.Interfaces;

namespace ServerApp.Controllers;

[ApiController]
[Authorize]
[Route("api/chats")]
public class ChatsController : ControllerBase
{
    private readonly IChatsService _chats;
    private readonly ILogger<ChatsController> _logger;

    public ChatsController(IChatsService chats, ILogger<ChatsController> logger)
    {
        _chats = chats;
        _logger = logger;
    }

    public record NewChat(string Title, string Description, int Type);
    public record ChatMember(string Username);

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        if (string.IsNullOrEmpty(id))
            return NotFound();

        var result = await _chats.Get(id);

        return result.Success
            ? Ok(result.Value) 
            : BadRequest(result.Error);
    }

    [HttpGet("")]
    public async Task<IActionResult> GetList()
    {
        var user = User?.Identity?.Name;
        if (user == null)
            return BadRequest("No current username");

        var chats = await _chats.GetForUser(user);
        return Ok(chats);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] NewChat chat)
    {
        _logger.LogInformation("User creates new chat: @{Chat}", chat);
        var user = User?.Identity?.Name;
        if (user == null)
            return BadRequest("No current username");

        var result = await _chats.Create(chat.Title, chat.Description, user, (ChatType)chat.Type);

        if (result.Success)
        {
            _logger.LogInformation("Chat created: @{Id}", result.Value);
            return Ok($"New chat created (id:{result.Value})");
        }
        _logger.LogError("Failed to create new chat: @{Error}", result.Error);
        return BadRequest(result.Error);
    }

    [HttpPost("{id}/update")]
    public async Task<IActionResult> Update([FromBody] NewChat chat, string id)
    {
        if (string.IsNullOrEmpty(id))
            return BadRequest("No id provided");

        var user = User?.Identity?.Name;
        if (user == null)
            return BadRequest("No current username");

        if (!await _chats.CanEdit(user, id))
            return Unauthorized("You have no rights to update this conversation");

        var result = await _chats.Update(id, chat.Title, chat.Description);

        return result.Success
            ? Ok("Chat was updated") 
            : BadRequest(result.Error);
    }

    [HttpPost("{chatId}/add-member")]
    public async Task<IActionResult> AddMember([FromBody] ChatMember member, string chatId)
    {
        if (string.IsNullOrEmpty(chatId))
            return BadRequest("No id provided");
        if (string.IsNullOrEmpty(member.Username))
            return BadRequest("No member username provided");

        var user = User?.Identity?.Name;
        if (user == null)
            return BadRequest("No current username");

        if (!await _chats.CanEdit(user, chatId))
            return Unauthorized("You have no rights to add members");

        var result = await _chats.AddMember(member.Username, chatId);

        return result.Success
            ? Ok($"member {member.Username} was added") 
            : BadRequest(result.Error);
    }

    [HttpPost("{chatId}/remove-member")]
    public async Task<IActionResult> RemoveMember([FromBody] ChatMember member, string chatId)
    {
        if (string.IsNullOrEmpty(chatId))
            return BadRequest("No id provided");
        if (string.IsNullOrEmpty(member.Username))
            return BadRequest("No member username provided");

        var user = User?.Identity?.Name;
        if (user == null)
            return BadRequest("No current username");

        if (!await _chats.CanEdit(user, chatId))
            return Unauthorized("You have no rights to add members");

        var result = await _chats.RemoveMember(member.Username, chatId);

        return result.Success
            ? Ok($"member {member.Username} was removed") 
            : BadRequest(result.Error);
    }

    [HttpGet("{id}/messages")]
    public async Task<IActionResult> GetMessages(string id)
    {
        if (string.IsNullOrEmpty(id))
            return BadRequest("No id provided");

        var messages = await _chats.GetMessages(id);
        return Ok(messages);
    }

    [HttpPost("{id}/update-image")]
    public async Task<IActionResult> UpdateImage(IFormFile image, string id)
    {
        if (string.IsNullOrEmpty(id))
            return BadRequest("No chat ID provided");
        if (!IsImageValid(image))
            return BadRequest("Image is not valid (max size - 2MB, allowed types: jpeg, png)");

        using var stream = new MemoryStream();
        await image.CopyToAsync(stream);
        var bytes = stream.ToArray();

        var result = await _chats.UpdateImage(id, bytes);

        return result.Success
            ? Ok("Image updated") 
            : BadRequest(result.Error);
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