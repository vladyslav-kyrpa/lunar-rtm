using ServerApp.BusinessLogic.Common;
using ServerApp.BusinessLogic.Models;

namespace ServerApp.BusinessLogic.Services.Interfaces;

public interface IChatsService
{
    /// <summary>
    /// Create new chat.
    /// </summary>
    /// <param name="title">Chat title. Lengh limit [1-100] symbols</param>
    /// <param name="description">Chat description. Length limit [1-500] symbols</param>
    /// <param name="ownderUsername">Owner username</param>
    /// <param name="type">Chat type</param>
    /// <returns>New chat ID</returns>
    Task<Result<string>> Create(string title, string description, string ownderUsername, ChatType type);

    /// <summary>
    /// Update chat information. 
    /// </summary>
    /// <param name="id">Chat ID</param>
    /// <param name="title">New title</param>
    /// <param name="description">New description</param>
    /// <returns>Operation result</returns>
    Task<Result> Update(string id, string title, string description);

    /// <summary>
    /// Update chat image.
    /// </summary>
    /// <param name="id">Chat ID</param>
    /// <param name="bytes">Image bytes</param>
    /// <returns>Operation result</returns>
    Task<Result> UpdateImage(string id, byte[] bytes);

    /// <summary>
    /// Delete chat.
    /// </summary>
    /// <param name="id">Chat ID</param>
    /// <returns>Operation result</returns>
    Task<Result> Detele(string id);

    /// <summary>
    /// Get a chat by ID.
    /// </summary>
    /// <param name="id">Chat ID</param>
    /// <returns>Chat</returns>
    Task<Result<Chat>> Get(string id);

    /// <summary>
    /// Get chat list for given user.
    /// </summary>
    /// <param name="username">Username</param>
    /// <returns>Chat list</returns>
    Task<List<ChatHeader>> GetForUser(string username);

    /// <summary>
    /// Add member to a chat.
    /// </summary>
    /// <param name="username">Member username</param>
    /// <param name="chatId">Chat ID</param>
    /// <returns>Operation result</returns>
    Task<Result> AddMember(string username, string chatId);

    /// <summary>
    /// Change member role.
    /// </summary>
    /// <param name="username">Member username</param>
    /// <param name="chatId">Chat ID</param>
    /// <param name="role">New role</param>
    /// <returns>Operation result</returns>
    Task<Result> ChangeMemberRole(string username, string chatId, string role);

    /// <summary>
    /// Remove member from a chat.
    /// </summary>
    /// <param name="username">Member username</param>
    /// <param name="chatId">Chat ID</param>
    /// <returns>Operation result</returns>
    Task<Result> RemoveMember(string username, string chatId);

    /// <summary>
    /// Get stored chat messages.
    /// </summary>
    /// <param name="id">Chat ID</param>
    /// <returns>Chat messages</returns>
    Task<List<Message>> GetMessages(string id);

    /// <summary>
    /// Store message permanently
    /// </summary>
    /// <param name="content">Message content</param>
    /// <param name="sender">Message sender</param>
    /// <param name="chatId">Chat ID</param>
    /// <returns>Message ID</returns>
    Task<Result<string>> StoreMessage(string content, string sender, string chatId);

    /// <summary>
    /// Get chat member permissions 
    /// </summary>
    /// <param name="chatId">Chat ID</param>
    /// <param name="username">Member's username</param>
    /// <returns>Permissions</returns>
    Task<Result<ChatMemberPermissions>> GetMemberPermissions(string chatId, string username);
}