using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace Soenneker.Bradix;

/// <summary>Provides generated JSON metadata for Bradix interop payloads.</summary>
[JsonSourceGenerationOptions(JsonSerializerDefaults.Web, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(BradixDelegatedFocusEvent))]
[JsonSerializable(typeof(BradixDelegatedInteractionOptions))]
[JsonSerializable(typeof(BradixDelegatedKeyboardEvent))]
[JsonSerializable(typeof(BradixDelegatedMouseEvent))]
[JsonSerializable(typeof(BradixFormControlSnapshot))]
[JsonSerializable(typeof(BradixFormValiditySnapshot))]
[JsonSerializable(typeof(BradixPopperInteropOptions))]
[JsonSerializable(typeof(BradixPresenceSnapshot))]
[JsonSerializable(typeof(string[]))]
[JsonSerializable(typeof(IReadOnlyList<string>))]
public partial class BradixInteropJsonContext : JsonSerializerContext;
