namespace Ucms.Application.Handlers.Report;

using System.Threading;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Services;
using Ucms.Application.Abstractions.Mediator;

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
        var stream = await _service.GetExcelAsync(message.Data, cancellationToken);

        return stream;
    }
}
