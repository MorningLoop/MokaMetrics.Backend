using MokaMetrics.Models.Entities;
using System.Reflection;
using Xunit.Sdk;

namespace Mokametrics.Tests.Mocks;

public class OrdersMockList : DataAttribute
{
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        yield return new object[]
        {
            new List<Order>
            {
                new Order
                {
                    Id = 1,
                    CustomerId = 1,
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    UpdatedAt = DateTime.UtcNow.AddDays(-10),
                    Lots = new List<Lot>
                    {
                        new Lot { Id = 1, OrderId = 1, CreatedAt = DateTime.UtcNow.AddDays(-10), UpdatedAt = DateTime.UtcNow.AddDays(-10) }
                    }
                },
                new Order
                {
                    Id = 2,
                    CustomerId = 2,
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    UpdatedAt = DateTime.UtcNow.AddDays(-5),
                    Lots = new List<Lot>
                    {
                        new Lot { Id = 2, OrderId = 2, CreatedAt = DateTime.UtcNow.AddDays(-5), UpdatedAt = DateTime.UtcNow.AddDays(-5) }
                    }
                }
            }
        };
    }
}
