using Microsoft.Extensions.Diagnostics.HealthChecks;
using MokaMetrics.DataAccess.Abstractions.Influx;

namespace MokaMetrics.API.HealthChecks;

public class InfluxDb3HealthCheck : IHealthCheck
{
    private readonly IInfluxDb3Service _influxDb3Service;

    public InfluxDb3HealthCheck(IInfluxDb3Service influxDb3Service)
    {
        _influxDb3Service = influxDb3Service;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var isHealthy = await _influxDb3Service.HealthCheckAsync(cancellationToken);
            return isHealthy
                ? HealthCheckResult.Healthy("InfluxDB 3.0 is responsive")
                : HealthCheckResult.Unhealthy("InfluxDB 3.0 is not responsive");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("InfluxDB 3.0 health check failed", ex);
        }
    }
}
