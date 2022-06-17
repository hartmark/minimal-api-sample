using Infrastructure.DataService;

namespace Api.Apis;

public class AuthApi : ApiBase, IAuthApi
{
    private readonly IJwtGenerator _jwtGenerator;

    public AuthApi(ILogger<AuthApi> logger, IJwtGenerator jwtGenerator, IUserService userService) : base(logger, userService)
    {
        _jwtGenerator = jwtGenerator;
    }

    public IResult Login(string username, string password)
    {
        return UserService.TryLogin(username, password) ?
            Results.Ok(_jwtGenerator.GetJwt(username)) : 
            Results.Unauthorized();
    }
}