namespace ServerApp.DataAccess.Entities;

public class ChatEntity
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Type { get; set; }
    public Guid? ImageId { get; set; }
    public ChatImageEntity? Image { get; set; }
    public DateTime CreationDate { get; set; } = DateTime.MinValue;
    public string OwnerId { get; set; } = string.Empty;
    public UserProfileEntity Owner { get; set; }
    public List<string> MemberIDs { get; set; } = [];
}