using FPTS.FIT.BDRD.BuildingBlocks.EventBusKafka;
using FPTS.FIT.BDRD.BuildingBlocks.EventBusKafka.Configurations;
using System.Reflection;

namespace FPTS.FIT.BDRD.Services.Ordering.App.Extensions
#nullable disable
{
    public static class ServiceCollectionExtensions
    {
        private const string c_dbConnectionKey = "OrderDB";
        public static IServiceCollection UseServiceCollectionConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddMediatorConfiguration()
                .AddKafkaConfiguration(configuration)
                .AddLoggerConfiguration(configuration)
                .AddPersistanceConfiguration(configuration)
                .AddScopeService();
        }

        private static IServiceCollection AddLoggerConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddLogging(options =>
            {
                var allowConsoleLog = configuration.GetValue("AllowConsoleLog", false);
                if (!allowConsoleLog)
                {
                    options.ClearProviders();
                }
                options.AddKafkaLogger(config =>
                {
                    var loggerConfiguration = configuration.GetSection("KafkaLogger").Get<LoggerKafkaConfiguration>();
                    config.BootstrapServers = loggerConfiguration.BootstrapServers;
                    config.Targets = loggerConfiguration.Targets;
                    config.Rules = loggerConfiguration.Rules;
                    config.AppName = loggerConfiguration.AppName;
                });
            });
            return services;
        }
        private static IServiceCollection AddPersistanceConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var dbConnectionString = configuration.GetConnectionString(c_dbConnectionKey);
            services.AddDbContext<OrderDbContext>((options) =>
            {
                if (!String.IsNullOrEmpty(dbConnectionString))
                {
                    options.UseSqlServer(dbConnectionString);
                }
            });
            return services;
        }
        private static IServiceCollection AddMediatorConfiguration(this IServiceCollection services)
        {
            services
                .AddMediatR(typeof(CreateOrderCommand));
            return services;
        }
        private static IServiceCollection AddKafkaConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var config = new ProducerBuilderConfiguration
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"]
            };
            services.AddSingleton<IRequestManager<string>, InMemoryRequestManager>();
            services.AddSingleton<IPublisher<ProducerData<Null, string>>, KafkaPublisher<Null, string>>(sp =>
            {
                return new KafkaPublisher<Null, string>(config);
            });

            services.AddSingleton<IPublisher<ProducerData<string, string>>, KafkaPublisher<string, string>>(sp =>
            {
                return new KafkaPublisher<string, string>(config);
            });
            return services;
        }
        private static IServiceCollection AddScopeService(this IServiceCollection services)
        {
            services.AddScoped<IInMemoryOrderStore, InMemoryOrderStore>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            return services;
        }
    }
}
