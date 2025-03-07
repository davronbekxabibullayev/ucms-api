namespace Ucms.Stock.Api.Application.Consumers.Sku;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Infrastructure.Persistance;
public record GetSkuMessage(Guid Id): IRequest<SkuModel>;

public class GetSkuConsumer : RequestHandler<GetSkuMessage, SkuModel>
{
    private readonly IStockDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetSkuConsumer(IStockDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    protected override async Task<SkuModel> Handle(GetSkuMessage message, CancellationToken cancellationToken)
    {
        var sku = await _dbContext.Skus
           .FirstOrDefaultAsync(f => f.Id == message.Id, cancellationToken)
           ?? throw new NotFoundException($"Sku with ID: {message.Id}, not found!");

        return _mapper.Map<SkuModel>(sku);
    }
}
