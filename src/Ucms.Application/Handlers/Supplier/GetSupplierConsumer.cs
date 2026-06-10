namespace Ucms.Application.Handlers.Supplier;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Exceptions;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;

public record GetSupplierMessage(Guid Id) : IRequest<SupplierModel>;

public class GetSupplierConsumer : RequestHandler<GetSupplierMessage, SupplierModel>
{
    private readonly IAppDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetSupplierConsumer(IAppDbContext dbContext, IMapper mapper)
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

