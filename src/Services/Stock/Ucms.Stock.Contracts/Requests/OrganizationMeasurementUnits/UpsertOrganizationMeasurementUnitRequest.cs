namespace Ucms.Stock.Contracts.Requests.OrganizationMeasurementUnits;

using Ucms.Stock.Domain.Models.Enums;

public record UpsertOrganizationMeasurementUnitRequest
{
    public MeasurementUnitType Type { get; set; }
    public Guid MeasurementUnitId { get; set; }
}
