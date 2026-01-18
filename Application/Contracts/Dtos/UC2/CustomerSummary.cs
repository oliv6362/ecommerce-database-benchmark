namespace EcommerceDatabaseBenchmark.Application.Contracts.Dtos.UC2
{
    public sealed record CustomerSummary(
        int CustomerId,
        string FirstName,
        string LastName,
        string Email);
}
