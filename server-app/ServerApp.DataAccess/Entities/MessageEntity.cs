namespace ServerApp.DataAccess.Entities;

public class MessageEntity
{
    public Guid Id { get; set; } = Guid.Empty;
    // Points to user that sent a message. User may not exist anymore.
    public string? SenderId { get; set; }
    // Points to user that sent a message. User may not exist anymore.
    public UserProfileEntity? Sender { get; set; }
    public Guid ChatId { get; set; } = Guid.Empty;
    public ChatEntity? Chat { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreationDate { get; set; }
}