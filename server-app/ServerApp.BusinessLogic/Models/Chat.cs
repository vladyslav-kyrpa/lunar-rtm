using ServerApp.DataAccess.Entities;

namespace ServerApp.BusinessLogic.Models;

public class Chat
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public ChatType Type { get; set; } = 0;
    public string Description { get; set; } = string.Empty;
    public string ImageId { get; set; } = string.Empty;
    public DateTime CreationTime { get; set; }
    public List<UserProfileHeader> Members { get; set; } = [];
    public ChatMemberPermissions CurrentPermissions { get; set; }
        = new ChatMemberPermissions(ChatMemberRole.Regular);
}

public enum ChatType
{
    Monologue = 1,
    Dialogue = 2,
    Polylogue = 3
}

public class ChatHeader
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string ImageId { get; set; } = string.Empty;
}