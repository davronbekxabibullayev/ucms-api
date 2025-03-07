namespace Ucms.Stock.Contracts.Requests.MeasurementUnits;

public class GetMeasurementUnitsRequest : PagingRequest
{
    public string? Query { get; set; }
}
