namespace Ucms.Application.Services;

using Ucms.Application.Features.Reports;


public interface IProductBalanceReportService
{
    public Task<MemoryStream> GetExcelAsync(ProductBalanceReportModel data, CancellationToken cancellationToken = default);
}
