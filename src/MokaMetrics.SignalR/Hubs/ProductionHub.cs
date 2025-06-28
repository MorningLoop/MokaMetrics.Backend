using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace MokaMetrics.SignalR.Hubs;

public class ProductionHub : Hub
{
    private readonly ILogger<ProductionHub> _logger;
    public ProductionHub(ILogger<ProductionHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}
