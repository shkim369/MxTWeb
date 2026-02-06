using System.Collections.Concurrent;

namespace MxTWeb.Services
{
    public class ActiveUserInfo
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public DateTime ConnectedAt { get; set; }
        public DateTime LastActivity { get; set; }
        public string? IpAddress { get; set; }
    }

    public class ActiveUserService
    {
        // Key = CircuitId (unique per Blazor connection)
        private readonly ConcurrentDictionary<string, ActiveUserInfo> _activeUsers = new();

        public void AddUser(string circuitId, ActiveUserInfo userInfo)
        {
            _activeUsers[circuitId] = userInfo;
        }

        public void RemoveUser(string circuitId)
        {
            _activeUsers.TryRemove(circuitId, out _);
        }

        public void UpdateActivity(string circuitId)
        {
            if (_activeUsers.TryGetValue(circuitId, out var info))
            {
                info.LastActivity = DateTime.Now;
            }
        }

        public List<ActiveUserInfo> GetActiveUsers()
        {
            return _activeUsers.Values
                .OrderByDescending(u => u.ConnectedAt)
                .ToList();
        }

        public int GetActiveUserCount() => _activeUsers.Count;
    }
}
