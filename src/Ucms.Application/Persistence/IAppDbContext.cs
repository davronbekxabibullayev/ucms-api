namespace Ucms.Application.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Ucms.Domain.Entities;
using Ucms.Domain.Entities.Identity;

public interface IUcmsDbContext
{
    // Tashkilot
    DbSet<Organization> Organizations { get; set; }

    // Loyiha va smeta
    DbSet<Project> Projects { get; set; }
    DbSet<EstimateSection> EstimateSections { get; set; }
    DbSet<EstimateItem> EstimateItems { get; set; }

    // Brigadalar
    DbSet<Brigade> Brigades { get; set; }

    // Bajarilgan ishlar
    DbSet<WorkLog> WorkLogs { get; set; }

    // Zakazchik akti va to'lovlar
    DbSet<ClientAct> ClientActs { get; set; }
    DbSet<ClientActItem> ClientActItems { get; set; }
    DbSet<ClientPayment> ClientPayments { get; set; }

    // Brigada to'lovlari
    DbSet<BrigadePayment> BrigadePayments { get; set; }

    // Loyiha xarajatlari
    DbSet<ProjectExpense> ProjectExpenses { get; set; }

    // Maoshlar
    DbSet<Salary> Salaries { get; set; }

    // O'lchov birliklari (spravochnik)
    DbSet<MeasurementUnit> MeasurementUnits { get; set; }
    DbSet<OrganizationMeasurementUnit> OrganizationMeasurementUnits { get; set; }

    // Mahsulotlar va ishlab chiqaruvchilar
    DbSet<Product>      Products      { get; set; }
    DbSet<Manufacturer> Manufacturers { get; set; }
    DbSet<Supplier>     Suppliers     { get; set; }
    DbSet<Sku>          Skus          { get; set; }

    // Ombor
    DbSet<Stock>                Stocks                { get; set; }
    DbSet<StockSku>             StockSkus             { get; set; }
    DbSet<StockDemand>          StockDemands          { get; set; }
    DbSet<StockDemandItem>      StockDemandItems      { get; set; }
    DbSet<StockBalanceRegister> StockBalanceRegisters { get; set; }
    DbSet<OrganizationSku>      OrganizationSkus      { get; set; }

    // Kirim va chiqim
    DbSet<Income>        Incomes        { get; set; }
    DbSet<IncomeItem>    IncomeItems    { get; set; }
    DbSet<IncomeOutcome> IncomeOutcomes { get; set; }
    DbSet<Outcome>       Outcomes       { get; set; }
    DbSet<OutcomeItem>   OutcomeItems   { get; set; }

    // Identity
    DbSet<User>         Users         { get; set; }
    DbSet<Role>         Roles         { get; set; }
    DbSet<UserRole>     UserRoles     { get; set; }
    DbSet<RefreshToken> RefreshTokens { get; set; }

    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
    IExecutionStrategy CreateExecutionStrategy();
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    DatabaseFacade Database { get; }
}
