using Microsoft.EntityFrameworkCore;
using MokaMetrics.DataAccess.Abstractions.Contexts;
using MokaMetrics.Models;
using MokaMetrics.Models.Entities;

namespace MokaMetrics.DataAccess.Contexts;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<IndustrialFacility> IndustrialFacilities { get; set; }
    public DbSet<Lot> Lots { get; set; }
    public DbSet<Machine> Machines { get; set; }
    public DbSet<MachineActivityStatus> MachineActivityStatuses { get; set; }
    public DbSet<Order> Orders { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        // cycling all entities flagged for creation/update and automatically setting updatedAt timestamp
        foreach (var item in ChangeTracker.Entries<Entity>().AsEnumerable())
            item.Entity.UpdatedAt = DateTime.UtcNow;

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(e =>
        {
            e.HasMany(x => x.Orders)
                .WithOne(x => x.Customer)
                .HasForeignKey(x => x.CustomerId);

            e.Property(p => p.Name)
                .HasColumnType("varchar").HasMaxLength(255)
                .IsRequired();

            e.Property(p => p.Email).HasColumnType("varchar").HasMaxLength(255).IsRequired();
            e.Property(p => p.Address).HasColumnType("varchar").HasMaxLength(255).IsRequired(false);
            e.Property(p => p.Country).HasColumnType("varchar").HasMaxLength(255).IsRequired(false);
            e.Property(p => p.ZipCode).HasColumnType("varchar").HasMaxLength(20).IsRequired(false);
            e.Property(p => p.City).HasColumnType("varchar").HasMaxLength(255).IsRequired(false);
            e.Property(p => p.Phone).HasColumnType("varchar").HasMaxLength(20).IsRequired(false);
            e.Property(p => p.FiscalId).HasColumnType("varchar").HasMaxLength(50).IsRequired();
        });

        modelBuilder.Entity<Order>(e =>
        {
            e.HasMany(x => x.Lots)
                .WithOne(x => x.Order)
                .HasForeignKey(x => x.OrderId);

            e.HasOne(x => x.Customer)
                .WithMany(x => x.Orders)
                .HasForeignKey(x => x.CustomerId);

            e.Property(p => p.QuantityMachines).IsRequired();
            e.Property(p => p.OrderDate).HasColumnType("timestamp").IsRequired();
            e.Property(p => p.Deadline).HasColumnType("timestamp").IsRequired(false);
            e.Property(p => p.FullfilledDate).HasColumnType("timestamp").IsRequired(false);
        });

        modelBuilder.Entity<Lot>(e =>
        {
            e.HasOne(x => x.Order)
                .WithMany(x => x.Lots)
                .HasForeignKey(x => x.OrderId);

            e.HasOne(x => x.IndustrialFacility)
                .WithMany(x => x.Lots)
                .HasForeignKey(x => x.IndustrialFacilityId);

            e.Property(p => p.TotalQuantity).IsRequired();
            e.Property(p => p.ManufacturedQuantity).IsRequired();
            e.Property(p => p.OrderId).IsRequired();
            e.Property(p => p.LotCode).HasColumnType("varchar").HasMaxLength(50).IsRequired();
            e.Property(p => p.StartDate).HasColumnType("timestamp").IsRequired();
            e.Property(p => p.EndDate).HasColumnType("timestamp").IsRequired(false);
        });

        modelBuilder.Entity<IndustrialFacility>(e =>
        {
            e.HasMany(x => x.Lots)
                .WithOne(x => x.IndustrialFacility)
                .HasForeignKey(x => x.IndustrialFacilityId);

            e.Property(p => p.Name).HasColumnType("varchar").HasMaxLength(255).IsRequired();
            e.Property(p => p.Address).HasColumnType("varchar").HasMaxLength(255).IsRequired(false);
            e.Property(p => p.Country).HasColumnType("varchar").HasMaxLength(255).IsRequired(false);
            e.Property(p => p.ZipCode).HasColumnType("varchar").HasMaxLength(20).IsRequired(false);
            e.Property(p => p.City).HasColumnType("varchar").HasMaxLength(255).IsRequired(false);
            e.Property(p => p.Phone).HasColumnType("varchar").HasMaxLength(20).IsRequired(false);
        });

        modelBuilder.Entity<Machine>(e =>
        {
            e.HasOne(x => x.IndustrialFacility)
                .WithMany()
                .HasForeignKey(x => x.IndustrialFacilityId);

            e.Property(p => p.IndustrialFacilityId).IsRequired();

            e.Property(p => p.Code).HasColumnType("varchar").HasMaxLength(255).IsRequired();
            e.Property(p => p.Model).HasColumnType("varchar").HasMaxLength(255).IsRequired();
            e.Property(p => p.Status).IsRequired();
        });

        modelBuilder.Entity<MachineActivityStatus>(e =>
        {
            e.HasOne(x => x.Machine)
                .WithMany()
                .HasForeignKey(x => x.MachineId);

            e.Property(p => p.Date).HasColumnType("timestamp").IsRequired();
            e.Property(p => p.Status).IsRequired();
            e.Property(p => p.ErrorMessage).HasColumnType("varchar").HasMaxLength(255).IsRequired(false);
        });
    }
}