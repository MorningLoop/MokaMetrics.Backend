using MokaMetrics.DataAccess.Abstractions.Influx;
using MokaMetrics.DataAccess.Influx;
using MokaMetrics.DataAccess.Influx.Settings;

namespace MokaMetrics.API.Extensions;

public static class ServiceCollectionExtensions
{
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
}
