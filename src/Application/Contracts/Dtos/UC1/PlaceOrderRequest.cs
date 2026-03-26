namespace EcommerceDatabaseBenchmark.Application.Contracts.Dtos.UC1
{
    public class PlaceOrderRequest
    {
        public int CustomerId { get; init; }
        public IReadOnlyList<OrderItemRequest> OrderItems { get; init; } = [];
    }
}
