using Microsoft.AspNetCore.Identity;

public class UserProfileEntity : IdentityUser
{
    public Guid? ImageId { get; set; }
    public ProfileImageEntity? Image { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public DateTime CreationDate { get; set; } = DateTime.MinValue;
}