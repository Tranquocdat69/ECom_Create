using FPTS.FIT.BDRD.Services.Ordering.App.Application.IntegrationEvents;

namespace ECom.Services.Ordering.App.Application.DomainEventHandlers.OrderConfirmed
{
    public class PersistentWhenOrderConfirmedDomainEventHandler : IDomainEventHandler<OrderConfirmedDomainEvent>
    {
        private readonly IOrderRepository _orderRepository;

        public PersistentWhenOrderConfirmedDomainEventHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task Handle(OrderConfirmedDomainEvent eventData, CancellationToken cancellationToken)
        {
            _orderRepository.Add(eventData.Order);
            await _orderRepository.UnitOfWork.SaveChangesAsync();
        }
    }
}
