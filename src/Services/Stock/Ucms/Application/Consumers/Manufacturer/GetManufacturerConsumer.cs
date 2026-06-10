namespace Ucms.Stock.Api.Application.Consumers.Manufacturer;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Infrastructure.Persistance;

public record GetManufacturerMessage(Guid Id) : IRequest<ManufacturerModel>;

public class GetManufacturerConsumer : RequestHandler<GetManufacturerMessage, ManufacturerModel>
{
    private readonly IStockDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetManufacturerConsumer(IStockDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    protected override async Task<ManufacturerModel> Handle(GetManufacturerMessage message,
        CancellationToken cancellationToken)
    {
        var manufacturer = await _dbContext.Manufacturers
                               .FirstOrDefaultAsync(x => x.Id == message.Id, cancellationToken)
                           ?? throw new NotFoundException($"Manufacturer with Id: {message.Id}, is not found");

        var result = _mapper.Map<ManufacturerModel>(manufacturer);
        return result;
    }
}
