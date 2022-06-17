using Entities.Enums;
using Infrastructure.DataAccess;

namespace Infrastructure.DataService;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public IEnumerable<Role> GetRolesForUser(string username)
    {
        return _userRepository.GetRolesForUser(username);
    }

    public bool TryLogin(string username, string password)
    {
        return _userRepository.TryLogin(username, password);
    }
}