using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ServerApp.BusinessLogic.Models;
using ServerApp.BusinessLogic.Services.Interfaces;

namespace ServerApp.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly ILogger<ChatHub> _logger;
    private readonly IChatsService _chats;
    private readonly IPresenceService _connections;

    public ChatHub(IChatsService chats, IPresenceService connections, ILogger<ChatHub> logger)
    {
        _logger = logger;
        _connections = connections;
        _chats = chats;
    }

    public record IncomingMessage(string Content, string ChatId);
    public record MessageToDelete(string Id, string ChatId);

    public override Task OnConnectedAsync()
    {
        var username = Context?.User?.Identity?.Name;
        var connectionId = Context?.ConnectionId;

        if (username != null && connectionId != null)
        {
            _logger.LogInformation("User @{username} connected", username);
            _connections.AddAsync(username, connectionId);
        }
        else _logger.LogError("Failed to connect a user. Username or connectionId is null");

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionId = Context.ConnectionId;
        if (exception != null)
        {
            _logger.LogError("User @{User} has disconnected with error: @{expection}",
                Context?.User?.Identity?.Name, exception.Message);
        }
        else if (connectionId != null)
        {
            _connections.RemoveAsync(Context.ConnectionId);
            _logger.LogInformation("User @{User} has disconnected",
                Context?.User?.Identity?.Name);
        }
        else
        {
            _logger.LogError("Failed to handle user disconnection. Connection ID is null");
        }
        return base.OnDisconnectedAsync(exception);
    }

    [HubMethodName("send-message")]
    public async Task SendMessage(IncomingMessage message)
    {
        if (string.IsNullOrEmpty(message.Content))
            return;

        var sender = Context.User?.Identity?.Name;
        if (sender == null)
        {
            _logger.LogError("Authentication error, cannot find current user name");
            return;
        }

        var result = await _chats.StoreMessageAsync(message.Content, sender, message.ChatId);

        if (result.IsFailed)
        {
            _logger.LogError("Failed to store message: @{Error}", result.Error);
            return;
        }

        var connections = await GetChatMemberConnnections(message.ChatId);

        await Clients.Clients(connections).SendAsync("receive-message", new Message
        {
            Id = result.Value,
            ChatId = message.ChatId,
            Content = message.Content,
            Timestamp = DateTime.UtcNow,
            Sender = sender
        });
    }

    private async Task<List<string>> GetChatMemberConnnections(string chatId)
    {
        var result = await _chats.GetAsync(chatId);
        if (result.IsFailed)
            return [];

        var members = result.Value.Members
            .Select(m => m.Username)
            .ToArray();

        return _connections.GetMultipleAsync(members);
    }
}