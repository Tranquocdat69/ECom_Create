namespace FPTS.FIT.BDRD.Services.Ordering.Infrastructure
{
    public class DataSeedFactory
    {
        public async Task CreateSeed(OrderDbContext context, ILogger<DataSeedFactory> logger, IInMemoryOrderStore store)
        {
            var policy = CreatePolicy(logger, nameof(DataSeedFactory));
            await policy.ExecuteAsync(async () => {
                // Thêm data seed ở đây
                // Order hiện tại ko cần
                if (!context.Orders.Any())
                {
                    Random random = new();
                    for (int i = 0; i < 10; i++)
                    {
                        var address = new Address(i + " - Street", i + " - City");
                        // Tạo Order
                        var order = new Order((i % 2 == 0) ? 1 : 2, address);
                        // Thêm item vào order
                        var itemCount = random.Next(2, 5);
                        for (int j = 0; j < itemCount; j++)
                        {
                            order.AddOrderItem(j, i + " - Proname", (j + 1) * 1000, 0, "", random.Next(5, 10));
                        }
                        order.ClearDomainEvents();
                        context.Orders.Add(order);
                    }
                    await context.SaveChangesAsync();
                }
                AddDataToCache(context.Orders, store);
            });
        }

        private void AddDataToCache(IEnumerable<Order> orders, IInMemoryOrderStore store)
        {
            foreach(var order in orders)
            {
                store.Add(order.Id,order);
            }
        }

        private AsyncRetryPolicy CreatePolicy(ILogger<DataSeedFactory> logger, string prefix, int retries = 3)
        {
            return Policy.Handle<Exception>().
                WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", prefix, exception.GetType().Name, exception.Message, retry, retries);
                    }
                );
        }
    }
}
