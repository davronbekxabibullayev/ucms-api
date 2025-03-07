namespace Ucms.Stock.Domain.Models.Enums;

public enum SkuStatus
{
    Default = 0,

    // For Fire Emergency Service
    FightingMode = 1,
    Reserve = 2,
    Defective = 3,
    Tested = 4
}
