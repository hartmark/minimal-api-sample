using Entities.Enums;

namespace Infrastructure.DataService;

public interface IUserService
{
    IEnumerable<Role> GetRolesForUser(string username);
    bool TryLogin(string username, string password);
}