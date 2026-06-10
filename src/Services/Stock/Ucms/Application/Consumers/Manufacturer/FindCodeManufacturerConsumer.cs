namespace Ucms.Stock.Api.Application.Consumers.Manufacturer;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Infrastructure.Persistance;

public record FindCodeManufacturerMessage(string Code) : IRequest<ManufacturerModel>;

public class FindCodeManufacturerConsumer : RequestHandler<FindCodeManufacturerMessage, ManufacturerModel>
{
    private readonly IStockDbContext _dbContext;
    private readonly IMapper _mapper;

    public FindCodeManufacturerConsumer(IStockDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    protected override async Task<ManufacturerModel> Handle(FindCodeManufacturerMessage message, CancellationToken cancellationToken)
    {

        var manufacturer = await _dbContext.Manufacturers
                                           .FirstOrDefaultAsync(f => f.Code == message.Code, cancellationToken)
                                           ?? throw new NotFoundException($"Manufacturer with Code: {message.Code} is not found");

        var result = _mapper.Map<ManufacturerModel>(manufacturer);

        return result;
    }
}
