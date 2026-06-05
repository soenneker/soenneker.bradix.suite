namespace Soenneker.Bradix;

/// <summary>
/// Represents the bradix collection entry record.
/// </summary>
/// <typeparam name="TItem">The TItem type.</typeparam>
/// <param name="Key">The key.</param>
/// <param name="Item">The item.</param>
public sealed record BradixCollectionEntry<TItem>(string Key, TItem Item);
