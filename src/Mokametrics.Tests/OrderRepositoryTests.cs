using Mokametrics.Tests.Mocks;
using MokaMetrics.DataAccess.Abstractions;
using MokaMetrics.DataAccess.Contexts;
using MokaMetrics.Models.Entities;
using Moq;

namespace Mokametrics.Tests
{
    public class OrderRepositoryTests
    {
        private readonly Mock<IUnitOfWork> _contextMock;

        public OrderRepositoryTests()
        {
            _contextMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
        }

        [Theory]
        [OrdersMockList]
        [Trait("Orders", "Unit")]
        public async Task GetAllOrdersAsync_should_return_all_order_list(List<Order> ordersMock)
        {            
            _contextMock.Setup(x => x.Orders.GetAllAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(ordersMock);
            
            var orderRepository = _contextMock.Object.Orders;

            // Act
            var result = await orderRepository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ordersMock.Count, result.Count);
            Assert.Equal(ordersMock, result);
            _contextMock.Verify(x => x.Orders.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        [Trait("Orders", "Unit")]
        public async Task GetOrderWithLotsAsync_should_throw_when_order_not_found()
        {
            // Arrange
            int nonExistentOrderId = 9999;
            _contextMock.Setup(x => x.Orders.GetOrderWithLotsAsync(nonExistentOrderId))
                       .ThrowsAsync(new KeyNotFoundException($"Order with ID {nonExistentOrderId} not found"));
            
            var orderRepository = _contextMock.Object.Orders;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
                async () => await orderRepository.GetOrderWithLotsAsync(nonExistentOrderId));
            
            Assert.Contains($"Order with ID {nonExistentOrderId} not found", exception.Message);
            _contextMock.Verify(x => x.Orders.GetOrderWithLotsAsync(nonExistentOrderId), Times.Once);
        }
    }
}
