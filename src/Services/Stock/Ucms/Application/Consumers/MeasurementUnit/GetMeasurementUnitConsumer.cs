namespace Ucms.Stock.Api.Application.Consumers.MeasurementUnit;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Infrastructure.Persistance;
public record GetMeasurementUnitMessage(Guid Id) : IRequest<MeasurementUnitModel>;

public class GetMeasurementUnitConsumer : RequestHandler<GetMeasurementUnitMessage, MeasurementUnitModel>
{
    private readonly IStockDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetMeasurementUnitConsumer(IStockDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    protected override async Task<MeasurementUnitModel> Handle(GetMeasurementUnitMessage message, CancellationToken cancellationToken)
    {
        var measurementUnit = await _dbContext.MeasurementUnits
            .FirstOrDefaultAsync(f => f.Id == message.Id, cancellationToken)
            ?? throw new NotFoundException($"Measurement unit with ID: {message.Id}, not found!");

        return _mapper.Map<MeasurementUnitModel>(measurementUnit);
    }
}
