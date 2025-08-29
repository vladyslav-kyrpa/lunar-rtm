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

    public async Task<Result> DeleteAsync(string username)
    {
        var user = await _users.FindByNameAsync(username);
        if (user == null)
            return Result.Fail("User was not found");

        await _users.DeleteAsync(user);

        return Result.Ok();
    }

    public async Task<Result<UserProfile>> GetAsync(string username)
    {
        var user = await _users.FindByNameAsync(username);
        if (user == null)
            return Result<UserProfile>.Fail("User was not found");

        UserProfile profile = EntityToModel(username, user);

        return Result<UserProfile>.Ok(profile);
    }

    private static UserProfile EntityToModel(string username, UserProfileEntity user) => new()
    {
        ImageId = user.ImageId.ToString() ?? "empty",
        Username = username,
        DisplayName = user.DisplayName,
        Bio = user.Bio,
        CreationTime = user.CreationDate
    };

    public async Task<Result> UpdateAsync(string username, UpdateProfileRequest request)
    {
        var user = await _users.FindByNameAsync(username);
        if (user == null)
            return Result.Fail("User was not found");

        var validationResult = await ValidateNewValues(username, request);
        if (validationResult.IsFailed)
            return validationResult;

        user.Bio = request.Bio ?? "";
        user.DisplayName = request.DisplayName ?? "";
        user.UserName = request.Username;

        var result = await _users.UpdateAsync(user);
        if (result.Succeeded) return Result.Ok();

        var errors = string.Join("; ", result.Errors.Select(e => e.Description));
        return Result.Fail(errors);
    }

    private async Task<Result> ValidateNewValues(string username, UpdateProfileRequest request)
    {
        if(username != request.Username && (await _users.FindByNameAsync(request.Username)) != null)
            return Result.Fail("Username is already taken");
        if (!UserProfileInputValidator.IsUsernameValid(request.Username))
            return Result.Fail("Username is not valid");
        if (!string.IsNullOrEmpty(request.DisplayName) && !UserProfileInputValidator.IsDisplayNameValid(request.DisplayName))
            return Result.Fail("DisplayName is not valid");
        if (!string.IsNullOrEmpty(request.Bio) && !UserProfileInputValidator.IsDisplayNameValid(request.Bio))
            return Result.Fail("Bio is not valid");
        return Result.Ok();
    } 

    public async Task<Result> UpdateImageAsync(string username, byte[] bytes)
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