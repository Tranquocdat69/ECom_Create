namespace FPTS.FIT.BDRD.Services.Ordering.App.Application.IntegrationEvents
#nullable disable
{
    public class UpdateProductAvaibleStockIntegrationEvent : IIntegrationEvent
    {
        public UpdateProductAvaibleStockIntegrationEvent(IDictionary<int, int> items, string replyAddress)
        {
            OrderItems = items;
            ReplyAddress = replyAddress;
        }

        public IDictionary<int, int> OrderItems { get;}
        public string ReplyAddress { get; }
    }
}
