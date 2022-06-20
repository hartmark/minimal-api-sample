using System.Net.Http;
using Infrastructure.DataAccess;

namespace Tests.ApiTests;

public record TestHost(string Name, HttpClient HttpClient, IWeatherRepository WeatherRepositoryMock)
{
}