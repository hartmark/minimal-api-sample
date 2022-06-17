using System.Text.Json.Serialization;

namespace Entities.Jwt;

public record WeatherServiceJwtPayload
{
    [JsonInclude]
    public string Username { get; set; }
    
    [JsonInclude]
    public List<string> Roles { get; set; }
}