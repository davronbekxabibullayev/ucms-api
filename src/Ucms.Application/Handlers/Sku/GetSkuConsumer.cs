namespace Ucms.Application.Handlers.Sku;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Exceptions;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;

public record GetSkuMessage(Guid Id): IRequest<SkuModel>;

public class GetSkuConsumer : RequestHandler<GetSkuMessage, SkuModel>
{
    private readonly IAppDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetSkuConsumer(IAppDbContext dbContext, IMapper mapper)
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
