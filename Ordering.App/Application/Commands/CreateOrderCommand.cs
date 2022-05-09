
namespace FPTS.FIT.BDRD.Services.Ordering.App.Application.Commands
#nullable disable
{
    public class CreateOrderCommand : IRequest<OrderDTO>
    {
        public int UserId { get; set; }
        public string City { get; set; }
        public string Street { get; set; }

        private readonly IEnumerable<OrderItemDTO> _orderItems;
        public IEnumerable<OrderItemDTO> OrderItems => _orderItems;

        public CreateOrderCommand(IEnumerable<OrderItemDTO> orderItems, int userId, string city, string street)
        {
            _orderItems = orderItems;
            UserId = userId;
            City = city;
            Street = street;
        }
    }
}
