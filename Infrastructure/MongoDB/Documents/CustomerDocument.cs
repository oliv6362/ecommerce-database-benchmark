using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EcommerceDatabaseBenchmark.Infrastructure.MongoDB.Documents;

/// <summary>
/// MongoDB document representing a customer.
/// </summary>
public sealed class CustomerDocument
{
    [BsonId] public ObjectId Id { get; set; }
    public int CustomerId { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public DateTimeOffset CreatedAt { get; set; }
}
