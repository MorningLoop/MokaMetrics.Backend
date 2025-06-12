using MokaMetrics.DataAccess.Abstractions.Repositories;

namespace MokaMetrics.DataAccess.Abstractions;

public interface IUnitOfWork
{
    ICustomerRepository Customers { get; }
    IIndustrialFacilityRepository IndustrialFacilities { get; }
    ILotRepository Lots { get; }
    IMachineActivityStatusRepository MachineActivityStatuses { get; }
    IMachineRepository Machines { get; }
    IOrderRepository Orders { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
