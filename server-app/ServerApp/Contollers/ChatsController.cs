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
    public record MemberPromotion(string Username, string Role);

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        if (string.IsNullOrEmpty(id))
            return NotFound();

        var user = User?.Identity?.Name ??
            throw new Exception("No username for an authorized user"); 

        var chat = await _chats.Get(id);
        if (chat.IsFailed)
        {
            _logger.LogError("Error during getting chat by ID @{id}: @{error}",
                id, chat.Error);
            return BadRequest(chat.Error);
        }

        var permissions = await _chats.GetMemberPermissions(id, user);
        if (permissions.IsFailed)
        {
            _logger.LogError("Error during getting chat @{id} member's @{user} permissions: @{error}",
                id, user, permissions.Error);
            return BadRequest(permissions.Error);
        } 

        if (chat.Value.Type == ChatType.Monologue && !permissions.Value.CanEdit)
            permissions.Value.CanSendMessages = false;

        chat.Value.CurrentPermissions = permissions.Value;

        _logger.LogInformation("Get chat @{id}", id);
        return Ok(chat.Value);
    }

    [HttpGet("")]
    public async Task<IActionResult> GetList()
    {
        var user = User?.Identity?.Name ??
            throw new Exception("No username for an authorized user"); 

        var chats = await _chats.GetForUser(user);

        _logger.LogInformation("Get chats for user @{user}", user);
        return Ok(chats);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] NewChat chat)
    {
        var user = User?.Identity?.Name ??
            throw new Exception("No username for an authorized user");

        var result = await _chats.Create(chat.Title, chat.Description, user, (ChatType)chat.Type);
        if (result.IsFailed)
        {
            _logger.LogError("Failed to create new chat: @{Error}", result.Error);
            return BadRequest(result.Error);
        }

        _logger.LogInformation("User @{user} creates chat @{Id}", user, result.Value);
        return Ok($"New chat created (id:{result.Value})");
    }

    [HttpPost("{id}/update")]
    public async Task<IActionResult> Update([FromBody] NewChat chat, string id)
    {
        if (string.IsNullOrEmpty(id))
            return BadRequest("No id provided");

        var user = User?.Identity?.Name ??
            throw new Exception("No username for an authorized user"); 

        var permissions = await _chats.GetMemberPermissions(id, user);
        if (permissions.IsFailed)
        {
            _logger.LogError("Error during getting chat @{id} member's @{user} permissions: @{error}",
                id, user, permissions.Error);
            return BadRequest(permissions.Error);
        }
        if (!permissions.Value.CanEdit)
        {
            _logger.LogWarning("User @{user} tries to update chat @{id} without permission",
                id, user);
            return Unauthorized("You have no rights to update this conversation");
        }

        var result = await _chats.Update(id, chat.Title, chat.Description);
        if(result.IsFailed)
        {
            _logger.LogError("Failed to update chat @{id}: @{error}",
                id, result.Error);
            return BadRequest(result.Error);
        }

        _logger.LogInformation("User @{user} updates chat @{chat}", user, id);
        return Ok("Chat was updated");
    }

    [HttpPost("{id}/delete")]
    public async Task<IActionResult> Delete(string id)
    {
        var user = User?.Identity?.Name ??
            throw new Exception("No username for an authorized user"); 

        var permissions = await _chats.GetMemberPermissions(id, user ?? "");
        if(permissions.IsFailed)
        {
            _logger.LogError("Failed to get chat member's permissions: @{error}",
                permissions.Error);
            return BadRequest(permissions.Error);
        }
        if (!permissions.Value.CanDelete)
        {
            _logger.LogWarning("User @{user} tries to delete chat @{id} without permission",
                user, id);
            return BadRequest("You have no permissions");
        }

        var result = await _chats.Detele(id);
        if (result.IsFailed)
        {
            _logger.LogError("Failed to remove chat: @{error}", result.Error);
            return BadRequest($"Failed to remove: {result.Error}");
        }

        _logger.LogInformation("User @{user} deletes chat @{id}", user, id);
        return Ok("Chat removed");
    }

    [HttpPost("{chatId}/add-member")]
    public async Task<IActionResult> AddMember([FromBody] ChatMember member, string chatId)
    {
        if (string.IsNullOrEmpty(chatId))
            return BadRequest("No id provided");
        if (string.IsNullOrEmpty(member.Username))
            return BadRequest("No member username provided");

        var user = User?.Identity?.Name ??
            throw new Exception("No username for an authorized user"); 

        var permissions = await _chats.GetMemberPermissions(chatId, user);
        if(permissions.IsFailed)
        {
            _logger.LogError("Failed to get chat member's permissions: @{error}",
                permissions.Error);
            return BadRequest(permissions.Error);
        }
        if (!permissions.Value.CanAddMember)
        {
            _logger.LogWarning("User @{user} tries to add member @{member} without permissions",
                user, member.Username);
            return Unauthorized("You have no rights to add members");
        }

        var result = await _chats.AddMember(member.Username, chatId);
        if (result.IsFailed)
        {
            _logger.LogError("Failed to add chat member: @{error}", result.Error);
            return BadRequest(result.Error);
        }

        _logger.LogInformation("User @{user} adds member @{member} to chat @{chat}",
            user, member.Username, chatId);
        return Ok($"Member {member.Username} was added");
    }

    [HttpPost("{chatId}/promote-member")]
    public async Task<IActionResult> PromoteMember(string chatId, MemberPromotion promotion)
    {
        if (string.IsNullOrEmpty(chatId))
            return BadRequest("No id provided");

        var user = User?.Identity?.Name ??
            throw new Exception("No username for an authorized user"); 

        var permissions = await _chats.GetMemberPermissions(chatId, user);
        if(permissions.IsFailed)
        {
            _logger.LogError("Failed to get chat member's permissions: @{error}",
                permissions.Error);
            return BadRequest(permissions.Error);
        }
        if (!permissions.Value.CanPromote)
        {
            _logger.LogWarning("User @{user} tries to promote member @{member} without permissions",
                user, promotion.Username);
            return Unauthorized("You have no rights to change member's role");
        }

        var result = await _chats.ChangeMemberRole(promotion.Username, chatId, promotion.Role);
        if (result.IsFailed)
        {
            _logger.LogError("Failed to change member role: @{error}", result.Error);
            return BadRequest(result.Error);
        }

        _logger.LogInformation("User @{user} promotes chat @{chat} member @{member} to @{role}",
            user, chatId, promotion.Username, promotion.Role);
        return Ok($"member {promotion.Username} was promoted to {promotion.Role}"); 
    }

    [HttpPost("{chatId}/remove-member")]
    public async Task<IActionResult> RemoveMember([FromBody] ChatMember member, string chatId)
    {
        if (string.IsNullOrEmpty(chatId))
            return BadRequest("No id provided");
        if (string.IsNullOrEmpty(member.Username))
            return BadRequest("No member username provided");

        var user = User?.Identity?.Name ??
            throw new Exception("No username for an authorized user"); 

        var permissions = await _chats.GetMemberPermissions(chatId, user);
        if(permissions.IsFailed)
        {
            _logger.LogError("Failed to get chat member's permissions: @{error}",
                permissions.Error);
            return BadRequest(permissions.Error);
        }
        if (!permissions.Value.CanRemoveMember)
        {
            _logger.LogWarning("User @{user} tries to remove member @{member} from chat @{chat}",
                user, member.Username, chatId);
            return Unauthorized("You have no rights to remove members");
        }

        var result = await _chats.RemoveMember(member.Username, chatId);
        if (result.IsFailed)
        {
            _logger.LogError("Failed to remove user from a chat: @{error}", result.Error);
            return BadRequest(result.Error);
        }

        _logger.LogInformation("User @{user} removes member @{member} from chat @{chat}",
            user, member.Username, chatId);
        return Ok($"member {member.Username} was removed");
    }

    [HttpPost("{chatId}/leave")]
    public async Task<IActionResult> LeaveChat(string chatId)
    {
        if (string.IsNullOrEmpty(chatId))
            return BadRequest("No id provided");

        var user = User?.Identity?.Name ??
            throw new Exception("No username for an authorized user"); 

        var result = await _chats.RemoveMember(user, chatId);
        if(result.IsFailed)
        {
            _logger.LogError("Failed to remove member {user} from chat @{chat}: @{error}",
                user, chatId, result.Error);
            return BadRequest(result.Error);
        }

        _logger.LogInformation("User @{user} leaves chat @{chat}", user, chatId);
        return Ok("Removed from the chat");
    }

    [HttpGet("{id}/messages")]
    public async Task<IActionResult> GetMessages(string id)
    {
        if (string.IsNullOrEmpty(id))
            return BadRequest("No id provided");

        var messages = await _chats.GetMessages(id);

        _logger.LogInformation("Get chat @{chat} messages", id);
        return Ok(messages);
    }

    [HttpPost("{chatId}/update-image")]
    public async Task<IActionResult> UpdateImage(IFormFile image, string chatId)
    {
        if (string.IsNullOrEmpty(chatId))
            return BadRequest("No chat ID provided");

        var user = User?.Identity?.Name ??
            throw new Exception("No username for an authorized user");

        if (!IsImageValid(image))
        {
            _logger.LogWarning("User @{user} tries to upload an invalid image: @{size}, @{type}",
                user, image.Length, image.ContentType);
            return BadRequest("Image is not valid (max size - 2MB, allowed types: jpeg, png)");
        }

        var permissions = await _chats.GetMemberPermissions(chatId, user);
        if(permissions.IsFailed)
        {
            _logger.LogError("Failed to get chat member's permissions: @{error}",
                permissions.Error);
            return BadRequest(permissions.Error);
        }
        if(!permissions.Value.CanEdit)
        {
            _logger.LogWarning("User @{user} tries to update chat @{chat} image without permissions",
                user, chatId);
            return Unauthorized("You have no rights to change this chat image");
        }

        using var stream = new MemoryStream();
        await image.CopyToAsync(stream);
        var bytes = stream.ToArray();

        var result = await _chats.UpdateImage(chatId, bytes);
        if(result.IsFailed)
        {
            _logger.LogError("Failed to update chat @{chat} image: @{error}",
                chatId, result.Error);
            return BadRequest(result.Error);
        }

        _logger.LogInformation("User @{user} updated chat @{chat} image", user, chatId);
        return Ok("Chat image updated"); 
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