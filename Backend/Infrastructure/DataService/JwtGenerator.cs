using System.Security.Cryptography;
using JWT.Algorithms;
using JWT.Builder;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.DataService;

public class JwtGenerator : IJwtGenerator
{
    private readonly ECDsa _publicKey;
    private readonly ECDsa _privateKey;
    private readonly string _secret;

    public JwtGenerator(IConfiguration configuration, ECDsa publicKey, ECDsa privateKey)
    {
        _publicKey = publicKey;
        _privateKey = privateKey;
        _secret = configuration["Secret"] ?? throw new Exception("Secret missing in configuration");
    }
    
    public string GetJwt(string username)
    {
        var token = JwtBuilder.Create()
            .WithAlgorithm(new ES512Algorithm(_publicKey, _privateKey))
            .WithSecret(_secret);
        token.AddClaim("Username", username);

        return token.Encode();
    }
}