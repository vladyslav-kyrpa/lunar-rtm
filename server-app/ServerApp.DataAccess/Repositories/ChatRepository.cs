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
            OwnerId = ownerId,
            Type = type,
            CreationDate = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();
        return id.ToString();
    }

    public async Task AddMember(string id, string memberId)
    {
        var guidId = Guid.Parse(id);
        var chat = await _context.Chats.FirstOrDefaultAsync(c => c.Id == guidId);
        if (chat == null)
            throw new ArgumentException("Chat was not found", nameof(id));
        chat.MemberIDs.Add(memberId);
        await _context.SaveChangesAsync();
    }

    public async Task<ChatEntity?> Get(string id)
    {
        var guidId = Guid.Parse(id);
        return await _context.Chats
            .Include(x => x.Owner)
            .FirstOrDefaultAsync(c => c.Id == guidId);
    }

    public IQueryable<ChatEntity> GetList(string userId)
    {
        return _context.Chats
            .AsNoTracking()
            .Where(c => c.MemberIDs.Contains(userId));
    }

    public async Task<bool> IsExists(string id)
    {
        var guidId = Guid.Parse(id);
        return await _context.Chats
            .AsNoTracking()
            .AnyAsync(c => c.Id == guidId);
    }

    public async Task Remove(string id)
    {
        var guidId = Guid.Parse(id);
        var chat = await _context.Chats.FirstOrDefaultAsync(c => c.Id == guidId);
        if (chat == null)
            throw new ArgumentException("Chat was not found", nameof(id));

        _context.Chats.Remove(chat);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveMember(string id, string memberId)
    {
        var guidId = Guid.Parse(id);
        var chat = await _context.Chats.FirstOrDefaultAsync(c => c.Id == guidId);
        if (chat == null)
            throw new ArgumentException("Chat was not found", nameof(id));

        chat.MemberIDs.Remove(memberId);
        await _context.SaveChangesAsync();
    }

    public async Task Update(ChatEntity chat)
    {
        if (!IsChatValid(chat))
            throw new ArgumentException("Invalid chat entity", nameof(chat));

        _context.Chats.Update(chat);
        await _context.SaveChangesAsync();
    }

    private bool IsChatValid(ChatEntity chat)
        => chat != null && chat.Id != Guid.Empty
            && chat.OwnerId != string.Empty;
}