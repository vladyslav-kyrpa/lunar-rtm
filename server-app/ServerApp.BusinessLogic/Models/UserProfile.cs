namespace ServerApp.BusinessLogic.Models;

public class UserProfile
{
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string ImageId { get; set; } = string.Empty;
    public DateTime CreationTime { get; set; }
}

public class UserProfileHeader
{
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string ImageId { get; set; } = string.Empty;
}