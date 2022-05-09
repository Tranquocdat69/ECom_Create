namespace FPTS.FIT.BDRD.Services.Ordering.Domain.AggregateModels.OrderAggregate
{
    public interface IOrderRepository : IRepositoryBase<Order>
    {
        void Update(Order order);
        void Add(Order order);
        bool Delete(Order t);
        Order GetOrder(string id);
        IEnumerable<OrderItem> GetItems(string id);
        IEnumerable<Order> GetAll();
    }
}
