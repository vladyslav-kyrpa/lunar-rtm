using ServerApp.DataAccess.Entities;

namespace ServerApp.DataAccess.Repositories.Interfaces;

public interface IMessageRepository
{
    Task<string> Add(string content, string senderId, string chatId);
    Task Update(MessageEntity msg);
    Task Delete(string id);
    IQueryable<MessageEntity> GetByChatId(string chatId);
}