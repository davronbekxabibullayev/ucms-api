namespace Ucms.Application.Handlers.Manufacturer;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Exceptions;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;

public record GetManufacturerMessage(Guid Id) : IRequest<ManufacturerModel>;

public class GetManufacturerConsumer : RequestHandler<GetManufacturerMessage, ManufacturerModel>
{
    private readonly IUcmsDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetManufacturerConsumer(IUcmsDbContext dbContext, IMapper mapper)
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
