using Microsoft.EntityFrameworkCore;
using ServerApp.DataAccess.Entities;
using ServerApp.DataAccess.Repositories.Interfaces;

namespace ServerApp.DataAccess.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly MainDbContext _context;

    public MessageRepository(MainDbContext context)
    {
        _context = context;
    }

    public async Task<string> Add(string content, string senderId, string chatId)
    {
        var id = Guid.NewGuid();
        _context.Messages.Add(new MessageEntity
        {
            Id = id,
            Content = content,
            SenderId = senderId,
            ChatId = Guid.Parse(chatId),
            CreationDate = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();
        return id.ToString();
    }

    public async Task Delete(string id)
    {
        var guidId = Guid.Parse(id);
        var msg = await _context.Messages.FirstOrDefaultAsync(m => m.Id == guidId);
        if (msg == null)
            throw new ArgumentException(nameof(id), "Message was not found");
        _context.Messages.Remove(msg);
        await _context.SaveChangesAsync();
    }

    public IQueryable<MessageEntity> GetByChatId(string chatId)
    {
        var guidId = Guid.Parse(chatId);
        return _context.Messages.Where(m => m.ChatId == guidId);
    }

    public async Task Update(MessageEntity msg)
    {
        if (!IsMessageValid(msg))
            throw new ArgumentException(nameof(msg), "Message is not valid");
        _context.Messages.Update(msg);
        await _context.SaveChangesAsync();
    }

    private bool IsMessageValid(MessageEntity msg)
        => msg.Id != Guid.Empty && !string.IsNullOrEmpty(msg.Content)
        && msg.ChatId != Guid.Empty && !string.IsNullOrEmpty(msg.SenderId);
}