using ServerApp.DataAccess.Entities;

public class ProfileImageEntity
{
    public Guid Id { get; set; }
    public string ProfileId { get; set; } = string.Empty;
    public UserProfileEntity? Profile { get; set; }
    public byte[] BytesSmall { get; set; } = [];
    public byte[] BytesMedium { get; set; } = [];
    public byte[] BytesLarge { get; set; } = [];
}

public class ChatImageEntity
{
    public Guid Id { get; set; }
    public Guid ChatId { get; set; }
    public ChatEntity? Chat { get; set; }
    public byte[] BytesSmall { get; set; } = [];
    public byte[] BytesMedium { get; set; } = [];
    public byte[] BytesLarge { get; set; } = [];
}

public class ImageEntity
{
    public Guid Id { get; set; }
    public byte[] Bytes { get; set; } = [];
}

public enum ImageSize
{
    Small = 1, Medium = 2, Large = 3
}