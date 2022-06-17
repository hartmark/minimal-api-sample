namespace Api.Apis;

public interface IAuthApi
{
    IResult Login(string username, string password);
}