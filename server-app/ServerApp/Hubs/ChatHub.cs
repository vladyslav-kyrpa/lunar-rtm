using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ServerApp.BusinessLogic.Models;

namespace ServerApp.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(ILogger<ChatHub> logger)
    {
        _logger = logger;
    }

    public record IncomingMessage(string Content, string ChatId);

    [HubMethodName("send-message")]
    public async Task SendMessage(IncomingMessage message)
    {
        _logger.LogInformation("Message received: @{Message}", message);
        // TODO: add messages processing logic
        await Clients.All.SendAsync("receive-message", new Message
        {
            Id = "1234",
            Content = message.Content,
            CreationTime = DateTime.UtcNow,
            Sender = Context.User.Identity.Name ?? "unknown",
        } );
    }
}