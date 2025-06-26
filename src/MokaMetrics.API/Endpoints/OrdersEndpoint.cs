using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MokaMetrics.DataAccess.Abstractions;
using MokaMetrics.Kafka.Abstractions;
using MokaMetrics.Models.DTO;
using MokaMetrics.Models.Entities;
using MokaMetrics.Models.Kafka.Messages;
using MokaMetrics.Models.Mappers;

namespace MokaMetrics.API.Endpoints;

public static class OrdersEndpoint
{
    public static IEndpointRouteBuilder MapOrdersEndPoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/orders")
            .WithTags("Orders");

        group.MapGet("/", GetAllOrdersAsync)
           .WithName("GetAllOrders");
        group.MapGet("/{id:int}", GetOrderByIdAsync)
            .WithName("GetOrderById");
        group.MapPost("/", CreateOrderAsync)
            .WithName("CreateOrder");
        group.MapPut("/{id:int}", UpdateOrderAsync)
            .WithName("UpdateOrder");
        group.MapDelete("/{id:int}", DeleteOrderAsync)
            .WithName("DeleteOrder");
        return builder;
    }

    private static async Task<Ok<IEnumerable<Order>>> GetAllOrdersAsync(IUnitOfWork _uow)
    {
        IEnumerable<Order> orders = await _uow.Orders.GetAllAsync();
        return TypedResults.Ok(orders ?? Array.Empty<Order>());
    }
    private static async Task<Results<Ok<Order>, NotFound>> GetOrderByIdAsync(IUnitOfWork _uow, int id)
    {
        var order = await _uow.Orders.GetById(id);
        if (order == null)
        {
            return TypedResults.NotFound();
        }
        return TypedResults.Ok(order);
    }
    private static async Task<Created> CreateOrderAsync(IUnitOfWork _uow, IKafkaProducer _kafkaProducer, [FromBody]OrderWithLotsCreateDto orderDto)
    {
        var order = OrderMapper.MapCreateOrder(orderDto);
        _uow.Orders.Add(order);
        await _uow.SaveChangesAsync();

        foreach (var lot in order.Lots)
        {
            var orderLotMessage = new NewOrderLotMessage()
            {
                LocalTimestamp = DateTime.UtcNow,
                UtcTimestamp = DateTime.UtcNow,
                LotCode = lot.LotCode,
                Site = (await _uow.IndustrialFacilities.GetById(lot.IndustrialFacilityId)).Country ?? "Italy",
                MachinesToProduce = lot.TotalQuantity
            };

            await _kafkaProducer.ProduceAsync("orders", "order", orderLotMessage);
        }

        return TypedResults.Created($"/api/v1/orders/");
    }
    private static async Task<Results<Ok, NotFound>> UpdateOrderAsync(IUnitOfWork _uow, int id, OrderDtoStrict categoryDto)
    {
        return TypedResults.Ok();
    }
    private static async Task<Results<NoContent, NotFound>> DeleteOrderAsync(IUnitOfWork _uow, int id)
    {
        var order = await _uow.Orders.GetById(id);
        if (order == null)
        {
            return TypedResults.NotFound();
        }

        _uow.Orders.Delete(order);
        await _uow.SaveChangesAsync();

        return TypedResults.NoContent();
    }
}
