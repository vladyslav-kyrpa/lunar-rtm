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

    public async Task<Result<ImageData>> GetChatImageAsync(string id, AvatarSize size)
    {
        if (!IsValidSize((int)size))
            return Result<ImageData>.Fail("Image size is not supported");

        var image = await _chatImages.Get(id, (ImageSize)size);
        if (image == null)
            return Result<ImageData>.Fail("Chat image was not found");

        return Result<ImageData>
            .Ok(ConvertToImageData(image));
    }

    public async Task<Result<ImageData>> GetProfileImageAsync(string id, AvatarSize size)
    {
        if (!IsValidSize((int)size))
            return Result<ImageData>.Fail("Image size is not supported");

        var image = await _profileImages.Get(id, (ImageSize)size);
        if (image == null)
            return Result<ImageData>.Fail("Profile image was not found");

        return Result<ImageData>
            .Ok(ConvertToImageData(image));
    }

    private static bool IsValidSize(int size)
        => Enum.IsDefined(typeof(ImageSize), size);

    private static ImageData ConvertToImageData(ImageEntity image) => new()
    {
        Id = image.Id.ToString(),
        Type = "webp",
        Bytes = image.Bytes
    };
}