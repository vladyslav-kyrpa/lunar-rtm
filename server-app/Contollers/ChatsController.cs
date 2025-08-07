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
            return NotFound();

        var chat = new Chat
        {
            Id = id,
            Title = "random chat",
            Description = "Just random chat",
            ImageUrl = "http://localhost:5219/profiles/none/image",
            Members = new List<UserHeader>
            {
                new () { Username = "1234", DisplayName = "User 1234", ImageUrl="http://localhost:5219/profiles/none/image"},
                new () { Username = "12345", DisplayName = "User 123456", ImageUrl="http://localhost:5219/profiles/none/image"},
                new () { Username = "123456", DisplayName = "User 123456", ImageUrl="http://localhost:5219/profiles/none/image"},
            }
        };
        return Ok(chat);
    }

    [HttpGet("")]
    public IActionResult GetList()
    {
        var chats = new List<ChatHeader>() {
            new () { Id = "1", Title = "Conv1", ImageUrl="http://localhost:5219/profiles/none/image", NewMessagesCount=123},
            new () { Id = "2", Title = "Conv2", ImageUrl="http://localhost:5219/profiles/none/image", NewMessagesCount=13},
            new () { Id = "3", Title = "Conv3", ImageUrl="http://localhost:5219/profiles/none/image", NewMessagesCount=12},
            new () { Id = "4", Title = "Conv4444", ImageUrl="http://localhost:5219/profiles/none/image", NewMessagesCount=1 },
        };
        return Ok(chats);
    }

    [HttpPost("create")]
    public IActionResult Create([FromBody] NewChat chat)
    {
        Console.WriteLine($"Create chat with title:{chat.Title}, desc:{chat.Description}");
        return Ok("New chat created");
    }

    [HttpPost("{id}/update")]
    public IActionResult Update([FromBody] NewChat chat, string id)
    {
        Console.WriteLine($"Update chat:{id} with title:{chat.Title}, desc:{chat.Description}");
        if (string.IsNullOrEmpty(id))
            return BadRequest("No id provided");
        return Ok("Update chat");
    }

    [HttpPost("{id}/add-member")]
    public IActionResult AddMember([FromBody] ChatMember member, string id)
    {
        Console.WriteLine($"Add member @{member.Username} to chat:{id}");
        if (string.IsNullOrEmpty(id))
            return BadRequest("No id provided");
        if (string.IsNullOrEmpty(member.Username))
            return BadRequest("No member username provided");
        return Ok($"member {member.Username} added");
    }

    [HttpGet("{id}/messages")]
    public IActionResult GetMessages(string id)
    {
        Console.WriteLine($"Get messages {id}");
        return Ok(new Message[] {
            new () {Id = "1", ChatId = id, Content = "message text", Sender = "1", Timestamp = "01-01-2025 01:01"},
            new () {Id = "2", ChatId = id, Content = "message text1", Sender = "2", Timestamp = "01-01-2025 01:01"},
            new () {Id = "3", ChatId = id, Content = "message text2", Sender = "1", Timestamp = "01-01-2025 01:01"},
            new () {Id = "4", ChatId = id, Content = "message text3", Sender = "2", Timestamp = "01-01-2025 01:01"},
            new () {Id = "5", ChatId = id, Content = "message text4", Sender = "1", Timestamp = "01-01-2025 01:01"},
        });
    }

    [HttpPost("{id}/update-image")]
    public async Task<IActionResult> UpdateImage([FromForm] IFormFile image, string id)
    {
        Console.WriteLine($"Update {id} chat picture");

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
        Console.WriteLine($"Get chat picture {id}");

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

public class Message {
    public string Id { get; set; } = string.Empty;
    public string Sender { get; set; } = string.Empty;
    public string ChatId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Timestamp { get; set; } = string.Empty;
}

public class ChatMember
{
    public string Username { get; set; } = string.Empty;
}

public class NewChat
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class ChatHeader
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int NewMessagesCount { get; set; } = 0;
}

public class Chat
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public List<UserHeader> Members { get; set; } = [];
}

public class UserHeader
{
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}