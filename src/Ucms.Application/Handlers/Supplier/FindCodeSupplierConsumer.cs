namespace Ucms.Application.Handlers.Supplier;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Exceptions;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;

public record FindCodeSupplierMessage(string Code) : IRequest<SupplierModel>;

public class FindCodeSupplierConsumer : RequestHandler<FindCodeSupplierMessage, SupplierModel>
{
    private readonly IAppDbContext _dbContext;
    private readonly IMapper _mapper;

    public FindCodeSupplierConsumer(IAppDbContext dbContext, IMapper mapper)
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
