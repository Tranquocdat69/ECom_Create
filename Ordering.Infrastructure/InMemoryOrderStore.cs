namespace FPTS.FIT.BDRD.Services.Ordering.Infrastructure
#nullable disable
{
    public class InMemoryOrderStore : IInMemoryOrderStore
    {
        private static Dictionary<string, Order> s_dataStore;
        private readonly IMediator _mediator;

        public InMemoryOrderStore(IMediator mediator)
        {
            s_dataStore = s_dataStore ?? new();
            _mediator = mediator;
        }

        public IEnumerable<OrderItem> GetItemsOfOrder(string id)
        {
            return Get(id).OrderItems;
        }

        public IEnumerable<Order> GetOrders()
        {
            return s_dataStore.Select(x => x.Value);
        }

        public void Clear()
        {
            s_dataStore.Clear();
        }

        public void Add(string id, Order t)
        {
            s_dataStore.Add(id, t);
        }

        public bool Exist(string id)
        {
            throw new NotImplementedException();
        }

        public Order Get(string id)
        {
            return s_dataStore[id];
        }

        public async Task DispatchDomainEvent(Order order)
        {
            await _mediator.DispatchDomainEventsAsync(order);
        }

        public Order GetT(string id)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string orderId)
        {
            throw new NotImplementedException();
        }
    }
}
