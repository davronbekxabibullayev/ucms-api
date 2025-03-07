namespace Ucms.Stock.Infrastructure.EntityFramework;

using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using Ucms.Stock.Domain.Models;
using Ucms.Stock.Infrastructure.Persistance;

public class StockDbContext : DbContext, IStockDbContext
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public StockDbContext(DbContextOptions<StockDbContext> options) : base(options)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        NpgsqlConnection.GlobalTypeMapper.EnableDynamicJson();
    }

    public DbSet<Income> Incomes { get; set; }
    public DbSet<IncomeItem> IncomeItems { get; set; }
    public DbSet<MeasurementUnit> MeasurementUnits { get; set; }
    public DbSet<Outcome> Outcomes { get; set; }
    public DbSet<OutcomeItem> OutcomeItems { get; set; }
    public DbSet<Sku> Skus { get; set; }
    public DbSet<Stock> Stocks { get; set; }
    public DbSet<StockDemand> StockDemands { get; set; }
    public DbSet<StockDemandItem> StockDemandItems { get; set; }
    public DbSet<StockSku> StockSkus { set; get; }
    public DbSet<Manufacturer> Manufacturers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<IncomeOutcome> IncomeOutcomes { get; set; }
    public DbSet<OrganizationSku> OrganizationSkus { get; set; }
    public DbSet<StockBalanceRegister> StockBalanceRegistry { get; set; }
    public DbSet<OrganizationMeasurementUnit> OrganizationMeasurementUnits { get; set; }

    public IExecutionStrategy CreateExecutionStrategy()
    {
        return Database.CreateExecutionStrategy();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return await Database.BeginTransactionAsync(cancellationToken);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}
