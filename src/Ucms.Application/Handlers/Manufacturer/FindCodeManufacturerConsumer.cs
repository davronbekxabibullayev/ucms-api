namespace Ucms.Application.Handlers.Manufacturer;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Exceptions;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;

public record FindCodeManufacturerMessage(string Code) : IRequest<ManufacturerModel>;

public class FindCodeManufacturerConsumer : RequestHandler<FindCodeManufacturerMessage, ManufacturerModel>
{
    private readonly IUcmsDbContext _dbContext;
    private readonly IMapper _mapper;

    public FindCodeManufacturerConsumer(IUcmsDbContext dbContext, IMapper mapper)
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
