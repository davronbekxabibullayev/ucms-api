namespace Ucms.Application.Services;

using Ucms.Application.DTOs.Models;

public interface IProductBalanceReportService
{
    public Task<MemoryStream> GetExcelAsync(ProductBalanceReportModel data, CancellationToken cancellationToken = default);
}
