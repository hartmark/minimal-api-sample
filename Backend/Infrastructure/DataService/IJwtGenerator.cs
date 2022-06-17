namespace Infrastructure.DataService;

public interface IJwtGenerator
{
    string GetJwt(string username);
}