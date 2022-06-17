using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Infrastructure.Extensions;

public static class ObjectExtensions
{
    public static string ToJson(this object value, bool indented = false)
    {
        return JsonConvert
            .SerializeObject(
                value,
                (indented ? Formatting.Indented : Formatting.None),
                new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Converters = { new StringEnumConverter() }
                });
    }
}