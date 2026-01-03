namespace EcommerceDatabaseBenchmark.Application.Dtos
{
    public class PlaceOrderRequest
    {
        public int CustomerId { get; init; }
        public IReadOnlyList<OrderItemRequest> OrderItems { get; init; } = [];
    }
}
