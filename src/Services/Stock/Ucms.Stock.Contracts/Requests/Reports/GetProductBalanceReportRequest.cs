namespace Ucms.Stock.Contracts.Requests.Reports;

public record GetProductBalanceReportRequest
{
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public Guid OrganizationId { get; set; }
}
