using Microsoft.EntityFrameworkCore;
using ServerApp.DataAccess.Entities;
using ServerApp.DataAccess.Repositories.Interfaces;

namespace ServerApp.DataAccess.Repositories;

public class ChatRepository : IChatRepository
{
    private readonly MainDbContext _context;

    public ChatRepository(MainDbContext context)
    {
        _context = context;
    }

    public async Task<string> Add(string title, string description, string ownerId, int type)
    {
        var id = Guid.NewGuid();
        _context.Chats.Add(new ChatEntity
        {
            Id = id,
            Title = title,
            Description = description,
            Type = type,
            CreationDate = DateTime.UtcNow
        });
        _context.ChatMembers.Add(new ChatMemberEntity
        {
            ChatId = id,
            UserId = ownerId,
            Role = ChatMemberRole.Owner
        });
        await _context.SaveChangesAsync();
        return id.ToString();
    }

    public async Task AddMember(string chatId, string memberId, ChatMemberRole role)
    {
        var guidId = Guid.Parse(chatId);

        _context.ChatMembers.Add(new ChatMemberEntity
        {
            ChatId = guidId,
            UserId = memberId,
            Role = role
        });

        await _context.SaveChangesAsync();
    }

    public async Task<ChatEntity?> Get(string id)
    {
        var guidId = Guid.Parse(id);
        return await _context.Chats
            .Include(x => x.Members)
            .ThenInclude(x=>x.User)
            .FirstOrDefaultAsync(c => c.Id == guidId);
    }

    public IQueryable<ChatEntity> GetList(string userId)
    {
        return _context.Chats
            .AsNoTracking()
            .Where(c => c.Members.Any(x => x.UserId == userId));
    }

    public async Task<bool> IsExists(string id)
    {
        var guidId = Guid.Parse(id);
        return await _context.Chats
            .AsNoTracking()
            .AnyAsync(c => c.Id == guidId);
    }

    public async Task<ChatMemberEntity?> GetMember(string userId, string chatId)
    {
        var guidId = Guid.Parse(chatId);
        var member = await _context.ChatMembers
            .FirstOrDefaultAsync(x => x.UserId == userId && x.ChatId == guidId);
        return member;
    }

    public async Task UpdateMember(ChatMemberEntity member)
    {
        _context.Update(member);
        await _context.SaveChangesAsync();
    } 

    public async Task Remove(string id)
    {
        var guidId = Guid.Parse(id);
        var chat = await _context.Chats.FirstOrDefaultAsync(c => c.Id == guidId)
            ?? throw new ArgumentException("Chat was not found", nameof(id));

        _context.Chats.Remove(chat);

        await _context.SaveChangesAsync();
    }

    public async Task RemoveMember(string id, string memberId)
    {
        var guidId = Guid.Parse(id);
        var member = await _context.ChatMembers
            .FirstOrDefaultAsync(x => x.UserId == memberId && x.ChatId == guidId)
            ?? throw new ArgumentException("User is not a member", nameof(memberId));

        _context.ChatMembers.Remove(member);

        await _context.SaveChangesAsync();
    }

    public async Task Update(ChatEntity chat)
    {
        _context.Chats.Update(chat);
        await _context.SaveChangesAsync();
    }
}