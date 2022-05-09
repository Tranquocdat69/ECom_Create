namespace FPTS.FIT.BDRD.Services.Ordering.Domain.AggregateModels.OrderAggregate;
#nullable disable
public class Order : BaseEntity, IAggregateRoot
{
    public new string Id { get; set; }
    public DateTime OrderDate { get; set; }
    public Address Address { get; private set; }
    public int CustomerId { get; set; }
    private readonly List<OrderItem> _orderItems;
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;

    public Order(int cusId, Address add) : this()
    {
        CustomerId = cusId;
        Address = add;
        OrderDate = DateTime.Now;
    }

    protected Order()
    {
        Id = DateTime.Now.ToString("yyyyMMddHHmmssffffff");
        _orderItems = new List<OrderItem>();
    }

    // DDD Patterns comment
    // This Order AggregateRoot's method "AddOrderitem()" should be the only way to add Items to the Order,
    // so any behavior (discounts, etc.) and validations are controlled by the AggregateRoot 
    // in order to maintain consistency between the whole Aggregate. 
    public void AddOrderItem(int productId, string productName, decimal unitPrice, decimal discount, string pictureUrl, int units = 1)
    {
        var existingOrderForProduct = _orderItems.Where(o => o.ProductId == productId)
            .SingleOrDefault();

        if (existingOrderForProduct != null)
        {
            if (discount > existingOrderForProduct.GetCurrentDiscount())
            {
                existingOrderForProduct.SetNewDiscount(discount);
            }
            existingOrderForProduct.AddUnits(units);
        }
        else
        {
            var orderItem = new OrderItem(productId, productName, unitPrice, discount, pictureUrl, units);
            _orderItems.Add(orderItem);
        }
    }

    public void SetOrderConfirmed()
    {
        var @event = new OrderConfirmedDomainEvent(this);
        AddDomainEvent(@event);
    }

    public decimal GetTotal()
    {
        return _orderItems.Sum(o => o.GetUnits() * o.GetUnitPrice());
    }

    public override string ToString()
    {
        var orderItems = string.Join(",",_orderItems.Select(x => x.ToString()));
        return "{\"OrderDate\":\""+OrderDate.ToString()+"\",\"Address\":"+Address.ToString()+",\"CustomerId\":"+CustomerId+",\"OrderItems\":["+orderItems+"]}";
    }
}
