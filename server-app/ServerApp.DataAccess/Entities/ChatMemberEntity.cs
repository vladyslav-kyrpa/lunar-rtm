namespace ServerApp.DataAccess.Entities;

public class ChatMemberEntity
{
    public string UserId { get; set; } = string.Empty;
    public UserProfileEntity? User { get; set; }
    public Guid ChatId { get; set; }
    public ChatEntity? Chat{ get; set; }
    public ChatMemberRole Role { get; set; }
}

public enum ChatMemberRole
{
    Owner = 0, Moderator = 1, Regular = 2
}