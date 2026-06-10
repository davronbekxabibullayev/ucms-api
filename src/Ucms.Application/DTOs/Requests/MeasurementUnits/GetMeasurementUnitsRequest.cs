namespace Ucms.Application.DTOs.Requests.MeasurementUnits;

using QueryForge.Abstractions;

public class GetMeasurementUnitsRequest : PagedRequest
{
    public string? Query { get; set; }
}
