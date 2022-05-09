namespace FPTS.FIT.BDRD.Services.Ordering.Domain.AggregateModels.OrderAggregate
{
    public interface IInMemoryOrderStore : IKeyValuePairRepository<Order,string>
    {
        Task DispatchDomainEvent(Order order);
        IEnumerable<OrderItem> GetItemsOfOrder(string id);
        IEnumerable<Order> GetOrders();
        bool Remove(string orderId);
    }
}
