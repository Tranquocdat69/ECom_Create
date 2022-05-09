namespace FPTS.FIT.BDRD.Services.Ordering.Infrastructure
#nullable disable
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _context;
        private readonly ILogger<OrderRepository> _logger;

        public OrderRepository(OrderDbContext context, ILogger<OrderRepository> logger)
        {
            _context = context;
            _logger  = logger;
        }
        public IUnitOfWork UnitOfWork => _context;

        public void Add(Order order)
        {
            _context.Add(order);
        }

        public bool Delete(Order t)
        {
            try
            {
                _context.Orders.Remove(t);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }

            return true;
        }

        public IEnumerable<Order> GetAll()
        {
            return _context.Orders.AsEnumerable();
        }

        public IEnumerable<OrderItem> GetItems(string id)
        {
            return _context.Orders.Find(id)?.OrderItems;
        }

        public Order GetOrder(string id)
        {
            return _context.Orders.Find(id);
        }

        public void Update(Order order)
        {
            _context.Orders.Update(order);
        }
    }
}
