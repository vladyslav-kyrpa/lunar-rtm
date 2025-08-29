using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServerApp.BusinessLogic.Common;
using ServerApp.BusinessLogic.Models;
using ServerApp.BusinessLogic.Services.Interfaces;
using ServerApp.DataAccess.Entities;
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

    public async Task<Result> AddMemberAsync(string username, string chatId)
    {
        var user = await _users.FindByNameAsync(username);
        if (user == null)
            return Result.Fail("User was not found");

        var chat = await _chats.Get(chatId);
        if (chat == null)
            return Result.Fail("Chat was not found");

        if (chat.Type == (int)ChatType.Dialogue && chat.Members.Count >= 2)
            return Result.Fail("Dialogue can have only two members");

        await _chats.AddMember(chatId, user.Id, ChatMemberRole.Regular);
        return Result.Ok();
    }

    public async Task<Result<string>> CreateChatAsync(string title, string description, string ownerUsername, ChatType type)
    {
        if (!ChatInputValidator.IsValidTitle(title))
            return Result<string>.Fail("Title is not valid");
        if (!ChatInputValidator.IsValidDescription(description))
            return Result<string>.Fail("Description is not valid");

        var owner = await _users.FindByNameAsync(ownerUsername);
        if (owner == null)
            return Result<string>.Fail("Owner is not found");

        var id = await _chats.Add(title, description, owner.Id, (int)type);

        return Result<string>.Ok(id);
    }

    public async Task<Result> DeteleChatAsync(string id)
    {
        if (!await _chats.IsExists(id))
            return Result.Fail("Chat was not found");

        await _chats.Remove(id);
        return Result.Ok();
    }

    public async Task<Result<Chat>> GetAsync(string id)
    {
        var chat = await _chats.Get(id);
        if (chat == null)
            return Result<Chat>.Fail("Chat was not found");
        return Result<Chat>.Ok(EntityToModel(chat));
    }

    private Chat EntityToModel(ChatEntity chat)
    {
        var members = chat.Members.Select(m => UserEntityToHeader(m.User ??
            throw new Exception("Member user is null")));
        var model = new Chat
        {
            Id = chat.Id.ToString(),
            Title = chat.Title,
            Type = (ChatType)chat.Type,
            Description = chat.Description,
            ImageId = chat.ImageId.ToString() ?? "empty",
            Members = members.ToList()
        };
        return model;
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

    public async Task<List<ChatHeader>> GetChatsForUserAsync(string username)
    {
        var user = (await _users.FindByNameAsync(username)) ??
            throw new ArgumentException(nameof(username), "User was not found");
        return EntitiesToHeaders(_chats.GetList(user.Id));
    }

    private static List<ChatHeader> EntitiesToHeaders(IQueryable<ChatEntity> chats)
    {
        return chats.Select(c => new ChatHeader
        {
            Id = c.Id.ToString(),
            Title = c.Title,
            ImageId = c.ImageId.ToString() ?? "empty",
        }).ToList();
    }

    public async Task<Result> RemoveMemberAsync(string username, string chatId)
    {
        var user = await _users.FindByNameAsync(username);
        if (user == null)
            return Result.Fail("User was not found");

        if (!await _chats.IsExists(chatId))
            return Result.Fail("Chat was not found");

        var member = await _chats.GetMember(user.Id, chatId);
        if (member?.Role == ChatMemberRole.Owner)
            return Result.Fail("Owner cannot be removed");

        await _chats.RemoveMember(chatId, user.Id);

        return Result.Ok();
    }

    public async Task<Result> UpdateChatAsync(string id, string title, string description)
    {
        if (!ChatInputValidator.IsValidTitle(title))
            return Result.Fail("Title is not valid");
        if (!ChatInputValidator.IsValidDescription(description))
            return Result.Fail("Description is not valid");

        var chat = await _chats.Get(id);
        if (chat == null)
            return Result.Fail($"Chat:{id} doesn't exist");

        chat.Description = description;
        chat.Title = title;

        await _chats.Update(chat);

        return Result.Ok();
    }

    public async Task<List<Message>> GetChatMessagesAsync(string id)
    {
        if (!await _chats.IsExists(id))
            throw new ArgumentException(nameof(id), "Chat was not found");

        var messages = _messages.GetByChatId(id).ToList();
        var senders = await GetSenders(messages);

        return messages
            .Select(m => MessageEntityToModel(m, senders))
            .ToList();
    }

    private static Message MessageEntityToModel(MessageEntity message,
        Dictionary<string, UserProfileEntity> senders) => new()
        {
            Id = message.Id.ToString(),
            Content = message.Content,
            Timestamp = message.CreationDate,
            Sender = message.SenderId == null
                ? "deleted-user"
                : senders[message.SenderId].UserName ?? throw new Exception("User with no username"),
        };

    private async Task<Dictionary<string, UserProfileEntity>> GetSenders(List<MessageEntity> messages)
    {
        var ids = messages.Select(m => m.SenderId).ToList();
        var pairs = await _users.Users
            .Where(u => ids.Contains(u.Id))
            .Select(i => new KeyValuePair<string, UserProfileEntity>(i.Id, i))
            .ToListAsync();
        var senders = new Dictionary<string, UserProfileEntity>(pairs);
        return senders;
    }

    public async Task<Result> UpdateImageAsync(string id, byte[] bytes)
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

    public async Task<Result<string>> StoreMessageAsync(string content, string sender, string chatId)
    {
        var user = await _users.FindByNameAsync(sender);
        if (user == null)
            return Result<string>.Fail("Sender was not found");
        if (!await _chats.IsExists(chatId))
            return Result<string>.Fail("Chat was not found");

        var id = await _messages.Add(content, user.Id, chatId);

        return Result<string>.Ok(id);
    }

    public async Task<Result<ChatMemberPermissions>> GetMemberPermissionsAsync(string chatId, string username)
    {
        if (!await _chats.IsExists(chatId))
            return Result<ChatMemberPermissions>.Fail("Chat was not found");

        var user = await _users.FindByNameAsync(username);
        if (user == null)
            return Result<ChatMemberPermissions>.Fail("User was not found");

        var member = await _chats.GetMember(user.Id, chatId);
        if (member == null)
            return Result<ChatMemberPermissions>.Fail("User is not a member");

        var permissions = new ChatMemberPermissions(member?.Role ??
            throw new Exception("Member role is null"));

        return Result<ChatMemberPermissions>.Ok(permissions);
    }

    public async Task<Result> ChangeMemberRoleAsync(string username, string chatId, string role)
    {
        var user = await _users.FindByNameAsync(username);
        if (user == null)
            return Result.Fail("User was not found");

        var member = await _chats.GetMember(user.Id, chatId);
        if (member == null)
            return Result.Fail("User is not a member");

        if (member.Role == ChatMemberRole.Owner)
            return Result.Fail("Cannot change an owner");

        var chatMemberRole = MapStringToRole(role);
        if (chatMemberRole == null)
            return Result.Fail("Unsupported role");

        member.Role = chatMemberRole ??
            throw new Exception("Role shouldn't be null here");

        await _chats.UpdateMember(member);

        return Result.Ok();
    }

    private static ChatMemberRole? MapStringToRole(string value)
    {
        return value switch
        {
            "regular" => ChatMemberRole.Regular,
            "moderator" => ChatMemberRole.Moderator,
            _ => null
        };
    }
}