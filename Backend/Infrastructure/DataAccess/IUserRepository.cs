using Entities.Enums;

namespace Infrastructure.DataAccess;

public interface IUserRepository
{
    IEnumerable<Role> GetRolesForUser(string username);
    bool TryLogin(string username, string password);
}