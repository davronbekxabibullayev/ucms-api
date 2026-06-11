namespace Ucms.Infrastructure.Persistence;

using System.Reflection;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Ucms.Application.Abstractions;
using Ucms.Application.Persistence;
using Ucms.Domain.Common;
using Ucms.Domain.Entities;
using Ucms.Domain.Entities.Identity;

public class UcmsDbContext(
    DbContextOptions<UcmsDbContext> options,
    ICurrentContext context,
    IMediator? bus)
    : IdentityDbContext<
        User, Role, Guid,
        UserClaim, UserRole, UserLogin,
        RoleClaim, UserToken>(options),
      IUcmsDbContext
{
    // ── Tashkilot ──────────────────────────────────────────────────────────
    public DbSet<Organization> Organizations { get; set; }

    // ── Loyiha va smeta ────────────────────────────────────────────────────
    public DbSet<Project> Projects { get; set; }
    public DbSet<EstimateSection> EstimateSections { get; set; }
    public DbSet<EstimateItem> EstimateItems { get; set; }

    // ── Brigadalar ─────────────────────────────────────────────────────────
    public DbSet<Brigade> Brigades { get; set; }

    // ── Bajarilgan ishlar ──────────────────────────────────────────────────
    public DbSet<WorkLog> WorkLogs { get; set; }

    // ── Zakazchik akti va to'lovlar ────────────────────────────────────────
    public DbSet<ClientAct> ClientActs { get; set; }
    public DbSet<ClientActItem> ClientActItems { get; set; }
    public DbSet<ClientPayment> ClientPayments { get; set; }

    // ── Brigada to'lovlari ─────────────────────────────────────────────────
    public DbSet<BrigadePayment> BrigadePayments { get; set; }

    // ── Spravochniklar ─────────────────────────────────────────────────────
    public DbSet<MeasurementUnit> MeasurementUnits { get; set; }

    // ── Identity (override) ────────────────────────────────────────────────
    public override DbSet<User> Users { get; set; }
    public override DbSet<Role> Roles { get; set; }
    public override DbSet<UserRole> UserRoles { get; set; }

    // ── Auth ───────────────────────────────────────────────────────────────
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    // ── IUcmsDbContext infra ────────────────────────────────────────────────
    public IExecutionStrategy CreateExecutionStrategy()
    {
        return Database.CreateExecutionStrategy();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default)
    {
        return await Database.BeginTransactionAsync(ct);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ── Identity jadvallari → "Identity" schemasi ──────────────────────
        builder.Entity<User>().ToTable("Users", "Identity");
        builder.Entity<Role>().ToTable("Roles", "Identity");
        builder.Entity<UserRole>().ToTable("UserRoles", "Identity");
        builder.Entity<UserClaim>().ToTable("UserClaims", "Identity");
        builder.Entity<UserLogin>().ToTable("UserLogins", "Identity");
        builder.Entity<RoleClaim>().ToTable("RoleClaims", "Identity");
        builder.Entity<UserToken>().ToTable("UserTokens", "Identity");

        // User ↔ Role navigations
        builder.Entity<User>()
            .HasMany(u => u.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId)
            .IsRequired();

        builder.Entity<Role>()
            .HasMany(r => r.UserRoles)
            .WithOne(ur => ur.Role)
            .HasForeignKey(ur => ur.RoleId)
            .IsRequired();

        // RefreshToken
        builder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId);

        // ── Domain konfiguratsiyalari ───────────────────────────────────────
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // ── Global query filterlar ─────────────────────────────────────────
        ApplyGlobalFilters(builder);
    }

    // ── SaveChanges ────────────────────────────────────────────────────────
    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        ApplyAuditInfo();
        var result = await base.SaveChangesAsync(ct);
        await PublishDomainEventsAsync(ct);
        return result;
    }

    // ── Global filters ─────────────────────────────────────────────────────
    /// <summary>
    /// Barcha entity turlari uchun soft-delete va organization filtrlari
    /// </summary>
    private void ApplyGlobalFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;
            var hasOrg = typeof(IHasOrganization).IsAssignableFrom(clrType);
            var isDeletable = typeof(IDeletable).IsAssignableFrom(clrType);

            if (!isDeletable)
                continue;

            if (hasOrg)
            {
                // Organization + soft-delete filtri
                GetType()
                    .GetMethod(nameof(SetOrgAndDeleteFilter), BindingFlags.NonPublic | BindingFlags.Instance)!
                    .MakeGenericMethod(clrType)
                    .Invoke(this, [modelBuilder]);
            }
            else
            {
                // Faqat soft-delete filtri
                typeof(UcmsDbContext)
                    .GetMethod(nameof(SetSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Static)!
                    .MakeGenericMethod(clrType)
                    .Invoke(null, [modelBuilder]);
            }
        }
    }

    private void SetOrgAndDeleteFilter<T>(ModelBuilder modelBuilder)
        where T : class, IDeletable, IHasOrganization
    {
        modelBuilder.Entity<T>().HasQueryFilter(e =>
                !e.IsDeleted &&
                (context.OrganizationId == null || e.OrganizationId == context.OrganizationId));
    }

    private static void SetSoftDeleteFilter<T>(ModelBuilder modelBuilder)
        where T : class, IDeletable
    {
        modelBuilder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);
    }

    // ── Audit info ─────────────────────────────────────────────────────────
    /// <summary>
    /// Qo'shilgan/o'zgartirilgan entitylarga CreatedAt, UpdatedAt, CreatedBy, UpdatedBy yozadi
    /// </summary>
    private void ApplyAuditInfo()
    {
        var now = DateTimeOffset.UtcNow;
        var userId = context.UserId;
        var orgId = context.OrganizationId;

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedAt = now;
                    if (userId.HasValue)
                    {
                        entry.Entity.CreatedBy = userId.Value;
                        entry.Entity.UpdatedBy = userId.Value;
                    }
                    // IHasOrganization bo'lsa va OrganizationId bo'sh bo'lsa — contextdan to'ldiradi
                    if (orgId.HasValue &&
                        entry.Entity is IHasOrganization hasOrg &&
                        hasOrg.OrganizationId == Guid.Empty)
                    {
                        hasOrg.OrganizationId = orgId.Value;
                    }
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    if (userId.HasValue)
                        entry.Entity.UpdatedBy = userId.Value;
                    break;
            }
        }

        // Identity User audit
        foreach (var entry in ChangeTracker.Entries<User>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedAt = now;
                    if (userId.HasValue)
                    {
                        entry.Entity.CreatedBy = userId.Value;
                        entry.Entity.UpdatedBy = userId.Value;
                    }
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    if (userId.HasValue)
                        entry.Entity.UpdatedBy = userId.Value;
                    break;
            }
        }
    }

    // ── Domain events ──────────────────────────────────────────────────────
    /// <summary>
    /// Barcha entity lardagi domain eventlarni MassTransit orqali publish qiladi
    /// </summary>
    private async Task PublishDomainEventsAsync(CancellationToken ct)
    {
        var entities = ChangeTracker
            .Entries<Entity>()
            .Where(e => e.Entity.DomainEvents.Count != 0)
            .Select(e => e.Entity)
            .ToList();

        var events = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        entities.ForEach(e => e.ClearDomainEvents());

        if (bus is null) return;

        foreach (var domainEvent in events)
            await bus.Publish(domainEvent, domainEvent.GetType(), ct);
    }
}
