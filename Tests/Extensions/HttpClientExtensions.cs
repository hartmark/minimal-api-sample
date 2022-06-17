using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Entities.DataContract.ErrorResponse;
using Microsoft.Net.Http.Headers;

namespace Tests.Extensions;

public static class HttpClientExtensions
{
    public static async Task<(string Content, HttpStatusCode StatusCode, T ResponseObject)> GetExtendedAsync<T>(
        this HttpClient client, string url, string jwt)
    {
        SetJwtHeader(client, jwt);

        var response = await client.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();

        var responseObject = default(T);
        if (!typeof(T).Name.Equals("string", StringComparison.InvariantCultureIgnoreCase))
        {
            responseObject = await response.Content.ReadFromJsonAsync<T>(options: DefaultSerializerOptions);
        }
        else
        {
            content = content.TrimStart('\"');
            content = content.TrimEnd('\"');
        }

        return (content, response.StatusCode, responseObject);
    }

    public static async Task<(string Content, HttpStatusCode StatusCode, BadRequestResponse BadRequestResponse)>
        PostExtendedAsync<T>(this HttpClient client, string url, T request, string jwt)
    {
        SetJwtHeader(client, jwt);

        var response = await client.PostAsJsonAsync(url, request, DefaultSerializerOptions);

        var content = await response.Content.ReadAsStringAsync();

        BadRequestResponse badRequestResponse = null;
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            badRequestResponse = await response.Content.ReadFromJsonAsync<BadRequestResponse>();
        }

        return (content, response.StatusCode, badRequestResponse);
    }

    public static async Task<(string Content, HttpStatusCode StatusCode, BadRequestResponse BadRequestResponse)>
        PutExtendedAsync<T>(this HttpClient client, string url, T request, string jwt)
    {
        SetJwtHeader(client, jwt);

        var response = await client.PutAsJsonAsync(url, request, DefaultSerializerOptions);

        var content = await response.Content.ReadAsStringAsync();

        BadRequestResponse badRequestResponse = null;
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            badRequestResponse = await response.Content.ReadFromJsonAsync<BadRequestResponse>();
        }

        return (content, response.StatusCode, badRequestResponse);
    }

    public static async Task<(string Content, HttpStatusCode StatusCode, BadRequestResponse BadRequestResponse)>
        PutExtendedAsync(this HttpClient client, string url, string jwt)
    {
        SetJwtHeader(client, jwt);

        var response = await client.PutAsync(url, null);

        var content = await response.Content.ReadAsStringAsync();

        BadRequestResponse badRequestResponse = null;
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            badRequestResponse = await response.Content.ReadFromJsonAsync<BadRequestResponse>();
        }

        return (content, response.StatusCode, badRequestResponse);
    }

    public static async Task<(string Content, HttpStatusCode StatusCode)> DeleteExtendedAsync(this HttpClient client,
        string url, string jwt)
    {
        SetJwtHeader(client, jwt);

        var response = await client.DeleteAsync(url);

        var content = await response.Content.ReadAsStringAsync();
        return (content, response.StatusCode);
    }

    private static void SetJwtHeader(HttpClient client, string jwt)
    {
        if (client.DefaultRequestHeaders.Contains(HeaderNames.Authorization))
        {
            client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
        }

        client.DefaultRequestHeaders.Add(HeaderNames.Authorization, "Bearer " + jwt);
    }

    private static JsonSerializerOptions DefaultSerializerOptions => new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = {new JsonStringEnumConverter()}
    };
}