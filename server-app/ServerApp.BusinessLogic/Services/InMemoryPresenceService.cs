using ServerApp.BusinessLogic.Services.Interfaces;

namespace ServerApp.BusinessLogic.Services;

public class PresenceService : IPresenceService
{
    private Dictionary<string, HashSet<string>> _connections = []; // username, connectionID
    private readonly object _lock = new object();

    public void AddAsync(string username, string connnection)
    {
        lock (_lock)
        {
            if (!_connections.ContainsKey(username))
                _connections[username] = new HashSet<string>();
            _connections[username].Add(connnection);
        }
    }

    public List<string> GetMultipleAsync(string[] usernames)
    {
        lock (_lock)
        {
            return usernames
                .Where(_connections.ContainsKey)
                .SelectMany(u => _connections[u])
                .ToList();
        }
    }

    public void RemoveAsync(string connection)
    {
        lock (_lock)
        {
            var userConnections = _connections.FirstOrDefault(x => x.Value.Contains(connection));
            if (userConnections.Key == null)
                throw new ArgumentException("User have no active connections");

            userConnections.Value.Remove(connection);
        
            if (userConnections.Value.Count == 0)
                _connections.Remove(userConnections.Key);
        }
    }
}