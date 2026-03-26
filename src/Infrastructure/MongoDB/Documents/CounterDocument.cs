using MongoDB.Bson.Serialization.Attributes;

namespace EcommerceDatabaseBenchmark.Infrastructure.MongoDB.Documents;

/// <summary>
/// MongoDB document used for generating sequential numeric identifiers.
/// </summary>
public sealed class CounterDocument
{
    // Identifier of the counter (e.g. "orderId").
    [BsonId] public string Id { get; set; } = default!;
    public int Value { get; set; }
}
