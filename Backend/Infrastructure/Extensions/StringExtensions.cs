using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Infrastructure.Extensions;

public static class StringExtensions
{
    public static T ToObject<T>(this string json)
    {
        return JsonConvert
            .DeserializeObject<T>(
                json,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Converters = { new StringEnumConverter() }
                });
    }
}