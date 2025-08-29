namespace ServerApp.BusinessLogic.Services.Interfaces;

public interface IPresenceService
{
    /// <summary>
    /// Get existent connections of given users.
    /// </summary>
    /// <param name="usernames">Usernames</param>
    /// <returns>Connections ID</returns>
    List<string> GetMultipleAsync(string[] usernames);

    /// <summary>
    /// Add user connection
    /// </summary>
    /// <param name="username">Username</param>
    /// <param name="connnection">Connnection ID</param>
    void AddAsync(string username, string connnection);

    /// <summary>
    /// Remove connection
    /// </summary>
    /// <param name="connection">Connection ID</param>
    void RemoveAsync(string connection);
}