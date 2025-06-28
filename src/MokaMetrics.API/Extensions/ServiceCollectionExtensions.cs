using MokaMetrics.DataAccess;
using MokaMetrics.DataAccess.Abstractions;
using MokaMetrics.DataAccess.Abstractions.Influx;
using MokaMetrics.DataAccess.Abstractions.Repositories;
using MokaMetrics.DataAccess.Influx;
using MokaMetrics.DataAccess.Influx.Settings;
using MokaMetrics.DataAccess.Repositories;
using MokaMetrics.Kafka.Abstractions;
using MokaMetrics.Kafka.MessageParsers;
using MokaMetrics.Kafka.MessageParsers.Base;

namespace MokaMetrics.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUnitOfWork(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IIndustrialFacilityRepository, IndustrialFacilityRepository>();
        services.AddScoped<ILotRepository, LotRepository>();
        services.AddScoped<IMachineActivityStatusRepository, MachineActivityStatusRepository>();
        services.AddScoped<IMachineRepository, MachineRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        
        return services;
    }
    public static IServiceCollection AddInfluxDb3(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection("InfluxDb3").Get<InfluxDb3Settings>()
            ?? throw new InvalidOperationException("InfluxDb3 configuration is missing");
        services.AddSingleton(settings);
        services.AddSingleton<IInfluxDb3Service, InfluxDb3Service>();

        return services;
    }

    public static IServiceCollection AddInfluxDb3(this IServiceCollection services, Action<InfluxDb3Settings> configureSettings)
    {
        var settings = new InfluxDb3Settings();
        configureSettings(settings);

        services.AddSingleton(settings);
        services.AddSingleton<IInfluxDb3Service, InfluxDb3Service>();

        return services;
    }

    public static IServiceCollection AddMessageParsers(this IServiceCollection services)
    {
        services.AddTransient<IMessageParser, CncMessageParser>();
        services.AddTransient<IMessageParser, LatheMessageParser>();
        services.AddTransient<IMessageParser, AssemblyMessageParser>();
        services.AddTransient<IMessageParser, TestingMessageParser>();
        services.AddTransient<IMessageParser, LotCompletionMessageParser>();
        services.AddTransient<IMessageParser, NewOrderLotMessageParser>();
        services.AddSingleton<MessageParserFactory>();
        return services;
    }
}
