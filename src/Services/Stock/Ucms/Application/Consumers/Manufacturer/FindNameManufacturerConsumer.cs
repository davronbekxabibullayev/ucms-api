namespace Ucms.Stock.Api.Application.Consumers.Manufacturer;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Infrastructure.Persistance;

public record FindNameManufacturerMessage(string Name) : IRequest<ManufacturerModel>;

public class FindNameManufacturerConsumer : RequestHandler<FindNameManufacturerMessage, ManufacturerModel>
{
    private readonly IStockDbContext _dbContext;
    private readonly IMapper _mapper;

    public FindNameManufacturerConsumer(IStockDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    protected override async Task<ManufacturerModel> Handle(FindNameManufacturerMessage message, CancellationToken cancellationToken)
    {
        var query = message.Name.ToLower();

        var manufacturer = await _dbContext.Manufacturers
                                           .FirstOrDefaultAsync(x => x.Name.ToLower().Contains(query)
                                                               || x.NameEn!.ToLower().Contains(query)
                                                               || x.NameRu.ToLower().Contains(query)
                                                               || x.NameKa!.ToLower().Contains(query)
                                                               , cancellationToken)
                                           ?? throw new NotFoundException($"Manufacturer with name:{message.Name} is not found!");

        var result = _mapper.Map<ManufacturerModel>(manufacturer);

        return result;
    }
}
