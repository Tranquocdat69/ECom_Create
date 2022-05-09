using FPTS.FIT.BDRD.Services.Ordering.App.Application.IntegrationEvents;
using System.Text.Json;

namespace FPTS.FIT.BDRD.Services.Ordering.App.Application.Commands
#nullable disable
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDTO>
    {
        private readonly IInMemoryOrderStore _orderStore;
        private readonly IPublisher<ProducerData<string, string>> _producer;
        private readonly string _balanceTopic;
        private readonly string _catalogTopic;
        private const int PartitionIdBalanceTopic = 0;
        private const int PartitionIdCatalogTopic = 0;
        private const string KeyCommand = "command";
        private const string Delimiter = " - ";
        private Dictionary<string, string> _suffixRequestId = new()
        {
            {"BalanceSuffix", "balance"},
            {"CatalogSuffix", "catalog"}
        };
        private readonly string _replyAddress;

        public CreateOrderCommandHandler(
            IInMemoryOrderStore orderStore,
            IPublisher<ProducerData<string, string>> producer,
            IConfiguration configuration)
        {
            _orderStore = orderStore;
            _producer = producer;
            _balanceTopic = configuration.GetSection("Kafka").GetSection("CommandTopic").GetSection("Balance").Value;
            _catalogTopic = configuration.GetSection("Kafka").GetSection("CommandTopic").GetSection("Catalog").Value;
            _replyAddress = configuration.GetSection("ExternalAddress").Value;
        }

        public async Task<OrderDTO> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await AddOrderIntoMemory(request);
            var dictionarySerializedIntegrationEvent = AddToDictionarySerializedIntegrationEvent(order);

            var balanceRequestId = GetGenaratedRequestId("BalanceSuffix");
            var catalogRequestId = GetGenaratedRequestId("CatalogSuffix");

            PublishIntegrationEvent(dictionarySerializedIntegrationEvent["Balance"], balanceRequestId, _balanceTopic, PartitionIdBalanceTopic);
            PublishIntegrationEvent(dictionarySerializedIntegrationEvent["Catalog"], catalogRequestId, _catalogTopic, PartitionIdCatalogTopic);

            return new OrderDTO(order.Address.City, order.Address.Street, order.OrderDate, order.OrderItems.Count());
        }

        private async Task<Order> AddOrderIntoMemory(CreateOrderCommand request)
        {
            var address = new Address(request.Street, request.City);
            var order = new Order(request.UserId, address);

            foreach (var item in request.OrderItems)
            {
                order.AddOrderItem(item.ProductId, item.ProductName, item.UnitPrice, item.Discount, item.PictureUrl, item.Units);
            }

            order.SetOrderConfirmed();
            _orderStore.Add(order.Id, order);

            await _orderStore.DispatchDomainEvent(order);

            return order;
        }

        private Dictionary<string, string> AddToDictionarySerializedIntegrationEvent(Order order)
        {
            Dictionary<string, string> dictionarySerializedIntegrationEvent = new();
            var integrationBalanceEvent = new UpdateCreditLimitIntegrationEvent(order.CustomerId, order.GetTotal(), _replyAddress);
            var integrationCatalogEvent = new UpdateProductAvaibleStockIntegrationEvent(order.OrderItems.ToDictionary(x => x.ProductId, x => x.GetUnits()), _replyAddress);
            var serializeIntegrationBalanceEvent = JsonSerializer.Serialize(integrationBalanceEvent);
            var serializeIntegrationCatalogEvent = JsonSerializer.Serialize(integrationCatalogEvent);

            dictionarySerializedIntegrationEvent.Add("Balance", serializeIntegrationBalanceEvent);
            dictionarySerializedIntegrationEvent.Add("Catalog", serializeIntegrationCatalogEvent);

            return dictionarySerializedIntegrationEvent;
        }

        private string SerializeIntegrationEvent(IIntegrationEvent integrationEvent)
        {
            string result = JsonSerializer.Serialize(integrationEvent);
            return result;
        }

        private string GetGenaratedRequestId(string key)
        {
            return KeyCommand + Guid.NewGuid().ToString() + Delimiter + _suffixRequestId[key];
        }

        private void PublishIntegrationEvent(string value, string key, string topic, int partitionId)
        {
            ProducerData<string, string> data = new ProducerData<string, string>(
                value: value,
                key: key,
                topic: topic,
                partitionId);
            _producer.Publish(data);
        }
    }
}
