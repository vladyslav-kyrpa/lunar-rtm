using Microsoft.AspNetCore.Identity;
using ServerApp.BusinessLogic.Common;
using ServerApp.BusinessLogic.Models;
using ServerApp.BusinessLogic.Services.Interfaces;
using ServerApp.DataAccess.Repositories.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace ServerApp.BusinessLogic.Services;

public class ProfilesService : IProfilesService
{
    private readonly IAvatarRepository<ProfileImageEntity> _images;
    private readonly UserManager<UserProfileEntity> _users;

    public ProfilesService(IAvatarRepository<ProfileImageEntity> images,
        UserManager<UserProfileEntity> users)
    {
        _images = images;
        _users = users;
    }

    public async Task<Result> Delete(string username)
    {
        var user = await _users.FindByNameAsync(username);
        if (user == null)
            return Result.Fail("User was not found");

        await _users.DeleteAsync(user);

        return Result.Ok();
    }

    public async Task<Result<UserProfile>> GetByUsername(string username)
    {
        var user = await _users.FindByNameAsync(username);
        if (user == null)
            return Result<UserProfile>.Fail("User was not found");

        var profile = new UserProfile
        {
            ImageId = user.ImageId.ToString() ?? "empty",
            Username = username,
            DisplayName = user.DisplayName,
            Bio = user.Bio,
            CreationTime = user.CreationDate
        };

        return Result<UserProfile>.Ok(profile);
    }

    public async Task<Result> Update(string username, string newUsername, string? newDisplayName, string? newBio)
    {
        var user = await _users.FindByNameAsync(username);
        if (user == null)
            return Result.Fail("User was not found");
        if(username != newUsername && (await _users.FindByNameAsync(newUsername)) != null)
            return Result.Fail("Username is already taken");
        if (!UserProfileInputValidator.IsUsernameValid(newUsername))
            return Result.Fail("Username is not valid");

        user.Bio = newBio ?? "";
        user.DisplayName = newDisplayName ?? "";
        user.UserName = newUsername;

        var result = await _users.UpdateAsync(user);

        if (result.Succeeded)
            return Result.Ok();

        var errors = string.Join("; ", result.Errors.Select(e => e.Description));
        return Result.Fail(errors);
    }

    public async Task<Result> UpdateImage(string username, byte[] bytes)
    {
        var user = await _users.FindByNameAsync(username);
        if (user == null)
            return Result.Fail("User was not found");

        const int smSize = 64; //px
        const int mdSize = 128; //px
        const int lgSize = 256; //px

        var imgSmall = ConvertImage(bytes, smSize);
        var imgMedium = ConvertImage(bytes, mdSize);
        var imgLarge = ConvertImage(bytes, lgSize);

        // Store
        var imageId = user.ImageId ?? Guid.NewGuid();

        await _images.UpdateOrAddAvatar(new ProfileImageEntity
        {
            Id = imageId,
            ProfileId = user.Id,
            BytesSmall = imgSmall,
            BytesMedium = imgMedium,
            BytesLarge = imgLarge,
        });

        user.ImageId = imageId;
        await _users.UpdateAsync(user);

        return Result.Ok();
    }

    private byte[] ConvertImage(byte[] bytes, int size)
    {
        using var img = Image.Load(bytes);
        img.Mutate(e => e.Resize(size, size));
        return ToWebpByteArray(img);
    }

    private byte[] ToWebpByteArray(Image img)
    {
        using var stream = new MemoryStream();
        img.Save(stream, new WebpEncoder { Quality = 90 });
        return stream.ToArray();
    }
}