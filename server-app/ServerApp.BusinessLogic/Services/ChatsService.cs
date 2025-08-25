using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServerApp.BusinessLogic.Common;
using ServerApp.BusinessLogic.Models;
using ServerApp.BusinessLogic.Services.Interfaces;
using ServerApp.DataAccess.Repositories.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
namespace ServerApp.BusinessLogic.Services;

public class ChatsServices : IChatsService
{
    private readonly IChatRepository _chats;
    private readonly IMessageRepository _messages;
    private readonly UserManager<UserProfileEntity> _users;
    private readonly IAvatarRepository<ChatImageEntity> _images;

    public ChatsServices(IAvatarRepository<ChatImageEntity> images,
        IMessageRepository messages,
        IChatRepository chats,
        UserManager<UserProfileEntity> users)
    {
        _chats = chats;
        _users = users;
        _messages = messages;
        _images = images;
    }

    public async Task<Result> AddMember(string username, string chatId)
    {
        var user = await _users.FindByNameAsync(username);
        if (user == null)
            return Result.Fail("User was not found");
        if (!await _chats.IsExists(chatId))
            return Result.Fail("Chat was not found");

        await _chats.AddMember(chatId, user.Id);
        return Result.Ok();
    }

    public async Task<bool> CanEdit(string username, string chatId)
    {
        var user = await _users.FindByNameAsync(username);
        if (user == null)
            return false;

        var chat = await _chats.Get(chatId);
        if (chat == null)
            return false; 

        return chat.OwnerId == user.Id;
    }

    public async Task<Result<string>> Create(string title, string description, string ownerUsername, ChatType type)
    {
        if (!ChatInputValidator.IsValidTitle(title))
            return Result<string>.Fail("Title is not valid");
        if (!ChatInputValidator.IsValidDescription(description))
            return Result<string>.Fail("Description is not valid");

        var owner = await _users.FindByNameAsync(ownerUsername);
        if (owner == null)
            return Result<string>.Fail("Owner is not found");

        var id = await _chats.Add(title, description, owner.Id, (int)type);
        await _chats.AddMember(id, owner.Id);

        return Result<string>.Ok(id);
    }

    public async Task<Result> Detele(string id)
    {
        if (!await _chats.IsExists(id))
            return Result.Fail("Chat was not found");

        await _chats.Remove(id);
        return Result.Ok();
    }

    public async Task<Result<Chat>> Get(string id)
    {
        var chat = await _chats.Get(id);
        if(chat == null)
            return Result<Chat>.Fail("Chat was not found");

        // Member may not exist anymore, so we need a plug
        var members = await GetMultipleProfiles(chat.MemberIDs);

        var model = new Chat
        {
            Id = chat.Id.ToString(),
            Title = chat.Title,
            Type = (ChatType)chat.Type,
            Description = chat.Description,
            ImageId = chat.ImageId.ToString() ?? "empty",
            Owner = UserEntityToHeader(chat.Owner
                ?? throw new Exception("Owner profile was not found")),
            Members = [.. members.Select(UserEntityToHeader)]
        };
        return Result<Chat>.Ok(model);
    }

    private async Task<List<UserProfileEntity>> GetMultipleProfiles(List<string> ids)
    {
        return await _users.Users
            .Where(u => ids.Contains(u.Id))
            .ToListAsync();
    }

    private UserProfileHeader UserEntityToHeader(UserProfileEntity user)
    {
        return new UserProfileHeader
        {
            Username = user.UserName
                ?? throw new Exception("User has no username"),
            DisplayName = user.DisplayName,
            ImageId = user.ImageId.ToString() ?? "empty",
        };
    }

    public async Task<List<ChatHeader>> GetForUser(string username)
    {
        var user = await _users.FindByNameAsync(username);
        if (user == null)
            throw new ArgumentException(nameof(username), "User was not found");

        var chats = _chats.GetList(user.Id);
        return chats.Select(c => new ChatHeader
        {
            Id = c.Id.ToString(),
            Title = c.Title,
            ImageId = c.ImageId.ToString() ?? "empty",
        }).ToList();
    }

    public async Task<Result> RemoveMember(string username, string chatId)
    {
        var user = await _users.FindByNameAsync(username);
        if (user == null)
            return Result.Fail("User was not found"); 
        if (!await _chats.IsExists(chatId))
            return Result.Fail("Chat was not found");
        if (await _chats.IsOwner(user.Id, chatId))
            return Result.Fail("Cannot remove an owner");

        await _chats.RemoveMember(chatId, user.Id);

        return Result.Ok();
    }

    public async Task<Result> Update(string id, string title, string description)
    {
        if (!ChatInputValidator.IsValidTitle(title))
            return Result.Fail("Title is not valid");
        if (!ChatInputValidator.IsValidDescription(description))
            return Result.Fail("Description is not valid");

        var chat = await _chats.Get(id);
        if(chat == null)
            return Result.Fail($"Chat:{id} doesn't exist");

        chat.Description = description;
        chat.Title = title;
        
        await _chats.Update(chat);
        return Result.Ok();
    }

    public async Task<List<Message>> GetMessages(string id)
    {
        if (!await _chats.IsExists(id))
            throw new ArgumentException(nameof(id), "Chat was not found");
        var messages = _messages.GetByChatId(id).ToList();

        // Get senders
        var ids = messages.Select(m => m.SenderId).ToList();
        var pairs = await _users.Users
            .Where(u => ids.Contains(u.Id))
            .Select(i => new KeyValuePair<string, UserProfileEntity>(i.Id, i))
            .ToListAsync();
        var senders = new Dictionary<string, UserProfileEntity>(pairs);

        // Map and return
        var model = new List<Message>();
        foreach (var m in messages)
        {
            model.Add(new Message
            {
                Id = m.Id.ToString(),
                Content = m.Content,
                Timestamp = m.CreationDate,
                Sender = m.SenderId == null
                    ? "deleted-user"
                    : senders[m.SenderId].UserName ?? throw new Exception("User with no username"),
            });
        }
        return model;
    }

    public async Task<Result> UpdateImage(string id, byte[] bytes)
    {
        var chat = await _chats.Get(id);
        if (chat == null)
            return Result.Fail("Chat was not found");

        const int smSize = 64; //px
        const int mdSize = 128; //px
        const int lgSize = 256; //px

        var imgSmall = ConvertImage(bytes, smSize);
        var imgMedium = ConvertImage(bytes, mdSize);
        var imgLarge = ConvertImage(bytes, lgSize);

        // Store
        var imageId = chat.ImageId ?? Guid.NewGuid();

        await _images.UpdateOrAddAvatar(new ChatImageEntity
        {
            Id = imageId,
            ChatId = Guid.Parse(id),
            BytesSmall = imgSmall,
            BytesMedium = imgMedium,
            BytesLarge = imgLarge,
        });
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

    public async Task<Result<string>> StoreMessage(string content, string sender, string chatId)
    {
        var user = await _users.FindByNameAsync(sender);
        if (user == null)
            return Result<string>.Fail("Sender was not found");
        if(!await _chats.IsExists(chatId))
            return Result<string>.Fail("Chat was not found");

        var id = await _messages.Add(content, user.Id, chatId);

        return Result<string>.Ok(id);
    }
}

public class ChatInputValidator
{
    public static bool IsValidTitle(string value)
    {
        return value != null && value.Length > 0 && value.Length <= 100;
    }

    public static bool IsValidDescription(string value)
    {
        return value != null && value.Length > 0 && value.Length <= 500;
    }
}