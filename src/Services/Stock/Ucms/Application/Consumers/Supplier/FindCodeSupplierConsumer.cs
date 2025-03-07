namespace Ucms.Stock.Api.Application.Consumers.Supplier;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Infrastructure.Persistance;

public record FindCodeSupplierMessage(string Code) : IRequest<SupplierModel>;

public class FindCodeSupplierConsumer : RequestHandler<FindCodeSupplierMessage, SupplierModel>
{
    private readonly IStockDbContext _dbContext;
    private readonly IMapper _mapper;

    public FindCodeSupplierConsumer(IStockDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    protected override async Task<SupplierModel> Handle(FindCodeSupplierMessage message,
        CancellationToken cancellationToken)
    {
        var supplier = await _dbContext.Suppliers
            .FirstOrDefaultAsync(f => f.Code == message.Code, cancellationToken)
           ?? throw new NotFoundException($"Supplier with Code: {message.Code}, not found");

        return _mapper.Map<SupplierModel>(supplier);
    }
}
