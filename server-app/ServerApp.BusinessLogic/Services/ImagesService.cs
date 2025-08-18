using ServerApp.BusinessLogic.Common;
using ServerApp.BusinessLogic.Services.Interfaces;
using ServerApp.DataAccess.Repositories.Interfaces;

namespace ServerApp.BusinessLogic.Services;

public class ImagesService : IImagesService
{
    private readonly IAvatarRepository<ProfileImageEntity> _profileImages;
    private readonly IAvatarRepository<ChatImageEntity> _chatImages;

    public ImagesService(IAvatarRepository<ProfileImageEntity> profileImages,
        IAvatarRepository<ChatImageEntity> chatImages)
    {
        _profileImages = profileImages;
        _chatImages = chatImages;
    }

    public async Task<Result<ImageData>> GetChatImage(string id, AvatarSize size)
    {
        var image = await _chatImages.Get(id, (ImageSize)size);
        if (image == null)
            return Result<ImageData>.Fail("Chat image was not found");

        var img = new ImageData
        {
            Id = image.Id.ToString(),
            Type = "webp",
            Bytes = image.Bytes
        };

        return Result<ImageData>.Ok(img);
    }

    public async Task<Result<ImageData>> GetProfileImage(string id, AvatarSize size)
    {
        var image = await _profileImages.Get(id, (ImageSize)size);
        if (image == null)
            return Result<ImageData>.Fail("Profile image was not found");

        var img = new ImageData
        {
            Id = image.Id.ToString(),
            Type = "webp",
            Bytes = image.Bytes
        };

        return Result<ImageData>.Ok(img);
    }
}