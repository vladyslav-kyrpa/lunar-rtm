using Microsoft.AspNetCore.Identity;

public class UserProfileEntity : IdentityUser
{
    public Guid? ProfileImageId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
}