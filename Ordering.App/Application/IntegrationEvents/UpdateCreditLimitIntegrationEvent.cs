namespace FPTS.FIT.BDRD.Services.Ordering.App.Application.IntegrationEvents
#nullable disable
{
    public class UpdateCreditLimitIntegrationEvent : IIntegrationEvent
    {
        public UpdateCreditLimitIntegrationEvent(int userId, decimal totalCost, string replyAddress)
        {
            UserId = userId;
            TotalCost = totalCost;
            ReplyAddress = replyAddress;
        }

        public int UserId { get; }
        public decimal TotalCost { get; }
        public string ReplyAddress { get; }

    }
}
