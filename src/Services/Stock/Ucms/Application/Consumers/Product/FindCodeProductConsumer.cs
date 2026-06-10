namespace Ucms.Stock.Api.Application.Consumers.Product;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Infrastructure.Persistance;

public record FindCodeProductMessage(string Code) : IRequest<ProductModel>;

public class FindCodeProductConsumer : RequestHandler<FindCodeProductMessage, ProductModel>
{
    private readonly IStockDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IWorkContext _workContext;

    public FindCodeProductConsumer(IStockDbContext dbContext, IMapper mapper, IWorkContext workContext)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _workContext = workContext;
    }

    protected override async Task<ProductModel> Handle(FindCodeProductMessage message,
        CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products
            .FirstOrDefaultAsync(f => f.Code == message.Code && f.EmergencyType == _workContext.EmergencyType, cancellationToken)
                      ?? throw new NotFoundException($"Product with Code: {message.Code}, not found");

        var result = _mapper.Map<ProductModel>(product);

        return result;
    }
}
