using ServerApp.BusinessLogic.Common;

namespace ServerApp.BusinessLogic.Services.Interfaces;

public interface IImagesService
{
    /// <summary>
    /// Get profile image of a given size.
    /// </summary>
    /// <param name="id">Image ID</param>
    /// <param name="size">Size</param>
    /// <returns>Image data</returns>
    Task<Result<ImageData>> GetProfileImageAsync(string id, AvatarSize size);

    /// <summary>
    /// Get chat image of a given size.
    /// </summary>
    /// <param name="id">Image ID</param>
    /// <param name="size">Size</param>
    /// <returns>Image data</returns>
    Task<Result<ImageData>> GetChatImageAsync(string id, AvatarSize size);
}

public class ImageData
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public byte[] Bytes { get; set; } = [];
}