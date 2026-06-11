namespace Ucms.Infrastructure.Persistence;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using Polly;
using Polly.Retry;
using Ucms.Domain.Entities;
using Ucms.Domain.Entities.Identity;
using Ucms.Domain.Enums;

/// <summary>
/// Tizimni birinchi ishga tushirishda zaruriy ma'lumotlarni yaratadi.
/// Idempotent: mavjud bo'lsa qayta yaratmaydi.
/// </summary>
public class UcmsDbContextSeed
{
    // ── Fixed GUIDs for repeatable seeding ─────────────────────────────────────
    private static readonly Guid AdminUserId    = new("00000000-0000-0000-0000-000000000001");
    private static readonly Guid AdminRoleId    = new("00000000-0000-0000-0000-000000000010");
    private static readonly Guid ManagerRoleId  = new("00000000-0000-0000-0000-000000000011");
    private static readonly Guid BrigadirRoleId = new("00000000-0000-0000-0000-000000000012");
    private static readonly Guid AccountantRoleId = new("00000000-0000-0000-0000-000000000013");
    private static readonly Guid OrgId          = new("00000000-0000-0000-0001-000000000001");
    private static readonly Guid ProjectId      = new("00000000-0000-0000-0002-000000000001");
    private static readonly Guid BrigadeId      = new("00000000-0000-0000-0003-000000000001");
    private static readonly Guid Section1Id     = new("00000000-0000-0000-0004-000000000001");
    private static readonly Guid Section2Id     = new("00000000-0000-0000-0004-000000000002");

    public async Task SeedAsync(IServiceProvider services)
    {
        var logger = services.GetService<ILogger<UcmsDbContextSeed>>();
        var policy = CreatePolicy(logger!, nameof(UcmsDbContextSeed));

        await policy.ExecuteAsync(async () =>
        {
            var db          = services.GetRequiredService<UcmsDbContext>();
            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<Role>>();

            await SeedRolesAsync(roleManager, logger);
            await SeedMeasurementUnitsAsync(db, logger);
            await SeedOrganizationAsync(db, logger);
            await SeedAdminUserAsync(db, userManager, logger);
            await SeedSampleProjectAsync(db, logger);
            await SeedSampleBrigadeAsync(db, logger);
        });
    }

    // ── Roles ──────────────────────────────────────────────────────────────────

