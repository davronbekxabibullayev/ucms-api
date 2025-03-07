namespace Ucms.Stock.Api.Application.Consumers.Report;

using System.Threading;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Api.Application.Services;

public record GetProductBalanceExcelMessage(ProductBalanceReportModel Data) : IRequest<MemoryStream>;

public class GetProductBalanceExcelConsumer : RequestHandler<GetProductBalanceExcelMessage, MemoryStream>
{
    private readonly IProductBalanceReportService _service;

    public GetProductBalanceExcelConsumer(IProductBalanceReportService service)
    {
        _service = service;
    }

    protected override async Task<MemoryStream> Handle(GetProductBalanceExcelMessage message, CancellationToken cancellationToken)
    {
        var stream = await _service.GetExcelAsync(message.Data);

        return stream;
    }
}
