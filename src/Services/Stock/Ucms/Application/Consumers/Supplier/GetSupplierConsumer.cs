namespace Ucms.Stock.Api.Application.Consumers.Supplier;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Infrastructure.Persistance;

public record GetSupplierMessage(Guid Id) : IRequest<SupplierModel>;

public class GetSupplierConsumer : RequestHandler<GetSupplierMessage, SupplierModel>
{
    private readonly IStockDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetSupplierConsumer(IStockDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    protected override async Task<SupplierModel> Handle(GetSupplierMessage message, CancellationToken cancellationToken)
    {
        var supplier = await _dbContext.Suppliers
           .FirstOrDefaultAsync(f => f.Id == message.Id, cancellationToken)
           ?? throw new NotFoundException($"Supplier with ID: {message.Id}, not found");

        return _mapper.Map<SupplierModel>(supplier);
    }
}

