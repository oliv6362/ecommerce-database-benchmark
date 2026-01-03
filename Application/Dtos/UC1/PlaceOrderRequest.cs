namespace EcommerceDatabaseBenchmark.Application.Dtos.UC1
{
    public class PlaceOrderRequest
    {
        public int CustomerId { get; init; }
        public IReadOnlyList<OrderItemRequest> OrderItems { get; init; } = [];
    }
}
