namespace Ucms.Stock.Api.Application.Consumers.Product;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Infrastructure.Persistance;

public record GetProductMessage(Guid Id) : IRequest<ProductModel>;

public class GetProductConsumer : RequestHandler<GetProductMessage, ProductModel>
{
    private readonly IStockDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IWorkContext _workContext;

    public GetProductConsumer(IStockDbContext dbContext, IMapper mapper, IWorkContext workContext)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _workContext = workContext;
    }
    protected override async Task<ProductModel> Handle(GetProductMessage message, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products
           .FirstOrDefaultAsync(f => f.Id == message.Id && f.EmergencyType == _workContext.EmergencyType, cancellationToken)
           ?? throw new NotFoundException($"product with ID: {message.Id}, not found");

        var result = _mapper.Map<ProductModel>(product);

        return result;
    }
}

