namespace Ucms.Application.Handlers.Manufacturer;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Exceptions;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;

public record FindNameManufacturerMessage(string Name) : IRequest<ManufacturerModel>;

public class FindNameManufacturerConsumer : RequestHandler<FindNameManufacturerMessage, ManufacturerModel>
{
    private readonly IUcmsDbContext _dbContext;
    private readonly IMapper _mapper;

    public FindNameManufacturerConsumer(IUcmsDbContext dbContext, IMapper mapper)
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