    private static async Task SeedRolesAsync(RoleManager<Role> roleManager, ILogger? logger)
    {
        var roles = new[]
        {
            new Role { Id = AdminRoleId,     Name = "Admin",      NormalizedName = "ADMIN",
                       Description = "Tizim administratori — to'liq huquq" },
            new Role { Id = ManagerRoleId,   Name = "Manager",    NormalizedName = "MANAGER",
                       Description = "Loyiha menejeri — loyiha va smeta boshqaruvi" },
            new Role { Id = BrigadirRoleId,  Name = "Brigadir",   NormalizedName = "BRIGADIR",
                       Description = "Brigada boshlig'i — ish jurnaliga yozish" },
            new Role { Id = AccountantRoleId, Name = "Accountant", NormalizedName = "ACCOUNTANT",
                       Description = "Hisobchi — to'lov va aktlar boshqaruvi" },
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role.Name!))
            {
                var result = await roleManager.CreateAsync(role);
                if (result.Succeeded)
                    logger?.LogInformation("[Seed] Rol yaratildi: {Role}", role.Name);
                else
                    logger?.LogError("[Seed] Rol yaratishda xato ({Role}): {Errors}",
                        role.Name, string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }

    // ── Measurement units ──────────────────────────────────────────────────────

    private static async Task SeedMeasurementUnitsAsync(UcmsDbContext db, ILogger? logger)
    {
        if (await db.MeasurementUnits.AnyAsync())
            return;

        var now = DateTimeOffset.UtcNow;

        var units = new List<MeasurementUnit>
        {
            // Maydon
            new() { Id = NewId(), Code = "M2",   Name = "m²",      NameRu = "м²",    NameEn = "m²",
                    Multiplier = 1, Type = MeasurementUnitType.Volume,   IsDeleted = false },
            // Hajm
            new() { Id = NewId(), Code = "M3",   Name = "m³",      NameRu = "м³",    NameEn = "m³",
                    Multiplier = 1, Type = MeasurementUnitType.Volume,   IsDeleted = false },
            // Uzunlik
            new() { Id = NewId(), Code = "M",    Name = "m",       NameRu = "м",     NameEn = "m",
                    Multiplier = 1, Type = MeasurementUnitType.Length,   IsDeleted = false },
            // Metr pogonn
            new() { Id = NewId(), Code = "MP",   Name = "m.p.",    NameRu = "м.п.",  NameEn = "lm",
                    Multiplier = 1, Type = MeasurementUnitType.Length,   IsDeleted = false },
            // Dona
            new() { Id = NewId(), Code = "DONA", Name = "dona",    NameRu = "шт.",   NameEn = "pcs",
                    Multiplier = 1, Type = MeasurementUnitType.Quantity, IsDeleted = false },
            // Kilogram
            new() { Id = NewId(), Code = "KG",   Name = "kg",      NameRu = "кг",    NameEn = "kg",
                    Multiplier = 1, Type = MeasurementUnitType.Weight,   IsDeleted = false },
            // Tonna
            new() { Id = NewId(), Code = "TON",  Name = "tonna",   NameRu = "тонн",  NameEn = "ton",
                    Multiplier = 1000, Type = MeasurementUnitType.Weight, IsDeleted = false },
        };

        await db.MeasurementUnits.AddRangeAsync(units);
        await db.SaveChangesAsync();
        logger?.LogInformation("[Seed] {Count} ta o'lchov birligi yaratildi", units.Count);
    }

    // ── Organization ───────────────────────────────────────────────────────────

    private static async Task SeedOrganizationAsync(UcmsDbContext db, ILogger? logger)
    {
        if (await db.Organizations.AnyAsync(o => o.Id == OrgId))
            return;

        var now = DateTimeOffset.UtcNow;

        var org = new Organization
        {
            Id         = OrgId,
            Name       = "Demo Qurilish Kompaniyasi",
            TaxId      = "1234567890",
            Address    = "Toshkent shahri, Chilonzor tumani",
            Phone      = "+998712345678",
            Email      = "info@demo-qurilish.uz",
            IsDeleted  = false,
            CreatedAt  = now,
            UpdatedAt  = now,
            CreatedBy  = AdminUserId,
            UpdatedBy  = AdminUserId,
        };

        await db.Organizations.AddAsync(org);
        await db.SaveChangesAsync();
        logger?.LogInformation("[Seed] Tashkilot yaratildi: {Org}", org.Name);
    }

    // ── Admin user ─────────────────────────────────────────────────────────────

    private static async Task SeedAdminUserAsync(
        UcmsDbContext db,
        UserManager<User> userManager,
        ILogger? logger)
    {
        if (await userManager.FindByNameAsync("admin") is not null)
            return;

        var now = DateTimeOffset.UtcNow;

        var admin = new User
        {
            Id             = AdminUserId,
            UserName       = "admin",
            NormalizedUserName = "ADMIN",
            Email          = "admin@ucms.uz",
            NormalizedEmail = "ADMIN@UCMS.UZ",
            EmailConfirmed = true,
            FullName       = "Tizim Administratori",
            OrganizationId = OrgId,
            IsDeleted      = false,
            CreatedAt      = now,
            UpdatedAt      = now,
            CreatedBy      = AdminUserId,
            UpdatedBy      = AdminUserId,
        };

        var result = await userManager.CreateAsync(admin, "Admin123!");
        if (!result.Succeeded)
        {
            logger?.LogError("[Seed] Admin foydalanuvchi yaratishda xato: {Errors}",
                string.Join(", ", result.Errors.Select(e => e.Description)));
            return;
        }

        await userManager.AddToRoleAsync(admin, "Admin");
        logger?.LogInformation("[Seed] Admin foydalanuvchi yaratildi: admin / Admin123!");
    }

    // ── Sample project ─────────────────────────────────────────────────────────

    private static async Task SeedSampleProjectAsync(UcmsDbContext db, ILogger? logger)
    {
        if (await db.Projects.AnyAsync(p => p.Id == ProjectId))
            return;

        var now = DateTimeOffset.UtcNow;

        var project = new Project
        {
            Id             = ProjectId,
            OrganizationId = OrgId,
            Name           = "ИКС — 2-sektsiya ta'mirlash",
            Address        = "Toshkent shahri, Yunusobod tumani, 14-mavze, 3-uy",
            ContractNumber = "2024/001",
            ContractDate   = new DateTimeOffset(2024, 1, 15, 0, 0, 0, TimeSpan.Zero),
            StartDate      = new DateTimeOffset(2024, 2,  1, 0, 0, 0, TimeSpan.Zero),
            EndDate        = new DateTimeOffset(2024, 8, 31, 0, 0, 0, TimeSpan.Zero),
            Status         = ProjectStatus.InProgress,
            IsDeleted      = false,
            CreatedAt      = now,
            UpdatedAt      = now,
            CreatedBy      = AdminUserId,
            UpdatedBy      = AdminUserId,
        };

        // EstimateSections
        var sec1 = new EstimateSection
        {
            Id        = Section1Id,
            ProjectId = ProjectId,
            Name      = "Pol ishlari",
            Order     = 1,
        };

        var sec2 = new EstimateSection
        {
            Id        = Section2Id,
            ProjectId = ProjectId,
            Name      = "Devor ishlari",
            Order     = 2,
        };

        // EstimateItems — pol
        var items1 = new List<EstimateItem>
        {
            new()
            {
                Id               = NewId(),
                SectionId        = Section1Id,
                Name             = "Pol stяjkasi (M-200 beton)",
                Unit             = "m²",
                Volume           = 450m,
                ClientUnitPrice  = 85_000m,
                BrigadeUnitPrice = 55_000m,
                Order            = 1,
            },
            new()
            {
                Id               = NewId(),
                SectionId        = Section1Id,
                Name             = "Keramik plitka qo'yish",
                Unit             = "m²",
                Volume           = 450m,
                ClientUnitPrice  = 120_000m,
                BrigadeUnitPrice = 80_000m,
                Order            = 2,
            },
            new()
            {
                Id               = NewId(),
                SectionId        = Section1Id,
                Name             = "Taxta podval (OSB)",
                Unit             = "m²",
                Volume           = 120m,
                ClientUnitPrice  = 95_000m,
                BrigadeUnitPrice = 65_000m,
                Order            = 3,
            },
        };

        // EstimateItems — devor
        var items2 = new List<EstimateItem>
        {
            new()
            {
                Id               = NewId(),
                SectionId        = Section2Id,
                Name             = "Gips shtukaturka",
                Unit             = "m²",
                Volume           = 1_200m,
                ClientUnitPrice  = 48_000m,
                BrigadeUnitPrice = 30_000m,
                Order            = 1,
            },
            new()
            {
                Id               = NewId(),
                SectionId        = Section2Id,
                Name             = "Bo'yoq (2 qavat)",
                Unit             = "m²",
                Volume           = 1_200m,
                ClientUnitPrice  = 22_000m,
                BrigadeUnitPrice = 14_000m,
                Order            = 2,
            },
            new()
            {
                Id               = NewId(),
                SectionId        = Section2Id,
                Name             = "Gips kartón (GKL) montaj",
                Unit             = "m²",
                Volume           = 320m,
                ClientUnitPrice  = 75_000m,
                BrigadeUnitPrice = 50_000m,
                Order            = 3,
            },
        };

        await db.Projects.AddAsync(project);
        await db.EstimateSections.AddRangeAsync(sec1, sec2);
        await db.EstimateItems.AddRangeAsync(items1);
        await db.EstimateItems.AddRangeAsync(items2);
        await db.SaveChangesAsync();
        logger?.LogInformation("[Seed] Namunaviy loyiha yaratildi: {Name}", project.Name);
    }

    // ── Sample brigade ─────────────────────────────────────────────────────────

    private static async Task SeedSampleBrigadeAsync(UcmsDbContext db, ILogger? logger)
    {
        if (await db.Brigades.AnyAsync(b => b.Id == BrigadeId))
            return;

        var now = DateTimeOffset.UtcNow;

        var brigades = new List<Brigade>
        {
            new()
            {
                Id             = BrigadeId,
                OrganizationId = OrgId,
                Name           = "Abdullayev brigada",
                ForemanName    = "Abdullayev Jamshid Shodiyevich",
                Phone          = "+998901234568",
                IsActive       = true,
                IsDeleted      = false,
                CreatedAt      = now,
                UpdatedAt      = now,
                CreatedBy      = AdminUserId,
                UpdatedBy      = AdminUserId,
            },
            new()
            {
                Id             = NewId(),
                OrganizationId = OrgId,
                Name           = "Karimov brigada",
                ForemanName    = "Karimov Sherzod Aliyevich",
                Phone          = "+998901234569",
                IsActive       = true,
                IsDeleted      = false,
                CreatedAt      = now,
                UpdatedAt      = now,
                CreatedBy      = AdminUserId,
                UpdatedBy      = AdminUserId,
            },
        };

        await db.Brigades.AddRangeAsync(brigades);
        await db.SaveChangesAsync();
        logger?.LogInformation("[Seed] {Count} ta brigada yaratildi", brigades.Count);
    }

    // ── Helpers ────────────────────────────────────────────────────────────────

    private static Guid NewId() => Guid.NewGuid();

    private static AsyncRetryPolicy CreatePolicy(ILogger<UcmsDbContextSeed> logger, string prefix, int retries = 3)
    {
        return Policy.Handle<NpgsqlException>().WaitAndRetryAsync(
            retryCount: retries,
            sleepDurationProvider: _ => TimeSpan.FromSeconds(5),
            onRetry: (exception, _, retry, _) =>
            {
                logger.LogWarning(exception,
                    "[{Prefix}] Urinish {Retry}/{Retries} — {Message}",
                    prefix, retry, retries, exception.Message);
            });
    }
}
