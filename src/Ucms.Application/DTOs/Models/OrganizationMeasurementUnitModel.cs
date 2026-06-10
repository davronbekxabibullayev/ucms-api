namespace Ucms.Application.DTOs.Models;

using Ucms.Domain.Enums;

public record OrganizationMeasurementUnitModel
{
    public Guid Id { get; set; }
    public MeasurementUnitType Type { get; set; }
    public Guid MeasurementUnitId { get; set; }
    public Guid OrganizationId { get; set; }
    public MeasurementUnitModel? MeasurementUnit { get; set; }
};
