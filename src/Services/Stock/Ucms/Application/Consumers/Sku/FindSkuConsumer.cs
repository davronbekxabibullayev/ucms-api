namespace Ucms.Stock.Api.Application.Consumers.Sku;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Infrastructure.Persistance;
public record FindSkuMessage(string SerialNumber) : IRequest<SkuModel>;

public class FindSkuConsumer : RequestHandler<FindSkuMessage, SkuModel>
{
    private readonly IStockDbContext _dbContext;
    private readonly IMapper _mapper;

    public FindSkuConsumer(IStockDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    protected override async Task<SkuModel> Handle(FindSkuMessage message, CancellationToken cancellationToken)
    {
        var sku = await _dbContext.Skus
           .FirstOrDefaultAsync(f => f.SerialNumber == message.SerialNumber, cancellationToken)
           ?? throw new NotFoundException($"Sku with serial number: {message.SerialNumber}, not found!");

        return _mapper.Map<SkuModel>(sku);
    }
}
