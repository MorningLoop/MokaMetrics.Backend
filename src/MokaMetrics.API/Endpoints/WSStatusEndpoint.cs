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
            group.Map("/", WSStatus)
               .WithName("WSStatus");

            return builder;
        }

        // Metodo per gestire il WebSocket
        private static async Task<Results<Ok, BadRequest<string>>> WSStatus(HttpContext _httpContext, IKafkaService _kafkaService)
        {
            if (_httpContext.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await _httpContext.WebSockets.AcceptWebSocketAsync();
                var buffer = new byte[8];
                //mettere controllo jvt (non lo facciamo tranq)
                _kafkaService.GetValueTopic();


                await webSocket.SendAsync(
                    buffer,
                    WebSocketMessageType.Text,
                    endOfMessage: true,
                    CancellationToken.None
                );




            }

            return TypedResults.BadRequest("This connection not is a websocket!");            
        }
    }
}
