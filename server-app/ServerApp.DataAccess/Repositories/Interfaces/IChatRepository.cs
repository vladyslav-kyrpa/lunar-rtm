using ServerApp.DataAccess.Entities;

namespace ServerApp.DataAccess.Repositories.Interfaces;

public interface IChatRepository
{
    Task<string> Add(string title, string description, string ownerId, int type);
    Task AddMember(string chatId, string memberId, ChatMemberRole role);
    Task RemoveMember(string id, string memberId);
    Task Remove(string id);
    Task<ChatEntity?> Get(string id);
    Task Update(ChatEntity chat);
    IQueryable<ChatEntity> GetList(string userId);
    Task<bool> IsExists(string id);
    Task<ChatMemberEntity?> GetMember(string userId, string chatId);
    Task UpdateMember(ChatMemberEntity member);
}