namespace Ucms.Application.DTOs.Requests.OrganizationMeasurementUnits;

using Ucms.Domain.Enums;

public record UpsertOrganizationMeasurementUnitRequest
{
    public MeasurementUnitType Type { get; set; }
    public Guid MeasurementUnitId { get; set; }
}
