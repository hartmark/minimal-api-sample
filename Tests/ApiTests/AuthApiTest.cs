using System.Net;
using System.Threading.Tasks;
using Tests.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Tests.ApiTests;

[Trait("Category", "CI")]
public class AuthApiTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public AuthApiTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }
    
    [Theory]
    [ClassData(typeof(TestDataGenerator))]
    public async Task Login_WhenMissingUsernameAndPassword_ShouldReturnBadRequest(TestHost testHost)
    {
        var (_, statusCode, _) = await testHost.HttpClient.GetExtendedAsync<string>(
            $"/api/v1/login", jwt: null);

        Assert.Equal(HttpStatusCode.BadRequest, statusCode);
    }
    
    [Theory]
    [ClassData(typeof(TestDataGenerator))]
    public async Task Login_WhenMissingUsername_ShouldReturnBadRequest(TestHost testHost)
    {
        const string password = "foo";
        var (_, statusCode, _) = await testHost.HttpClient.GetExtendedAsync<string>(
            $"/api/v1/login?password={password}", jwt: null);

        Assert.Equal(HttpStatusCode.BadRequest, statusCode);
    }
    
    [Theory]
    [ClassData(typeof(TestDataGenerator))]
    public async Task Login_WhenMissingPassword_ShouldReturnBadRequest(TestHost testHost)
    {
        const string username = "foo";
        var (_, statusCode, _) = await testHost.HttpClient.GetExtendedAsync<string>(
            $"/api/v1/login?username={username}", jwt: null);

        Assert.Equal(HttpStatusCode.BadRequest, statusCode);
    }
    
    [Theory]
    [ClassData(typeof(TestDataGenerator))]
    public async Task Login_WhenUsingInvalidUser_ShouldReturnUnauthorized(TestHost testHost)
    {
        const string username = "invalid";
        const string password = "badpass";
        var (content, statusCode, _) = await testHost.HttpClient.GetExtendedAsync<string>(
            $"/api/v1/login?username={username}&password={password}", jwt: null);

        Assert.Equal(HttpStatusCode.Unauthorized, statusCode);
        _testOutputHelper.WriteLine(content);
    }
    
    [Theory]
    [ClassData(typeof(TestDataGenerator))]
    public async Task Login_WhenUsingValidUser_ShouldReturnJwt(TestHost testHost)
    {
        const string username = "harre";
        const string password = "errah";
        var (content, statusCode, _) = await testHost.HttpClient.GetExtendedAsync<string>(
            $"/api/v1/login?username={username}&password={password}", jwt: null);

        Assert.Equal(HttpStatusCode.OK, statusCode);
        _testOutputHelper.WriteLine(content);
        Assert.NotNull(content);
    }
}