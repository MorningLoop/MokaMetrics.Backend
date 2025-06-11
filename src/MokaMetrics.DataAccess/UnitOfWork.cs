using Microsoft.Extensions.DependencyInjection;
using MokaMetrics.DataAccess.Abstractions;
using MokaMetrics.DataAccess.Abstractions.Contexts;
using MokaMetrics.DataAccess.Abstractions.Repositories;

namespace MokaMetrics.DataAccess;

public class UnitOfWork : IUnitOfWork
{
    private readonly IApplicationDbContext _context;
    private readonly IServiceProvider _serviceProvider;
    public UnitOfWork(IApplicationDbContext context, IServiceProvider serviceProvider)
    {
        _context = context;
        _serviceProvider = serviceProvider;
    }

    private ICustomerRepository? _customerRepository;
    private IIndustrialFacilityRepository? _industrialFacilityRepository;
    private ILotRepository? _lotRepository;
    private IMachineActivityStatusRepository? _machineActivityStatusRepository;
    private IMachineRepository? _machineRepository;
    private IOrderRepository? _orderRepository;
    public ICustomerRepository Customers => _customerRepository ??= _serviceProvider.GetRequiredService<ICustomerRepository>();
    public IIndustrialFacilityRepository IndustrialFacilities => _industrialFacilityRepository ??= _serviceProvider.GetRequiredService<IIndustrialFacilityRepository>();
    public ILotRepository Lots => _lotRepository ??= _serviceProvider.GetRequiredService<ILotRepository>();
    public IMachineActivityStatusRepository MachineActivityStatuses => _machineActivityStatusRepository ??= _serviceProvider.GetRequiredService<IMachineActivityStatusRepository>();
    public IMachineRepository Machines => _machineRepository ??= _serviceProvider.GetRequiredService<IMachineRepository>();
    public IOrderRepository Orders => _orderRepository ??= _serviceProvider.GetRequiredService<IOrderRepository>();

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
