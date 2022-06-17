using Entities.Enums;

namespace Infrastructure.DataAccess;

public class UserRepository : IUserRepository
{
    private readonly Dictionary<string, string> _usersAndPasswords = new()
    {
        { "harre", "errah" },
        { "noob", "password" }
    };
    
    public IEnumerable<Role> GetRolesForUser(string username)
    {
        switch (username)
        {
            case "harre":
                yield return Role.Write;
                yield return Role.Read;
                yield break;
            case "noob":
                yield return Role.Read;
                yield break;
            default:
                yield return Role.None;
                yield break;
        }
    }

    public bool TryLogin(string username, string password)
    {
        return _usersAndPasswords.ContainsKey(username) && _usersAndPasswords[username].Equals(password);
    }
}