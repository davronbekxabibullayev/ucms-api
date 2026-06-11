namespace Ucms.Application.Handlers.Sku;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Exceptions;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;

public record FindSkuMessage(string SerialNumber) : IRequest<SkuModel>;

public class FindSkuConsumer : RequestHandler<FindSkuMessage, SkuModel>
{
    private readonly IUcmsDbContext _dbContext;
    private readonly IMapper _mapper;

    public FindSkuConsumer(IUcmsDbContext dbContext, IMapper mapper)
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
