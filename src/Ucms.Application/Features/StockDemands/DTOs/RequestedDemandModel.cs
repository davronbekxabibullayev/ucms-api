namespace Ucms.Application.Features.StockDemands;

public record RequestedDemandModel : StockDemandModel
{
    public string StockName { get; set; } = default!;
    public string StockNameRu { get; set; } = default!;
    public string StockNameEn { get; set; } = default!;
    public string StockNameKa { get; set; } = default!;
    public string OrganizationName { get; set; } = default!;
}
