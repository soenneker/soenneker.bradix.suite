using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Soenneker.Bradix;

internal static class BradixInteropJson
{
    // A null JS result stays null; custom models are always read with generated metadata.
    internal static T? Deserialize<T>(JsonElement? payload, JsonTypeInfo<T> typeInfo) =>
        payload is null || payload.Value.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined
            ? default
            : JsonSerializer.Deserialize(payload.Value, typeInfo);
}
