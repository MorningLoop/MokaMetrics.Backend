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

            // Endpoint per testare Kafka e ottenere l'ultimo messaggio
            group.MapGet("/test", TestKafkaConnection)
                .WithName("TestKafkaConnection");

            // Endpoint per ottenere tutti i messaggi disponibili
            group.MapGet("/all", GetAllMessages)
                .WithName("GetAllKafkaMessages");

            return builder;
        }

        
        private static async Task<Results<Ok, BadRequest<string>>> WSStatusMachine(HttpContext _httpContext, IKafkaService _kafkaService)
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
                    string? lastMessage = await _kafkaService.GetLastAvailableMessageAsync();
                    if (!string.IsNullOrEmpty(lastMessage))
                    {
                        Console.WriteLine($"Invio ultimo messaggio disponibile: {lastMessage}");
                        var bytes = Encoding.UTF8.GetBytes(lastMessage);
                        await webSocket.SendAsync(
                            bytes,
                            WebSocketMessageType.Text,
                            endOfMessage: true,
                            cancellationToken
                        );
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Errore nell'invio dell'ultimo messaggio: {ex.Message}");
                }

                // Loop principale per inviare nuovi messaggi
                while (webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        // Ottieni il messaggio da Kafka con timeout
                        string? message = await _kafkaService.GetLatestMessageAsync(cancellationToken);
                        
                        if (!string.IsNullOrEmpty(message))
                        {
                            Console.WriteLine($"Nuovo messaggio ricevuto da Kafka: {message}");
                            
                            var bytes = Encoding.UTF8.GetBytes(message);
                            await webSocket.SendAsync(
                                bytes,
                                WebSocketMessageType.Text,
                                endOfMessage: true,
                                cancellationToken
                            );
                        }
                        
                        // Aggiungi un piccolo delay per evitare consumo eccessivo di CPU
                        await Task.Delay(100, cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        Console.WriteLine("WebSocket operazione cancellata");
                        break;
                    }
                    catch (WebSocketException wsEx)
                    {
                        Console.WriteLine($"WebSocket errore: {wsEx.Message}");
                        break;
                    }
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

        private static async Task<Results<Ok<string>, BadRequest<string>>> TestKafkaConnection(IKafkaService kafkaService)
        {
            try
            {
                Console.WriteLine("Testing Kafka connection...");
                
                // Test per ottenere l'ultimo messaggio disponibile
                string? lastMessage = await kafkaService.GetLastAvailableMessageAsync();
                
                if (!string.IsNullOrEmpty(lastMessage))
                {
                    return TypedResults.Ok($"Kafka connesso. Ultimo messaggio: {lastMessage}");
                }
                
                // Test per consumare un nuovo messaggio
                string? newMessage = await kafkaService.GetLatestMessageAsync(CancellationToken.None);
                
                if (!string.IsNullOrEmpty(newMessage))
                {
                    return TypedResults.Ok($"Kafka connesso. Nuovo messaggio: {newMessage}");
                }
                
                return TypedResults.Ok("Kafka connesso, ma nessun messaggio disponibile al momento.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore test Kafka: {ex.Message}");
                return TypedResults.BadRequest($"Errore connessione Kafka: {ex.Message}");
            }
        }

        private static async Task<Results<Ok<List<string>>, BadRequest<string>>> GetAllMessages(IKafkaService kafkaService, int maxMessages = 50)
        {
            try
            {
                Console.WriteLine($"Recupero di tutti i messaggi disponibili (max: {maxMessages})...");
                
                var messages = await kafkaService.GetAllAvailableMessagesAsync(maxMessages);
                
                if (messages.Any())
                {
                    return TypedResults.Ok(messages);
                }
                
                return TypedResults.Ok(new List<string> { "Nessun messaggio disponibile nel topic" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore recupero messaggi: {ex.Message}");
                return TypedResults.BadRequest($"Errore recupero messaggi: {ex.Message}");
            }
        }
    }
}
