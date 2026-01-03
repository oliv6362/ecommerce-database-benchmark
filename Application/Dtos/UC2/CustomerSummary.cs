namespace EcommerceDatabaseBenchmark.Application.Dtos.UC2
{
    public sealed record CustomerSummary(
        int CustomerId,
        string FirstName,
        string LastName,
        string Email);
}
