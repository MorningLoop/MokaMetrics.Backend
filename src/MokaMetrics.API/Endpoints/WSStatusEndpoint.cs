using Confluent.Kafka;
using Microsoft.AspNetCore.Http.HttpResults;
using MokaMetrics.Kafka.Abstractions;
using System.Net.WebSockets;
using System.Text;

namespace MokaMetrics.API.Endpoints
{
    public static class WSStatusEndpoint
    {

        public static IEndpointRouteBuilder MapWSStatusEndPoints(this IEndpointRouteBuilder builder)
        {
            var group = builder.MapGroup("/ws/status")
                .WithTags("WSStatus");
            group.Map("/", WSStatusMachine)
               .WithName("WSStatus");


            return builder;
        }


        private static async Task<Results<Ok, BadRequest<string>>> WSStatusMachine(HttpContext _httpContext, IKafkaProducer _kafkaProducer)
        {
            if (!_httpContext.WebSockets.IsWebSocketRequest)
            {
                return TypedResults.BadRequest("This connection is not a WebSocket!");
            }

            try
            {
                var webSocket = await _httpContext.WebSockets.AcceptWebSocketAsync();
                var cancellationToken = _httpContext.RequestAborted;

                Console.WriteLine("WebSocket connesso");

                // Invia immediatamente l'ultimo messaggio disponibile
                try
                {
                    //string? lastMessage = await _kafkaProducer.GetLastAvailableMessageAsync();
                    //if (!string.IsNullOrEmpty(lastMessage))
                    //{
                    //    Console.WriteLine($"Invio ultimo messaggio disponibile: {lastMessage}");
                    //    var bytes = Encoding.UTF8.GetBytes(lastMessage);
                    //    await webSocket.SendAsync(
                    //        bytes,
                    //        WebSocketMessageType.Text,
                    //        endOfMessage: true,
                    //        cancellationToken
                    //    );
                    //}
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Errore nell'invio dell'ultimo messaggio: {ex.Message}");
                }


                while (webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
                {
                    //try
                    //{

                    //    string? message = await _kafkaProducer.GetLatestMessageAsync(cancellationToken);

                    //    if (!string.IsNullOrEmpty(message))
                    //    {
                    //        Console.WriteLine($"Nuovo messaggio ricevuto da Kafka: {message}");

                    //        var bytes = Encoding.UTF8.GetBytes(message);
                    //        await webSocket.SendAsync(
                    //            bytes,
                    //            WebSocketMessageType.Text,
                    //            endOfMessage: true,
                    //            cancellationToken
                    //        );
                    //    }

                    //    await Task.Delay(100, cancellationToken);
                    //}
                    //catch (OperationCanceledException)
                    //{
                    //    Console.WriteLine("WebSocket operazione cancellata");
                    //    break;
                    //}
                    //catch (WebSocketException wsEx)
                    //{
                    //    Console.WriteLine($"WebSocket errore: {wsEx.Message}");
                    //    break;
                    //}
                }

                // Chiudi la connessione WebSocket se non è già chiusa
                if (webSocket.State == WebSocketState.Open)
                {
                    await webSocket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "Connessione chiusa",
                        CancellationToken.None
                    );
                }

                return TypedResults.Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore WebSocket: {ex.Message}");
                return TypedResults.BadRequest($"Errore WebSocket: {ex.Message}");
            }
        }


    }
}
