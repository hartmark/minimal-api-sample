using System.Net;
using System.Threading.Tasks;
using Tests.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Tests.ApiTests;

[Trait("Category", "CI")]
public class AuthApiTest : ApiTestBase
{
    private readonly ITestOutputHelper _testOutputHelper;

    public AuthApiTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }
    
    [Fact]
    public async Task Login_WhenMissingUsernameAndPassword_ShouldReturnBadRequest()
    {
        var (_, statusCode, _) = await Client.GetExtendedAsync<string>(
            $"/api/v1/login", jwt: null);

        Assert.Equal(HttpStatusCode.BadRequest, statusCode);
    }
    
    [Fact]
    public async Task Login_WhenMissingUsername_ShouldReturnBadRequest()
    {
        const string password = "foo";
        var (_, statusCode, _) = await Client.GetExtendedAsync<string>(
            $"/api/v1/login?password={password}", jwt: null);

        Assert.Equal(HttpStatusCode.BadRequest, statusCode);
    }
    
    [Fact]
    public async Task Login_WhenMissingPassword_ShouldReturnBadRequest()
    {
        const string username = "foo";
        var (_, statusCode, _) = await Client.GetExtendedAsync<string>(
            $"/api/v1/login?username={username}", jwt: null);

        Assert.Equal(HttpStatusCode.BadRequest, statusCode);
    }
    
    [Fact]
    public async Task Login_WhenUsingInvalidUser_ShouldReturnUnauthorized()
    {
        const string username = "invalid";
        const string password = "badpass";
        var (content, statusCode, _) = await Client.GetExtendedAsync<string>(
            $"/api/v1/login?username={username}&password={password}", jwt: null);

        Assert.Equal(HttpStatusCode.Unauthorized, statusCode);
        _testOutputHelper.WriteLine(content);
    }
    
    [Fact]
    public async Task Login_WhenUsingValidUser_ShouldReturnJwt()
    {
        const string username = "harre";
        const string password = "errah";
        var (content, statusCode, _) = await Client.GetExtendedAsync<string>(
            $"/api/v1/login?username={username}&password={password}", jwt: null);

        Assert.Equal(HttpStatusCode.OK, statusCode);
        _testOutputHelper.WriteLine(content);
        Assert.NotNull(content);
    }
}