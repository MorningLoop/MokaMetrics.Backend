using Confluent.Kafka;
using Microsoft.AspNetCore.Http.HttpResults;
using MokaMetrics.Services.ServicesInterfaces;
using System.Net.WebSockets;
using System.Text;

namespace MokaMetrics.API.Endpoints
{
    public static class WSStatusEndpoint
    {
        // Aggiungi un gruppo di WebSocket
        public static IEndpointRouteBuilder MapWSStatusEndPoints(this IEndpointRouteBuilder builder)
        {
            var group = builder.MapGroup("/ws/status")
                .WithTags("WSStatus");

            // Endpoint di connessione WebSocket
            group.Map("/", WSStatusMachine)
               .WithName("WSStatus");

            return builder;
        }

        
        private static async Task<Results<Ok, BadRequest<string>>> WSStatusMachine(HttpContext _httpContext, IKafkaService _kafkaService)
        {
            try
            {
                if (_httpContext.WebSockets.IsWebSocketRequest)
                { 
                    var webSocket = await _httpContext.WebSockets.AcceptWebSocketAsync();
                    while (webSocket.State == WebSocketState.Open)
                    {
                    
                        Message<Ignore, string> message = _kafkaService.GetValueTopicBrasil();
                        if (message != null && !string.IsNullOrEmpty(message.Value))
                        {
                        
                            var bytes = Encoding.UTF8.GetBytes(message.Value);
                            await webSocket.SendAsync(
                                bytes,
                                WebSocketMessageType.Text,
                                endOfMessage: true,
                                CancellationToken.None
                            );

                        }

                        await Task.Delay(100);
                    
                    }
                   
                }

                return TypedResults.BadRequest("This connection not is a WebSocket!");
            }
            catch (Exception ex)
            {
                // Handle exceptions (connection closed, etc.)
                return TypedResults.BadRequest($"WebSocket error: {ex.Message}");
            }
        }
    }
}
