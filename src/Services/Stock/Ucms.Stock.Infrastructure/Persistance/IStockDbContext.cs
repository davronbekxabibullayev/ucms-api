namespace Ucms.Stock.Infrastructure.Persistance;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Ucms.Stock.Domain.Models;

public interface IStockDbContext
{
    DbSet<Income> Incomes { get; set; }
    DbSet<IncomeItem> IncomeItems { get; set; }
    DbSet<MeasurementUnit> MeasurementUnits { get; set; }
    DbSet<Outcome> Outcomes { get; set; }
    DbSet<OutcomeItem> OutcomeItems { get; set; }
    DbSet<Sku> Skus { get; set; }
    DbSet<Stock> Stocks { get; set; }
    DbSet<StockDemand> StockDemands { get; set; }
    DbSet<StockDemandItem> StockDemandItems { get; set; }
    DbSet<StockSku> StockSkus { get; set; }
    DbSet<Manufacturer> Manufacturers { get; set; }
    DbSet<Product> Products { get; set; }
    DbSet<Supplier> Suppliers { get; set; }
    DbSet<IncomeOutcome> IncomeOutcomes { get; set; }
    DbSet<OrganizationSku> OrganizationSkus { get; set; }
    DbSet<StockBalanceRegister> StockBalanceRegistry { get; set; }
    DbSet<OrganizationMeasurementUnit> OrganizationMeasurementUnits { get; set; }
    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
    IExecutionStrategy CreateExecutionStrategy();
    DatabaseFacade Database { get; }
}
